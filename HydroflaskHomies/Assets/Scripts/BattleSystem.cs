using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//these states will allow us to easily determine at what part of the battle we are at
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, PLAYERACTION } 

public class BattleSystem : MonoBehaviour {

	//this battlestate element will help us keep track of where we are by setting state = the correct BattleState
	public BattleState state;

	private GameObject global; 

	//These gameObjects will allow us to choose the correct enemy for each battle, and use an updated player prefab every fight
	public GameObject playerPrefab; 
	private GameObject enemyPrefab; 
	public GameObject skillPrefab;

	//used to hide and show dialoguebox in full
	public GameObject dialogueBox; 

	//Keeps track of our player and enemy's stats (from the player and unit scripts respectively) 
	Player playerUnit; 
	Enemy enemyUnit; 
	PlayerLevel playerLevel;
	//PlayerStats playerStats; 

	//create the battle station locations so we know where to spawn in our graphics during battle setup 
	public Transform playerBattleStation;
	public Transform enemyBattleStation; 

	public Transform skillContent;

	//Allows us to access the information in BattleHud for both the player and enemy. This includes hp, level, and name
	public BattleHud playerHUD; 
	public BattleHud enemyHUD; 

	public Text dialogueText;
	public Text skillText;

	//The skill list tree 
	public GameObject skills;

	private bool isGuarding; 

	//Specifically for glass cannon 
	private bool isLoading; 

	//Status effects 
	private bool isSick;
	private bool hasBubbleButt;
	//this is here because christa is dumb and lazy 
	int storeMobility; 

	//When our game starts, we want to set the BattleState to START, and begin the setup.
	void Start () {
		state = BattleState.START;
		skills.SetActive (false);
		dialogueBox.SetActive (true);
		//In order to have delay between battle setup and player turn, we need it to be a coroutine
		StartCoroutine(SetupBattle ());
	}

	IEnumerator SetupBattle(){
		//Instantiate both player and enemy into the battle 
		global = GameObject.Find("GlobalObject"); 
		isLoading = false; 

		GameObject player = Instantiate (playerPrefab, playerBattleStation);
		GlobalControl globalObject = global.GetComponent<GlobalControl> (); 
		GameObject enemy = (GameObject)Instantiate (Resources.Load(globalObject.nextFight), enemyBattleStation);

		//Get the information about the enemy and player 
		playerUnit = player.GetComponent<Player> (); 
		enemyUnit = enemy.GetComponent<Enemy> (); 

		playerLevel = player.GetComponent<PlayerLevel> (); 

		LoadPlayerData ();
		//playerStats = player.GetComponent<PlayerStats> ();
		playerHUD.SetPlayerHUD (playerUnit); 

		storeMobility = playerLevel.stats [4];

		Debug.Log ("Current player level: " + playerLevel.Level);
		Debug.Log ("Player's HP when level is set: " + playerUnit.currentHP);
		playerUnit.attackList = new List<Attack> ();

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
		playerUnit.addAttack (recycle);
		playerUnit.addAttack (compost);

		Debug.Log (playerUnit.attackList.Count);

		for (int i = 0; i < playerUnit.attackList.Count; i++) {
			GameObject newButton = Instantiate (skillPrefab, skillContent);
			Attack thisAttack = newButton.GetComponent<Attack>();
			thisAttack.SetValue (playerUnit.attackList [i].damage, playerUnit.attackList [i].type,
				playerUnit.attackList [i].name, playerUnit.attackList [i].isSkill, playerUnit.attackList[i].spCost);
			newButton.GetComponentInChildren<Text> ().text = thisAttack.name;
			newButton.SetActive (true);
			print (thisAttack.type);
		}
			
		print (playerUnit.attackList.Count);

		dialogueText.text = enemyUnit.unitName + " is here to fight!"; 

		playerUnit.unitLevel = playerLevel.Level;
		//passes in the information about player and enemy unit to set their hud displays
		playerHUD.SetPlayerHUD (playerUnit);
		enemyHUD.SetEnemyHUD (enemyUnit); 

		//This waits a small amount of time before giving the dialogue to the player that they should choose an action
		yield return new WaitForSeconds (2f); 

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


		//Now the state is set to player's turn, and we move on to the PlayerTurn function
		if (playerLevel.stats [4] >= enemyUnit.stats [4]) {
			Debug.Log ("Player attacks first"); 
			state = BattleState.PLAYERTURN;
			PlayerTurn ();
		} else { 
			state = BattleState.ENEMYTURN;
			StartCoroutine (EnemyTurn ());
			Debug.Log ("Enemy attacks first"); 
		}
	}

