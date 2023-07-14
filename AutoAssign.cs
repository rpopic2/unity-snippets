// Feel free to use or modify my code, as long as you maintain this header.
// Autor: rpopic2 (github.com/rpopic2/unity-snippets)
// Last Modified: 3 July 2023
// Description: Automatically assign gameobjects to scripts.
// 1. Add [Assign("gameObject_name")] attribute to your instance fields.
// 2. For private fields, you'll need to also add [SerializeField] attribute.
// 3. The gameobjects will be assigned on script assembly reload.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AutoAssign : EditorWindow
{
// private:
    static AutoAssign() {
        AssemblyReloadEvents.afterAssemblyReload += Bind;
    }

    static Dictionary<Type, UnityEngine.Object[]?> _cache = new();

    static void Bind() {
        println("Start assigning");

        // Find all classes which derive from MonoBehaviour
        var asm = typeof(AssignAttribute).Assembly;
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
                var attributes = f.GetCustomAttribute<AssignAttribute>();
                UnityEngine.Object? targetGameObject = null;
                Type targetType = f.FieldType;
                if (!targetType.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;

                string? targetName = null;
                if (attributes is AssignAttribute att) {
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
                    f.SetValue(g, targetGameObject);
                    println($"Assign {targetGameObject?.name} to {g.name}");
                }
            }
        }
        EditorSceneManager.MarkAllScenesDirty();
        println("Done assigning");
    }

    void OnGUI() {
        if (GUILayout.Button("Assign")) {
            Bind();
        }
    }

    [MenuItem("Window/AutoAssign")]
    static void InitWindow() {
        EditorWindow.GetWindow<AutoAssign>();
    }

    static void println(string s) {
        Debug.Log($"[AutoAssign] {s}");
    }
}
#endif

class AssignAttribute : Attribute
{
// public:
    public string GameObjectName { get; private set; }

    public AssignAttribute(string gameObjectName) {
        GameObjectName = gameObjectName;
    }
}
