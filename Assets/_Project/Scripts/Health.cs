using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Platformer
{
    public class Health : MonoBehaviour
    {
        [SerializeField] int maxHealth = 100;
        [SerializeField] FloatEventChannel playerHealthChannel;

        int currentHealth;

        [SerializeField] private HealthBar healthBar;

        public bool IsDead => currentHealth <= 0;

        // 🔹 Added public getters for UI access
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        void Awake()
        {
            currentHealth = maxHealth;
        }

        void Start()
        {
            PublishHealthPercentage();
            healthBar.UpdateHealthBar(maxHealth, currentHealth);
        }

        public void TakeDamage(int damage)
        {
            // 🔹 Subtract before updating UI
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            PublishHealthPercentage();
        }

        void PublishHealthPercentage()
        {
            if (playerHealthChannel != null)
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);

            healthBar.UpdateHealthBar(maxHealth, currentHealth);
        }
    }
}
