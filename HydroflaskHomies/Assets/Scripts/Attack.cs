using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour{
	public int damage;
	public string type; 
	public string name;
	public int spCost;
	public bool isSkill;

	public void SetValue(int d, string t, string n, bool skill, int sp){
		damage = d;
		type = t;
		name = n;
		isSkill = skill;
		spCost = sp; 
	}
}

