using UnityEngine;
using System.Collections;


public class Cannon : MonoBehaviour {

	static public int livesLeft;
	static public bool firstShotFired = false;
	
	public GameObject shot;
	private LevelManager levelManager;
	public GameObject cannonExplosion;
	public AudioClip explosionSound;

	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		livesLeft = LevelManager.getInitialNumberOfLives();
		Shot.shotsLeft = LevelManager.getInitialNumberOfShots();
	
	}
	
	void Update () {
		HandleMovement();
		if(Input.GetMouseButtonDown(0)){
			//shoot once when the user clicks
			ShootCannon();
		}
	
	}
	
	void HandleMovement(){
		//cannon is moved horizontally by the location of the user's mouse
		Vector2 cannonPosition = new Vector2(0.5f, this.transform.position.y);
		float cannonXPosition = Input.mousePosition.x / Screen.width * 16;
		cannonPosition.x = Mathf.Clamp(cannonXPosition, 0.5f, 15.5f);
		this.transform.position = cannonPosition;
	}
	
	void ShootCannon(){
		if(Shot.shotsLeft>0){
			//creates a shot with upward velocity at the same location as the cannon 
			CreateShot(shot, gameObject.transform.position);
			//play the shot sound 
			audio.Play();
		}
	}
	
	void CreateShot(GameObject s, Vector3 position){
		//spawns a shot at the specified position with upward velocity
		//places the shot initially right above the cannon without touching it
		Vector3 newShotPosition = new Vector3(position.x, position.y+2.01f, 0);
		GameObject newShot = Instantiate(s, newShotPosition, Quaternion.identity) as GameObject;
		newShot.rigidbody2D.velocity = new Vector2(0f, 20f);
		newShot.transform.parent = transform; //makes the player's shots into children of the cannon
		Shot.shotsLeft--;
	}
	
	
	void OnTriggerEnter2D(Collider2D collider){
		if(livesLeft>1){
			HandleNonFatalHits();
		}
		else{
			HandleFatalHits();
		}
		livesLeft--;
		levelManager.CheckLives();
	}
	
	void HandleFatalHits(){
		//spawn explosion sound and effect at location of cannon before it is destroyed
		Instantiate(cannonExplosion, new Vector2(transform.position.x, transform.position.y+2f), Quaternion.identity);
		AudioSource.PlayClipAtPoint(explosionSound, transform.position);
		Destroy(gameObject);
	}
	
	void HandleNonFatalHits(){
		//change the sprite to look damaged
	}
}
