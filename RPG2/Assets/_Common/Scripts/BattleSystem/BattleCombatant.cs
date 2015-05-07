using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class BattleCombatant : MonoBehaviour {
	public string Name { get; protected set; }
	public Stat Health { get; protected set; }
	public int Strength { get; protected set; }
	public List<Ability> Abilities = new List<Ability>(4);

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
			anim.SetBool("isDead", true);
		}
	}

	public void Heal(int value) {
		Health.Current += value;
		ObjectUI.SetHealthFillAmount(Health.GetRatio());
		ObjectUI.ShowHealValue(value.ToString());
		anim.SetTrigger("playHealing");
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
