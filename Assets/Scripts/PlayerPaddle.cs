using UnityEngine;

public class PlayerPaddle : MonoBehaviour
{
    // Velocidad de la pala (unidades del mundo por segundo)
    public float speed = 12f;

    // Límite superior/inferior del campo en Z para que la pala no se salga
    public float yLimit = 6.5f;
    // Limite horizontal
    public float xLimitPos = -14f;
    public float xLimitNeg = -10f;

    // Factor de suavizado del movimiento
    public float smoothFactor = 0.1f; // Aseguramos que sea un valor mayor para suavizar

    // Variables para controlar la posición y suavizado
    private Vector3 targetPosition;

    void Start()
    {
        // Inicializamos la posición objetivo con la posición actual
        targetPosition = transform.position;
    }

    void Update()
    {
        // Obtenemos la entrada del jugador
        float inputY = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        // Calculamos el movimiento
        Vector3 movement = new Vector3(inputX, inputY, 0f) * speed * Time.deltaTime;

        // Aplicamos el movimiento
        Vector3 newPosition = transform.position + movement;

        // Limitamos la posición
        newPosition.x = Mathf.Clamp(newPosition.x, xLimitPos, xLimitNeg);
        newPosition.y = Mathf.Clamp(newPosition.y, -yLimit, yLimit);

        // Asignamos la nueva posición
        transform.position = newPosition;
    }
}
