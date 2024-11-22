using Godot;
using System;

public partial class CameraManager : Camera2D
{
    [Export]
    public Humanoid target;

    public static CameraManager current;

    private Vector2 recoilOffset = Vector2.Zero;
    private float recoilRotation = 0f;

    public override void _Ready()
    {
        MakeCurrent();  // 이 카메라를 현재 활성화된 카메라로 설정
        current = this;
        IgnoreRotation= false;
    }
    
    public override void _Process(double delta)
    {
        if (target != null)
        {
            // target 위치와 realAimPoint 사이의 1:3 비율 위치
            Vector2 targetPosition = target.GlobalPosition.Lerp(target.virtualAimPoint, 0.25f);

            // 반동 효과를 반영한 카메라 위치 및 회전
            GlobalPosition = targetPosition + recoilOffset;
            Rotation = recoilRotation;
            //GD.Print(Rotation);

            // 반동 효과를 서서히 줄임
            recoilOffset = recoilOffset.Lerp(Vector2.Zero, 0.1f);
            recoilRotation = Mathf.Lerp(recoilRotation, 0f, 0.1f);
        }
    }

    // Humanoid 객체를 지정하는 메서드
    public void SetTarget(Humanoid newTarget)
    {
        target = newTarget;
    }


    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;
    // 반동 효과를 적용하는 메서드
    public void ApplyRecoil(float strength)
    {
        recoilOffset += Vector2.FromAngle(randFloat * 360f) * strength * randFloat;
        recoilRotation += randFloat * strength / 180f / (float)Math.PI;
    }
}
