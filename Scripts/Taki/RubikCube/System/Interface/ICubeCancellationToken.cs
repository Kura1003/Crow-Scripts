using System.Threading;

namespace Taki.RubiksCube.System
{
    internal interface ICubeCancellationToken
    {
        CancellationToken GetToken();
        void CancelAndDispose();
    }
}