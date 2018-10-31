using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MHA.UserInterface
{
    public class HealthBarControl : MonoBehaviour
    {

        public Image currentHealthImage;
        public Image backHealthImage;

        public Transform cam;

        void Start()
        {
            cam = Camera.main.transform;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            transform.forward = cam.forward;
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float healthRatio = currentHealth / maxHealth;
            currentHealthImage.fillAmount = healthRatio;
        }
    }

}
