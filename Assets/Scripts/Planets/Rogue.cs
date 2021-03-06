﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : Planet
{

    private Planet dominatedPlanetScript;
	private GameController gc2; 

    public Rogue()
    {
        addCarbon = 0;
        addNitrogen = 0;
        addHydrogen = 0;
        turnsToBuild = 0;
        defensePower = 0;
        attackPower = 0;
        turnsToDie = 0;
    }

    public override void Start()
    {
		gc2 = GameObject.Find("Game Manager").GetComponent<GameController>();
        base.Start();
        //dominatedPlanets = new List<Planet>();
    }

    // function used to steal a fraction of dominatedPlanet's resources
    // parameters are how many resources stolen per turn
    public void Steal(int sCarbon, int sNitrogen, int sHydrogen)
	{
		if (die != true){
			foreach (var dominatedPlanet in linkedWith) {
				dominatedPlanetScript = dominatedPlanet.GetComponent<Planet> ();
				// take away and add to this rogue planet's resources only if dominatedPlanet has more than 0 of that resource
				if (dominatedPlanetScript.stone > 0) {
					dominatedPlanetScript.stone -= sCarbon;
					stone += sCarbon;
				}
				if (dominatedPlanetScript.water > 0) {
					dominatedPlanetScript.water -= sNitrogen;
					water += sNitrogen;
				}
				if (dominatedPlanetScript.gas > 0) {
					dominatedPlanetScript.gas -= sHydrogen;
					gas += sHydrogen;
				}
			}
		}
    }


    // function used to attack another planet anywhere that has been built
    public void Attack()
    {
		if (die != true) {
			if (linkedWith != null) {
				foreach (var dominatedPlanet in linkedWith) {
					dominatedPlanetScript = dominatedPlanet.GetComponent<Planet> ();
					dominatedPlanetScript.health -= 25;
					if (dominatedPlanetScript.health <= 0) {
						dominatedPlanetScript.die = true;

					}
				}
			} else {
				dominatedPlanetScript = gc2.planets [0].GetComponent<Planet>();
				dominatedPlanetScript.health -= 25;
				if (dominatedPlanetScript.health <= 0) {
					dominatedPlanetScript.die = true;
				

				}
			}
		}
    }

    // function used to increase attribute value
    public void IncreaseAttribute(int attributeConst, int amount)
    {
        switch (attributeConst)
        {
            case Constants.DEFENSE:
                defensePower += amount;
                break;
            case Constants.ATTACK:
                attackPower += amount;
                break;
            default:
                break;
        }
    }
}
