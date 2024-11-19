using Godot;
using System;

public partial class Interactable : RigidBody2D
{
    public static Player player;
    public string interactableText {
        get => ((Label)this.FindByName("Label")).Text;
        set => ((Label)this.FindByName("Label")).Text = value;
    }

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
