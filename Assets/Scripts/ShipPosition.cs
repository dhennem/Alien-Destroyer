using UnityEngine;
using System.Collections;

public class ShipPosition : MonoBehaviour {

	//draws a circle around each ship spawn location in the scene view
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, 1);
	}
}
