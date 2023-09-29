using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Minesweeper
{
    public class MinesweeperBoardManager : MonoBehaviour
    {
        [SerializeField] private BoardData _boardData;
        [SerializeField] private MinesweeperRenderer _msRenderer;
        [SerializeField] private int _mineAmount;
        private MinesweeperCell[,] _minesweeperBoard;


        private int _flagCount;

        public bool isGameStarted;

        private void Awake()
        {
            ClearGame();
        }

        private void OnEnable()
        {
            _msRenderer.OnSelectCell += OnClickCell;
        }

        private void OnDisable()
        {
            _msRenderer.OnSelectCell -= OnClickCell;
        }

        private void Start()
        {
            _msRenderer.DrawBoard(_boardData);
        
        }
    
        /// <summary>
        /// Clears the game board.
        /// Sets the game started flag to false and creates a new minesweeper board.
        /// </summary>
        private void ClearGame()
        {
            // Set the game started flag to false.
            isGameStarted = false;
        
            // Create a new minesweeper board.
            _minesweeperBoard = new MinesweeperCell[_boardData.BoardHeight, _boardData.BoardWidth];
        
            // Iterate over the board and initialize each cell.
            for (int y = 0; y < _boardData.BoardHeight; y++)
            {
                for (int x = 0; x < _boardData.BoardWidth; x++)
                {
                    _minesweeperBoard[y, x] = new MinesweeperCell();
                }
            }
        }

        /// <summary>
        /// Restart the game.
        /// This function clears the game board and renders it again.
        /// </summary>
        public void Restart()
        {
            // Clear the game board.
            ClearGame();
        
            // Render the game board.
            _msRenderer.RenderBoard(_boardData, _minesweeperBoard);
        }
    
        /// <summary>
        /// Handles a click on a cell in the game board.
        /// </summary>
        /// <param name="x">The x-coordinate of the clicked cell.</param>
        /// <param name="y">The y-coordinate of the clicked cell.</param>
        /// <param name="clickType">The type of click that was performed.</param>
        private void OnClickCell(int x, int y, ClickType clickType)
        {
            switch (clickType)
            {
                case ClickType.LEFT:
                    // Check if the game has started before planting mines
                    // This prevents the player from accidentally planting mines
                    // on the first click of the game
                    if (!isGameStarted)
                    {
                        PlantMines(x, y, _mineAmount);
                        isGameStarted = true;
                    }
                
                    // Reveal the clicked cell
                    RevealCell(x, y);
                    break;
                case ClickType.WHEEL:
                    // Reveal all nearby cells
                    RevealNearbyCells(x, y);
                    break;
                case ClickType.RIGHT:
                    // Toggle the flag on the clicked cell
                    ToggleFlag(x, y);
                    break;
                default:
                    // Log an error if the click type is not recognized
                    // TODO: Remove on build
                    Debug.Log("Undefined Input");
                    break;
            }
        }

        /// <summary>
        /// Plant mines on the board.
        /// </summary>
        /// <param name="firstX"> The x-coordinate of the first cell. </param>
        /// <param name="firstY"> The y-coordinate of the first cell. </param>
        /// <param name="mineAmount"> The number of mines to plant. </param>
        private void PlantMines(int firstX, int firstY, int mineAmount)
        {
            // Create a HashSet to keep track of which cells have already been mined.
            // This will prevent us from placing a mine on the same cell twice.
            HashSet<(int X, int Y)> mineCoordinates = new HashSet<(int X, int Y)>();
        
            // Keep track of how many mines have been placed.
            int minesPlaced = 0;
        
            // While we haven't placed all of the mines,
            // keep generating random coordinates and placing mines there.
            while (minesPlaced < mineAmount)
            {
                // Generate random x and y coordinates.
                int x = Random.Range(0, _boardData.BoardWidth);
                int y = Random.Range(0, _boardData.BoardHeight);

                // If we're trying to place a mine on the first cell, skip it.
                if (x == firstX && y == firstY) continue;

                // If the mine coordinates set doesn't already contain the current coordinates,
                // place a mine at those coordinates
                // and update the nearby mine counts of all of its neighbors.
                if (!mineCoordinates.Contains((x, y)))
                {
                    _minesweeperBoard[y, x].IsMine = true;
                    minesPlaced++;

                    mineCoordinates.Add((x, y));

                    foreach (var (nX, nY) in GetNeighbors(x, y))
                    {
                        if (!_minesweeperBoard[nY, nX].IsMine) _minesweeperBoard[nY, nX].NearbyMineCount += 1;
                    }
                }
            }
        }
    
        /// <summary>
        /// Toggle the flag on the cell at the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        private void ToggleFlag(int x, int y)
        {
            // Get the cell at the specified coordinates.
            MinesweeperCell cell = _minesweeperBoard[y, x];
        
            // If the cell is already revealed, do nothing.
            if (cell.IsRevealed) return;
        
            // Toggle the cell's flagged state.
            cell.IsFlagged = !cell.IsFlagged;
        
            // Update the flag counter.
            _flagCount = cell.IsFlagged ? _flagCount - 1 : _flagCount + 1;
        
            // Render the board.
            _msRenderer.RenderBoard(_boardData, _minesweeperBoard);
        }

        /// <summary>
        /// Reveals the cell at the specified coordinates.
        /// </summary>
        /// <param name="x"> The x-coordinate of the cell. </param>
        /// <param name="y"> The y-coordinate of the cell. </param>
        private void RevealCell(int x, int y)
        {
            MinesweeperCell cell = _minesweeperBoard[y, x];

            // Check if the cell is already revealed or flagged.
            if (cell.IsRevealed || cell.IsFlagged) return;
        
            // If the cell is a mine,
            // reveal the whole board and lose.
            if (cell.IsMine)
            {
                RevealWholeBoard(false);
                //TODO: Game End!
            }
            else
            {
                // If the cell is not a mine and has no nearby mines,
                // reveal all nearby empty cells.
                if (cell.NearbyMineCount == 0)
                {
                    RevealNearbyEmptyCells(x, y);
                }
                // Otherwise, simply reveal the cell.
                else
                {
                    cell.IsRevealed = true;
                }
            }

            // Check if the game is won.
            if (IsGameWon())
            {
                RevealWholeBoard(true);
                // TODO: Win
            }

            // Render the board.
            _msRenderer.RenderBoard(_boardData, _minesweeperBoard);
        }
    
        /// <summary>
        /// Recursively reveals all empty cells nearby.
        /// </summary>
        /// <param name="centerX"> The x-coordinate of center cell. </param>
        /// <param name="centerY"> The y-coordinate of center cell. </param>
        /// <param name="checkedCells"> The list of already checked cells. </param>
        private void RevealNearbyEmptyCells(int centerX, int centerY, HashSet<(int, int)> checkedCells = null)
        {
            // Null-conditional assignment operator to ensure that checkedCells is never null
            checkedCells ??= new HashSet<(int, int)>();

            // Check if the cell has already been checked to avoid infinite recursion
            if (checkedCells.Contains((centerY,centerX))) return;

            // Check if the cell is outside of the grid
            if (IsUnavailableGridPos(centerX, centerY)) return;
        
            // Add the cell to the list of checked cells
            checkedCells.Add((centerY,centerX));
        
            // Get the current cell from the minesweeper board
            var currentCell = _minesweeperBoard[centerY, centerX];
        
            // Reveal the cell if it is not already revealed
            if (!currentCell.IsRevealed) currentCell.IsRevealed = true;
        
            // Unflag the cell if it is flagged
            if (currentCell.IsFlagged) currentCell.IsFlagged = false;
        
            // If the cell has no nearby mines, recursively reveal all of its neighbors
            if (currentCell.NearbyMineCount != 0) return;
            foreach (var (nX, nY) in GetNeighbors(centerX, centerY))
            {
                RevealNearbyEmptyCells(nX, nY, checkedCells);
            }
        }

        /// <summary>
        /// If number of nearby flagged cells are equal to center cell's nearbyMineNumber,
        /// then reveal every non-flagged cells nearby center cell.
        /// </summary>
        /// <param name="centerX"> The x-coordinate of center cell. </param>
        /// <param name="centerY"> The y-coordinate of center cell. </param>
        private void RevealNearbyCells(int centerX, int centerY)
        {
            // Get the center cell.
            var centerCell = _minesweeperBoard[centerY, centerX];

            // If the center cell is already revealed or has no nearby mines, then return.
            if (!centerCell.IsRevealed || centerCell.NearbyMineCount == 0) return;
        
            // Count the number of nearby flagged cells.
            int flagCnt = 0;
            foreach (var (x, y) in GetNeighbors(centerX, centerY))
            {
                if (_minesweeperBoard[y, x].IsFlagged) flagCnt++;
            }

            // If the number of nearby flagged cells
            // is not equal to the center cell's nearbyMineNumber,
            // then return.
            if (centerCell.NearbyMineCount != flagCnt) return;
        
            // Reveal every non-flagged cell nearby the center cell.
            foreach (var (x, y) in GetNeighbors(centerX, centerY))
            {
                if (!_minesweeperBoard[y, x].IsFlagged) RevealCell(x,y);
            }
        
            // Render the minesweeper board.
            _msRenderer.RenderBoard(_boardData, _minesweeperBoard);
        }
    
        /// <summary>
        /// Returns a list of all the neighbors of a given cell in a grid.
        /// The neighbors are defined as the 8 cells that are adjacent to the given cell.
        /// </summary>
        /// <param name="centerX"> The x-coordinate of the center cell. </param>
        /// <param name="centerY"> The y-coordinate of the center cell. </param>
        /// <returns> A list of all the neighbors of the given cell. </returns>
        private IEnumerable<(int x, int y)> GetNeighbors(int centerX, int centerY)
        {
            // Iterate over all the neighbors of the center cell.
            for (int x = centerX - 1; x <= centerX + 1; x++)
            {
                for (int y = centerY - 1; y <= centerY + 1; y++)
                {
                    // If the neighbor is out of bounds, skip it.
                    if (IsUnavailableGridPos(x, y)) continue;
                
                    // Yield the neighbor.
                    yield return (x, y);
                }
            }
        }

        /// <summary>
        /// Check if the coordinates are outside of the grid.
        /// </summary>
        /// <param name="x"> The x-coordinate to check. </param>
        /// <param name="y"> The y-coordinate to check. </param>
        /// <returns></returns>
        private bool IsUnavailableGridPos(int x, int y)
        {
            return (x < 0 || x >= _boardData.BoardWidth || y < 0 || y >= _boardData.BoardHeight);
        }
    
        /// <summary>
        /// Check if there are any unrevealed cells that are not mines.
        /// </summary>
        private bool IsGameWon()
        {
            foreach (MinesweeperCell mapGridObject in _minesweeperBoard)
            {
                if (!mapGridObject.IsMine && !mapGridObject.IsRevealed)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reveals the entire minesweeper board,
        /// either because the game has been won or lost.
        /// </summary>
        /// <param name="isGameWon"> A boolean value indicating whether the game has been won. </param>
        private void RevealWholeBoard(bool isGameWon)
        {
            foreach (var cell in _minesweeperBoard)
            {
                //if game won
                if (isGameWon)
                {
                    //if cell is mine
                    if (cell.IsMine)
                    {
                        //flag cell
                        cell.IsFlagged = true;
                    }
                    //if cell is not mine
                    else
                    {
                        //reveal cell
                        cell.IsRevealed = true;
                    }
                }
                //if game lose
                else
                {
                    //if cell is mine
                    if (cell.IsMine)
                    {
                        //and not flagged
                        if (!cell.IsFlagged)
                            //reveal mine
                            cell.IsRevealed = true;
                    }
                    // TODO: Highlight wrong-flagged cell?
                }
            }
        
            // Render the game board.
            _msRenderer.RenderBoard(_boardData, _minesweeperBoard);
        }
    
    }
}