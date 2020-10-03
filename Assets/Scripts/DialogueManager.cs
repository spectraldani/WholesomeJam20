using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    private Queue<String> lines = new Queue<string>();
    [SerializeField] private CanvasRenderer dialogueBox = null;
    [SerializeField] private TextMeshProUGUI nameTextbox = null;
    [SerializeField] private TextMeshProUGUI bodyTextbox = null;
    [SerializeField] private Image nextSign = null;
    [SerializeField] private PlayerController player = null;

    [UsedImplicitly]
    public void Awake() {
        dialogueBox.gameObject.SetActive(false);
    }

    public void Update() {
        if (Input.GetButtonDown("Submit")) {
            NextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue) {
        dialogueBox.gameObject.SetActive(true);
        player.SetFreeze(true);
        lines.Clear();

        nameTextbox.text = dialogue.name;

        foreach (var dialogueSentence in dialogue.sentences) {
            lines.Enqueue(dialogueSentence);
        }

        nextSign.gameObject.SetActive(lines.Count > 1);
        NextLine();
    }

    public void NextLine() {
        switch (lines.Count) {
            case 0:
                dialogueBox.gameObject.SetActive(false);
                player.SetFreeze(false);
                break;
            case 1:
                nextSign.gameObject.SetActive(false);
                bodyTextbox.text = lines.Dequeue();
                break;
            default:
                bodyTextbox.text = lines.Dequeue();
                break;
        }
    }
}
