using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {
	private GameObject global; 
	// Use this for initialization
	void Start () {
		global = GameObject.Find ("GlobalObject"); 
		object[] obj = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (object o in obj)
		{
			GameObject g = (GameObject) o;
			string name = g.name; 
			bool toHide = false; 
			for (int i = 0; i < global.GetComponent<GlobalControl>().gameObjectsToHide.Count; i++) { 
				if(name == global.GetComponent<GlobalControl>().gameObjectsToHide[i]){ 
					toHide = true; 
					Debug.Log ("hid " + name); 
				}
			} 
			if (!toHide) { 
				g.GetComponent<ObjectHiding>().hidden = false; 
			} 
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
