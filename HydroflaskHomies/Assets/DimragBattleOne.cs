using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum DimragStateOne
{
	START, PLAYERTURN, FIRSTENEMYTURN, SECONDENEMYTURN, THIRDENEMYTURN, WON, LOST, PLAYERACTION, ENEMYSELECTHIT,
	ENEMYSELECTSKILL
}

public class DimragBattleOne : MonoBehaviour
{
	//this DimragStateOne element will help us keep track of where we are by setting state = the correct DimragStateOne
	public DimragStateOne state;

	private GameObject global;

	//These gameObjects will allow us to choose the correct enemy for each battle, and use an updated player prefab every fight
	public GameObject playerPrefab;

	//All three gameObjects represent the three enemies in battle 
	private GameObject enemyOne;
	private GameObject enemyTwo;
	private GameObject enemyThree;

	public GameObject skillPrefab;

	//used to hide and show dialoguebox in full
	public GameObject dialogueBox;
	private GameObject actions;

	//Keeps track of our player and enemy's stats (from the player and unit scripts respectively) 
	private Player playerUnit;

	private Enemy enemyUnitOne;
	private Enemy enemyUnitTwo;
	private Enemy enemyUnitThree;

	private String[] turnOrder;
	private int currentTurn;

	private int enemiesAlive;

	private PlayerLevel playerLevel;
	//PlayerStats playerStats; 

	//create the battle station locations so we know where to spawn in our graphics during battle setup 
	public Transform playerBattleStation;

	public Transform enemyBattleStation;
	public Transform enemyBattleStation2;
	public Transform enemyBattleStation3;

	public Transform skillContent;

	//Allows us to access the information in BattleHud for both the player and enemy. This includes hp, level, and name
	public BattleHud playerHUD;

	public BattleHud enemyHUD;
	public BattleHud enemyHUD2;
	public BattleHud enemyHUD3;

	public Text dialogueText;
	public Text skillText;

	//The skill list tree 
	public GameObject skills;

	private Attack thisAttack;

	private bool isGuarding;

	//Status effects 
	private bool isSick;
	private bool hasBubbleButt;
	//this is here because christa is dumb and lazy 
	int storeMobility;

	//When our game starts, we want to set the DimragStateOne to START, and begin the setup.
	void Start()
	{
		state = DimragStateOne.START;
		skills.SetActive(false);
		dialogueBox.SetActive(true);
		//In order to have delay between battle setup and player turn, we need it to be a coroutine
		StartCoroutine(SetupBattle());
	}

	IEnumerator SetupBattle()
	{
		//Instantiate both player and enemy into the battle 
		enemiesAlive = 3;
		global = GameObject.Find("GlobalObject");
		actions = GameObject.Find("Actions");
		actions.SetActive(false);

		GameObject player = Instantiate(playerPrefab, playerBattleStation);
		GlobalControl globalObject = global.GetComponent<GlobalControl>();

		GameObject enemyOne = (GameObject)Instantiate(Resources.Load("Capling"), enemyBattleStation);
		GameObject enemyTwo = (GameObject)Instantiate(Resources.Load("Capling"), enemyBattleStation2);
		GameObject enemyThree = (GameObject)Instantiate(Resources.Load("Capling"), enemyBattleStation3);

		//Get the information about the enemy and player 
		playerUnit = player.GetComponent<Player>();

		enemyUnitOne = enemyOne.GetComponent<Enemy>();
		enemyUnitTwo = enemyTwo.GetComponent<Enemy>();
		enemyUnitThree = enemyThree.GetComponent<Enemy>();

		playerLevel = player.GetComponent<PlayerLevel>();

		LoadPlayerData();

		playerHUD.SetPlayerHUD(playerUnit);

		storeMobility = playerLevel.stats[4];

		Debug.Log("Current player level: " + playerLevel.Level);
		Debug.Log("Player's HP when level is set: " + playerUnit.currentHP);
		playerUnit.attackList = new List<Attack>();

		setPlayerSkills();

		Debug.Log(playerUnit.attackList.Count);

		for (int i = 0; i < playerUnit.attackList.Count; i++)
		{
			GameObject newButton = Instantiate(skillPrefab, skillContent);
			Attack toAdd = newButton.GetComponent<Attack>();
			toAdd.SetValue(playerUnit.attackList[i].damage, playerUnit.attackList[i].type,
				playerUnit.attackList[i].name, playerUnit.attackList[i].isSkill, playerUnit.attackList[i].spCost);
			newButton.GetComponentInChildren<Text>().text = toAdd.name;
			newButton.SetActive(true);
			print(toAdd.type);
		}

		print(playerUnit.attackList.Count);

		//StartCoroutine(ReadDialogue("Three Caplings appear in front of you!"));
		dialogueText.text = "Three Caplings appear in front of you!";

		playerUnit.unitLevel = playerLevel.Level;
		//passes in the information about player and enemy unit to set their hud displays
		playerHUD.SetPlayerHUD(playerUnit);

		enemyHUD.SetEnemyHUD(enemyUnitOne);
		enemyHUD2.SetEnemyHUD(enemyUnitTwo);
		enemyHUD3.SetEnemyHUD(enemyUnitThree);

		//This waits a second to progress
		yield return new WaitForSeconds(2f);

		//Attack: 0
		//Defense: 1
		//Skill: 2
		//Technique: 3
		//Mobility: 4
		Debug.Log("Current Attack stat: " + playerLevel.stats[0]);
		Debug.Log("Current Defense stat: " + playerLevel.stats[1]);
		Debug.Log("Current Skill stat: " + playerLevel.stats[2]);
		Debug.Log("Current Technique stat: " + playerLevel.stats[3]);
		Debug.Log("Current Mobility stat: " + playerLevel.stats[4]);

		turnOrder = new string[4];
		currentTurn = -1;

		if (playerLevel.stats[4] >= enemyUnitOne.stats[4])
		{
			turnOrder[0] = "Player";
			turnOrder[1] = "EnemyOne";
			turnOrder[2] = "EnemyTwo";
			turnOrder[3] = "EnemyThree";
			Debug.Log("Player attacks first");
			state = DimragStateOne.PLAYERTURN;
			PlayerTurn();
		}
		else
		{
			turnOrder[0] = "EnemyOne";
			turnOrder[1] = "EnemyTwo";
			turnOrder[2] = "EnemyThree";
			turnOrder[3] = "Player";
			Debug.Log("Enemy attacks first");
			state = DimragStateOne.FIRSTENEMYTURN;
			StartCoroutine(FirstEnemyTurn());
		}
	}


