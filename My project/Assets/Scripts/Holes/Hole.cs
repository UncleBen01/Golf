using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private HoleData holeData;
    [SerializeField] private Transform flagTransform;
    [SerializeField] private Transform teeTransform;

    public void StartHole()
    {
        // À appeler lorsque le joueur dépose la balle sur le teebox?
    }

    public Transform GetFlagTransform() => flagTransform;

    public Transform GetTeeTransform() => teeTransform;

    public HoleData GetHoleData() => holeData;
}
