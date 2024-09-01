using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float speed = 300;

	[Export]
	public PackedScene bullet;

    private Vector2 syncPos = Vector2.Zero;
    private float syncRotation = 0f;

    public override void _PhysicsProcess(double delta)
    {
        if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
        {
            Vector2 velocity = Velocity;

            if (Input.IsActionJustPressed("fire"))
            {
                Rpc("Fire", GetNode<Node2D>("GunRotation").RotationDegrees, GetNode<Node2D>("GunRotation/BulletSpawn").GlobalPosition);
            }

            Node2D gun = GetNode<Node2D>("GunRotation");
            float rotation = (GetGlobalMousePosition() - gun.GlobalPosition).Angle();
            gun.GlobalRotation = rotation;

            Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            if (direction != Vector2.Zero)
            {
                velocity = direction * speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(velocity.X, 0, speed);
                velocity.Y = Mathf.MoveToward(velocity.Y, 0, speed);
            }

            Velocity = velocity;
            MoveAndSlide();
            syncPos = GlobalPosition;
            syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
        }
        else 
        {
            GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
            GetNode<Node2D>("GunRotation").RotationDegrees = Mathf.Lerp( GetNode<Node2D>("GunRotation").RotationDegrees, syncRotation, .1f );
        }
    }



    public override void _Ready()
	{
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
	}

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void Fire(float rot, Vector2 pos)
    {
        Node2D b = bullet.Instantiate<Node2D>();
        b.RotationDegrees = rot;
        b.GlobalPosition = pos;
        GetTree().Root.AddChild(b);
    }

    public void SetupPlayer(string name) 
    {
        GetNode<Label>("Label").Text = name;
    }
}
