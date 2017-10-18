using UnityEditor;
using UnityEngine;

namespace LiveRecompileDisabler.Editor
{
    [InitializeOnLoad]
    [ExecuteInEditMode]
    internal sealed class LiveRecompileDisabler
    {
#pragma warning disable 414
        // ReSharper disable once NotAccessedField.Local
        private static readonly LiveRecompileDisabler instance;
#pragma warning restore 414

        static LiveRecompileDisabler()
        {
#if LOG_LRD
            LiveRecompileDisablerLogger.LogStaticCtor(typeof (LiveRecompileDisabler));
#endif

            instance = new LiveRecompileDisabler();
        }

        LiveRecompileDisabler()
        {
#if LOG_LRD
            LiveRecompileDisablerLogger.LogCtor(typeof (LiveRecompileDisabler));
#endif

            EditorApplication.update += OnEditorUpdate;
            LiveRecompileDisablerManualReloadTracker.ReloadRequested += OnManualReloadRequested;

            HandleAssembliesReloading();
        }

        ~LiveRecompileDisabler()
        {
#if LOG_LRD
            LiveRecompileDisablerLogger.LogDestructor(typeof (LiveRecompileDisabler));
#endif

            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= OnEditorUpdate;
            LiveRecompileDisablerManualReloadTracker.ReloadRequested -= OnManualReloadRequested;

            if (LiveRecompileDisablerUnityThreadChecker.IsUnityThread)
            {
                if (ReloadAssembliesLocked)
                {
                    UnlockReloadAssemblies();
                }
            }
        }

        private static bool DisableTemporaryReloadAssembliesLocking { get; set; }

        private static bool ReloadAssembliesLocked { get; set; }

        public static LiveRecompileDisablerReloadAssembliesInPlayMode ReloadAssembliesInCurrentMode
        {
            get
            {
                return (EditorApplication.isPlayingOrWillChangePlaymode)
                    ? LiveRecompileDisablerEditorPrefs.ReloadAssembliesInPlayMode
                    : (LiveRecompileDisablerReloadAssembliesInPlayMode)
                        LiveRecompileDisablerEditorPrefs.ReloadAssembliesInEditMode;
            }
        }

        private static void HandleAutoRefreshDisabling()
        {
            if (LiveRecompileDisablerEditorPrefs.EnableAutoRefreshAfterStopRequired)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
#if LOG_LRD
                    LiveRecompileDisablerLogger.Log("Enabling Auto Refresh after Play Mode stop");
#endif
                    LiveRecompileDisablerEditorPrefs.AutoRefresh = true;
                    LiveRecompileDisablerEditorPrefs.EnableAutoRefreshAfterStopRequired = false;
                }
            }

