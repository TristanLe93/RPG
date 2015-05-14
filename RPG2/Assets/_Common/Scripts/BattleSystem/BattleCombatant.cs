using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A BattleCombatant is a participant in a battle who fights other BattleCombatants. 
/// This class maintains statistics like health and strength.
/// </summary>
public abstract class BattleCombatant : MonoBehaviour {
	public string Name;

	// The stats of this combatant, attached via GameObject
	[HideInInspector]
	public CombatantStats Stats;

	// The UI Canvas controller to display things on the screen, 
	// like health bars and damage text.
	[HideInInspector]
	public ObjectUI ObjectUI;
	[HideInInspector]
	public bool IsDead = false;

	// Each combatant has their own sprite layer for animations, 
	// specified by AnimationLayerIndex.
	public int AnimationLayerIndex = 0;
	protected Animator anim;
	
	public List<Ability> Abilities;
	public List<StatusEffect> StatusEffects;

	
	public virtual void Start() {
		Stats = GetComponent<CombatantStats>();
		ObjectUI = GetComponent<ObjectUI>();
		anim = GetComponent<Animator>();
		anim.SetLayerWeight(AnimationLayerIndex, 1.0f);
	}

	public virtual void Update() { 
	}

	/// <summary>
	/// Reduce the Health Stat by the value.
	/// </summary>
	public void Damage(int value) {
		// deal at least one damage when being damaged
		if (value <= 0) {
			value = 1;
		}

		Stats.Health.Current -= value;
		ObjectUI.SetHealthFillAmount(Stats.Health.GetRatio());
		ObjectUI.ShowDamageValue(value.ToString());
		anim.SetTrigger("playStruck");

		// set dead condition if Health is zero
		if (Stats.Health.IsCurrentZero()) {
			IsDead = true;
			StatusEffects.Clear();
			anim.SetBool("isDead", true);
		}
	}

	/// <summary>
	/// Increase the Health stat by the value
	/// </summary>
	public void Heal(int value) {
		if (value > 0) {
			Stats.Health.Current += value;
			ObjectUI.SetHealthFillAmount(Stats.Health.GetRatio());
			ObjectUI.ShowHealValue(value.ToString());
		}

		anim.SetTrigger("playHealing");
	}
	
	/// <summary>
	/// Use the ability on the target.
	/// </summary>
	protected void ExecuteAbility(Ability ability, BattleCombatant target) {
		int dmg = 0;
		
		switch (ability.Type) {
		case AbilityType.Melee: 
			dmg = ability.Power + Stats.Strength - target.Stats.Defense;
			target.Damage(dmg);
			break;
			
		case AbilityType.Magic: 
			dmg = ability.Power + Stats.Magic - target.Stats.Spirit;
			target.Damage(dmg);
			break;
			
		case AbilityType.Heal: 
			dmg = ability.Power;
			target.Heal(dmg);
			break;
			
		case AbilityType.Ranged: 
			dmg = ability.Power + Stats.Strength - target.Stats.Defense;
			target.Damage(dmg);
			break;
		}
		
		// apply status effect to target if any
		if (ability.ApplyStatus.Count > 0) {
			foreach (StatusEffect status in ability.ApplyStatus) {
				StatusEffect statusClone = Object.Instantiate(status) as StatusEffect;
				target.StatusEffects.Add(statusClone);
			}
		}
	}


	/// <summary>
	/// Performs Damage over Turn (DoT) status effects on the player
	/// </summary>
	public void DoStatusEffects() {
		int damage = 0;

		// go through status effects and add damage if any.
		for (int i = StatusEffects.Count-1; i >= 0; i--) {
			damage += StatusEffects[i].DamagePerTurn;

			StatusEffects[i].Duration -= 1;

			// remove completed status effects
			if (StatusEffects[i].Duration <= 0) {
				StatusEffects.RemoveAt(i);
			}
		}

		// damage combatant if there is any
		if (damage > 0) {
			this.Damage(damage);
		}
	}

	/// <summary>
	/// Determines whether the player stunned by Status Effect.
	/// </summary>
	public bool IsStunned() {
		foreach (StatusEffect status in StatusEffects) {
			if (status.SkipsTurn) {
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Determines whether the combatant is animating an action.
	/// </summary>
	public bool isAnimating() {
		AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(AnimationLayerIndex);
		return !currentState.IsName("Idle") && 
				!currentState.IsName("Dead") && 
				!currentState.IsName("Victory");
	}

	/// <summary>
	/// Play a specific animation depending on AbilityType
	/// </summary>
	protected void PlayAnimation(AbilityType type) {
		switch (type) {
		case AbilityType.Melee: PlayAttackAnim(); break;
		case AbilityType.Magic: PlayMagicAnim(); break;
		case AbilityType.Heal: PlayItemAnim(); break;
		case AbilityType.Ranged: PlayAttackAnim(); break;
		}
	}

	private void PlayAttackAnim() {
		int rng = Random.Range(0, 2);
		if (rng == 0)
			anim.SetTrigger("playAttack");
		else
			anim.SetTrigger("playAttack2");
	}

	private void PlayMagicAnim() {
		anim.SetTrigger("playMagic");
	}

	private void PlayItemAnim() {
		anim.SetTrigger("playItem");
	}
}
