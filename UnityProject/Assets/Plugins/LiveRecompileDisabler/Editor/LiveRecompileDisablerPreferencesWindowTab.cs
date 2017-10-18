using UnityEditor;
using UnityEngine;

namespace LiveRecompileDisabler.Editor
{
    internal sealed class LiveRecompileDisablerPreferencesWindowTab
    {
        [PreferenceItem("Live Recompile")]
        public static void PreferencesGUI()
        {
            if (!LiveRecompileDisablerEditorPrefs.AutoRefresh)
            {
                if (!LiveRecompileDisablerEditorPrefs.EnableAutoRefreshAfterStopRequired)
                {
                    EditorGUILayout.HelpBox("Auto Refresh is disabled.", MessageType.Warning);
                }
            }

            LiveRecompileDisablerEditorPrefs.DisableAutoRefreshInPlayMode =
                EditorGUILayout.ToggleLeft("Disable Auto Refresh in Play Mode",
                    LiveRecompileDisablerEditorPrefs.DisableAutoRefreshInPlayMode);

            EditorGUILayout.BeginHorizontal();
            {
                LiveRecompileDisablerEditorPrefs.ChangePlayMode =
                    EditorGUILayout.ToggleLeft("Change Play Mode on compilation to",
                        LiveRecompileDisablerEditorPrefs.ChangePlayMode, "WordWrappedLabel");
                EditorGUI.BeginDisabledGroup(!LiveRecompileDisablerEditorPrefs.ChangePlayMode);
                {
                    LiveRecompileDisablerEditorPrefs.PlayMode =
                        (LiveRecompileDisablerPlayMode)
                            EditorGUILayout.EnumPopup(LiveRecompileDisablerEditorPrefs.PlayMode);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Recompiled assemblies reloading");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20.0f);
                EditorGUILayout.BeginVertical();

                LiveRecompileDisablerEditorPrefs.ReloadAssembliesInEditMode =
                    (LiveRecompileDisablerReloadAssembliesInEditMode)
                        EditorGUILayout.EnumPopup("In Edit Mode",
                            LiveRecompileDisablerEditorPrefs.ReloadAssembliesInEditMode);
                LiveRecompileDisablerEditorPrefs.ReloadAssembliesInPlayMode =
                    (LiveRecompileDisablerReloadAssembliesInPlayMode)
                        EditorGUILayout.EnumPopup("In Play Mode",
                            LiveRecompileDisablerEditorPrefs.ReloadAssembliesInPlayMode);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Press Ctrl+R to reload assemblies manually.\n" +
                "Assemblies always reloaded on entering Play Mode.",
                MessageType.Info);
        }
    }
}