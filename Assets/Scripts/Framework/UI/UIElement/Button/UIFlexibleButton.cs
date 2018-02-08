
using UnityEngine.UI;

public class UIFlexibleButton : FlexibleUI
{
    private Button button;

    public UIButtonData data;
	// Use this for initialization
	void Start ()
	{
	    button = GetComponent<Button>();

        if (data != null)
            OnSkinGUI();
	}

    public override void OnSkinGUI()
    {
        if ( useOwn )
        {
            //this.button.onClick = data.onclick;
            //this.button.colors = data.buttonColorBlock;
        }
    }
}
