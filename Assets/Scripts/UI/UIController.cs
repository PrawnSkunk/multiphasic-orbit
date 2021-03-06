﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// -- Create UI Controller
// private UIController ui;
// ui = GameObject.Find ("Canvas").GetComponent<UIController> ();

// -- Update the UI when needed.
// ui.UpdateSelectedPlanet();

// -- Update just the health.
// ui.UpdateHealth();

// -- Set Phase
// ui.SetPhase("Planning");
// ui.SetPhase("Choose a planet for " + p.name + " to link with");

public class UIController : MonoBehaviour 
{
	/* Declare Top Panel
	   ========================================================================== */
	// Turn Panel
	public Text topTurnText;
    public Text topPhaseText;
	// Distance Panel
	public Text topDistanceText;

	/* Declare Left Panel
	   ========================================================================== */
    public RectTransform leftPanel;
	// Selection Panel
    public RectTransform leftSelectionPanel;
	public Text leftLeftText;
	public Text leftNameText;
	public Text leftRightText;
	// Preview Panel
    public RectTransform leftPreviewPanel;
	public Image leftPreviewImage;
	// Resource Panel
	public Text leftStoneText;
	public Text leftWaterText;
	public Text leftGasText;
    public RectTransform leftResourcePanel;
    public Text leftStoneCollectedText;
    public Text leftWaterCollectedText;
    public Text leftGasCollectedText;
	// Status Panel
	public Text leftPopulationText;
	public Text leftHealthText;
    public RectTransform leftStatusPanel;

    // Technology Panel
    public RectTransform leftTechnologyPanel;
    private Button[] _technologyButtons; // Keep private
    private Text[] _technologyText; // Keep private
    public RectTransform leftLinkToButton;

    [HideInInspector]
    public Planet selectedPlanet; // Keep private
    // Building Panel
    public RectTransform leftBuildingPanel;
    public Button leftBuildingButton;
    public Text leftBuildingText;
    // Helper variables
    [HideInInspector]
    public Planet previousPlanet;
    public Button leftTechnology1Button;
    public Button leftTechnology2Button;
    public Button leftTechnology3Button;
    public Button leftTechnology4Button;
    public Button leftTechnology5Button;
    private bool fadeCoroutineRunning;
    public Button leftAttackToButton;

	/* Declare Right Panel
	   ========================================================================== */
    // Action Panel
    public Button rightNextTurnButton;
    public Button rightStoneButton;
    public Button rightWaterButton;
    public Button rightGasButton;
    public Button rightMission1Button;
    public Button rightMission2Button;
    public Button rightMission3Button;

	/* Declare Tooltips
	   ========================================================================== */
    public GameObject tooltipContainer;

    /* Declare Hints
	   ========================================================================== */
    public GameObject hintContainer;

    /* Declare Orbital Plane
	   ========================================================================== */
    public RectTransform orbitalPlanes;

    /* Declare Audio Source
	   ========================================================================== */
    private AudioSource audioSource { get { return GetComponent<AudioSource>(); } }


    // GameController
    public GameController gc;

    private void Awake()
    {
        // GameController
        gc = GameObject.Find("Game Manager").GetComponent<GameController>();
        gc.ui = this;

        gameObject.AddComponent<AudioSource>();
    }

    private void Start()
	{
		/* Initialize Top Panel
	       ====================================================================== */
		SetTurn (1);
        SetPhase ("Planning");
		SetDistance (600);

		/* Initialize Left Panel
	       ====================================================================== */
		SetSelectedPlanet (null);

		/* Initialize Right Panel
		   ====================================================================== */
        SetPlayInteractive(false);

        /* Initialize External Scripts
           ====================================================================== */
        SetupTooltipController();
        SetupHintController();

        /* Initialize Orbital Plane
           ====================================================================== */
        SetupOrbitalPlane();
	}

	/* Set Top Panel
	   ========================================================================== */
	public void SetTurn (int turn = 1)
	{
		topTurnText.text = "Turn " + turn.ToString ();
	}

