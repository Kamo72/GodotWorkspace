using Godot;
using Microsoft.VisualBasic;
using System;
using static Humanoid;

public partial class Player : Humanoid
{
    public static Player player;
    public override void _Ready()
    {
        base._Ready();

        player = this;
        Interactable.player = this;

        inventory.firstWeapon.DoEquipItem(new MP_155());
        inventory.secondWeapon.DoEquipItem(new MP_133());

        inventory.backpack.DoEquipItem(new TestBackpack());
        inventory.sContainer.DoEquipItem(new TestContainer());

        inventory.TakeItem(new TestItem());
        inventory.TakeItem(new TestItemSmall());
        inventory.TakeItem(new TestItemSmall());
        inventory.TakeItem(new M855 { stackNow = 100 });
        inventory.TakeItem(new M855 { stackNow = 50 });
        inventory.TakeItem(new M855 { stackNow = 25 });
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new AR15_StanagMag_30(typeof(M855)));
        inventory.TakeItem(new G12_Grizzly { stackNow = 20 });
        inventory.TakeItem(new G12_BuckShot_7p5 { stackNow = 20 });
    }

    public Control mainUI => GetTree().Root.FindByName("MainUi") as Control;

    public override void _Process(double delta)
    {
        base._Process(delta);

        // WASD 키 입력에 따라 moveVec 조정
        moveVec = new Vector2(
            (Input.IsActionPressed("move_right") ? 1 : 0) - (Input.IsActionPressed("move_left") ? 1 : 0),
            (Input.IsActionPressed("move_back") ? 1 : 0) - (Input.IsActionPressed("move_forward") ? 1 : 0)
        );

        // moveVec가 0 또는 인벤토리를 보는 중 일 경우 정지 
        if (moveVec.Length() == 0 || isInventory)
            moveVec = new Vector2(0, 0);

        // 마우스 위치를 얻어 Player의 위치로부터 방향 계산
        var mousePosition = GetGlobalMousePosition();
        var direction = mousePosition - GlobalPosition;
        var angle = direction.Angle();

        // 바라보는 방향 설정
        facingDir = angle;

        mainUI.Visible = isInventory;


        // 발사 입력 감지 및 무기 발사 호출
        if (Input.IsActionPressed("fire") && !isInventory)
        {
            if (equippedWeapon == null) return;
            if (equippedWeapon.IsBusy())
            {
                if(equippedWeapon.isReloading)
                    equippedWeapon.DisruptReload();
                return;
            }

            bool isShoot = equippedWeapon.Shoot();

            if (isShoot)
            {
                OnShoot();
                CameraManager.current.ApplyRecoil(equippedWeapon.status.aimDt.strength);
            }
        }
        if (Input.IsActionJustReleased("fire"))
                equippedWeapon?.SetRealease();

        // 재장전 수행
        if (Input.IsActionPressed("reload") && !isInventory)
            equippedWeapon?.Reload();

        if (Input.IsActionJustPressed("inventory"))
        {
            isInventory = !isInventory;

            Control mainUI = GetTree().Root.FindByName("MainUi") as Control;
            mainUI.Visible = !mainUI.Visible;
        }

        if (Input.IsActionJustPressed("interact"))
        {
            if (interactables.Count > 0)
                interactables[0].Interacted(this);
        }
        if (Input.IsActionJustPressed("firstWeapon"))
        {
            if (inventory.firstWeapon.item != null)
                targetEquip = inventory.firstWeapon;
        }
        if (Input.IsActionJustPressed("secondWeapon"))
        {
            if (inventory.secondWeapon.item != null)
                targetEquip = inventory.secondWeapon;
        }
        if (Input.IsActionJustPressed("subWeapon"))
        {
            if (inventory.subWeapon.item != null)
                targetEquip = inventory.subWeapon;
        }
    }


    public override void _Draw()
    {
        base._Draw();

        // 실제 조준점을 화면에 그리기
        if(equippedWeapon != null)
            DrawLine(equippedWeapon.Position, (realAimPoint - GlobalPosition).Rotated(equippedWeapon.Rotation - facingDir), Colors.Red, 1);
    }
}
