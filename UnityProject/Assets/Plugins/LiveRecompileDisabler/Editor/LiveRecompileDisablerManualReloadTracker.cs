using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LiveRecompileDisabler.Editor
{
    [InitializeOnLoad]
    [ExecuteInEditMode]
    internal static class LiveRecompileDisablerManualReloadTracker
    {
        private static readonly FieldInfo globalEventHandlerFieldInfo =
            typeof (EditorApplication).GetField("globalEventHandler", BindingFlags.Static | BindingFlags.NonPublic);

        private static bool previousControl;

        public static event Action ReloadRequested;

        static LiveRecompileDisablerManualReloadTracker()
        {
#if LOG_LRD
            LiveRecompileDisablerLogger.LogStaticCtor(typeof (LiveRecompileDisablerManualReloadTracker));
#endif

            if (globalEventHandlerFieldInfo == null)
            {
                LiveRecompileDisablerLogger.LogError("Could not bind globalEventHandler method of EditorApplication");
                return;
            }

            var value = (EditorApplication.CallbackFunction) globalEventHandlerFieldInfo.GetValue(null);
            value += OnEditorApplicationGlobalEvent;
            globalEventHandlerFieldInfo.SetValue(null, value);
        }

        private static void OnEditorApplicationGlobalEvent()
        {
            var e = Event.current;

            if (!e.isKey) { return; }

#if LOG_LRD_KEY_EVENTS
            var logType = e.type.ToString().Replace("Key", string.Empty).ToLower();
            var logModifiers = e.modifiers.ToString().Replace(", ", "+");
            var logKey = e.keyCode.ToString();
            var logHotKeyFormat = (e.control) ? "{1}" : (e.control) ? "{1}+{0}" : "{0}";
            LiveRecompileDisablerLogger.Log(string.Format("Key {0} {1}", logType,
                string.Format(logHotKeyFormat, logKey, logModifiers)));
#endif

            // Most times Control+R will not be triggered because it is Unity editor hot key, but key up events triggered as usual.
            if (!e.control && !previousControl) { return; }
            previousControl = (e.control || e.keyCode == KeyCode.LeftControl);

            if (e.alt) { return; }
            if (e.shift) { return; }
            if (e.command) { return; }
            if (e.keyCode != KeyCode.R) { return; }

#if LOG_LRD
            LiveRecompileDisablerLogger.Log("Manual refresh requested (Control+R pressed)");
#endif

            if (ReloadRequested != null) { ReloadRequested(); }
        }
    }
}