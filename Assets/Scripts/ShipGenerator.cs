using UnityEngine;
using System.Collections;

public class ShipGenerator : MonoBehaviour {
	
	//defined in the inspector as the respective ship prefabs
	public GameObject oneHit;
	public GameObject twoHit;
	public GameObject threeHit;
	
	//these are unique to each level and are definied in Start()
	private int[] shipList; //this represents an unordered collection of ships unique to each level; it indicates how many of each type of ship (1hit, 2hit, 3hit) the current level should have spawned
	private int numberOfSpawnPoints; 
	private int numberOfShips; 
	private int avgNumberOfShipsPerSpawnPoint; 
	
	//using the Knuth shuffle algorithm to shuffle the shipList
	int[] Shuffle(int[] unshuffled){
		int[] unshuffledCopy = unshuffled;
		for(int i = 0; i < unshuffledCopy.Length; i++){
			int temp = unshuffled[i];
			int j = Random.Range(i,unshuffledCopy.Length);
			unshuffledCopy[i] = unshuffledCopy[j];
			unshuffledCopy[j] = temp;	
		}
		return unshuffledCopy;
	}


	void Start () {
		
		shipList = LevelManager.getShipList(); //retrieve the shipList for the current level from LevelManager's settings
	
		int[] shuffledShipList = Shuffle(shipList);
		numberOfSpawnPoints = transform.childCount;
		numberOfShips = shuffledShipList.Length;
		avgNumberOfShipsPerSpawnPoint = numberOfShips/numberOfSpawnPoints;
	
	
		//this algorithm spawns the appropriate amount of each type of ship (1hit, 2hit, 3hit) for this level as specified by shipList
		//although the number of each type of ship is always constant for each level, the arrangement of these ships' spawn points is always random
		//for instance, for a shipList of [1,1,2,3], 2 1hit ships, 1 2hit ship, and 1 3hit ship will always be spawned, but the arrangement of these 4 ships is random
		//this algorithm is designed to work for any amount of spawn points and any amount of ships listed in shipList
		//if the number of spawn points does not divide evenly into the number of ships that need to be spawned, any remainder ships are spawned from the highest spawn point (greatest priority) to the lowest spawn point (least priority) 
		//note that this algorithm assumes that the spawn points are all arranged as children of the Ships object, which this script is attached to
		int spawnedShipCount = 0;
		while(spawnedShipCount<numberOfShips){
			for (int i = 0; i<transform.childCount; i++){
				for (int j = 0; j<avgNumberOfShipsPerSpawnPoint && spawnedShipCount<shuffledShipList.Length; j++){
					print (spawnedShipCount);
					switch(shuffledShipList[spawnedShipCount]){
						case 1:
							GameObject enemy1Ship = Instantiate(oneHit, transform.GetChild(i).transform.position, Quaternion.identity) as GameObject;
							enemy1Ship.transform.parent = transform.GetChild(i);
							break;
						case 2:
							GameObject enemy2Ship = Instantiate(twoHit, transform.GetChild(i).transform.position, Quaternion.identity) as GameObject;
							enemy2Ship.transform.parent = transform.GetChild(i);
							break;
						case 3:
							GameObject enemy3Ship = Instantiate(threeHit, transform.GetChild(i).transform.position, Quaternion.identity) as GameObject;
							enemy3Ship.transform.parent = transform.GetChild(i);
							break;
					}
					spawnedShipCount++; 
				}
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
