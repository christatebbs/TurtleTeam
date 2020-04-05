using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionDetection : MonoBehaviour {
	private GameObject global;
	public static CollisionDetection Instance;

	void Awake(){ 
		StartCoroutine (Lag ()); 
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
		Debug.Log (SceneManager.GetActiveScene ().name); 
		if (SceneManager.GetActiveScene().name == "Overworld") { 
			gameObject.GetComponent<BoxCollider2D>().enabled = true;
			gameObject.GetComponent<Renderer>().enabled = true;
		} else { 
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			gameObject.GetComponent<Renderer>().enabled = false;
		} 
		global = GameObject.Find ("GlobalObject"); 
		Debug.Log ("Awake"); 
		global.GetComponent<GlobalControl>().nextFight = "";
	}

	void Update(){ 
		if (SceneManager.GetActiveScene().name == "Overworld") { 
			gameObject.GetComponent<BoxCollider2D>().enabled = true;
			gameObject.GetComponent<Renderer>().enabled = true;
		} else { 
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
			gameObject.GetComponent<Renderer>().enabled = false;
		} 
	} 

	void OnCollisionEnter2D(Collision2D collision){
		Debug.Log ("Collision detected"); 
		StartCoroutine (Lag ()); 
		if (collision.gameObject.GetComponent<OverworldObject>().name == "FirstBattle") { 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("FirstBattle");
		} 
		if (collision.gameObject.GetComponent<OverworldObject>().name == "SecondBattle") { 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("SecondBattle");
		} 
		if (collision.gameObject.GetComponent<OverworldObject>().name == "Wrapuchin") { 
			global.GetComponent<GlobalControl>().nextFight = "Wrapuchin"; 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("Battle");
		} else if (collision.gameObject.GetComponent<OverworldObject>().name == "Octopeel") { 
			global.GetComponent<GlobalControl>().nextFight = "Octopeel"; 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("Battle");
		} else if (collision.gameObject.GetComponent<OverworldObject>().name == "Twocan") { 
			global.GetComponent<GlobalControl>().nextFight = "Twocan"; 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("Battle");
		} 
		else if (collision.gameObject.GetComponent<OverworldObject>().name == "Capling") { 
			global.GetComponent<GlobalControl>().nextFight = "Capling"; 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("Battle");
		} else if (collision.gameObject.GetComponent<OverworldObject>().name == "GlassCannon") { 
			global.GetComponent<GlobalControl>().nextFight = "GlassCannon"; 
			collision.gameObject.GetComponent<Renderer> ().enabled = false;
			collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			SceneManager.LoadScene ("Battle");
		} 

	}

	IEnumerator Lag(){ 
		yield return new WaitForSeconds(.5f);
	} 
}
