using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MHA.BattleBehaviours
{
    public class BBDealDamageAnim : BattleAnimation
    {
        LivingCreature givenCreature;
        float healthRemaining;
        float maxHealth;

        public BBDealDamageAnim(LivingCreature _givenCreature, float _healthRemaining, float _maxHealth)
        {
            givenCreature = _givenCreature;
            healthRemaining = _healthRemaining;
            maxHealth = _maxHealth;

            LoadBattleAnimation();
        }

        protected override void PlayBattleAnimationImpl()
        {
            mono.StartCoroutine(DisplayDamage());
        }

        private IEnumerator DisplayDamage()
        {
            givenCreature.healthBar.UpdateHealth(healthRemaining, maxHealth);
            yield return new WaitForSeconds(0.3f);

            AnimFinished = true;
        }

    }
}
