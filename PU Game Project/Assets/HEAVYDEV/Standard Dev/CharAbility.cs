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

    public List<Action<EffectDataPacket>> castableAbilities = new List<Action<EffectDataPacket>>();




    public void InitiateAbility(int abilityIndex)
    {
        CancelTargets();
        collectorCoroutine = CollectTargets(abilityIndex);
        associatedCreature.StartCoroutine(collectorCoroutine);
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {
        TargetPacket targets = new TargetPacket();
        int collectorIndex = 0;

        while(collectorIndex < targetSelectors.Count)
        {
            GameObject spawnedSelector = GameObject.Instantiate(targetSelectors[abilityIndex][collectorIndex], associatedCreature.GetComponent<Unit>().centerPoint.position, Quaternion.identity);
            AttackSelection selectorScript = spawnedSelector.GetComponent<AttackSelection>();
            selectorScript.givenAbility = this;
            selectorScript.attachedTargetPacket = targets;

            yield return new WaitUntil(() => selectorScript.hasLoadedTargets);

            collectorIndex++;
        }

        CastAbility(abilityIndex, targets);
    }

    public void CancelTargets()
    {
        if(collectorCoroutine != null)
        {
            associatedCreature.StopCoroutine(collectorCoroutine);
        }
    }

    private void CastAbility(int abilityIndex, TargetPacket givenTargets)
    {
        totalCastIndex += 1;

        EffectDataPacket effectPacket = new EffectDataPacket(associatedCreature, this, totalCastIndex, givenTargets);

        castableAbilities[abilityIndex].Invoke(effectPacket);
    }


}
