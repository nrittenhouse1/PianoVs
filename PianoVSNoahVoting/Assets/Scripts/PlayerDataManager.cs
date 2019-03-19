﻿///Author: Noah Rittenhouse
///This script will be attached to each of the keyboard prefabs and will handle any and all player data, score/AI bool, etc
///Last Modified By: Noah Rittenhouse
///Last Modified Date: Feb-19-2019
///Dependencies: Score displays must be assigned in the editor
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataManager : MonoBehaviour
{
    public bool isPlayer3Or4, isRecording, isAI;

    float playerScore, playerMultiplier;//Ints that hold the players score and multiplier
    float holdingScore;//Float that holds score from holding note so it can be rounded for the display
    int multiplierCount;//Variable that keeps track of how many good hits the player has gotten so far
    [SerializeField]
    int comboCount;//Variable that keeps track of how many good hits the player needs    

    [SerializeField]
    Text playerScoreDisplay;

    void Start()
    {
        playerScore = 0;
        holdingScore = 0;

        multiplierCount = 0;
        playerMultiplier = 1;
        isAI = false;
        ToggleAI();
        isRecording = true;
        ToggleRecording();
        if (isPlayer3Or4)
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.player3or4 = true;
            }
        }
        else
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.player3or4 = false;
            }
        }
        if (comboCount <= 0)
        {
            comboCount = 10;//Defaults to 10 good hits needed
        }


        #region Set KeyNumbers
        int keyNum = 0;
        foreach (GameObject key in GameObject.FindGameObjectsWithTag("Key"))
        {
            key.GetComponent<IndividualKeyScript>().keyNum = keyNum;
            keyNum++;
            if (keyNum == 49)
            {
                keyNum = 0;
            }
        }

        #endregion
    }

    void Update()
    {
        try
        {
            playerScoreDisplay.text = "Score " + Mathf.RoundToInt(playerScore + holdingScore) + "\n" + "Multiplier " + playerMultiplier;
        }
        catch
        {

        }
        if (isRecording)
        {
            SongRecording.SongPlaying();
        }
    }

    public void AddScore(float scoreToAdd)//This method is called when a note is hit by the player and simply adds an amount decided by how timely they hit it
    {
        playerScore += scoreToAdd * playerMultiplier;
    }
    public void AddHoldingScore(float scoreToAdd)
    {
        holdingScore += scoreToAdd;
    }
    public void IncreaseMultiplier(int amount)//Increases multiplier every time they get a good hit
    {
        if (playerMultiplier < 8)
        {
            multiplierCount += amount;
            if (multiplierCount >= comboCount)
            {
                multiplierCount = 0;
                playerMultiplier++;
            }
        }
    }
    public void BreakMultiplier()//Reset the multiplier variables if the player messes up
    {
        multiplierCount = 0;
        playerMultiplier = 1;
    }

    public void ToggleAI()
    {
        isAI = !isAI;   
        if (isAI)
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.AI = true;
                keyScript.StopNote();
            }
        }
        else
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.AI = false;
                keyScript.StopNote();
            }
        }
    }
    public void ToggleRecording()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.songrecording = true;
            }
        }
        else
        {
            foreach (IndividualKeyScript keyScript in transform.GetComponentsInChildren<IndividualKeyScript>())
            {
                keyScript.songrecording = false;
            }
        }
    }
}
