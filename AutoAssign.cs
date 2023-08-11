// Feel free to use or modify my code, as long as you maintain this header.
// Autor: rpopic2 (github.com/rpopic2/unity-snippets)
// Last Modified: 7 Aug 2023
// Description: Automatically assign gameobjects to scripts.
// 1. Add [Bind] attribute to your instance fields. Make sure the name of the field is equal to the name of the GameObject to be assigned.
// 2. For private fields, you'll need to also add [SerializeField] attribute.
// 3. The gameobjects will be assigned on script assembly reload.

#nullable enable
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

class AutoAssign : EditorWindow
{
    static readonly Dictionary<Type, UnityEngine.Object[]?> _cache = new();
    static readonly Assembly _asm = typeof(AutoAssign).Assembly;

    static AutoAssign()
    {
        AssemblyReloadEvents.afterAssemblyReload += Bind;
    }

    static IEnumerable<Type> GetTypes()
    {
        return _asm.GetTypes().Where(
            x => x.BaseType == typeof(MonoBehaviour)
        );
    }

    static void Bind()
    {
        bool isDirty = false;

        foreach (var t in GetTypes()) {
            if (t.IsGenericType)
                continue;
            var gos = GameObject.FindObjectsOfType(t);
            if (gos.Length <= 0)
                continue;

            var fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var f in fields) {
                Type targetType = f.FieldType;
                if (!targetType.IsSubclassOf(typeof(UnityEngine.Object)))
                    continue;

                BindAttribute attributes = f.GetCustomAttribute<BindAttribute>();
                UnityEngine.Object? targetGameObject = null;

                if (attributes is BindAttribute att) {
                    var targetName = f.Name;
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
                    if (f.GetValue(g) == (object?)targetGameObject) {
                        continue;
                    }
                    f.SetValue(g, targetGameObject);
                    println($"Assign {targetGameObject?.name} to {g.name}");
                    isDirty = true;
                }
            }
        }
        if (isDirty)
            EditorSceneManager.MarkAllScenesDirty();
    }

    void OnGUI() {
        if (GUILayout.Button("Bind")) {
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


[Conditional("UNITY_EDITOR")]
class BindAttribute : Attribute { }

