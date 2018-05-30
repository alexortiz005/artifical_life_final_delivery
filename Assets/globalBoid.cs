using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalBoid : MonoBehaviour {

	public GameObject fishPrefab;

	public static int numFish = 120;

	public static float tankSize = 5;

	public static ArrayList fishes = new ArrayList();

	public static Vector2 goalPos= Vector2.zero;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < numFish; i++) {
			Vector2 pos = new Vector2 (Random.Range (-tankSize, tankSize),
									   Random.Range (-tankSize, tankSize));
			fishes.Add ((GameObject)Instantiate (fishPrefab, pos, Quaternion.identity));
		}		
	}
	
	// Update is called once per frame
	void Update () {	

		if (Random.Range (0, 10000) < 200) {
			goalPos = new Vector2 (Random.Range (-tankSize, tankSize),
								   Random.Range (-tankSize, tankSize));
		}			
	}

}
