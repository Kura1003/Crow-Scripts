using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal readonly struct PieceInfo
    {
        internal Transform Transform { get; }
        internal string Id { get; }
        internal Face InitialFace { get; } 

        internal PieceInfo(Transform transform, string id, Face initialFace)
        {
            Transform = transform;
            Id = id;
            InitialFace = initialFace;
        }
    }
}