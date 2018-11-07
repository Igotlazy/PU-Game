using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.Kits
{
    public interface IUnitAllyAbilities
    {

        void UnitPassive();

        void UnitBasic();

        void UnitAttack1();

        void UnitAttack2();

        void UnitAttack3();

        void UnitAbilityCleanup();
    }
}
