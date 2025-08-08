using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Platformer
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image _healthBarSprite;
        [SerializeField] private float reduceSpeed = 2f;
        private Camera cam;
        private float target;

        private void Start()
        {
            cam = Camera.main;
        }

        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            if (maxHealth <= 0)
            {
                Debug.LogError("Max health must be greater than 0");
                return;
            }

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            Debug.Log($"Max Health: {maxHealth}, Current Health: {currentHealth}");

            _healthBarSprite.fillAmount = currentHealth / maxHealth;
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
            _ = _healthBarSprite.fillAmount - Mathf.MoveTowards(_healthBarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
        }
    }
}
