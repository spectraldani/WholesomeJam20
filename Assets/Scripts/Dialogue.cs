using UnityEngine;

public enum FinishAction {
    StopRepeat, StopNext, Stop, Next
}

[System.Serializable]
public class Dialogue {
    public bool isPlayer = false;
    public FinishAction FinishAction;
    [TextArea(3, 20)] public string[] Sentences;
}
