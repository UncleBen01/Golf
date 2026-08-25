using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private const float RAYON_CAST_SPHERE = .1f;
    private const float DISTANCE_INTERACTION = 3f;
    [SerializeField] private LayerMask layerMask;

    private Transform cameraMainTransform;
    private PlayerInputActions input;
    private IInteraction currentInteraction;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.PlayerActions.Interact.started += TryInteract;
        input.PlayerActions.Interact.performed += TryInteract;
        input.PlayerActions.Interact.canceled += TryInteract;
    }
    private void OnEnable() => input.PlayerActions.Enable();
    private void OnDisable() => input.PlayerActions.Disable();


    private void Start()
    {
        cameraMainTransform = Camera.main.transform;
    }

    public void TryInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            currentInteraction = GetInteractableObject();

        if (currentInteraction == null) return;

        // HOLD
        //if (currentInteraction.CanInteract(InteractionType.Hold))
        //{
        //    currentInteraction.Interact(InteractionType.Hold, transform, context);
        //    return;
        //}

        // USE (une seule fois au start)
        if (context.started && currentInteraction.CanInteract(InteractionType.Use))
        {
            currentInteraction.Interact(InteractionType.Use, transform/*, context*/);
        }
    }

    public IInteraction GetInteractableObject()
    {
        RaycastHit[] hits = Physics.SphereCastAll(
            cameraMainTransform.position,
            RAYON_CAST_SPHERE,
            cameraMainTransform.forward,
            DISTANCE_INTERACTION,
            layerMask
        );

        // Trier par distance
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            IInteraction interaction = hit.transform.GetComponentInParent<IInteraction>();
            if (interaction != null)
                return interaction;
        }

        //Aucun objet interactible trouvé
        return null;
    }
}
