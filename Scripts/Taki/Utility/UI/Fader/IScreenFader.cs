using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Taki.Utility
{
    internal interface IScreenFader
    {
        void Initialize(Texture2D texture, Rect positionRect);
        void SetTextureColor(Color color);
        void SetFadeDuration(float duration);
        void SetFadeInTargetAlpha(float alpha);
        UniTask FadeIn(
            CancellationToken token, 
            bool keepDrawingAfterFade = false);
        UniTask FadeOut(CancellationToken token);
        void SetAlphaToZero();
    }
}