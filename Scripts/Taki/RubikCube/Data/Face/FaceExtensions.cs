using System;
using System.Collections.Generic;
using Taki.Utility.Core;

namespace Taki.RubiksCube.Data
{
    internal static class FaceExtensions
    {
        private static readonly Dictionary<Face, SideRotationLineInfo[]> _sideLineInfoMap;

        static FaceExtensions()
        {
            var frontSideLines = new[]
            {
                new SideRotationLineInfo(Face.Top, Line.Row, true),
                new SideRotationLineInfo(Face.Left, Line.Row, true),
                new SideRotationLineInfo(Face.Bottom, Line.Row, true),
                new SideRotationLineInfo(Face.Right, Line.Row, true)
            };

            var backSideLines = new[]
            {
                new SideRotationLineInfo(Face.Top, Line.Row, false),
                new SideRotationLineInfo(Face.Left, Line.Row, false),
                new SideRotationLineInfo(Face.Bottom, Line.Row, false),
                new SideRotationLineInfo(Face.Right, Line.Row, false)
            };

            var leftSideLines = new[]
            {
                new SideRotationLineInfo(Face.Top, Line.Column, false),
                new SideRotationLineInfo(Face.Back, Line.Column, false),
                new SideRotationLineInfo(Face.Bottom, Line.Column, false),
                new SideRotationLineInfo(Face.Front, Line.Column, false)
            };

            var rightSideLines = new[]
            {
                new SideRotationLineInfo(Face.Top, Line.Column, true),
                new SideRotationLineInfo(Face.Back, Line.Column, true),
                new SideRotationLineInfo(Face.Bottom, Line.Column, true),
                new SideRotationLineInfo(Face.Front, Line.Column, true)
            };

            var topSideLines = new[]
            {
                new SideRotationLineInfo(Face.Front, Line.Row, true),
                new SideRotationLineInfo(Face.Left, Line.Column, true),
                new SideRotationLineInfo(Face.Back, Line.Row, true),
                new SideRotationLineInfo(Face.Right, Line.Column, true)
            };

            var bottomSideLines = new[]
            {
                new SideRotationLineInfo(Face.Front, Line.Row, false),
                new SideRotationLineInfo(Face.Left, Line.Column, false),
                new SideRotationLineInfo(Face.Back, Line.Row, false),
                new SideRotationLineInfo(Face.Right, Line.Column, false)
            };

            _sideLineInfoMap = new Dictionary<Face, SideRotationLineInfo[]>
            {
                { Face.Front, frontSideLines },
                { Face.Back, backSideLines },
                { Face.Left, leftSideLines },
                { Face.Right, rightSideLines },
                { Face.Top, topSideLines },
                { Face.Bottom, bottomSideLines }
            };
        }

        internal static bool IsContainedIn(this Face self, Face other)
        {
            Thrower.IfTrue(
                !IsSingleFlag(self),
                $"IsContainedInメソッドは単一のFaceフラグにのみ使用できます。");

            return other.HasFlag(self);
        }

        internal static Face Opposite(this Face self)
        {
            Thrower.IfTrue(
                !IsSingleFlag(self),
                $"Oppositeメソッドは単一のFaceフラグにのみ使用できます。");

            return self switch
            {
                Face.Front => Face.Back,
                Face.Back => Face.Front,
                Face.Left => Face.Right,
                Face.Right => Face.Left,
                Face.Top => Face.Bottom,
                Face.Bottom => Face.Top,
                _ => throw new ArgumentException(
                    $"予期しないFaceフラグです: {self}",
                    nameof(self))
            };
        }

        internal static Face GetRotationFace(this Face self, RotationLayerInfo layerInfo)
        {
            Thrower.IfTrue(
                layerInfo.IsMiddleLayer,
                $"中央の段を回す場合、回転面は定義されません。");

            return layerInfo.IsFrontLayer ? self : self.Opposite();
        }

        internal static RotationLineInfo[] GetRotationLineInfos(
            this Face self,
            int layerIndex,
            int size)
        {
            var sideLineInfos = GetSideRotationLineInfos(self);
            var lineInfos = new RotationLineInfo[sideLineInfos.Length];

            for (int i = 0; i < lineInfos.Length; i++)
            {
                lineInfos[i] = sideLineInfos[i].GetLineInfo(layerIndex, size);
            }

            return lineInfos;
        }

        private static SideRotationLineInfo[] GetSideRotationLineInfos(Face self)
        {
            Thrower.IfTrue(
                !IsSingleFlag(self),
                $"GetSideRotationLineInfosメソッドは単一のFaceフラグにのみ使用できます。");

            return _sideLineInfoMap[self];
        }

        internal static bool IsSingleFlag(Face face)
            => face != 0 && (face & (face - 1)) == 0;
    }
}