using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MHA.UserInterface
{
    public class DamageIndicator : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI textMeshComp;

        // Use this for initialization
        void Start()
        {
            this.transform.forward = Camera.main.transform.forward;
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            Destroy(this.gameObject, clipInfo[0].clip.length/1.35f); //Magic number, slightly bigger than animation play speed multiplier.
        }

        public void SetText(float givenValue)
        {
            string givenValueString = givenValue.ToString();

            if(givenValue == 0)
            {
                textMeshComp.text = givenValueString;
            }
            else
            {
                textMeshComp.text = "-" + givenValueString;
            }
        }
    }
}

