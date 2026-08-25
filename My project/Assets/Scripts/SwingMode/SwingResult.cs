using UnityEngine;

public struct SwingResult
{
    public Vector3 AimDirection;    // Direction normalisée visée par le joueur
    public float NormalizedForce; // 0–1, produit par ForcePhase
    public float LockedLoft;
    public bool IsValid;         // false si le swing a été annulé

    public static SwingResult Cancelled => new SwingResult { IsValid = false };
}
