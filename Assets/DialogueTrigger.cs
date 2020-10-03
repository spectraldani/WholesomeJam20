using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public Dialogue Dialogue;

    [UsedImplicitly]
    public void TriggerDialogue() {
        FindObjectOfType<DialogueManager>().StartDialogue(Dialogue);
    }

    public void OnTriggerEnter2D(Collider2D x) {
        if (x.gameObject.layer == 9) {
            TriggerDialogue();
        }
    }
}
