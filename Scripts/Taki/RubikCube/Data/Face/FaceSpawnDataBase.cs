using Taki.Utility;
using UnityEngine;

namespace Taki.RubiksCube.Data
{
    internal abstract class FaceSpawnDataBase
    {
        public Face Face { get; protected set; }
        public FaceSpawnInfo SpawnInfo { get; protected set; }
    }

    internal sealed class FrontFaceSpawnData : FaceSpawnDataBase
    {
        internal FrontFaceSpawnData()
        {
            Face = Face.Front;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.XY,
                Vector3.forward,
                new Vector3(0, 180, 180));
        }
    }

    internal sealed class BackFaceSpawnData : FaceSpawnDataBase
    {
        internal BackFaceSpawnData()
        {
            Face = Face.Back;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.XY,
                Vector3.back,
                new Vector3(0, 180, 0));
        }
    }

    internal sealed class LeftFaceSpawnData : FaceSpawnDataBase
    {
        internal LeftFaceSpawnData()
        {
            Face = Face.Left;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.YZ,
                Vector3.left,
                new Vector3(0, 180, 180));
        }
    }

    internal sealed class RightFaceSpawnData : FaceSpawnDataBase
    {
        internal RightFaceSpawnData()
        {
            Face = Face.Right;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.YZ,
                Vector3.right,
                new Vector3(0, 180, 0));
        }
    }

    internal sealed class TopFaceSpawnData : FaceSpawnDataBase
    {
        internal TopFaceSpawnData()
        {
            Face = Face.Top;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.XZ,
                Vector3.up,
                new Vector3(0, 180, 180));
        }
    }

    internal sealed class BottomFaceSpawnData : FaceSpawnDataBase
    {
        internal BottomFaceSpawnData()
        {
            Face = Face.Bottom;
            SpawnInfo = new FaceSpawnInfo(
                GridPlane.XZ,
                Vector3.down,
                new Vector3(0, 180, 180));
        }
    }
}