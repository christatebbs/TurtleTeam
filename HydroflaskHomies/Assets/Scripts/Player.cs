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

	private GameObject global; 
	private PlayerLevel playerLevel; 

	//this function subtracts damage to update the currentHP
	//it also returns either true or false depending on if the unit has died 

	void Start(){ 
		global = GameObject.Find("GlobalObject"); 
		GlobalControl globalObject = global.GetComponent<GlobalControl> (); 
		playerLevel = gameObject.GetComponent<PlayerLevel> ();
	} 

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

	public void addItem(Item item){ 
		inventoryList.Add (item); 
	} 

	public void addItemFromDrop(Item item){ 
		Item toAdd = global.AddComponent<Item> (); 
		toAdd.name = item.name; 
		toAdd.description = item.description; 
		toAdd.effect = item.effect; 
		toAdd.healing = item.healing;
		inventoryList.Add (toAdd); 
	} 

	public void savePlayerData(){ 
		GlobalControl.Instance.attackList = attackList;
		GlobalControl.Instance.inventoryList = inventoryList; 
		GlobalControl.Instance.maxHP = maxHP; 
		GlobalControl.Instance.currentHP = currentHP;
		GlobalControl.Instance.maxSP = maxSP; 
		GlobalControl.Instance.currentSP = currentSP;
		GlobalControl.Instance.pt = pt; 
	}

	public void loadPlayerData(){ 
		attackList = GlobalControl.Instance.attackList;
		inventoryList = GlobalControl.Instance.inventoryList; 
		maxHP = GlobalControl.Instance.maxHP; 
		currentHP = GlobalControl.Instance.currentHP;
		maxSP = GlobalControl.Instance.maxSP; 
		currentSP = GlobalControl.Instance.currentSP;
		pt = GlobalControl.Instance.pt; 

	} 
}