    public void SetPhase (string myString = "No Phase Set")
	{
		Text myHiddenParentText = topPhaseText.GetComponent<Text> ();
		Text myVisualChildText = topPhaseText.GetComponentsInChildren<Text> ()[1].GetComponent<Text>();
		myHiddenParentText.text = myVisualChildText.text = myString;
		topPhaseText.text = myString;
	}

	public void SetDistance (int distance = 20)
	{
		topDistanceText.text = distance.ToString() + " uu";
	}

	/* Set Left Panel
    ========================================================================== */

    public void SetSelectedPlanet (Planet p = null)
	{

		// Planet selected
		if (p != null) 
		{
            //Debug.Log("Planet selected");
            // Set global variables
            this.previousPlanet = this.selectedPlanet;
            this.selectedPlanet = p;
		} 
        else 
        {
            //Debug.Log("No planet selected");
        }

        // Update the planet
        UpdateSelectedPlanet();
	}

    public void UpdateSelectedPlanet ()
    {
        //Debug.Log("UpdateSelectedPlanet() called.");
        if (selectedPlanet != null) 
		{
			// Replace placeholder values with planet data
			SetSelectedName (selectedPlanet.name);
			SetSelectedPreview (selectedPlanet.planetSprite);
			SetSelectedResources (selectedPlanet.stone, selectedPlanet.water, selectedPlanet.gas);
            SetSelectedResourcesCollected(selectedPlanet.stoneCollected, selectedPlanet.waterCollected, selectedPlanet.gasCollected);
			SetSelectedPopulation (selectedPlanet.population);
			SetSelectedHealth (selectedPlanet.health, selectedPlanet.maxHealth);
            SetSelectedTechnology (selectedPlanet.technologyLevel);
            SetSelectedBuilding(selectedPlanet.population);

            // Enable last
            SetSelectionPanelActive(true);
            SetPreviewPanelActive(false);
            SetResourcePanelActive(true);
            SetStatusPanelActive(true);
            SetTechnologyPanelActive(selectedPlanet.population > 0);
            SetBuildingPanelActive(selectedPlanet.population <= 0);

            // GFX
            SetSelectedOrbit();
        }
        else 
        {
            // Disable First
            SetSelectionPanelActive();
            SetPreviewPanelActive();
            SetResourcePanelActive();
            SetStatusPanelActive();
            SetTechnologyPanelActive();
            SetBuildingPanelActive();

			// Replace placeholder values with default values
			SetSelectedName ();
			SetSelectedPreview ();
			SetSelectedResources ();
            SetSelectedResourcesCollected();
			SetSelectedPopulation ();
			SetSelectedHealth ();
            SetSelectedBuilding();

            // GFX
            SetSelectedOrbit();
        }
    }

    public void SetNoPlanetSelected()
    {
        SetSelectionPanelActive(true);
        SetPreviewPanelActive(true);
        // Play animation
        OpenLeftPanel();
    }

    public void SetSelectedOrbit() 
    {
        // Hide orbit of previous planet, only if it exists
        if (previousPlanet != null) 
        {
            //Debug.Log("previousPlanet: " + previousPlanet.name + ", selectedPlanet: " + selectedPlanet.name);
            previousPlanet.SetOrbitColor("deselected");
        }
        if (selectedPlanet != null) 
		{
            // Highlight orbit of selected planet every time
            selectedPlanet.SetOrbitColor("selected");
        }
    }

    private void SetSelectionPanelActive (bool active = false) 
    {
        leftSelectionPanel.gameObject.SetActive(active);
    }

    private void SetPreviewPanelActive (bool active = false) 
    {
        leftPreviewPanel.gameObject.SetActive(active);
    }

    private void SetResourcePanelActive (bool active = false) 
    {
        leftResourcePanel.gameObject.SetActive(active);
    }

    private void SetStatusPanelActive (bool active = false) 
    {
        leftStatusPanel.gameObject.SetActive(active);
    }

    private void SetTechnologyPanelActive (bool active = false) 
    {
        leftTechnologyPanel.gameObject.SetActive(active);
    }

    private void SetBuildingPanelActive (bool active = false) 
    {
        leftBuildingPanel.gameObject.SetActive(active);
    }

