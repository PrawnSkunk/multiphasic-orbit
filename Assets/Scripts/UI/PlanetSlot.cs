﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Use event systems instead of event triggers in the Inspector
using UnityEngine.EventSystems;

// Implement pointer interfaces 
public class PlanetSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Transform planetsParent; // Reference to skill tree slots parent
    public Transform sun; // Reference to planet objects parent
    public GameObject planet; // Reference to prefab planet to generate
    public GameObject carbon; // Reference to prefab carbon to generate

    private bool mouseHover; // Track whether mouse is over skill tree
    private GameObject go; // New planet game object
    private Planet p; // Access new planet script
    private bool planetPlaced = false; // Flag for drawing planet orbit in realtime

    private GameController gc; // Access Game Controller script

    // Following code is generated by right clicking IPointerEnter/ExitHandler, and selecting "Refactor"
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseHover = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseHover = false;
    }

    void Start()
    {
        gc = GameObject.Find("Game Manager").GetComponent<GameController>();
    }

    void Update()
    {
        // When player clicks an active planet slot
        if (go == null && Input.GetMouseButtonUp(0) && this.GetComponent<Button>().interactable && mouseHover)
        {
            // if button's name is Carbon
            if (this.name == "Carbon")
            {
                // Create a new planet
                go = Instantiate(carbon) as GameObject;
                // Make planet a child object of the Sun
                go.transform.parent = sun.transform;
                // Add planet to array of planets
                gc.planets.Add(go);
                // Access the planet's script
                p = go.GetComponent<Carbon>();
            }

            // Create a new planet
            //go = Instantiate(planet) as GameObject;
            // Make planet a child object of the Sun
            //go.transform.parent = sun.transform;
            // Access the orbit script
            //p = go.GetComponent<Planet>();

            // Set all planet slot buttons as uninteractable
            var buttons = planetsParent.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].interactable)
                {
                    buttons[i].interactable = false;
                }
            }
        }

        // When player is placing planet
        if (go != null && !planetPlaced)
        {
            // Calculate 3D mouse coordinates
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Set final orbit position on mouse up
            if (!planetPlaced && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    // Only set orbit if clicking in-bounds
                    if (hit.collider.gameObject.name == "Orbit Plane")
                    {
                        planetPlaced = true;
                        p.planetPlaced = true;
                        p.orbitActive = false;
                    }
                }
            }

            // Update planet location with the mouse in realtime
            if (!planetPlaced)
            {
                Plane hPlane = new Plane(Vector3.up, Vector3.zero);
                float distance = 0;
                if (hPlane.Raycast(ray, out distance))
                {
                    RaycastHit hit = new RaycastHit();
                    if (Physics.Raycast(ray, out hit))
                    {
                        // Only set orbit if clicking in-bounds
                        if (hit.collider.gameObject.name == "Orbit Plane")
                        {
                            Vector3 location = ray.GetPoint(distance);

                            // Commented to prevent planet being placed under mouse
                            //go.transform.position = location;

                            // Simulate orbit path (absolute so the orbit direction doesn't change)
                            p.enabled = true;
                            p.orbitPath.xAxis = (Mathf.Abs(location.x));
                            p.orbitPath.yAxis = (Mathf.Abs(location.z));
                            float scale = 1.0f;
                            p.transform.localScale = new Vector3(scale, scale, scale);
                        }
                    }
                }
            }
        }
    }
}