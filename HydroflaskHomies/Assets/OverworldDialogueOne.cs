using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class OverworldDialogueOne : MonoBehaviour {
	private Transform playerPosition;
	public Transform npcPosition;
	public Text dialogueText;
	public GameObject dialogueBox;
	public GameObject interactionPrompt;
	public bool hasPass;
	public int interactionStage; 

	private string[] dialogueOptions = new string[9];
	private float delay = 0.05f;
	private string currentText = ""; 

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.Find("Player");
		playerPosition = player.transform;
		hasPass = false; 
		InitializeDialogue();
	}
	
	// Update is called once per frame
	void Update () {
        if(Vector3.Distance(playerPosition.position, npcPosition.position) < 2f){
			interactionPrompt.SetActive(true); 
        } 
		if (Vector3.Distance(playerPosition.position, npcPosition.position) < 2f && Input.GetKeyDown("i"))
		{
			interactionPrompt.SetActive(false);
			dialogueBox.SetActive(true);
			Debug.Log("????");
			StartCoroutine(ReadStartDialogue());
		}
	}

    IEnumerator ReadStartDialogue()
	{
		string dialogueString = "";
		for (int i = 0; i < 5; i++)
		{
			dialogueString = dialogueOptions[i];
			for (int j = 0; j <= dialogueString.Length; j++)
			{
				currentText = dialogueString.Substring(0,j);
				dialogueText.text = currentText; 
				yield return new WaitForSeconds(delay);
			}
			//yield return new WaitForSeconds(1f);
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}
		}
        if (hasPass)
        {
			StartCoroutine(ReadPassDialogue());
			yield break;
        }
        else if(!hasPass)
        {
			StartCoroutine(ReadNoPassDialogue());
			yield break; 
        }
	}


    IEnumerator ReadNoPassDialogue()
    {
		interactionStage = 3;
        for (int i = 0; i <= dialogueOptions[5].Length; i++)
        {
			currentText = dialogueOptions[5].Substring(0, i);
			dialogueText.text = currentText;
			yield return new WaitForSeconds(delay); 
        }

		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}

		dialogueBox.SetActive(false);
		yield break;
	}

    IEnumerator ReadPassDialogue()
    {
		interactionStage = 2;
		string dialogueString = ""; 
        for(int i = 6; i <=7; i++)
        {
			dialogueString = dialogueOptions[i];
            for(int j = 0; j <= dialogueString.Length; j++)
            {
				currentText = dialogueString.Substring(0, j);
				dialogueText.text = currentText;
				yield return new WaitForSeconds(delay);
            }
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}
		}
		dialogueBox.SetActive(false);
		yield break;
	}

    //IEnumerator ReadFollowUpDialogue()
    //{

    //}

    IEnumerator WaitForClick()
    {
		while (!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
		Debug.Log("lmb clicked");
	}

	public void InitializeDialogue()
	{
        //1st interaction tree 
        dialogueOptions[0] = "And who be ye? Another tourist?";
		dialogueOptions[1] = "LISTEN! Dimrag’s a town for serious pirates only.";
		dialogueOptions[2] = "Ya want ta’ get inside, ya need a pass.";
		dialogueOptions[3] = "Only the roughest, toughest sailors on the sea get ‘em.";
        dialogueOptions[4] = "Or the ones who are part of our crew.";
            //with no pass 
            dialogueOptions[5] = "So scram!";
		    //with pass
		    dialogueOptions[6] = "Oh, ye’ve got a pass?";
		    dialogueOptions[7] = "Well the passes don't lie. It's your funeral.";

        //Repeated interaction
		dialogueOptions[8] = "I told ya ye need a pass to get in!";
        //if no pass - repeat dialogue 5
        //if yes pass - start dialogue 6 & 7 
	}
}
