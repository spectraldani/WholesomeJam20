using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    private Queue<String> lines = new Queue<string>();
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private CanvasRenderer dialogueBox = null;
    [SerializeField] private TextMeshProUGUI bodyTextbox = null;
    [SerializeField] private Image nextSign = null;
    [SerializeField] private PlayerController player = null;
    public Sprite[] BubbleSprites;

    [UsedImplicitly]
    public void Awake() {
        canvas.gameObject.SetActive(false);
    }

    public void Update() {
        if (Input.GetButtonDown("Submit")) {
            NextLine();
        }
    }

    public void StartDialogue(Dialogue dialogue, Vector2 speakerPosition) {
        canvas.gameObject.SetActive(true);

        var playerPosition = player.transform.position;
        var howFarX = speakerPosition.x - playerPosition.x;

        var dialogueImage = dialogueBox.GetComponent<Image>();
        dialogueImage.sprite = howFarX > 0 ? BubbleSprites[1] : BubbleSprites[0];

        speakerPosition.x += Mathf.Sign(-howFarX) * (((RectTransform)canvas.transform).rect.xMax* canvas.transform.localScale[0]) * 0.44f;

        canvas.transform.position = speakerPosition;
        player.SetFreeze(true);
        lines.Clear();

        foreach (var dialogueSentence in dialogue.sentences) {
            lines.Enqueue(dialogueSentence);
        }

        nextSign.gameObject.SetActive(lines.Count > 1);
        NextLine();
    }

    public void NextLine() {
        switch (lines.Count) {
            case 0:
                canvas.gameObject.SetActive(false);
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
