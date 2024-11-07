using Godot;
using System;

public partial class Interactable : RigidBody2D
{
    public static Player player;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        highlightValue += (isHighlighted ? (float)delta : -(float)delta) / highlightDelay;
        highlightValue = Math.Clamp(highlightValue, 0f, 1f);

        Control ui = FindChild("InteractionUI") as Control;
        ui.Modulate = new Color(1, 1, 1, highlightValue * 1);
    }

    public float interactableRange = 100f;

    bool isHighlighted => (GlobalPosition - player.GlobalPosition).Length() < interactableRange;
    float highlightValue = 0f;
    float highlightDelay = 0.4f;

    public virtual void Interacted(Humanoid humanoid)
    {
        GD.Print("I'm interacted!");
        QueueFree();
    }
}
