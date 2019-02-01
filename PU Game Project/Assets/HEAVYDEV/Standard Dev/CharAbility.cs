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
    public int currentCooldown;

    /*
    public enum AbilityType
    {
        Passive,
        Movement,
        Activatable,
        Item
    }
    public AbilityType abilityType;
    */


    //References:
    [HideInInspector]
    public Unit associatedUnit;
    public static int totalCastIndex;

    public List<List<SelectorPacket>> selectorPacketBaseData = new List<List<SelectorPacket>>();
    public List<Action<EffectDataPacket>> castableAbilities = new List<Action<EffectDataPacket>>();
    public AttackSelection currentActiveSelector;

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

        while(selectorIndex < selectorPacketBaseData.Count)
        {
            SelectorPacket targets = SelectorPacket.Clone(selectorPacketBaseData[abilityIndex][selectorIndex]);
            GameObject selectorRef = AbilityPrefabRef.instance.GiveNodeSelectorPrefab(targets.selectorData);

            GameObject spawnedSelector = GameObject.Instantiate(selectorRef, associatedUnit.transform.position, Quaternion.identity);
            currentActiveSelector = spawnedSelector.GetComponentInChildren<AttackSelection>();


            currentActiveSelector.givenAbility = this;
            currentActiveSelector.attachedTargetPacket = targets;
            currentActiveSelector.Initialize();

            yield return new WaitUntil(() => currentActiveSelector.hasLoadedTargets);
            targetPacketList.Add(targets);

            selectorIndex++;
        }

        currentActiveSelector = null;
        CastAbility(abilityIndex, targetPacketList);
    }
    IEnumerator collectorCoroutine;

    public void CancelTargets()
    {
        if(collectorCoroutine != null)
        {
            associatedUnit.StopCoroutine(collectorCoroutine);
            currentActiveSelector = null;
        }
    }

    private void CastAbility(int abilityIndex, List<SelectorPacket> givenTargets)
    {
        totalCastIndex += 1;

        EffectDataPacket effectPacket = new EffectDataPacket(associatedUnit, this);
        foreach(SelectorPacket currentPacket in givenTargets)
        {
            effectPacket.AppendValue("Targets", currentPacket);
        }

        PayEnergyCost(effectPacket);
        SetCooldown(effectPacket);

        castableAbilities[abilityIndex].Invoke(effectPacket);
    }

    protected virtual void PayEnergyCost(EffectDataPacket givenPacket)
    {
        if(associatedUnit != null && associatedUnit.CreatureScript != null)
        {
            associatedUnit.CreatureScript.CurrentEnergy -= energyCost;
        }
    }

    protected virtual void SetCooldown(EffectDataPacket givenPacket)
    {
        currentCooldown = turnCooldown;
    }

    public virtual void CooldownDecrease()
    {
        if(currentCooldown > 0)
        {
            currentCooldown -= 1;
        }
    }


}
