using UnityEngine.Rendering.PostProcessing;

namespace Taki.Utility
{
    internal interface IPostProcessEffectProvider
    {
        T GetEffect<T>() where T : PostProcessEffectSettings;
    }
}