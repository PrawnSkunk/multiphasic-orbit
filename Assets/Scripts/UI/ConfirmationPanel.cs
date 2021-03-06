﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    private GameController gc;
    public GameObject panel;
    public Text titleText;
    public Text messageText;
    public Button confirmButton;

    private void Awake()
    {
        // GameController
        gc = GameObject.Find("Game Manager").GetComponent<GameController>();
        gc.cp = this;
    }

    private void Start()
    {
        confirmButton.onClick.AddListener(ClosePanel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.Space)))
        {
            ClosePanel();
        }
    }

    public void ShowPanel(string title, string message)
    {
        panel.SetActive(true);
        titleText.text = title;
        messageText.text = message;
    }

    public void ShowPanel(string title, string message, string reward)
    {
        panel.SetActive(true);
        titleText.text = title;
        messageText.text = message;
        messageText.text += "\r\n\r\n" + "<b>Reward: " + reward + "</b>";
    }

    private void ClosePanel()
    {
        panel.SetActive(false);
        
        // Update game state on panel close
        //Debug.Log(gc.GAME_STATE);
        //Debug.Log("turn " + gc.turn);

        if (GameController.level == 1 && gc.GAME_STATE == Constants.LEARNERS_MISSION_1) gc.GAME_STATE = Constants.LEARNERS_MISSION_2;
        if (GameController.level == 1 && gc.GAME_STATE == Constants.LEARNERS_MISSION_3 && gc.turn == 3 && gc.canBuild) gc.GAME_STATE = Constants.LEARNERS_MISSION_5;
        if (GameController.level == 1 && gc.GAME_STATE == Constants.LEARNERS_MISSION_5_PLAY && gc.turn == 4 && gc.canBuild) gc.GAME_STATE = Constants.LEARNERS_MISSION_6;
        if (GameController.level == 1 && gc.GAME_STATE == Constants.LEARNERS_MISSION_6_PLAY && gc.canBuild) gc.GAME_STATE = Constants.LEARNERS_MISSION_7;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }

    public void Final()
    {
        SceneManager.LoadScene("Final");
    }

    public void CheckMissions()
    {
        gc.m.CheckMissions(gc.m.missions);
                if (GameController.level == 1 && gc.GAME_STATE == Constants.LEARNERS_MISSION_3 && gc.turn == 2 && gc.canBuild) gc.GAME_STATE = Constants.LEARNERS_MISSION_4;
    }

    public void Title()
    {
        SceneManager.LoadScene("title");
    }
}
