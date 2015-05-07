using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleUIController : MonoBehaviour {
	public Toggle BtnAbility1;
	public Toggle BtnAbility2;
	public Toggle BtnAbility3;
	public Toggle BtnAbility4;
	public Toggle BtnAbility5;

	public Text Ability1;
	public Text Ability2;
	public Text Ability3;
	public Text Ability4;

	public Image HealthBar;
	public Text HealthText;

	public Text CharacterName;
	public Text CharacterJob;
	public Image CharacterFace;

	public Animator AbilityNameAnim;
	public Text AbilityNameText;

	void Start() {
		DisableButtons();
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

	public void DisableButtons() {
		BtnAbility1.interactable = false;
		BtnAbility2.interactable = false;
		BtnAbility3.interactable = false;
		BtnAbility4.interactable = false;
		BtnAbility5.interactable = false;
	}

	public void EnableButtons(List<Ability> abilities, int rank) {
		if (abilities.Count < 4) return;

		BtnAbility1.interactable = abilities[0].IsUsable(rank);
		BtnAbility2.interactable = abilities[1].IsUsable(rank);
		BtnAbility3.interactable = abilities[2].IsUsable(rank);
		BtnAbility4.interactable = true;
		//BtnAbility4.interactable = abilities[3].IsUsable(rank);
	}

	public void EnableButtons() {
		BtnAbility1.interactable = true;
		BtnAbility2.interactable = true;
		BtnAbility3.interactable = true;
		BtnAbility4.interactable = true;
		BtnAbility5.interactable = true;
	}

	public void ResetButtons() {
		BtnAbility1.isOn = false;
		BtnAbility2.isOn = false;
		BtnAbility3.isOn = false;
		BtnAbility4.isOn = false;
		BtnAbility5.isOn = false;
	}

	/// <summary>
	/// Return true if there is at least one button selected
	/// </summary>
	public bool IsAButtonSelected() {
		return BtnAbility1.isOn || BtnAbility2.isOn ||
				BtnAbility3.isOn || BtnAbility4.isOn ||
				BtnAbility5.isOn;
	}

	public void ShowAbilityName(string abilityName) {
		AbilityNameText.text = abilityName;
		AbilityNameAnim.Play("AbilityNameShow");
	}
}
