# unity-snippets

Useful Unity Engine code snippets.

Feel free to use my code, as long as you maintain the header of the source code as follows:
```
// Feel free to use or modify my code, as long as you maintain this header.
// Autor: rpopic2 (github.com/rpopic2/unity-snippets)
// Last Modified: (date)
// Description: (description)
```

## SceneSwitcherEditorWindow.cs

A simple scene switcher. It lists all scenes added to the build settings.

## AutoAssign.cs
Automatically assign gameobjects to scripts.
1. Add [Assign("gameObject_name")] attribute to your instance fields.
2. For private fields, you'll need to also add [SerializeField] attribute.
3. The gameobjects will be assigned on script assembly reload.
  
  

## KoreanFileNameFixer.cs

This script renames all files in a given directory to their normalized names to fix issues with Korean file names.

맥에서 유니티가 한글 이름을 사용할 때 유니코드 정규화 방식의 차이로 자모가 분리되는 현상이 있습니다. 'Normalization Form C'로 통일하여 이러한 현상을 스크립트 하나로 일괄 수정할 수 있게 해줍니다.
