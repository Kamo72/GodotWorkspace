using Godot;
using Godot.NativeInterop;
using System;
using static AmmoStatus;

public partial class Weapon : Node2D
{
    /* Reference*/
    protected Humanoid master => GetParent<Humanoid>();
    public WeaponStatus status;
    public WeaponItem weaponItem;
    public int ammoInMagazine => weaponItem.ammoInMagazine;

    /* State */
    private float fireCooldown = 0f;    //격발 지연
    public bool isReloading = false; // 재장전 중 여부
    public bool isCharging = false; // 재장전 중 여부
    bool isReloadDisrupt = false;
    public bool isRealeased = false;
    /* Instantiate */
    public Weapon(WeaponItem weaponItem, String code)
    {
        this.weaponItem = weaponItem;

        status = WeaponLibrary.Get(code);

        var sprite = new Sprite2D();
        sprite.Position = Vector2.Zero;
        sprite.Scale = Vector2.One * 2.5f;
        sprite.Texture = ResourceLoader.Load<Texture2D>(weaponItem.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(sprite);
    }


    /* Process */
    public override void _Process(double delta)
    {
        // fireCooldown 감소 (발사 간격 관리)
        if (fireCooldown > 0f)
            fireCooldown -= (float)delta;
    }


    /* Callable Actions */
    public bool Shoot()
    {
        if (IsBusy()) return false;
        if (fireCooldown > 0f) return false;

        if (status.typeDt.selectorList[0] == SelectorType.SEMI && !isRealeased) return false;
        isRealeased = false;

        var ammoStatusNullable = weaponItem.DetonateAmmo();
        if (!ammoStatusNullable.HasValue)
        {
            if (weaponItem.ammoInMagazine > 0)
                Charging();

            return false;
        }
        weaponItem.FeedAmmo();

        var ammoStatus = ammoStatusNullable.Value;

        //투사체 생성
        for (int i = 0; i < ammoStatus.lethality.pellitCount; i++)
        {
            // Projectile 인스턴스 생성 및 설정
            var projectile = new Projectile();
            projectile.Initialize(
                ammoStatus,
                status.detailDt.muzzleVelocity * (1f + 10f/ 100f * randFloat),
                GlobalPosition + Vector2.FromAngle(GlobalRotation) * status.detailDt.muzzleDistance,
                GlobalRotation + status.aimDt.moa / 100f * randFloat
                );

            GetParent().GetParent().AddChild(projectile);
        }
        
        // 탄피 배출
        var shell = new ShellEject(); // ShellEject 장면 인스턴스화
        shell.Position = GlobalPosition;
        shell.Rotation = GlobalRotation;
        GetParent().GetParent().AddChild(shell);

        // 발사 후 fireCooldown 설정 (rpm을 기준으로 발사 간격 계산)
        fireCooldown = 60f / status.detailDt.roundPerMinute;

        return true;
    }

    public void Reload()
    {
        if (IsBusy()) return; // 이미 재장전 중인 경우
        isReloadDisrupt = false;
        MagazineType magazineType = status.typeDt.magazineType;

        switch (magazineType)
        {
            case MagazineType.MAGAZINE:
                ReloadMagazine();
                break;
            case MagazineType.INTERNAL:
                break;
            case MagazineType.TUBE:
                ReloadTube();
                break;
            case MagazineType.SYLINDER:
                break;

        }
    }
    async void ReloadMagazine()
    {
        var foundMag = weaponItem.FindMagazine(master.inventory);
        {
            //적절한 탄창을 찾지 못한 경우
            if (foundMag.HasValue == false)
            {
                GD.PushError("적절한 탄창을 찾지 못했습니다!");
                return;
            }

            isReloading = true;
        }

        //[timeout] Inspect - in
        await ToSignal(GetTree().CreateTimer(status.timeDt.inspectTime.Item1), "timeout");
        {
            var magBefore = weaponItem.DetachMagazine();
            if (magBefore != null)
            {
                if (master.inventory.TakeItem(magBefore))
                {
                    if (Player.player.mainUI.FindByName("InventoryPage") is InventoryPage ip)
                        ip.UpdateAllUI();

                    //[timeout] reloadTime - detach
                    await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item1), "timeout"); // 재장전 시간 대기
                }
                else
                    GD.PushError("교체되는 탄창 저장 실패");
            }

            GD.Print("재장전 중...");
        }

