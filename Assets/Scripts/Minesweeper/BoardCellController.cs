using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ClickType
{
    LEFT,
    WHEEL,
    RIGHT,
}

public class BoardCellController : MonoBehaviour
{
    private int posX, posY;
    public event Action<int, int, ClickType> OnClickCell = delegate{  };

    public void Init(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public void OnClick(ClickType clickType) => OnClickCell(posX, posY, clickType);
    
}
