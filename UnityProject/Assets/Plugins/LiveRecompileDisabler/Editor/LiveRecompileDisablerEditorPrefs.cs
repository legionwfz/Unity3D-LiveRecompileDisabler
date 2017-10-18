using System;
using UnityEditor;

namespace LiveRecompileDisabler.Editor
{
    internal enum LiveRecompileDisablerReloadAssembliesInEditMode
    {
        Manual = 0,
        Automatical = 1,
    }

    internal enum LiveRecompileDisablerReloadAssembliesInPlayMode
    {
        Manual = LiveRecompileDisablerReloadAssembliesInEditMode.Manual,
        Automatical = LiveRecompileDisablerReloadAssembliesInEditMode.Automatical,
        AutomaticalAfterStop = 2,
    }

    [Flags]
    internal enum LiveRecompileDisablerPlayMode
    {
        Pause = (1 << 0),
        Stop = (1 << 1),
        Restart = (1 << 2) | Stop,
    }

    internal static class LiveRecompileDisablerEditorPrefs
    {
        private const LiveRecompileDisablerReloadAssembliesInEditMode RELOAD_ASSEMBLIES_IN_EDIT_MODE_PREFS_DEFAULT =
            LiveRecompileDisablerReloadAssembliesInEditMode.Automatical;

        private const LiveRecompileDisablerReloadAssembliesInPlayMode RELOAD_ASSEMBLIES_IN_PLAY_MODE_PREFS_DEFAULT =
            LiveRecompileDisablerReloadAssembliesInPlayMode.Automatical;

        private const LiveRecompileDisablerPlayMode PLAY_MODE_PREFS_DEFAULT = LiveRecompileDisablerPlayMode.Stop;

        private const string AUTO_REFRESH_PREFS_KEY = "kAutoRefresh";

        private const string CHANGE_PLAY_MODE_PREFS_KEY = "live_recompile_disabler.change_play_mode";

        private const string DISABLE_AUTO_REFRESH_IN_PLAY_MODE_PREFS_KEY =
            "live_recompile_disabler.disable_auto_refresh_in_play_mode";

        private const string ENABLE_AUTO_REFRESH_AFTER_STOP_REQUIRED_PREFS_KEY =
            "live_recompile_disabler.enable_auto_refresh_after_stop_required";

        private const string PLAY_MODE_PREFS_KEY = "live_recompile_disabler.play_mode";

        private const string RELOAD_ASSEMBLIES_IN_EDIT_MODE_PREFS_KEY =
            "live_recompile_disabler.reload_assemblies_in_edit_mode";

        private const string RELOAD_ASSEMBLIES_IN_PLAY_MODE_PREFS_KEY =
            "live_recompile_disabler.reload_assemblies_in_play_mode";

        private const string RESTART_AFTER_RELOADING_ASSEMBLIES_REQUIRED_PREFS_KEY =
            "live_recompile_disabler.restart_after_compilation_Required";

        private const string RELOAD_ASSEMBLIES_AFTER_STOP_REQUIRED_PREFS_KEY =
            "live_recompile_disabler.reload_assemblies_after_stop_required";

        private static bool? changePlayMode;
        private static bool? disableAutoRefreshInPlayMode;
        private static bool? enableAutoRefreshAfterStopRequired;
        private static LiveRecompileDisablerPlayMode? playMode;
        private static LiveRecompileDisablerReloadAssembliesInEditMode? reloadAssembliesInEditMode;
        private static LiveRecompileDisablerReloadAssembliesInPlayMode? reloadAssembliesInPlayMode;
        private static bool? restartAfterCompilationRequired;
        private static bool? reloadAssembliesAfterStopRequired;

        public static bool AutoRefresh
        {
            get { return EditorPrefs.GetBool(AUTO_REFRESH_PREFS_KEY); }
            set
            {
                if (AutoRefresh != value)
                {
                    EditorPrefs.SetBool(AUTO_REFRESH_PREFS_KEY, value);
                }
            }
        }

        public static bool ChangePlayMode
        {
            get
            {
                return
                    (changePlayMode ??
                     (changePlayMode = EditorPrefs.GetBool(CHANGE_PLAY_MODE_PREFS_KEY)))
                        .Value;
            }
            set
            {
                if (changePlayMode != value)
                {
                    EditorPrefs.SetBool(CHANGE_PLAY_MODE_PREFS_KEY, (bool) (changePlayMode = value));
                }
            }
        }

