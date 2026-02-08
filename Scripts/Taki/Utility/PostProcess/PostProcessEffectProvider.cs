using UnityEngine.Rendering.PostProcessing;

namespace Taki.Utility
{
    internal class PostProcessEffectProvider : IPostProcessEffectProvider
    {
        private readonly PostProcessVolume _volume;

        public PostProcessEffectProvider(PostProcessVolume volume)
        {
            _volume = volume;
        }

        public T GetEffect<T>() where T : PostProcessEffectSettings
        {
            T setting = _volume.profile.GetSetting<T>();

            if (setting != null)
            {
                return setting;
            }

            return null;
        }
    }
}