using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turingMorphScript : MonoBehaviour {

	public float convergeProbability;

	public int innerYRadius;

	public int innerXRadius;

	public int outerYRadius;

	public int outerXRadius;

	public bool randomRatio;

	public float ratio;

	public bool randomInitialDensity;
	
	public float initialDensity;

	public int dimension;

	bool[,] activationGrid;

	bool[,] pastActivationGrid;

	Color activated= new Color(128, 0, 0);
	Color deactivated =new Color(0,128, 0);

	bool converged = false;

	// Use this for initialization
	void Start () {
		if (randomRatio)
			ratio = Random.Range (0.0f, 1.0f);
		if (randomInitialDensity)
			initialDensity = Random.Range (0.0f, 1.0f);
		initActivationGrid ();
		paint ();


	}
	
	// Update is called once per frame
	void Update () {		
		if (!converged) {
			if (Random.Range (0.0f, 1.0f) < convergeProbability) {
				updateActivationGrid ();
				paint ();
				checkConvergence ();
			}
		}			
	}

	void paint(){
		Texture2D texture = new Texture2D(dimension, dimension);
		GetComponent<Renderer>().material.mainTexture = texture;

		for (int y = 0; y < texture.height; y++)
		{
			for (int x = 0; x < texture.width; x++)
			{
				Color color = (activationGrid [y, x])? activated: deactivated;
				texture.SetPixel(x, y, color);
			}
		}
		texture.Apply();
	}

	void initActivationGrid(){
		
		activationGrid = new bool[dimension, dimension];

		for (int i = 0; i < dimension; i++) {
			for (int j = 0; j < dimension; j++) {
				if (Random.Range (0.0f, 1.0f) <= initialDensity) {
					activationGrid [i, j] = true;
				} else {
					activationGrid [i, j] = false;
				}
			}
		}
	}

	int getActivators(int y, int x){
		
		int count = 0;

		for (int i = y - innerYRadius; i <= y + innerYRadius; i++) {
			for (int j = x - innerXRadius; j <= x + innerXRadius; j++) {
				if ((i == y) && (j == x))
					continue;				
				if (activationGrid [mod(i,dimension),mod(j,dimension)])
					count++;			
			}
		}

		return count;

	}

	int getInhibitors(int y , int x){
		
		int count = 0;

		//cuenta los de arriba
		for (int i = y - outerYRadius; i <= y - innerYRadius; i++) {
			for (int j = x - outerXRadius; j <= x + outerXRadius; j++) {			
				if (activationGrid [mod(i,dimension),mod(j,dimension)])
					count++;			
			}
		}
		//cuenta los de la izquierda
		for (int i = y - innerYRadius; i <= y + innerYRadius; i++) {
			for (int j = x - outerXRadius; j <= x - innerXRadius; j++) {			
				if (activationGrid [mod(i,dimension),mod(j,dimension)])
					count++;			
			}
		}

		//cuenta los de la derecha
		for (int i = y - innerYRadius; i <= y + innerYRadius; i++) {
			for (int j = x + innerXRadius; j <= x + outerXRadius; j++) {			
				if (activationGrid [mod(i,dimension),mod(j,dimension)])
					count++;			
			}
		}

		//cuenta los de abajo
		for (int i = y + innerYRadius; i <= y + outerYRadius; i++) {
			for (int j = x - outerXRadius; j <= x + outerXRadius; j++) {			
				if (activationGrid [mod(i,dimension),mod(j,dimension)])
					count++;			
			}
		}

		return count;
	}

	void updateActivationGrid(){
		
		pastActivationGrid = (bool[,]) activationGrid.Clone ();

		for (int i = 0; i < dimension; i++) {
			for (int j = 0; j < dimension; j++) {
				int activators =  getActivators(i,j);
				int inhibitors =  getInhibitors(i,j);
				float diferencia = activators - ratio * inhibitors;
				if (diferencia > 0) {
					activationGrid [i, j] = true;
				}
				if (diferencia < 0) {
					activationGrid [i, j] = false;
				}
			}
		}
		
	}

	int mod(int a, int n) {
		return ((a%n)+n) % n;
	}

	void checkConvergence(){

		if (matrixEquals (activationGrid, pastActivationGrid)) {
			converged = true;
			Debug.Log ("congratulations someone converged!!");
		}
	}

	bool matrixEquals(bool[,] matrix1, bool[,] matrix2){

		bool result = true;
		
		for (int i = 0; i < dimension; i++) {
			for (int j = 0; j < dimension; j++) {
				if (matrix1 [i, j] != matrix2 [i, j])
					return false;
			}
		}	

		return result;
	}

}
