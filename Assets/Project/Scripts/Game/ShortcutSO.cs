using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Shortcuts", order = 1)]
public class ShortcutSO : ScriptableObject
{
    public Shortcut shortCut;
}

[Serializable]
public class Shortcut
{
    public string shortCutEnum;
    public Sprite shortCutImage;
    public string shortCutText;
}