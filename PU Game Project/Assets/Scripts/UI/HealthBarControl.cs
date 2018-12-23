using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MHA.UserInterface
{
    public class HealthBarControl : MonoBehaviour
    {
        public LivingCreature associatedCreature;
        [Space]

        [Header("Health Related:")]
        public Image currentHealthImage;
        public Image backHealthImage;
        public TextMeshProUGUI currentHealthValue;
        [Space]

        [Header("Hit Chance Related:")]
        public GameObject hitChanceObject;
        public TextMeshProUGUI hitChanceText;

        void Start()
        {
            if(associatedCreature != null)
            {
                UpdateHealth(associatedCreature.currentHealth, associatedCreature.maxHealth.Value);
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float healthRatio = currentHealth / maxHealth;
            currentHealthImage.fillAmount = healthRatio;
            currentHealthValue.text = currentHealth.ToString();
        }

        public void DisplayHitChance(float hitValue)
        {
            hitValue = Mathf.Clamp(hitValue, 0f, 100f);
            hitChanceText.text = hitValue + "%";
            hitChanceObject.SetActive(true);
        }

        public void HideHitChance()
        {
            hitChanceObject.SetActive(false);
        }
    }

}
