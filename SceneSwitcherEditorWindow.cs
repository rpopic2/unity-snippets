// Author: rpopic2 (github.com/rpopic2)
// Last Modified: 2024-04-15
// Description: A simple scene switcher window for Unity Editor
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class SceneSwitcherEditorWindow : EditorWindow
{
    private Dictionary<string, string> scenes = new Dictionary<string, string>();
    [MenuItem("NOAH/SceneSwitcher")]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(SceneSwitcherEditorWindow), false, "Scene Switcher", true);
    }
    private void OnEnable()
    {
        Refresh();
    }
    private void OnGUI()
    {
        foreach (var (name, path) in scenes)
        {
            if (GUILayout.Button(name, GUILayout.Height(30)))
            {
                var dirty = false;
                for (int i = 0; i < EditorSceneManager.sceneCount; ++i) {
                    if (EditorSceneManager.GetSceneAt(i).isDirty) {
                        dirty = true;
                        break;
                    }
                }
                if (dirty) {
                    var ok = EditorUtility.DisplayDialog("Warning", "Do you want to save all open scenes?", "Yes", "No");
                    if (ok) {
                        for (int i = 0; i < EditorSceneManager.sceneCount; ++i) {
                            var scene = EditorSceneManager.GetSceneAt(i);
                            EditorSceneManager.SaveScene(scene);
                        }
                    } else {
                        return;
                    }
                }
                EditorSceneManager.OpenScene(path);
            }
        }
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            Refresh();
        }
    }
    private void Refresh()
    {
        scenes.Clear();
        foreach (var s in EditorBuildSettings.scenes)
        {
            var path = s.path;
            var slashLastIndex = path.LastIndexOf('/');
            var sceneName = path.Substring(slashLastIndex + 1, path.LastIndexOf('.') - slashLastIndex - 1);
            scenes.Add(sceneName, path);
        }
        scenes.Add("Editor", "Assets/Scenes/Editor.unity");
    }
}
