using Godot;
using System;

public partial class ShellEject : RigidBody2D
{
    private float bounceFactor = 0.9f;  // 튕기는 힘 (1에 가까울수록 많이 튕김)
    private float friction = 0.95f;     // 마찰력 (낮을수록 더 멀리 이동)
    private Vector2 initialVelocity;

    public override void _Ready()
    {
        // 초기 속도 설정 (배출 방향과 속도는 더 강하게 설정)
        initialVelocity = new Vector2((float)GD.RandRange(-500, 500), (float)GD.RandRange(500, -500)).Rotated(GlobalRotation);
        LinearVelocity = initialVelocity;

        // 탄피의 회전 속도 설정
        AngularVelocity = (float)GD.RandRange(-50, 50);

        // 중력 비활성화 (탑뷰 시점에서는 필요 없음)
        GravityScale = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity *= friction;
        
        // 속도가 매우 느려지면 탄피 제거
        if (LinearVelocity.Length() < 0.5f)
        {
            QueueFree();
        }
    }


    public override void _Draw()
    {
        DrawRect(new Rect2(-3, -1, 7, 2), Colors.Gold);
    }
}
