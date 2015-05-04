using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectUI : MonoBehaviour {
	public Image HealthBar;
	public Text DamageText;
	public Animator DamageTextAnim;
	public Animator TargetArrowAnim;

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
}
