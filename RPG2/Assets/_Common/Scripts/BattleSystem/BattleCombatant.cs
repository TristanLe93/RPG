using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class BattleCombatant : MonoBehaviour {
	public string Name { get; protected set; }
	public Stat Health { get; protected set; }
	public int Strength { get; protected set; }
	public List<Ability> Abilities = new List<Ability>(4);
	public List<StatusEffect> StatusEffects = new List<StatusEffect>();

	public int AnimationLayerIndex = 0;
	public bool IsDead = false;

	public ObjectUI ObjectUI { get; private set; }
	protected Animator anim;

	
	public virtual void Start() {
		ObjectUI = GetComponent<ObjectUI>();
		anim = GetComponent<Animator>();
		anim.SetLayerWeight(AnimationLayerIndex, 1.0f);
	}

	public virtual void Update() {
	}

	public void Damage(int value) {
		Health.Current -= value;
		ObjectUI.SetHealthFillAmount(Health.GetRatio());
		ObjectUI.ShowDamageValue(value.ToString());
		anim.SetTrigger("playStruck");

		if (Health.IsCurrentZero()) {
			IsDead = true;
			StatusEffects.Clear();
			anim.SetBool("isDead", true);
		}
	}

	public void Heal(int value) {
		Health.Current += value;
		ObjectUI.SetHealthFillAmount(Health.GetRatio());
		ObjectUI.ShowHealValue(value.ToString());
		anim.SetTrigger("playHealing");
	}

	public void DoStatusEffects() {
		int damage = 0;

		for (int i = 0; i < StatusEffects.Count; i++) {
			if (StatusEffects[i].DamagePerTurn > 0) {
				damage += StatusEffects[i].DamagePerTurn;
			}

			StatusEffects[i].Duration -= 1;

			// remove completed status effects
			if (StatusEffects[i].Duration <= 0) {
				StatusEffects.RemoveAt(i);
				i--;
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
