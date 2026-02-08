using UnityEngine;

namespace Taki.Utility
{
    public class ContinuousRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeed = 30f;
        [SerializeField] private bool _ignoreTimeScale = false;

        [SerializeField] private bool _applyInitialRandomRotation = false;

        private bool _isRotationEnabled = true;

        private void Awake()
        {
            if (!_isRotationEnabled)
            {
                return;
            }

            if (_applyInitialRandomRotation)
            {
                float randomRotationAngle = (float)RandomUtility.Range(0.0, 360.0);
                transform.Rotate(_rotationAxis, randomRotationAngle, Space.Self);
            }
        }

        private void Update()
        {
            if (!_isRotationEnabled)
            {
                return;
            }

            float deltaTime = _ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.Rotate(_rotationAxis, _rotationSpeed * deltaTime, Space.Self);
        }

        public void SetRotationEnabled(bool isEnabled)
        {
            _isRotationEnabled = isEnabled;
        }
    }
}