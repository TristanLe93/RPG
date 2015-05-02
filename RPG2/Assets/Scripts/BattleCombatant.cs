using UnityEngine;
using System.Collections.Generic;

public abstract class BattleCombatant : MonoBehaviour {
	public Stat Health { get; protected set; }
	public int Strength { get; protected set; }
	public List<Ability> Abilities { get; private set; }
	public bool isDead = false;

	private Animator anim;
	private ObjectUI objectUI;


	public virtual void Start() {
		anim = GetComponent<Animator>();
		objectUI = GetComponent<ObjectUI>();

		// set up attack
		Ability attack = new Ability();
		attack.Name = "Attack";
		attack.Power = 5;

		Abilities = new List<Ability>();
		Abilities.Add(attack);
	}

	public virtual void Update() {
	}

	public void Damage(int value) {
		Health.Current -= value;
		objectUI.SetHealthFillAmount(Health.GetRatio());
		objectUI.ShowDamageValue(value.ToString());
		PlayStruckAnim();

		if (Health.IsCurrentZero()) {
			isDead = true;
			anim.SetBool("isDead", true);
		}
	}

	/// <summary>
	/// Returns true or false if the object is animating an action.
	/// (returns false for Idle states)
	/// </summary>
	public bool isAnimating() {
		AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
		return !currentState.IsName("Idle") && !currentState.IsName("Dead");
	}

	public void PlayAttackAnim() {
		anim.SetTrigger("playAttack");
	}
	
	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}

	private void PlayStruckAnim() {
		anim.SetTrigger("playStruck");
	}
}
