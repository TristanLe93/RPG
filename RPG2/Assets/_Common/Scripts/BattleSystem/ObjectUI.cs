using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectUI : MonoBehaviour {
	public Image HealthBar;
	public Image TurnIcon;
	public Image TargetArrow;
	public Text DamageText;
	public Animator DamageTextAnim;


	void Start() {
		HideTurnIcon();
		HideTargetArrow();
	}

	public void SetHealthFillAmount(float fillAmount) {
		HealthBar.fillAmount = fillAmount;
	}

	public void ShowDamageValue(string value) {
		DamageText.text = value;
		DamageTextAnim.Play("DamageShow");
	}

	public void ShowHealValue(string value) {
		DamageText.text = value;
		DamageTextAnim.Play("HealShow");
	}

	public void ShowTargetArrow() {
		TargetArrow.color = new Color(255, 255, 255, 255);
	}

	public void HideTargetArrow() {
		TargetArrow.color = new Color(255, 255, 255, 0);
	}

	public void ShowTurnIcon() {
		TurnIcon.color = new Color(255, 255, 255, 255);
	}

	public void HideTurnIcon() {
		TurnIcon.color = new Color(255, 255, 255, 0);
	}
}