        public static bool DisableAutoRefreshInPlayMode
        {
            get
            {
                return
                    (disableAutoRefreshInPlayMode ??
                     (disableAutoRefreshInPlayMode = EditorPrefs.GetBool(DISABLE_AUTO_REFRESH_IN_PLAY_MODE_PREFS_KEY)))
                        .Value;
            }
            set
            {
                if (disableAutoRefreshInPlayMode != value)
                {
                    EditorPrefs.SetBool(DISABLE_AUTO_REFRESH_IN_PLAY_MODE_PREFS_KEY,
                        (bool) (disableAutoRefreshInPlayMode = value));
                }
            }
        }


        public static bool EnableAutoRefreshAfterStopRequired
        {
            get
            {
                return
                    (enableAutoRefreshAfterStopRequired ??
                     (enableAutoRefreshAfterStopRequired =
                         EditorPrefs.GetBool(ENABLE_AUTO_REFRESH_AFTER_STOP_REQUIRED_PREFS_KEY)))
                        .Value;
            }
            set
            {
                if (enableAutoRefreshAfterStopRequired != value)
                {
                    EditorPrefs.SetBool(ENABLE_AUTO_REFRESH_AFTER_STOP_REQUIRED_PREFS_KEY,
                        (bool) (enableAutoRefreshAfterStopRequired = value));
                }
            }
        }

        public static LiveRecompileDisablerPlayMode PlayMode
        {
            get
            {
                return
                    (playMode ??
                     (playMode = (LiveRecompileDisablerPlayMode) EditorPrefs.GetInt(PLAY_MODE_PREFS_KEY, (int) PLAY_MODE_PREFS_DEFAULT)))
                        .Value;
            }
            set
            {
                if (playMode != value)
                {
                    EditorPrefs.SetInt(PLAY_MODE_PREFS_KEY, (int) (playMode = value));
                }
            }
        }

        public static LiveRecompileDisablerReloadAssembliesInEditMode ReloadAssembliesInEditMode
        {
            get
            {
                return
                    (reloadAssembliesInEditMode ??
                     (reloadAssembliesInEditMode =
                         (LiveRecompileDisablerReloadAssembliesInEditMode)
                             EditorPrefs.GetInt(RELOAD_ASSEMBLIES_IN_EDIT_MODE_PREFS_KEY,
                                 (int) RELOAD_ASSEMBLIES_IN_EDIT_MODE_PREFS_DEFAULT))).Value;
            }
            set
            {
                if (reloadAssembliesInEditMode != value)
                {
                    EditorPrefs.SetInt(RELOAD_ASSEMBLIES_IN_EDIT_MODE_PREFS_KEY,
                        (int) (reloadAssembliesInEditMode = value));
                }
            }
        }

        public static LiveRecompileDisablerReloadAssembliesInPlayMode ReloadAssembliesInPlayMode
        {
            get
            {
                return
                    (reloadAssembliesInPlayMode ??
                     (reloadAssembliesInPlayMode =
                         (LiveRecompileDisablerReloadAssembliesInPlayMode)
                             EditorPrefs.GetInt(RELOAD_ASSEMBLIES_IN_PLAY_MODE_PREFS_KEY,
                                 (int) RELOAD_ASSEMBLIES_IN_PLAY_MODE_PREFS_DEFAULT))).Value;
            }
            set
            {
                if (reloadAssembliesInPlayMode != value)
                {
                    EditorPrefs.SetInt(RELOAD_ASSEMBLIES_IN_PLAY_MODE_PREFS_KEY,
                        (int) (reloadAssembliesInPlayMode = value));
                }
            }
        }

        public static bool RestartAfterReloadingAssembliesRequired
        {
            get
            {
                return
                    (restartAfterCompilationRequired ??
                     (restartAfterCompilationRequired =
                         EditorPrefs.GetBool(RESTART_AFTER_RELOADING_ASSEMBLIES_REQUIRED_PREFS_KEY)))
                        .Value;
            }
            set
            {
                if (restartAfterCompilationRequired != value)
                {
                    EditorPrefs.SetBool(RESTART_AFTER_RELOADING_ASSEMBLIES_REQUIRED_PREFS_KEY,
                        (bool) (restartAfterCompilationRequired = value));
                }
            }
        }

        public static bool ReloadAssembliesAfterStopRequired
        {
            get
            {
                return
                    (reloadAssembliesAfterStopRequired ??
                     (reloadAssembliesAfterStopRequired =
                         EditorPrefs.GetBool(RELOAD_ASSEMBLIES_AFTER_STOP_REQUIRED_PREFS_KEY)))
                        .Value;
            }
            set
            {
                if (reloadAssembliesAfterStopRequired != value)
                {
                    EditorPrefs.SetBool(RELOAD_ASSEMBLIES_AFTER_STOP_REQUIRED_PREFS_KEY,
                        (bool) (reloadAssembliesAfterStopRequired = value));
                }
            }
        }
    }
}