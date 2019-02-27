using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.UserInterface;


namespace MHA.BattleAnimations
{
    public class BAnimDealDamage : BattleAnimation
    {
        Unit givenUnit;
        float healthRemaining;
        float damage;

        public BAnimDealDamage(object _source, Unit _givenCreature, float _healthRemaining, float _damage) : base (_source)
        {
            givenUnit = _givenCreature;
            healthRemaining = _healthRemaining;
            damage = _damage;
        }

        protected override void PlayBattleAnimationImpl()
        {
            //mono.StartCoroutine(DisplayDamage());
            
            givenUnit.healthBar.UpdateHealth(healthRemaining, givenUnit.maxHealth.Value);

            GameObject indicatorObj = GameObject.Instantiate(givenUnit.damageIndicator, new Vector3(givenUnit.transform.position.x, givenUnit.transform.position.y + 2f, givenUnit.transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(damage);

            AnimFinished = true;
            
        }

        private IEnumerator DisplayDamage()
        {

            givenUnit.healthBar.UpdateHealth(healthRemaining, givenUnit.maxHealth.Value);

            GameObject indicatorObj = GameObject.Instantiate(givenUnit.damageIndicator, new Vector3 (givenUnit.transform.position.x, givenUnit.transform.position.y + 2f, givenUnit.transform.position.z), Quaternion.identity);
            indicatorObj.GetComponent<DamageIndicator>().SetText(damage);

            yield return new WaitForSeconds(2f);

            AnimFinished = true;
        }

    }
}
