// -----------------------------------------------
// PowerUpUI.cs
// Muestra el power-up activo y su tiempo restante
// Adjuntar a un objeto Canvas en la escena
// -----------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerUpUI : MonoBehaviour
{
    [Header("Referencias")]
    public PowerUpManager powerUpManager;

    [Header("UI Elements")]
    public GameObject powerUpPanel;
    public TMP_Text powerUpNameText;
    public TMP_Text powerUpTimerText;
    public Image powerUpIcon;
    public Image timerFillBar;

    [Header("Animación")]
    public float pulseSpeed = 2f;
    public float pulseScale = 1.1f;

    private float maxDuration;
    private Vector3 originalScale;

    void Start()
    {
        if (powerUpPanel != null)
        {
            originalScale = powerUpPanel.transform.localScale;
            powerUpPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (powerUpManager == null) return;

        // Verificar si hay un power-up activo
        if (powerUpManager.currentActivePowerUp != null)
        {
            if (!powerUpPanel.activeSelf)
            {
                // Activar panel
                powerUpPanel.SetActive(true);
                maxDuration = powerUpManager.currentActivePowerUp.duration;

                // Configurar textos
                if (powerUpNameText != null)
                {
                    powerUpNameText.text = powerUpManager.currentActivePowerUp.powerUpName;
                    powerUpNameText.color = powerUpManager.currentActivePowerUp.glowColor;
                }
            }

            // Actualizar timer
            float timeRemaining = powerUpManager.currentEffectTimeRemaining;

            if (powerUpTimerText != null)
            {
                powerUpTimerText.text = $"{timeRemaining:F1}s";
            }

            // Actualizar barra de progreso
            if (timerFillBar != null)
            {
                timerFillBar.fillAmount = timeRemaining / maxDuration;
                timerFillBar.color = powerUpManager.currentActivePowerUp.glowColor;
            }

            // Efecto de pulso cuando queda poco tiempo
            if (timeRemaining < 2f && powerUpPanel != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * 3f) * 0.15f;
                powerUpPanel.transform.localScale = originalScale * pulse;
            }
        }
        else
        {
            // Desactivar panel si no hay power-up activo
            if (powerUpPanel != null && powerUpPanel.activeSelf)
            {
                powerUpPanel.SetActive(false);
                powerUpPanel.transform.localScale = originalScale;
            }
        }
    }
}
