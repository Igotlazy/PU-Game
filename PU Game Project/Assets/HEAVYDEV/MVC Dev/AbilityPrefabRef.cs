using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPrefabRef : MonoBehaviour {

    public static AbilityPrefabRef instance;

    private Dictionary<string, GameObject> abilityDictionary = new Dictionary<string, GameObject>();
    private string abilityPathBasic = "UnitAbilityPrefabs/";
    private Dictionary<string, GameObject> nodeSelectorDictionary = new Dictionary<string, GameObject>();
    private string nodeSelectorPathBasic = "NodeCollectorPrefabs/";


    void Awake ()
    {
        if (instance == null) { instance = this; } else { Destroy(this); }
    }


    public GameObject GiveAbilityPrefab(string keyString)
    {
        return retrieveFromDictionary(keyString, abilityPathBasic, abilityDictionary);
    }


    public GameObject GiveNodeSelectorPrefab(SelectorData givenSelector)
    {
        return retrieveFromDictionary(givenSelector.SelectorName, nodeSelectorPathBasic, nodeSelectorDictionary);
    }


    private GameObject retrieveFromDictionary(string keyString, string mainPath, Dictionary<string, GameObject> givenDic)
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

    public abstract class SelectorData
    {
        public string SelectorName { protected set; get; }
        [HideInInspector]
        public float range;
    }
    public class BasicMoveSelectorData : SelectorData
    {
        /*
        public enum SelectorType
        {
            Locked,

        }
        */
        
        public BasicMoveSelectorData()
        {
            SelectorName = BasicMoveSelector;
        }
    }
    [System.Serializable]
    public class CircleSelectorData : SelectorData
    {
        public float radius;

        public CircleSelectorData()
        {
            SelectorName = CircleSelector;
            range = radius;
        }
    }
    public static readonly string BasicMoveSelector = "Basic Move Selector";
    public static readonly string CircleSelector = "Circle Selector";
    public static readonly string LineAttackSelector = "Line Attack Selector";

    public static readonly string TakahiroBasic = "Takahiro/Projectile";
    public static readonly string TakahiroA1 = "Takahiro/A1";
    public static readonly string TakahiroA3 = "Takahiro/A3";
}
