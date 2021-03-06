﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Make class require a LineRenderer to be attached whenever we run this script
[RequireComponent(typeof(LineRenderer))]
public class Planet : MonoBehaviour
{
    public bool die = false;
    // class fields
    public int tier;
    public int addCarbon;
    public int addNitrogen;
    public int addHydrogen;
    public int turnsToBuild;
    public int defensePower;
    public int attackPower;
    public int turnsToDie;
    public int OriginaddCarbon;
    public int OriginaddNitrogen;
    public int OriginaddHydrogen;
    public int halfaddCarbon;
    public int halfaddNitrogen;
    public int halfaddHydrogen;
    public int addlinkchance;
    public bool stormsheid = false;
    public bool moreResource = false;
    public int technologyLevel = 0;
    public bool iflinkactive = false;
    public bool ifattackactive = false;


    public int iftech1;
    public int iftech2;
    public int iftech3;
    public int iftech4;
    public int iftech5;
    // Get a reference to the LineRenderer
    LineRenderer lr;

    // Range attribute between 3 and 36 to determine # of segments in ellipse
    [Range(100, 100)]
    public int segments;

    // orbit
    // Will reference the motion of the planet model
    public Transform orbitingObject;

    // Serialized ellipse object


    public EllipseTester orbitPath;

    // How far along the path of the ellipse we are
    // Clamp between 0-1
    [Range(0f, 1f)]
    public float orbitProgress = 0f;

    // How long it will take in seconds to complete one orbit
    public float orbitPeriod;

    public float orbitSpeedMultiplier = 1f;

    // Checks if in Fast or Slow Universe
    public bool fastUniverse;

    // Was the planet placed?
    public bool planetPlaced;

    // Resource Counters
    public int stone;
    public int water;
    public int gas;

    // scripts 
    private GameController gc; // Access Game Controller script

    // store coroutine when placing so we can stop once planet is placed
    public Coroutine placing;
    public bool placingCoroutineRunning;

    //add a collider for the object
    public SphereCollider sc;

    // GUI
    private float rectx, recty;
    public Vector3 pos;
    public Sprite planetSprite;
    public int population = 0;
    public int health;
    public int maxHealth;

    // collecting
    public bool collecting = false;
    private int count = 0;
    private int collectionMultiplier = 0;

    private bool ifHover = false;

    // trading
    public int maxResourceType = 0;
    public int maxResource = 0;
    public int tradecarbon = 0;
    public int tradenitrogen = 0;
    public int tradehydrogen = 0;

    public string planetname = " ";

	public List<GameObject> tradeship=new List<GameObject>();
    // linking
    public List<Planet> linkedWith = new List<Planet>(); // Each planet will have their own list of planets they have linked with
    public LineRenderer[] links;
    public GameObject[] lines;

    private GUIStyle guiStyle = new GUIStyle();

    private UIController ui;
    // Move here so it is referencable by the UI script
    public int stoneCollected;
    public int waterCollected;
    public int gasCollected;

    public Planet()
    {
        tier = 0;
        addCarbon = 0;
        addNitrogen = 0;
        addHydrogen = 0;
        turnsToBuild = 0;
        defensePower = 0;
        attackPower = 0;
        turnsToDie = 0;
    }

    // Set Orbit Color (takes selected, deselected, or hovered)
    public void SetOrbitColor(string state = "")
    {
        Color color = Color.white;
        switch (state)
        {
            case "selected":
                color.a = 1f;
                break;
            case "deselected":
                color.a = 0.04f;
                break;
            case "hovered":
                color.a = 0.2f;
                break;
            default:
                color.a = 0f;
                break;
        }
        lr.material.SetColor("_TintColor", color);
    }

    public void Awake()
    {
        // Get reference when we start the game
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 2f;

        SetOrbitColor("deselected"); // Deselected by default

        // Calculate ellipse right when we start the game
        if (orbitingObject.transform.parent != null)
        {
            CalculateEllipse();
        }
    }

