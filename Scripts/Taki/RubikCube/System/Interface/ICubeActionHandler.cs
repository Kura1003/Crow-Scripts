using Cysharp.Threading.Tasks;
using System;

namespace Taki.RubiksCube.System
{
    internal interface ICubeActionHandler : IDisposable
    {
        UniTask Execute();
    }
}