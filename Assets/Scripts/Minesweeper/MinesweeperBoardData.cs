using UnityEngine;

namespace Minesweeper
{
    [CreateAssetMenu(fileName = "MsBoardData", menuName = "Data/MinesweeperBoardData", order = 11)]
    public class MinesweeperBoardData : BoardData
    {
        [SerializeField] private int _mineAmount;

        public int MineAmount => _mineAmount;
    }
}