	IEnumerator ReadDialogue(string dialogueString) {
		string currentText; 
		for (int j = 0; j <= dialogueString.Length; j++)
		{
			currentText = dialogueString.Substring(0, j);
			dialogueText.text = currentText;
			yield return new WaitForSeconds(.05f);
		}
		yield break;
	}

	public void PlayerTurn()
	{
		actions.SetActive(true);

		if (currentTurn == 4)
		{
			currentTurn = 0;
		}
		else
		{
			currentTurn++;
		}

		if (isGuarding)
		{
			isGuarding = false;
		}

		if (hasBubbleButt)
		{
			System.Random getRandom = new System.Random();
			int randomNum = getRandom.Next(0, 101);
			if (randomNum < 16)
			{
				StartCoroutine(CureBubbleButt());
				playerLevel.stats[4] = storeMobility;
				hasBubbleButt = false;
			}
		}

		if (isSick)
		{
			System.Random getRandom = new System.Random();
			int randomNum = getRandom.Next(0, 101);

			if (randomNum < 16)
			{
				StartCoroutine(CureSick());
				isSick = false;
			}
			else
			{
				StartCoroutine(sickDamage());
			}
		}

		skills.SetActive(false);
		dialogueBox.SetActive(true);
		//StartCoroutine(ReadDialogue("Choose an action:"));
		dialogueText.text = "Choose an action:";
	}


