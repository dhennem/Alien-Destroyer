using UnityEngine;
using System.Collections;

public class CommanderShip : MonoBehaviour {

	public int maxHits;
	
	public GameObject shot;
	public GameObject explosion;
	public GameObject shipFire;
	public AudioClip explosionSound;
	public AudioClip enemyShotSound;
	
	
	
	private LevelManager levelManager;
	private int hitsLeft;
	private float height; //defined in Start()
	private float speed; //the speed for the commanderShip is different for each stage; defined in start()
	private bool movingRight; //the direction for each commanderShip is random with each start()
	private bool movingUp;
	private float leftBoundary; //the horizontal and vertical boundaries for the commanderShip are randomly chosen in start()
	private float rightBoundary;
	private float upperBoundary;
	private float lowerBoundary;
	private bool isDamaged; //defined as true once the commanderShip takes its first hit
	private GameObject fire; //defined in createShipFire() when a commanderShip is damaged
	//general settings for every ship
	private float chanceOfEnemyShot; //chanceOfEnemyShot and enemyShotVelocity are retrieved from LevelManager's levelSettings during Start()
	private float enemyShotVelocity; 


	void Awake(){
		Ship.shipCount += 1;
	}

	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager>();
		hitsLeft = maxHits;
		isDamaged = false;
		speed = LevelManager.getMaxShipSpeed();
		chanceOfEnemyShot = LevelManager.getChanceOfEnemyShot() * 2; //commander ships are twice as likely to fire a shot than regular ships
		enemyShotVelocity = LevelManager.getEnemyShotVelocity();
		rigidbody2D.transform.position = new Vector3(8f,6f,0f);
		if(Random.Range(0f,1f)>0.5f) movingRight = true;
		if(Random.Range(0f,1f)>0.5f) movingUp = true;
		RandomizeLeftBoundary();
		RandomizeRightBoundary();
		RandomizeUpperBoundary();
		RandomizeLowerBoundary();
	
	}
	
	void Update () {
		HandleMovement();
		if(Random.Range(0f,1f)<(chanceOfEnemyShot)){
			EnemyShot(); 
		}
		//if the ship is damaged keep the fire's location equal to the damaged ship's location
		if(isDamaged){
			fire.transform.position = transform.position;
		}
	
	}
	
	void HandleMovement(){
		//the movement of the commander ship is handled in this method
		Vector2 shipPosition = gameObject.transform.position;
		
		//handle changes in direction once horizontal boundaries are reached
		if(shipPosition.x >= rightBoundary){
			//the right boundary has been crossed
			movingRight = false;
			RandomizeLeftBoundary();
		}
		if(shipPosition.x <= leftBoundary){
			//the left boundary has been crossed
			movingRight = true;
			RandomizeRightBoundary();
		}
		
		//handle changes in direction once vertical boundaries are reached
		if(shipPosition.y >= upperBoundary){
			//the upper boundary has been crossed
			movingUp = false;
			RandomizeLowerBoundary();
		}
		if(shipPosition.y <= lowerBoundary){
			//the lower boundary has been crossed
			movingUp = true;
			RandomizeUpperBoundary();
		}
		
		//move the ship left or right depending on its current direction of movement
		if(movingRight){
			gameObject.transform.position = new Vector2(shipPosition.x + 0.1f*speed, shipPosition.y);
		}
		else if(!movingRight){
			gameObject.transform.position = new Vector2(shipPosition.x - 0.1f*speed, shipPosition.y);
		}
		//move the ship up or down depending on its current direction of movement
		if(movingUp){
			gameObject.transform.position += new Vector3(0f, 0.075f*speed, 0f);
		}
		else if(!movingUp){
			gameObject.transform.position += new Vector3(0f, -0.075f*speed, 0f);
		}
		
	
	}
	
	//methods that choose new horizontal and vertical boundaries for the commmanderShip at random; called during HandleMovement() when there is a change in direction
	void RandomizeLeftBoundary(){
		leftBoundary = Random.Range(1f, 5.5f);
	}
	void RandomizeRightBoundary(){
		rightBoundary = Random.Range(10.5f, 15f);
	}
	void RandomizeUpperBoundary(){
		upperBoundary = Random.Range(7.5f, 11f);
	}
	void RandomizeLowerBoundary(){
		lowerBoundary = Random.Range(4f, 6.5f);
	}
	
	void EnemyShot(){
		//creates a shot with downwards velocity from the commander ship
		Vector3 commanderShipPosition = transform.position;
		Vector3 enemyShotPosition = new Vector3(commanderShipPosition.x, commanderShipPosition.y-0.5f, 0);
		GameObject enemyShot = Instantiate(shot, enemyShotPosition, Quaternion.identity) as GameObject;
		if(movingRight){
			enemyShot.rigidbody2D.velocity = new Vector2(Random.Range(0f,10f), enemyShotVelocity);
		}
		else{
			enemyShot.rigidbody2D.velocity = new Vector2(Random.Range(-10f,0f), enemyShotVelocity);

		}
		enemyShot.transform.parent = GameObject.Find("EnemyShots").transform;
		//spawns an enemy shot sound at the location of the ship once it shoots
		AudioSource.PlayClipAtPoint(enemyShotSound, transform.position);
		
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "enemyShot"){
			return; //do not allow enemy shots to affect the commander ship
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
		Ship.shipCount -= 1;
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
