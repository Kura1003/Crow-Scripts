using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Threading;
using Taki.Audio;
using Taki.RubiksCube.Data;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.RubiksCube.System
{
    public class RubiksCubeFactory : MonoBehaviour, ICubeFactory, ICubeCancellationToken
    {
        [SerializeField] private GameObject _cubePiecePrefab;
        [SerializeField] private Color[] _faceColors;
        [SerializeField] private Transform _parentTransform;

        private const int MAX_CUBE_SIZE = 10;

        private readonly Subject<Unit> _onCubeCreated = new Subject<Unit>();
        private readonly Subject<Unit> _onCubeDestroyed = new Subject<Unit>();

        public Observable<Unit> OnCubeCreated => _onCubeCreated;
        public Observable<Unit> OnCubeDestroyed => _onCubeDestroyed;

        private CancellationTokenSource _cancellationTokenSource;
        private Dictionary<Face, GameObject[,]> _instantiatedFacePieces = new Dictionary<Face, GameObject[,]>();
        private List<GameObject> _instantiatedRotationAxes = new List<GameObject>();
        private int _cachedSize;

        private readonly Stack<GameObject[,]> _pool = new Stack<GameObject[,]>();

        public CancellationToken GetToken() =>
            _cancellationTokenSource != null ? _cancellationTokenSource.Token : CancellationToken.None;

        private void OnDestroy()
        {
            _onCubeCreated.Dispose();
            _onCubeDestroyed.Dispose();
            CancelAndDispose();
        }

        public void CancelAndDispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async UniTask PrewarmPoolAsync(CancellationToken token)
        {
            if (_cubePiecePrefab == null) return;

            int faceCount = FaceUtility.GetFaceCount();
            for (int i = 0; i < faceCount; i++)
            {
                GameObject[,] matrix = CreateNewFaceMatrix();

                for (int row = 0; row < MAX_CUBE_SIZE; row++)
                {
                    for (int col = 0; col < MAX_CUBE_SIZE; col++)
                    {
                        matrix[row, col].SetActive(true);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    if (token.IsCancellationRequested) return;
                }

                for (int row = 0; row < MAX_CUBE_SIZE; row++)
                {
                    for (int col = 0; col < MAX_CUBE_SIZE; col++)
                    {
                        matrix[row, col].SetActive(false);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    if (token.IsCancellationRequested) return;
                }

                _pool.Push(matrix);
            }
        }

        private GameObject[,] CreateNewFaceMatrix()
        {
            GameObject[,] matrix = new GameObject[MAX_CUBE_SIZE, MAX_CUBE_SIZE];
            for (int row = 0; row < MAX_CUBE_SIZE; row++)
            {
                for (int col = 0; col < MAX_CUBE_SIZE; col++)
                {
                    GameObject instance = Instantiate(_cubePiecePrefab, _parentTransform);
                    instance.SetActive(false);
                    matrix[row, col] = instance;
                }
            }
            return matrix;
        }

        private GameObject[,] GetFaceMatrixFromPool()
        {
            return (_pool.Count > 0) ? _pool.Pop() : CreateNewFaceMatrix();
        }

        public async UniTask<CubeGenerationInfo?> GenerateCube(
            float pieceSpacing,
            int cubeSize,
            bool shouldPiecesBeActive,
            bool skipIntermediateYields = false)
        {
            try
            {
                CancelAndDispose();
                _cancellationTokenSource = new CancellationTokenSource();
                CancellationToken token = _cancellationTokenSource.Token;

                _cachedSize = cubeSize;
                Thrower.IfNull(_parentTransform, nameof(_parentTransform));
                Thrower.IfNull(_cubePiecePrefab, nameof(_cubePiecePrefab));

                List<FaceSpawnDataBase> faceSpawnDataList = new List<FaceSpawnDataBase>
                {
                    new FrontFaceSpawnData(), new BackFaceSpawnData(),
                    new LeftFaceSpawnData(), new RightFaceSpawnData(),
                    new TopFaceSpawnData(), new BottomFaceSpawnData()
                };

                Dictionary<Face, FaceManagers> faceManagersMap = new Dictionary<Face, FaceManagers>();
                Dictionary<Face, RotationAxisInfo> axisInfoMap = new Dictionary<Face, RotationAxisInfo>();

                _instantiatedFacePieces.Clear();
                ClearRotationAxes();

                int faceIndex = 0;
                foreach (var config in faceSpawnDataList)
                {
                    Face face = config.Face;
                    FaceSpawnInfo faceSpawnInfo = config.SpawnInfo;

                    Color faceColor = (_faceColors != null && faceIndex < _faceColors.Length)
                        ? _faceColors[faceIndex]
                        : Color.white;

                    faceIndex++;

                    Vector3 faceNormal = _parentTransform.TransformDirection(faceSpawnInfo.Normal);
                    Vector3 faceCenter = _parentTransform.position + faceNormal * (_cachedSize * pieceSpacing / 2f);
                    PieceInfo[,] piecesMatrix = new PieceInfo[_cachedSize, _cachedSize];

                    GameObject[,] facePieces = GetFaceMatrixFromPool();
                    Vector3[,] gridPoints = GridPointCalculator
                        .GenerateGridPoints(Vector3.zero, _cachedSize, _cachedSize, pieceSpacing, faceSpawnInfo.Plane);

                    for (int row = 0; row < _cachedSize; row++)
                    {
                        for (int col = 0; col < _cachedSize; col++)
                        {
                            Vector3 spawnPosition = faceCenter + gridPoints[row, col];
                            Quaternion rotation = 
                                Quaternion.LookRotation(faceNormal) * 
                                Quaternion.Euler(faceSpawnInfo.RotationOffset);

                            GameObject piece = facePieces[row, col];
                            piece.transform.SetPositionAndRotation(spawnPosition, rotation);

                            if (piece.TryGetComponent<SpriteRenderer>(out var renderer))
                            {
                                renderer.color = faceColor;
                            }

                            piece.SetActive(shouldPiecesBeActive);

                            string pieceId = face.ToString().Substring(0, 1) + (row * _cachedSize + col + 1);
                            piecesMatrix[row, col] = new PieceInfo(piece.transform, pieceId, face);
                        }

                        if (!skipIntermediateYields) await UniTask.Yield(token);
                    }

                    _instantiatedFacePieces.Add(face, facePieces);
                    SetupRotationAxes(face, piecesMatrix, faceSpawnInfo, faceManagersMap, axisInfoMap);
                }

                _onCubeCreated.OnNext(Unit.Default);
                return new CubeGenerationInfo(faceManagersMap, axisInfoMap);
            }
            catch (OperationCanceledException) { return default; }
            catch (Exception ex) { Debug.LogError(ex.Message); return default; }
        }

        public async UniTask AnimateSpiralActivation(float intervalSeconds, CancellationToken token)
        {
            IReadOnlyList<(int row, int col)> indices = GridNavigationPattern.GetSpiral(_cachedSize);

            UniTask frontTask = AnimateFaceSpiral(Face.Front, indices, intervalSeconds, token);
            UniTask backTask = AnimateFaceSpiral(Face.Back, indices, intervalSeconds, token);
            UniTask leftTask = AnimateFaceSpiral(Face.Left, indices, intervalSeconds, token);
            UniTask rightTask = AnimateFaceSpiral(Face.Right, indices, intervalSeconds, token);
            UniTask bottomTask = AnimateFaceSpiral(Face.Bottom, indices, intervalSeconds, token);
            UniTask topTask = DelayedAnimateFaceSpiral(Face.Top, indices, intervalSeconds, 0.3f, token);

            await UniTask.WhenAll(frontTask, backTask, leftTask, rightTask, bottomTask, topTask);
        }

        public async UniTask AnimateAllFacesSimultaneously(float intervalSeconds, CancellationToken token)
        {
            IReadOnlyList<(int row, int col)> indices = GridNavigationPattern.GetSpiral(_cachedSize);
            List<UniTask> tasks = new List<UniTask>();

            foreach (Face face in FaceUtility.GetAllFaces())
            {
                tasks.Add(AnimateFaceSpiral(face, indices, intervalSeconds, token));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask DelayedAnimateFaceSpiral(
            Face face, 
            IReadOnlyList<(int row, int col)> indices, 
            float intervalSeconds, 
            float delaySeconds, 
            CancellationToken token)
        {
            await UniTask.WaitForSeconds(delaySeconds, cancellationToken: token);
            await AnimateFaceSpiral(face, indices, intervalSeconds, token);
        }

        private async UniTask AnimateFaceSpiral(
            Face face, 
            IReadOnlyList<(int row, int col)> indices, 
            float intervalSeconds, 
            CancellationToken token)
        {
            if (!_instantiatedFacePieces.TryGetValue(face, out GameObject[,] matrix)) return;

            for (int i = 0; i < indices.Count; i++)
            {
                if (token.IsCancellationRequested) return;
                (int row, int col) = indices[i];

                if (matrix[row, col] != null)
                {
                    matrix[row, col].SetActive(true);
                }

                if (intervalSeconds <= 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                else
                {
                    await UniTask.WaitForSeconds(intervalSeconds, cancellationToken: token);
                }
            }
        }

        private void SetupRotationAxes(
            Face face,
            PieceInfo[,] piecesMatrix,
            FaceSpawnInfo faceSpawnInfo,
            Dictionary<Face, FaceManagers> faceManagersMap,
            Dictionary<Face, RotationAxisInfo> rotationAxisInfoMap)
        {
            Vector3 axisPosition = (piecesMatrix[0, 0].Transform.position
                + piecesMatrix[_cachedSize - 1, _cachedSize - 1].Transform.position) / 2f;

            List<GameObject> axesGameObjects = new List<GameObject>();
            List<Transform> axesTransforms = new List<Transform>();

            Quaternion axisRotation = Quaternion.LookRotation(_parentTransform.position - axisPosition);

            for (int i = 0; i < _cachedSize; i++)
            {
                GameObject axisGameObject = new GameObject($"{face}Axis{i + 1}");
                axisGameObject.transform.SetPositionAndRotation(axisPosition, axisRotation);
                axisGameObject.transform.SetParent(_parentTransform);

                axesGameObjects.Add(axisGameObject);
                axesTransforms.Add(axisGameObject.transform);
            }

            faceManagersMap.Add(face, new FaceManagers(piecesMatrix, _cachedSize));
            rotationAxisInfoMap.Add(face, new RotationAxisInfo(faceSpawnInfo.Normal, axesTransforms));

            _instantiatedRotationAxes.AddRange(axesGameObjects);
        }

        private void ClearRotationAxes()
        {
            foreach (var axis in _instantiatedRotationAxes) if (axis != null) Destroy(axis);
            _instantiatedRotationAxes.Clear();
        }

        public void Destroy()
        {
            CancelAndDispose();
            AudioManager.Instance.Play("Destroy", gameObject).SetVolume(0.5f);

            foreach (var kvp in _instantiatedFacePieces)
            {
                GameObject[,] matrix = kvp.Value;

                for (int row = 0; row < _cachedSize; row++)
                {
                    for (int col = 0; col < _cachedSize; col++)
                    {
                        if (matrix[row, col] != null)
                        {
                            matrix[row, col].SetActive(false);
                        }
                    }
                }

                _pool.Push(matrix);
            }

            _instantiatedFacePieces.Clear();
            ClearRotationAxes();
            _cachedSize = 0;
            _onCubeDestroyed.OnNext(Unit.Default);
        }
    }
}