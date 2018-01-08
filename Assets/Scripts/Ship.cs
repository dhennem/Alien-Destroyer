using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {
	
	static public int shipCount = 0;
	//keeps track of how many ships are in each row; index 0 represents row 1, index 1 represents row 2, and index 2 represents row 3
	static public int[] rowCounts = new int[] {0,0,0};
	
	//number of hits this particular ship can take before being destroyed; defined differently in the inspector for each prefab
	public int maxHits;
	
	//defined in the inspector
	public GameObject shot;
	public GameObject explosion;
    public GameObject shipFire;
	public AudioClip explosionSound;
	public AudioClip enemyShotSound;
	
	//settings for each individual ship
	private LevelManager levelManager;
	private int hitsLeft;
	private float height; //defined differently for each row in Start()
	private float speed; //the speed for each ship is random with each start()
	private bool movingRight; //the direction for each ship is random with each start()
	private float leftBoundary; //the horizontal boundaries for each ship are randomly chosen in start()
	private float rightBoundary;
	private bool isDamaged; //initially defined to false for ships that can take multiple hits; defined as true once they take their first hit
	private GameObject fire; //defined in createShipFire() for multi-hit ships that are damaged
	//general settings for every ship
	private float chanceOfEnemyShot; //chanceOfEnemyShot and enemyShotVelocity are retrieved from LevelManager's levelSettings during Start()
	private float enemyShotVelocity; 
	private float minSpeed; //minSpeed and maxSpeed are retrieved from LevelManager's levelSettings during Start()
	private float maxSpeed;
	
	//keeping track of which rows of ships are not hit
	static public bool row1Alive = true;
	static public bool row2Alive = true;
	static public bool row3Alive = true;
	//the lowest row ship will always be the one shooting
	static public string lowestRowAlive = "row1";
	
	void Awake(){
		//need to make sure that shipCount is incremented first before the header is displayed
		shipCount++;
	}
	
	void Start () {
		//defining general properties for all ships
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		hitsLeft = maxHits;
		isDamaged = false;
		minSpeed = LevelManager.getMinShipSpeed(); 
		maxSpeed = LevelManager.getMaxShipSpeed();
		chanceOfEnemyShot = LevelManager.getChanceOfEnemyShot();
		enemyShotVelocity = LevelManager.getEnemyShotVelocity();
		height = transform.position.y;
		speed = Random.Range(minSpeed,maxSpeed);
		if(Random.Range(0f,1f)>0.5f) movingRight = true;
		RandomizeLeftBoundary();
		RandomizeRightBoundary();
	
	}
	
	void Update () {
		HandleMovement();
		if(Random.Range(0f,1f)<(chanceOfEnemyShot/shipCount)){ //when there are less ships alive, chance of enemy shot per ship is higher
			EnemyShot();
		}
		//if the ship is damaged (implying that it is a multi-hit ship currently on fire) keep the fire's location equal to the damaged ship's location
		if(isDamaged){
			fire.transform.position = transform.position;
		}
		
	}
	
	void HandleMovement(){
		//the movement of each ship is handled in this method
		Vector2 shipPosition = gameObject.transform.position;
		
		//handle changes in direction once horizontal boundaries are reached
		if(shipPosition.x >= rightBoundary){
			//the right boundary has been crossed
			movingRight = false;
			//the speed of each ship increases with each change in direction; every ship gets faster over time
			speed+=0.02f;
			RandomizeLeftBoundary();
		}
		if(shipPosition.x <= leftBoundary){
			//the left boundary has been crossed
			movingRight = true;
			//the speed of each ship increases with each change in direction; every ship gets faster over time
			speed+=0.02f;
			RandomizeRightBoundary();
		}
		
		//move the ship left or right depending on its current direction of movement
		if(movingRight){
			gameObject.transform.position = new Vector2(shipPosition.x + 0.1f*speed, height);
		}
		else if(!movingRight){
			gameObject.transform.position = new Vector2(shipPosition.x - 0.1f*speed, height);
		}
		
		//cap the speed at maxSpeed for all ships
		if(speed>=maxSpeed) speed=maxSpeed; 
	}
	
	void EnemyShot(){
		//creates a shot with downwards velocity from an enemy ship
		Vector3 enemyShipPosition = transform.position;
		Vector3 enemyShotPosition = new Vector3(enemyShipPosition.x, enemyShipPosition.y-0.5f, 0);
		GameObject enemyShot = Instantiate(shot, enemyShotPosition, Quaternion.identity) as GameObject;
		if(movingRight){
			enemyShot.rigidbody2D.velocity = new Vector2(2f, enemyShotVelocity);
			//enemyShot.transform.parent = transform; //makes every enemy shot into a child of the ship that shot it
		}
		else{
			enemyShot.rigidbody2D.velocity = new Vector2(-2f, enemyShotVelocity);
			//enemyShot.transform.parent = transform; //makes every enemy shot into a child of the ship that shot it
		}
		//enemyShot.transform.parent = GameObject.FindGameObjectWithTag("EnemyShots").transform;
		enemyShot.transform.parent = GameObject.Find("EnemyShots").transform;
		//spawns an enemy shot sound at the location of the ship once it shoots
		AudioSource.PlayClipAtPoint(enemyShotSound, transform.position);
		
	}
	
	/*void DetermineLowestRowAlive(){
		if(!row1Alive){
			if(!row2Alive){
				lowestRowAlive = "row3";
			}
			else{
				lowestRowAlive = "row2";
			}
		}
		else{
			lowestRowAlive = "row1";
		}
	}*/
	
	//methods that choose new horizontal boundaries for each ship at random; called during HandleMovement() when there is a change in direction
	void RandomizeLeftBoundary(){
		leftBoundary = Random.Range(1f, 5.5f);
	}
	void RandomizeRightBoundary(){
		rightBoundary = Random.Range(10.5f, 15f);
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "enemyShot"){
			return; //do not allow enemy shots to affect the enemy ships
		}
		if(collider.tag == "ship"){
			//this trigger is two ships in the same row moving past eachother, so change direction of both ships
			return;
		}
		else{ //this was a trigger from a player shot
			if(hitsLeft > 1){
				HandleNonFatalHits();
			}
			else{
				HandleFatalHits();
			}
		}
	}
	
	//called when a hit to a ship would destroy it
	void HandleFatalHits(){
		//if this is a multi-hit ship taking its last hit, destroy the fire before destroying the ship.
		if(isDamaged){
			Destroy(fire);
		}
		
		//spawn an explosion and explosion sound at the location of the ship before it is destroyed
		AudioSource.PlayClipAtPoint(explosionSound, transform.position);
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
		shipCount--;
		levelManager.CheckShips(); //see if the player won
	}
	
	//called when a hit to a ship would only damage it and not destroy it
	void HandleNonFatalHits(){
		if(!isDamaged){
			//if this is the multi-hit ship's first hit, spawn the fire
			createShipFire();
			isDamaged = true;
		}
		else{
			//if this is not the multi-hit ship's first hit, make the fire bigger
			//fire.particleSystem.emissionRate += 10f;
			//fire.GetComponent<ParticleSystem>().duration += 0.1f;
		}
		hitsLeft--;
	}
	
	//creates a shipFire at the current location of the damaged ship. The shipFire's location will be continuously kept equal to this ship's location in the ship's Update()
	void createShipFire(){
		fire = Instantiate(shipFire, transform.position, Quaternion.identity) as GameObject;
	}
	
}
