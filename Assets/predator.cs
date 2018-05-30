using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predator : MonoBehaviour {

	public float upperLimitX;
	public float lowerLimitX;
	public float upperLimitY;
	public float lowerLimitY;
	public float upperLimitZ=5;
	public float lowerLimitZ=-5;

	public ParticleSystem blood;
	public AudioSource crunch;

	public float max_speed = 0.1f;
	float speed;
	public float rotationSpeed = 4.0f;
	public float neighbourDistance = 2.0f;
	public float loveDistance = 3.0f;

	float maxDistPrey=0;


	bool turning = false;

	Animator m_Animator;

	// Use this for initialization
	void Start () {
		speed = Random.Range (0.1f, max_speed);	
		m_Animator = gameObject.GetComponent<Animator>();		
	}
	
	// Update is called once per frame
	void Update () {

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

		if (this.transform.position.z>=upperLimitZ||this.transform.position.z<=lowerLimitZ) {
			turning = true;
		} else {
			turning = false;
		}

		if (turning) {
			Vector3 direction = Vector3.zero - (Vector3)transform.position;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), rotationSpeed * Time.deltaTime);
			speed = Random.Range (0.1f, max_speed);	

		} else {
				ApplyRules ();
		}	

		transform.Translate (0, 0, Time.deltaTime * speed);
		m_Animator.speed = speed * 5;
		
	}

	void ApplyRules(){
		ArrayList fishes;
		GameObject[] predators;

		fishes = globalBoid.fishes;
		predators = globalPack.predators;

		Vector2 vavoid = Vector2.zero;
		Vector2 vlove = Vector2.zero;


		float dist;


		foreach (GameObject fish in fishes ) {

			dist = Vector2.Distance (this.transform.position , fish.transform.position);
			if (dist <= loveDistance) {


				vlove +=  (Vector2)(fish.transform.position- this.transform.position );

			}
						
		}


		foreach (GameObject predator in predators) {	

			if (predator != this.gameObject) {
				dist = Vector3.Distance (this.transform.position , predator.transform.position);
				if (dist <= neighbourDistance) {
					vavoid +=  (Vector2)(this.transform.position - predator.transform.position);
				}		
			}				

		}


			
		Vector2 direction = ( vlove + vavoid) - (Vector2)transform.position;

		if (direction.Equals (Vector3.zero)) {
			Debug.Log ("aca pasa algo");
		}

		if (direction != Vector2.zero) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), rotationSpeed * Time.deltaTime);
		}

		float distPrey = Vector3.Magnitude (vlove);

		if (distPrey > maxDistPrey)
			maxDistPrey = distPrey;

		speed = max_speed * ((distPrey+1)/maxDistPrey);


	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "Fish(Clone)") {
			blood.Play ();
			crunch.Play ();
		}

	}


	public void purge(){
		Destroy (gameObject);
		Destroy (this);
		globalBoid.fishes.Remove (gameObject);
	}




}
