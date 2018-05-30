using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamaScript : MonoBehaviour {

	public float force;
	Rigidbody rb ;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
			
	}
	
	// Update is called once per frame
	void Update () {				
	}

	void FixedUpdate(){
		rb.AddForce(Physics.gravity*(-force));
	}



}
