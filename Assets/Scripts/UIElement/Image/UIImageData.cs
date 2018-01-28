using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/UIData/Image")]
public class UIImageData : ScriptableObject
{
    public Material m_Material = (Material)null;

    public bool m_RaycastTarget;

    public Color m_Color;
    
    public Image.Type m_Type = Image.Type.Simple;
    
    public bool m_PreserveAspect = false;
    
    public bool m_FillCenter = true;
    
    public Image.FillMethod m_FillMethod = Image.FillMethod.Radial360;
    [Range(0.0f, 1f)]
    
    public float m_FillAmount = 1f;
    
    public bool m_FillClockwise = true;
    
    public Sprite m_Sprite;
    
    public int m_FillOrigin;

}
