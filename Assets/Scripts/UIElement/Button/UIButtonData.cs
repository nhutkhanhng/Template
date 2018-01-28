using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;


/// <summary>
/// This class 
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/UIData/Button/ColorTint")]
public class UIButtonData : ScriptableObject {

    #region Button


    //public Selectable.Transition m_Transition = Selectable.Transition.ColorTint;

    ///// <summary>
    /////  This variable is contain all color of button on UnityEngine.UI.Button
    ///// </summary>
    //[Header("Button Colors")] public ColorBlock buttonColorBlock;

    //[Header("Event onClick")] public Button.ButtonClickedEvent onclick;

    //public AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

    //public SpriteState m_SpriteState;

    [FormerlySerializedAs("navigation")]
    public Navigation m_Navigation = Navigation.defaultNavigation;

    [FormerlySerializedAs("transition")]
    public Selectable.Transition m_Transition = Selectable.Transition.ColorTint;

    [FormerlySerializedAs("colors")]
    public ColorBlock m_Colors = ColorBlock.defaultColorBlock;

    [FormerlySerializedAs("animationTriggers")]
    public AnimationTriggers m_AnimationTriggers = new AnimationTriggers();

    [Tooltip("Can the Selectable be interacted with?")]
    public bool m_Interactable = true;

    [FormerlySerializedAs("spriteState")]
    public SpriteState m_SpriteState;


    #endregion
}