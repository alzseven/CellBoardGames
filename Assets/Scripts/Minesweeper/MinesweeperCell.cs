using UnityEngine;

namespace Content.Minesweeper
{
    public class MinesweeperCell
    {
        public bool IsMine;
        public bool IsRevealed;
        public int NearbyMineCount;
        public bool IsFlagged;

        public MinesweeperCell()
        {
            IsMine = false;
            IsRevealed = false;
            NearbyMineCount = 0;
            IsFlagged = false;
        }
    }
}