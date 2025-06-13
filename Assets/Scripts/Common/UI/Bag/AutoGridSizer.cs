using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoGridSizer : MonoBehaviour
{
    public int rows = 3;
    public int columns = 5;
    public Vector2 spacing = new Vector2(5, 5);


    public RectOffset padding;

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;

    private void Awake()
    {

        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        if (padding == null)
        {
            padding = new RectOffset(0, 0, 0, 0);
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        float availableWidth = rectTransform.rect.width
                                 - (padding.left + padding.right)
                                 - spacing.x * (columns - 1);
        float availableHeight = rectTransform.rect.height
                                 - (padding.top + padding.bottom)
                                 - spacing.y * (rows - 1);
        float cellSide = Mathf.Min(availableWidth / columns, availableHeight / rows);

        gridLayoutGroup.padding = padding;
        gridLayoutGroup.spacing = spacing;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        gridLayoutGroup.cellSize = new Vector2(cellSide, cellSide);
    }
}