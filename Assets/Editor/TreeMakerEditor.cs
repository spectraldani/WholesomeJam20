using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeMaker))]
public class TreeEditor : Editor {
    private TreeMaker tree;
    SerializedProperty gridSerializedProperty;
    private GameObject tree2Prefab;
    private GameObject tree1Prefab;
    private GameObject branchPrefab;
    private GameObject bushPrefab;

    void OnEnable() {
        gridSerializedProperty = serializedObject.FindProperty("grid");

        tree = (TreeMaker)target;
        tree2Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/tree_2.prefab");
        tree1Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/tree_1.prefab");

        branchPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Branch.prefab");
        bushPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bush.prefab");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        gridSerializedProperty.arraySize =
            9 * EditorGUILayout.IntSlider("Height", gridSerializedProperty.arraySize / 9, 0, 10);
        var currentSize = gridSerializedProperty.arraySize;
        var height = currentSize / 9;


        // if (currentSize < height * 9) {
        //     for (int i = 0; i < 9 * height - currentSize; i++) {
        //         for (int j = 0; j < 9; j++) {
        //             gridSerializedProperty.InsertArrayElementAtIndex(currentSize + i * 9 + j);
        //         }
        //     }
        // } else if (currentSize > height * 9) {
        //     for (int i = 0; i < currentSize - 9 * height; i++) {
        //         gridSerializedProperty.DeleteArrayElementAtIndex(currentSize - i - 1);
        //     }
        // }

        EditorGUILayout.PropertyField(gridSerializedProperty);

        for (int i = 0; i < height; i++) {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 9; j++) {
                var cell = gridSerializedProperty.GetArrayElementAtIndex(i * 9 + j);

                var guiStyle = new GUIStyle();
                guiStyle.fontSize = 9;
                guiStyle.alignment = TextAnchor.MiddleCenter;
                guiStyle.normal.background = Texture2D.whiteTexture;
                guiStyle.margin = new RectOffset(0, 10, 0, 10);
                cell.enumValueIndex = (int)(PlatformType)EditorGUILayout.EnumPopup((PlatformType)cell.enumValueIndex,
                    guiStyle, new[] {GUILayout.Width(25f), GUILayout.ExpandWidth(false)});
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Build")) {
            // Dispose current children
            int size = tree.transform.childCount;
            for (int i = size - 1; i >= 0; i--)
                GameObject.DestroyImmediate(tree.transform.GetChild(i).gameObject);

            var treeLength = Mathf.CeilToInt((height - 1) / 2f) + 2;

            // Create trunks
            var startingY = tree.GetComponent<SpriteRenderer>().sprite.bounds.max.y;
            for (int i = 0; i < treeLength; i++) {
                var currentPrefab = i % 2 == 0 ? tree2Prefab : tree1Prefab;
                var instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(currentPrefab, tree.transform);

                startingY += instantiatedPrefab.GetComponent<SpriteRenderer>().sprite.bounds.max.y;

                var t = instantiatedPrefab.transform.localPosition;
                t.y = startingY;
                instantiatedPrefab.transform.localPosition = t;
                startingY += instantiatedPrefab.GetComponent<SpriteRenderer>().sprite.bounds.max.y;
            }

            var gridSize = new Vector2(4.35f, 4.8f);

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < 9; j++) {
                    var cell = (PlatformType)gridSerializedProperty.GetArrayElementAtIndex(i * 9 + j).enumValueIndex;
                    switch (cell) {
                        case PlatformType.SBush:
                        case PlatformType.MBush:
                        case PlatformType.LBush: {
                            var offset = new Vector2(2.08f - gridSize.x * 4, 2.52f);
                            var newBush =
                                (GameObject)PrefabUtility.InstantiatePrefab(bushPrefab, tree.transform);
                            newBush.transform.position =
                                new Vector2(offset.x + gridSize.x * j, offset.y + gridSize.y * i);
                            if (cell == PlatformType.MBush) {
                                newBush.GetComponent<Branch>().ChangeBranchLength(PlatformHeight.Medium);
                            } else if (cell == PlatformType.LBush) {
                                newBush.GetComponent<Branch>().ChangeBranchLength(PlatformHeight.Long);
                            }

                            break;
                        }
                        case PlatformType.Branch: {
                            var offset = new Vector2(-11.15f, -1.05f);
                            var newBranch =
                                (GameObject)PrefabUtility.InstantiatePrefab(branchPrefab, tree.transform);
                            newBranch.transform.position =
                                new Vector2(offset.x + gridSize.x * j, offset.y + gridSize.y * i);
                            if (j > 4) {
                                newBranch.transform.localScale = new Vector3(-1,1,1);
                                newBranch.transform.position =
                                    new Vector2(newBranch.transform.position.x-7.6f, newBranch.transform.position.y);
                            }
                            break;
                        }
                        case PlatformType.None:
                            break;
                        default:
                            throw new Exception($"Unknown type");
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
