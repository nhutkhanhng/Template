
using UnityEngine.UI;

public class UIImageFlexible : FlexibleUI
{

    private Image image;
    public UIImageData data;

    private void Awake()
    {
        image = GetComponent<Image>();
        
        if (useOwn)
            OnSkinGUI();
    }

    public override void OnSkinGUI()
    {
        if (useOwn)
        {
            this.image.sprite = data.m_Sprite;
            this.image.raycastTarget = data.m_RaycastTarget;
            this.image.color = data.m_Color;
            this.image.type = data.m_Type;

            this.image.preserveAspect = data.m_PreserveAspect;
            this.image.fillCenter = data.m_FillCenter;
            this.image.fillMethod = data.m_FillMethod;
            this.image.fillAmount = data.m_FillAmount;
            this.image.fillClockwise = data.m_FillClockwise;
            this.image.fillOrigin = data.m_FillOrigin;
        }

    }

}
