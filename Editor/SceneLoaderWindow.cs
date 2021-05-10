/*
Modified & better version of https://github.com/Demkeys/SceneLoaderWindow

Scene Loader Window is a simple Editor Tool that provides you with easy access to all the scenes in your project.
You can choose between OpenScene and OpenSceneAdditive, and the tool will open your scene accordingly.
NOTES:
- Make sure this script is in the Editor folder.
- The scenes do not need to be added in Build Settings.
*/

using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneLoaderWindow : EditorWindow {

	private Vector2 scrollPos;

    private int m_SceneIndexBeforePlay;

    [MenuItem("Extra Tools/Scene Loader")]
    internal static void Init()
    {
        var window = (SceneLoaderWindow)GetWindow(typeof(SceneLoaderWindow), false, "Scene Switch");
        window.position = new Rect(window.position.xMin + 100f, window.position.yMin + 100f, 200f, 400f);
    }

    internal void OnGUI()
    {
        float width = this.position.width;

        EditorGUILayout.BeginVertical();
        this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos, false, false);

        using (var disableGroup = new EditorGUI.DisabledGroupScope(EditorApplication.isPlaying))
        {
            GUILayout.Label("Scenes In Build Settings", EditorStyles.boldLabel);
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];

                using (var horizontalScope = new EditorGUILayout.HorizontalScope())
                {
                    var bgCol = GUI.backgroundColor;

                    GUI.backgroundColor = SceneManager.GetActiveScene().path == scene.path ? Color.green : bgCol;

                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    var loadPressed = GUILayout.Button(sceneName, new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleCenter });
                    if (loadPressed)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scene.path);
                        }
                    }

                    GUI.backgroundColor = EditorApplication.isPlaying && SceneManager.GetActiveScene().path == scene.path ? Color.green : bgCol;
                    var playPressed = GUILayout.Button("Play", new GUIStyle(GUI.skin.GetStyle("Button")) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(width * 0.2f));
                    if (playPressed)
                    {
                        play(i);
                    }

                    GUI.backgroundColor = bgCol;
                }
            }
        }

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Stop", new GUIStyle(GUI.skin.GetStyle("Button"))))
            {
                stop();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void play(int index)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            m_SceneIndexBeforePlay = EditorSceneManager.GetActiveScene().buildIndex;

            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[index].path);

            EditorApplication.isPlaying = true;
        }
    }

    private void stop()
    {
        EditorApplication.isPlaying = false;

        EditorApplication.playModeStateChanged += onEnteredEditMode;
    }

    private void onEnteredEditMode(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[m_SceneIndexBeforePlay].path);

            EditorApplication.playModeStateChanged -= onEnteredEditMode;
        }
    }
}
