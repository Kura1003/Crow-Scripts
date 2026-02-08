using System;
using System.Collections.Generic;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal class CubeDataProcessor :
        ICubeDataProvider,
        ICubeRotationLogic,
        ICubeTransformManipulator,
        ICubeStateSaver,
        ICubeStateRestorer
    {
        private const int SIDE_LINE_COUNT = 4;

        private Dictionary<Face, FaceManagers> _faceManagersMap;
        private int _cachedSize;

        private RotationBuffers[] _rotationBuffers;

        private sealed class RotationBuffers
        {
            internal RotationLineInfo[] RotationLineInfo;
            internal Face RotationFace;
            internal RotationLayerInfo RotationLayerInfo;
        }

        public void Setup(
            Dictionary<Face, FaceManagers> faceManagersMap,
            int cubeSize)
        {
            Thrower.IfNull(faceManagersMap, nameof(faceManagersMap));

            _faceManagersMap = faceManagersMap;
            _cachedSize = cubeSize;

            _rotationBuffers = new RotationBuffers[_cachedSize];
        }

        public FaceManagers GetFaceManagers(Face face)
        {
            Thrower.IfTrue(
                !_faceManagersMap.ContainsKey(face),
                $"指定された面 {face} に対応するマネージャが見つかりません。"
            );

            return _faceManagersMap[face];
        }

        public bool ContainsPiece(Face face, string pieceId)
        {
            var faceManagers = GetFaceManagers(face);
            return faceManagers.Validator.ContainsPieceId(pieceId);
        }

        public bool ValidateAllFacesInitialState()
        {
            foreach (var pair in _faceManagersMap)
            {
                Face currentFace = pair.Key;
                FaceManagers managers = pair.Value;

                if (!managers.Validator.AreAllFacesAligned(currentFace))
                {
                    Debug.Log(
                        $"ValidateAllFacesInitialState " +
                        $"失敗: 面 {currentFace} のピースに初期面との不一致が見つかりました。");
                    return false;
                }
            }

            Debug.Log(
                $"ValidateAllFacesInitialState " +
                $"成功: すべての面が初期面と一致しています。");

            return true;
        }

        public void SetRotationBuffers(
            Face face,
            int layerIndex)
        {
            Thrower.IfOutOfRange(layerIndex, 0, _cachedSize - 1);

            var rotationLineInfo = face.GetRotationLineInfos(layerIndex, _cachedSize);
            var rotationLayerInfo = new RotationLayerInfo(layerIndex, _cachedSize);

            Face rotationFace =
                rotationLayerInfo.IsMiddleLayer
                    ? default
                    : face.GetRotationFace(rotationLayerInfo);

            _rotationBuffers[layerIndex] = new RotationBuffers
            {
                RotationLineInfo = rotationLineInfo,
                RotationFace = rotationFace,
                RotationLayerInfo = rotationLayerInfo
            };
        }

        public void ClearRotationBuffers(int layerIndex)
        {
            _rotationBuffers[layerIndex] = null;
        }

        private RotationBuffers GetBuffers(int layerIndex)
        {
            var buffers = _rotationBuffers[layerIndex];
            Thrower.IfNull(buffers, nameof(buffers));
            return buffers;
        }

        private PieceInfo[] GetLinePieces(RotationLineInfo lineInfo)
        {
            return GetFaceManagers(lineInfo.Face)
                .Swapper.GetLinePieces(lineInfo);
        }

        private void SetLinePieces(
            RotationLineInfo lineInfo,
            PieceInfo[] otherPieces)
        {
            GetFaceManagers(lineInfo.Face)
                .Swapper.ReplacePieces(lineInfo, otherPieces);
        }

        private bool GetCorrectRotationDirectionForSide(
            Face face,
            bool initialIsClockwise)
        {
            return initialIsClockwise ^ face.IsContainedIn(FaceCombinations.BRT);
        }

        private bool ShouldReverseSide(int sideIndex, bool isClockwise)
        {
            return isClockwise ^ sideIndex.IsOdd();
        }

        private readonly int[] _clockwiseOrder = { 0, 1, 2, 3 };
        private readonly int[] _counterClockwiseOrder = { 0, 3, 2, 1 };

        private readonly PieceInfo[][] _sidePiecesBuffer =
            new PieceInfo[SIDE_LINE_COUNT][];

        public void RotateSideLines(
            Face face,
            int layerIndex,
            bool isClockwise)
        {
            var buffers = GetBuffers(layerIndex);

            isClockwise =
                GetCorrectRotationDirectionForSide(face, isClockwise);

            for (int i = 0; i < SIDE_LINE_COUNT; i++)
            {
                var pieces =
                    GetLinePieces(buffers.RotationLineInfo[i]);

                if (ShouldReverseSide(i, isClockwise))
                {
                    Array.Reverse(pieces);
                }

                _sidePiecesBuffer[i] = pieces;
            }

            _sidePiecesBuffer.Cycle(
                isClockwise
                    ? _clockwiseOrder
                    : _counterClockwiseOrder);

            for (int i = 0; i < SIDE_LINE_COUNT; i++)
            {
                SetLinePieces(
                    buffers.RotationLineInfo[i],
                    _sidePiecesBuffer[i]);
            }
        }

        private bool GetCorrectRotationDirectionForSurface(
            bool initialIsClockwise,
            RotationBuffers buffers)
        {
            if (buffers.RotationLayerInfo.IsOppositeLayer)
            {
                return initialIsClockwise ^
                       buffers.RotationFace.IsContainedIn(FaceCombinations.BLT);
            }

            return initialIsClockwise ^
                   !buffers.RotationFace.IsContainedIn(FaceCombinations.BLT);
        }

        public void RotateFaceSurface(
            int layerIndex,
            bool isClockwise)
        {
            var buffers = GetBuffers(layerIndex);

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            isClockwise =
                GetCorrectRotationDirectionForSurface(isClockwise, buffers);

            GetFaceManagers(buffers.RotationFace)
                .Swapper.Rotate(isClockwise);
        }

        public void ParentBufferedPiecesTo(
            int layerIndex,
            Transform parent)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .Manipulator.ParentLine(lineInfo, parent);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace)
                .Manipulator.UnparentAll(parent);
        }

        public void RotateFaceSurfacePieces(
            int layerIndex,
            int angle,
            Vector3 localAxis)
        {
            var buffers = GetBuffers(layerIndex);

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            if (buffers.RotationLayerInfo.IsOppositeLayer)
            {
                angle = angle.Negate();
            }

            GetFaceManagers(buffers.RotationFace)
                .Manipulator.RotateAll(angle, localAxis);
        }

        public void RotateSideLinePieces(
            int layerIndex,
            int angle,
            Vector3 worldAxis)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .Manipulator.RotateLine(lineInfo, angle, worldAxis);
            }
        }

        public void SaveAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.SaveAllPositions();
            }
        }

        public void SaveAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.SaveAllRotations();
            }
        }

        public void SaveBufferedPiecePositions(int layerIndex)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .CoordinateRegistry.SavePositions(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace)
                .CoordinateRegistry.SaveAllPositions();
        }

        public void SaveBufferedPieceRotations(int layerIndex)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .CoordinateRegistry.SaveRotations(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace)
                .CoordinateRegistry.SaveAllRotations();
        }

        public void RestoreAllPiecePositions()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.RestoreAllPositions();
            }
        }

        public void RestoreAllPieceRotations()
        {
            foreach (var manager in _faceManagersMap.Values)
            {
                manager.CoordinateRegistry.RestoreAllRotations();
            }
        }

        public void RestoreBufferedPiecePositions(int layerIndex)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .CoordinateRegistry.RestorePositions(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace)
                .CoordinateRegistry.RestoreAllPositions();
        }

        public void RestoreBufferedPieceRotations(int layerIndex)
        {
            var buffers = GetBuffers(layerIndex);

            foreach (var lineInfo in buffers.RotationLineInfo)
            {
                GetFaceManagers(lineInfo.Face)
                    .CoordinateRegistry.RestoreRotations(lineInfo);
            }

            if (buffers.RotationLayerInfo.IsMiddleLayer) return;

            GetFaceManagers(buffers.RotationFace)
                .CoordinateRegistry.RestoreAllRotations();
        }
    }
}
