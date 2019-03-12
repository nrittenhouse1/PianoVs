﻿///Author: Noah Rittenhouse
///This script will handle all the menu button presses and what they do
///Last Modified By: Noah Rittenhouse
///Last Modified Date: 5-Mar-2019
///Dependencies: 
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuHandler : MonoBehaviour
{
    bool doOnce;//Bool just for a fake start method to only play once
    bool menuStatus, toggleTimerStatus, votingPanelStatus, waitingMessageStatus, voted;
    float toggleTimer;
    int currentSongID;

    GameObject menuPanel, menuToggleText;
    GameObject dropdownMenu, votingText, votePanel, waitingMessage;
    [SerializeField]
    GameObject noteSpawner;
    string songName;
    List<string> songNames;
    void Start()
    {
        songName = "default";
        menuStatus = false;
        toggleTimerStatus = false;
        votingPanelStatus = false;
        waitingMessageStatus = false;
        voted = false;
        toggleTimer = 0.5f;
        doOnce = true;
        currentSongID = GameDataManager.GetCurrentSongID();
        songNames = new List<string>();
        //Find children assets
        menuPanel = transform.Find("MenuPanel").gameObject;
        menuToggleText = transform.Find("MenuToggle/Countdown").gameObject;
        dropdownMenu = transform.Find("MenuPanel/Dropdown").gameObject;
        votingText = transform.Find("MenuPanel/VotingStuff/VotePanel/VoteText").gameObject;
        votePanel = transform.Find("MenuPanel/VotingStuff/VotePanel").gameObject;
        waitingMessage = transform.Find("MenuPanel/VotingStuff/WaitingMessage").gameObject;
        //

    }
    void Update()
    {

        if (doOnce)//Do once...clear dropdown, and for every song add that songs name to the dropdown list
        {
            dropdownMenu.GetComponent<Dropdown>().ClearOptions();
   
            for (int i = 0; i < noteSpawner.GetComponent<NoteSpawning>().songs.Count - 1; i++)
            {
                songNames.Add(noteSpawner.GetComponent<NoteSpawning>().songs[i].GetSongName());
            }
            dropdownMenu.GetComponent<Dropdown>().AddOptions(songNames);
            doOnce = false;
        }



        menuPanel.SetActive(menuStatus);//Either enable or disable the menu

        waitingMessage.SetActive(waitingMessageStatus);
        votePanel.SetActive(votingPanelStatus);//Close vote panel

        if (toggleTimerStatus)//if they are holding the button decrement the timer variable
        {
            toggleTimer -= Time.deltaTime;
        }
        menuToggleText.GetComponent<Text>().text = toggleTimer.ToString();//Just a display that shows how long they have pressed

        if (GameDataManager.voteInProgress && !voted)//If there is a vote occurring and you havent voted yet make sure the menu is on
        {
            VotingActive();
        }
        else if (!GameDataManager.voteInProgress)//If there is no vote disable shit
        {
            VotingInactive();
        }
        currentSongID = GameDataManager.GetCurrentSongID();
        dropdownMenu.GetComponent<Dropdown>().value = currentSongID;
    }

    #region Menu Activation
    public void PressedMenuToggle()//When the player presses the menu button begin a timer, or if the menu is already open then just close it
    {
        if (!menuStatus)
        {
            toggleTimerStatus = true;//Start timer
        }
        else
        {
            menuStatus = false;//Close menu
        }
    }
    public void StoppedPressingMenuToggle()//Once the player lets go of the button, if they pressed it for over a second, turn the menu on
    {
        toggleTimerStatus = false;//Stop timer
        if (toggleTimer <= 0)
        {
            menuStatus = true;//Open menu
        }
        toggleTimer = 0.5f;//Reset timer
    }
    #endregion

    #region Difficulty Selection

    #endregion


    #region Song Selection
    public void SelectSong()
    {
        if (!GameDataManager.voteInProgress)//If there is not already a vote going on...
        {
            //Debug.Log("Ooga");
            int songID = dropdownMenu.GetComponent<Dropdown>().value;//Set the players choice as the songID
            if (songID != currentSongID)//if the players choice is not already playing...
            {
                currentSongID = songID;
                GameDataManager.voteInProgress = true;//Let the game know there is now a vote active
                GameDataManager.SetNewSongID(songID);//Let the game know the ID of the voted song
                VotingActive();
            }
        }
    }

    public void SubmitVote(bool vote)
    {
        //true = yes
        //false = no
        if (vote)//Yes
        {
            GameDataManager.IncrementVote(1);
        }
        else//No
        {
            GameDataManager.IncrementVote(-1);
        }
        menuStatus = false;//Close menu
        voted = true;//They have voted
        waitingMessageStatus = true;//Open message
        votingPanelStatus = false;//Close panel
    }

    public void VotingActive()
    {
        votingText.GetComponent<Text>().text = "Switch to " + GameDataManager.GetSongName();
        votingPanelStatus = true;
        dropdownMenu.GetComponent<Dropdown>().interactable = false;
    }
    public void VotingInactive()
    {
        dropdownMenu.GetComponent<Dropdown>().interactable = true;
        voted = false;
        waitingMessageStatus = false;//Open message
    }
    #endregion
}
