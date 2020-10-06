using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class PlatformHealth : MonoBehaviour {
    public float SecDuration = 4f;
    private float currentSecDuration;
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

        currentSecDuration = SecDuration;
    }

    [UsedImplicitly]
    void Update() {
        if (currentSecDuration <= 0 && platform.isActiveAndEnabled) {
            platform.enabled = false;
        }

        canDropLeafs = particleSystem && !particleSystem.isPlaying;
    }

    [UsedImplicitly]
    void OnDrawGizmos() {
        // var text = $"Hp: {currentSecDuration * 100:0}ms";
        // var position = transform.position;
        // position.x -= 0.3f;
        // position.y += 5.5f;
        // var guiStyle = new GUIStyle {
        //     alignment = TextAnchor.MiddleCenter, normal = {textColor = Color.white}, fontSize = 15
        // };
        // Handles.Label(position, text, guiStyle);
    }

    public void TakeHit(float damage) {
        currentSecDuration -= damage;
        if (particleSystem && canDropLeafs) {
            canDropLeafs = false;
            particleSystem.Play();
        }

        if (currentSecDuration < 0 && GetComponent<Animator>()) {
            GetComponent<Animator>().Play("Fade");
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(3f);
        currentSecDuration = SecDuration;
        platform.enabled = true;
        GetComponent<Animator>().Play("Default");
    }
}
