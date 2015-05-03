using UnityEngine;
using System.Collections.Generic;

public abstract class BattleCombatant : MonoBehaviour {
	public string Name { get; protected set; }
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
		attack.Type = AbilityType.Attack;
		attack.Power = 5;

		Ability heal = new Ability();
		heal.Name = "Heal";
		heal.Type = AbilityType.Heal;
		heal.Power = 20;

		Abilities = new List<Ability>();
		Abilities.Add(attack);
		Abilities.Add(heal);
	}

	public virtual void Update() {
	}

	public abstract void UseAbility(Ability ability, BattleCombatant target);

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

	public void Heal(int value) {
		Health.Current += value;
		objectUI.SetHealthFillAmount(Health.GetRatio());
		objectUI.ShowHealValue(value.ToString());
		PlayHealAnim();
	}

	/// <summary>
	/// Returns true or false if the object is animating an action.
	/// (returns false for Idle states)
	/// </summary>
	public bool isAnimating() {
		AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
		return !currentState.IsName("Idle") && 
				!currentState.IsName("Dead") && 
				!currentState.IsName("Victory");
	}

	public void PlayAttackAnim() {
		anim.SetTrigger("playAttack");
	}

	public void PlayUseItemAnim() {
		anim.SetTrigger("playUseItem");
	}

	public void PlayVictoryAnim() {
		anim.SetBool("isVictory", true);
	}

	private void PlayStruckAnim() {
		anim.SetTrigger("playStruck");
	}

	private void PlayHealAnim() {
		anim.SetTrigger("playHealing");
	}
}
