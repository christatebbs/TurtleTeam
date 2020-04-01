using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	//keep track of unit info and update UI
	public string unitName;
	public int unitLevel;
	public bool isKilled;

	//keep track of what damage it does
	//public int damage;
	public int[] stats = new int[5]; 

	//Similar to our player, the enemy will need a list of Attacks to cycle between. 

	//public List<Attack> attackList;

	//keep track of health and current health
	public int maxHP; 
	public int currentHP; 

	//Keeps track of enemy type 
	public string type;

	//Keeps track of currency and EXP on drop 
	public int experience; 
	public int pt; 

	public void Start(){ 
		
	} 
	//this function subtracts damage to update the currentHP
	//it also returns either true or false depending on if the unit has died 
	public bool TakeDamage(int damage){ 
		currentHP -= damage; 
		if (currentHP <= 0) { 
			return true; 
		} else { 
			return false;
		}
	}

	public void HealDamage(int amount){ 
		currentHP += amount; 
		if (currentHP > maxHP) { 
			currentHP = maxHP; 
		}
	}
}
