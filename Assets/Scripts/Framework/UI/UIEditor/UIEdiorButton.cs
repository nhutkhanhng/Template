using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Animations;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CustomEditor(typeof(UIButtonData), false)]
[CanEditMultipleObjects]
public class UIEdiorButton : Editor
{
    private static List<UIEdiorButton> s_Editors = new List<UIEdiorButton>();
    private static bool s_ShowNavigation = false;
    private static string s_ShowNavigationKey = "UIEdiorButton.ShowNavigation";
    private GUIContent m_VisualizeNavigation = new GUIContent("Visualize", "Show navigation flows between selectable UI elements.");
    private AnimBool m_ShowColorTint = new AnimBool();
    private AnimBool m_ShowSpriteTrasition = new AnimBool();
    private AnimBool m_ShowAnimTransition = new AnimBool();

    private SerializedProperty m_InteractableProperty;

    private SerializedProperty m_TransitionProperty;
    private SerializedProperty m_ColorBlockProperty;
    private SerializedProperty m_SpriteStateProperty;
    private SerializedProperty m_AnimTriggerProperty;
    private SerializedProperty m_NavigationProperty;
    private string[] m_PropertyPathToExcludeForChildClasses;



    protected virtual void OnEnable()
    {
        this.m_InteractableProperty = this.serializedObject.FindProperty("m_Interactable");

        this.m_TransitionProperty = this.serializedObject.FindProperty("m_Transition");
        this.m_ColorBlockProperty = this.serializedObject.FindProperty("m_Colors");
        this.m_SpriteStateProperty = this.serializedObject.FindProperty("m_SpriteState");
        this.m_AnimTriggerProperty = this.serializedObject.FindProperty("m_AnimationTriggers");
        this.m_NavigationProperty = this.serializedObject.FindProperty("m_Navigation");

        Selectable.Transition transition = GetTransition(this.m_TransitionProperty);
        this.m_ShowColorTint.value = transition == Selectable.Transition.ColorTint;
        this.m_ShowSpriteTrasition.value = transition == Selectable.Transition.SpriteSwap;
        this.m_ShowAnimTransition.value = transition == Selectable.Transition.Animation;
        this.m_ShowColorTint.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowSpriteTrasition.valueChanged.AddListener(new UnityAction(((Editor)this).Repaint));

    }


    protected virtual void OnDisable()
    {
        this.m_ShowColorTint.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));
        this.m_ShowSpriteTrasition.valueChanged.RemoveListener(new UnityAction(((Editor)this).Repaint));

    }

    private static Selectable.Transition GetTransition(SerializedProperty transition)
    {
        return (Selectable.Transition)transition.enumValueIndex;
    }



    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        EditorGUILayout.PropertyField(this.m_InteractableProperty);
        Selectable.Transition transition = GetTransition(this.m_TransitionProperty);


        this.m_ShowColorTint.target = !this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.ColorTint;
        this.m_ShowSpriteTrasition.target = !this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.SpriteSwap;
        this.m_ShowAnimTransition.target = !this.m_TransitionProperty.hasMultipleDifferentValues && transition == Selectable.Transition.Animation;
        EditorGUILayout.PropertyField(this.m_TransitionProperty);
        ++EditorGUI.indentLevel;

        if ( transition == Selectable.Transition.SpriteSwap )
            EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.",
                MessageType.Warning);
        
        
        if (EditorGUILayout.BeginFadeGroup(this.m_ShowColorTint.faded))
            EditorGUILayout.PropertyField(this.m_ColorBlockProperty);
        EditorGUILayout.EndFadeGroup();


        if (EditorGUILayout.BeginFadeGroup(this.m_ShowSpriteTrasition.faded))
            EditorGUILayout.PropertyField(this.m_SpriteStateProperty);
        EditorGUILayout.EndFadeGroup();

        if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimTransition.faded))
        {
            EditorGUILayout.PropertyField(this.m_AnimTriggerProperty);
        }
        EditorGUILayout.EndFadeGroup();

        --EditorGUI.indentLevel;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(this.m_NavigationProperty);
        EditorGUI.BeginChangeCheck();
        Rect controlRect1 = EditorGUILayout.GetControlRect();
        controlRect1.xMin += EditorGUIUtility.labelWidth;


        //SelectableEditor.s_ShowNavigation = GUI.Toggle(controlRect1, SelectableEditor.s_ShowNavigation, this.m_VisualizeNavigation, EditorStyles.miniButton);
        //if (EditorGUI.EndChangeCheck())
        //{
        //    EditorPrefs.SetBool(SelectableEditor.s_ShowNavigationKey, SelectableEditor.s_ShowNavigation);
        //    SceneView.RepaintAll();
        //}
        this.ChildClassPropertiesGUI();
        this.serializedObject.ApplyModifiedProperties();
    }

    private void ChildClassPropertiesGUI()
    {
        if (this.IsDerivedSelectableEditor())
            return;
    }

    private bool IsDerivedSelectableEditor()
    {
        return this.GetType() != typeof(UIEdiorButton);
    }




  
}
