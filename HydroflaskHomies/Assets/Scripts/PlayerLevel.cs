using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevel : MonoBehaviour {
	public int Level;
	public int currentExperience;
	public int requiredExperience; 
	public int[] stats = new int[5];
	public GameObject PlayerPrefab;
	Player playerUnit; 

	void Start () { 
		LoadLevelInfo ();
		playerUnit = PlayerPrefab.GetComponent<Player> (); 
		Debug.Log ("setting up level system"); 
		LevelSet (Level); 
	}

	public void GetEnemyExperience(Enemy enemy) { 
		GrantExperience (enemy.experience); 
	} 

	public bool GrantExperience(int amount) { 
		currentExperience += amount; 
		bool didLevelUp;
		Debug.Log (requiredExperience); 
		if (currentExperience >= requiredExperience) { 
			//Debug.Log ("yo"); 
			Level += 1; 
			didLevelUp = true; 
			LevelSet (Level);
		} else { 
			didLevelUp = false; 
		}
		return didLevelUp;
	} 

	// TODO: name me!!!!!!!!
	public void setStats(int a, int d, int s, int t, int m) { 
		stats [0] = a;
		stats [1] = d; 
		stats [2] = s;
		stats [3] = t;
		stats [4] = m;
	}

	public void modifyStats(int a, int d, int s, int t, int m) { 
		stats [0] += a;
		stats [1] += d; 
		stats [2] += s;
		stats [3] += t;
		stats [4] += m;
	}

	public void SetPlayerAttack(int value) { 
		stats [0] = value;
	}

	public void SetPlayerDefense(int value) { 
		stats [1] = value;
	}

	public void SetPlayerSkill(int value) { 
		stats [2] = value;
	}

	public void SetPlayerTechnique(int value) { 
		stats [3] = value;
	}

	public void SetPlayerMobility(int value) { 
		stats [4] = value;
	}

	public void LevelSet(int level) { 
		Level = level; 
		switch (Level) { 
		case 1:
			requiredExperience = 20; 
			setStats (5, 5, 5, 5, 5); 
			playerUnit.maxHP = 40; 
			//playerUnit.currentHP = 25; 
			playerUnit.maxSP = 25; 
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
			break;
		case 2: 
			requiredExperience = 20; 
			modifyStats (1, 2, 2, 1, 1); 
			break;
		case 3: 
			requiredExperience = 40; 
			modifyStats(1, 2, 2, 1, 1); 
			break; 
		case 4: 
			requiredExperience = 60; 
			modifyStats(1, 2, 2, 1, 1); 
			break; 
		case 5: 
			requiredExperience = 100;
			modifyStats(1, 2, 2, 1, 1); 
			break; 
		case 6:
			requiredExperience = 150; 
			modifyStats(1, 2, 2, 1, 1); 
			break;
		case 7: 
			requiredExperience = 200;
			modifyStats(1, 2, 2, 1, 1); 
			break;
		default:
			Debug.Log ("An unexpected value ({levelCaseSwitch})");
			break;
		}	
	}

	public void SaveLevelInfo(){ 
		GlobalControl.Instance.playerLevel = Level; 
		GlobalControl.Instance.currentExperience = currentExperience; 
		GlobalControl.Instance.requiredExperience = requiredExperience;
		GlobalControl.Instance.stats = stats;
	}

	public void LoadLevelInfo(){ 
		Level = GlobalControl.Instance.playerLevel; 
		currentExperience = GlobalControl.Instance.currentExperience; 
		requiredExperience = GlobalControl.Instance.requiredExperience; 
		stats = GlobalControl.Instance.stats;
	}
}
