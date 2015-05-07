using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectUI : MonoBehaviour {
	public Image HealthBar;
	public Image TurnIcon;
	public Text DamageText;
	public Animator DamageTextAnim;
	public Animator TargetArrowAnim;

	void Start() {
		HideTurnIcon();
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
		TargetArrowAnim.Play("TargetArrowAnim");
	}

	public void HideTargetArrow() {
		TargetArrowAnim.Play("TargetArrowIdle");
	}

	public void ShowTurnIcon() {
		TurnIcon.color = new Color(255, 255, 255, 255);
	}

	public void HideTurnIcon() {
		TurnIcon.color = new Color(255, 255, 255, 0);
	}
}
