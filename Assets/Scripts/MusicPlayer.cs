using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
	
	static MusicPlayer instance = null;

	void Awake(){
		//destroys duplicate music player objects when the game loops through the start screen
		if(instance!=null){
			Destroy(gameObject);
		}
		else{
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}

}
