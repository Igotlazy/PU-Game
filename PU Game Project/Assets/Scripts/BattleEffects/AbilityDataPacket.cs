using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDataPacket {

    public Dictionary<string, object> staticBlackboard = new Dictionary<string, object>();
    public Dictionary<string, List<object>> variableBlackboard = new Dictionary<string, List<object>>();


    public object GetStaticValue(string getKey, bool zero)
    {
        if (staticBlackboard.ContainsKey(getKey))
        {
            return staticBlackboard[getKey];
        }
        else
        {
            Debug.LogError("WARNING: KEY ON STATIC DATA PACKET DOES NOT EXIST");
            if (zero)
            {
                staticBlackboard.Add(getKey, 0);
            }
            else
            {
                staticBlackboard.Add(getKey, null);
            }
            return staticBlackboard[getKey];
        }
    }

    public void AppendStaticValue(string setString, object setObject)
    {
        staticBlackboard.Add(setString, setObject);
    }

    public List<object> GetVarValue(string getKey, bool zero)
    {
        if (variableBlackboard.ContainsKey(getKey))
        {
            return variableBlackboard[getKey];
        }
        else
        {
            Debug.LogError("WARNING: KEY ON VARIABLE DATA PACKET DOES NOT EXIST");
            if (zero)
            {
                variableBlackboard.Add(getKey, new List<object> { 0 });
            }
            else
            {
                variableBlackboard.Add(getKey, new List<object> { null });
            }
            return variableBlackboard[getKey];
        }
    }

    public void AppendValue(string setKey, object setObject)
    {
        if (variableBlackboard.ContainsKey(setKey))
        {
            variableBlackboard[setKey].Add(setObject);
        }
        else
        {
            variableBlackboard.Add(setKey, new List<object>());
            variableBlackboard[setKey].Add(setObject);
        }
    }
}
