using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDataPacket {

    public Dictionary<string, List<object>> blackboard = new Dictionary<string, List<object>>();


    public EffectDataPacket(Unit _caster, CharAbility _charAbility, CharAbility.AbilityType _abilityType, int _slotValue, int _castIndex)
    {
        AppendValue("Caster", _caster);
        AppendValue("CharacterAbility", _charAbility);
        AppendValue("AbilityType", _abilityType);
        AppendValue("SlotValue", _slotValue);
        AppendValue("CastIndex", _castIndex);
        
    }

    public List<object> GetValue(string getKey, bool zero)
    {
        if (blackboard.ContainsKey(getKey))
        {
            return blackboard[getKey];
        }
        else
        {
            Debug.LogWarning("WARNING: KEY TO ON EDP DOES NOT EXIST");
            if (zero)
            {
                blackboard.Add(getKey, new List<object> { 0 });
            }
            else
            {
                blackboard.Add(getKey, new List<object> { null });
            }
            return blackboard[getKey];
        }
    }

    public void AppendValue(string setKey, object setObject)
    {
        if (blackboard.ContainsKey(setKey))
        {
            blackboard[setKey].Add(setObject);
        }
        else
        {
            blackboard.Add(setKey, new List<object>());
            blackboard[setKey].Add(setObject);
        }
    }
}
