using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectUI : MonoBehaviour {
	public Image HealthBar;
	public Text DamageText;
	public Animator DamageTextAnim;

	public void SetHealthFillAmount(float fillAmount) {
		HealthBar.fillAmount = fillAmount;
	}

	public void ShowDamageValue(string value) {
		DamageText.text = value;
		DamageTextAnim.Play("DamageShow");
	}
}
