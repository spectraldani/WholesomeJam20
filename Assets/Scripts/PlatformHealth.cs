using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class PlatformHealth : MonoBehaviour {
    public float secDuration = 4f;
    public Collider2D platform = null;

    [UsedImplicitly]
    void Awake() {
        platform = GetComponent<Collider2D>();
    }

    [UsedImplicitly]
    void Update() {
        if (secDuration <= 0 && platform.isActiveAndEnabled) {
            platform.enabled = false;
        }
    }

    [UsedImplicitly]
    void OnDrawGizmos() {
        var text = $"Hp: {secDuration * 100:0}ms";
        var position = transform.position;
        position.x -= 0.3f;
        position.y += 5.5f;
        var guiStyle = new GUIStyle {
            alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}, fontSize = 15
        };
        Handles.Label(position, text, guiStyle);
    }

    public void TakeHit(float damage) {
        secDuration -= damage;
    }
}
