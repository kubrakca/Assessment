using UnityEditor;

[InitializeOnLoad]
public static class AutoOpenGameEditorWindow
{
    static AutoOpenGameEditorWindow()
    {
        EditorApplication.update += OpenOnce;
    }

    static void OpenOnce()
    {
        EditorApplication.update -= OpenOnce;
        GameEditorWindow.OpenWindow();
    }
}

