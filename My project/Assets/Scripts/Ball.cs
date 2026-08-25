using UnityEngine;

public class Ball : MonoBehaviour, IInteraction
{
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private SwingController swingController;

    private void Awake()
    {
        if (swingController == null)
            swingController = FindFirstObjectByType<SwingController>();
    }

    public bool CanInteract(InteractionType action)
    {
        return interactionType == action;
    }
    public void Interact(InteractionType type, Transform interactor)
    {
        swingController.EnterAimPhase(transform); //TODO: Voir comment j'envoie le transform du drapeau
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
