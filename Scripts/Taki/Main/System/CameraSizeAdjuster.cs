using R3;
using UnityEngine;
using Taki.RubiksCube.System;
using VContainer;
using Taki.Main.View;

namespace Taki.Main.System
{
    [RequireComponent(typeof(Camera))]
    public class CameraSizeAdjuster : MonoBehaviour
    {
        [SerializeField] private CameraShaker _cameraShaker;

        [Inject] private readonly ICubeSizeManager _cubeSizeManager;
        [Inject] private readonly ICubeFactory _cubeFactory;

        [SerializeField] private float _baseCameraYPosition = 80.0f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Start()
        {
            _cubeFactory.OnCubeCreated
                .Subscribe(_ =>
                {
                    AdjustCameraYPosition();
                    _cameraShaker.SetOriginalPosition();
                    _cameraShaker.ShakeCamera();

                }).AddTo(this);
        }

        private void AdjustCameraYPosition()
        {
            float factor = _cubeSizeManager.GetSizeScaleFactor();
            float newYPosition = _baseCameraYPosition * factor;

            Vector3 newLocalPosition = _mainCamera.transform.localPosition;
            newLocalPosition.y = newYPosition;
            _mainCamera.transform.localPosition = newLocalPosition;

            Debug.Log(
                $"カメラのY座標を調整しました。" +
                $"ベース座標: {_baseCameraYPosition}, " +
                $"ファクター: {factor:F2}, " +
                $"新しいY座標: {newYPosition:F2}");
        }
    }
}