    // Use this for initialization
    public virtual void Start()
    {
        ui = GameObject.Find("Canvas").GetComponent<UIController>();

        lines = new GameObject[10];
        links = new LineRenderer[10];
        for (int i = 0; i < 10; i++)
        {
            lines[i] = new GameObject();
            links[i] = lines[i].AddComponent<LineRenderer>();
            links[i].SetWidth(0.8f, 0.8f);



            links[i].SetColors(Color.red, Color.red);

        }
	
        gc = GameObject.Find("Game Manager").GetComponent<GameController>();

        orbitPeriod = 0.001f;
        health = 100;
        maxHealth = 100;

        // Set orbiting object position
        SetOrbitingObjectPosition();

        if (!planetPlaced)
        {
            // Debug.Log("StartCoroutine (AnimateOrbit (1)); called from Planet.cs");
            placing = StartCoroutine(AnimateOrbit(Constants.ANIMATE_SPEED_TEST));
        }

        //add a collider for this planet
        sc = gameObject.GetComponent<SphereCollider>();
        //	linkline = gameObject.AddComponent<LineRenderer> ();
        //sc.radius = 0.5f;
        //sc.center = new Vector3(0, 0, 0);

        if (addCarbon == 4)
        {
            planetname = "Carbon";

        }
        else if (addCarbon == 6)
        {
            planetname = "Silicon";
        }
        else if (addNitrogen == 6)
        {
            planetname = "Nitrogen";
        }
        else if (addHydrogen == 6)
        {
            planetname = "Hydrogen";
        }
        OriginaddCarbon = addCarbon;
        OriginaddHydrogen = addHydrogen;
        OriginaddNitrogen = addNitrogen;
        halfaddCarbon = addCarbon / 2;
        halfaddNitrogen = addNitrogen / 2;
        halfaddHydrogen = addHydrogen / 2;
    }

    public void FixedUpdate()
    {
        if (die == true)
        {
            this.gameObject.SetActive(false);

            foreach (var line in lines)
            {
                line.SetActive(false);
            }
        }
        // Constant rotation
        if (fastUniverse)
        {
            transform.Rotate(9 * Time.deltaTime, 40 * Time.deltaTime, 9 * Time.deltaTime);
        }
        else
        {
            transform.Rotate(3 * Time.deltaTime, 18 * Time.deltaTime, 4 * Time.deltaTime);
        }


        if (die == false)
        {
            if (linkedWith.Count > 0)
            {

                for (int i = 0; i < linkedWith.Count; i++)
                {

                    if (linkedWith[i].die == false)
                    {
                        //change the line renderer color here
                        if (linkedWith[i].CompareTag("Rogue"))
                        {

                            links[i].SetColors(Color.red, Color.red);
                        }
                        links[i].SetPosition(0, transform.position);
                        links[i].SetPosition(1, linkedWith[i].transform.position);
                    }
                    else
                    {
                        lines[i].SetActive(false);
                    }
                }
            }
        }
        if (Application.isPlaying && lr != null)
        {
            CalculateEllipse();
            //Debug.Log(fastUniverse);
        }

        // pop up of resources collected after simulation
        if (collecting)
        {
            count++;
            if (count == 30)
            {
                pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                rectx = pos.x;
                recty = pos.y;
                //Debug.Log(rectx);
                //Debug.Log(recty);


            }
            if (count > 30)
            {

                recty += 1;
            }
            if (count > 60)
            {

     

                count = 0;
                collecting = false;
                //gc.simulate = false;

                // Update Game State
                if (gc.GAME_STATE == Constants.TURN_1_WATCH_SIMULATION)
                {
                    // Go to next step if the skill tree isn't open
                    if (GameObject.Find("Macro Skill Tree") == null)
                    {
                        //gc.GAME_STATE = Constants.TURN_2_SKILL_TREE;
                    }
                    else
                    {
                        // Otherwide, skip ahead
                        //gc.GAME_STATE = Constants.TURN_2_PLANET_SLOT;
                    }
                }
                else if (gc.GAME_STATE == Constants.TURN_2_WATCH_SIMULATION)
                {
                    // Make button interactable
                    //var button = GameObject.Find("Micro Skill Tree Button").GetComponent<Button>();
                    //button.interactable = true;

                    // Go to next step if the skill tree isn't open
                    //gc.GAME_STATE = Constants.TURN_3_TECH_TREE;
                }
            }
        }
    }

