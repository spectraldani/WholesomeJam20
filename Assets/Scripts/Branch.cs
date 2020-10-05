using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BranchLength {
    Short, Medium, Long
}

public class Branch : MonoBehaviour {
    [SerializeField] private Sprite longSprite = null;
    [SerializeField] private Sprite mediumSprite = null;
    [SerializeField] private Sprite shortSprite = null;
    public SpriteRenderer SpriteRenderer = null;
    public Transform MainBranchTransform = null;

    public void OnEnable() {
        MainBranchTransform = transform.GetChild(0);
        SpriteRenderer = MainBranchTransform.GetComponent<SpriteRenderer>();
    }

    public void ChangeBranchLength(BranchLength height) {
        switch (height) {
            case BranchLength.Short:
                MainBranchTransform.localPosition = new Vector3(-2.2f, 0.05f);
                SpriteRenderer.sprite = shortSprite;
                break;
            case BranchLength.Medium:
                MainBranchTransform.localPosition = new Vector3(-1.22f, 0.05f);
                SpriteRenderer.sprite = mediumSprite;
                break;
            case BranchLength.Long:
                MainBranchTransform.localPosition = new Vector3(5.4f, 0.05f);
                SpriteRenderer.sprite = longSprite;
                break;
        }
    }
}
