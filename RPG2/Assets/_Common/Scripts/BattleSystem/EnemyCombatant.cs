using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class EnemyCombatant : BattleCombatant {
	
	public override void Start () {
		base.Start();
	}

	public override void Update () {
		base.Start();
	}

	public virtual IEnumerator BattleAI(BattleUIController ui, List<BattleCombatant> targetList) {
		List<BattleCombatant> aliveTargets = targetList.Where(t => t.Health.Current > 0).ToList();
		BattleCombatant target = aliveTargets[Random.Range(0, aliveTargets.Count)];
		Ability ability = Abilities[0];

		ui.ShowAbilityName(ability.Name);
		yield return new WaitForSeconds(1.5f);
		yield return StartCoroutine(UseAbility(ui, ability, target));
	}

	protected virtual IEnumerator UseAbility(BattleUIController ui, Ability ability, BattleCombatant target) {
		PlayAnimation(ability.Type);

		do {
			yield return null;
		} while (isAnimating());
		
		ExecuteAbility(ability, target);
		ui.UpdateUI(target.Name, target.Abilities);
		ui.UpdateHealthBar(target.Health);
		ui.UpdateStatusEffectsIcons(target.StatusEffects);

		do {
			yield return null;
		} while (target.isAnimating());
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
}
