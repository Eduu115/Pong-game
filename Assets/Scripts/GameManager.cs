using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public BallController ball;
    public Transform paddlePlayer;
    public Transform paddleAI;
    public TMP_Text scoreText;
    public TMP_Text winnerText;
    public GameObject startScreen;   // ⬅ añadido desde GameManager.cs

    [Header("Configuración")]
    public int pointsToWin = 5;

    public Vector3 ballStartPos = Vector3.zero;
    public Vector3 playerStartPos = new Vector3(-3f, 0.5f, 0f);
    public Vector3 aiStartPos = new Vector3(3f, 0.5f, 0f);

    private int scoreLeft = 0;
    private int scoreRight = 0;
    private bool gameEnded = false;
    private bool waitingForServe = false;
    private int servingDirection = 0;

    void Start()
    {
        // Empezamos PAUSADOS y con la pantalla de inicio visible (lógica tomada de GameManager.cs)
        Time.timeScale = 0f;

        scoreLeft = 0;
        scoreRight = 0;
        gameEnded = false;
        waitingForServe = false;

        // Reposicionamos todo pero NO lanzamos la bola todavía
        RepositionEverything();
        UpdateScoreUI();

        if (winnerText) winnerText.gameObject.SetActive(false);
        if (startScreen) startScreen.SetActive(true);
    }

    // ========= FUNCIÓN QUE USA EL BOTÓN START =========
    public void StartGame()
    {
        Debug.Log("StartGame() llamado desde el botón");

        if (startScreen) startScreen.SetActive(false);

        Time.timeScale = 1f;

        scoreLeft = 0;
        scoreRight = 0;
        gameEnded = false;
        waitingForServe = false;

        // Reseteamos velocidad de la bola y empezamos la ronda
        if (ball) ball.ResetSpeed();
        ResetRound(0);      // lanza la bola
        UpdateScoreUI();
    }
    // ==================================================

    public void GoalScored(Goal.Side side)
    {
        if (gameEnded) return;

        // Sumamos puntos
        if (side == Goal.Side.Left)
            scoreRight++;
        else
            scoreLeft++;

        // Actualizamos marcador con mensaje
        if (scoreText)
        {
            scoreText.text = $"{scoreLeft} – {scoreRight}\n<color=yellow>(Pulsa ESPACIO para sacar)</color>";
        }

        // Paramos la bola
        ball.Stop();

        // ¿Alguien ganó?
        if (scoreLeft >= pointsToWin || scoreRight >= pointsToWin)
        {
            EndGame(scoreLeft > scoreRight ? "Jugador" : "IA");
            return; // Salimos, no esperamos saque
        }

        // Calculamos dirección del saque (quien recibió gol saca)
        servingDirection = (side == Goal.Side.Left) ? -1 : 1;

        // Reposicionamos TODO (bola y palas)
        RepositionEverything();

        // Activamos flag de espera
        waitingForServe = true;

        // Incrementamos velocidad YA (antes del saque)
        ball.ResetSpeed();
        ball.IncreaseSpeed(scoreLeft + scoreRight);
    }

    void Update()
    {
        // Esperando saque tras gol
        if (waitingForServe && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("¡ESPACIO DETECTADO! Lanzando bola...");

            waitingForServe = false;

            // Quitamos el mensaje
            UpdateScoreUI();

            // LANZAMOS LA BOLA
            ball.Launch(servingDirection);
        }

        // Reinicio tras victoria
        // IMPORTANTE: Cuando Time.timeScale = 0, Update() sigue ejecutándose
        // pero necesitamos reactivar el tiempo ANTES de mover objetos
        if (gameEnded && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reiniciando partida...");

            // Primero reactivamos el tiempo
            Time.timeScale = 1f;

            // Reseteamos variables
            scoreLeft = 0;
            scoreRight = 0;
            gameEnded = false;
            waitingForServe = false;

            // Ocultamos texto ganador
            if (winnerText) winnerText.gameObject.SetActive(false);

            // Reseteamos velocidad de la bola
            ball.ResetSpeed();

            // Reposicionamos TODO y lanzamos
            ResetRound(0);

            // Actualizamos UI
            UpdateScoreUI();

            Debug.Log($"Posiciones tras reset - Bola: {ball.transform.position}, Player: {paddlePlayer.position}, AI: {paddleAI.position}");
        }
    }

    void EndGame(string winnerName)
    {
        gameEnded = true;
        ball.Stop();

        if (winnerText)
        {
            winnerText.text = $"¡{winnerName} gana!\nPulsa R para reiniciar";
            winnerText.gameObject.SetActive(true);
        }

        Time.timeScale = 0.0f;
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"{scoreLeft} – {scoreRight}";
    }

    // Reposiciona bola y palas sin lanzar
    void RepositionEverything()
    {
        Debug.Log("Reposicionando elementos...");

        if (ball)
        {
            ball.Stop(); // Paramos primero
            ball.transform.position = ballStartPos; // Forzamos posición

            // Aseguramos que el Rigidbody también está en la posición correcta
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb)
            {
                ballRb.position = ballStartPos;
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }

        if (paddlePlayer)
        {
            paddlePlayer.position = playerStartPos;
            // Si las palas tienen Rigidbody, también lo reseteamos
            Rigidbody playerRb = paddlePlayer.GetComponent<Rigidbody>();
            if (playerRb)
            {
                playerRb.position = playerStartPos;
                playerRb.velocity = Vector3.zero;
            }
        }

        if (paddleAI)
        {
            paddleAI.position = aiStartPos;
            Rigidbody aiRb = paddleAI.GetComponent<Rigidbody>();
            if (aiRb)
            {
                aiRb.position = aiStartPos;
                aiRb.velocity = Vector3.zero;
            }
        }
    }

    // Reseteo completo de ronda (usado al inicio y al reiniciar partida)
    void ResetRound(int direction)
    {
        Debug.Log($"ResetRound llamado con dirección: {direction}");

        // Reposicionamos todo primero
        RepositionEverything();

        // Pequeña pausa para asegurar que las físicas se actualizan
        // (esto ayuda cuando venimos de Time.timeScale = 0)
        if (ball)
        {
            // Forzamos que el Rigidbody se "despierte"
            ball.GetComponent<Rigidbody>().WakeUp();

            // Lanzamos la bola
            ball.Launch(direction);
        }
    }
}
