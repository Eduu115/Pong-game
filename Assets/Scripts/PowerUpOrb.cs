// -----------------------------------------------
// PowerUpOrb.cs
// Representa un orbe de power-up flotante en el campo
// Se activa cuando la pelota lo golpea
// -----------------------------------------------
// CÓMO USAR:
// 1. Crear GameObject con SphereCollider (IsTrigger = true)
// 2. Añadir este script
// 3. Asignar un PowerUpData en el Inspector
// 4. Opcional: añadir MeshRenderer, Light, ParticleSystem
// -----------------------------------------------

using UnityEngine;

public class PowerUpOrb : MonoBehaviour
{
    [Header("Referencias")]
    public PowerUpData powerUpData; 
    public ParticleSystem orbParticles;
    public Light orbLight;
    public MeshRenderer orbRenderer;

    [Header("Configuración")]
    public float rotationSpeed = 50f;
    public float floatSpeed = 1f;
    public float floatAmplitude = 0.3f;

    private Vector3 startPosition;
    private float floatTimer;
    private Material orbMaterial;

    void Start()
    {
        startPosition = transform.position;

        // Configurar visual según el power-up
        if (powerUpData != null)
        {
            SetupVisuals();
        }
    }

    void Update()
    {
        // Rotación continua
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Movimiento flotante (bobbing)
        floatTimer += Time.deltaTime * floatSpeed;
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(floatTimer) * floatAmplitude;
        transform.position = pos;
    }

    void SetupVisuals()
    {
        // Configurar color del orbe
        if (orbRenderer != null)
        {
            orbMaterial = orbRenderer.material;
            orbMaterial.SetColor("_EmissionColor", powerUpData.glowColor * 2f);
            orbMaterial.EnableKeyword("_EMISSION");
        }

        // Configurar luz
        if (orbLight != null)
        {
            orbLight.color = powerUpData.glowColor;
            orbLight.intensity = 3f;
        }

        // Configurar partículas
        if (orbParticles != null)
        {
            var main = orbParticles.main;
            main.startColor = powerUpData.glowColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Solo se activa con la pelota
        if (other.CompareTag("Ball"))
        {
            ActivatePowerUp();
        }
    }

    void ActivatePowerUp()
    {
        // Notificar al manager
        PowerUpManager manager = FindObjectOfType<PowerUpManager>();
        if (manager != null)
        {
            manager.ActivatePowerUp(powerUpData);
        }

        // Efecto visual de explosión
        if (powerUpData.particlePrefab != null)
        {
            Instantiate(powerUpData.particlePrefab, transform.position, Quaternion.identity);
        }

        // Reproducir sonido
        if (powerUpData.activationSound != null)
        {
            AudioSource.PlayClipAtPoint(powerUpData.activationSound, transform.position);
        }

        // Destruir el orbe
        Destroy(gameObject);
    }
}