using Godot;
using Microsoft.VisualBasic;
using System;

public partial class Player : Humanoid
{
    public static Player player;
    public override void _Ready()
    {
        //정적 변수 설정
        player = this;

        //사전 준비
        base._Ready();
        intelligence = new PlayerController(this);
        CameraManager.current.SetTarget(this);


        //테스트용 피통 늘리기
        healthNow = 2000f;

        //테스트용 아이템 획득
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

    public override void _Process(double delta)
    {
        base._Process(delta);

        UiMain.instance.Visible = isInventory;
    }

    public override void _Draw()
    {
        base._Draw();

        // 실제 조준점을 화면에 그리기
        if(equippedWeapon != null)
            DrawLine(equippedWeapon.Position, (realAimPoint - GlobalPosition).Rotated(equippedWeapon.Rotation - facingDir), Colors.Red, 1);
    }

    protected override void Dispose(bool disposing)
    {
        player = null;
        base.Dispose(disposing);
    }
}


public class PlayerController : Humanoid.Intelligence
{
    public PlayerController(Humanoid humanoid) : base(humanoid) { }

    public override void Process(float delta)
    {
        // WASD 키 입력에 따라 moveVec 조정
        vectorMap["MoveVec"] = new Vector2(
            (Input.IsActionPressed("move_right") ? 1 : 0) - (Input.IsActionPressed("move_left") ? 1 : 0),
            (Input.IsActionPressed("move_back") ? 1 : 0) - (Input.IsActionPressed("move_forward") ? 1 : 0)
        );

        // moveVec가 0 또는 인벤토리를 보는 중 일 경우 정지 
        if (vectorMap["MoveVec"].Length() == 0 || master.isInventory)
            vectorMap["MoveVec"] = new Vector2(0, 0);


        vectorMap["AimPos"] = master.GetGlobalMousePosition();

        commandMap["Reload"] = Input.IsActionJustPressed("reload") && !master.isInventory;
        commandMap["Inventory"] = Input.IsActionJustPressed("inventory");
        commandMap["Interact"] = Input.IsActionJustPressed("interact") && !master.isInventory;
        commandMap["FirstWeapon"] = Input.IsActionJustPressed("firstWeapon") && !master.isInventory;
        commandMap["SecondWeapon"] = Input.IsActionJustPressed("secondWeapon") && !master.isInventory;
        commandMap["SubWeapon"] = Input.IsActionJustPressed("subWeapon") && !master.isInventory;
        commandMap["Fire"] = Input.IsActionPressed("fire") && !master.isInventory;
        commandMap["FireReleased"] = Input.IsActionJustReleased("fire");

    }
}