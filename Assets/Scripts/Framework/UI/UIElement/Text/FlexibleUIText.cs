using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FlexibleUIText : FlexibleUI {

    private Text text;

    public UITextData data;

    void Start()
    {

        text = GetComponent<Text>();

        if (data)
            base.Initalization();
    }

    public override void OnSkinGUI()
    {
        if (useOwn)
        {
            text.color = data.color;
            text.font = data.font;

            text.alignment = data.alignText;
            text.horizontalOverflow = data.horizontal;
            text.verticalOverflow = data.vertical;

            text.fontSize = data.size;
            text.raycastTarget = data.RaycastTarget;
            text.resizeTextForBestFit = data.BestFit;
        }

    }
}
