using Godot;
using System;

public partial class Weapon : Node2D
{
    public WeaponStatus status;
    private float fireCooldown = 0f;

    public Weapon(int rpm, float damage, float muzzleSpeed)
    {
        status = new WeaponStatus(rpm, damage, muzzleSpeed);
    }

    public override void _Process(double delta)
    {
        // fireCooldown 감소 (발사 간격 관리)
        if (fireCooldown > 0f)
        {
            fireCooldown -= (float)delta;
        }
    }
    public void Shoot()
    {
        if (fireCooldown <= 0f)
        {
            // Projectile 인스턴스 생성 및 설정
            var projectile = new Projectile();
            projectile.Initialize(status.damage, status.muzzleSpeed, GlobalPosition + Vector2.FromAngle(GlobalRotation) * 50, GlobalRotation);
            GetParent().GetParent().AddChild(projectile);

            // 발사 후 fireCooldown 설정 (rpm을 기준으로 발사 간격 계산)
            fireCooldown = 60f / status.rpm;
        }
    }

    public override void _Draw()
    {
        base._Draw();
        DrawRect(new Rect2(new Vector2(50, 0), new Vector2(70, 30)), Colors.OrangeRed);
    }
}
