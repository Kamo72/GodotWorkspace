using Godot;
using Godot.NativeInterop;
using System;
using static AmmoStatus;

public partial class Weapon : HandAnimation
{
    /* Reference*/
    public Sprite2D spriteMag => this.FindByName("SpriteMag") as Sprite2D;
    public PointLight2D light2D => this.FindByName("PointLight2D") as PointLight2D;

    public WeaponStatus status;
    public WeaponItem weaponItem;
    public int ammoInMagazine => weaponItem.ammoInMagazine;

    /* State */
    private float fireCooldown = 0f;    //격발 지연
    public bool isReloading = false; // 재장전 중 여부
    public bool isCharging = false; // 재장전 중 여부
    bool isReloadDisrupt = false; //재장전 중단 요청 입력됨
    public bool isRealeased = false; //단발 전용 트리거

    /* Instantiate */
    public Weapon(WeaponItem weaponItem, String code)
    {
        this.weaponItem = weaponItem;

        status = WeaponLibrary.Get(code);

        var sprite = new Sprite2D();
        sprite.Name = "SpriteMag";
        sprite.Scale = Vector2.One * 2.5f;
        sprite.Texture = ResourceLoader.Load<Texture2D>(weaponItem.magazine.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(sprite);
        sprite.Position = status.attachDt.magAttachPos * 2.5f;

        sprite = new Sprite2D();
        sprite.Scale = Vector2.One * 2.5f;
        sprite.Texture = ResourceLoader.Load<Texture2D>(weaponItem.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(sprite);
        sprite.Position = Vector2.Zero;

        var light = new PointLight2D();
        light.Name = "PointLight2D";
        light.Position = new(status.detailDt.muzzleDistance, 0f);
        light.TextureScale = 4.0f;
        light.Energy = 2f;
        light.Color = new Color(Colors.LightYellow, 0f);
        light.Texture = ResourceLoader.Load<Texture2D>("res://Asset/Particle/RadialAlphaGradient.png");
        light.TextureFilter = TextureFilterEnum.Nearest;
        light.ShadowEnabled = true;
        AddChild(light);
    }

    /* Process */
    public override void _Process(double delta)
    {
        // fireCooldown 감소 (발사 간격 관리)
        if (fireCooldown > 0f)
            fireCooldown -= (float)delta;

        light2D.Color = new Color(light2D.Color, light2D.Color.A * 0.5f);

        //탄창 애니메이션
        if (magPosIncreasing && magPosValue < 1f ||
            !magPosIncreasing && magPosValue > 0f)
        {
            magPosValue += (magPosIncreasing ? +1 : -1) * (float)delta / magPosTime;
            magPosTime -= (float)delta;
            SetMagazinePos(magPosValue);
        }
        else
        {
            magPosValue = magPosIncreasing ? 1f : 0f;
        }
    }

    /* Callable Actions */
    public bool Shoot()
    {
        if (IsBusy()) return false;
        if (fireCooldown > 0f) return false;
        if (master.movement.sprintValue > 0.01f) return false;

        if (status.typeDt.selectorList[0] == SelectorType.SEMI && !isRealeased) return false;
        isRealeased = false;

        var ammoStatusNullable = weaponItem.DetonateAmmo();
        if (!ammoStatusNullable.HasValue)
        {
            if (weaponItem.ammoInMagazine > 0)
                Charging();

            return false;
        }

        //GD.Print("status.typeDt.mechanismType : " + status.typeDt.mechanismType);

        if (status.typeDt.mechanismType != MechanismType.MANUAL_RELOAD &&
            status.typeDt.mechanismType != MechanismType.NONE)
            weaponItem.FeedAmmo();
        else
            Charging();

        var ammoStatus = ammoStatusNullable.Value;

        //투사체 생성
        Vector2 startPos = GlobalPosition + Vector2.FromAngle(GlobalRotation) * status.detailDt.muzzleDistance;
        for (int i = 0; i < ammoStatus.lethality.pellitCount; i++)
        {
            Vector2 targetPos = master.realAimPoint
            + Vector2.FromAngle(randFloat /** 360 * MathF.PI*/) * status.aimDt.moa / 100f * randFloat;

            //GD.Print("targetPos : " + targetPos);
            // Projectile 인스턴스 생성 및 설정
            var projectile = new Projectile();
            projectile.Initialize(
                status,
                ammoStatus,
                status.detailDt.muzzleVelocity * (1f + 10f/ 100f * randFloat),
                startPos,
                targetPos
                );

            GetParent().GetParent().AddChild(projectile);
        }

        // 탄피 배출
        if (status.typeDt.mechanismType != MechanismType.MANUAL_RELOAD &&
            status.typeDt.mechanismType != MechanismType.NONE)
            EffectShellEject();

        // 발사 후 fireCooldown 설정 (rpm을 기준으로 발사 간격 계산)
        fireCooldown = 60f / status.detailDt.roundPerMinute;
        
        //총구 섬광
        light2D.Color = new Color(light2D.Color, 1f);

        //소리 발생
        Sound.MakeSelf(master, GlobalPosition, 2000f, 1f, GetSoundRscShot());

        return true;
    }

    public void Reload(Item item = null)
    {
        if (IsBusy()) return; // 이미 재장전 중인 경우
        isReloadDisrupt = false;
        MagazineType magazineType = status.typeDt.magazineType;

        switch (magazineType)
        {
            case MagazineType.MAGAZINE:
                ReloadMagazine(item as Magazine);
                break;
            case MagazineType.INTERNAL:
                break;
            case MagazineType.TUBE:
                ReloadTube(item as Ammo);
                break;
            case MagazineType.SYLINDER:
                break;

        }
    }
    async void ReloadMagazine(Magazine magazine = null)
    {
        
        var foundMag = weaponItem.FindMagazine(master.inventory);
        if(magazine == null)
        {
            //적절한 탄창을 찾지 못한 경우
            if (foundMag.HasValue == false)
            {
                GD.PushError("적절한 탄창을 찾지 못했습니다!");
                return;
            }

            magazine = foundMag.Value.node.item as Magazine;
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
                    InventoryPage.instance?.UpdateAllUI();

                    //[timeout] reloadTime - detach
                    SetMagazineRoutine(false, status.timeDt.reloadTime.Item1);
                    Sound.MakeSelf(master, GlobalPosition, 200f, 0.1f, GetSoundRscClipOut());
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
            Sound.MakeSelf(master, GlobalPosition, 200f, 0.1f, GetSoundRscClipIn());
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item2), "timeout"); // 재장전 시간 대기


            //탄창 장착
            weaponItem.AttachMagazine(magazine as Magazine);
        }

        if (!isReloadDisrupt)
        {
            //[timeout] reloadTime - attach
            SetMagazineRoutine(true, status.timeDt.reloadTime.Item3);
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item3), "timeout"); // 재장전 시간 대기
                //새 탄창을 Storage에서 제거
                bool res = magazine.onStorage.RemoveItem(magazine);
                if (res == false)
                    GD.PushError("새로 장착한 탄창이 Storage에서 제대로 제거되지 않았습니다!");

