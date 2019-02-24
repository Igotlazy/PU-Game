using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityPrefabRef
{

    private static Dictionary<string, GameObject> abilityDictionary = new Dictionary<string, GameObject>();
    private static string abilityPathBasic = "UnitAbilityPrefabs/";
    private static Dictionary<string, GameObject> nodeSelectorDictionary = new Dictionary<string, GameObject>();
    private static string nodeSelectorPathBasic = "NodeCollectorPrefabs/";


    public static GameObject GiveAbilityPrefab(string keyString)
    {
        return retrieveFromDictionary(keyString, abilityPathBasic, abilityDictionary);
    }


    public static GameObject GiveSelectorPrefab(SelectorData givenSelector)
    {
        return retrieveFromDictionary(givenSelector.SelectorName, nodeSelectorPathBasic, nodeSelectorDictionary);
    }


    private static GameObject retrieveFromDictionary(string keyString, string mainPath, Dictionary<string, GameObject> givenDic)
    {
        if (givenDic.ContainsKey(keyString))
        {
            return givenDic[keyString];
        }
        else
        {
            GameObject resourcePrefab = (GameObject) Resources.Load(mainPath + keyString);
            if(resourcePrefab == null)
            {
                Debug.LogAssertion("Improper string given for Resource Prefab: " + keyString);
            }

            givenDic.Add(keyString, resourcePrefab);
            return givenDic[keyString];
        }
    }


    //---------------------------------------------------------------------------------------------------------------------------------

    //String paths to Resources.

    public static readonly string BasicMoveSelector = "Basic Move Selector";
    public static readonly string CircleSelector = "Circle Selector";
    public static readonly string BoxSelector = "Box Selector";

    public static readonly string TakahiroBasic = "Takahiro/Projectile";
    public static readonly string TakahiroA1 = "Takahiro/A1";
    public static readonly string TakahiroA3 = "Takahiro/A3";

    public static readonly string SandyBasic1 = "Sandy/Basic1";
    public static readonly string SandyBasic2 = "Sandy/Basic2";
}
