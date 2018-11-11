using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.Kits.TakahiroAbilities;

namespace MHA.Kits
{
    public class Takahiro : HeroCharacter
    {
        [Header("Ability Indicators")]
        public GameObject basicIndicator;
        public GameObject attack1Indicator;
        public GameObject attack2Indicator;
        public GameObject attack3Indicator;
        GameObject storedBasicIndicator;
        GameObject storedAttack1Indicator;
        GameObject storedAttack2Indicator;
        GameObject storedAttack3Indicator;
        [Space]

        [Header("Ability Objects")]
        public GameObject basicEffect;
        public GameObject attack1Effect;
        public GameObject attack2Effect;
        public GameObject attack3Effect;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }




        protected override void UnitPassivePrepImpl()
        {
            throw new System.NotImplementedException();
        }

        protected override void UnitBasicPrepImpl()
        {
            storedBasicIndicator = Instantiate(basicIndicator, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
            storedBasicIndicator.transform.GetChild(0).GetComponent<AttackSelection>().Initialize(this);
        }

        protected override void UnitAttack1PrepImpl()
        {
            storedAttack1Indicator = Instantiate(attack1Indicator, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
            storedAttack1Indicator.transform.GetChild(0).GetComponent<AttackSelection>().Initialize(this);
        }

        protected override void UnitAttack2PrepImpl()
        {
            storedAttack2Indicator = Instantiate(attack2Indicator, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
        }

        protected override void UnitAttack3PrepImpl()
        {
            storedAttack3Indicator = Instantiate(attack3Indicator, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
            storedAttack3Indicator.transform.GetChild(0).GetComponent<AttackSelection>().Initialize(this);
        }

        protected override void UnitAbilityCleanupImpl()
        {
            if (basicActive)
            {
                storedBasicIndicator.transform.GetChild(0).GetComponent<AttackSelection>().NodeDisplayCleanup();
                Destroy(storedBasicIndicator);
                basicActive = false;
            }
            if (a1Active)
            {
                storedAttack1Indicator.transform.GetChild(0).GetComponent<AttackSelection>().NodeDisplayCleanup();
                Destroy(storedAttack1Indicator);
                a1Active = false;
            }
            if(a2Active)
            {
                storedAttack2Indicator.transform.GetChild(0).GetComponent<AttackSelection>().NodeDisplayCleanup();
                Destroy(storedAttack2Indicator);
                a2Active = false;
            }
            if (a3Active)
            {
                storedAttack3Indicator.transform.GetChild(0).GetComponent<AttackSelection>().NodeDisplayCleanup();
                Destroy(storedAttack3Indicator);
                a3Active = false;
            }
        }


        protected override BattleEvent UnitBasicInit(List<Node> relevantNodes)
        {
            Attack basicAttack = new Attack(5f, Attack.DamageType.Physical, this.gameObject);
            BETakahiroBasic basicBattleEvent = new BETakahiroBasic(basicAttack, basicEffect, this.gameObject, relevantNodes);
            return basicBattleEvent;
        }

        protected override BattleEvent UnitAttack1Init(List<Node> relevantNodes)
        {
            Attack a1Attack = new Attack(5f, Attack.DamageType.Physical, this.gameObject);
            BETakahiroBasic a3BattleEvent = new BETakahiroBasic(a1Attack, attack1Effect, this.gameObject, relevantNodes);
            return a3BattleEvent;
        }

        protected override BattleEvent UnitAttack2Init(List<Node> relevantNodes)
        {
            throw new System.NotImplementedException();
        }

        protected override BattleEvent UnitAttack3Init(List<Node> relevantNodes)
        {
            Attack a3Attack = new Attack(10f, Attack.DamageType.Physical, this.gameObject);
            BETakahiroBasic a3BattleEvent = new BETakahiroBasic(a3Attack, attack3Effect, this.gameObject, relevantNodes);
            return a3BattleEvent;
        }
    }
}