            if (LiveRecompileDisablerEditorPrefs.DisableAutoRefreshInPlayMode)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (LiveRecompileDisablerEditorPrefs.AutoRefresh)
                    {
#if LOG_LRD
                        LiveRecompileDisablerLogger.Log("Requesting enabling Auto Refresh after Play Mode stop");
#endif
                        LiveRecompileDisablerEditorPrefs.EnableAutoRefreshAfterStopRequired = true;
                        LiveRecompileDisablerEditorPrefs.AutoRefresh = false;
                    }
                }
            }
        }

        private static void HandleAssembliesReloading()
        {
            if (LiveRecompileDisablerEditorPrefs.ReloadAssembliesAfterStopRequired)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    LiveRecompileDisablerEditorPrefs.ReloadAssembliesAfterStopRequired = false;
                    if (ReloadAssembliesLocked)
                    {
#if LOG_LRD
                        LiveRecompileDisablerLogger.Log("Reloading assemblies after Play Mode stop");
#endif
                        UnlockReloadAssemblies();
                    }
                }
            }

            // Before Playing we should reload assemblies any way.
            if (ReloadAssembliesLocked)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    UnlockReloadAssemblies();
                }
            }

            if (DisableTemporaryReloadAssembliesLocking)
            {
                if (!EditorApplication.isCompiling)
                {
                    DisableTemporaryReloadAssembliesLocking = false;
                }
                else
                {
                    return;
                }
            }

            var reloadAssembliesInCurrentMode = (EditorApplication.isPlayingOrWillChangePlaymode)
                ? LiveRecompileDisablerEditorPrefs.ReloadAssembliesInPlayMode
                : (LiveRecompileDisablerReloadAssembliesInPlayMode)
                    LiveRecompileDisablerEditorPrefs.ReloadAssembliesInEditMode;

            if (reloadAssembliesInCurrentMode == LiveRecompileDisablerReloadAssembliesInPlayMode.Automatical)
            {
                if (ReloadAssembliesLocked)
                {
                    UnlockReloadAssemblies();
                }
            }
            else
            {
                if (EditorApplication.isCompiling && LiveRecompileDisablerEditorPrefs.AutoRefresh &&
                    !ReloadAssembliesLocked)
                {
                    LockReloadAssemblies();

                    // Reloading assemblies after Play Mode stop.
                    if (reloadAssembliesInCurrentMode ==
                        LiveRecompileDisablerReloadAssembliesInPlayMode.AutomaticalAfterStop)
                    {
                        if (EditorApplication.isPlayingOrWillChangePlaymode)
                        {
#if LOG_LRD
                            LiveRecompileDisablerLogger.Log("Requesting reloading assemblies after Play Mode stop");
#endif
                            LiveRecompileDisablerEditorPrefs.ReloadAssembliesAfterStopRequired = true;
                        }
                    }
                }
            }
        }

        private static void HandlePlayModeChanging()
        {
            if (LiveRecompileDisablerEditorPrefs.RestartAfterReloadingAssembliesRequired)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    if (!EditorApplication.isCompiling)
                    {
#if LOG_LRD
                        LiveRecompileDisablerLogger.Log("Restarting after reloading assemblies");
#endif
                        LiveRecompileDisablerEditorPrefs.RestartAfterReloadingAssembliesRequired = false;
                        EditorApplication.isPlaying = true;
                    }
                }
            }

            if (LiveRecompileDisablerEditorPrefs.ChangePlayMode)
            {
                if (EditorApplication.isPlaying)
                {
                    if (EditorApplication.isCompiling)
                    {
                        if (!ReloadAssembliesLocked)
                        {
                            if (LiveRecompileDisablerEditorPrefs.PlayMode == LiveRecompileDisablerPlayMode.Pause)
                            {
#if LOG_LRD
                                LiveRecompileDisablerLogger.Log("Pausing Play Mode on compilation");
#endif
                                EditorApplication.isPaused = true;
                            }
                            else if ((LiveRecompileDisablerEditorPrefs.PlayMode & LiveRecompileDisablerPlayMode.Stop) ==
                                     LiveRecompileDisablerPlayMode.Stop)
                            {
#if LOG_LRD
                                LiveRecompileDisablerLogger.Log("Stoping Play Mode on compilation");
#endif
                                EditorApplication.isPlaying = false;
                                EditorApplication.isPaused = false;
                                LiveRecompileDisablerEditorPrefs.RestartAfterReloadingAssembliesRequired =
                                    (LiveRecompileDisablerEditorPrefs.PlayMode ==
                                     LiveRecompileDisablerPlayMode.Restart);
                                if (LiveRecompileDisablerEditorPrefs.RestartAfterReloadingAssembliesRequired)
                                {
#if LOG_LRD
                                    LiveRecompileDisablerLogger.Log("Restarting after reloading assemblies requested");
#endif
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void LockReloadAssemblies()
        {
            if (ReloadAssembliesLocked) { return; }
            ReloadAssembliesLocked = true;

#if LOG_LRD
            LiveRecompileDisablerLogger.Log("Reloading assemblies locked");
#endif

            EditorApplication.LockReloadAssemblies();
        }

        private static void OnEditorUpdate()
        {
            HandleAutoRefreshDisabling();
            HandleAssembliesReloading();
            HandlePlayModeChanging();
        }

        private static void OnManualReloadRequested()
        {
            if (ReloadAssembliesLocked)
            {
                UnlockReloadAssemblies();
                DisableTemporaryReloadAssembliesLocking = true;
            }
        }

        private static void UnlockReloadAssemblies()
        {
            if (!ReloadAssembliesLocked) { return; }
            ReloadAssembliesLocked = false;

#if LOG_LRD
            LiveRecompileDisablerLogger.Log("Reloading assemblies unlocked");
#endif

            EditorApplication.UnlockReloadAssemblies();

            if (EditorApplication.isCompiling)
            {
                DisableTemporaryReloadAssembliesLocking = true;
            }
        }
    }
}