// -----------------------------------------------
// PowerUpManager.cs
// Gestiona la generación, activación y efectos de todos los power-ups
// -----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [Header("Referencias del Juego")]
    public GameManager gameManager;
    public BallController ball;
    public Transform paddlePlayer;
    public Transform paddleAI;

    [Header("Configuración de Spawn")]
    public GameObject powerUpOrbPrefab;
    public List<PowerUpData> availablePowerUps;
    public float spawnInterval = 15f;
    public Vector3 spawnArea = new Vector3(8f, 5f, 0f); // Área de spawn
    public int maxActiveOrbs = 2;

    [Header("Estado Actual")]
    public PowerUpData currentActivePowerUp;
    public float currentEffectTimeRemaining;

    // Control interno
    private List<GameObject> activeOrbs = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private Coroutine effectCoroutine;

    // Referencias para efectos activos
    private bool isShieldActive = false;
    private bool isMirrorFieldActive = false;
    private GameObject cloneBall;
    private bool controlsInverted = false;
    private bool paddleFrozen = false;

    void Start()
    {
        // Iniciar generación de power-ups
        StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnPowerUpsRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        ClearAllOrbs();
    }

    IEnumerator SpawnPowerUpsRoutine()
    {
        yield return new WaitForSeconds(5f); // Espera inicial

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeOrbs.Count < maxActiveOrbs)
            {
                SpawnRandomPowerUp();
            }
        }
    }

    void SpawnRandomPowerUp()
    {
        if (availablePowerUps.Count == 0 || powerUpOrbPrefab == null) return;

        // Seleccionar power-up aleatorio
        PowerUpData randomPowerUp = availablePowerUps[Random.Range(0, availablePowerUps.Count)];

        // Posición aleatoria en el área de spawn
        Vector3 spawnPos = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            12f
        );

        // Crear orbe
        GameObject orb = Instantiate(powerUpOrbPrefab, spawnPos, Quaternion.identity);
        PowerUpOrb orbScript = orb.GetComponent<PowerUpOrb>();

        if (orbScript != null)
        {
            orbScript.powerUpData = randomPowerUp;
        }

        activeOrbs.Add(orb);

        Debug.Log($"🌟 Power-up spawneado: {randomPowerUp.powerUpName} en {spawnPos}");
    }

    public void ActivatePowerUp(PowerUpData powerUp)
    {
        if (powerUp == null) return;

        Debug.Log($"⚡ Activando power-up: {powerUp.powerUpName}");

        // Detener efecto anterior si existe
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            DeactivateCurrentEffect();
        }

        // Guardar nuevo power-up activo
        currentActivePowerUp = powerUp;
        currentEffectTimeRemaining = powerUp.duration;

        // Activar efecto según tipo
        effectCoroutine = StartCoroutine(PowerUpEffectRoutine(powerUp));
    }

    IEnumerator PowerUpEffectRoutine(PowerUpData powerUp)
    {
        // Activar efecto
        ApplyPowerUpEffect(powerUp, true);

        // Esperar duración
        float timer = 0f;
        while (timer < powerUp.duration)
        {
            timer += Time.deltaTime;
            currentEffectTimeRemaining = powerUp.duration - timer;
            yield return null;
        }

        // Desactivar efecto
        ApplyPowerUpEffect(powerUp, false);
        currentActivePowerUp = null;
        currentEffectTimeRemaining = 0f;
    }

    void ApplyPowerUpEffect(PowerUpData powerUp, bool activate)
    {
        switch (powerUp.type)
        {
            case PowerUpType.ShieldPulse:
                ApplyShieldPulse(activate);
                break;

            case PowerUpType.MirrorField:
                ApplyMirrorField(activate);
                break;

            case PowerUpType.CloneBall:
                ApplyCloneBall(activate);
                break;

            case PowerUpType.Inverter:
                ApplyInverter(activate);
                break;

            case PowerUpType.Freeze:
                ApplyFreeze(activate);
                break;

            case PowerUpType.SpeedBoost:
                ApplySpeedBoost(activate, powerUp.effectIntensity);
                break;

            case PowerUpType.SlowMotion:
                ApplySlowMotion(activate, powerUp.effectIntensity);
                break;

            case PowerUpType.GhostBall:
                ApplyGhostBall(activate);
                break;
        }
    }

    // ============================================
    // IMPLEMENTACIÓN DE CADA POWER-UP
    // ============================================

    void ApplyShieldPulse(bool activate)
    {
        isShieldActive = activate;

        if (activate)
        {
            Debug.Log("🛡️ Shield Pulse ACTIVADO - Próxima pelota será devuelta automáticamente");
            // Aquí puedes añadir efecto visual en la portería
        }
        else
        {
            Debug.Log("🛡️ Shield Pulse desactivado");
        }
    }

    void ApplyMirrorField(bool activate)
    {
        isMirrorFieldActive = activate;

        if (activate)
        {
            Debug.Log("🪞 Mirror Field ACTIVADO");
            // Crear efecto visual de espejo
        }
        else
        {
            Debug.Log("🪞 Mirror Field desactivado");
        }
    }

    void ApplyCloneBall(bool activate)
    {
        if (activate && ball != null)
        {
            Debug.Log("👥 Clone Ball ACTIVADO");

            // Crear pelota clonada
            cloneBall = Instantiate(ball.gameObject, ball.transform.position, Quaternion.identity);
            BallController cloneBallScript = cloneBall.GetComponent<BallController>();

            if (cloneBallScript != null)
            {
                // Lanzar en dirección opuesta
                Vector3 originalVelocity = ball.GetComponent<Rigidbody>().velocity;
                cloneBallScript.GetComponent<Rigidbody>().velocity = new Vector3(
                    originalVelocity.x,
                    -originalVelocity.y,
                    originalVelocity.z
                );
            }

            // Hacer la clon más transparente
            Renderer cloneRenderer = cloneBall.GetComponent<Renderer>();
            if (cloneRenderer != null)
            {
                Color c = cloneRenderer.material.color;
                c.a = 0.6f;
                cloneRenderer.material.color = c;
            }
        }
        else if (cloneBall != null)
        {
            Debug.Log("👥 Clone Ball desactivado");
            Destroy(cloneBall);
        }
    }

    void ApplyInverter(bool activate)
    {
        controlsInverted = activate;

        // Necesitarás modificar PlayerPaddle para usar esta variable
        PlayerPaddle playerScript = paddlePlayer.GetComponent<PlayerPaddle>();
        if (playerScript != null)
        {
            // Añadir una variable pública en PlayerPaddle: public bool controlsInverted;
            // playerScript.controlsInverted = activate;
        }

        Debug.Log(activate ? "🔄 Controles INVERTIDOS" : "🔄 Controles normales");
    }

    void ApplyFreeze(bool activate)
    {
        paddleFrozen = activate;

        AIPaddle aiScript = paddleAI.GetComponent<AIPaddle>();
        if (aiScript != null)
        {
            aiScript.enabled = !activate;
        }

        Debug.Log(activate ? "❄️ IA CONGELADA" : "❄️ IA descongelada");
    }

    void ApplySpeedBoost(bool activate, float intensity)
    {
        if (ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (activate)
            {
                rb.velocity *= intensity; // Ej: 2.0 = doble velocidad
                Debug.Log($"⚡ Velocidad aumentada x{intensity}");
            }
            else
            {
                rb.velocity /= intensity;
                Debug.Log("⚡ Velocidad normalizada");
            }
        }
    }

    void ApplySlowMotion(bool activate, float intensity)
    {
        if (activate)
        {
            Time.timeScale = intensity; // Ej: 0.5 = mitad de velocidad
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Debug.Log($"🐌 Slow Motion activado: {intensity}x");
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            Debug.Log("🐌 Velocidad normal");
        }
    }

    void ApplyGhostBall(bool activate)
    {
        if (ball != null)
        {
            Collider ballCollider = ball.GetComponent<Collider>();
            if (ballCollider != null)
            {
                // Hacer que atraviese las palas pero no las paredes
                // Necesitarás usar Layers para esto
                ballCollider.isTrigger = activate;
            }

            Debug.Log(activate ? "👻 Ghost Ball ACTIVADO" : "👻 Ghost Ball desactivado");
        }
    }

    void DeactivateCurrentEffect()
    {
        if (currentActivePowerUp != null)
        {
            ApplyPowerUpEffect(currentActivePowerUp, false);
        }
    }

    void ClearAllOrbs()
    {
        foreach (GameObject orb in activeOrbs)
        {
            if (orb != null) Destroy(orb);
        }
        activeOrbs.Clear();
    }

    // Método público para verificar si shield está activo (llamar desde Goal.cs)
    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    public bool IsMirrorFieldActive()
    {
        return isMirrorFieldActive;
    }
}