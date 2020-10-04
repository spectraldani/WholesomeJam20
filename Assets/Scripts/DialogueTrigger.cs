using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public Dialogue Dialogue;
    private SpriteRenderer spriteRenderer = null;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [UsedImplicitly]
    public void TriggerDialogue() {
        var position = transform.position;
        if (spriteRenderer != null) {
            position.y = spriteRenderer.bounds.max.y;
            position.y += 0.5f;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(Dialogue, position);
    }

    public void OnTriggerEnter2D(Collider2D x) {
        if (x.gameObject.layer == 9) {
            TriggerDialogue();
        }
    }
}