    //function that trade 
    void trade(Planet temp)
    {
        getMaxResource();
        temp.getMaxResource();

        if (maxResourceType == 1)
        {
            stone -= 1;
            
 
        }else if (maxResourceType == 2)
        {
            water -= 1;
           
 

        }else if (maxResourceType == 3)
        {
            gas -= 1;
        

        }

		if (temp.maxResourceType == 1)
		{
			stone += 1;
	

		}else if (temp.maxResourceType == 2)
		{
			water += 1;
		


		}else if (temp.maxResourceType == 3)
		{
			gas += 1;


		}	

    }

    void getMaxResource()
    {
        maxResource = Mathf.Max(stone, water, gas);
        if (maxResource == stone)
        {
            maxResourceType = 1;
        }
        if (maxResource == water)
        {
            maxResourceType = 2;
        }
        if (maxResource == gas)
        {
            maxResourceType = 3;
        }
    }

    void OnMouseOver()
    {

        //Debug.Log("ui.selectedPlanet != this:" + (ui.selectedPlanet != this).ToString());
        ifHover = true;
        if (ui.selectedPlanet != this)
        {
            // Hover planet orbit on highlight
            Color color = Color.white;
            color.a = 0.2f;
            lr.material.SetColor("_TintColor", color);
        }
    }
    void OnMouseExit()
    {
        //Debug.Log("ui.selectedPlanet != this:" + (ui.selectedPlanet != this).ToString());
        ifHover = false;
        if (ui.selectedPlanet != this)
        {
            // Return to regular color on mouse exit
            Color color = Color.white;
            color.a = 0.04f;
            lr.material.SetColor("_TintColor", color);
        }
    }

    //Function that can show the resource of this object
    void CollectNow()
    {
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;
        if (gc.storm && stormsheid == false)
        {
            guiStyle.normal.textColor = Color.cyan;
        }

        if (ifHover == true)
        {
            //GUI.Box(new Rect(rectx + 20, Screen.height - recty, 300, 50), planetname + ": Carbon: " + carbon + ", Nitrogen: " + nitrogen + ", Hydrogen: " + hydrogen + "\n This is a Tier " + tier + " Planet.",guiStyle);

        }
        //}
        // pop up of resources collected after simulation
        if (collecting == true/* && count > 30*/)
        {
            stoneCollected = addCarbon * collectionMultiplier;
            waterCollected = addNitrogen * collectionMultiplier;
            gasCollected = addHydrogen * collectionMultiplier;

            //Debug.Log("addCarbon: " + addCarbon + ", collectionMultiplier:" + collectionMultiplier);

        }
    }

    public void IncreasePopulation()
    {
        if (turnsToBuild < 1)
        {
            population = (int)(population + 1);
			gc.l.UpdateLogPopulation (planetname);
        }
    }

