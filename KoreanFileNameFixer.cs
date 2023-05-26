// Feel free to use or modify my code, as long as you maintain this header.
// Autor: rpopic2 (github.com/rpopic2/unity-snippets)
// Last Modified: 26 May 2023
// Description: This script renames all files in a given directory to their normalized names to fix issues with Korean file names.
using UnityEditor;
using UnityEngine;

public class KoreanFileNameFixer : EditorWindow
{
// private:
    string _searchFilter = string.Empty;
    string _searchDirectory = string.Empty;
    string[] _stringDirectoryTokens = new string[1] { string.Empty };

    [MenuItem("Window/한국어 파일이름 고치기")]
    static void Window() {
        EditorWindow.GetWindow(typeof(KoreanFileNameFixer), true, "한국어 파일이름 고치기", true);
    }

    void OnGUI() {
        GUILayout.Label("한국어 파일이름 고치기");
        GUILayout.Label("유니코드 정규화 방식을 NFC로 일괄 변경해줍니다.");
        GUILayout.Label("필터 예시: t:texture2D");
        _searchFilter = EditorGUILayout.TextField("필터", _searchFilter);

        GUILayout.Label("경로 예시: Assets/Sprites/");
        _searchDirectory = EditorGUILayout.TextField("경로", _searchDirectory);

        if (GUILayout.Button("실행")) {
            _stringDirectoryTokens = new string[1];
            _stringDirectoryTokens[0] = _searchDirectory;
            FixNames();
            Debug.Log("파일이름 고치기 완료");
        }
    }

    void FixNames() {
        var guids = AssetDatabase.FindAssets(_searchFilter, _stringDirectoryTokens);
        foreach (string name in guids) {
            var path = AssetDatabase.GUIDToAssetPath(name);
            var slashIndex = path.LastIndexOf('/');
            var fileName = path[(slashIndex + 1)..];
            Debug.Log($"Renaming {fileName}");
            fileName = fileName.Normalize(System.Text.NormalizationForm.FormC);
            AssetDatabase.RenameAsset(path, fileName);
        }
    }
}
