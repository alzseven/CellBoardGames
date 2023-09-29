using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public MinesweeperBoardManager minesweeperBoard;
    public bool isGameStarted;
    public float gameTime;
    public bool isGameEnded;


    // TODO: Start Game when First click
    // TODO: BoardManager should init when isGameStarted Set true?
    public void StartGame()
    {
        isGameStarted = true;
    }
    

    public void EndGame()
    {
        isGameEnded = true;
    }
    
    private void Update()
    {
        if (!isGameStarted || isGameEnded) return;

        gameTime += Time.deltaTime;
    }

    public void ReStart()
    {
        gameTime = 0;
        isGameStarted = false;
        if(minesweeperBoard != null)
            minesweeperBoard.Restart();
    }

    
    
    
}