	IEnumerator PlayerAttack(Button button){
		//damage the enemy 
		bool isDead; 
		Attack thisAttack = button.GetComponent<Attack>();
		//if the character uses a 'skill' attack vs an 'attack' attack, we need to remove SP so it's important to have a variable let 
		//us know.
		//After the character selects a skill, we stop displaying them and instead display dialogue 
		skills.SetActive (false);
		dialogueBox.SetActive (true);

		//Hit chance: 95% - (Opponent Mobility - Your Mobility) to a minimum of 50%
		bool doesHit;

		//need to check for hit before we can determine damage 
		System.Random getRandom = new System.Random(); 
		int randomNum = getRandom.Next (0, 101); 

		dialogueText.text = "You attack the enemy!"; 

		int hitChance = 95 - (enemyUnit.stats [4] - playerLevel.stats [4]); 

		if (hitChance < 50) { 
			hitChance = 50;
		}

		if (randomNum < hitChance) { 
			Debug.Log ("Player hits!"); 
			doesHit = true; 
		} else { 
			Debug.Log ("Player misses :("); 
			doesHit = false; 
		}

		//If the player's attack misses, all we need to do is let them know and then proceed with the enemy's turn
		if (!doesHit) {
			yield return new WaitForSeconds(2f); 
			dialogueText.text = "The attack missed!"; 
			yield return new WaitForSeconds(2f); 
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn()); 
		}

		int totalDamage = 1; 

