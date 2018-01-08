using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	private bool isPaused;
	private bool readyToLoadNewLevel;
	private float pauseTimeLeft;
	private bool won;
	private bool lost;
	
	//matrix where the rows represent the preset number of lives and shots respectively for each level
	static int[,] levelSettingsA = new int[,] {{3,6}, {3,7}, {2,7}, {2,8}, {1,9}};
	//matrix where the rows represent the preset minimum ship speeds, maximum ship speeds, chances of enemy shots, and enemy shot velocities respectively for each level
	static float[,] levelSettingsB = new float[,] {{0.75f,1f,0.01f,-6f},{0.85f,1.10f,0.01f,-8f},{1.0f,1.25f,0.0115f,-10f},{1.25f,1.50f,0.0125f,-12f},{1.50f,1.75f,0.015f,-14f},{1.75f,2.0f,0.0175f,-16f}};
	//lists that represent an unordered collection of ship types (1hit,2hit,3hit) that each level should spawn in ShipGenerator
	static int[] shipList1 = {1,1,1,1,1};
	static int[] shipList2 = {1,1,1,2,2,2};
	static int[] shipList3 = {1,1,1,2,2,3};
	static int[] shipList4 = {1,1,1,1,2,2,3};
	static int[] shipList5 = {1,1,1,2,2,2,3,3};

	public void LoadLevel(string levelName){
		RefreshSettings();
		Application.LoadLevel(levelName);
	}
	public void QuitRequest(){
		Debug.Log("Quit requested");
	}
	public void LoadNextLevel(){
		Application.LoadLevel(Application.loadedLevel + 1);
		RefreshSettings();
	}
	
	
	void Start(){
		isPaused = false;
		readyToLoadNewLevel = false;
		won = false;
		lost = false;
	}
	
	void Update(){
		if(!won && !lost){ //while the player has not yet won or lost
			CheckShots();
		}
		if(readyToLoadNewLevel){
			if(won){
				LoadNextLevel();
			}
			if(lost){
				LoadLevel("Lose");
				
			}
		}
		if(isPaused){
			//if the conditions to load a new level are detected in CheckShips(), CheckShots(), or CheckLives(), then isPaused = true and pauseTimeLeft = xf, effectively pausing the game for x seconds before loading a new level
			pauseTimeLeft -= Time.deltaTime;
			if(pauseTimeLeft <= 0){
				readyToLoadNewLevel = true; //so that the next frame after the pause will load the new level
			}	
		}
		
	}
	
	void RefreshSettings(){
	//returns variables to their original values while a new level is loaded
		Ship.shipCount = 0;
		Shot.outOfShots = false;
		Cannon.firstShotFired = false;
		Ship.row1Alive = true;
		Ship.row2Alive = true;
		Ship.row3Alive = true;
		Ship.lowestRowAlive = "row1";
		isPaused = false;
		readyToLoadNewLevel = false;
		won = false;
		lost = false;
	}
	
	
	//returns the number of lives allotted for the current level
	static public int getInitialNumberOfLives(){
		//each subarray in levelSettingsA has the lives at index 0
		return levelSettingsA[getLevelIndex(), 0];
	}
	//returns the number of shots allotted for the current level
	static public int getInitialNumberOfShots(){
		//each subarray in levelSettingsA has the shots at index 1
		return levelSettingsA[getLevelIndex(), 1];
	}
	//returns the minimum ship speed allowed for the current level
	static public float getMinShipSpeed(){
		//each subarray in levelSettingsB has the min ship speed at index 0
		return levelSettingsB[getLevelIndex(), 0];
	}
	//returns the maximum ship speed allowed for the current level
	static public float getMaxShipSpeed(){
		//each subarray in levelSettingsB has the max ship speed at index 1
		return levelSettingsB[getLevelIndex(), 1];
	}
	//returns the chance of an enemy shot (for each frame) for the current level
	static public float getChanceOfEnemyShot(){
		//each subarray in levelSettingsB has the chance of enemy shot at index 2
		return levelSettingsB[getLevelIndex(), 2];
	}
	//returns the velocity of an enemy shot for the current level
	static public float getEnemyShotVelocity(){
		//each subarray in levelSettingsB has the enemy shot velocity at index 3
		return levelSettingsB[getLevelIndex(), 3];
	}
	//returns an unordered collection of ship types that the current level should spawn in ShipGenerator
	static public int[] getShipList(){
		int[] shipList;
		switch(Application.loadedLevel){
			case 1:
				shipList = shipList1;
				return shipList;
				break;
			case 2:
				shipList = shipList2;
				return shipList;
				break;
			case 3:
				shipList = shipList3;
				return shipList;
				break;
			case 4:
				shipList = shipList4;
				return shipList;
				break;
			case 5:
				shipList = shipList5;
				return shipList;
				break;
		}
		return new int[]{};
	}
	//returns appropriate index to access the current level's information in levelSettings
	static public int getLevelIndex(){
		return (Application.loadedLevel - 1);
	}
	
	//methods for checking lives, shots, and ships left to see if the player won or lost
	public void CheckShips(){
		//this is called after each ship is destroyed to see if the next level needs to be loaded
		if(Ship.shipCount <= 0){
			won = true;
			lost = false;
			pauseGameBeforeLoadingNewLevel(1.5f); //pause game for 1.5 seconds
		}
	}
	public void CheckLives(){
		//this is called after the cannon receives a hit to see if it has any more lives left
		if(Cannon.livesLeft <= 0){
			won = false;
			lost = true;
			LoseMessage.lostDueToLives = true;
			pauseGameBeforeLoadingNewLevel(5f); //pause game for 5 seconds
		}
	}
	public void CheckShots(){
		//this is called every frame in Update() to check if there are any more shots left
		if(Shot.outOfShots && !isPaused && !readyToLoadNewLevel){
			won = false;
			lost = true;
			LoseMessage.lostDueToLives = false;
			pauseGameBeforeLoadingNewLevel(0f); //no pause if the player loses by running out of shots
		}
	}
	
	//called by the previous 3 methods whenever the conditions for winning/losing a level are detected; Update() handles what happens after the pause is over
	private void pauseGameBeforeLoadingNewLevel(float pauseDuration){
		isPaused = true;
		pauseTimeLeft = pauseDuration;
	}

}
