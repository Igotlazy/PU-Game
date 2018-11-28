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

    public static int charIndex;

    public string abilityName;
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;

    protected List<List<GameObject>> targetCollectors = new List<List<GameObject>>();
    public List<List<Node>> abilityTargets = new List<List<Node>>();

    public List<Action<int>> castableAbilities = new List<Action<int>>();




    public void InitiateAbility(int abilityIndex)
    {
        collectorCoroutine = CollectTargets(abilityIndex);
        associatedCreature.StartCoroutine(collectorCoroutine);
    }

    private IEnumerator CollectTargets(int abilityIndex)
    {
        int collectorIndex = 0;

        while(collectorIndex < targetCollectors.Count)
        {
            GameObject spawnedCollector = GameObject.Instantiate(targetCollectors[abilityIndex][collectorIndex], associatedCreature.GetComponent<Unit>().centerPoint.position, Quaternion.identity);
            spawnedCollector.GetComponent<AttackSelection>().givenAbility = this;

            yield return new WaitUntil(() => spawnedCollector.GetComponent<AttackSelection>().hasSentTargets);

            collectorIndex++;
        }

        CastAbility(abilityIndex);
    }

    public void CancelTargets()
    {
        associatedCreature.StopCoroutine(collectorCoroutine);
        abilityTargets.Clear();
    }

    private void CastAbility(int abilityIndex)
    {
        if(abilityTargets.Count > 0)
        {
            charIndex += 1;
            castableAbilities[abilityIndex].Invoke(charIndex);
        }

    }
}
