using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformType {
    None, SBush, MBush, LBush, Branch
}

public class TreeMaker : MonoBehaviour {
    [SerializeField] private PlatformType[] grid = new PlatformType[0];
}
