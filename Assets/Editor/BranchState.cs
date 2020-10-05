using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Branch))]
public class BranchState : Editor {
    private Branch branch = null;
    private BranchLength state = BranchLength.Medium;

    public void OnEnable() {
        branch = (Branch)target;
        branch.MainBranchTransform = branch.transform.GetChild(0);
        branch.SpriteRenderer = branch.MainBranchTransform.GetComponent<SpriteRenderer>();
        branch.ChangeBranchLength(state);
    }

    public override void OnInspectorGUI() {
        state = (BranchLength)EditorGUILayout.EnumPopup("Platform height", state);
        branch.ChangeBranchLength(state);
    }
}
