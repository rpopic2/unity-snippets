// Feel free to use or modify my code, as long as you maintain this header.
// Autor: rpopic2 (github.com/rpopic2/unity-snippets)
// Last Modified: 3 July 2023
// Description: Binds gameobjects to scripts by names.
// 1. Add [Bind("gameObject_name")] attribute to your instance fields.
// 2. The gameobjects will be assigned on script assembly reload.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Binder : EditorWindow
{
// private:
    static Binder() {
        AssemblyReloadEvents.afterAssemblyReload += Bind;
    }

    static Dictionary<Type, UnityEngine.Object[]?> _cache = new();

    static void Bind() {
        println("Start binding");

        // Find all classes which derive from MonoBehaviour
        var asm = typeof(BindAttribute).Assembly;
        var types = asm.GetTypes()
            .Where(x =>
                    x.BaseType == typeof(MonoBehaviour));

        foreach (var t in types) {
            var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            // Find all gameobjects with that type
            var gos = GameObject.FindObjectsOfType(t);
            if (gos.Length < 1)
                continue;

            foreach (var f in fields) {
                var s  = f.CustomAttributes;
                var attributes = f.GetCustomAttribute<BindAttribute>();
                UnityEngine.Object? targetGameObject = null;
                Type targetType = f.FieldType;
                if (!targetType.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;

                string? targetName = null;
                if (attributes is BindAttribute att) {
                    targetName = att.GameObjectName;
                    if (!_cache.ContainsKey(targetType)) {
                        _cache.Add(targetType, GameObject.FindObjectsOfType(targetType, true));
                    }
                    var vis = _cache[targetType].Where(x => x.name == targetName);
                    var cnt = vis.Count();
                    if (cnt == 0) {
                        Debug.LogError($"Cannot find GameObject {targetName}");
                        continue;
                    } else if (cnt != 1) {
                        Debug.LogWarning($"Found more than one {targetName}. Assigning with the first occurance");
                    }
                    targetGameObject = vis.ElementAt(0);
                } else {
                    continue;
                }

                foreach (var g in gos) {
                    var so = new SerializedObject(g);
                    f.SetValue(g, targetGameObject);
                    println($"Assign {targetGameObject?.name} to {g.name}");
                }
            }
        }
        EditorSceneManager.MarkAllScenesDirty();
        println("Done binding");
    }

    void OnGUI() {
        if (GUILayout.Button("Bind")) {
            Bind();
        }
    }

    [MenuItem("Window/BindManager")]
    static void InitWindow() {
        EditorWindow.GetWindow<Binder>();
    }

    static void println(string s) {
        Debug.Log($"[BindManager] {s}");
    }
}

class BindAttribute : Attribute
{
// public:
    public string GameObjectName { get; private set; }

    public BindAttribute(string gameObjectName) {
        GameObjectName = gameObjectName;
    }
}

