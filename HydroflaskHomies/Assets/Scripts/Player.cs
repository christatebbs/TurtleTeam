using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	//keep track of unit info and update UI
	public string unitName;
	public int unitLevel;

	//keep track of health and current health
	public int maxHP; 
	public int currentHP; 

	//keep track of players skill points 
	public int maxSP; 
	public int currentSP; 

	//to keep track of total currency the player has
	public int pt; 

	//Hopefully will work to hold and dynamically change skills
	public List<Attack> attackList;
	public List<Item> inventoryList; 

	//this function subtracts damage to update the currentHP
	//it also returns either true or false depending on if the unit has died 
	public bool TakeDamage(int damage) { 
		currentHP -= damage; 
		if (currentHP <= 0) { 
			return true; 
		} else { 
			return false;
		}
	}

	public void useSP(int points){ 
		currentSP -= points; 
		if (currentSP < 0) { 
			currentSP = 0; 
		}
	}

	public void HealDamage(int amount) { 
		currentHP += amount; 
		if (currentHP > maxHP) { 
			currentHP = maxHP; 
		}
	}

	public void addAttack(Attack attack) {
		attackList.Add (attack);
	}
}
