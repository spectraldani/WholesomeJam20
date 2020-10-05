using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class DialogueManager : MonoBehaviour {
    private Queue<String> lines = new Queue<string>();
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private CanvasRenderer dialogueBox = null;
    [SerializeField] private TextMeshProUGUI bodyTextbox = null;
    [SerializeField] private Image nextSign = null;
    [SerializeField] private PlayerController player = null;
    public Sprite[] BubbleSprites;

    private bool canSubmit = false;
    private DialogueTrigger currentTrigger = null;
    private int currentParagraphIndex = 0;
    private Vector2 cameraBounds;
    private RectTransform canvasTransform;

    [UsedImplicitly]
    public void Awake() {
        canvas.gameObject.SetActive(false);
        canvasTransform = ((RectTransform)canvas.transform);
        cameraBounds = new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height,
            Camera.main.orthographicSize);
    }

    public void Update() {
        if (Input.GetButtonDown("Submit") && canSubmit) {
            NextLine();
        }
    }

    public void FixedUpdate() {
        if (currentTrigger) {
            MoveDialogueBox();
        }
    }

    private void MoveDialogueBox() {
        Vector2 speakerPosition = currentTrigger.transform.position;
        var playerPosition = (Vector2)player.transform.position;
        var howFarX = speakerPosition.x - playerPosition.x;
        if (!currentTrigger.Paragraphs[currentParagraphIndex].isPlayer) {
            speakerPosition.y += currentTrigger.YDelta;
        } else {
            speakerPosition = player.transform.position;
            speakerPosition.y += player.gameObject.GetComponent<SpriteRenderer>().bounds.size.y;
            howFarX = -howFarX;
        }

        var cameraPosition = Camera.main.transform.position.x;
        var dialogueImage = dialogueBox.GetComponent<Image>();
        dialogueImage.sprite = howFarX > 0 ? BubbleSprites[1] : BubbleSprites[0];
        speakerPosition.x += Mathf.Sign(-howFarX) *
                             (canvasTransform.rect.xMax * canvasTransform.localScale[0]) * 0.5f;

        speakerPosition.x = Mathf.Clamp(
            speakerPosition.x,
            cameraPosition - cameraBounds.x + (canvasTransform.rect.width * canvasTransform.localScale[0] * 0.5f),
            cameraPosition + cameraBounds.x - (canvasTransform.rect.width * canvasTransform.localScale[0] * 0.5f)
        );
        canvas.transform.position = speakerPosition;
    }

    public IEnumerator StartDialogue(DialogueTrigger trigger) {
        canvas.gameObject.SetActive(true);
        player.SetFreeze(true);
        currentTrigger = trigger;

        currentParagraphIndex = 0;
        MoveDialogueBox();

        NextParagraph();
        NextLine();
        yield return new WaitForSeconds(0.2f);
        canSubmit = true;
        yield return null;
    }

    private void NextParagraph() {
        lines.Clear();
        var currentParagraph = currentTrigger.Paragraphs[currentParagraphIndex];
        foreach (var sentence in currentParagraph.Sentences) {
            lines.Enqueue(sentence);
        }
    }

    public void NextLine() {
        switch (lines.Count) {
            case 0: {
                var currentParagraph = currentTrigger.Paragraphs[currentParagraphIndex];
                switch (currentParagraph.FinishAction) {
                    case FinishAction.Next:
                        currentParagraphIndex++;
                        NextParagraph();
                        NextLine();
                        break;
                    case FinishAction.StopRepeat:
                        canSubmit = false;
                        currentTrigger.FinishDialogue();
                        currentTrigger = null;
                        canvas.gameObject.SetActive(false);
                        player.SetFreeze(false);
                        break;
                    case FinishAction.StopNext:
                        canSubmit = false;
                        currentTrigger.FinishDialogue();
                        canvas.gameObject.SetActive(false);
                        player.SetFreeze(false);
                        currentTrigger.Paragraphs = currentTrigger.Paragraphs.Skip(1).ToArray();
                        currentTrigger = null;
                        break;
                    case FinishAction.Stop:
                        canSubmit = false;
                        currentTrigger.FinishDialogue();
                        canvas.gameObject.SetActive(false);
                        player.SetFreeze(false);
                        currentTrigger.enabled = false;
                        currentTrigger = null;
                        break;
                }

                break;
            }
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
