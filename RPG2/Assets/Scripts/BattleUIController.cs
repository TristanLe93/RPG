using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleUIController : MonoBehaviour {
	public Button BtnAbility1;
	public Button BtnAbility2;
	public Button BtnAbility3;
	public Button BtnAbility4;
	public Button BtnAbility5;
	
	public Text Ability1;
	public Text Ability2;
	public Text Ability3;
	public Text Ability4;

	public Image HealthBar;
	public Text HealthText;

	public Text CharacterName;
	public Text CharacterJob;
	public Image CharacterFace;


	void Start() {
		DisableUI();
	}

	public void UpdateUI(string characterName, List<Ability> abilities) {
		CharacterName.text = characterName;
		// TODO: Character JOB
		Ability1.text = abilities[0].Name;
		Ability2.text = abilities[1].Name;
		Ability3.text = abilities[2].Name;
		Ability4.text = abilities[3].Name;
	}

	public void UpdateHealthBar(Stat health) {
		HealthText.text = health.ToString();
		HealthBar.fillAmount = health.GetRatio();
	}

	public void DisableUI() {
		BtnAbility1.interactable = false;
		BtnAbility2.interactable = false;
		BtnAbility3.interactable = false;
		BtnAbility4.interactable = false;
		BtnAbility5.interactable = false;
	}

	public void EnableUI() {
		BtnAbility1.interactable = true;
		BtnAbility2.interactable = true;
		BtnAbility3.interactable = true;
		BtnAbility4.interactable = true;
		BtnAbility5.interactable = true;
	}
}