		//On a hit, we need to start with calculating base damage for the attack
		if (doesHit) { 
			//Uses this formula: (Your Attack * 2) - (Opponent Defense) to a minimum of 1.
			if (!thisAttack.isSkill) { 
				totalDamage = (playerLevel.stats [0] * 2) - enemyUnit.stats [1];
			} else { 
				totalDamage = (playerLevel.stats [2] * 2) - enemyUnit.stats [3];
			} 

			if (totalDamage < 1) { 
				totalDamage = 1;
			}

			Debug.Log ("Total Damage " + totalDamage); 

				if (enemyUnit.type == "trash") {
					if (thisAttack.type == "trash") {
					isDead = enemyUnit.TakeDamage (totalDamage * 2 * thisAttack.damage);
						dialogueText.text = "You deal extra damage with the correct waste disposal method!";
					} else {
					isDead = enemyUnit.TakeDamage (totalDamage * thisAttack.damage);
						dialogueText.text = "There may be a more proper way to handle this enemy";
					}
				} else if (enemyUnit.type == "recycle") { 
					print (thisAttack.type);
					if (thisAttack.type == "recycle") {
					isDead = enemyUnit.TakeDamage (totalDamage * 2 * thisAttack.damage);
						dialogueText.text = "You deal extra damage with the correct waste disposal method!";
					} else {
					isDead = enemyUnit.TakeDamage (totalDamage * thisAttack.damage);
						dialogueText.text = "There may be a more proper way to handle this enemy";
					}
				} else { 
					if (thisAttack.type == "compost") {
					isDead = enemyUnit.TakeDamage (totalDamage * 2 * thisAttack.damage);
						dialogueText.text = "You deal extra damage with the correct waste disposal method!";
					} else {
					isDead = enemyUnit.TakeDamage (totalDamage * thisAttack.damage);
						dialogueText.text = "There may be a more proper way to handle this enemy";
					}
				}
		
			//We need to reset enemy HP after they take damage
			enemyHUD.SetHP (enemyUnit.currentHP); 

			yield return new WaitForSeconds (2f); 

			//check if the enemy is dead 
			//change state based on result 
			if (isDead) {
				//End the battle
				state = BattleState.WON;
				StartCoroutine (EndBattle ());  

			} else {
				//Enemy Turn
				state = BattleState.ENEMYTURN;
				StartCoroutine (EnemyTurn ()); 
			}
		}
	}
		
	IEnumerator PlayerGuard(){ 
		//playerUnit.HealDamage (5); 
		//playerHUD.SetHP (playerUnit.currentHP); 

		dialogueText.text = "You prepare for the next attack!";
		isGuarding = true; 

		yield return new WaitForSeconds (2f); 

		state = BattleState.ENEMYTURN; 
		StartCoroutine(EnemyTurn ()); 
	}

	IEnumerator EnemyTurn(){ 

		//idk this is the best way I've found rn for sorting the enemy attacks. I think each attack will have its own function so
		//that this one doesn't get cluttered
		dialogueText.text = enemyUnit.unitName + " attacks!"; 

		yield return new WaitForSeconds (1f); 

		if (enemyUnit.unitName == "Wrapuchin") { 
			StartCoroutine (wrapuchinAttacks ()); 
		} else if (enemyUnit.unitName == "Octopeel") { 
			StartCoroutine (octopeelAttacks ());
		} else if (enemyUnit.unitName == "Twocan") {
			StartCoroutine (twocanAttacks ()); 
		} else if (enemyUnit.unitName == "Glass Cannon") { 
			StartCoroutine (glassCannonAttacks ()); 
		} else if (enemyUnit.unitName == "Capling") { 
			StartCoroutine (caplingAttacks ()); 
		} else { 
			bool isDead = playerUnit.TakeDamage (1); 

			Debug.Log ("Something went wrong..."); 
			playerHUD.SetHP (playerUnit.currentHP); 

			yield return new WaitForSeconds (1f); 

			//bool isDead; 
			if (isDead) { 
				state = BattleState.LOST; 
				StartCoroutine (EndBattle ()); 
			} else { 
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
			}
		}
	} 

	IEnumerator EndBattle(){ 
		if (state == BattleState.WON) { 

			dialogueText.text = "The enemy dropped " + enemyUnit.pt + " PT!"; 
			playerUnit.pt += enemyUnit.pt; 

			yield return new WaitForSeconds (2f); 

			dialogueText.text = "You gained "  + enemyUnit.experience + " experience!";

			yield return new WaitForSeconds (2f);

			Debug.Log (playerLevel.currentExperience);

			bool didLevelUp = playerLevel.GrantExperience(enemyUnit.experience);

			Debug.Log (playerLevel.currentExperience); 

			if (didLevelUp) { 
				dialogueText.text = "You leveled up!";
				playerUnit.unitLevel = playerLevel.Level;
				//passes in the information about player and enemy unit to set their hud displays
				playerHUD.SetPlayerHUD (playerUnit);
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

			yield return new WaitForSeconds (2f);

			dialogueText.text = "You won the battle!"; 

		//	PrefabUtility.RecordPrefabInstancePropertyModifications(playerPrefab);
		} else if (state == BattleState.LOST) { 
			dialogueText.text = "You were defeated."; 
		}
			
		yield return new WaitForSeconds (2f); 
		SavePlayerData (); 
		SceneManager.LoadScene ("Overworld");
	}

	void PlayerTurn(){
		if (isGuarding) {
			isGuarding = false; 
		}

		if (hasBubbleButt) { 
			System.Random getRandom = new System.Random(); 
			int randomNum = getRandom.Next (0, 101); 
			if (randomNum < 16) { 
				StartCoroutine (CureBubbleButt()); 
				playerLevel.stats [4] = storeMobility;
				hasBubbleButt = false; 
			} 
		} 

		if (isSick) { 

			System.Random getRandom = new System.Random(); 
			int randomNum = getRandom.Next (0, 101); 

			if (randomNum < 16) { 
				StartCoroutine (CureSick ()); 
				isSick = false; 
			} else { 
				StartCoroutine(sickDamage()); 
			} 
		} 

		skills.SetActive (false);
		dialogueBox.SetActive (true);
		dialogueText.text = "Choose an action:"; 
	}

	IEnumerator sickDamage(){ 
		bool isDead = playerUnit.TakeDamage (playerUnit.maxHP/20); 

		playerHUD.SetHP (playerUnit.currentHP); 

		dialogueText.text = "You take some damage from sickness!"; 

		yield return new WaitForSeconds (2f); 

		if (isDead) { 
			state = BattleState.LOST; 
			StartCoroutine (EndBattle ()); 
			yield break; 
		} else { 
			state = BattleState.PLAYERTURN; 
			PlayerTurn ();
			yield break; 
		}
	} 

	IEnumerator CureBubbleButt(){ 
		dialogueText.text = "Your bubblebutt has been cured!"; 
		yield return new WaitForSeconds (2f); 
		PlayerTurn (); 
		yield break; 
	} 

	IEnumerator CureSick(){ 
		dialogueText.text = "Your sickness has been cured!"; 
		yield return new WaitForSeconds (2f); 
		PlayerTurn (); 
		yield break; 
	} 

	//This checks to see if the player chooses the 'Fight' Button
	public void OnAttackButton(){
		//We want to make sure that if the player presses the attack button and it is not their turn, nothing happens
		if (state != BattleState.PLAYERTURN) { 
			return; 
		}
		state = BattleState.PLAYERACTION;
		skills.SetActive (true);
		dialogueBox.SetActive (false);
	}

	public void OnHitButton(){
		if (state == BattleState.PLAYERTURN) { 
			StartCoroutine (Hit ());
		}
	}

	IEnumerator Hit(){ 
		state = BattleState.PLAYERACTION;
		bool doesHit;

		//need to check for hit before we can determine damage 
		System.Random getRandom = new System.Random(); 
		int randomNum = getRandom.Next (0, 101); 

		dialogueText.text = "You attack the enemy!"; 

		int hitChance = 95 - (enemyUnit.stats [4] - playerLevel.stats [4]); 

		if (hitChance < 50) { 
			hitChance = 50;
		}

		if (randomNum < hitChance) { 
			Debug.Log ("Player hits!"); 
			doesHit = true; 
		} else { 
			Debug.Log ("Player misses :("); 
			doesHit = false; 
		}

		//If the player's attack misses, all we need to do is let them know and then proceed with the enemy's turn
		if (!doesHit) {
			yield return new WaitForSeconds(2f); 
			dialogueText.text = "The attack missed!"; 
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn()); 
		}

		//On a hit, we need to start with calculating base damage for the attack
		//Uses this formula: (Your Attack * 2) - (Opponent Defense) to a minimum of 1.
		int totalDamage = (playerLevel.stats [0] * 2) - enemyUnit.stats [1];

		if (totalDamage < 1) { 
			totalDamage = 1;
		}

		bool isDead = enemyUnit.TakeDamage (totalDamage);

		enemyHUD.SetHP (enemyUnit.currentHP); 

		//dialogueText.text = "You hit the enemy!"; 
		yield return new WaitForSeconds (2f); 

		//check if the enemy is dead 
		//change state based on result 
		if (isDead) {
			//End the battle
			state = BattleState.WON;
			StartCoroutine (EndBattle ());  

		} else {
			//Enemy Turn
			state = BattleState.ENEMYTURN;
			StartCoroutine (EnemyTurn ()); 
		}
	}

	public void OnGuardButton(){
		//We want to make sure that if the player presses the attack button and it is not their turn, nothing happens
		if (state != BattleState.PLAYERTURN) { 
			return; 
		}
		state = BattleState.PLAYERACTION;

		StartCoroutine (PlayerGuard ()); 
	}
		
	//This is attached to the skill button, so we are aware which button was pressed
	//Different from onAttackButton, because OnAttackButton only determines if player wishes to attack, not 
	//what skill they choose.
	public void OnSkillSelected(Button button){ 
		Attack thisAttack = button.GetComponent<Attack>();
		if (thisAttack.isSkill) { 
			if (thisAttack.spCost > playerUnit.currentSP) {
				dialogueText.text = "Too little SP to use this skill, please try again"; 
			} else {
				playerUnit.currentSP -= thisAttack.spCost;
				StartCoroutine (PlayerAttack (button));
			}
		} else { 
			StartCoroutine (PlayerAttack (button));
		}
	}

	public void OnBackSelected(){ 
		state = BattleState.PLAYERTURN; 
		PlayerTurn (); 
	} 

	public void SavePlayerData(){ 
		playerLevel.SaveLevelInfo ();
		GlobalControl.Instance.attackList = playerUnit.attackList;
		GlobalControl.Instance.maxHP = playerUnit.maxHP; 
		GlobalControl.Instance.currentHP = playerUnit.currentHP;
		GlobalControl.Instance.maxSP = playerUnit.maxSP; 
		GlobalControl.Instance.currentSP = playerUnit.currentSP;
		GlobalControl.Instance.pt = playerUnit.pt; 
	} 

	public void LoadPlayerData(){ 
		playerLevel.LoadLevelInfo ();
		playerUnit.attackList = GlobalControl.Instance.attackList;
		playerUnit.maxHP = GlobalControl.Instance.maxHP; 
		playerUnit.currentHP = GlobalControl.Instance.currentHP;
		playerUnit.maxSP = GlobalControl.Instance.maxSP; 
		playerUnit.currentSP = GlobalControl.Instance.currentSP;
		playerUnit.pt = GlobalControl.Instance.pt; 
		//enemyUnit.thisFight = GlobalControl.Instance.nextFight; 
	} 

	//Methods for each enemy's attacks
	IEnumerator wrapuchinAttacks(){

		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random(); 

		//Wrapuchin has 2 attacks, so we will pick between those.
		int randomNum = getRandom.Next (1, 3); 
		
			//IDLE
		if (randomNum == 1) {
			dialogueText.text = "Wrapuchin thinks about what it did"; 
			yield return new WaitForSeconds (2f);
			state = BattleState.PLAYERTURN; 
			PlayerTurn (); 
			yield break; 
			//ATTACK
		} else { 

			//Flavor text 
			dialogueText.text = "Wrapuchin swings towards you!"; 

			//Determine hit chance 
			System.Random hitRandom = new System.Random(); 
			randomNum = hitRandom.Next (0, 101); 

			int hitChance = 95 - (playerLevel.stats [4] - enemyUnit.stats [4]); 

			if (hitChance < 50) { 
				hitChance = 50;
			}
		
			//Determine if attack hits  
			bool doesHit; 
			if (randomNum < hitChance) { 
				Debug.Log ("Player hits!"); 
				doesHit = true; 
			} else { 
				Debug.Log ("Player misses :("); 
				doesHit = false; 
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit) {
				yield return new WaitForSeconds (2f); 
				dialogueText.text = "The attack missed!"; 
				state = BattleState.PLAYERTURN;
				PlayerTurn (); 
				yield break; 
			}

			int totalDamage = (enemyUnit.stats [0] * 2) - playerLevel.stats [1];

			//If attack does hit, calculate damage 
			if (totalDamage < 1 && isGuarding) { 
				totalDamage = 0; 
			} else if (isGuarding) { 
				totalDamage = totalDamage / 2; 
				Debug.Log ("player is guarding"); 
			}
					
			if (totalDamage < 1 & isGuarding == false) { 
				totalDamage = 1;
				Debug.Log ("Player is not guarding"); 
			}

			Debug.Log ("Total Enemy Damage by Wrapuchin: " + totalDamage); 

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage (totalDamage); 

			playerHUD.SetHP (playerUnit.currentHP); 

			yield return new WaitForSeconds (2f); 

			if (isDead) { 
				state = BattleState.LOST; 
				StartCoroutine (EndBattle ()); 
				yield break; 
			} else { 
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
				yield break; 
			}
		}
	} 

	IEnumerator twocanAttacks(){ 
		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random(); 

		int randomNum = getRandom.Next (1, 4); 

		//IDLE: Jokes (: 
		if(randomNum == 1){
			randomNum = getRandom.Next (1, 11);
			String dialogue = "The twocan says, ";
			switch(randomNum){ 
			case(1): 
				dialogue += "'Hey calm down. There's no need to get emoceanal'"; 
				break; 
			case(2): 
				dialogue += "'You need to work on getting out of your shell.'"; 
				break; 
			case(3):
				dialogue += "'It’s cool to get out of can school and meet all the alumni.'"; 
				break; 
			case(4): 
				dialogue += "'Water you doing? I can’t sea how this helps you.'"; 
				break; 
			case(5): 
				dialogue += "'Hey, I can see Seattle from here. We should sail further out.'"; 
				break; 
			case(6): 
				dialogue += "'I can not believe I have two fight you.'"; 
				break; 
			case(7): 
				dialogue += "'I’m gonna beach ya.'"; 
				break; 
			case(8): 
				dialogue += "'This place is as big a dump as your mom’s house.'"; 
				break; 
			case(9): 
				dialogue += "'It’s a good thing my material’s recyclable.'"; 
				break; 
			case(10): 
				dialogue += "'I’ve seen better turtling playing Risk.'"; 
				break; 
			default: 
				break; 
			} 
			dialogueText.text = dialogue; 
			yield return new WaitForSeconds(2f);
			state = BattleState.PLAYERTURN; 
			PlayerTurn (); 
			yield break; 
			//ATTACK
		} else if (randomNum == 2) { 
				//Flavor text 
				dialogueText.text = "The Twocan scratches off part of its label."; 
				yield return new WaitForSeconds(1f);
				randomNum = getRandom.Next (1, 4);
				if (randomNum == 3) { 
					dialogueText.text = "Something has made you sick. Every turn, you take damage equal to 5% of your max health, rounded down.";
					yield return new WaitForSeconds (2f); 
					state = BattleState.PLAYERTURN; 
					PlayerTurn (); 
				yield break; 
				}
			state = BattleState.PLAYERTURN;
			PlayerTurn();
			yield break; 
		} else { 
			dialogueText.text = "The Twocan punches down at you."; 
	
			//Determine hit chance 
			System.Random hitRandom = new System.Random(); 
			randomNum = hitRandom.Next (0, 101); 

			int hitChance = 95 - (playerLevel.stats [4] - enemyUnit.stats [4]); 

			if (hitChance < 50) { 
				hitChance = 50;
			}

			//Determine if attack hits  
			bool doesHit; 
			if (randomNum < hitChance) { 
				Debug.Log ("Twocan hits!"); 
				doesHit = true; 
			} else { 
				Debug.Log ("Twocan misses :("); 
				doesHit = false; 
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit) {
				yield return new WaitForSeconds(2f); 
				dialogueText.text = "The attack missed!"; 
				state = BattleState.PLAYERTURN;
				PlayerTurn(); 
				yield break; 
			}

			//If attack does hit, calculate damage 
			int totalDamage = (enemyUnit.stats [0] * 2) - playerLevel.stats [1];

			if(totalDamage < 1 && !isGuarding){ 
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Twocan: " + totalDamage); 

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage (totalDamage); 

			playerHUD.SetHP(playerUnit.currentHP); 

			yield return new WaitForSeconds (2f); 

			if (isDead) { 
				state = BattleState.LOST; 
				StartCoroutine(EndBattle ());
				yield break; 
			} else { 
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
				yield break; 
			}
		} 
	} 

	IEnumerator octopeelAttacks(){

		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random(); 

		int randomNum = getRandom.Next (1, 4); 

		//IDLE
		if(randomNum == 1){
			dialogueText.text = "Octopeel slithers around on the ground."; 
			yield return new WaitForSeconds(2f);
			state = BattleState.PLAYERTURN; 
			PlayerTurn ();
			yield break; 
			//ATTACK
		} else { 
			if (randomNum == 2) { 
				//Flavor text 
				dialogueText.text = "Octopeel attracts a squadron of seagulls."; 
			} else { 
				dialogueText.text = "Octopeel slips under your flipper and trips you."; 
			}
			//Determine hit chance 
			System.Random hitRandom = new System.Random(); 
			randomNum = hitRandom.Next (0, 101);

			int hitChance = 95 - (playerLevel.stats [4] - enemyUnit.stats [4]); 

			if (hitChance < 50) { 
				hitChance = 50;
			}

			//Determine if attack hits  
			bool doesHit; 
			if (randomNum < hitChance) { 
				Debug.Log ("Octopeel hits!"); 
				doesHit = true; 
			} else { 
				Debug.Log ("Octopeel misses :("); 
				doesHit = false; 
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit) {
				yield return new WaitForSeconds(2f); 
				dialogueText.text = "The attack missed!"; 
				state = BattleState.PLAYERTURN;
				PlayerTurn(); 
				yield break; 
			}

			//If attack does hit, calculate damage 
			//For octopeel, damage changes depending on which of its two attacks it uses. If the randomNum generated 2, 
			//octopeel needs to do standard skill damage. 
			int totalDamage = 1; 
			
			if (randomNum == 3) { 
				totalDamage = (enemyUnit.stats [0] * 2) - playerLevel.stats [1];
			} else { 
				totalDamage = (enemyUnit.stats [2] * 2) - playerLevel.stats [3];
			}

			if(totalDamage < 1){ 
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Octopeel: " + totalDamage); 

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage (totalDamage); 

			playerHUD.SetHP(playerUnit.currentHP); 

			yield return new WaitForSeconds (2f); 

			if (isDead) { 
				state = BattleState.LOST; 
				StartCoroutine(EndBattle ()); 
				yield break; 
			} else { 
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
				yield break; 
			}
		} 
	}

	IEnumerator glassCannonAttacks(){

		//glass cannon has the chance to 'hold an attack' by lighting its fuse. we need to make sure not to do typical 
		//attack sequence if it needs to use this other attack. 

		System.Random getRandom = new System.Random(); 

		if (isLoading) {
			isLoading = false; 
			int totalDamage = (enemyUnit.stats [2] * 2) - playerLevel.stats [3];
			int randomNum = getRandom.Next (0, 11); 
			if (randomNum != 1) { 
				dialogueText.text = "Glass Cannon fires.";
				bool isDead = playerUnit.TakeDamage (totalDamage); 

				playerHUD.SetHP(playerUnit.currentHP); 

				yield return new WaitForSeconds (2f); 

				if (isDead) { 
					state = BattleState.LOST; 
					StartCoroutine(EndBattle ()); 
					yield break; 
				} else { 
					state = BattleState.PLAYERTURN; 
					PlayerTurn (); 
					yield break; 
				}
			} else { 
				dialogueText.text = "Glass Cannon misfires.";
				bool isDead = enemyUnit.TakeDamage (totalDamage);

				enemyHUD.SetHP (enemyUnit.currentHP);
				yield return new WaitForSeconds (2f);
				if (isDead) { 
					state = BattleState.WON; 
					StartCoroutine(EndBattle ()); 
					yield break; 
				} else { 
					state = BattleState.PLAYERTURN; 
					PlayerTurn (); 
					yield break; 
				}	
			} 
		} else if(!isLoading){ 
			int randomNum = getRandom.Next (1, 5); 

			//IDLE
			if(randomNum == 1){
				dialogueText.text = "Glass Cannon loosens up."; 
				yield return new WaitForSeconds(2f);
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
				//ATTACK
			} else if (randomNum == 2) { 
				//Flavor text 
				dialogueText.text = "Glass Cannon cuts you with its sharp edges."; 
			} else  { 
				dialogueText.text = "Glass Cannon lights its fuse.";
				isLoading = true; 
				yield return new WaitForSeconds(2f);
				Debug.Log ("lit fuse"); 
				state = BattleState.PLAYERTURN;
				PlayerTurn (); 
				yield break; 
			}

			if (randomNum == 2) { 
				//Determine hit chance 
				System.Random hitRandom = new System.Random(); 
				randomNum = hitRandom.Next (0, 101); 

				int hitChance = 95 - (playerLevel.stats [4] - enemyUnit.stats [4]); 

				if (hitChance < 50) { 
					hitChance = 50;
				}

				//Determine if attack hits  
				bool doesHit; 
				if (randomNum < hitChance) { 
					Debug.Log ("Glass Cannon hits!"); 
					doesHit = true; 
				} else { 
					Debug.Log ("Glass Cannon misses :("); 
					doesHit = false; 
				}

				//If attack does not hit, inform player and then switch back to PLAYERTURN
				if (!doesHit) {
					yield return new WaitForSeconds (2f); 
					dialogueText.text = "The attack missed!"; 
					state = BattleState.PLAYERTURN;
					PlayerTurn (); 
					yield break; 
				}

				//If attack does hit, calculate damage 
				//For Glass cannon, it only does standard attack damage except for inside of its firing section
				int totalDamage = (enemyUnit.stats [2] * 2) - playerLevel.stats [3];


				if (totalDamage < 1) { 
					totalDamage = 1;
				}

				Debug.Log ("Total Enemy Damage by Glass Cannon: " + totalDamage); 

				//Use this TakeDamage() function to deal the proper amount of damage to player
				bool isDead = playerUnit.TakeDamage (totalDamage); 

				playerHUD.SetHP (playerUnit.currentHP); 

				yield return new WaitForSeconds (2f); 

				if (isDead) { 
					state = BattleState.LOST; 
					StartCoroutine (EndBattle ()); 
					yield break; 
				} else { 
					state = BattleState.PLAYERTURN; 
					PlayerTurn ();
					yield break; 
				}
			}
		} 
	}

	IEnumerator caplingAttacks(){

		//There is a lot of randomness in enemy attacks, so it needs to start with randomly picking between x enemy attacks
		System.Random getRandom = new System.Random(); 

		int randomNum = getRandom.Next (1, 5); 
		bool causesBubblebutt = false; 

		//IDLE
		if(randomNum == 1){
			dialogueText.text = "Capling tries to hoist a flag that is no longer there."; 
			yield return new WaitForSeconds(2f);
			state = BattleState.PLAYERTURN; 
			PlayerTurn ();
			yield break; 
			//ATTACK
		} else { 
			if (randomNum == 2) { 
				//Flavor text 
				dialogueText.text = "Capling jumps down your throat."; 
				randomNum = getRandom.Next (1, 5);  
				if (randomNum == 4) { 
					Debug.Log ("bubblebutt :P"); 
					causesBubblebutt = true; 
				}
			} else { 
				dialogueText.text = "Capling scratches you with its hook hand."; 
			}
			//Determine hit chance 
			System.Random hitRandom = new System.Random(); 
			randomNum = hitRandom.Next (0, 101); 

			int hitChance = 95 - (playerLevel.stats [4] - enemyUnit.stats [4]); 

			if (hitChance < 50) { 
				hitChance = 50;
			}
				
			//Determine if attack hits  
			bool doesHit; 
			if (randomNum < hitChance) { 
				Debug.Log ("Capling hits!"); 
				doesHit = true; 
			} else { 
				Debug.Log ("Capling misses :("); 
				doesHit = false; 
			}

			//If attack does not hit, inform player and then switch back to PLAYERTURN
			if (!doesHit) {
				yield return new WaitForSeconds (2f); 
				dialogueText.text = "The attack missed!"; 
				yield return new WaitForSeconds (2f);
				state = BattleState.PLAYERTURN;
				PlayerTurn();
				yield break; 
			}

			//If attack does hit, calculate damage 
			//For Capling, damage changes depending on which of its two attacks it uses. If the randomNum generated is 2, it needs
			//to do skill damage and check for bubblebutt. 50% of the time is regular attack damage (randomNums 3 and 4) 

			int totalDamage = 1; 

			if (randomNum == 2) { 
				Debug.Log (" a throat jump"); 
				totalDamage = (enemyUnit.stats [2] * 2) - playerLevel.stats [3];
			} else { 
				totalDamage = (enemyUnit.stats [0] * 2) - playerLevel.stats [1];
			}

			if(totalDamage < 1){ 
				totalDamage = 1;
			}

			Debug.Log("Total Enemy Damage by Capling: " + totalDamage); 

			//Use this TakeDamage() function to deal the proper amount of damage to player
			bool isDead = playerUnit.TakeDamage (totalDamage); 

			playerHUD.SetHP(playerUnit.currentHP); 

			yield return new WaitForSeconds (2f); 

			if (causesBubblebutt) { 
				dialogueText.text = "You have been inflicted with Bubblebutt. Gases built up inside of you cause you to float! Your mobility is reduced to 0."; 
				playerLevel.stats [4] = 0; 
				hasBubbleButt = true; 
				yield return new WaitForSeconds (2f); 
			} 

			if (isDead) { 
				state = BattleState.LOST; 
				StartCoroutine(EndBattle ()); 
				yield break; 
			} else { 
				state = BattleState.PLAYERTURN; 
				PlayerTurn (); 
				yield break; 
			}
		} 
	}
}