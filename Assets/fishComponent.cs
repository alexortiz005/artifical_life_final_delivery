using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishComponent : MonoBehaviour {


	public GameObject fishPrefab;

	Animator m_Animator;

	public float upperLimitX;
	public float lowerLimitX;
	public float upperLimitY;
	public float lowerLimitY;
	public float upperLimitZ=5;
	public float lowerLimitZ=-5;


	public float fishesSpeed = 2.0f;
	float max_speed;
	float speed;
	public float rotationSpeed = 4.0f;
	Vector2 averageHeading;
	Vector2 averagePosition;
	public float fishesVision = 4.0f;
	public float probReproduction=0.1f;
	public float probMutation=0.01f;
	public float fishes_health=100;
	public float scaleLimit=3.0f;
	float vision;
	float max_health;
	float health;
	float metabolism;
	public bool isAlive;

	bool turning = false;
	bool predefined=false;

	int grow_times=0;

	float x_modifier;
	float y_modifier;
	float z_modifier;



	void Start () {
		
		m_Animator = gameObject.GetComponent<Animator>();

		if (predefined) {
			
		} else {

			max_health=Random.Range(fishes_health*(9/10),fishes_health);

			max_speed=Random.Range (0, fishesSpeed);

			vision = Random.Range (0, fishesVision);

			grow_times = 10;

			x_modifier=Random.Range(0.2f,1.5f);
			y_modifier=Random.Range(0.2f,1.5f);
			z_modifier=Random.Range(0.2f,1.5f);

		}

		health = max_health;

		isAlive = true;

		speed = Random.Range (max_speed*(3/4), max_speed);

		metabolism = max_speed/2;

		transform.localScale = new Vector3(x_modifier, y_modifier, z_modifier);	


	}
	
	// Update is called once per frame
	void Update () {

		if (Random.Range (0, 100) < 1) {
			if (grow_times < 10) {
				grow_times++;
				transform.localScale = new Vector3(1.0717F, 1.0717f, 1.0717f);	
			}			
		}

		if (health <= 0) {
			isAlive = false;
			this.GetComponent<Rigidbody> ().useGravity= true;
		}

		if (Random.Range (0, 100) < 2) {
			health-=metabolism;
		}

		if (this.transform.position.y >= upperLimitY) {
			transform.position = new Vector3(this.transform.position.x,lowerLimitY+0.01f, this.transform.position.z);			
		}

		if (this.transform.position.y <= lowerLimitY) {
			if (!isAlive) {
				this.purge ();				
				return;
			}			
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


		if (!isAlive) {
			speed = 0;
			m_Animator.speed = 0;
			return;
		}

		if (turning) {
			Vector3 direction = (Vector3) globalBoid.goalPos - (Vector3)transform.position;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), rotationSpeed * Time.deltaTime);
			speed = Random.Range (max_speed*(3/4), max_speed);	

		} else {			
			ApplyRules ();
		}

		transform.Translate (0, 0, Time.deltaTime * speed);
		m_Animator.speed = speed * 5;
		
	}

	void ApplyRules(){

		ArrayList fishes;
		GameObject[] predators;
		ArrayList plants;


		fishes = globalBoid.fishes;
		predators = globalPack.predators;
		plants = globalPlants.plants;


		Vector2 vcenter = Vector2.zero;
		Vector2 vavoid = Vector2.zero;
		Vector2 vlove = Vector2.zero;

		bool danger = false;

		float gSpeed = 2.0f;

		Vector2 goalPos = globalBoid.goalPos;

		float dist;

		int groupSize = 0;

		foreach (GameObject fish in fishes ) {

			fishComponent otherFishComponent = fish.GetComponent<fishComponent> ();

			if (fish != this.gameObject) {
				dist = Vector2.Distance (this.transform.position , fish.transform.position);
				if (dist <= vision) {
					vcenter += (Vector2)fish.transform.position;
					groupSize++;

					if (dist < 0.5f) {
						vavoid +=  (Vector2)(this.transform.position - fish.transform.position);
					}

					gSpeed = gSpeed + otherFishComponent.speed;
				}
			}			
		}

	

		foreach (GameObject predator in predators) {			

			dist = Vector3.Distance (this.transform.position , predator.transform.position);
			if (dist <= vision/2) {
				danger = true;
				vavoid +=  (Vector2)(this.transform.position - predator.transform.position);
			}	

		}


		foreach (GameObject plant in plants) {

			dist = Vector3.Distance (this.transform.position , plant.transform.position);

			if (dist <= vision) {
				goalPos = plant.transform.position;
				vcenter=plant.transform.position;
				break;

			}

		}

		if (groupSize > 0) {
			
			vcenter =  ((Vector3) vcenter / groupSize) + (Vector3)goalPos*2- (Vector3) this.transform.position;
			speed = (gSpeed / groupSize);

			if (speed > max_speed)speed = max_speed;	

			Vector2 direction = Vector2.zero;

			if (danger) {
				direction = (vavoid) - (Vector2)this.transform.position;;
			} else {
				direction = (vcenter + vavoid) - (Vector2)this.transform.position;;
			}

			 


			if (direction != Vector2.zero) {
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), rotationSpeed * Time.deltaTime);
			}

		}

	}

	void OnCollisionEnter (Collision col)
	{
		
		switch (col.gameObject.tag) {

		case "Predator":
			this.purge ();
			break;

		case "Fish":					
			
			GameObject otherFish = col.gameObject;
			fishComponent otherFishComponent = otherFish.GetComponent<fishComponent> ();

			if (grow_times >= 10) {			
				if (Random.Range (0.0f, 1.0f) < probReproduction) {
					reproduce (otherFishComponent);
				}
			}
			break;

		case "Plant":			
			this.GetComponent<fishComponent> ().hurt (-10);
			col.gameObject.GetComponent<plantComponent> ().getBitten ();
			break;
		
		default:
			break;
		}	

	}

	void reproduce(fishComponent otherFishComponent){
		hurt (20);	
		GameObject newFish;
		newFish= Instantiate (fishPrefab, this.transform.position+Random.insideUnitSphere*0.2f, Quaternion.identity) as GameObject;
		newFish.transform.localScale = new Vector3(0.5F, 0.5f, 0.5f);	

		BitArray thisFishBinary = toBinary ();
		BitArray otherFishBinary = otherFishComponent.toBinary ();

		BitArray genome = recombineBinary (thisFishBinary, otherFishBinary) [Random.Range (0, 1)];

		float newMaxHealth = Mathf.Abs(getPartOfBinary32AsFloat (genome, 0));
		float newMaxSpeed = Mathf.Abs(getPartOfBinary32AsFloat (genome, 1));
		float newVision = Mathf.Abs(getPartOfBinary32AsFloat (genome, 2));

		float newXModifier = Mathf.Abs(getPartOfBinary32AsFloat (genome, 3));
		float newYModifier = Mathf.Abs(getPartOfBinary32AsFloat (genome, 4));
		float newZModifier = Mathf.Abs(getPartOfBinary32AsFloat (genome, 5));

		if (newXModifier > scaleLimit)
			newXModifier = scaleLimit;
		if (newYModifier > scaleLimit)
			newYModifier = scaleLimit;
		if (newZModifier > scaleLimit)
			newZModifier = scaleLimit;
				

		globalBoid.fishes.Add (newFish);	
		newFish.GetComponent<fishComponent> ().predefine(newMaxHealth,newMaxSpeed,newVision,newXModifier,newYModifier,newZModifier);

	}

	float getPartOfBinary32AsFloat(BitArray current,int i){
		BitArray result = sliceBinary (current, i * 32, ((i + 1) * 32)-1);
		return binaryToFloat (result);
	}


	BitArray[] recombineBinary(BitArray current, BitArray other){
		
		BitArray[] result = new BitArray[2];
		current = mutateBinary (current);
		other = mutateBinary (other);

		int index = Random.Range (0, current.Length-1);

		BitArray currentBegin = sliceBinary (current, 0, index);
		BitArray currentEnd = sliceBinary (current, index+1, current.Length-1);

		BitArray otherBegin = sliceBinary (other, 0, index);
		BitArray otherEnd = sliceBinary (other, index+1, other.Length-1);

		result [0] = concatBinary (currentBegin,otherEnd);
		result [1] = concatBinary (otherBegin,currentEnd);

		return result;

	}

	BitArray mutateBinary(BitArray binary){

		for (int i = 0; i < binary.Length; i++) {
			if (Random.Range (0.0f, 1.0f) < probMutation) {
				bool value = binary.Get (i);
				binary.Set (i, !value);
			}
		}

		return binary;
		
	}

	public BitArray toBinary(){

		BitArray result = new BitArray(0);
		
		BitArray maxHealthBinary = floatToBinary (max_health);
		BitArray maxSpeedBinary = floatToBinary (max_speed);
		BitArray visionBinary = floatToBinary (vision);
		BitArray xModifierBinary = floatToBinary (x_modifier);
		BitArray yModifierBinary = floatToBinary (y_modifier);
		BitArray zModifierBinary = floatToBinary (z_modifier);

		result = concatBinary (result, maxHealthBinary);
		result = concatBinary (result, maxSpeedBinary);
		result = concatBinary (result, visionBinary);
		result = concatBinary (result, xModifierBinary);
		result = concatBinary (result, yModifierBinary);
		result = concatBinary (result, zModifierBinary);

		return result;
		
	}

	BitArray floatToBinary(float f){

		int intfloat = (int)(f * 1000000);
		return new BitArray(new int[] { intfloat } );
		
	}

	BitArray concatBinary(BitArray current, BitArray after) {
		var bools = new bool[current.Count + after.Count];
		current.CopyTo(bools, 0);
		after.CopyTo(bools, current.Count);
		return new BitArray(bools);
	}

	float binaryToFloat(BitArray ba){		

		int[] array = new int[1];
		ba.CopyTo(array, 0);
		int result=array[0];
		return result / 1000000.0f;
	}


	public void hurt(int amount){
		health -= amount;
	}

	public BitArray sliceBinary(BitArray current, int start, int end){
		var bools = new bool[end - start+1];
		int j = 0;
		for (int i = start; i <= end; i++) {
			bools [j] = current.Get (i);		
			j++;
		}

		return new BitArray(bools);
	}


	public void purge(){
		Destroy (gameObject);
		Destroy (this);
		globalBoid.fishes.Remove (gameObject);
	}

	public void predefine(float p_maxHealth, float p_maxSpeed, float p_vision,float p_Xmodifier,float p_Ymodifier,float p_Zmodifier){
		max_health = p_maxHealth;
		max_speed = p_maxSpeed;
		vision = p_vision;
		x_modifier = p_Xmodifier;
		y_modifier = p_Ymodifier;
		z_modifier = p_Zmodifier;
		predefined = true;	
	}


	//Getters and Setters


	public float Max_speed {
		get {
			return this.max_speed;
		}
		set {
			max_speed = value;
		}
	}

	public float Vision {
		get {
			return this.vision;
		}
		set {
			vision = value;
		}
	}

	public float Max_health {
		get {
			return this.max_health;
		}
		set {
			max_health = value;
		}
	}

	public float Metabolism {
		get {
			return this.metabolism;
		}
		set {
			metabolism = value;
		}
	}

	public float Health {
		get {
			return this.health;
		}
		set {
			health = value;
		}
	}

	public void setHealth(int amount){
		health = amount;
	}

	public double getHealth(){
		return health;
	}

	public float X_modifier {
		get {
			return this.x_modifier;
		}
		set {
			x_modifier = value;
		}
	}

	public float Y_modifier {
		get {
			return this.y_modifier;
		}
		set {
			y_modifier = value;
		}
	}

	public float Z_modifier {
		get {
			return this.z_modifier;
		}
		set {
			z_modifier = value;
		}
	}


}
