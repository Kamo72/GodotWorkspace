using Godot;
using System;

public partial class CameraManager : Camera2D
{
    private Node2D target;
    private float shakePower = 0f;
    private float shakeTimer = 0f;
    private Random random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        target = GetTree().Root.FindByName("Player") as Node2D;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (target != null)
        {
            Position = target.Position + ShakeProcess((float)delta);
        }
        else
        {
            GD.Print("CameraManager: target == null!");
        }
    }

    // 흔들림 효과를 계산하여 반환
    private Vector2 ShakeProcess(float delta)
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= delta;
            float shakeAmount = shakePower * (shakeTimer / shakePower); // 남은 시간에 따라 흔들림이 줄어듭니다.
            float offsetX = (float)(random.NextDouble() * 2 - 1) * shakeAmount;
            float offsetY = (float)(random.NextDouble() * 2 - 1) * shakeAmount;
            return new Vector2(offsetX, offsetY);
        }
        return Vector2.Zero;
    }

    // 외부에서 흔들림을 시작할 때 호출하는 함수
    public void GetShake(float power, float duration = 0.5f)
    {
        shakePower = power;
        shakeTimer = duration;
    }
}
