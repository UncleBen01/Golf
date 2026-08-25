using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class SwingController : MonoBehaviour
{
    private PlayerInputActions input;

    [Header("Bâton actif")]
    [SerializeField] private ClubData activeClub;

    [Header("Phases")]
    [SerializeField] private MonoBehaviour aimPhaseBehaviour;
    [SerializeField] private MonoBehaviour forcePhaseBehaviour;

    [Header("Caméra")]
    [SerializeField] private MonoBehaviour cameraController;
    [SerializeField] private CinemachineVirtualCamera vcamFirstPerson;
    [SerializeField] private CinemachineVirtualCamera vcamSwingMode;

    [Header("Références")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CourseManager courseManager;
    [SerializeField] private Inventory inventory;

    public event Action<SwingResult, ClubData> OnSwingCompleted;
    public event Action OnSwingCancelled;
    public event Action<SwingState> OnStateChanged;

    public SwingState CurrentState { get; private set; } = SwingState.Out;

    private IAimPhase aimPhase;
    private IForcePhase forcePhase;
    private Vector3 lockedDirection;
    private float lockedLoft;
    private Vector2 aimInput;
    private bool swingPressed;
    private float scrollInput;

    private void Awake()
    {
        input = new PlayerInputActions();

        aimPhase = ValidatePhase<IAimPhase>(aimPhaseBehaviour, "IAimPhase");
        forcePhase = ValidatePhase<IForcePhase>(forcePhaseBehaviour, "IForcePhase");
        courseManager = FindFirstObjectByType<CourseManager>();
        inventory.OnClubSelected += SetActiveClub; //Event pour changer de club depuis l'inventaire
    }

    private void OnEnable()
    {
        input.SwingMode.Aim.performed += ctx => aimInput = ctx.ReadValue<Vector2>();
        input.SwingMode.Aim.canceled += ctx => aimInput = Vector2.zero;
        input.SwingMode.ConfirmAim.performed += ctx => swingPressed = true;
        input.SwingMode.IncreaseLoft.performed += ctx => scrollInput = ctx.ReadValue<float>();
        input.SwingMode.ReduceLoft.performed += ctx => scrollInput = -ctx.ReadValue<float>();
        input.SwingMode.IncreaseLoft.canceled += ctx => scrollInput = 0f;
        input.SwingMode.ReduceLoft.canceled += ctx => scrollInput = 0f;
        input.SwingMode.CancelAim.performed += ctx => OnCancelPerformed();
        //input.ForceMode.Confirm.performed += ctx => swingPressed = true;
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case SwingState.Aiming:
                HandleAimPhase();
                break;

            case SwingState.ChargingForce:
                HandleForcePhase();
                break;
        }

        swingPressed = false; // consommé, remet à zero chaque fin de frame
    }

    // -------------------------------------------------------------------------
    // Handlers par état — appelés chaque frame quand l'état est actif
    // -------------------------------------------------------------------------

    private void HandleAimPhase()
    {
        playerController.FreezePlayer();
        vcamSwingMode.Priority = 20;
        vcamFirstPerson.Priority = 0;

        bool confirmed = aimPhase.Tick(aimInput, scrollInput, swingPressed, activeClub, out Vector3 direction, out float loft);

        if (confirmed)
        {
            lockedDirection = direction;
            lockedLoft = loft;
            EnterForcePhase();
        }
    }

    private void HandleForcePhase()
    {
        bool confirmed = forcePhase.Tick(swingPressed, out float force);

        if (confirmed)
            CommitSwing(force);
    }

    private void OnCancelPerformed()
    {
        if (CurrentState == SwingState.Aiming || CurrentState == SwingState.ChargingForce)
            CancelSwing();
    }

    // -------------------------------------------------------------------------
    // Transitions — appelées UNE SEULE FOIS à l'entrée dans chaque état
    // -------------------------------------------------------------------------

    /// <summary>
    /// Point d'entrée public — appelé par Ball.Interact() quand le joueur
    /// interagit avec la balle.
    /// </summary>
    public void EnterAimPhase(Transform ballTransform) //TODO: Est ce que c'est la balle qui doit connaitre le transform du drapeau????
    {
        if (CurrentState != SwingState.Out) return;
        if (activeClub == null) { Debug.LogWarning("[SwingController] Aucun bâton actif."); return; }

        SetCameraControlEnabled(false); // bloque la cam, libère le curseur pour l'aim
        playerController.SnapToBall(ballTransform, courseManager.CurrentHole.GetFlagTransform());
        aimPhase.Begin(courseManager.CurrentHole.GetFlagTransform());
        SetState(SwingState.Aiming);
    }

    private void EnterForcePhase()
    {
        forcePhase.Begin(activeClub.forceTolerance);
        SetState(SwingState.ChargingForce);
    }

    private void CommitSwing(float normalizedForce)
    {
        SetState(SwingState.Swinging);

        SwingResult result = new SwingResult
        {
            AimDirection = lockedDirection,
            NormalizedForce = Mathf.Clamp01(normalizedForce),
            LockedLoft = lockedLoft,
            IsValid = true
        };

        OnSwingCompleted?.Invoke(result, activeClub);
    }

    private void CancelSwing()
    {
        aimPhase?.Cancel();
        forcePhase?.Cancel();
        OnSwingCancelled?.Invoke();
        ExitSwingMode();
    }

    private void ExitSwingMode()
    {
        vcamSwingMode.Priority = 0;
        vcamFirstPerson.Priority = 20;
        SetCameraControlEnabled(true); //J'en ai encore besoin ou pas?
        SetState(SwingState.Out);
        playerController.UnFreezePlayer();
    }

    // -------------------------------------------------------------------------
    // API publique
    // -------------------------------------------------------------------------

    public void EndSwingAnimation()
    {
        if (CurrentState == SwingState.Swinging)
            
        ExitSwingMode();
    }

    public void SetActiveClub(ClubData club)
    {
        if (CurrentState != SwingState.Out) CancelSwing();
        activeClub = club;
    }

    public void ForceInterrupt()
    {
        if (CurrentState != SwingState.Out) CancelSwing();
    }

    // -------------------------------------------------------------------------
    // Caméra
    // -------------------------------------------------------------------------

    /// <summary>
    /// Active ou désactive le script de caméra.
    /// Glisse ton CinemachineInputProvider (ou script custom) dans le slot Inspector.
    /// Quand disabled, la souris contrôle l'aim au lieu de la cam.
    /// </summary>
    private void SetCameraControlEnabled(bool enabled)
    {
        if (cameraController != null)
            cameraController.enabled = enabled;
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    public void SetState(SwingState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    private T ValidatePhase<T>(MonoBehaviour behaviour, string interfaceName) where T : class
    {
        if (behaviour == null)
        {
            Debug.LogError($"[SwingController] {interfaceName} n'est pas assigné dans l'Inspector.");
            return null;
        }
        T phase = behaviour as T;
        if (phase == null)
            Debug.LogError($"[SwingController] {behaviour.name} n'implémente pas {interfaceName}.");
        return phase;
    }
}

public enum SwingState
{
    Out,
    Aiming,
    ChargingForce,
    Swinging
}