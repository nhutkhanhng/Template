using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/UIData/Text")]
public class UITextData : ScriptableObject
{
    [Header("Text Attribute")]
    public Font font;
    public int size;
    public Color color;

    public TextAnchor alignText;

    public HorizontalWrapMode horizontal;
    public VerticalWrapMode vertical;
    public bool RaycastTarget = false;
    public bool BestFit = false;

}
