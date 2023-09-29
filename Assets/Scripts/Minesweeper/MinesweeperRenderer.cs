using System;
using Minesweeper;
using TMPro;
using UnityEngine;

public class MinesweeperRenderer : MonoBehaviour
{
    [SerializeField] private BoardCellController cellPrefab;
    [SerializeField] private Sprite flagSprite;
    [SerializeField] private Sprite mineSprite;

    public bool revealEntireMap;
    
    public event Action<int, int, ClickType> OnSelectCell;

    // Cache
    private SpriteRenderer[,] _icons;
    private TMP_Text[,] _texts;
    private Transform[,] _hidden;

    private BoardCellController[,] _cellControllers;
    
    public void DrawBoard(BoardData boardData)
    {
        _icons = new SpriteRenderer[boardData.BoardHeight,boardData.BoardWidth];
        _texts = new TMP_Text[boardData.BoardHeight,boardData.BoardWidth];
        _hidden = new Transform[boardData.BoardHeight,boardData.BoardWidth];
        _cellControllers = new BoardCellController[boardData.BoardHeight,boardData.BoardWidth];
        
        for (int y = 0; y < boardData.BoardHeight; y++)
        {
            for (int x = 0; x < boardData.BoardWidth; x++)
            {
                var cell = Instantiate(cellPrefab);
                if (cell.transform.Find("iconSprite").TryGetComponent<SpriteRenderer>(out var r)) _icons[y, x] = r;
                if (cell.transform.Find("mineIndicatorText").TryGetComponent<TMP_Text>(out var t)) _texts[y, x] = t;
                _hidden[y, x] = cell.transform.Find("hiddenSprite");
                if (cell.TryGetComponent<BoardCellController>(out var c))
                {
                    _cellControllers[y, x] = cell;
                    c.Init(x,y);
                    c.OnClickCell += OnClickCell;
                }

                cell.transform.position = new Vector3( -boardData.BoardWidth * 0.5f + x,
                    -boardData.BoardHeight * 0.5f + y, 0);
            }
        }

    }

    private void OnDestroy()
    {
        foreach (var c in _cellControllers)
        {
            c.OnClickCell -= OnClickCell;
        }
    }

    public void OnClickCell(int x, int y, ClickType clickType)
    {
        OnSelectCell?.Invoke(x, y, clickType);
    }

    public void RenderBoard(BoardData boardData, MinesweeperCell[,] board)
    {
        for (int y = 0; y < boardData.BoardHeight; y++)
        {
            for (int x = 0; x < boardData.BoardWidth; x++)
            {

                MinesweeperCell cell = board[y, x];
                
                SetupVisualNode(cell, x, y);
            }
        }

    }

    //TODO: Optimize
    private void SetupVisualNode(MinesweeperCell cell, int x, int y)
    {
        if (cell.IsRevealed || revealEntireMap)
        {
            // Node is revealed
            _hidden[y, x].gameObject.SetActive(false);

            if (cell.IsMine && !cell.IsFlagged)
            {
                _texts[y, x].gameObject.SetActive(false);
                _icons[y, x].gameObject.SetActive(true);
                _icons[y, x].sprite = mineSprite;
            }
            //else if (mapGridObject.isFlagged)
            //{
            //    indicatorText.gameObject.SetActive(false);
            //    iconSpriteRenderer.gameObject.SetActive(true);
            //    iconSpriteRenderer.sprite = flagSprite;
            //}
            else if (cell.NearbyMineCount == 0)
            {
                _texts[y, x].gameObject.SetActive(false);
                _icons[y, x].gameObject.SetActive(false);
            }
            else
            {
                _texts[y, x].gameObject.SetActive(true);
                _icons[y, x].gameObject.SetActive(false);
                _texts[y, x].text = cell.NearbyMineCount.ToString();
            }
        }
        else
        {
            // Node is hidden
            if (cell.IsFlagged)
            {
                _hidden[y, x].gameObject.SetActive(false);
                _texts[y, x].gameObject.SetActive(false);
                _icons[y, x].gameObject.SetActive(true);
                _icons[y, x].sprite = flagSprite;
            }
            else
            {
                _hidden[y, x].gameObject.SetActive(true);
                _icons[y, x].gameObject.SetActive(false);
            }
        }
    }
}