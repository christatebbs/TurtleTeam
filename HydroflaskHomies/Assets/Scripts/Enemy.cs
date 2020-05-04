using UnityEngine;

public class Enemy : MonoBehaviour {
	//keep track of unit info and update UI
	public string unitName;
	public int unitLevel;
	public bool isKilled;

	private Item drop;
	public string itemName;
	public string itemDescription;
	public string itemEffect; 
	public int itemHealing; 
 	
	public int dropChance; 

	//keep track of what damage it does
	//public int damage;
	public int[] stats = new int[5]; 

	//keep track of health and current health
	public int maxHP; 
	public int currentHP; 

	//Keeps track of enemy type 
	public string type;

	//Keeps track of currency and EXP on drop 
	public int experience; 
	public int pt; 

	void Start(){ 
		drop = gameObject.AddComponent<Item> (); 
		drop.name = itemName; 
		drop.description = itemDescription; 
		drop.effect = itemEffect; 
		drop.healing = itemHealing; 
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
