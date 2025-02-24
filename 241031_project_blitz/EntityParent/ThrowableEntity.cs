using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class ThrowableEntity : RigidBody2D
{
    public bool isImpact = false;
    public float fuseRemain = 10f;
    public string throwCode;
    public ThrowStatus throwStatus;

    public float height;
    public float gravity;

    public ThrowableEntity(string throwCode, Vector2 tPos, float speed) 
    {
        // Collision 생성 및 설정
        var collision = new CollisionShape2D();
        collision.Shape = new CircleShape2D() { Radius = 5 };
        collision.Position = Vector2.Zero;
        AddChild(collision);

        GravityScale = 0;

        SetCollisionLayerValue(1, true); // Humanoid 레이어
        SetCollisionLayerValue(3, true); // Wall 레이어
        SetCollisionMaskValue(1, false);
        SetCollisionMaskValue(2, false);  // Projectile과 충돌 감지
        SetCollisionMaskValue(3, false);  // Wall과 충돌 감지

        this.throwCode = throwCode;
        SetValue(throwCode);
        height = 10f;
        gravity = -2f;
    }

    void SetValue(string code) 
    {
    
    }


    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    void Impact() { }
    void Explosive() { }

}