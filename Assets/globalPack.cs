using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalPack : MonoBehaviour {

	public GameObject predatorPrefab;

	public int numPredators;

	public static int tankSize = 5;

	public static GameObject[] predators;

	// Use this for initialization
	void Start () {
		
		predators = new GameObject[numPredators];

		for (int i = 0; i < numPredators; i++) {
			Vector2 pos = new Vector2 (Random.Range (-tankSize, tankSize),
				Random.Range (-tankSize, tankSize));
			predators [i] = (GameObject)Instantiate (predatorPrefab, pos, Quaternion.identity);				
		}		
	}

	// Update is called once per frame
	void Update () {

	}
}
