using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class Wall : RigidBody2D
{
    CollisionShape2D collision;
    RectangleShape2D shape => collision.Shape as RectangleShape2D;

    public override void _Ready()
    {
        // 충돌 영역 설정
        collision = new CollisionShape2D();
        collision.Shape = new RectangleShape2D() { Size = new Vector2(50, 50) };
        AddChild(collision);

        // Wall 전용 레이어와 마스크 설정
        SetCollisionLayerValue(3, true); // Wall 레이어 설정
        SetCollisionMaskValue(1, true);  // Humanoid와 충돌 감지
        SetCollisionMaskValue(2, true);  // Projectile과 충돌 감지

        GravityScale = 0;
        LockRotation = true;
        Freeze = true;
    }

    public override void _Draw()
    {
        DrawRect(shape.GetRect(), Colors.DarkKhaki, true);
    }
}