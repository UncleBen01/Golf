using UnityEngine;

[CreateAssetMenu(fileName = "ClubData", menuName = "Scriptable Objects/ClubData")]
public class ClubData : ScriptableObject
{
    [Header("Club Information")]
    public string clubName;
    public ClubType clubType;
    public Sprite icon;
    public GameObject prefab;

    [Tooltip("Distance maximale avec un swing parfait")]
    public float maxDistance;

    [Tooltip("Multiplicateur de force au Rigidbody")]
    public float forceMultiplier;

    [Range(0f, 90f)]
    [Tooltip("Angle de la face du club")]
    public float loftAngle;

    public float loftMin;
    public float loftMax;

    [Tooltip("Vitesse d'animation du swing")]
    public float swingSpeed;

    [Tooltip("Tolérance de force pour un swing parfait (0-1)")]
    public float forceTolerance;

    // Force donnée sur la balle
    public Vector3 CalculateLaunchVector(Vector3 aimDirection, float normalizedForce)
    {
        float clampedForce = Mathf.Clamp01(normalizedForce);
        Vector3 horizontal = aimDirection.normalized;

        // Applique le loft : incline le vecteur vers le haut
        float loftRad = loftAngle * Mathf.Deg2Rad;
        Vector3 withLoft = new Vector3(
            horizontal.x * Mathf.Cos(loftRad),
            Mathf.Sin(loftRad),
            horizontal.z * Mathf.Cos(loftRad)
        ).normalized;

        return withLoft * (forceMultiplier * clampedForce);
    }

    public Vector3 CalculateLaunchVectorWithLoft(Vector3 aimDirection, float normalizedForce, float overrideLoft)
    {
        float clampedForce = Mathf.Clamp01(normalizedForce);
        Vector3 horizontal = aimDirection.normalized;

        float loftRad = overrideLoft * Mathf.Deg2Rad;
        Vector3 withLoft = new Vector3(
            horizontal.x * Mathf.Cos(loftRad),
            Mathf.Sin(loftRad),
            horizontal.z * Mathf.Cos(loftRad)
        ).normalized;

        return withLoft * (forceMultiplier * clampedForce);
    }

}



public enum ClubType
{
    Driver,
    Iron,
    Putter
}
