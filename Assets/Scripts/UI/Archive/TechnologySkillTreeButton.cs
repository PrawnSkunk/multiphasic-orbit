﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Track events through event systems
using UnityEngine.EventSystems;

// Implement pointer interfaces programatically, instead of Scene event triggers
public class TechnologySkillTreeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public GameObject microSkillTree; // Reference to macro skill tree
	private bool mouseHover; // Tracks if mouse is hovering over panel
	private GameController gc; // Access Game Controller script

	// Following code is generated by right clicking IPointerEnter/ExitHandler, and selecting "Refactor"
	public void OnPointerEnter (PointerEventData eventData) {
		mouseHover = true;
	}
	public void OnPointerExit (PointerEventData eventData) {
		mouseHover = false;
	}

	void Start()
	{
		gc = GameObject.Find("Game Manager").GetComponent<GameController>();
	}

	void Update () {
		// When player clicks on the icon
		if (Input.GetMouseButtonUp (0) && mouseHover && gc.GAME_STATE >= Constants.TURN_3_TECH_TREE) {
			// Toggle the panel
			microSkillTree.SetActive (!microSkillTree.activeSelf);

			// // Handle player clicking skill tree when skill tree is open
			// if (gc.GAME_STATE == Constants.TURN_3_TECH_SLOT) {
			// 	gc.GAME_STATE = Constants.TURN_3_TECH_TREE;
			// }
			// // Switch the hint indicator
			// if (microSkillTree.activeSelf && gc.GAME_STATE == Constants.TURN_3_TECH_TREE) {
			// 	gc.GAME_STATE = Constants.TURN_3_TECH_SLOT;
			// }
		}
	}
}
