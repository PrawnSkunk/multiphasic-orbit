﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	
	// Tracks if mouse is hovering over panel
	public Transform planetsParent;
	private bool mouseHover;
	private GameController gc; // Access Game Controller script
	public bool ifhover;
	//add a collider for the object
	public BoxCollider bc;
	// Following code is generated by right clicking IPointerEnter/ExitHandler, and selecting "Refactor"
	public void OnPointerEnter (PointerEventData eventData)
	{
		mouseHover = true;

	}
	public void OnPointerExit (PointerEventData eventData)
	{
		mouseHover = false;
	}

	void Start()
	{
		gc = GameObject.Find("Game Manager").GetComponent<GameController>();
		bc=gameObject.GetComponent<BoxCollider>();
	}

	void Update () {
		// When player clicks outside the panel
		if (Input.GetMouseButtonUp (0) && mouseHover) {

			// Actually on second thought, let the simulation run first before updating the game state...
			if (gc.GAME_STATE == Constants.TURN_1_END_TURN) {
				//gc.GAME_STATE = Constants.TURN_1_WATCH_SIMULATION;
			}
			if (gc.GAME_STATE == Constants.TURN_2_END_TURN) {
				//gc.GAME_STATE = Constants.TURN_2_WATCH_SIMULATION;
			}
			mouseHover = false;
		}
	}



}
