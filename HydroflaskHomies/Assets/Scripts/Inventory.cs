using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Inventory : MonoBehaviour {
	private GameObject itemPopup;
	private Text itemDescription; 
	private Text itemTitle; 
	private Image itemImage; 
	private GlobalControl global; 
	private GameObject openObject; 
	public GameObject itemButton; 
	public Transform inventoryContent; 

	private string objectName; 
	private Item itemInfo;
	private GameObject inventory; 
	private Text maxHPText; 
	private Text currentHPText; 
	bool inventoryShown; 
	private List<Item> inventoryList; 

	// Use this for initialization
	void Start () {
		inventoryShown = false; 
		currentHPText = GameObject.Find ("CurrentHP").GetComponent<Text> (); 
		maxHPText = GameObject.Find ("MaxHP").GetComponent<Text> (); 
		global = GameObject.Find ("GlobalObject").GetComponent<GlobalControl>(); 

		currentHPText.text = "" + global.currentHP; 
		maxHPText.text = "" + global.maxHP;

		inventoryList = global.inventoryList; 

		itemPopup = GameObject.Find ("ItemPopup");
		GameObject desc = GameObject.Find ("ItemDescription"); 
		itemDescription = desc.GetComponent<UnityEngine.UI.Text> ();
		GameObject itemImageHolder = GameObject.Find ("ItemImage"); 
		GameObject title = GameObject.Find ("ItemTitle"); 
		itemTitle = title.GetComponent<UnityEngine.UI.Text> ();
		itemImage = itemImageHolder.GetComponent<Image> (); 
		inventory = GameObject.Find ("Inventory"); 
		inventory.SetActive (false);
		itemButton.SetActive (false); 
		itemPopup.SetActive (false);

		setItems ();
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

	public void setItems(){ 
		for (int i = 0; i < inventoryList.Count; i++) {
//			GameObject newButton = Instantiate (itemButton, inventoryContent);
//			Item toAdd = newButton.GetComponent<Item>();
//			toAdd.SetValue (inventoryList[i].name, inventoryList[i].description,
//				inventoryList[i].effect, inventoryList[i].healing);
//			newButton.SetActive (true);
//			GameObject childImage = newButton.transform.GetChild (0).gameObject; 
//			Image imageComponent = childImage.GetComponent<Image> (); 
//			Debug.Log (toAdd.name);
//			Sprite itemSprite = Resources.Load<Sprite>(toAdd.name); 
//			imageComponent.sprite = itemSprite; 
//			newButton.name = toAdd.name; 

			GameObject newButton = Instantiate (itemButton, inventoryContent);
			newButton.AddComponent<Item> (); 
			Item thisItem = newButton.GetComponent<Item> (); 

			thisItem.SetValue (inventoryList[i].name, inventoryList[i].description,
			inventoryList[i].effect, inventoryList[i].healing);
			thisItem = inventoryList [i]; 
			GameObject childImage = newButton.transform.GetChild (0).gameObject; 
			Image imageComponent = childImage.GetComponent<Image> (); 
			Sprite itemSprite = Resources.Load<Sprite> (thisItem.name); 
			imageComponent.sprite = itemSprite; 
			newButton.name = inventoryList [i].name; 
			newButton.SetActive (true); 
			
		}
	} 

	public void addItem(Item newItem){ 
		
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
		itemTitle.text = itemInfo.name;
		itemDescription.text = itemInfo.description; 
		itemImage.sprite = Resources.Load<Sprite> (itemInfo.name); 
		itemPopup.SetActive (true); 
	} 

	public void OnClickExit(){ 
		Debug.Log ("???");
		openObject.SetActive (false); 
	} 

	public void OnObjectUse(){ 
		if (itemInfo.effect == "Heal") { 
			if (global.currentHP == global.maxHP) { 
				Debug.Log ("user is at full health, need to find a way to display this"); 
			} else { 
				global.currentHP += itemInfo.healing; 
				if (global.currentHP >= global.maxHP) { 
					global.currentHP = global.maxHP; 
				} 
				currentHPText.text = "" + global.currentHP;
				GameObject toRemove = GameObject.Find (objectName); 
				toRemove.SetActive (false);
				global.removeFromInventory (itemInfo); 
			}
		} 
		itemPopup.SetActive (false); 
	} 

	public void ShowInventory(){
		inventory.SetActive (true); 
	}

	public void HideInventory(){ 
		inventory.SetActive (false); 
	} 
}
