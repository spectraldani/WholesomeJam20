using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformHeight {
    Short, Medium, Long
}

public class Branch : MonoBehaviour {
    [SerializeField] private Sprite mediumSprite = null;
    [SerializeField] private Sprite longSprite = null;
    [SerializeField] private Sprite shortSprite = null;

    public void ChangeBranchLength(PlatformHeight height) {
        switch (height) {
            case PlatformHeight.Short:
                transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = shortSprite;
                break;
            case PlatformHeight.Medium:
                transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = mediumSprite;
                break;
            case PlatformHeight.Long:
                transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite = longSprite;
                break;
        }
    }
}
