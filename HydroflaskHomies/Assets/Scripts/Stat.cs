using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This doesn't need to be attached to any gameObject, so we don't need to keep monobehavior for stats. 
//Removing monobehavior allows for this to be used as a regular class with a constructor.
public class Stat{
	public int BaseValue { get; set; } 
	public string StatName { get; set; } 
	public string StatDescription { get; set; } 

	//A statbonus is different from base stat, as it may be a temporary increase due to item or gear 
	public List<int> statBonuses { get; set; } 

	//Constructor
	public Stat(int baseValue, string statName, string statDescription){ 
		statBonuses = new List<int> ();
		BaseValue = baseValue;
		StatName = statName;
		StatDescription = statDescription;
	}

	//Allows for increase of base stat upon levelup, etc.
	public void statIncrease(int amount){ 
		BaseValue += amount; 
	} 

	//This allows us to add a new statbonus from some sort of item or move temporarily 
	public void addBonus(int statBonus){ 
		statBonuses.Add (statBonus); 
	} 

	//Allows for removal of temporary stat bonus 
	public void removeBonus(int targetBonus){ 
		statBonuses.Remove (targetBonus); 
	}

	//Calculates the total stat value including all modifiers
	public int calculateTotalValue(){ 
		int finalStat = 0;
		if(statBonuses.Count != 0){ 
			for(int i = 0; i < statBonuses.Count; i++){ 
				finalStat += statBonuses [i];
			} 
		}
		finalStat += BaseValue; 
		return finalStat;
	}
}
