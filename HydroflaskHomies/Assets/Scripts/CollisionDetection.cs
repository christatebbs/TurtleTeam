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
		StartCoroutine(Lag());
        if(collision.gameObject.tag == "Enemy") { 
            if (collision.gameObject.GetComponent<OverworldObject>().name == "GlassCannon") { 
			    global.GetComponent<GlobalControl>().nextFight = "GlassCannon"; 
			    collision.gameObject.GetComponent<Renderer> ().enabled = false;
			    collision.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
			    collision.gameObject.GetComponent<ObjectHiding> ().hidden = true; 
			    global.GetComponent<GlobalControl>().addHiddenGameObject (collision.gameObject.name); 
			    SceneManager.LoadScene ("SingleBattle");
		    }
            else if (collision.gameObject.GetComponent<OverworldObject>().name == "DimragFightTwo")
		    {
			    collision.gameObject.GetComponent<Renderer>().enabled = false;
			    collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			    collision.gameObject.GetComponent<ObjectHiding>().hidden = true;
			    global.GetComponent<GlobalControl>().addHiddenGameObject(collision.gameObject.name);
			    SceneManager.LoadScene("DimragOne");
		    }
		}
	}

	IEnumerator Lag(){ 
		yield return new WaitForSeconds(.5f);
	} 
}
