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
        float damage;

        public BBDealDamageAnim(LivingCreature _givenCreature, float _healthRemaining, float _damage)
        {
            givenCreature = _givenCreature;
            healthRemaining = _healthRemaining;
            damage = _damage;
        }

        protected override void PlayBattleAnimationImpl()
        {
            //mono.StartCoroutine(DisplayDamage());
            
            givenCreature.healthBar.UpdateHealth(healthRemaining, givenCreature.maxHealth.Value);

            GameObject indicatorObj = GameObject.Instantiate(givenCreature.damageIndicator, new Vector3(givenCreature.transform.position.x, givenCreature.transform.position.y + 2f, givenCreature.transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(damage);

            AnimFinished = true;
            
        }

        private IEnumerator DisplayDamage()
        {

            givenCreature.healthBar.UpdateHealth(healthRemaining, givenCreature.maxHealth.Value);

            GameObject indicatorObj = GameObject.Instantiate(givenCreature.damageIndicator, new Vector3 (givenCreature.transform.position.x, givenCreature.transform.position.y + 2f, givenCreature.transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(damage);

            yield return new WaitForSeconds(2f);

            AnimFinished = true;
        }

    }
}
