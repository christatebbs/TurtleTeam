using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
	public string name; 
	public string description; 
	public string effect; 
	public int healing; 

	public void SetValue(string n, string d, string e, int h){
		name = n; 
		description = d; 
		effect = e; 
		healing = h; 
	}
}