	private void SetSelectedName (string name = null)
	{
		// Planet selected
		if (name != null) 
		{
			leftLeftText.text = "«";
			leftNameText.text = name;
			leftRightText.text = "»";
		}
		// No planet selected
		else 
		{
			leftLeftText.text = "";
			leftNameText.text = "No Planet Selected";
			leftRightText.text = "";
		}
	}

	private void SetSelectedPreview (Sprite sprite = null)
	{
		// Planet selected
		if (sprite != null) 
		{
			leftPreviewImage.GetComponent<Image>().sprite = sprite;
		}
		// No planet selected
		else 
		{
			// Resources.Load searches in the directory "Assets/Resources"
			sprite = (Sprite)Resources.Load<Sprite>("Icons/delete-material-white");
			leftPreviewImage.GetComponent<Image>().sprite = sprite;
		}
	}

	private void SetSelectedResources (int stone = -1, int water = -1, int gas = -1)
	{
		// Planet selected
		if (stone > -1 && water > -1 && gas > -1) 
		{
			leftStoneText.text = stone.ToString();
			leftWaterText.text = water.ToString();
			leftGasText.text = gas.ToString();
		}
		// No planet selected
		else 
		{
			leftStoneText.text = leftWaterText.text = leftGasText.text = "-";
		}
	}

	public void SetSelectedResourcesCollected (int stoneCollected = -999, int waterCollected = -999, int gasCollected = -999)
	{
		// Planet selected
		if (stoneCollected > -999 && waterCollected > -999 && gasCollected > -999) 
		{
            string positive = "#4CAF50FF";
            string negative = "#F44336FF";
            string neutral = "#FFFFFF00";
            string stoneColor = "";
            string waterColor = "";
            string gasColor = "";
            string stonePrefix = "";
            string waterPrefix = "";
            string gasPrefix = "";

            // Determine if resources collected was negative/neutral/positive
            if (stoneCollected < 0) {stoneColor = negative; stonePrefix = "-";}
            else if (stoneCollected == 0) {stoneColor = neutral; stonePrefix = "";}
            else if (stoneCollected > 0) {stoneColor = positive; stonePrefix = "+";}

            if (waterCollected < 0) {stoneColor = negative; waterPrefix = "-";}
            else if (waterCollected == 0) {waterColor = neutral; waterPrefix = "";}
            else if (waterCollected > 0) {waterColor = positive; waterPrefix = "+";}

            if (gasCollected < 0) {gasColor = negative; gasPrefix = "-";}
            else if (gasCollected == 0) {gasColor = neutral; gasPrefix = "";}
            else if (gasCollected > 0) {gasColor = positive; gasPrefix = "+";}

            // Assign text appropriate colour
			leftStoneCollectedText.text = stonePrefix + stoneCollected.ToString();
            Color myColor = new Color();
            ColorUtility.TryParseHtmlString (stoneColor, out myColor);
            leftStoneCollectedText.color = myColor;
            if (stoneCollected != 0) StartCoroutine(FadeTextToFullAlpha(0.5f, leftStoneCollectedText));

			leftWaterCollectedText.text = waterPrefix + waterCollected.ToString();
            myColor = new Color();
            ColorUtility.TryParseHtmlString (waterColor, out myColor);
            leftWaterCollectedText.color = myColor;
            if (waterCollected != 0) StartCoroutine(FadeTextToFullAlpha(0.5f, leftWaterCollectedText));

			leftGasCollectedText.text = gasPrefix + gasCollected.ToString();
            myColor = new Color();
            ColorUtility.TryParseHtmlString (gasColor, out myColor);
            leftGasCollectedText.color = myColor;
            if (gasCollected != 0) StartCoroutine(FadeTextToFullAlpha(0.5f, leftGasCollectedText));
		}
		// No planet selected
		else 
		{
            //StartCoroutine(FadeTextToZeroAlpha(1f, leftStoneCollectedText));
			leftStoneCollectedText.text = leftWaterCollectedText.text = leftGasCollectedText.text = "<color=#FFFFFF00></color>";
		}
	}
	private void SetSelectedPopulation (int population = -1)
	{
		// Planet selected
		if (population > -1) 
		{
			if (population > 0) 
			{
				leftPopulationText.text = population.ToString () + " Billion";
			} 
			else 
			{
				leftPopulationText.text = "Uninhabited";
			}
		}
		// No planet selected
		else 
		{
			leftPopulationText.text = "-";
		}
	}

