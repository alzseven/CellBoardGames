using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Minesweeper
{
    public enum ClickType
    {
        LEFT,
        WHEEL,
        RIGHT,
    }
    
    public class BoardCellController : MonoBehaviour
    {
        private int posX, posY;
        public static event Action<int, int, ClickType> OnClickAnyCell = delegate {  };
        
        public void Init(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public void OnClick(ClickType clickType) => OnClickAnyCell(posX, posY, clickType);
    }
}