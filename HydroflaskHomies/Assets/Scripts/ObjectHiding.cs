using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class ObjectHiding : MonoBehaviour {
	public static ObjectHiding Instance; 
	public bool hidden = false; 

	void Awake(){ 

	} 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene().name == "Overworld" && !hidden) { 
			gameObject.GetComponent<BoxCollider2D>().enabled = true;
			gameObject.GetComponent<Renderer>().enabled = true;
		} else { 
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			gameObject.GetComponent<Renderer>().enabled = false;
		} 
	}

	void OnCollisionEntered2D(Collision2D collision){ 
		hidden = true; 
		Debug.Log ("fast"); 
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		gameObject.GetComponent<Renderer>().enabled = false;
	} 
}
