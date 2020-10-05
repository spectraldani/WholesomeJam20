using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class PlatformHealth : MonoBehaviour {
    public float SecDuration = 4f;
    private new ParticleSystem particleSystem = null;
    private Collider2D platform = null;

    private bool canDropLeafs = true;

    [UsedImplicitly]
    void Awake() {
        platform = GetComponent<Collider2D>();
        try {
            particleSystem = GetComponentInChildren<ParticleSystem>();
        } catch {
            // ignored
        }
    }

    [UsedImplicitly]
    void Update() {
        if (SecDuration <= 0 && platform.isActiveAndEnabled) {
            platform.enabled = false;
        }

        canDropLeafs = particleSystem && !particleSystem.isPlaying;
    }

    [UsedImplicitly]
    void OnDrawGizmos() {
        var text = $"Hp: {SecDuration * 100:0}ms";
        var position = transform.position;
        position.x -= 0.3f;
        position.y += 5.5f;
        var guiStyle = new GUIStyle {
            alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}, fontSize = 15
        };
        Handles.Label(position, text, guiStyle);
    }

    public void TakeHit(float damage) {
        SecDuration -= damage;
        if (particleSystem && canDropLeafs) {
            canDropLeafs = false;
            particleSystem.Play();
        }
    }
}
