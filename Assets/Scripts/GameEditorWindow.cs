using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameEditorWindow : EditorWindow
{
    Vector2 scrollPosition;
    List<GameObject> gameObjects = new List<GameObject>();
    List<GameObject> selectedObjects = new List<GameObject>();

    string searchText = "";

    bool filterMeshRenderer = false;
    bool filterCollider = false;
    bool filterRigidbody = false;

    string[] componentOptions = new string[] { "MeshRenderer", "BoxCollider", "Rigidbody" };
    int selectedComponentIndex = 0;
    int selectedRemoveComponentIndex = 0;

    [MenuItem("Tools/Game Editor Window")]
    public static void OpenWindow()
    {
        GetWindow<GameEditorWindow>("Game Editor Window");
    }

    private void OnFocus()
    {
        LoadGameObjects();
    }

    void LoadGameObjects()
    {
        gameObjects.Clear();
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in children)
            {
                gameObjects.Add(t.gameObject);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("GameObject List", EditorStyles.boldLabel);

        searchText = EditorGUILayout.TextField("Search (name):", searchText);

        GUILayout.Space(10);

        GUILayout.Label("Filter (by Components):", EditorStyles.boldLabel);
        filterMeshRenderer = EditorGUILayout.Toggle("Only Mesh Renderer", filterMeshRenderer);
        filterCollider = EditorGUILayout.Toggle("Only Collider", filterCollider);
        filterRigidbody = EditorGUILayout.Toggle("Only Rigidbody", filterRigidbody);

        GUILayout.Space(10);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (GameObject obj in gameObjects)
        {
            if (!string.IsNullOrEmpty(searchText) && !obj.name.ToLower().Contains(searchText.ToLower()))
            {
                continue;
            }

            if (filterMeshRenderer && obj.GetComponent<MeshRenderer>() == null)
            {
                continue;
            }

            if (filterCollider && obj.GetComponent<Collider>() == null)
            {
                continue;
            }

            if (filterRigidbody && obj.GetComponent<Rigidbody>() == null)
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            bool isSelected = selectedObjects.Contains(obj);
            bool toggle = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

            if (toggle != isSelected)
            {
                if (toggle)
                    selectedObjects.Add(obj);
                else
                    selectedObjects.Remove(obj);
            }

            EditorGUILayout.LabelField(obj.name);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (selectedObjects.Count > 0)
        {
            GUILayout.Label("Selected " + selectedObjects.Count + " GameObject(s)", EditorStyles.boldLabel);

            Vector3 newPos = EditorGUILayout.Vector3Field("Position", selectedObjects[0].transform.position);
            if (newPos != selectedObjects[0].transform.position)
            {
                Undo.RecordObjects(selectedObjects.ToArray(), "Position Changed");
                foreach (GameObject obj in selectedObjects)
                {
                    obj.transform.position = newPos;
                }
            }

            Vector3 newRot = EditorGUILayout.Vector3Field("Rotation", selectedObjects[0].transform.rotation.eulerAngles);
            if (newRot != selectedObjects[0].transform.rotation.eulerAngles)
            {
                Undo.RecordObjects(selectedObjects.ToArray(), "Rotation Changed");
                foreach (GameObject obj in selectedObjects)
                {
                    obj.transform.rotation = Quaternion.Euler(newRot);
                }
            }

            Vector3 newScale = EditorGUILayout.Vector3Field("Scale", selectedObjects[0].transform.localScale);
            if (newScale != selectedObjects[0].transform.localScale)
            {
                Undo.RecordObjects(selectedObjects.ToArray(), "Scale Changed");
                foreach (GameObject obj in selectedObjects)
                {
                    obj.transform.localScale = newScale;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Add Component Option", EditorStyles.boldLabel);
            selectedComponentIndex = EditorGUILayout.Popup("Select Component:", selectedComponentIndex, componentOptions);

            if (GUILayout.Button("Add Component"))
            {
                foreach (GameObject obj in selectedObjects)
                {
                    Undo.RegisterCompleteObjectUndo(obj, "Add Component");

                    string selected = componentOptions[selectedComponentIndex];

                    if (selected == "MeshRenderer" && obj.GetComponent<MeshRenderer>() == null)
                    {
                        obj.AddComponent<MeshRenderer>();
                    }
                    else if (selected == "BoxCollider" && obj.GetComponent<BoxCollider>() == null)
                    {
                        obj.AddComponent<BoxCollider>();
                    }
                    else if (selected == "Rigidbody" && obj.GetComponent<Rigidbody>() == null)
                    {
                        obj.AddComponent<Rigidbody>();
                    }
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Remove Component Option", EditorStyles.boldLabel);
            selectedRemoveComponentIndex = EditorGUILayout.Popup("Select Component:", selectedRemoveComponentIndex, componentOptions);

            if (GUILayout.Button("Remove Component"))
            {
                foreach (GameObject obj in selectedObjects)
                {
                    Undo.RegisterCompleteObjectUndo(obj, "Remove Component");

                    string selected = componentOptions[selectedRemoveComponentIndex];

                    if (selected == "MeshRenderer")
                    {
                        MeshRenderer comp = obj.GetComponent<MeshRenderer>();
                        if (comp != null)
                            DestroyImmediate(comp);
                    }
                    else if (selected == "BoxCollider")
                    {
                        BoxCollider comp = obj.GetComponent<BoxCollider>();
                        if (comp != null)
                            DestroyImmediate(comp);
                    }
                    else if (selected == "Rigidbody")
                    {
                        Rigidbody comp = obj.GetComponent<Rigidbody>();
                        if (comp != null)
                            DestroyImmediate(comp);
                    }
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Active/Inactive"))
            {
                Undo.RecordObjects(selectedObjects.ToArray(), "Active/Inactive");
                foreach (GameObject obj in selectedObjects)
                {
                    obj.SetActive(!obj.activeSelf);
                }
            }
        }
    }
}
