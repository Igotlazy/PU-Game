using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class CharAbility{

    public LivingCreature associatedCreature;
    IEnumerator collectorCoroutine;
    

    public CharAbility(LivingCreature _associatedCreature)
    {
        associatedCreature = _associatedCreature;
    }

    public static int totalCastIndex;

    public string abilityName;
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;

    protected List<List<GameObject>> targetSelectors = new List<List<GameObject>>();
    protected List<List<TargetPacket>> targetPacketBaseData = new List<List<TargetPacket>>();

    public List<Action<EffectDataPacket>> castableAbilities = new List<Action<EffectDataPacket>>();




    public void InitiateAbility(int abilityIndex)
    {
        CancelTargets();
        collectorCoroutine = CollectTargets(abilityIndex);
        associatedCreature.StartCoroutine(collectorCoroutine);
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {
        List<TargetPacket> targetPacketList = new List<TargetPacket>();
        int selectorIndex = 0;

        while(selectorIndex < targetSelectors.Count)
        {
            TargetPacket targets = TargetPacket.Clone(targetPacketBaseData[abilityIndex][selectorIndex]);

            GameObject spawnedSelector = GameObject.Instantiate(targetSelectors[abilityIndex][selectorIndex], associatedCreature.transform.position, Quaternion.identity);
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

    public void CancelTargets()
    {
        if(collectorCoroutine != null)
        {
            associatedCreature.StopCoroutine(collectorCoroutine);
        }
    }

    private void CastAbility(int abilityIndex, List<TargetPacket> givenTargets)
    {
        totalCastIndex += 1;

        EffectDataPacket effectPacket = new EffectDataPacket(associatedCreature, this, totalCastIndex);
        foreach(TargetPacket currentPacket in givenTargets)
        {
            effectPacket.AppendValue("Targets", currentPacket);
        }

        castableAbilities[abilityIndex].Invoke(effectPacket);
    }


}
