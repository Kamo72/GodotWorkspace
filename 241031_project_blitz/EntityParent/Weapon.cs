using Godot;
using System;

public partial class Weapon : Node2D
{
    public WeaponStatus status;
    private float fireCooldown = 0f;

    private int ammoInMagazine;      // 현재 탄창 내의 탄약 수
    private int magazineSize = 30;   // 탄창 크기
    private float reloadTime = 2.0f; // 재장전 시간 (초)
    private bool isReloading = false; // 재장전 중 여부

    public Weapon(Code code)
    {
        status = GetStatByCode(code);
        ammoInMagazine = magazineSize; // 초기 탄창을 가득 채움
    }

    public override void _Process(double delta)
    {
        // fireCooldown 감소 (발사 간격 관리)
        if (fireCooldown > 0f)
        {
            fireCooldown -= (float)delta;
        }
    }

    public bool CanShoot()
    {
        return !isReloading && ammoInMagazine > 0;
    }

    public void Shoot()
    {
        if (CanShoot())
        {
            if (fireCooldown <= 0f)
            {
                ammoInMagazine--;
                // Projectile 인스턴스 생성 및 설정
                var projectile = new Projectile();
                projectile.Initialize(status.damage, status.muzzleSpeed, GlobalPosition + Vector2.FromAngle(GlobalRotation) * 50, GlobalRotation);
                GetParent().GetParent().AddChild(projectile);

                // 발사 후 fireCooldown 설정 (rpm을 기준으로 발사 간격 계산)
                fireCooldown = 60f / status.rpm;
            }

        }
        else {
            GD.Print("탄약이 없습니다! 재장전 필요.");
        }
    }

    public async void Reload()
    {
        if (isReloading || ammoInMagazine == magazineSize) return; // 이미 재장전 중이거나 탄창이 가득 차 있는 경우

        isReloading = true;
        GD.Print("재장전 중...");
        await ToSignal(GetTree().CreateTimer(reloadTime), "timeout"); // 재장전 시간 대기
        ammoInMagazine = magazineSize; // 탄창 가득 채움
        isReloading = false;
        GD.Print("재장전 완료! 탄창이 가득 찼습니다.");
    }

    public override void _Draw()
    {
        DrawRect(
            new Rect2(new Vector2(50, 0), new Vector2(70, 30)),
            isReloading? Colors.Cyan : Colors.OrangeRed
            );
    }
}
