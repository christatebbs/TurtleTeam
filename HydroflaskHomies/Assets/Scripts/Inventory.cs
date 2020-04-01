using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Inventory : MonoBehaviour {
	private GameObject healthyDescription;
	private GlobalControl global; 
	private GameObject openObject; 
	private string objectName; 
	private Item itemInfo;
	private GameObject inventory; 
	private Text maxHPText; 
	private Text currentHPText; 
	bool inventoryShown; 

	// Use this for initialization
	void Start () {
		inventoryShown = false; 
		currentHPText = GameObject.Find ("CurrentHP").GetComponent<Text> (); 
		maxHPText = GameObject.Find ("MaxHP").GetComponent<Text> (); 
		global = GameObject.Find ("GlobalObject").GetComponent<GlobalControl>(); 

		currentHPText.text = "" + global.currentHP; 
		maxHPText.text = "" + global.maxHP;

		healthyDescription = GameObject.Find ("HealthyDescription");
		inventory = GameObject.Find ("Inventory"); 
		inventory.SetActive (false);
		healthyDescription.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("e"))
		{
			print("e key was pressed");
			if (inventoryShown) { 
				inventory.SetActive (false); 
				inventoryShown = false; 
			} else { 
				inventory.SetActive (true); 
				inventoryShown = true; 
			}
		}
	}

	public void onClickItems(){ 
		inventory.SetActive (true); 
	}

	public void onCloseItems(){ 
		inventory.SetActive (false); 
	}  

	public void OnClickObject(Button item){ 
		Debug.Log ("Hiii"); 
		objectName = item.name; 
		itemInfo = item.GetComponent<Item> (); 
		openObject = healthyDescription;
		openObject.SetActive (true); 
	} 

	public void OnClickExit(){ 
		Debug.Log ("???");
		openObject.SetActive (false); 
	} 

	public void OnObjectdUse(){ 
		if (itemInfo.effect == "Heal") { 
			if (global.currentHP == global.maxHP) { 
				
			} else { 
				global.currentHP += itemInfo.healing; 
				if (global.currentHP >= global.maxHP) { 
					global.currentHP = global.maxHP; 
				} 
				currentHPText.text = "" + global.currentHP;
				GameObject toRemove = GameObject.Find (objectName); 
				toRemove.SetActive (false);
			}
		} 
		openObject.SetActive (false); 

	} 

	public void ShowInventory(){
		inventory.SetActive (true); 
	}

	public void HideInventory(){ 
		inventory.SetActive (false); 
	} 
}
