using Godot;
using System;

public partial class Humanoid : RigidBody2D
{
    public Vector2 moveVec = new Vector2(0, 0);
    private float speed = 100f;  // 이동 속도 조절
    private float inertia = 0.15f; // 관성 계수 조절

    protected Weapon equippedWeapon;
    private float facingDir = 0f; // 현재 바라보는 방향 (라디안 단위)

    public override void _Ready()
    {
        // Collision 생성 및 설정
        var collision = new CollisionShape2D();
        collision.Shape = new CircleShape2D() { Radius = 50 };
        collision.Position = Vector2.Zero;
        AddChild(collision);

        GravityScale = 0;
        LockRotation = true;

        // 무기 초기 장착 (기본 무기 생성 및 장착)
        EquipWeapon(new Weapon(600, 10, 500)); // 임시로 무기 장착 (rpm, damage, muzzleSpeed 설정)


        SetCollisionLayerValue(1, true);
        SetCollisionMaskValue(1, true);
        SetCollisionMaskValue(2, true);
    }

    public void EquipWeapon(Weapon weapon)
    {
        // 무기를 장착하여 자식 노드에 추가
        if (equippedWeapon != null)
        {
            RemoveChild(equippedWeapon);
        }

        equippedWeapon = weapon;
        AddChild(equippedWeapon);

        // Weapon 위치 및 초기 회전 설정
        equippedWeapon.Position = Vector2.Zero;
        UpdateWeaponRotation();
    }

    public override void _PhysicsProcess(double delta)
    {
        // moveVec이 길이 1을 초과하면 normalize 처리
        if (moveVec.Length() > 1)
        {
            moveVec = moveVec.Normalized();
        }

        // 이동 처리: 관성을 적용해 이동
        LinearVelocity += moveVec * speed;
        LinearVelocity = LinearVelocity.Lerp(Vector2.Zero, inertia);

        // 무기 회전 업데이트
        UpdateWeaponRotation();
    }

    private void UpdateWeaponRotation()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Rotation = facingDir;
        }
    }

    public void SetFacingDirection(float direction)
    {
        // 바라보는 방향 설정
        facingDir = direction;
        UpdateWeaponRotation();
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 50, Colors.AliceBlue);
    }
}
