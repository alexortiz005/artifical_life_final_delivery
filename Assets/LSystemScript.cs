using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemScript : MonoBehaviour {

	static readonly float timePeriod=1200;

	static readonly float sigmaTime=50;

	public float springTime;

	public TextAsset rulesFile; 

	public int iterations;

	public float delta;

	public GameObject rama;

	public GameObject comida;

	float probSpawnComida;

	Dictionary<string, string> rules = new Dictionary<string, string>();

	string axiom;

	string conclusion;

	Vector3 spawnPosition;

	Quaternion spawnRotation;

	Quaternion spawnPositionRotation;

	float spawnSize;

	float halfSpawnSize;

	Vector3 step;

	Vector3 halfStep;

	Vector3 halfAngledStep ;

	GameObject ramaPrevia;

	GameObject ramaRaiz;

	ArrayList ramas = new ArrayList();

	int rotationCounter=0;

	bool turning=false;

	Stack<GameObject> ramasPrevias = new Stack<GameObject>();
	Stack<Vector3> spawnPositions = new Stack<Vector3>();
	Stack<Quaternion> spawnRotations = new Stack<Quaternion>();
	Stack<Quaternion>  spawnPositionRotations = new Stack<Quaternion> ();
	Stack<int> rotationCounters = new Stack<int>();


	// Use this for initialization
	void Start () {		

		probSpawnComida = 0;

		makeRules ();
		makeConclusion ();
		makeTree ();

	}


	// Update is called once per frame
	void Update () {
		float frame = Time.frameCount % timePeriod;

		if (Random.Range (0.0f, 1.0f) < probSpawnComida) {
			makeComida ();			
		}

		probSpawnComida=Gaussian(frame,springTime,sigmaTime);
	}

	public void makeComida(){
		int index = Random.Range (0, ramas.Count);
		GameObject ramaSeleccionada =(GameObject) ramas [index];
		GameObject new_plant = (GameObject)Instantiate(comida, ramaSeleccionada.transform.position,Quaternion.Euler(-90,0,0));
		globalPlants.plants.Add (new_plant);
	}

	string[] readTextFileLines() { 		
		return rulesFile.text.Split('\n');
	}

	void makeRules (){

		string [] textFileRules = readTextFileLines ();

		bool foundAxiom = false;
		foreach (var rule in textFileRules) {
			if (!foundAxiom) {
				foundAxiom = true;
				axiom = rule;
			} else {
				string[] splittedRule = rule.Split ('→');	
				if (splittedRule.Length == 2) {
					string key = splittedRule [0];
					string value = splittedRule [1];
					rules.Add (key, value);
				}
			}
		}

	}

	void makeConclusion(){
		conclusion = axiom;
		for (int i = 0; i < iterations; i++) {
			string newConclusion = "";
			foreach (char c in conclusion)
			{
				if (rules.ContainsKey (c.ToString ())) {
					newConclusion += rules [c.ToString ()];
				} else {
					newConclusion += c.ToString ();
				}
			}
			conclusion = newConclusion;
		}
	}

	void makeTree(){
		
		ramaRaiz = (GameObject)Instantiate(rama, transform.position, rama.transform.rotation);
		ramaRaiz.transform.SetParent (this.transform);

		Destroy (ramaRaiz.GetComponent<SpringJoint> ());
		Rigidbody rb = ramaRaiz.GetComponent<Rigidbody> ();
		rb.isKinematic = true;

		ramaPrevia = ramaRaiz;

		spawnPosition = ramaPrevia.transform.position;
		spawnSize = ramaPrevia.transform.localScale.y;
		halfSpawnSize = spawnSize / 2;
		spawnRotation = ramaPrevia.transform.rotation;

		processString ();

	}

	public void makeChild(){	
		rotationCounter = 0;	
		updateTurtle ();
		if (turning) {
			turning = false;
			spawnPosition =spawnPosition +halfStep+halfAngledStep ;
		} else {
			spawnPosition += step;
		}

		GameObject newRama = (GameObject)Instantiate(rama, spawnPosition, spawnRotation);
		newRama.transform.SetParent (this.transform);

		SpringJoint hg = newRama.GetComponent<SpringJoint> ();
		hg.connectedBody = ramaPrevia.GetComponent<Rigidbody>();

		ramas.Add (newRama);

		ramaPrevia= newRama;
	}

	void updateTurtle(){

		step = (Vector3.Normalize (ramaPrevia.transform.up) * spawnSize);
		halfStep = (Vector3.Normalize (ramaPrevia.transform.up) * halfSpawnSize);
		halfAngledStep = spawnPositionRotation * halfStep;
	}


	void rotateClockwise(){
		rotationCounter--;
		rotate ();
	}

	void rotateAnticlockwise(){
		rotationCounter++;
		rotate ();
	}

	void rotate(){
		turning = true;
		spawnRotation = Quaternion.AngleAxis(rotationCounter*delta, ramaRaiz.transform.forward)*ramaPrevia.transform.rotation;
		spawnPositionRotation = Quaternion.AngleAxis (rotationCounter*delta, ramaRaiz.transform.forward);		

	}

	void processString(){
		foreach (char c in conclusion) {
			switch (c) {
			case 'F':
				makeChild ();
				break;
			case '-':
				rotateClockwise ();
				break;
			case '+':
				rotateAnticlockwise ();
				break;
			case '[':
				ramasPrevias.Push (ramaPrevia);
				spawnPositions.Push (spawnPosition);
				spawnRotations.Push (spawnRotation);
				spawnPositionRotations.Push (spawnPositionRotation);
				rotationCounters.Push (rotationCounter);
				break;
			case ']':
				ramaPrevia = ramasPrevias.Pop ();
				spawnPosition = spawnPositions.Pop ();
				rotationCounter = rotationCounters.Pop();
				spawnRotation = spawnRotations.Pop ();
				spawnPositionRotation = spawnPositionRotations.Pop ();
				updateTurtle ();
				break;
			default:
				break;
			}
		}
	}

	public float Gaussian(float x, float mu, float sigma){
		return Mathf.Exp (-(x - mu)*(x - mu)  / (2 * sigma *sigma));
	}



}
