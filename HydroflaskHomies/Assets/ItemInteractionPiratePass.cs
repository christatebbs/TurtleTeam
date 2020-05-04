using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInteractionPiratePass : MonoBehaviour {
	private Transform playerPosition;
	public Transform itemPosition;
	public Text dialogueText;
	public GameObject dialogueBox;
	public GameObject interactionPrompt;
    public bool chestOpen; 
	// Use this for initialization
	void Start ()
    {
		GameObject player = GameObject.Find("Player");
		playerPosition = player.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Vector3.Distance(playerPosition.position, itemPosition.position) < 2f)
		{
			Debug.Log("within range");
			interactionPrompt.SetActive(true);
		}
	
		if (Vector3.Distance(playerPosition.position, itemPosition.position) < 2f && Input.GetKeyDown("i"))
		{
			interactionPrompt.SetActive(false);
			dialogueBox.SetActive(true);
			//dialogueBox.SetActive(true);
			//dialogueText.text = "You found a pirate pass!";
			StartCoroutine(ReadPassDialogue());
			GameObject releventNPC = GameObject.Find("Capling_NPC");
			OverworldDialogueOne dialogueScript = releventNPC.GetComponent<OverworldDialogueOne>();
			dialogueScript.hasPass = true;
		}
	}

    IEnumerator ReadPassDialogue()
    {
		string dialogueString = "You picked up a pirate pass! Looks like Ollie might've dropped this.";
		string currentText = ""; 
		for (int j = 0; j <= dialogueString.Length; j++)
		{
			currentText = dialogueString.Substring(0, j);
			dialogueText.text = currentText;
			yield return new WaitForSeconds(0.05f);
		}
		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
		dialogueBox.SetActive(false);
		yield break;
	}
}
