using UnityEngine;

public interface IForcePhase
{
    void Begin(float tolerance);

    /// <param name="confirmPressed">true si le joueur a appuyé sur Swing ce frame</param>
    /// <param name="normalizedForce">Force capturée (0–1)</param>
    /// <returns>true quand la force est confirmée</returns>
    bool Tick(bool confirmPressed, out float normalizedForce);

    void Cancel();
}
