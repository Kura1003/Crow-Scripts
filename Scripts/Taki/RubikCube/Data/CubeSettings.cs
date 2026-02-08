using DG.Tweening;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    [CreateAssetMenu(
        fileName = "CubeSettings",
        menuName = "ScriptableObjects/CubeSettings",
        order = 1)]
    public class CubeSettings :
        ScriptableObject
    {
        [Header("Cube Configuration")]
        [Range(3, 10)]
        [SerializeField]
        private int _cubeSize = 3;

        public int CubeSize
        {
            get => _cubeSize;
            set => _cubeSize = value;
        }

        [SerializeField]
        private float _pieceSpacing = 0.01f;

        public float PieceSpacing => _pieceSpacing;

        public readonly int RotationAngle = 90;

        [SerializeField]
        private float _rotationDuration = 0.2f;

        public float RotationDuration
        {
            get => _rotationDuration;
            set => _rotationDuration = value;
        }

        [SerializeField]
        private Ease _easingType = Ease.InOutSine;

        public Ease EasingType => _easingType;

        [Header("Move Limit")]
        [SerializeField]
        private int _rotationLimit = 50;

        public int RotationLimit => _rotationLimit;

        public bool IsLimitExceeded(int rotationCount)
            => rotationCount >= _rotationLimit;
    }
}