            InventoryPage.instance?.UpdateAllUI();
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
    async void ReloadTube(Ammo ammo = null)
    {
        Ammo tempAmmo = null;
        var foundAmmo = weaponItem.FindAmmo(master.inventory);
        if (ammo == null)
        {
            //적절한 탄창을 찾지 못한 경우
            if (foundAmmo.HasValue == false)
            {
                GD.PushError("적절한 탄을 찾지 못했습니다!");
                return;
            }
            ammo = foundAmmo.Value.node.item as Ammo;
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
                Sound.MakeSelf(master, GlobalPosition, 200f, 0.1f, GetSoundRscClipIn());
                await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item2), "timeout"); // 재장전 시간 대기

            tempAmmo = GetAmmoOne(ammo);
            if (tempAmmo == null) throw new Exception("ReloadTube - GetAmmoOne() returned null!!");
            weaponItem.chamber = tempAmmo;

            InventoryPage.instance?.UpdateAllUI();
        }

        while (weaponItem.magazine.ammoCount != weaponItem.magazine.magStatus.ammoSize)
        {
            if (isReloadDisrupt) break;
            try
            {
                tempAmmo = GetAmmoOne(ammo);
                if (tempAmmo == null) throw new Exception("ReloadTube() tempAmmo == null");
            }
            catch (Exception ex)
            {
                var newFoundAmmo = weaponItem.FindAmmo(master.inventory);
                if (!newFoundAmmo.HasValue) break;

                ammo = newFoundAmmo.Value.node.item as Ammo;
                tempAmmo = GetAmmoOne(ammo);
            }

            //[timeout] reloadTime - mag load
            Sound.MakeSelf(master, GlobalPosition, 200f, 0.1f, GetSoundRscClipOut());
            await ToSignal(GetTree().CreateTimer(status.timeDt.reloadTime.Item3), "timeout"); // 재장전 시간 대기
            {
                bool res = weaponItem.magazine.AmmoPush(tempAmmo);
                if (!res) throw new Exception("ReloadTube - AmmoPush(ammo) failed, what happened!?");


                InventoryPage.instance?.UpdateAllUI();
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

            if (status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD || 
                status.typeDt.mechanismType == MechanismType.NONE)
                EffectShellEject();
            
            InventoryPage.instance?.UpdateAllUI();
        }
        Sound.MakeSelf(master, GlobalPosition, 200f, 0.1f, GetSoundRscArming());
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

    bool magPosIncreasing = true;
    float magPosTime = 1f, magPosValue = 1f;
    void SetMagazineRoutine(bool increasing, float time) 
    {
        magPosIncreasing = increasing;
        magPosTime = time;
    }
    void SetMagazinePos(float reloadingRatio) 
    {
        reloadingRatio = Mathf.Pow(reloadingRatio - 0.5f, 3) * 4f + 0.5f;

        spriteMag.Position =
            ((status.attachDt.magAttachPos * 2.5f) * reloadingRatio +
            -Position * Scale.Y * (1f - reloadingRatio) +
            new Vector2(0f, 10f) * ((0.5f - Math.Abs(reloadingRatio - 0.5f)) * 2f));

        spriteMag.Rotation =
           (0f * reloadingRatio +
            -45f * (1f - reloadingRatio) +
            75f * ((0.5f - Math.Abs(reloadingRatio - 0.5f)) * 2f))
            / 180f * (float)Math.PI;

        spriteMag.Modulate = new Color(Colors.White, Math.Min(reloadingRatio * 5f, 1f));
    }

    void EffectShellEject() 
    {
        var shell = new ShellEject(); // ShellEject 장면 인스턴스화
        shell.Position = GlobalPosition;
        shell.Rotation = GlobalRotation;
        GetParent().GetParent().AddChild(shell);
        Sound.MakeSelf(master, GlobalPosition, 400f, 0.4f, GetSoundRscShell());
    }

    /* Comfort*/
    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;

    public bool IsBusy()
    {
        return isReloading || !master.isEquiped || isCharging;
    }

    Ammo GetAmmoOne(Ammo ammo = null)
    {
        var foundAmmo = weaponItem.FindAmmo(master.inventory);
        if(ammo == null)
        {
            //적절한 탄을 찾지 못한 경우
            if (foundAmmo.HasValue == false)
            {
                GD.PushError("적절한 탄을 찾지 못했습니다!");
                return null;
            }
            ammo = foundAmmo.Value.node.item as Ammo;
            isReloading = true;
        }


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

    string GetSoundRscShell() => 
    $"res://Asset/SFX-Firearm/KompositeGun/BULLETS/Shell/Shell_Short_0{Random.Shared.Next() % 2 + 1}_SFX.wav";
    string GetSoundRscShot() => 
    $"res://Asset/SFX-Firearm/KompositeGun/GUN/Pistol_01_Fire_0{Random.Shared.Next() % 5 + 1}_SFX.wav";
    string GetSoundRscClipIn() =>
    $"res://Asset/SFX-Firearm/KompositeGun/GUN/Handling_Gun_01_Clip_In_SFX.wav";
    string GetSoundRscClipOut() =>
    $"res://Asset/SFX-Firearm/KompositeGun/GUN/Handling_Gun_01_Clip_Out_SFX.wav";
    string GetSoundRscArming() =>
    $"res://Asset/SFX-Firearm/KompositeGun/GUN/Handling_Gun_01_Arming_SFX.wav";

}
