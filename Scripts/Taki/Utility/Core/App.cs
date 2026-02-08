using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Taki.Utility.Core
{
    internal readonly struct AppSettings
    {
        internal int TargetFrameRate { get; }
        internal bool RunInBackground { get; }
        internal bool LogEnabled { get; }

        internal AppSettings(
            int targetFrameRate,
            bool runInBackground,
            bool logEnabled)
        {
            TargetFrameRate = targetFrameRate;
            RunInBackground = runInBackground;
            LogEnabled = logEnabled;
        }
    }

    internal static class App
    {
        private static readonly AppSettings settings = new(
            targetFrameRate: 120,
            runInBackground: true,
            logEnabled: false
        );

        internal static void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL
            Application.Quit();
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (!Application.isEditor)
            {
                Debug.unityLogger.logEnabled = settings.LogEnabled;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = settings.TargetFrameRate;
            Application.runInBackground = settings.RunInBackground;
        }
    }
}