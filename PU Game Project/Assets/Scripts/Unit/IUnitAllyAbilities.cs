using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.Kits
{
    public interface IUnitAllyAbilities
    {

        void UnitPassivePrep();

        void UnitBasicPrep();

        void UnitAttack1Prep();

        void UnitAttack2Prep();

        void UnitAttack3Prep();

        void UnitAbilityCleanup();
    }
}
