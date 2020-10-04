using System;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(Bush))]
public class BushState : Editor {
    private Bush bush = null;
    private PlatformHeight state = PlatformHeight.Short;

    public void OnEnable() {
        bush = (Bush)target;
        bush.SpriteRenderer = bush.GetComponent<SpriteRenderer>();
        bush.ChangeBranchLength(state);
    }

    public override void OnInspectorGUI() {
        state = (PlatformHeight)EditorGUILayout.EnumPopup("Platform height", state);
        bush.ChangeBranchLength(state);
    }
}
