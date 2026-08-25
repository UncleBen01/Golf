using Cinemachine;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
public class AimPhase : MonoBehaviour, IAimPhase
{
    [Header("Paramètres d'aim")]
    [SerializeField] private float rotationSpeed;    // degrés/seconde

    [Header("Balle — origine de l'arc")]
    [SerializeField] private Transform ballTransform;

    [Header("Trajectoire")]
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;

    [SerializeField] private GameObject playerPrefab;

    private float loftScrollSpeed = 2f;
    private float _currentAngle;
    private float currentLoft;
    private bool _active;

    private void Awake()
    {
        ballTransform = FindAnyObjectByType<Ball>().transform;
    }

    public void Begin(Transform flagTransform)
    {
        Vector3 directionToFlag = (flagTransform.position - ballTransform.position);
        directionToFlag.y = 0f;
        _currentAngle = Mathf.Atan2(directionToFlag.x, directionToFlag.z) * Mathf.Rad2Deg; //Pour que le vecteur de direction soit aligné avec le drapeau au début de l'aim

        _active = true;
    }

    public bool Tick(Vector2 aimInput, float scrollInput, bool confirmPressed, ClubData activeClub, out Vector3 currentDirection, out float currentLoft)
    {
        currentDirection = Vector3.forward;
        currentLoft = this.currentLoft;
        if (!_active) return false;

        _currentAngle += aimInput.x * rotationSpeed * Time.deltaTime;
        currentDirection = Quaternion.Euler(0f, _currentAngle, 0f) * Vector3.forward;

        this.currentLoft += scrollInput * loftScrollSpeed;
        this.currentLoft = Mathf.Clamp(this.currentLoft, activeClub.loftMin, activeClub.loftMax);
        currentLoft = this.currentLoft;

        UpdateTrajectory(currentDirection, activeClub);

        if (confirmPressed)
        {
            trajectoryPredictor.Hide();
            _active = false;
            return true;
        }

        return false;
    }

    public void Cancel()
    {
        _active = false;
        trajectoryPredictor.Hide();
    }

    private void UpdateTrajectory(Vector3 direction, ClubData activeClub)
    {
        if (trajectoryPredictor == null || activeClub == null || ballTransform == null) return;

        // Puissance maximale (1f) pour montrer le meilleur cas
        Vector3 launchVector = activeClub.CalculateLaunchVectorWithLoft(direction, 1f, currentLoft);
        trajectoryPredictor.Show(ballTransform.position, launchVector);
    }
}
