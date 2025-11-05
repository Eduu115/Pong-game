// -----------------------------------------------
// AIPaddle.cs
// Controla la pala de la IA. Persigue la Z de la bola con suavizado
// -----------------------------------------------
// Conceptos:
// - Lerp (Linear Interpolation): acerca un valor hacia otro de forma suave.
// - reactionSmooth: cuanto mayor, más "precisa" (y difícil) la IA.
// - speed: velocidad máxima de desplazamiento por frame para limitar reacciones bruscas.
// -----------------------------------------------

using UnityEngine;

public class AIPaddle : MonoBehaviour
{
    // Referencia a la bola (asignar en el Inspector)
    public Transform ball;

    // Velocidad máxima de desplazamiento de la pala IA
    public float speed = 10f;

    // Límite superior/inferior del campo en Z
    public float yLimit = 6.5f;

    // Factor de suavizado para el seguimiento (más alto = más pegado a la bola)
    public float reactionSmooth = 8f;

    // Update: ejecuta lógica de seguimiento
    void Update()
    {
        // Si aún no hemos asignado la bola, no hacemos nada
        if (!ball) return;

        // Tomamos la posición actual
        Vector3 pos = transform.position;

        // Objetivo: igualar la Z de la bola
        float targetY = ball.position.y;

        // Lerp: mueve pos.z suavemente hacia targetZ
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * reactionSmooth);

        // Limitamos cuánto puede moverse en este frame para que no sea instantáneo
        float delta = Mathf.Clamp(pos.y - transform.position.y, -speed * Time.deltaTime, speed * Time.deltaTime);
        pos.y = transform.position.y + delta;

        // Evitamos salirnos de los límites del campo
        pos.y = Mathf.Clamp(pos.y, -yLimit, yLimit);

        // Aplicamos la posición
        transform.position = pos;
    }
}