        if (!isReloadDisrupt)
        {
            //[timeout] reloadTime - getMag
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item2), "timeout"); // 재장전 시간 대기
            
            //탄창 장착
            weaponItem.AttachMagazine(foundMag.Value.node.item as Magazine);
        }

        if (!isReloadDisrupt)
        {
            //[timeout] reloadTime - attach
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item3), "timeout"); // 재장전 시간 대기
                //새 탄창을 Storage에서 제거
                bool res = foundMag.Value.storage.RemoveItem(foundMag.Value.node.item);
                if (res == false)
                    GD.PushError("새로 장착한 탄창이 Storage에서 제대로 제거되지 않았습니다!");

                if (Player.player.mainUI.FindByName("InventoryPage") is InventoryPage ipp)
                    ipp.UpdateAllUI();
        }

        //[timeout] Inspect - out
        await ToSignal(GetTree().CreateTimer(status.timeDt.inspectTime.Item2), "timeout");
        {
            isReloading = false;
            //약실이 비었다면 피드
            if (weaponItem.chamber == null)
                Charging();
        }

        GD.Print("재장전 종료.");
    }
    async void ReloadTube()
    {
        var foundAmmo = weaponItem.FindAmmo(master.inventory);
        Ammo ammo = null;
        {
            //적절한 탄창을 찾지 못한 경우
            if (foundAmmo.HasValue == false)
            {
                GD.PushError("적절한 탄을 찾지 못했습니다!");
                return;
            }

            isReloading = true;
        }

        //[timeout] Inspect - in
        await ToSignal(GetTree().CreateTimer(status.timeDt.inspectTime.Item1), "timeout");
        { 
        
        }
        //[timeout] reloadTime - ready
        await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item1), "timeout"); // 재장전 시간 대기
        {

        }

        if (!isReloadDisrupt)
        if (weaponItem.chamber == null) { 
            //[timeout] reloadTime - chamber load
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item2), "timeout"); // 재장전 시간 대기

            ammo = GetAmmoOne();
            if (ammo == null) throw new Exception("ReloadTube - GetAmmoOne() returned null!!");
            weaponItem.chamber = ammo;
            
            if (Player.player.mainUI.FindByName("InventoryPage") is InventoryPage ipp)
                ipp.UpdateAllUI();
        }



        while (weaponItem.magazine.ammoCount != weaponItem.magazine.magStatus.ammoSize)
        {
            if (isReloadDisrupt) break;
            ammo = GetAmmoOne();
            if (ammo == null) break;

            //[timeout] reloadTime - mag load
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item3), "timeout"); // 재장전 시간 대기
            {
                bool res = weaponItem.magazine.AmmoPush(ammo);
                if (!res) throw new Exception("ReloadTube - AmmoPush(ammo) failed, what happened!?");

                if (Player.player.mainUI.FindByName("InventoryPage") is InventoryPage ipp)
                    ipp.UpdateAllUI();
            }
        }

        //[timeout] Inspect - out
        await ToSignal(GetTree().CreateTimer(status.timeDt.inspectTime.Item2), "timeout");
        {
            isReloading = false;
        }

    }

    public async void Charging()
    {
        if (IsBusy()) return; // 이미 재장전 중인 경우
        if (weaponItem.ammoInMagazine == 0) return;

        isCharging = true;

        //[timeout] bolt - rewind
        await ToSignal(GetTree().CreateTimer(status.timeDt.boltTime.Item2), "timeout"); // 재장전 시간 대기
        {
            weaponItem.FeedAmmo();

            if (Player.player.mainUI.FindByName("InventoryPage") is InventoryPage ip)
                ip.UpdateAllUI();
        }
        isCharging = false;
    }

    public void DisruptReload() 
    {
        if (isReloading == false)
                return;
        isReloadDisrupt = true;
    }
    public void SetRealease()
    {
        isRealeased = true;
    }

    /* Comfort*/
    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;

    public bool IsBusy()
    {
        return isReloading || !master.isEquiped || isCharging;
    }

    Ammo GetAmmoOne()
    {
        var foundAmmo = weaponItem.FindAmmo(master.inventory);
        {
            //적절한 탄을 찾지 못한 경우
            if (foundAmmo.HasValue == false)
            {
                GD.PushError("적절한 탄을 찾지 못했습니다!");
                return null;
            }

            isReloading = true;
        }

        Ammo ammo = foundAmmo.Value.node.item as Ammo;

        if (ammo.stackNow == 1)
        {
            foundAmmo.Value.storage.RemoveItem(ammo);
            return ammo;
        }
        else 
        {
            return ammo.Split(1) as Ammo;
        }
    }

}