	IEnumerator FirstEnemyTurn() {

		dialogueText.text = "First Capling's turn!";
		//StartCoroutine(ReadDialogue("First Capling's turn!"));

		yield return new WaitForSeconds(2f);
		//while (!Input.GetMouseButtonDown(0))
		//{
		//	yield return null;
		//}

		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random();

		int randomNum = getRandom.Next(1, 5);
		bool causesBubblebutt = false;

		//IDLE
		if (randomNum == 1)
		{
			dialogueText.text = "Capling tries to hoist a flag that is no longer there.";
			//StartCoroutine(ReadDialogue("Capling tries to hoist a flag that is no longer there."));
			//while (!Input.GetMouseButtonDown(0))
			//{
			//	yield return null;
			//}
			yield return new WaitForSeconds(2f);
			state = DimragStateOne.PLAYERTURN;
			PlayerTurn();
			yield break;
			//ATTACK
		}
		else
		{
			if (randomNum == 2)
			{
				//Flavor text 
				dialogueText.text = "Capling jumps down your throat.";

				//StartCoroutine(ReadDialogue("Capling jumps down your throat"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}

				yield return new WaitForSeconds(2f);

				randomNum = getRandom.Next(1, 5);
				if (randomNum == 4)
				{
					Debug.Log("bubblebutt :P");
					causesBubblebutt = true;
				}
			}
			else
			{
				dialogueText.text = "Capling scratches you with its hook hand.";
				//StartCoroutine(ReadDialogue("Capling scratches  you with its hook hand"));

                //while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}

                yield return new WaitForSeconds(2f);
			}
			//Determine hit chance 
			System.Random hitRandom = new System.Random();
			randomNum = hitRandom.Next(0, 101);

			int hitChance = 95 - (playerLevel.stats[4] - enemyUnitOne.stats[4]);

			if (hitChance < 50)
			{
				hitChance = 50;
			}

			//Determine if attack hits  
			bool doesHit;
			if (randomNum < hitChance)
			{
				Debug.Log("Capling hits!");
				doesHit = true;
			}
			else
			{
				Debug.Log("Capling misses :(");
				doesHit = false;
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit)
			{
				yield return new WaitForSeconds(2f);
				dialogueText.text = "The attack missed!";
				//StartCoroutine(ReadDialogue("The attack missed!"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);

				state = DimragStateOne.PLAYERTURN;
				PlayerTurn();
				yield break;
			}

			//If attack does hit, calculate damage 
			//For Capling, damage changes depending on which of its two attacks it uses. If the randomNum generated is 2, it needs
			//to do skill damage and check for bubblebutt. 50% of the time is regular attack damage (randomNums 3 and 4) 

			int totalDamage = 1;

			if (randomNum == 2)
			{
				Debug.Log(" a throat jump");
				totalDamage = (enemyUnitOne.stats[2] * 2) - playerLevel.stats[3];
			}
			else
			{
				totalDamage = (enemyUnitOne.stats[0] * 2) - playerLevel.stats[1];
			}

			if (totalDamage < 1)
			{
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Capling: " + totalDamage);

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage(totalDamage);

			playerHUD.SetHP(playerUnit.currentHP);

			yield return new WaitForSeconds(2f);

			if (causesBubblebutt)
			{
				dialogueText.text = "You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0.";
				//StartCoroutine(ReadDialogue("You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0."));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				playerLevel.stats[4] = 0;
				hasBubbleButt = true;
				yield return new WaitForSeconds(2f);
			}

			if (isDead)
			{
				state = DimragStateOne.LOST;
				StartCoroutine(EndBattle());
				yield break;
			}

			else
			{
				if (!enemyUnitTwo.isKilled)
				{
					state = DimragStateOne.SECONDENEMYTURN;
					StartCoroutine(SecondEnemyTurn());
					yield break;
				}
				else if (!enemyUnitThree.isKilled)
				{
					state = DimragStateOne.THIRDENEMYTURN;
					StartCoroutine(ThirdEnemyTurn());
					yield break;
				}
				else
				{
					state = DimragStateOne.PLAYERTURN;
					PlayerTurn();
					yield break;
				}
			}
		}
	}

	IEnumerator SecondEnemyTurn()
	{

		dialogueText.text = "Second Capling's turn!";
		//StartCoroutine(ReadDialogue("Second Capling's turn!"));

		yield return new WaitForSeconds(2f);
		//while (!Input.GetMouseButtonDown(0))
		//{
		//	yield return null;
		//}

		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random();

		int randomNum = getRandom.Next(1, 5);
		bool causesBubblebutt = false;

		//IDLE
		if (randomNum == 1)
		{
			dialogueText.text = "Capling tries to hoist a flag that is no longer there.";
			//StartCoroutine(ReadDialogue("Capling tries to hoist a flag that is no longer there."));
			//	while (!Input.GetMouseButtonDown(0))
			//{
			//	yield return null;
			//}
			yield return new WaitForSeconds(2f);
			state = DimragStateOne.PLAYERTURN;
			PlayerTurn();
			yield break;
			//ATTACK
		}
		else
		{
			if (randomNum == 2)
			{
				//Flavor text 
				dialogueText.text = "Capling jumps down your throat.";
				//StartCoroutine(ReadDialogue("Capling jumps down your throat"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
				randomNum = getRandom.Next(1, 5);
				if (randomNum == 4)
				{
					Debug.Log("bubblebutt :P");
					causesBubblebutt = true;
				}
			}
			else
			{
				dialogueText.text = "Capling scratches you with its hook hand.";
				//StartCoroutine(ReadDialogue("Capling scratches  you with its hook hand"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
			}
			//Determine hit chance 
			System.Random hitRandom = new System.Random();
			randomNum = hitRandom.Next(0, 101);

			int hitChance = 95 - (playerLevel.stats[4] - enemyUnitTwo.stats[4]);

			if (hitChance < 50)
			{
				hitChance = 50;
			}

			//Determine if attack hits  
			bool doesHit;
			if (randomNum < hitChance)
			{
				Debug.Log("Capling hits!");
				doesHit = true;
			}
			else
			{
				Debug.Log("Capling misses :(");
				doesHit = false;
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit)
			{
				//yield return new WaitForSeconds(2f);
				dialogueText.text = "The attack missed!";
				//StartCoroutine(ReadDialogue("The attack missed!"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
				state = DimragStateOne.PLAYERTURN;
				PlayerTurn();
				yield break;
			}

			//If attack does hit, calculate damage 
			//For Capling, damage changes depending on which of its two attacks it uses. If the randomNum generated is 2, it needs
			//to do skill damage and check for bubblebutt. 50% of the time is regular attack damage (randomNums 3 and 4) 

			int totalDamage = 1;

			if (randomNum == 2)
			{
				Debug.Log(" a throat jump");
				totalDamage = (enemyUnitTwo.stats[2] * 2) - playerLevel.stats[3];
			}
			else
			{
				totalDamage = (enemyUnitTwo.stats[0] * 2) - playerLevel.stats[1];
			}

			if (totalDamage < 1)
			{
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Capling: " + totalDamage);

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage(totalDamage);

			playerHUD.SetHP(playerUnit.currentHP);

			//yield return new WaitForSeconds(2f);

			if (causesBubblebutt)
			{
				dialogueText.text = "You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0.";
				//StartCoroutine(ReadDialogue("You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0."));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				playerLevel.stats[4] = 0;
				hasBubbleButt = true;
				yield return new WaitForSeconds(2f);
			}

			if (isDead)
			{
				state = DimragStateOne.LOST;
				StartCoroutine(EndBattle());
				yield break;
			}

			else
			{
				if (!enemyUnitThree.isKilled)
				{
					state = DimragStateOne.SECONDENEMYTURN;
					StartCoroutine(ThirdEnemyTurn());
					yield break;
				}
				else if (!enemyUnitOne.isKilled)
				{
					state = DimragStateOne.THIRDENEMYTURN;
					StartCoroutine(FirstEnemyTurn());
					yield break;
				}
				else
				{
					state = DimragStateOne.PLAYERTURN;
					PlayerTurn();
					yield break;
				}
			}
		}
	}

	IEnumerator ThirdEnemyTurn()
	{
		dialogueText.text = "Third Capling's turn!";
		//StartCoroutine(ReadDialogue("Third Capling's turn!"));

		//while (!Input.GetMouseButtonDown(0))
		//{
		//	yield return null;
		//}
		yield return new WaitForSeconds(2f);
		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random();

		int randomNum = getRandom.Next(1, 5);
		bool causesBubblebutt = false;

		//IDLE
		if (randomNum == 1)
		{
			dialogueText.text = "Capling tries to hoist a flag that is no longer there.";
			//StartCoroutine(ReadDialogue("Capling tries to hoist a flag that is no longer there."));
			//while (!Input.GetMouseButtonDown(0))
			//{
			//	yield return null;
			//}
			yield return new WaitForSeconds(2f);
			state = DimragStateOne.PLAYERTURN;
			PlayerTurn();
			yield break;
			//ATTACK
		}
		else
		{
			if (randomNum == 2)
			{
				//Flavor text 
				dialogueText.text = "Capling jumps down your throat.";
				//StartCoroutine(ReadDialogue("Capling jumps down your throat"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
				randomNum = getRandom.Next(1, 5);
				if (randomNum == 4)
				{
					Debug.Log("bubblebutt :P");
					causesBubblebutt = true;
				}
			}
			else
			{
				dialogueText.text = "Capling scratches you with its hook hand.";
				//StartCoroutine(ReadDialogue("Capling scratches  you with its hook hand"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
			}
			//Determine hit chance 
			System.Random hitRandom = new System.Random();
			randomNum = hitRandom.Next(0, 101);

			int hitChance = 95 - (playerLevel.stats[4] - enemyUnitThree.stats[4]);

			if (hitChance < 50)
			{
				hitChance = 50;
			}

			//Determine if attack hits  
			bool doesHit;
			if (randomNum < hitChance)
			{
				Debug.Log("Capling hits!");
				doesHit = true;
			}
			else
			{
				Debug.Log("Capling misses :(");
				doesHit = false;
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit)
			{
				//yield return new WaitForSeconds(2f);
				dialogueText.text = "The attack missed!";
				//StartCoroutine(ReadDialogue("The attack missed!"));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
				state = DimragStateOne.PLAYERTURN;
				PlayerTurn();
				yield break;
			}

			//If attack does hit, calculate damage 
			//For Capling, damage changes depending on which of its two attacks it uses. If the randomNum generated is 2, it needs
			//to do skill damage and check for bubblebutt. 50% of the time is regular attack damage (randomNums 3 and 4) 

			int totalDamage = 1;

			if (randomNum == 2)
			{
				Debug.Log(" a throat jump");
				totalDamage = (enemyUnitThree.stats[2] * 2) - playerLevel.stats[3];
			}
			else
			{
				totalDamage = (enemyUnitThree.stats[0] * 2) - playerLevel.stats[1];
			}

			if (totalDamage < 1)
			{
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Capling: " + totalDamage);

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage(totalDamage);

			playerHUD.SetHP(playerUnit.currentHP);

			//yield return new WaitForSeconds(2f);

			if (causesBubblebutt)
			{
				dialogueText.text = "You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0.";
				//StartCoroutine(ReadDialogue("You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0."));
				//while (!Input.GetMouseButtonDown(0))
				//{
				//	yield return null;
				//}
				yield return new WaitForSeconds(2f);
				playerLevel.stats[4] = 0;
				hasBubbleButt = true;
				//yield return new WaitForSeconds(2f);
			}

			if (isDead)
			{
				state = DimragStateOne.LOST;
				StartCoroutine(EndBattle());
				yield break;
			}

			else
			{
				state = DimragStateOne.PLAYERTURN;
				PlayerTurn();
				yield break;
			}
		}
	}

	IEnumerator EndBattle()
	{
		if (state == DimragStateOne.WON)
		{

			int expYield = enemyUnitOne.experience + enemyUnitTwo.experience + enemyUnitThree.experience;
			int ptYield = enemyUnitOne.pt + enemyUnitTwo.pt + enemyUnitThree.pt;

			dialogueText.text = "The enemy dropped " + ptYield + " PT!";
			playerUnit.pt += ptYield;

			yield return new WaitForSeconds(2f);

			dialogueText.text = "You gained " + expYield + " experience!";

			yield return new WaitForSeconds(2f);

			Debug.Log(playerLevel.currentExperience);

			bool didLevelUp = playerLevel.GrantExperience(expYield);

			Debug.Log(playerLevel.currentExperience);

			if (didLevelUp)
			{
				dialogueText.text = "You leveled up!";
				playerUnit.unitLevel = playerLevel.Level;
				//passes in the information about player and enemy unit to set their hud displays
				playerHUD.SetPlayerHUD(playerUnit);
			}

			//Attack: 0
			//Defense: 1
			//Skill: 2
			//Technique: 3
			//Mobility: 4
			Debug.Log("Current Attack stat: " + playerLevel.stats[0]);
			Debug.Log("Current Defense stat: " + playerLevel.stats[1]);
			Debug.Log("Current Skill stat: " + playerLevel.stats[2]);
			Debug.Log("Current Technique stat: " + playerLevel.stats[3]);
			Debug.Log("Current Mobility stat: " + playerLevel.stats[4]);

			yield return new WaitForSeconds(2f);

			dialogueText.text = "You won the battle!";

			//	PrefabUtility.RecordPrefabInstancePropertyModifications(playerPrefab);
		}
		else if (state == DimragStateOne.LOST)
		{
			dialogueText.text = "You were defeated.";
		}

		yield return new WaitForSeconds(2f);
		SavePlayerData();
		SceneManager.LoadScene("Overworld");
	}

	public void OnAttackButton()
	{
		actions.SetActive(false);
		//We want to make sure that if the player presses the attack button and it is not their turn, nothing happens
		if (state != DimragStateOne.PLAYERTURN)
		{
			return;
		}
		state = DimragStateOne.ENEMYSELECTSKILL;
		skills.SetActive(true);
	}

	public void OnHitButton()
	{
		if (state == DimragStateOne.PLAYERTURN)
		{
			//StartCoroutine (Hit ());
			dialogueText.text = "Please choose (click) the enemy you would like to attack.";
			state = DimragStateOne.ENEMYSELECTHIT;
		}
	}

	public void OnEnemySelected(Button button)
	{
		Debug.Log("enemy select");
		Debug.Log(button.name);

		String enemyToHit = button.name;

		Enemy enemyUnit = enemyUnitOne;

		if (enemyToHit == "EnemyOne")
		{
			enemyUnit = enemyUnitOne;
		}
		else if (enemyToHit == "EnemyTwo")
		{
			enemyUnit = enemyUnitTwo;
		}
		else if (enemyToHit == "EnemyThree")
		{
			Debug.Log("enemy three");
			enemyUnit = enemyUnitThree;
		}
		else
		{
			Debug.Log("Something is wrong");
		}

		if (enemyUnit.isKilled)
		{
			Debug.Log("selected dead enemy");
			dialogueText.text = "This enemy has already been defeated! Please select a different target.";
		}
		else
		{
			if (state == DimragStateOne.ENEMYSELECTHIT && !enemyUnit.isKilled)
			{
				StartCoroutine(Hit(enemyToHit));
			}
			else if (state == DimragStateOne.ENEMYSELECTSKILL)
			{
				StartCoroutine(PlayerAttack(thisAttack, enemyToHit));
			}
		}
	}

	IEnumerator Hit(String enemyToHit)
	{
		state = DimragStateOne.PLAYERACTION;
		bool doesHit;

		Enemy enemyUnit = enemyUnitOne;

		if (enemyToHit == "EnemyOne")
		{
			enemyUnit = enemyUnitOne;
		}
		else if (enemyToHit == "EnemyTwo")
		{
			enemyUnit = enemyUnitTwo;
		}
		else if (enemyToHit == "EnemyThree")
		{
			Debug.Log("enemy three");
			enemyUnit = enemyUnitThree;
		}
		else
		{
			Debug.Log("Something went wrong in enemy select");
			yield break;
		}

		//need to check for hit before we can determine damage 
		System.Random getRandom = new System.Random();
		int randomNum = getRandom.Next(0, 101);

		dialogueText.text = "You attack the enemy!";

		int hitChance = 95 - (enemyUnit.stats[4] - playerLevel.stats[4]);

		if (hitChance < 50)
		{
			hitChance = 50;
		}

		if (randomNum < hitChance)
		{
			Debug.Log("Player hits!");
			doesHit = true;
		}
		else
		{
			Debug.Log("Player misses :(");
			doesHit = false;
		}

		//If the player's attack misses, all we need to do is let them know and then proceed with the enemy's turn
		if (!doesHit)
		{
			yield return new WaitForSeconds(2f);
			dialogueText.text = "The attack missed!";
			state = DimragStateOne.FIRSTENEMYTURN;
			StartCoroutine(FirstEnemyTurn());
		}

		//On a hit, we need to start with calculating base damage for the attack
		//Uses this formula: (Your Attack * 2) - (Opponent Defense) to a minimum of 1.
		int totalDamage = (playerLevel.stats[0] * 2) - enemyUnit.stats[1];

		if (totalDamage < 1)
		{
			totalDamage = 1;
		}

		bool isDead = enemyUnit.TakeDamage(totalDamage);

		if (enemyToHit == "EnemyOne")
		{
			enemyHUD.SetHP(enemyUnit.currentHP);
		}
		else if (enemyToHit == "EnemyTwo")
		{
			enemyHUD2.SetHP(enemyUnit.currentHP); ;
		}
		else if (enemyToHit == "EnemyThree")
		{
			enemyHUD3.SetHP(enemyUnit.currentHP);
		}
		else
		{
			Debug.Log("Something went wrong in enemy select");
			yield break;
		}

		if (isDead)
		{
			enemyUnit.isKilled = true;
			enemiesAlive -= 1;
			dialogueText.text = "You defeated " + enemyUnit.unitName + "!";
			yield return new WaitForSeconds(2f);

			//System.Random randomDrop = new System.Random();
			//int dropChance = randomDrop.Next(0, 101);
			//Debug.Log("drop number generated: " + dropChance);
			//if (dropChance <= enemyUnit.dropChance)
			//{
			//	dialogueText.text = "The enemy dropped a " + enemyUnit.itemName + "!";
			//	playerUnit.addItemFromDrop(enemyUnit.GetComponent<Item>());
			//	yield return new WaitForSeconds(2f);
			//}

			System.Random thisRandom = new System.Random();
			int itemsDropped = thisRandom.Next(1, 4);
			Debug.Log("number of items dropped: " + itemsDropped);
			for (int i = 0; i <= itemsDropped; i++)
			{
				playerUnit.addItemFromDrop(enemyUnit.GetComponent<Item>());
			}
			if (itemsDropped == 1)
			{
				dialogueText.text = "The enemy dropped a " + "seaweed scrap!";
			}
			else
			{
				dialogueText.text = "The enemy dropped " + itemsDropped + "seaweed scraps!";
			}
            yield return new WaitForSeconds(2f);
		}

		//check if the enemy is dead 
		//change state based on result 
		if (enemiesAlive == 0)
		{
			//End the battle
			state = DimragStateOne.WON;
			StartCoroutine(EndBattle());

		}
		else
		{
			//Enemy Turn
			if (!enemyUnitOne.isKilled)
			{
				state = DimragStateOne.FIRSTENEMYTURN;
				StartCoroutine(FirstEnemyTurn());
				yield break;
			}
			else if (!enemyUnitTwo.isKilled)
			{
				state = DimragStateOne.SECONDENEMYTURN;
				StartCoroutine(SecondEnemyTurn());
				yield break;
			}
			else
			{
				state = DimragStateOne.THIRDENEMYTURN;
				StartCoroutine(ThirdEnemyTurn());
				yield break;
			}
		}
	}

	public void OnGuardButton()
	{
		//We want to make sure that if the player presses the attack button and it is not their turn, nothing happens
		if (state != DimragStateOne.PLAYERTURN)
		{
			return;
		}
		state = DimragStateOne.PLAYERACTION;

		StartCoroutine(PlayerGuard());
	}

	IEnumerator PlayerGuard()
	{
		//playerUnit.HealDamage (5); 
		//playerHUD.SetHP (playerUnit.currentHP); 

		dialogueText.text = "You prepare for the next attack!";
		isGuarding = true;

		yield return new WaitForSeconds(2f);

		state = DimragStateOne.FIRSTENEMYTURN;
		StartCoroutine(FirstEnemyTurn());
	}

	//This is attached to the skill button, so we are aware which button was pressed
	//Different from onAttackButton, because OnAttackButton only determines if player wishes to attack, not 
	//what skill they choose.
	public void OnSkillSelected(Button button)
	{
		thisAttack = button.GetComponent<Attack>();
		Debug.Log(thisAttack.name);
		if (thisAttack.spCost > playerUnit.currentSP)
		{
			dialogueText.text = "Too little SP to use this skill, please try again";
		}
		else
		{
			playerUnit.currentSP -= thisAttack.spCost;
			state = DimragStateOne.ENEMYSELECTSKILL;
			dialogueText.text = "Please select (click) an enemy to attack.";
		}
	}

	public void OnBackSelected()
	{
		actions.SetActive(true);
		state = DimragStateOne.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack(Attack attack, String enemyToHit)
	{
		//damage the enemy 
		bool isDead;
		//if the character uses a 'skill' attack vs an 'attack' attack, we need to remove SP so it's important to have a variable let 
		//us know.
		//After the character selects a skill, we stop displaying them and instead display dialogue 
		skills.SetActive(false);
		dialogueBox.SetActive(true);

		Enemy enemyUnit = enemyUnitOne;
		if (enemyToHit == "EnemyTwo")
		{
			enemyUnit = enemyUnitTwo;
		}
		else if (enemyToHit == "EnemyThree")
		{
			enemyUnit = enemyUnitThree;
		}

		if (enemyToHit == "EnemyOne")
		{
			enemyUnit = enemyUnitOne;
		}
		else if (enemyToHit == "EnemyTwo")
		{
			enemyUnit = enemyUnitTwo;
		}
		else if (enemyToHit == "EnemyThree")
		{
			enemyUnit = enemyUnitThree;
		}
		else
		{
			Debug.Log("Something went wrong in enemy select");
			yield break;
		}

		//Hit chance: 95% - (Opponent Mobility - Your Mobility) to a minimum of 50%
		bool doesHit;

		//need to check for hit before we can determine damage 
		System.Random getRandom = new System.Random();
		int randomNum = getRandom.Next(0, 101);

		dialogueText.text = "You attack the enemy!";

		int hitChance = 95 - (enemyUnitOne.stats[4] - playerLevel.stats[4]);

		if (hitChance < 50)
		{
			hitChance = 50;
		}

		if (randomNum < hitChance)
		{
			Debug.Log("Player hits!");
			doesHit = true;
		}
		else
		{
			Debug.Log("Player misses :(");
			doesHit = false;
		}

		//If the player's attack misses, all we need to do is let them know and then proceed with the enemy's turn
		if (!doesHit)
		{
			yield return new WaitForSeconds(2f);
			dialogueText.text = "The attack missed!";
			yield return new WaitForSeconds(2f);
			state = DimragStateOne.FIRSTENEMYTURN;
			StartCoroutine(FirstEnemyTurn());
		}

		int totalDamage = 1;

		//On a hit, we need to start with calculating base damage for the attack
		if (doesHit)
		{
			//Uses this formula: (Your Attack * 2) - (Opponent Defense) to a minimum of 1.
			totalDamage = (playerLevel.stats[2] * 2) - enemyUnit.stats[3];

			if (totalDamage < 1)
			{
				totalDamage = 1;
			}

			Debug.Log("Total Damage " + totalDamage);

			if (enemyUnit.type == "trash")
			{
				if (attack.type == "trash")
				{
					isDead = enemyUnit.TakeDamage(totalDamage * 2 * thisAttack.damage);
					dialogueText.text = "You deal extra damage with the correct waste disposal method!";
				}
				else
				{
					isDead = enemyUnit.TakeDamage(totalDamage * thisAttack.damage);
					dialogueText.text = "There may be a more proper way to handle this enemy";
				}
			}
			else if (enemyUnit.type == "recycle")
			{
				print(attack.type);
				if (attack.type == "recycle")
				{
					isDead = enemyUnit.TakeDamage(totalDamage * 2 * thisAttack.damage);
					dialogueText.text = "You deal extra damage with the correct waste disposal method!";
				}
				else
				{
					isDead = enemyUnit.TakeDamage(totalDamage * thisAttack.damage);
					dialogueText.text = "There may be a more proper way to handle this enemy";
				}
			}
			else
			{
				if (thisAttack.type == "compost")
				{
					isDead = enemyUnit.TakeDamage(totalDamage * 2 * thisAttack.damage);
					dialogueText.text = "You deal extra damage with the correct waste disposal method!";
				}
				else
				{
					isDead = enemyUnit.TakeDamage(totalDamage * thisAttack.damage);
					dialogueText.text = "There may be a more proper way to handle this enemy";
				}
			}

			//We need to reset enemy HP after they take damage
			if (enemyToHit == "EnemyOne")
			{
				enemyHUD.SetHP(enemyUnitOne.currentHP);
			}
			else if (enemyToHit == "EnemyTwo")
			{
				enemyHUD2.SetHP(enemyUnitTwo.currentHP);
			}
			else
			{
				enemyHUD3.SetHP(enemyUnitThree.currentHP);
			}

			yield return new WaitForSeconds(2f);

			//check if the enemy is dead 
			//change state based on result 
			if (isDead)
			{
				enemyUnit.isKilled = true;
				enemiesAlive -= 1;
				dialogueText.text = "You defeated " + enemyUnit.name + "!";
				yield return new WaitForSeconds(2f);

				//System.Random randomDrop = new System.Random();
				//int dropChance = randomDrop.Next(0, 101);
				//Debug.Log("drop number generated: " + dropChance);
				//if (dropChance <= enemyUnit.dropChance)
				//{
				//	dialogueText.text = "The enemy dropped a " + enemyUnit.itemName + "!";
				//	playerUnit.addItemFromDrop(enemyUnit.GetComponent<Item>());
				//	yield return new WaitForSeconds(2f);
				//}

				System.Random thisRandom = new System.Random();
				int itemsDropped = thisRandom.Next(1, 4);
				Debug.Log("number of items dropped: " + itemsDropped);
				for (int i = 0; i <= itemsDropped; i++)
				{
					playerUnit.addItemFromDrop(enemyUnit.GetComponent<Item>());
				}
				if (itemsDropped == 1)
				{
					dialogueText.text = "The enemy dropped a " + "seaweed scrap!";
				}
				else
				{
					dialogueText.text = "The enemy dropped " + itemsDropped + "seaweed scraps!";
				}
			}

			if (enemiesAlive == 0)
			{
				//End the battle
				state = DimragStateOne.WON;
				StartCoroutine(EndBattle());

			}
			else
			{
				//Enemy Turn
				if (!enemyUnitOne.isKilled)
				{
					state = DimragStateOne.FIRSTENEMYTURN;
					StartCoroutine(FirstEnemyTurn());
				}
				else if (!enemyUnitTwo.isKilled)
				{
					state = DimragStateOne.SECONDENEMYTURN;
					StartCoroutine(SecondEnemyTurn());
				}
				else
				{
					state = DimragStateOne.THIRDENEMYTURN;
					StartCoroutine(ThirdEnemyTurn());
				}
			}
		}
	}

	IEnumerator sickDamage()
	{
		bool isDead = playerUnit.TakeDamage(playerUnit.maxHP / 20);

		playerHUD.SetHP(playerUnit.currentHP);

		dialogueText.text = "You take some damage from sickness!";

		yield return new WaitForSeconds(2f);

		if (isDead)
		{
			state = DimragStateOne.LOST;
			StartCoroutine(EndBattle());
			yield break;
		}
		else
		{
			state = DimragStateOne.PLAYERTURN;
			PlayerTurn();
			yield break;
		}
	}

	IEnumerator CureBubbleButt()
	{
		dialogueText.text = "Your bubblebutt has been cured!";
		yield return new WaitForSeconds(2f);
		PlayerTurn();
		yield break;
	}

	IEnumerator CureSick()
	{
		dialogueText.text = "Your sickness has been cured!";
		yield return new WaitForSeconds(2f);
		PlayerTurn();
		yield break;
	}

	public void setPlayerSkills()
	{
		Attack trash = gameObject.AddComponent<Attack>();
		trash.damage = 1;
		trash.type = "trash";
		trash.name = "Dump";
		trash.spCost = 3;
		trash.isSkill = true;

		Attack recycle = gameObject.AddComponent<Attack>();
		recycle.damage = 1;
		recycle.type = "recycle";
		recycle.name = "Shred";
		recycle.spCost = 3;
		recycle.isSkill = true;

		Attack compost = gameObject.AddComponent<Attack>();
		compost.damage = 1;
		compost.type = "compost";
		compost.name = "Worm Strike";
		compost.spCost = 3;
		compost.isSkill = true;

		playerUnit.addAttack(trash);
		playerUnit.addAttack(recycle);
		playerUnit.addAttack(compost);
	}

	public void SavePlayerData()
	{
		playerLevel.SaveLevelInfo();
		playerUnit.savePlayerData();
	}

	public void LoadPlayerData()
	{
		playerLevel.LoadLevelInfo();
		playerUnit.loadPlayerData();
	}
}