	private void SetSelectedHealth (int health = -1, int maxHealth = -1)
	{
		// Planet selected
		if (health > -1 && maxHealth > -1) 
		{
			if (health <= maxHealth) 
			{
				leftHealthText.text = health.ToString () + "/" + maxHealth.ToString () + " HP";
			} 
			else 
			{
				leftHealthText.text = "ERROR: health > max health";
			}
		}
		// No planet selected
		else 
		{
			leftHealthText.text = "-";
		}
	}

    private void SetSelectedTechnology (int technology = -1)
	{
		_technologyButtons = leftTechnologyPanel.GetComponentsInChildren<Button>();
		// Planet selected
//		if (technology > -1) {
//			for (int i = 0; i < _technologyButtons.Length; i++)
//			{
//				if (_technologyButtons[i].interactable)
//				{
//					_technologyButtons[i].interactable = false;
//				}
//			}
//		}
//
//		// No planet selected
//		else 
//		{
//			SetTechnologyPanelActive(false);
//
//            // Make uninteractive
////            _technologyButtons = leftTechnologyPanel.GetComponentsInChildren<Button>();
////            for (int i = 0; i < _technologyButtons.Length; i++)
////            {
////                if (_technologyButtons[i].interactable)
////                {
////                    _technologyButtons[i].interactable = false;
////                }
////            }
//		}
	}

    private void SetSelectedBuilding (int population = 0)
	{
		// Planet selected
		if (population > -1) 
        {
            if (population >= 0) 
            {
                if (selectedPlanet != null)
                {
                    leftBuildingText.text = selectedPlanet.turnsToBuild.ToString() + " turn" + ((selectedPlanet.turnsToBuild>1) ? "s" : "") + " left to build.";
                }
            }
            else 
            {
                // When Play is pressed, let the text hang until the next turn begins
                leftBuildingText.text = "1 turn left to build.";
            }
		}
	}

	/* Set Right Panel
	   ========================================================================== */
    public void SetPlayInteractive (bool canSimulate = false)
    {
        rightNextTurnButton.interactable = canSimulate;
    }

	/* Set External Scripts
	   ========================================================================== */
    private void SetupTooltipController ()
    {
        // Right Panel
        SetTooltip((RectTransform)rightStoneButton.transform, Constants.TOOLTIP_RIGHT_STONE_BUTTON);
        SetTooltip((RectTransform)rightWaterButton.transform, Constants.TOOLTIP_RIGHT_WATER_BUTTON);
        SetTooltip((RectTransform)rightGasButton.transform, Constants.TOOLTIP_RIGHT_GAS_BUTTON);
        SetTooltip((RectTransform)rightNextTurnButton.transform, Constants.TOOLTIP_RIGHT_NEXT_TURN_BUTTON);
        //SetTooltip((RectTransform)rightMission1Button.transform, Constants.TOOLTIP_RIGHT_MISSION_1);
        //SetTooltip((RectTransform)rightMission2Button.transform, Constants.TOOLTIP_RIGHT_MISSION_2);
        //SetTooltip((RectTransform)rightMission3Button.transform, Constants.TOOLTIP_RIGHT_MISSION_3);
        // Left Panel
        SetTooltip((RectTransform)leftBuildingButton.transform, Constants.TOOLTIP_LEFT_BUILDING_BUTTON);
        SetTooltip((RectTransform)leftTechnology1Button.transform, Constants.TOOLTIP_LEFT_TECHNOLOGY_1_BUTTON);
        SetTooltip((RectTransform)leftTechnology2Button.transform, Constants.TOOLTIP_LEFT_TECHNOLOGY_2_BUTTON);
        SetTooltip((RectTransform)leftLinkToButton.transform, Constants.TOOLTIP_LEFT_LINK_TO_BUTTON);
        SetTooltip((RectTransform)leftTechnology3Button.transform, Constants.TOOLTIP_LEFT_TECHNOLOGY_3_BUTTON);
        SetTooltip((RectTransform)leftTechnology4Button.transform, Constants.TOOLTIP_LEFT_TECHNOLOGY_4_BUTTON);
        SetTooltip((RectTransform)leftTechnology5Button.transform, Constants.TOOLTIP_LEFT_TECHNOLOGY_5_BUTTON);
    }

