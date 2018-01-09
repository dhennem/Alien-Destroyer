using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoseMessage : MonoBehaviour {

	//this is used to determine the message that the player receives at the lose screen
	static public bool lostDueToLives; //true-> player lost because they ran out of lives; false->player lost because they ran out of shots

	//defined in the inspector to equal the text display message in the lose screen
	public Text message;
	//defined in the inspector to equal the continue from highest stage button
	public Button continueButton;
	public LevelManager levelManager;
	
	private string highestStage;

	void Start () {
		levelManager = FindObjectOfType<LevelManager>();
		manageLoseDescription();
		manageContinueButton();
	
	}
	
	void manageLoseDescription(){
		if(lostDueToLives){
			message.text = "Your cannon was not able to withstand the enemy's shots!";
		}
		else{
			message.text = "Your ammunition levels have been completely exhausted!";
		}
	}
	
	void manageContinueButton(){
		switch(LevelManager.highestStageReached){
			case 1:
				highestStage = "stage_01_01";
				break;
			case 2:
				highestStage = "stage_02_01";
				break;
			case 3:
				highestStage = "stage_03_01";
				break;
			case 4:
				highestStage = "stage_04_01";
				break;
			case 5:
				highestStage = "stage_05_01";
				break;
			default:
				highestStage = "Start";
				break;
		}
		continueButton.onClick.AddListener(LoadHighestStage);
		
	}
	
	void LoadHighestStage(){
		levelManager.LoadLevel(highestStage);
	}
	
}
