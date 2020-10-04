using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeMaker))]
public class TreeEditor : Editor {
    private TreeMaker tree;
    SerializedProperty testeSerializedProperty;
    private int height = 1;
    private GameObject tree2Prefab;
    private GameObject tree1Prefab;

    void OnEnable() {
        testeSerializedProperty = serializedObject.FindProperty("teste");
        tree = (TreeMaker)target;
        tree2Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/tree_2.prefab");
        tree1Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/tree_1.prefab");
    }

    // Update is called once per frame
    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(testeSerializedProperty);
        serializedObject.ApplyModifiedProperties();
        height = EditorGUILayout.IntSlider("Height", height, 1, 10);
        if (GUILayout.Button("Build")) {
            int size = tree.transform.childCount;
            for (int i = size - 1; i >= 0; i--)
                GameObject.DestroyImmediate(tree.transform.GetChild(i).gameObject);

            var startingY = tree.GetComponent<SpriteRenderer>().sprite.bounds.max.y;
            for (int i = 0; i < height; i++) {
                var currentPrefab = i % 2 == 0 ? tree2Prefab : tree1Prefab;
                var instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(currentPrefab, tree.transform);

                startingY += instantiatedPrefab.GetComponent<SpriteRenderer>().sprite.bounds.max.y;

                var t = instantiatedPrefab.transform.localPosition;
                t.y = startingY;
                instantiatedPrefab.transform.localPosition = t;
                startingY += instantiatedPrefab.GetComponent<SpriteRenderer>().sprite.bounds.max.y;
            }
        }
    }
}
