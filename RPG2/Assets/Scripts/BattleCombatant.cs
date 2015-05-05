using UnityEngine;
using System.Collections.Generic;

public abstract class BattleCombatant : MonoBehaviour {
	public string Name { get; protected set; }
	public Stat Health { get; protected set; }
	public int Strength { get; protected set; }
	public List<Ability> Abilities { get; private set; }

	public bool isDead = false;

	public ObjectUI ObjectUI { get; private set; }
	private Animator anim;

	
	public virtual void Start() {
		anim = GetComponent<Animator>();
		ObjectUI = GetComponent<ObjectUI>();

		// set up attack
		Ability attack = new Ability();
		attack.Name = "Attack";
		attack.Type = AbilityType.Attack;
		attack.Power = 5;

		Ability heal = new Ability();
		heal.Name = "Heal";
		heal.Type = AbilityType.Heal;
		heal.Power = 20;

		Ability magic = new Ability();
		magic.Name = "Magic";
		magic.Type = AbilityType.Magic;
		magic.Power = 10;

		Abilities = new List<Ability>();
		Abilities.Add(attack);
		Abilities.Add(heal);
		Abilities.Add(magic);
		Abilities.Add(attack);
	}

	public virtual void Update() {
	}
	
	public abstract void UseAbility(Ability ability, BattleCombatant target);

	public void Damage(int value) {
		Health.Current -= value;
		ObjectUI.SetHealthFillAmount(Health.GetRatio());
		ObjectUI.ShowDamageValue(value.ToString());
		PlayStruckAnim();

		if (Health.IsCurrentZero()) {
			isDead = true;
			anim.SetBool("isDead", true);
		}
	}

	public void Heal(int value) {
		Health.Current += value;
		ObjectUI.SetHealthFillAmount(Health.GetRatio());
		ObjectUI.ShowHealValue(value.ToString());
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
		int rng = Random.Range(0, 2);
		if (rng == 0)
			anim.SetTrigger("playAttack");
		else
			anim.SetTrigger("playAttack2");
	}

	public void PlayMagicAnim() {
		anim.SetTrigger("playMagic");
	}

	public void PlayItemAnim() {
		anim.SetTrigger("playItem");
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
