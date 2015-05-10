using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BattleUIController : MonoBehaviour {
	public List<Toggle> Buttons = new List<Toggle>(5);
	private List<string> abilityTooltips = new List<string>();

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

	public GameObject Tooltip;
	public Text TooltipText;

	
	void Start() {
		Tooltip.transform.position = new Vector3(10000, 10000, 0);
		DisableButtons();
	}

	public void UpdateUI(string characterName, List<Ability> abilities) {
		CharacterName.text = characterName;
		// TODO: Character JOB
		Ability1.text = abilities[0].Name;
		Ability2.text = abilities[1].Name;
		Ability3.text = abilities[2].Name;
		Ability4.text = abilities[3].Name;

		// update tooltips
		abilityTooltips.Clear();

		foreach (Ability a in abilities) {
			abilityTooltips.Add(a.ToString());
		}
	}

	public void UpdateHealthBar(Stat health) {
		HealthText.text = health.ToString();
		HealthBar.fillAmount = health.GetRatio();
	}

	public void DisableButtons() {
		foreach (Toggle t in Buttons) 
			t.interactable = false;
	}

	public void EnableButtons(List<Ability> abilities, int rank) {
		if (abilities.Count < 4) return;

		for (int i = 0; i < abilities.Count; i++) {
			Buttons[i].interactable = abilities[i].IsUsable(rank);
        }

		// swap button enabled
		Buttons[4].interactable = true;
	}

	public void ResetButtons() {
		foreach (Toggle t in Buttons) 
			t.isOn = false;
	}

	/// <summary>
	/// Return true if there is at least one button selected
	/// </summary>
	public bool IsAButtonSelected() {
		return Buttons.Find((Toggle t) => t.isOn);
	}

	public int WhichButtonIsSelected() {
		return Buttons.FindIndex((Toggle t) => t.isOn);
	}

	public void ShowAbilityName(string abilityName) {
		AbilityNameText.text = abilityName;
		AbilityNameAnim.Play("AbilityNameShow");
	}

	public void ShowTooltip(int index) {
		Toggle t = Buttons[index];
		RectTransform rt = (RectTransform)Tooltip.transform;
		float height = rt.sizeDelta.y;

		Vector3 newPos = new Vector3(t.transform.position.x, 
		                             t.transform.position.y + height, 
		                             t.transform.position.z);

		TooltipText.text = abilityTooltips[index];
		Tooltip.transform.position = newPos;
	}

	public void HideTooltip() {
		Tooltip.transform.position = new Vector3(10000, 10000, 0);
	}
}
