using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Platformer
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _healthBarSprite;
        [SerializeField] private float reduceSpeed = 2f;
        private Camera cam;
        private float targetFill;

        private void Start()
        {
            cam = Camera.main;
            targetFill = _healthBarSprite.fillAmount;
        }

        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            if (maxHealth <= 0)
            {
                Debug.LogError("Max health must be greater than 0");
                return;
            }

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            targetFill = currentHealth / maxHealth;

            Debug.Log($"Health Bar Updated → {currentHealth}/{maxHealth}");
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

            _healthBarSprite.fillAmount = Mathf.MoveTowards(
                _healthBarSprite.fillAmount,
                targetFill,
                reduceSpeed * Time.deltaTime
            );
        }
    }
}
