using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalControl : MonoBehaviour {

	//to save this instance 
	public static GlobalControl Instance;

	public List<string> gameObjectsToHide;

	//to store player data we will need within other scenes 
	public int playerLevel; 
	public int[] stats; 
	public List<Attack> attackList; 
	public int currentExperience;
	public int requiredExperience;
	public int maxHP; 
	public int currentHP; 
	public int maxSP;
	public int currentSP;
	public int pt; 

	public string nextFight; 

	void Awake ()   
	{
		if (Instance == null)
		{
			//needed for cross-scene persistence 
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy (gameObject);
		}

		//addHiddenGameObject ("Wrapuchin"); 
		Debug.Log ("Start"); 
	}

	void Start(){
	}

	public void addHiddenGameObject(string objectName){ 
		gameObjectsToHide.Add (objectName);
	} 
}
