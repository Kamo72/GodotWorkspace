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
		Fire();
    }




	public void Fire()
    {
        Projectile proj = ResourceLoader.Load<PackedScene>("res://Prefab/projectile.tscn").Instantiate() as Projectile;
        GetTree().Root.AddChild(proj);
    }
}
