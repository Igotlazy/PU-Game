using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.UserInterface;


namespace MHA.BattleBehaviours
{
    public class BBDealDamageAnim : BattleAnimation
    {
        LivingCreature givenCreature;
        float healthRemaining;
        Attack givenAttack;

        public BBDealDamageAnim(LivingCreature _givenCreature, float _healthRemaining, Attack _givenAttack)
        {
            givenCreature = _givenCreature;
            healthRemaining = _healthRemaining;
           givenAttack = _givenAttack;

            LoadBattleAnimation();
        }

        protected override void PlayBattleAnimationImpl()
        {
            mono.StartCoroutine(DisplayDamage());
        }

        private IEnumerator DisplayDamage()
        {

            givenCreature.healthBar.UpdateHealth(healthRemaining, givenCreature.maxHealth.Value);

            GameObject indicatorObj = GameObject.Instantiate(givenCreature.damageIndicator, new Vector3 (givenCreature.transform.position.x, givenCreature.transform.position.y + 2f, givenCreature.transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(givenAttack.damageValue);

            yield return new WaitForSeconds(0.2f);

            AnimFinished = true;
        }

    }
}
