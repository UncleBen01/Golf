using UnityEngine;

public class ForcePhase : MonoBehaviour, IForcePhase
{
    [SerializeField] private SkillCheckController skillCheck;
    [SerializeField] private float baseSpeed;

    private bool _resultReady;
    private float _pendingForce;
    private bool _active;

    private void Awake()
    {
        skillCheck.onSuccess.AddListener(OnSuccess);
        skillCheck.onFail.AddListener(OnFail);
    }

    // -------------------------------------------------------------------------
    // IForcePhase
    // -------------------------------------------------------------------------

    public void Begin(float tolerance)
    {
        _resultReady = false;
        _pendingForce = 0f;
        _active = true;

        float speed = baseSpeed / Mathf.Max(tolerance, 0.05f);
        skillCheck.StartSkillCheck(speed);
    }

    public bool Tick(bool confirmPressed, out float normalizedForce)
    {
        normalizedForce = 0f;
        if (!_active) return false;

        if (confirmPressed)
            skillCheck.Confirm();

        if (_resultReady)
        {
            normalizedForce = _pendingForce;
            _active = false;
            _resultReady = false;
            return true;
        }

        return false;
    }

    public void Cancel()
    {
        _active = false;
        skillCheck.Cancel();
    }

    // -------------------------------------------------------------------------
    // Callbacks SkillCheck
    // -------------------------------------------------------------------------

    private void OnSuccess()
    {
        _pendingForce = 1f;
        _resultReady = true;
    }

    private void OnFail()
    {
        _pendingForce = 0f;
        _resultReady = true;
    }
}