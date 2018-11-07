using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.DebugGame;

namespace MHA.Kits
{
    public class Takahiro : HeroCharacter
    {

        public GameObject attack1Effect;
        public GameObject attack2Effect;
        public GameObject attack3Effect;
        GameObject storedAttack1;
        GameObject storedAttack2;
        GameObject storedAttack3;

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




        protected override void UnitPassiveImpl()
        {

        }

        protected override void UnitBasicImpl()
        {
            CombatUtils.BasicAttackSelect(this.gameObject, this.range.Value);
        }

        protected override void UnitAttack1Impl()
        {
            storedAttack1 = Instantiate(attack1Effect, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
        }

        protected override void UnitAttack2Impl()
        {
            storedAttack2 = Instantiate(attack2Effect, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
        }

        protected override void UnitAttack3Impl()
        {
            CombatUtils.BasicAttackSelect(this.gameObject, 3.75f);
            storedAttack3 = Instantiate(attack3Effect, new Vector3(this.gameObject.transform.position.x, 0.5f, this.gameObject.transform.position.z), Quaternion.identity);
        }

        protected override void UnitAbilityCleanupImpl()
        {
            if (a1Active)
            {
                storedAttack1.transform.GetChild(0).GetComponent<SphereAttack>().Cleanup(); //Fucking using GetChild, ewwww. If the position of the Sphere changes in that hierarchy that breaks.
                Destroy(storedAttack1);
                a1Active = false;
            }
            if(a2Active)
            {
                Destroy(storedAttack2);
            }
            if (a3Active)
            {
                Destroy(storedAttack3);
            }
        }
    }
}

