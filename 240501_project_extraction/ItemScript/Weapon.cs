using Godot;
using System;

public partial class Weapon : Node2D
{
	public Sprite2D sprite;
	public float barrelLength;
	public PackedScene bullet;

	public override void _Ready()
	{
        sprite = FindChild("Sprite2D") as Sprite2D;
		barrelLength = GetMeta("BarrelLength").As<float>();

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Process(double delta)
	{
		base._Process(delta);
    }


    float cooldown = 0;
    public override void _PhysicsProcess(double delta)
    {
		GD.Print("processed!");
		if(Input.IsMouseButtonPressed(MouseButton.Left)){
			if()
			Fire();
			GD.Print("fired!");
			
		}
		if(Input.IsMouseButtonPressed(MouseButton.Right)){
			Fire();
			GD.Print("fired!");
		}

        base._PhysicsProcess(delta);
    }




    public void Fire()
    {
        Projectile proj = ResourceLoader.Load<PackedScene>("res://Prefab/projectile.tscn").Instantiate() as Projectile;
		proj.GlobalPosition = GlobalPosition + barrelLength * Vector2.FromAngle(GlobalRotation);
		proj.GlobalRotation = GlobalRotation;
		proj.LinearVelocity = Vector2.FromAngle(GlobalRotation) * 20f;
        GetTree().Root.AddChild(proj);
    }
}
