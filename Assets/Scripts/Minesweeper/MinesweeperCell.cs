namespace Minesweeper
{
    public class MinesweeperCell
    {
        public bool IsMine;
        public bool IsRevealed;
        public int NearbyMineCount;
        public bool IsFlagged;

        public MinesweeperCell(bool isMine, bool isRevealed, int nearbyMineCount, bool isFlagged)
        {
            IsMine = isMine;
            IsRevealed = isRevealed;
            NearbyMineCount = nearbyMineCount;
            IsFlagged = isFlagged;
        }
    }
}