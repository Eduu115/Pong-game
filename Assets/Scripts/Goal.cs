// -----------------------------------------------
// Goal.cs
// Detecta cuando la bola entra en una zona de gol (trigger) y avisa al GameManager
// -----------------------------------------------
// Requisitos de escena:
// - Los objetos Goal_Left y Goal_Right deben tener un BoxCollider con IsTrigger = true
// - La bola debe tener Tag = "Ball"
// -----------------------------------------------

using UnityEngine;

public class Goal : MonoBehaviour
{
    // Enum para indicar qué portería es (izquierda o derecha)
    public enum Side { Left, Right }

    // Selección en Inspector de este lado
    public Side side;

    // Referencia al GameManager para reportar goles
    public GameManager gameManager;

    // OnTriggerEnter se llama cuando otro collider (con Rigidbody) entra en este Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos que el objeto que ha entrado tiene Tag "Ball"
        if (other.CompareTag("Ball"))
        {
            // Avisamos al GameManager de que ha habido gol en este lado
            gameManager.GoalScored(side);
        }
    }
}
