using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoseMessage : MonoBehaviour {

	//this is used to determine the message that the player receives at the lose screen
	static public bool lostDueToLives; //true-> player lost because they ran out of lives; false->player lost because they ran out of shots

	//defined in the inspector to equal the text display message in the lose screen
	public Text message;

	void Start () {
		if(lostDueToLives){
			message.text = "Your cannon was not able to withstand the enemy's shots!";
		}
		else{
			message.text = "Your ammunition levels have been completely exhausted!";
		}
	
	}
	
}
