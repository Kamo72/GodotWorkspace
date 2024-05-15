using Godot;
using System;

public partial class Player : Humanoid
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();

        InputProcess = (thisObj, delta) =>
        {
            //Get Input
            thisObj.moveValue = Vector2.Zero;

            if (Input.IsKeyPressed(Key.A))
                thisObj.moveValue += new Vector2(-1f, +0f);
            if (Input.IsKeyPressed(Key.D))
                thisObj.moveValue += new Vector2(+1f, +0f);
            if (Input.IsKeyPressed(Key.W))
                thisObj.moveValue += new Vector2(+0f, -1f);
            if (Input.IsKeyPressed(Key.S))
                thisObj.moveValue += new Vector2(+0f, +1f);
        };

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        base._Process(delta);
        hands.GrabWeapon(GetTree().FindByName("Weapon") as Weapon );
	}
}
