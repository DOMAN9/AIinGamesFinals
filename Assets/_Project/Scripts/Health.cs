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
            PublishHealthPercentage();
            currentHealth -= damage;
            
        }

        void PublishHealthPercentage()
        {
            if (playerHealthChannel != null)
                playerHealthChannel.Invoke(currentHealth / (float)maxHealth);

            healthBar.UpdateHealthBar(maxHealth, currentHealth);
        }
    }
}