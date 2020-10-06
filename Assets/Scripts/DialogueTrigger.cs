using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public bool IsAuto = false;
    public bool GiveJump = false;
    public Dialogue[] Paragraphs;
    public float YDelta = 0;
    private SpriteRenderer spriteRenderer = null;
    private PlayerController player = null;
    private bool canStart = true;
    private bool isIn = false;
    private bool pastJumpStatus;

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

    public IEnumerator FinishDialogue() {
        if (GiveJump) {
            pastJumpStatus = true;
        }
        yield return new WaitForSeconds(0.2f);
        canStart = true;
        yield return null;
    }

    public void OnTriggerExit2D(Collider2D x) {
        player.CanJump = pastJumpStatus;
        isIn = false;
    }

    public void Update() {
        if ((Input.GetButtonDown("Jump") || IsAuto) && canStart && isIn) {
            TriggerDialogue();
        }
    }

    public void OnTriggerEnter2D(Collider2D x) {
        pastJumpStatus = player.CanJump;
        player.CanJump = false;
        isIn = true;
    }
}
