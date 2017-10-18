using System;
using UnityEditor;
using UnityEngine;

namespace LiveRecompileDisabler.Editor
{
    internal static class LiveRecompileDisablerLogger
    {
        private static readonly string LOG_FORMAT = string.Format("[LiveRecompileDisabler][{0:X}]",
            DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond) + "[{0}][{1}] {2}";

        private static string Compiling
        {
            get
            {
                if (!LiveRecompileDisablerUnityThreadChecker.IsUnityThread) { return "Undefined"; }
                if (!EditorApplication.isCompiling) { return "Compiling"; }
                return "Ready";
            }
        }

        private static string Playmode
        {
            get
            {
                if (!LiveRecompileDisablerUnityThreadChecker.IsUnityThread) { return "Undefined"; }
                if (!EditorApplication.isPlayingOrWillChangePlaymode) { return "Editor"; }
                if (!EditorApplication.isPlaying) { return "Change"; }
                return "Play";
            }
        }

#if LOG_LRD

        public static void Log(object message)
        {
            Debug.Log(string.Format(LOG_FORMAT, Playmode, Compiling, message));
        }

        public static void LogCtor(Type type)
        {
            Log(string.Format("Ctor {0}", type.Name));
        }

        public static void LogDestructor(Type type)
        {
            Log(string.Format("Destructor {0}", type.Name));
        }

#endif

        public static void LogError(object message)
        {
            Debug.LogError(string.Format(LOG_FORMAT, Playmode, Compiling, message));
        }

#if LOG_LRD

        public static void LogStaticCtor(Type type)
        {
            Log(string.Format("Static Ctor {0}", type.Name));
        }

#endif
    }
}