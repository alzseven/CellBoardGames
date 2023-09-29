﻿using System.Globalization;
using TMPro;
using UnityEngine;

namespace Minesweeper
{
    public class TimerUI : MonoBehaviour
    {
        public TMP_Text timerText;
        public GameManager gameManager;

        //TODO:Format String
        private void Update()
        {
            if (timerText)
                timerText.text = gameManager.gameTime.ToString(CultureInfo.CurrentCulture);
        }
    }
}