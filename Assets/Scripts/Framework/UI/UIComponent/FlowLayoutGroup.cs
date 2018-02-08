using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu("Layout/Flow Layout Group", 153)]
public class FlowLayoutGroup : LayoutGroup
{

    #region Enum for Axis | Corner | Constraint
    //
    // Summary:
    //     An axis that can be horizontal or vertical.
    public enum Axis
    {
        //
        // Summary:
        //     Horizontal.
        Horizontal = 0,

        //
        // Summary:
        //     Vertical.
        Vertical = 1
    }

    public enum Corner
    {
        UpperLeft = 0,
        UpperRight = 1,
        LowerLeft = 2,
        LowerRight = 3
    }

    public enum Constraint
    {
        Flexible = 0,
        FixedColumnCount = 1,
        FixedRowCount = 2
    }
    #endregion

    [SerializeField] protected Axis m_StartAxis;


    protected Vector2 m_CellSize = new Vector2(100, 100);

    [SerializeField] protected Vector2 m_Spacing = Vector2.zero;



    [SerializeField]
    protected bool m_ChildForceExpandWidth = true;
    [SerializeField]
    protected bool m_ChildForceExpandHeight = true;
    [SerializeField]
    protected bool m_ChildControlWidth = true;
    [SerializeField]
    protected bool m_ChildControlHeight = true;

    #region Properties of Variable

    public Axis StartAxis
    {
        get { return m_StartAxis; }
        set { SetProperty(ref m_StartAxis, value); }
    }


    public Vector2 CellSize
    {
        get { return m_CellSize; }
        set { SetProperty(ref m_CellSize, value); }
    }


    public Vector2 Spacing
    {
        get { return m_Spacing; }
        set { SetProperty(ref m_Spacing, value); }
    }


    /// <summary>
    ///   <para>Whether to force the children to expand to fill additional available horizontal space.</para>
    /// </summary>
    public bool childForceExpandWidth
    {
        get
        {
            return this.m_ChildForceExpandWidth;
        }
        set
        {
            this.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
        }
    }

    /// <summary>
    ///   <para>Whether to force the children to expand to fill additional available vertical space.</para>
    /// </summary>
    public bool childForceExpandHeight
    {
        get
        {
            return this.m_ChildForceExpandHeight;
        }
        set
        {
            this.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
        }
    }

    /// <summary>
    ///   <para>Returns true if the Layout Group controls the widths of its children. Returns false if children control their own widths.</para>
    /// </summary>
    public bool childControlWidth
    {
        get
        {
            return this.m_ChildControlWidth;
        }
        set
        {
            this.SetProperty<bool>(ref this.m_ChildControlWidth, value);
        }
    }


    /// <summary>
    ///   <para>Returns true if the Layout Group controls the heights of its children. Returns false if children control their own heights.</para>
    /// </summary>
    public bool childControlHeight
    {
        get
        {
            return this.m_ChildControlHeight;
        }
        set
        {
            this.SetProperty<bool>(ref this.m_ChildControlHeight, value);
        }
    }

    #endregion

    //[SerializeField] protected bool m_Horizontal = true;
    //public bool horizontal { get { return m_Horizontal; } set { SetProperty(ref m_Horizontal, value); } }

    protected FlowLayoutGroup()
    { }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }

#endif

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        int minColumns = 0;
        int preferredColumns = 0;



        minColumns = 1;
        preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));

        SetLayoutInputForAxis(
            padding.horizontal + (CellSize.x + Spacing.x) * minColumns - Spacing.x,
            padding.horizontal + (CellSize.x + Spacing.x) * preferredColumns - Spacing.x,
            -1, 0);
    }

    public override void CalculateLayoutInputVertical()
    {
        int minRows = 0;

        float width = rectTransform.rect.size.x;
        int cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));
        //		minRows = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX);
        minRows = 1;
        float minSpace = padding.vertical + (CellSize.y + Spacing.y) * minRows - Spacing.y;
        SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
    }

    public override void SetLayoutHorizontal()
    {
        SetCellsAlongAxis();
    }

    public override void SetLayoutVertical()
    {
        SetCellsAlongAxis();
    }



    int cellsPerMainAxis, actualCellCountX, actualCellCountY;
    int positionX;
    int positionY;
    float totalWidth = 0;
    float totalHeight = 0;

    float lastMax = 0;

    private void SetCellsAlongAxis()
    {
        // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
        // and only vertical values when invoked for the vertical axis.
        // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
        // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
        // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.


        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;

        int cellCountX = 1;
        int cellCountY = 1;

        if (CellSize.x + Spacing.x <= 0)
            cellCountX = int.MaxValue;
        else
            cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));

        if (CellSize.y + Spacing.y <= 0)
            cellCountY = int.MaxValue;
        else
            cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y)));

        cellsPerMainAxis = cellCountX;
        actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
        actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));

        Vector2 requiredSpace = new Vector2(
            actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spacing.x,
            actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spacing.y
        );
        Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
        );

        totalWidth = 0;
        totalHeight = 0;
        Vector2 currentSpacing = Vector2.zero;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            SetChildAlongAxis(rectChildren[i], 0, startOffset.x + totalWidth /*+ currentSpacing[0]*/, rectChildren[i].rect.size.x);
            SetChildAlongAxis(rectChildren[i], 1, startOffset.y + totalHeight  /*+ currentSpacing[1]*/, rectChildren[i].rect.size.y);

            currentSpacing = Spacing;

            if (StartAxis == Axis.Horizontal)
            {
                totalWidth += rectChildren[i].rect.width + currentSpacing[0];
                if (rectChildren[i].rect.height > lastMax)
                {
                    lastMax = rectChildren[i].rect.height;
                }

                if (i < rectChildren.Count - 1)
                {
                    if (totalWidth + rectChildren[i + 1].rect.width + currentSpacing[0] > width - padding.horizontal)
                    {
                        totalWidth = 0;
                        totalHeight += lastMax + currentSpacing[1];
                        lastMax = 0;
                    }
                }
            }
            else
            {
                totalHeight += rectChildren[i].rect.height + currentSpacing[1];
                if (rectChildren[i].rect.width > lastMax)
                {
                    lastMax = rectChildren[i].rect.width;
                }

                if (i < rectChildren.Count - 1)
                {
                    if (totalHeight + rectChildren[i + 1].rect.height + currentSpacing[1] > height - padding.vertical)
                    {
                        totalHeight = 0;
                        totalWidth += lastMax + currentSpacing[0];
                        lastMax = 0;
                    }
                }
            }
        }
    }
}
