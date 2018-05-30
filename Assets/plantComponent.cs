using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plantComponent : MonoBehaviour {

	public float upperLimitX;
	public float lowerLimitX;
	public float upperLimitY;
	public float lowerLimitY;

	public float numChunks=3;

	float chunks { get; set; }

	// Use this for initialization
	void Start () {

		chunks = (int) Random.Range (0, numChunks);
		
	}
	
	// Update is called once per frame
	void Update () {

		checkBounds ();		

		if (this.GetComponent<plantComponent> ().chunks <= 0) {
			this.purge ();			
		}	

	}

	void checkBounds(){
		if (this.transform.position.y >= upperLimitY) {
			transform.position = new Vector3(this.transform.position.x,lowerLimitY+0.01f, this.transform.position.z);			
		}

		if (this.transform.position.y <= lowerLimitY) {
			transform.position = new Vector3(this.transform.position.x,upperLimitY-0.01f, this.transform.position.z);			
		}

		if (this.transform.position.x >= upperLimitX) {
			transform.position = new Vector3(lowerLimitX+0.01f,this.transform.position.y, this.transform.position.z);			
		}

		if (this.transform.position.x <= lowerLimitX) {
			transform.position = new Vector3(upperLimitX-0.01f,this.transform.position.y, this.transform.position.z);			
		}
	}


	public void purge(){
		Destroy (gameObject);
		Destroy (this);
		globalPlants.plants.Remove (gameObject);
	}

	public void getBitten(){
		chunks --;
	}

}
