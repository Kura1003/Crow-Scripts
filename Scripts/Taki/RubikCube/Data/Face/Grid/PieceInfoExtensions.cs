using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal static class PieceInfoExtensions
    {
        internal static void RotateLocal(
            this PieceInfo pieceInfo,
            float angle,
            Vector3 localAxis)
        {
            pieceInfo.Transform.Rotate(localAxis, angle, Space.Self);
        }

        internal static void Rotate(
            this PieceInfo pieceInfo,
            float angle,
            Vector3 worldAxis)
        {
            pieceInfo.Transform.Rotate(worldAxis, angle, Space.World);
        }

        internal static void SwapLocalPosition(
            this PieceInfo pieceInfo,
            Transform otherTransform)
        {
            var pieceTransform = pieceInfo.Transform;
            (pieceTransform.localPosition, otherTransform.localPosition) =
                (otherTransform.localPosition, pieceTransform.localPosition);
        }

        internal static void ReplaceInfo(
            this ref PieceInfo pieceInfo,
            PieceInfo otherPieceInfo)
        {
            pieceInfo = otherPieceInfo;
        }

        internal static void Parent(
            this PieceInfo pieceInfo,
            Transform parent)
        {
            pieceInfo.Transform.SetParent(parent);
        }

        internal static bool IsSameAs(
            this PieceInfo pieceInfo,
            string otherId)
        {
            return pieceInfo.Id.Equals(otherId);
        }
    }
}