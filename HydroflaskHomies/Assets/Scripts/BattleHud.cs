using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class BattleHud : MonoBehaviour {
	public Text nameText; 
	public Text levelText; 
	public Slider hpSlider; 

	public void SetPlayerHUD(Player player){ 
		nameText.text = player.unitName;
		levelText.text = "Lvl " + player.unitLevel;
		hpSlider.maxValue = player.maxHP; 
		Debug.Log ("Player's HP when hud is set: " + player.currentHP);
		hpSlider.value = player.currentHP; 
	}

	public void SetEnemyHUD(Enemy enemy){
		nameText.text = enemy.unitName;
		levelText.text = "Lvl " + enemy.unitLevel;
		hpSlider.maxValue = enemy.maxHP; 
		hpSlider.value = enemy.currentHP; 
	}

	public void SetHP(int hp){ 
		//Debug.Log ("Player's HP when hud is set: " + player.currentHP);
		hpSlider.value = hp; 
	}
}
