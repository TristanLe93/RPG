using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class EnemyCombatant : BattleCombatant {
	protected BattleCombatant target;
	protected Ability ability;


	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Start();
	}

	public virtual IEnumerator BattleAI(BattleUIController ui, List<BattleCombatant> targetList) {
		List<BattleCombatant> aliveTargets = targetList.Where(t => t.Health.Current > 0).ToList();
		target = aliveTargets[Random.Range(0, aliveTargets.Count)];
		ability = Abilities[0];

		yield return StartCoroutine(ShowAbility(ui));
		yield return StartCoroutine(UseAbility(ability, target));

		// update target information
		ui.UpdateUI(target.name, target.Abilities);
		ui.UpdateHealthBar(target.Health);
		ui.UpdateStatusEffectsIcons(target.StatusEffects);
	}

	protected IEnumerator ShowAbility(BattleUIController ui) {
		ui.ShowAbilityName(ability.Name);
		yield return new WaitForSeconds(1.5f);
	}

	protected virtual IEnumerator UseAbility(Ability ability, BattleCombatant target) {
		PlayAnimation(ability.Type);

		do {
			yield return null;
		} while (isAnimating());
		
		ExecuteAbility(ability, target);
	}

	protected void PlayAnimation(AbilityType type) {
		switch (type) {
		case AbilityType.Melee: PlayAttackAnim(); break;
		case AbilityType.Magic: PlayMagicAnim(); break;
		case AbilityType.Heal: PlayItemAnim(); break;
		case AbilityType.Ranged: PlayAttackAnim(); break;
		}
	}
	
	protected void ExecuteAbility(Ability ability, BattleCombatant target) {
		int dmg = 0;
		
		switch (ability.Type) {
		case AbilityType.Melee: 
			dmg = ability.Power + Strength;
			target.Damage(dmg);
			break;
			
		case AbilityType.Magic: 
			dmg = ability.Power;
			target.Damage(dmg);
			break;
			
		case AbilityType.Heal: 
			dmg = ability.Power;
			target.Heal(dmg);
			break;
			
		case AbilityType.Ranged: 
			dmg = ability.Power + Strength;
			target.Damage(dmg);
			break;
		}
	}

	public IEnumerator WaitForTargetAnimation() {
		do {
			yield return null;
		} while (target.isAnimating());
	}
}
