using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor.UI;


// On or Off CustomEditor    
[CustomEditor(typeof(UIImageData), true)]
[CanEditMultipleObjects]
public class UIEditorImage : GraphicEditor
{
    private SerializedProperty m_FillMethod;
    private SerializedProperty m_FillOrigin;
    private SerializedProperty m_FillAmount;
    private SerializedProperty m_FillClockwise;
    private SerializedProperty m_Type;
    private SerializedProperty m_FillCenter;
    private SerializedProperty m_Sprite;
    private SerializedProperty m_PreserveAspect;

    private GUIContent m_SpriteContent;
    private GUIContent m_SpriteTypeContent;
    private GUIContent m_ClockwiseContent;

    private AnimBool m_ShowSlicedOrTiled;
    private AnimBool m_ShowSliced;
    private AnimBool m_ShowTiled;
    private AnimBool m_ShowFilled;
    private AnimBool m_ShowType;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.m_SpriteContent = new GUIContent("Source Image");
        this.m_SpriteTypeContent = new GUIContent("Image Type");
        this.m_ClockwiseContent = new GUIContent("Clockwise");

        this.m_Sprite = this.serializedObject.FindProperty("m_Sprite");
        this.m_Type = this.serializedObject.FindProperty("m_Type");
        this.m_FillCenter = this.serializedObject.FindProperty("m_FillCenter");
        this.m_FillMethod = this.serializedObject.FindProperty("m_FillMethod");
        this.m_FillOrigin = this.serializedObject.FindProperty("m_FillOrigin");
        this.m_FillClockwise = this.serializedObject.FindProperty("m_FillClockwise");
        this.m_FillAmount = this.serializedObject.FindProperty("m_FillAmount");
        this.m_PreserveAspect = this.serializedObject.FindProperty("m_PreserveAspect");
        this.m_ShowType = new AnimBool(this.m_Sprite.objectReferenceValue != (UnityEngine.Object)null);
        this.m_ShowType.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));
        Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
        this.m_ShowSlicedOrTiled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced);
        this.m_ShowSliced = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced);
        this.m_ShowTiled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Tiled);
        this.m_ShowFilled = new AnimBool(!this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Filled);

        this.m_ShowSlicedOrTiled.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));

        this.m_ShowSliced.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowTiled.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowFilled.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        this.SpriteGUI();
        this.AppearanceControlsGUI();
        this.RaycastControlsGUI();

        EditorGUILayout.PropertyField(this.m_PreserveAspect);

        this.m_ShowType.target = this.m_Sprite.objectReferenceValue != (UnityEngine.Object)null;


        if (EditorGUILayout.BeginFadeGroup(this.m_ShowType.faded))
            this.TypeGUI();


        EditorGUILayout.EndFadeGroup();

        this.NativeSizeButtonGUI();
        this.serializedObject.ApplyModifiedProperties();

    }

    protected override void OnDisable()
    {
        this.m_ShowType.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowSlicedOrTiled.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowSliced.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowTiled.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowFilled.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
    }


    protected void TypeGUI()
    {
        EditorGUILayout.PropertyField(this.m_Type, this.m_SpriteTypeContent, new GUILayoutOption[0]);
        ++EditorGUI.indentLevel;
        Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
        bool flag = !this.m_Type.hasMultipleDifferentValues && (enumValueIndex == Image.Type.Sliced || enumValueIndex == Image.Type.Tiled);
        this.m_ShowSlicedOrTiled.target = flag;
        this.m_ShowSliced.target = flag && !this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Sliced;
        this.m_ShowTiled.target = flag && !this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Tiled;
        this.m_ShowFilled.target = !this.m_Type.hasMultipleDifferentValues && enumValueIndex == Image.Type.Filled;
        UIImageData target = this.target as UIImageData;
        if (EditorGUILayout.BeginFadeGroup(this.m_ShowSlicedOrTiled.faded))
            EditorGUILayout.PropertyField(this.m_FillCenter);
        EditorGUILayout.EndFadeGroup();
        if (EditorGUILayout.BeginFadeGroup(this.m_ShowSliced.faded) && target.m_Sprite != (UnityEngine.Object)null)
            EditorGUILayout.HelpBox("This Image doesn't have a border.", MessageType.Warning);
        EditorGUILayout.EndFadeGroup();
        if (EditorGUILayout.BeginFadeGroup(this.m_ShowTiled.faded) && (UnityEngine.Object)target.m_Sprite != (UnityEngine.Object)null && (target.m_Sprite.texture.wrapMode != TextureWrapMode.Repeat || target.m_Sprite.packed))
            EditorGUILayout.HelpBox("It looks like you want to tile a sprite with no border. It would be more efficient to convert the Sprite to an Advanced texture, clear the Packing tag and set the Wrap mode to Repeat.", MessageType.Warning);
        EditorGUILayout.EndFadeGroup();
        if (EditorGUILayout.BeginFadeGroup(this.m_ShowFilled.faded))
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_FillMethod);
            if (EditorGUI.EndChangeCheck())
                this.m_FillOrigin.intValue = 0;
            switch ((int)(Image.FillMethod)this.m_FillMethod.enumValueIndex)
            {
                case 0:
                    this.m_FillOrigin.intValue = (int)(Image.OriginHorizontal)EditorGUILayout.EnumPopup("Fill Origin", (Enum)(object)(Image.OriginHorizontal)this.m_FillOrigin.intValue, new GUILayoutOption[0]);
                    break;
                case 1:
                    this.m_FillOrigin.intValue = (int)(Image.OriginVertical)EditorGUILayout.EnumPopup("Fill Origin", (Enum)(object)(Image.OriginVertical)this.m_FillOrigin.intValue, new GUILayoutOption[0]);
                    break;
                case 2:
                    this.m_FillOrigin.intValue = (int)(Image.Origin90)EditorGUILayout.EnumPopup("Fill Origin", (Enum)(object)(Image.Origin90)this.m_FillOrigin.intValue, new GUILayoutOption[0]);
                    break;
                case 3:
                    this.m_FillOrigin.intValue = (int)(Image.Origin180)EditorGUILayout.EnumPopup("Fill Origin", (Enum)(object)(Image.Origin180)this.m_FillOrigin.intValue, new GUILayoutOption[0]);
                    break;
                case 4:
                    this.m_FillOrigin.intValue = (int)(Image.Origin360)EditorGUILayout.EnumPopup("Fill Origin", (Enum)(object)(Image.Origin360)this.m_FillOrigin.intValue, new GUILayoutOption[0]);
                    break;
            }
            EditorGUILayout.PropertyField(this.m_FillAmount);

            if (this.m_FillMethod.enumValueIndex > 1)
                EditorGUILayout.PropertyField(this.m_FillClockwise, this.m_ClockwiseContent, new GUILayoutOption[0]);
        }
        EditorGUILayout.EndFadeGroup();
        --EditorGUI.indentLevel;
    }

    protected void SpriteGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(this.m_Sprite, this.m_SpriteContent, new GUILayoutOption[0]);
        if (!EditorGUI.EndChangeCheck())
            return;
        Sprite objectReferenceValue = this.m_Sprite.objectReferenceValue as Sprite;
        if ((bool)((UnityEngine.Object)objectReferenceValue))
        {
            Image.Type enumValueIndex = (Image.Type)this.m_Type.enumValueIndex;
            if ((double)objectReferenceValue.border.SqrMagnitude() > 0.0)
                this.m_Type.enumValueIndex = 1;
            else if (enumValueIndex == Image.Type.Sliced)
                this.m_Type.enumValueIndex = 0;
        }
    }
}



