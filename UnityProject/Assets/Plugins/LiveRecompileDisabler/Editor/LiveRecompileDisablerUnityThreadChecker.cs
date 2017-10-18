using System.Threading;
using UnityEditor;
using UnityEngine;

namespace LiveRecompileDisabler.Editor
{
    [InitializeOnLoad]
    [ExecuteInEditMode]
    internal static class LiveRecompileDisablerUnityThreadChecker
    {
        private static readonly Thread unityThread;

        static LiveRecompileDisablerUnityThreadChecker()
        {
            unityThread = Thread.CurrentThread;

#if LOG_LRD
            LiveRecompileDisablerLogger.LogStaticCtor(typeof (LiveRecompileDisablerUnityThreadChecker));
#endif
        }

        public static bool IsUnityThread
        {
            get { return ReferenceEquals(Thread.CurrentThread, unityThread); }
        }
    }
}