using Godot;
using System;

public partial class Player : Humanoid
{
    public override void _Ready()
    {
        base._Ready();

        // 새로운 무기 생성 및 장착 (예시로 rpm=600, damage=15, muzzleSpeed=400)
        EquipWeapon(new Weapon(Weapon.Code.K2));
    }

    public bool isInventory => ((Control)GetTree().Root.FindByName("MainUi")).Visible;

    public override void _Process(double delta)
    {
        base._Process(delta);

        // WASD 키 입력에 따라 moveVec 조정
        moveVec = new Vector2(
            (Input.IsActionPressed("move_right") ? 1 : 0) - (Input.IsActionPressed("move_left") ? 1 : 0),
            (Input.IsActionPressed("move_back") ? 1 : 0) - (Input.IsActionPressed("move_forward") ? 1 : 0)
        );

        // moveVec가 0일 경우 정지
        if (moveVec.Length() == 0 || isInventory)
        {
            moveVec = new Vector2(0, 0);
        }

        // 마우스 위치를 얻어 Player의 위치로부터 방향 계산
        var mousePosition = GetGlobalMousePosition();
        var direction = mousePosition - GlobalPosition;
        var angle = direction.Angle();

        // Humanoid의 facingDir 업데이트
        SetFacingDirection(angle);


        // 발사 입력 감지 및 무기 발사 호출
        if (Input.IsActionPressed("fire") && !isInventory)
        {
            bool? isShoot = equippedWeapon?.Shoot();

            if (isShoot.HasValue)
                if(isShoot.Value)
                    OnShoot();
        }

        // 재장전 수행
        if (Input.IsActionPressed("reload") && !isInventory)
            equippedWeapon?.Reload();

        if (Input.IsActionJustPressed("inventory"))
        {
            Control mainUI = GetTree().Root.FindByName("MainUi") as Control;
            mainUI.Visible = !mainUI.Visible;
        }
    }


    public override void _Draw()
    {
        base._Draw();

        // 실제 조준점을 화면에 그리기
        DrawLine(equippedWeapon.Position, realAimPoint - GlobalPosition, Colors.Red, 1);
    }
}
