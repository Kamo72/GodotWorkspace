
using Godot;

public interface IInteractable 
{
    float interactableRange { get; set; }

    void Interacted(Humanoid humanoid);
}
