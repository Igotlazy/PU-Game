using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDataPacket {

    public Dictionary<string, List<object>> blackboard = new Dictionary<string, List<object>>();


    public EffectDataPacket(LivingCreature _caster, CharAbility _charAbility, int _castIndex)
    {
        SetValueAtKey("Caster", _caster);
        SetValueAtKey("CharacterAbility", _charAbility);
        SetValueAtKey("CastIndex", _castIndex);
    }
    public EffectDataPacket(LivingCreature _caster, CharAbility _charAbility, int _castIndex, TargetPacket _targets)
    {
        SetValueAtKey("Caster", _caster);
        SetValueAtKey("CharacterAbility", _charAbility);
        SetValueAtKey("CastIndex", _castIndex);
        SetValueAtKey("Targets", _targets);
    }


    public List<object> GetValueAtKey(string getKey, bool zero)
    {
        if (blackboard.ContainsKey(getKey))
        {
            return blackboard[getKey];
        }
        else
        {
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

    public void SetValueAtKey(string setKey, object setObject)
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
