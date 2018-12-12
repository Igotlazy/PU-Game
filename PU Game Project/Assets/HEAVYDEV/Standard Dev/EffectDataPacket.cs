using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDataPacket {

    public Dictionary<string, List<object>> blackboard = new Dictionary<string, List<object>>();


    public EffectDataPacket(LivingCreature _caster, CharAbility _charAbility, int _castIndex)
    {
        AppendValueAtKey("Caster", _caster);
        AppendValueAtKey("CharacterAbility", _charAbility);
        AppendValueAtKey("CastIndex", _castIndex);
    }
    public EffectDataPacket(LivingCreature _caster, CharAbility _charAbility, int _castIndex, TargetPacket _targets)
    {
        AppendValueAtKey("Caster", _caster);
        AppendValueAtKey("CharacterAbility", _charAbility);
        AppendValueAtKey("CastIndex", _castIndex);
        AppendValueAtKey("Targets", _targets);
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

    public void AppendValueAtKey(string setKey, object setObject)
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
