using System;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("Références")]
    private SwingController swingController;
    [SerializeField] private ClubData activeClub;

    [Header("Balle")]
    [SerializeField] private Rigidbody ballRigidbody;

    [Header("Paramètres physiques")]
    [Tooltip("Multiplicateur global appliqué après les stats du bâton")]
    [SerializeField] private float globalForceScale = 1f;

    [Tooltip("Force angulaire (backspin/topspin) selon le loft")]
    [SerializeField] private float spinMultiplier = 0.4f;

    // -------------------------------------------------------------------------
    // Events — systèmes audio/horror/score s'abonnent ici
    // -------------------------------------------------------------------------

    public event Action<Vector3> OnBallLaunched;   // direction + magnitude
    public event Action<Vector3> OnBallLanded;     // position d'atterrissage
    public event Action<BallState> OnBallStateChanged;

    // -------------------------------------------------------------------------
    // État
    // -------------------------------------------------------------------------

    public BallState CurrentState { get; private set; } = BallState.AtRest;

    private Vector3 _lastLaunchVelocity;

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    private void Awake()
    {
        if (ballRigidbody == null)
            ballRigidbody = GetComponent<Rigidbody>();
        swingController = FindFirstObjectByType<SwingController>();
    }

    private void OnEnable()
    {
        if (swingController != null)
            swingController.OnSwingCompleted += HandleSwingCompleted;
    }

    private void OnDisable()
    {
        if (swingController != null)
            swingController.OnSwingCompleted -= HandleSwingCompleted;
    }

    private void FixedUpdate()
    {
        TrackBallState();
    }

    // -------------------------------------------------------------------------
    // Réception du SwingResult
    // -------------------------------------------------------------------------

    private void HandleSwingCompleted(SwingResult result, ClubData activeClub)
    {
        if (!result.IsValid) return;
        this.activeClub = activeClub; // Met à jour le club actif avant de lancer la balle
        Launch(result);
    }

    // -------------------------------------------------------------------------
    // Lancement
    // -------------------------------------------------------------------------

    private void Launch(SwingResult result)
    {
        // Réinitialise la vélocité avant d'appliquer la force (important pour putts successifs)
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;

        // Calcule le vecteur de lancement via ClubData
        Vector3 launchVector = activeClub.CalculateLaunchVectorWithLoft(
            result.AimDirection,
            result.NormalizedForce,
            result.LockedLoft
        ) * globalForceScale;

        ballRigidbody.AddForce(launchVector, ForceMode.Impulse);

        // Applique un spin réaliste (backspin selon l'axe latéral)
        ApplySpin(result.AimDirection, result.NormalizedForce);

        _lastLaunchVelocity = launchVector;

        SetState(BallState.InFlight);
        OnBallLaunched?.Invoke(launchVector);

        // Informe SwingController que l'animation peut se terminer
        swingController.EndSwingAnimation();
    }

    private void ApplySpin(Vector3 aimDirection, float normalizedForce)
    {
        // Axe de spin = perpendiculaire à la direction, dans le plan horizontal
        Vector3 spinAxis = Vector3.Cross(Vector3.up, aimDirection).normalized;
        float spinForce = normalizedForce * spinMultiplier * activeClub.loftAngle;

        ballRigidbody.AddTorque(spinAxis * spinForce, ForceMode.Impulse);
    }

    // -------------------------------------------------------------------------
    // Suivi d'état de la balle
    // -------------------------------------------------------------------------

    private void TrackBallState()
    {
        if (CurrentState == BallState.AtRest) return;

        bool isMoving = ballRigidbody.linearVelocity.magnitude > 0.05f;

        if (CurrentState == BallState.InFlight && !isMoving)
        {
            SetState(BallState.Rolling);
        }
        else if (CurrentState == BallState.Rolling && !isMoving)
        {
            SetState(BallState.AtRest);
            OnBallLanded?.Invoke(transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == BallState.InFlight)
            SetState(BallState.Rolling);
    }

    private void SetState(BallState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnBallStateChanged?.Invoke(newState);
    }

    // -------------------------------------------------------------------------
    // API publique
    // -------------------------------------------------------------------------

    /// <summary>
    /// Téléporte la balle à une position (respawn, trou en un, etc.)
    /// sans déclencher d'events de lancement.
    /// </summary>
    public void PlaceBall(Vector3 position)
    {
        ballRigidbody.linearVelocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        transform.position = position;
        SetState(BallState.AtRest);
    }

    /// <summary>
    /// Synchronise le bâton actif depuis ClubSelectionUI ou SwingController.
    /// </summary>
    public void SetActiveClub(ClubData club) => activeClub = club;
}

// -------------------------------------------------------------------------
// Enum
// -------------------------------------------------------------------------

public enum BallState
{
    AtRest,     // immobile, prête à être frappée
    InFlight,   // en l'air
    Rolling     // au sol mais encore en mouvement
}