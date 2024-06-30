using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayout : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform panelRectTransform;
    public int rows = 2;
    public int columns = 2;

    void Start()
    {
        UpdateGridLayout();
    }

    public void UpdateGridLayout()
    {
        float panelWidth = panelRectTransform.rect.width;
        float panelHeight = panelRectTransform.rect.height;

        float cellWidth = (panelWidth - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right - (columns - 1) * gridLayoutGroup.spacing.x) / columns;
        float cellHeight = (panelHeight - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom - (rows - 1) * gridLayoutGroup.spacing.y) / rows;

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }

    public void SetGridSize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        UpdateGridLayout();
    }
}
