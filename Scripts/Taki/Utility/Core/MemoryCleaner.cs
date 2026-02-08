using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taki.Utility.Core
{
    internal class MemoryCleaner
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnApplicationQuit;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CleanMemory();
        }

        private static void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.quitting -= OnApplicationQuit;
            CleanMemory();
        }

        private static void CleanMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Resources.UnloadUnusedAssets();

            GC.Collect();
            GC.WaitForPendingFinalizers();

#if UNITY_STANDALONE_WIN
            TryFreeMemory();
#endif
        }

#if UNITY_STANDALONE_WIN
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        private static void TryFreeMemory()
        {
            Process currentProcess = Process.GetCurrentProcess();
            SetProcessWorkingSetSize(currentProcess.Handle, -1, -1);
        }
#endif
    }
}