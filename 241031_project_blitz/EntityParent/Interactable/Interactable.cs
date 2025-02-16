using Godot;
using System;

public partial class Interactable : RigidBody2D, IInteractable
{
    public string interactableText {
        get => ((Label)this.FindByName("Label")).Text;
        set => ((Label)this.FindByName("Label")).Text = value;
    }

    protected float highlightValue = 0f;
    protected float highlightDelay = 0.4f;
    protected bool isHighlighted => Player.player == null ? false : (GlobalPosition - Player.player.GlobalPosition).Length() < interactableRange;
    
    public override void _EnterTree()
    {
        base._EnterTree();

        WorldManager.interactables.Add(this);
    }
    public override void _ExitTree()
    {
        base._ExitTree();

        WorldManager.interactables.Remove(this);
    }

    public override void _Process(double delta)
    {
        highlightValue += (isHighlighted ? (float)delta : -(float)delta) / highlightDelay;
        highlightValue = Math.Clamp(highlightValue, 0f, 1f);

        Control ui = FindChild("InteractionUI") as Control;
        ui.Modulate = new Color(1, 1, 1, highlightValue * 1);

        //Visibility Code
        VisibleProcess((float)delta);
        SetModulate();
    }

    //Visibility Code
    protected float visibility = 0f;
    void VisibleProcess(float delta)
    {
        const float getDelay = 0.1f, lossDelay = 5f;

        if (CheckLineOfSight() is Player)
            visibility += delta / getDelay;
        else
            visibility -= delta / lossDelay;

        visibility = visibility < 0f ? 0f : visibility > 1f ? 1f : visibility;

    }
    void SetModulate()
    {
        Modulate = new(1, 1, 1, visibility);
    }
    public Node CheckLineOfSight()
    {
        Player player = Player.player;

        if (player == null) return null;

        Vector2 from = GlobalPosition;           // Enemy 위치
        Vector2 to = player.GlobalPosition;      // Player 위치

        var spaceState = GetWorld2D().DirectSpaceState;

        var rayParams = new PhysicsRayQueryParameters2D
        {
            From = from,
            To = to,
            Exclude = new Godot.Collections.Array<Rid> { GetRid() } // Enemy 자신을 제외
        };

        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            var collider = (Node)result["collider"];

            return collider;
        }
        else
            return null;
    }

    public float interactableRange { get; set; } = 100f;

    public virtual void Interacted(Humanoid humanoid)
    {
        GD.Print("I'm interacted!");
        QueueFree();
    }
}
