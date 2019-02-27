using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MHA.UserInterface
{
    public class HealthBarControl : MonoBehaviour
    {
        [Header("Health Related:")]
        public Image currentHealthImage;
        public Image backHealthImage;
        public TextMeshProUGUI currentHealthValue;
        [Space]

        [Header("Hit Chance Related:")]
        public GameObject hitChanceObject;
        public TextMeshProUGUI hitChanceText;

        private Vector3 hitChanceScale;

        private Transform cameraTrans;

        void Start()
        {
            cameraTrans = Camera.main.transform;
            hitChanceScale = hitChanceObject.transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            transform.forward = cameraTrans.forward;
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float healthRatio = currentHealth / maxHealth;
            currentHealthImage.fillAmount = healthRatio;
            currentHealthValue.text = currentHealth.ToString();
        }

        public void SetHitChance(float hitValue)
        {
            hitValue = Mathf.Clamp(hitValue, 0f, 100f);
            hitChanceText.text = hitValue + "%";
        }

        public void DisplayHitChance()
        {
            Image imageComp = hitChanceObject.GetComponent<Image>();
            hitChanceObject.transform.localScale = new Vector3(hitChanceScale.x * 1.1f, hitChanceScale.y * 1.1f, hitChanceScale.z);
            Color changeColor = imageComp.color;
            changeColor.a = 1f;
            imageComp.color = changeColor;
            hitChanceObject.SetActive(true);
        }

        public void FadeHitChance()
        {
            Image imageComp = hitChanceObject.GetComponent<Image>();
            hitChanceObject.transform.localScale = hitChanceScale;
            Color changeColor = imageComp.color;
            changeColor.a = 0.2f;
            imageComp.color = changeColor;
            hitChanceObject.SetActive(true);
        }

        public void HideHitChance()
        {
            hitChanceObject.SetActive(false);
            hitChanceObject.transform.localScale = hitChanceScale;
        }
    }

}
