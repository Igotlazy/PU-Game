using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MHA.Events;

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
    public static int totalCastIndex;

    public List<List<SelectorData>> selectorData = new List<List<SelectorData>>();
    public List<Action<EffectDataPacket>> castableAbilities = new List<Action<EffectDataPacket>>();

    [HideInInspector] public GameEntity associatedEntity;
    [HideInInspector] public AbilitySelection currentActiveSelector;
    [HideInInspector] public Vector3 selectorSpawnLoc;

    public virtual void Initialize(GameEntity givenUnit)
    {
        this.associatedEntity = givenUnit;
        EventFlags.ANIMStartCast += AnimResponse;
    }

    public void InitiateAbility(int abilityIndex)
    {      
        CancelTargets();
        collectorCoroutine = CollectTargets(abilityIndex);
        associatedEntity.StartCoroutine(collectorCoroutine);
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {

        List<SelectorPacket> targetPacketList = new List<SelectorPacket>();
        int selectorIndex = 0;
        selectorSpawnLoc = associatedEntity.transform.position;

        while (selectorIndex < selectorData[abilityIndex].Count)
        {
            SelectorData data = selectorData[abilityIndex][selectorIndex].Clone();
            SelectorPacket targets = new SelectorPacket();
            targets.selectorData = data;

            GameObject selectorRef = AbilityPrefabRef.GiveSelectorPrefab(data);
            GameObject spawnedSelector = GameObject.Instantiate(selectorRef, selectorSpawnLoc, Quaternion.identity);

            currentActiveSelector = spawnedSelector.GetComponentInChildren<AbilitySelection>();
            currentActiveSelector.givenAbility = this;
            currentActiveSelector.selPacket = targets;
            currentActiveSelector.Initialize();

            yield return new WaitUntil(() => currentActiveSelector.hasLoadedTargets);

            targetPacketList.Add(targets);

            selectorIndex++;
            if(selectorIndex < selectorData[abilityIndex].Count)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }

        currentActiveSelector = null;

        CastAbility(abilityIndex, targetPacketList);
    }
    IEnumerator collectorCoroutine;

    public void CancelTargets()
    {
        if(collectorCoroutine != null)
        {
            associatedEntity.StopCoroutine(collectorCoroutine);
            currentActiveSelector = null;
        }
    }

    private void CastAbility(int abilityIndex, List<SelectorPacket> givenTargets)
    {
        totalCastIndex++;

        EffectDataPacket effectPacket = new EffectDataPacket();
        effectPacket.AppendStaticValue("CharacterAbility", this);
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
        if(associatedEntity != null && associatedEntity.entityType == GameEntity.EntityType.Unit)
        {
            Unit unit = (Unit)associatedEntity;
            unit.CurrentEnergy -= energyCost;
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

    public virtual void AnimResponse(object givenObject, EventFlags.ECastAnim givenCastAnim)
    {

    }


}
