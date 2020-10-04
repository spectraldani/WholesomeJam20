using System;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Branch))]
public class BranchState : Editor {
    private Branch branch = null;
    private PlatformHeight state = PlatformHeight.Short;

    public void OnEnable() {
        branch = (Branch)target;
        branch.SpriteRenderer = branch.GetComponent<SpriteRenderer>();
        branch.ChangeBranchLength(state);
    }

    public override void OnInspectorGUI() {
        state = (PlatformHeight)EditorGUILayout.EnumPopup("Platform height", state);
        branch.ChangeBranchLength(state);
    }
}
