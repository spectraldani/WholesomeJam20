using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformHeight {
    Short, Medium, Long
}

public class Bush : MonoBehaviour {
    [SerializeField] private Sprite mediumSprite = null;
    [SerializeField] private Sprite longSprite = null;
    [SerializeField] private Sprite shortSprite = null;
    public SpriteRenderer SpriteRenderer = null;

    public void OnEnable() {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeBranchLength(PlatformHeight height) {
        switch (height) {
            case PlatformHeight.Short:
                SpriteRenderer.sprite = shortSprite;
                break;
            case PlatformHeight.Medium:
                SpriteRenderer.sprite = mediumSprite;
                break;
            case PlatformHeight.Long:
                SpriteRenderer.sprite = longSprite;
                break;
        }
    }
}