    public void SetTooltip(RectTransform rt, string myString)
    {
        TooltipController tc = rt.gameObject.AddComponent<TooltipController>();
        tc.myString = myString;
    }

    public void HideAllTooltips()
    {
        // Hide tooltips
        RectTransform[] children = tooltipContainer.GetComponentsInChildren<RectTransform>();
        foreach (var child in children)
        {
            //Debug.Log(child.name);
            if (child.name == "Tooltip(Clone)") 
            {
                child.gameObject.SetActive(false);
            }
        }  
    }

    private void SetupHintController ()
    {
        // Right Panel
        SetHint((RectTransform)rightStoneButton.transform);
        SetHint((RectTransform)rightNextTurnButton.transform);

        // Left Panel
        SetHint((RectTransform)leftTechnology1Button.transform);
        SetHint((RectTransform)leftTechnology2Button.transform);
        SetHint((RectTransform)leftLinkToButton.transform);
    }

    public void SetupOrbitalPlane ()
    {
        switch(GameController.level)
        {
            case 1:
                 SetOrbitalPlaneActive(false);
                 break;
            case 2:
                 SetOrbitalPlaneActive(false);
                 break;
            case 3:
                 SetOrbitalPlaneActive(false);
                 break;
            default:
                 SetOrbitalPlaneActive(true);
                 break;
            break;
        }
    }

    // Enable or disable a hint from any script
    public void SetHintActive(RectTransform rt, bool active) 
    {
        //Debug.Log("Set " + rt.name + "'s hint " + active.ToString());
        if (rt.GetComponent<HintController>() != null)
        {
            rt.GetComponent<HintController>().SetHintActive(active);
        }
    }

    public void SetHint(RectTransform rt) {
        HintController hc = rt.gameObject.AddComponent<HintController>();
    }

   	/* Update Left Panel
	   ========================================================================== */ 
    public void UpdateResources ()
    {
        // Planet selected
        if (selectedPlanet != null)
        {
            SetSelectedResources(selectedPlanet.stone, selectedPlanet.water, selectedPlanet.gas);
			SetSelectedResourcesCollected(selectedPlanet.addCarbon, selectedPlanet.addNitrogen, selectedPlanet.addHydrogen);
        }
        // No planet selected
        else
        {
            SetSelectedResources();
            SetSelectedResourcesCollected();
        }
    }

    public void UpdatePopulation ()
    {
        // Planet selected
        if (selectedPlanet != null)
        {
            SetSelectedPopulation(selectedPlanet.population);
        }
        // No planet selected
        else
        {
            SetSelectedPopulation();
        }
    }

    public void UpdateHealth ()
    {
        // Planet selected
        if (selectedPlanet != null)
        {
            SetSelectedHealth(selectedPlanet.health, selectedPlanet.maxHealth);
        }
        // No planet selected
        else
        {
            SetSelectedHealth();
        }
    }

    public void SetOrbitalPlaneActive (bool active)
    {
        orbitalPlanes.gameObject.SetActive(active);
    }

    /* Animations
    ========================================================================== */ 
    public void OpenLeftPanel() 
    {
        Animator anim = leftPanel.GetComponent<Animator>();
        anim.enabled = true;
        anim.Play("LeftPanelSlideIn");
    }

    public void CloseLeftPanel() 
    {
        Animator anim = leftPanel.GetComponent<Animator>();
        anim.enabled = true;
        anim.Play("LeftPanelSlideOut");
    }

    private IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        fadeCoroutineRunning = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        fadeCoroutineRunning = false;
    }
 
    private IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        fadeCoroutineRunning = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            Debug.Log(i.color.a + "");
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        fadeCoroutineRunning = false;
    }
}