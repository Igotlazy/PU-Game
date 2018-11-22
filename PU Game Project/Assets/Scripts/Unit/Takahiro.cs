using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

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

        public GameObject basicParticle;
        TakahiroBasicModel basicModel;

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

        public override void InitializeAbilities()
        {
            basicModel = new TakahiroBasicModel(this);
            CharBasic = AbilityFactory.instance.CreateAbilityController(basicModel.takaHiroBasicModel);

            CharA1 = AbilityFactory.instance.CreateAbilityController((new TakahiroA1Model(this).takaHiroA1Model));
            CharA2 = AbilityFactory.instance.CreateAbilityController((new TakahiroA1Model(this).takaHiroA1Model));
            CharA3 = AbilityFactory.instance.CreateAbilityController((new TakahiroA1Model(this).takaHiroA1Model));
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


        protected override BattleEvent CharBasicInit(List<Node> relevantNodes)
        {
            Debug.Log(basicModel.takaHiroBasicModel.Equals(CharBasic.charModel));
            basicModel.dynamicTarget = relevantNodes[0].occupant.transform.position;
            Debug.Log("Name of Node: "+ relevantNodes[0].occupant.name);
            basicModel.objectToHit = relevantNodes[0].occupant;
            CharBasic.AbilityCast(0);
            return null;
        }

        protected override BattleEvent CharAbility1Init(List<Node> relevantNodes)
        {
            return null;
        }

        protected override BattleEvent CharAbility2Init(List<Node> relevantNodes)
        {
            throw new System.NotImplementedException();
        }

        protected override BattleEvent CharAbility3Init(List<Node> relevantNodes)
        {
            return null;
        }
    }
}

