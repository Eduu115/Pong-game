// -----------------------------------------------
// PowerUpData.cs
// ScriptableObject que define las propiedades de cada power-up
// Crear desde: Assets > Create > Pong Chaos > Power-Up Data
// -----------------------------------------------

using UnityEngine;

[CreateAssetMenu(fileName = "New PowerUp", menuName = "Pong Chaos/Power-Up Data")]
public class PowerUpData : ScriptableObject
{
    [Header("Identificación")]
    public string powerUpName = "Shield Pulse";
    public PowerUpType type;

    [Header("Propiedades")]
    [Tooltip("Duración del efecto en segundos")]
    public float duration = 3f;

    [Tooltip("Intensidad del efecto (uso variable según tipo)")]
    public float effectIntensity = 1f;

    [Header("Visual")]
    public Color glowColor = Color.cyan;
    public GameObject particlePrefab;

    [Header("Audio")]
    public AudioClip activationSound;
    public AudioClip loopSound;

    [Header("Descripción")]
    [TextArea(2, 4)]
    public string description = "Protege la portería durante un impacto";
}

public enum PowerUpType
{
    ShieldPulse,      // Protege la portería
    MirrorField,      // Devuelve la pelota automáticamente
    CloneBall,        // Duplica la pelota
    Inverter,         // Invierte controles del rival
    Freeze,           // Congela la pala del oponente
    SpeedBoost,       // Acelera la pelota
    SlowMotion,       // Ralentiza la pelota
    GhostBall         // La pelota atraviesa una vez
}