    public void CollectResources()
    {
        // Run before collecting universes, so collectionMultiplier is never 0.
        if (fastUniverse)
        {
            collectionMultiplier = 2;
        }
        else
        {
            collectionMultiplier = 2;
        }


		if (population > 0)
		{
			collecting = true;
			CollectNow();
		
			// old trade code
			//for (int i = 0; i < linkedWith.Count; i++)
			//{
			//    int j = 0;
			//    if (j == 0)
			//    {
			//        trade(linkedWith[i]);
			//        j = 1;
			//    }
			//}
		
			// updated trade code to only trade with non-rogue planets
			foreach (var planet in linkedWith)
			{
				if (!planet.CompareTag("Rogue") && !this.CompareTag("Rogue"))
				{
					//                    Debug.Log(this.name + " traded with: " + planet.name);
					trade(planet);
					if (this.maxResourceType == 1) {
						tradecarbon--;
					} else if (this.maxResourceType == 2) {
						tradenitrogen--;
					} else if (this.maxResourceType == 3) {
						tradehydrogen--;
					}
		
					if (planet.maxResourceType == 1) {
						tradecarbon++;
					} else if (planet.maxResourceType == 2) {
						tradenitrogen++;
					} else if (planet.maxResourceType == 3) {
						tradehydrogen++;
					}
					gc.l.UpdateLogTrade (this.planetname, planet.planetname, tradecarbon, tradenitrogen, tradehydrogen);
				}
				if (planet.CompareTag("Rogue") && planet.die == false)
				{
					//					Debug.Log(this.name + " was stole by  " + planet.name);
					tradecarbon--;
					tradenitrogen--;
					tradehydrogen--;
				}
				if (this.CompareTag("Rogue") && this.die == false)
				{
					//					Debug.Log(this.name + " stole from  " + planet.name);
					tradecarbon++;
					tradenitrogen++;
					tradehydrogen++;
		
					//gc.summary += " Rogue planet attacked " +planet.planetname+" and "+planet.planetname+" losed 25 hp!\n";
				}
		
			}
            stone += addCarbon * collectionMultiplier;
            water += addNitrogen * collectionMultiplier;
            gas += addHydrogen * collectionMultiplier;
            if (turnsToBuild < 1)
            {
				gc.summary += "It collected" + addCarbon * collectionMultiplier + " stones, " + addNitrogen * collectionMultiplier + " water, " + addHydrogen * collectionMultiplier + " gas.\n";
            }
            if (linkedWith.Count > 0)
            {


                gc.summary += "It traded" + tradecarbon + " stones, " + tradenitrogen + " water, " + tradehydrogen + " gas.\n";
            }
            if (iftech1 == 3)
            {
                gc.summary += "It learned collecting more resources technology\n";
                iftech1 = 4;
            }
            if (iftech2 == 3)
            {
                gc.summary += "It learned linking technology\n";
                iftech2 = 4;
            }
            if (iftech3 == 3)
            {
                gc.summary += "It learned more linking chance technology\n";
                iftech3 = 4;
            }
            if (iftech4 == 3)
            {
                gc.summary += "It learned stormsheid technology\n";
                iftech4 = 4;
            }
            if (iftech5 == 3)
            {
                gc.summary += "It learned attacking technology\n";
                iftech5 = 4;
            }
            ui.SetPhase("Planning");
            gc.showsummary = true;
            gc.simulate = false;
            gc.playButton.interactable = true;
            //gc.ToggleInteractability(true);
            gc.SetBuildingActive(true);
    
        }
    }

    // Control the orbit
    // Length determines how long will orbit in seconds
    public IEnumerator AnimateOrbit(float length)
    {
        placingCoroutineRunning = true;
        float starting = 0f;
        // Is the orbit really close to 0? We don't want it to move too fast.
        // Set it to a more reasonable minimum (every 1/10 of a second) so it won't divide by 0
        //if (orbitPeriod < 0.1f)
        //{
        //    orbitPeriod = 0.1f;
        //}

        while (starting < length) // https://answers.unity.com/questions/504843/c-make-something-happen-for-x-amount-of-seconds.html
        {
            if (gc.simulate) // only during simulation will orbit for specific duration, else constantly orbits
            {
                starting += Time.deltaTime;
            }

            // Make orbit speed be affected by universe (disabled)
            Vector2 orbitPos = orbitPath.Evaluate(orbitProgress);

            // when orbitPos.x is greater than 1, it is in the fast universe, else not
            fastUniverse = orbitPos.x < 0 ? false : true;
            int universeMultiplier = orbitPos.x < 0 ? 2 : 1;

            // Make orbit faster closer to sun
            float linearMultiplier = 8f;
            int exponentialMultiplier = 3;
            float orbitSpeedMultiplier = universeMultiplier * Mathf.Pow(Mathf.Max(Mathf.Abs(orbitPath.xAxis), Mathf.Abs(orbitPath.yAxis)) / linearMultiplier, exponentialMultiplier);



            // Division is one of the least efficient thing in basic C#
            // So use time.deltatime to see how far we're moving every frame
            // We want the inverse of orbitPeriod to see how fast we need to catch up
            float orbitSpeed = 1f / (orbitPeriod * orbitSpeedMultiplier);

            // (Amount of time frame has taken) * calculated orbit speed
            orbitProgress += Time.deltaTime * orbitSpeed;

            // Do not exceed float value if we add to the orbit progress over time
            // If we go beyond 1f, reset to between 0-1.
            orbitProgress %= 1f;

            // Set planet position based on orbit position
            SetOrbitingObjectPosition();

            yield return null;
        }


        // Add turn if a planet is building
        gc.SetBuildingActive(true);

        IncreasePopulation();
        CollectResources();

        placingCoroutineRunning = false;

        // Update UI last
        ui.UpdateSelectedPlanet();

        //gc.m.CheckMissions(gc.m.missions);

        // update how many resources collected per planet
        gc.l.UpdateLogPlanetRes(this.gameObject.name, stoneCollected, waterCollected, gasCollected);

        gc.l.LogBackLog();


    }

