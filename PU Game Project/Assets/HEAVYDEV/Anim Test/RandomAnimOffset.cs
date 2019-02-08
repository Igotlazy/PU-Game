using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimOffset : MonoBehaviour {

    AnimatorStateInfo stateInfo;


    void Start ()
    {
        Animator[] animators = transform.GetComponentsInChildren<Animator>();
        foreach(Animator currentanim in animators)
        {
            stateInfo = currentanim.GetCurrentAnimatorStateInfo(0);//could replace 0 by any other animation layer index
            currentanim.Play(stateInfo.fullPathHash, -1, Random.Range(0f, 1f));
        }
    }
}
