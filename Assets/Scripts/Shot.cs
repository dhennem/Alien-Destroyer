using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour {

	static public int shotsLeft;
	static public bool outOfShots = false;
	static public Shot lastShotInstance = null;
	
	//only enemy shots have XVelocity; this is defined in the Ship script when an ememy shot is fired from a ship
	public float XVelocity;
	
	//this is checked off in the inspector to distinguish between enemy shots and player shots
	public bool shotFromEnemy;
	
	void Awake(){
		//keep track of the player's last shot
		if(shotsLeft==1 && !shotFromEnemy){
			lastShotInstance = this;	
		}
	}
	
	void Start () {
		//only set shotsLeft equal to the max shots when the first shot is fired, or else it is redefined each time a shot is fired
		if(!Cannon.firstShotFired){
			shotsLeft = LevelManager.getInitialNumberOfShots();
			Cannon.firstShotFired = true;
		}
	
	}
	
	void Update () {
		if(lastShotInstance == this){
			if(!this.isInRange()){
			//if the last shot was fired and it missed (meaning it is out of range) then outOfShots = true;
				lastShotInstance = null;
				shotsLeft = LevelManager.getInitialNumberOfShots();
				outOfShots = true;
			}
		}
		if(!isInRange()){
			//destroy any shots out of range
			Destroy(gameObject);
		}
		//rotate each shot every frame
		transform.Rotate(new Vector3 (0,0,100)*Time.deltaTime);
		
	}
	
	
	//checks if the shot is within range
	private bool isInRange(){
		Vector3 pos = gameObject.transform.position;
		return !(pos.x < -10f || pos.x > 30f || pos.y <-10f || pos.y>30f);
	}
	
}