    // Calculate the ellipse
    public void CalculateEllipse()
    {
        // Create an array of Vector3's.
        // Populate LineRenderer with array of points to render
        // (segments + 1 to complete the ring around. We'll make the last element equal to the first element later.)
        Vector3[] points = new Vector3[segments + 1];

        // Iterate through these points
        for (int i = 0; i < segments; i++)
        {

            // Pass in t value (i/segments)
            Vector3 position3D = orbitPath.Evaluate((float)i / (float)segments);

            // Debug
            //Debug.Log ("Position of sun: (" + orbitingObject.transform.parent.localPosition.x + ", " + orbitingObject.transform.parent.localPosition.z + ")");


            // Set point at i equal to a new Vector2 of (x,y) and 0 for z value
            points[i] = new Vector3(
                position3D.x + orbitingObject.transform.parent.localPosition.x,
                position3D.z + orbitingObject.transform.parent.localPosition.z,
                position3D.y + orbitingObject.transform.parent.localPosition.y
            );

            if (GameController.ORBIT_4D)
            {
                if (i == 24) points[i].y *= 1000;
                if (i == 25) points[i] = new Vector3(99999999, 99999, 99999);
                if (i == 26) points[i].y *= 1000;
                if (i == 74) points[i].y *= 1000;
                if (i == 75) points[i] = new Vector3(99999999, -99999, 99999);
                if (i == 76) points[i].y *= 1000;
            }
        }
        // Remember we have segments + 1, and are 0-indexing
        // Very last point in the array is equal to first point, completing the ellipse
        points[segments] = points[0];

        // Set LineRenderer using newer method positionCount
        lr.positionCount = segments + 1;

        // Pass in points array
        lr.SetPositions(points);
    }

    // Call-back method. If we change values in-editor while we are playing the game, we can see those
    public void OnValidate()
    {
        if (Application.isPlaying && lr != null)
        {
            CalculateEllipse();
        }
    }

    public void SetOrbitingObjectPosition()
    {
        // Pass in current orbit progress
        Vector3 orbitPos = orbitPath.Evaluate(orbitProgress);

        // Set its local position to a new vector position
        // Set z axis as orbitPos.y
        orbitingObject.localPosition = new Vector3(orbitPos.x, orbitPos.z, orbitPos.y);
    }



    //depends on the planets, we can adjust it.
    public void addResourceTechnology()
    {

        stone -= 10;
        moreResource = true;
        if (moreResource == true)
        {
            OriginaddCarbon += 2;
            OriginaddHydrogen += 2;
            OriginaddNitrogen += 2;
            halfaddCarbon += 1;
            halfaddNitrogen += 1;
            halfaddHydrogen += 1;

        }
        //		addHydrogen++;
        //		addNitrogen++;
    }

    //set the addlinkchance and it will effect the CalculateFail() function in the gamecontroller
    public void linkchanceTechnology()
    {
        gas -= 15;
        water -= 10;
        addlinkchance++;
    }

    //set the storm shied and it will effect the Simulate() function in the gamcontroller
    public void StormShiedTechnology()
    {

        stone -= 15;
        water -= 15;
        stormsheid = true;
    }

}
