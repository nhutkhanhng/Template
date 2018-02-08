using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class FlexibleUI : MonoBehaviour, ISkinUI
{
    [Header("If useOwn = false | " +
            "Then Use Attribute of Default")]
    public bool useOwn = true;

    public void Initalization()
    {
        if ( useOwn )
		    OnSkinGUI();
	}

	public virtual void OnSkinGUI()
	{
		
	}

    public virtual void Update()
    {
        if (Application.isEditor)
        {
            OnSkinGUI();
        }
    }
}
