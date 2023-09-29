using UnityEngine;

namespace Minesweeper
{
    [CreateAssetMenu(fileName = "BoardData", menuName = "Data/BoardData", order = 0)]
    public class BoardData : ScriptableObject
    {
        [SerializeField] private int _boardWidth;
        [SerializeField] private int _boardHeight;


        public int BoardWidth => _boardWidth;
        public int BoardHeight => _boardHeight;
    }
}