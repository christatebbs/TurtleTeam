using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PTCounter_Running : MonoBehaviour {
	private GameObject global; 
	public Text ptCounter; 
	// Use this for initialization

	void Start () {
		global = GameObject.Find ("GlobalObject");
		ptCounter.text = "Total PT: " + global.GetComponent<GlobalControl>().pt; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
