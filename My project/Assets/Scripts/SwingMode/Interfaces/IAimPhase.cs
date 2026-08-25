using Unity.Hierarchy;
using UnityEngine;

public interface IAimPhase
{
    /// <summary>
    /// Pour set up le début de la phase d'aim.
    /// </summary>
    /// <param name="tolerance">jsp encore</param>
    void Begin(Transform flagTransform);

    /// <summary>
    /// Appelé chaque frame pendant la phase d'aim. Doit retourner true quand le joueur confirme son aim, et false s'il continue à viser.
    /// </summary>
    /// <param name="lockedDirection"></param>
    /// <returns></returns>
    bool Tick(Vector2 aimInput, float scrollInput, bool swingPressed, ClubData activeClub, out Vector3 lockedDirection, out float currentLoft);

    /// <summary>
    /// Le joueur cancel aiming et retourne à l'état Idle.
    /// </summary>
    void Cancel();
}
