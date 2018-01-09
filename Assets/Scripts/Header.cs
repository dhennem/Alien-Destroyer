using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Header : MonoBehaviour {

	//each of the 4 components of the header/scoreboard; defined in the inspector
	public Text levelDisplay;
	public Text livesDisplay;
	public Text shipsLeftDisplay;
	public Text shotsLeftDisplay;
	

	void Start () {
	//note that the initial values are retreived from the LevelManager, but updated values are retreived from
	//their respective classes (Cannon, Ship, Shot)
		levelDisplay.text = "Stage: " + (LevelManager.getCurrentStageIndex()+1).ToString();
		livesDisplay.text = "Lives: " + LevelManager.getInitialNumberOfLives().ToString();
		shipsLeftDisplay.text = "Ships Left: " + Ship.shipCount.ToString();
		shotsLeftDisplay.text = "Shots Left: " + LevelManager.getInitialNumberOfShots().ToString();
	}
	
	void Update () {
	//keep redefining the values in the header in case they change
		livesDisplay.text = "Lives: " + Cannon.livesLeft.ToString();
		shipsLeftDisplay.text = "Ships Left: " + Ship.shipCount.ToString();
		shotsLeftDisplay.text = "Shots Left: " + Shot.shotsLeft.ToString();
	
	}
	
}
