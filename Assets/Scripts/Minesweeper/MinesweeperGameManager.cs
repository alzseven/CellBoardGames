using System;
using UnityEngine;

namespace Minesweeper
{
    public class MinesweeperGameManager : MonoBehaviour
    {
        private MinesweeperBoardManager boardManager;
        [SerializeField] private MinesweeperBoardData boardData;
        [SerializeField] private MinesweeperRenderer _minesweeperRenderer;
        public bool isGameStarted;
        public float gameTime;
        public bool isGameEnded;

        private void Awake()
        {
            boardManager = new MinesweeperBoardManager(boardData);
            
            // boardManager.ClearGame();
        }

        private void OnEnable()
        {
            BoardCellController.OnClickAnyCell += OnClickCell;
            
        }

        private void OnDisable()
        {
            BoardCellController.OnClickAnyCell -= OnClickCell;
        }

        private void Start()
        {
            boardManager.OnBoardChanged += _minesweeperRenderer.RenderBoard;
            boardManager.OnGameEnded += EndGame;
            
            _minesweeperRenderer.DrawBoard(boardData);
            // boardManager.Start();
        }

        private void OnDestroy()
        {
            boardManager.OnBoardChanged -= _minesweeperRenderer.RenderBoard;
            boardManager.OnGameEnded -= EndGame;
        }

        //TODO: Separate init and reveal/flag/or something logic
        // TODO: GM as SingleTon?????
        /// <summary>
        /// Handles a click on a cell in the game board.
        /// </summary>
        /// <param name="x">The x-coordinate of the clicked cell.</param>
        /// <param name="y">The y-coordinate of the clicked cell.</param>
        /// <param name="clickType">The type of click that was performed.</param>
        private void OnClickCell(int x, int y, ClickType clickType)
        {
            if(isGameEnded) return;

            switch (clickType)
            {
                case ClickType.LEFT:
                    // Check if the game has started before planting mines
                    // This prevents the player from accidentally planting mines
                    // on the first click of the game
                    if (!isGameStarted)
                    {
                        boardManager.PlantMines(x, y, boardData.MineAmount);
                        isGameStarted = true;
                    }
                
                    // Reveal the clicked cell
                    boardManager.RevealCell(x, y);
                    break;
                case ClickType.WHEEL:
                    // Reveal all nearby cells
                    boardManager.RevealNearbyCells(x, y);
                    break;
                case ClickType.RIGHT:
                    // Toggle the flag on the clicked cell
                    boardManager.ToggleFlag(x, y);
                    break;
                default:
                    // Log an error if the click type is not recognized
                    // TODO: Remove on build
                    Debug.Log("Undefined Input");
                    break;
            }
        }
        
        
        // TODO: Start Game when First click
        // TODO: BoardManager should init when isGameStarted Set true?
        public void StartGame()
        {
            isGameStarted = true;
        }
    

        public void EndGame(bool isWon)
        {
            isGameEnded = true;
        }
    
        private void Update()
        {
            //TODO: Pause
            if (!isGameStarted || isGameEnded) return;

            gameTime += Time.deltaTime;
        }

        public void ReStart()
        {
            gameTime = 0;
            isGameStarted = false;
            isGameEnded = false;
            boardManager?.Restart();
        }

    
    
    
    }
}
