using Platformer;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName = "Dominique Manabat";
    public int playerScore = 0;
    public Transform playerTransform; // Drag your player here

    [Header("References")]
    public Health playerHealthScript; // Drag your existing Health component here
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI positionText;

    void Update()
    {
        if (nameText != null)
            nameText.text = "Name: " + playerName;

        if (healthText != null && playerHealthScript != null)
            healthText.text = "Health: "
                + playerHealthScript.CurrentHealth + " / "
                + playerHealthScript.MaxHealth;

        if (scoreText != null)
            scoreText.text = "Score: " + playerScore;

        if (positionText != null && playerTransform != null)
        {
            Vector3 pos = playerTransform.position;
            positionText.text = $"Position: ({pos.x:F1}, {pos.y:F1}, {pos.z:F1})";
        }
    }
}
