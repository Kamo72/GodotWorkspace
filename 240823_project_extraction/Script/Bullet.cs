using Godot;
using System;

public partial class Bullet : Node2D
{
	RayCast2D rayCast2D => GetNode<RayCast2D>("RayCast2D");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (rayCast2D.IsColliding() == false) return;

        Node2D tObj = rayCast2D.GetCollider() as Node2D;
        if(tObj is Player player)
			GetParent().RemoveChild(this);
    }
	private void _on_timer_timeout() 
	{
		QueueFree();
	}
}
