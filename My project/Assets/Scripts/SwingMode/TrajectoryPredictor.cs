using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Paramètres de simulation")]
    [SerializeField] private int pointCount = 60;   // plus = arc plus lisse
    [SerializeField] private float timeStep = 0.05f; // intervalle entre chaque point
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// Affiche l'arc. Appelé par AimPhase à chaque frame pendant le aim.
    /// </summary>
    /// <param name="origin">Position de la balle</param>
    /// <param name="launchVector">Vecteur retourné par ClubData.CalculateLaunchVector()</param>
    public void Show(Vector3 origin, Vector3 launchVector)
    {
        _lineRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeStep;
            Vector3 pos = CalculatePosition(origin, launchVector, t);
            _lineRenderer.SetPosition(i, pos);
        }
    }

    /// <summary>
    /// Cache l'arc. Appelé quand le joueur confirme l'aim.
    /// </summary>
    public void Hide()
    {
        _lineRenderer.positionCount = 0;
    }

    // -------------------------------------------------------------------------
    // Physique balistique — même formule que Unity Rigidbody sans drag
    // pos = origin + velocity * t + 0.5 * gravity * t²
    // -------------------------------------------------------------------------

    private Vector3 CalculatePosition(Vector3 origin, Vector3 velocity, float t)
    {
        return origin
             + velocity * t
             + 0.5f * Physics.gravity * t * t;
    }
}