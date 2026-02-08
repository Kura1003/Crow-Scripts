using System;

namespace Taki.RubiksCube.System
{
    internal interface ICubeSizeManager : IDisposable
    {
        float GetSizeScaleFactor();

        void SetCubeSize(int cubeSize);
    }
}