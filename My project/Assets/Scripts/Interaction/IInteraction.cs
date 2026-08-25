using UnityEngine;

public interface IInteraction
{
    bool CanInteract(InteractionType action);

    void Interact(InteractionType type, Transform interactor);

    Transform GetTransform();
}

public enum InteractionType
{
    None,
    Use,
    Hold
}
