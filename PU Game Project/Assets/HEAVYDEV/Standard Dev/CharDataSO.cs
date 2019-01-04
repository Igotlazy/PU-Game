using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharStats" , menuName = "Character Stats")]
public class CharDataSO : ScriptableObject
{
    public string heroName;
    public string realName;

    public GameObject spriteRig;

    public List<CharAbility> characterAbilities = new List<CharAbility>();

}
