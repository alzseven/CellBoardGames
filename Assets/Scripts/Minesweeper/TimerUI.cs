using System.Globalization;
using TMPro;
using UnityEngine;

namespace Minesweeper
{
    public class TimerUI : MonoBehaviour
    {
        public TMP_Text timerText;
        public MinesweeperGameManager minesweeperGameManager;

        //TODO:Format String
        private void Update()
        {
            if (timerText)
                timerText.text = minesweeperGameManager.gameTime.ToString(CultureInfo.CurrentCulture);
        }
    }
}