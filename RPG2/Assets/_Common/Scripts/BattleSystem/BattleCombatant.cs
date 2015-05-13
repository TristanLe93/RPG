using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class BattleCombatant : MonoBehaviour {
	public string Name;

	[HideInInspector]
	public CombatantStats Stats;
	[HideInInspector]
	public ObjectUI ObjectUI;
	[HideInInspector]
	public bool IsDead = false;


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

	public void Damage(int value) {
		if (value <= 0) {
			value = 1;
		}

		Stats.Health.Current -= value;
		ObjectUI.SetHealthFillAmount(Stats.Health.GetRatio());
		ObjectUI.ShowDamageValue(value.ToString());
		anim.SetTrigger("playStruck");

		// check if the player is dead
		if (Stats.Health.IsCurrentZero()) {
			IsDead = true;
			StatusEffects.Clear();
			anim.SetBool("isDead", true);
		}
	}

	public void Heal(int value) {
		if (value > 0) {
			Stats.Health.Current += value;
			ObjectUI.SetHealthFillAmount(Stats.Health.GetRatio());
			ObjectUI.ShowHealValue(value.ToString());
		}

		anim.SetTrigger("playHealing");
	}

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
			dmg = ability.Power + Stats.Strength  - target.Stats.Defense;
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

	protected void PlayAnimation(AbilityType type) {
		switch (type) {
		case AbilityType.Melee: PlayAttackAnim(); break;
		case AbilityType.Magic: PlayMagicAnim(); break;
		case AbilityType.Heal: PlayItemAnim(); break;
		case AbilityType.Ranged: PlayAttackAnim(); break;
		}
	}

	public void DoStatusEffects() {
		int damage = 0;

		for (int i = StatusEffects.Count-1; i >= 0; i--) {
			if (StatusEffects[i].DamagePerTurn > 0) {
				damage += StatusEffects[i].DamagePerTurn;
			}

			StatusEffects[i].Duration -= 1;

			// remove completed status effects
			if (StatusEffects[i].Duration <= 0) {
				StatusEffects.RemoveAt(i);
			}
		}

		if (damage > 0) {
			this.Damage(damage);
		}
	}

	public bool IsStunned() {
		foreach (StatusEffect status in StatusEffects) {
			if (status.SkipsTurn) {
				return true;
			}
		}

		return false;
	}

	public bool isAnimating() {
		AnimatorStateInfo currentState = 
			anim.GetCurrentAnimatorStateInfo(AnimationLayerIndex);
		return !currentState.IsName("Idle") && 
				!currentState.IsName("Dead") && 
				!currentState.IsName("Victory");
	}

	protected void PlayAttackAnim() {
		int rng = Random.Range(0, 2);
		if (rng == 0)
			anim.SetTrigger("playAttack");
		else
			anim.SetTrigger("playAttack2");
	}

	protected void PlayMagicAnim() {
		anim.SetTrigger("playMagic");
	}

	protected void PlayItemAnim() {
		anim.SetTrigger("playItem");
	}
}
