using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CharAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public string abilityName;
    [TextArea]
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;


    [HideInInspector]
    public Unit associatedUnit;
    //[HideInInspector]
    public enum AbilityType
    {
        Passive,
        Movement,
        Activatable,
        Item
    }
    public int slotValue; //Which ability 1, 2 etc.. it is. 
    public AbilityType abilityType;
    

    public static int totalCastIndex;

    protected List<List<GameObject>> targetSelectors = new List<List<GameObject>>();
    protected List<List<SelectorPacket>> targetPacketBaseData = new List<List<SelectorPacket>>();
    public List<Action<EffectDataPacket>> castableAbilities = new List<Action<EffectDataPacket>>();

    public virtual void Initialize(Unit givenUnit)
    {
        this.associatedUnit = givenUnit;
    }

    public void InitiateAbility(int abilityIndex)
    {
        CancelTargets();
        collectorCoroutine = CollectTargets(abilityIndex);
        associatedUnit.StartCoroutine(collectorCoroutine);
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {
        List<SelectorPacket> targetPacketList = new List<SelectorPacket>();
        int selectorIndex = 0;

        while(selectorIndex < targetSelectors.Count)
        {
            SelectorPacket targets = SelectorPacket.Clone(targetPacketBaseData[abilityIndex][selectorIndex]);

            GameObject spawnedSelector = GameObject.Instantiate(targetSelectors[abilityIndex][selectorIndex], associatedUnit.transform.position, Quaternion.identity);
            AttackSelection selectorScript = spawnedSelector.GetComponentInChildren<AttackSelection>();


            selectorScript.givenAbility = this;
            selectorScript.attachedTargetPacket = targets;
            selectorScript.Initialize();

            yield return new WaitUntil(() => selectorScript.hasLoadedTargets);
            targetPacketList.Add(targets);

            selectorIndex++;
        }

        CastAbility(abilityIndex, targetPacketList);
    }
    IEnumerator collectorCoroutine;

    public void CancelTargets()
    {
        if(collectorCoroutine != null)
        {
            associatedUnit.StopCoroutine(collectorCoroutine);
        }
    }

    private void CastAbility(int abilityIndex, List<SelectorPacket> givenTargets)
    {
        totalCastIndex += 1;

        EffectDataPacket effectPacket = new EffectDataPacket(associatedUnit, this, abilityType, slotValue, totalCastIndex);
        foreach(SelectorPacket currentPacket in givenTargets)
        {
            effectPacket.AppendValue("Targets", currentPacket);
        }

        castableAbilities[abilityIndex].Invoke(effectPacket);
    }

    protected virtual void PayEnergyCost()
    {
        
    }


}
