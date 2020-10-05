using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public Dialogue[] Paragraphs;
    public float YDelta = 0;
    private SpriteRenderer spriteRenderer = null;
    private PlayerController player = null;
    private bool canStart = true;
    private bool isIn = false;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            YDelta = spriteRenderer.bounds.extents.y;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    [UsedImplicitly]
    public void TriggerDialogue() {
        canStart = false;
        StartCoroutine(FindObjectOfType<DialogueManager>().StartDialogue(this));
    }

    public void FinishDialogue() {
        canStart = true;
    }

    public void OnTriggerExit2D(Collider2D x) {
        player.CanJump = true;
        isIn = false;
    }

    public void Update() {
        if (Input.GetButtonDown("Jump") && canStart && isIn) {
            TriggerDialogue();
        }
    }

    public void OnTriggerEnter2D(Collider2D x) {
        player.CanJump = false;
        isIn = true;
    }
}
