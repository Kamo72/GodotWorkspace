using Godot;
using System;

public partial class Weapon : Node2D
{
    public WeaponStatus status;
    private float fireCooldown = 0f;

    private int ammoInMagazine;      // 현재 탄창 내의 탄약 수
    public bool isReloading = false; // 재장전 중 여부

    public Weapon(Code code)
    {
        status = GetStatByCode(code);
        ammoInMagazine = status.magSize; // 초기 탄창을 가득 채움

        var sprite = new Sprite2D();
        sprite.Position = Vector2.Zero;
        sprite.Scale = new Vector2(0.7f, 0.7f);
        sprite.Texture = ResourceLoader.Load<Texture2D>(status.resPath);
        AddChild(sprite);

    }

    public override void _Process(double delta)
    {
        // fireCooldown 감소 (발사 간격 관리)
        if (fireCooldown > 0f)
            fireCooldown -= (float)delta;
    }

    public bool CanShoot()
    {
        return !isReloading && ammoInMagazine > 0;
    }

    public bool Shoot()
    {
        if (CanShoot())
        {
            if (fireCooldown <= 0f)
            {
                ammoInMagazine--;
                // Projectile 인스턴스 생성 및 설정
                var projectile = new Projectile();
                projectile.Initialize(
                    status.damage,
                    status.muzzleSpeed,
                    GlobalPosition + Vector2.FromAngle(GlobalRotation) * status.muzzleDistance,
                    GlobalRotation
                    );

                GetParent().GetParent().AddChild(projectile);

                // 탄피 배출
                var shell = new ShellEject(); // ShellEject 장면 인스턴스화
                shell.Position = GlobalPosition;
                shell.Rotation = GlobalRotation;
                GetParent().GetParent().AddChild(shell);

                // 발사 후 fireCooldown 설정 (rpm을 기준으로 발사 간격 계산)
                fireCooldown = 60f / status.rpm;

                return true;
            }

        }
        else {
            GD.Print("탄약이 없습니다! 재장전 필요.");
        }

        return false;
    }

    public async void Reload()
    {
        if (isReloading || ammoInMagazine == status.magSize) return; // 이미 재장전 중이거나 탄창이 가득 차 있는 경우

        isReloading = true;
        QueueRedraw();
        GD.Print("재장전 중...");

        await ToSignal(GetTree().CreateTimer(status.reloadTime), "timeout"); // 재장전 시간 대기
        
        ammoInMagazine = status.magSize; // 탄창 가득 채움
        isReloading = false;
        QueueRedraw();
        GD.Print("재장전 완료! 탄창이 가득 찼습니다.");
    }

}
