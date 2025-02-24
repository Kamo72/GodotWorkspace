using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Reflection;

public partial class Projectile : RayCast2D
{
    public AmmoStatus ammoStatus;
    public WeaponStatus weaponStatus;
    private float speed;
    public Vector2 velocity;
    public Vector2 startPos, aimPos;
    public float direction;
    public List<Humanoid> collidedList = new();
    bool isOverpene = false;

    public void Initialize(WeaponStatus weaponStatus, AmmoStatus ammoStatus, float speed, Vector2 startPos, Vector2 aimPos)
    {
        this.ammoStatus = ammoStatus;
        this.weaponStatus = weaponStatus;
        this.speed = speed;
        Position = startPos;

        // 속도 설정
        direction = (aimPos - startPos).Angle();
        
        this.startPos = startPos;
        this.aimPos = aimPos;

        velocity = new Vector2(speed, 0).Rotated(direction);
        TargetPosition = velocity.Normalized() * speed / 60; // Raycast 길이 설정 (속도에 따라 조정 가능)
        Enabled = true; // RayCast 활성화
        Modulate = new(1, 1, 1, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Raycast를 사용하여 투사체의 위치 업데이트 대신 충돌 체크
        ForceRaycastUpdate();
        
        if (IsColliding())
        {
            var collider = GetCollider();
            if (collider is Humanoid humanoid)
            {
                if (collidedList.Contains(humanoid))
                {
                    MoveAsVelocity(delta);
                }
                else {

                    GD.Print($"{humanoid.Name}에게 {ammoStatus.lethality.damage}의 피해를 입혔습니다.");
                    //humanoid.health.GetDamage(ammoStatus.lethality.damage);

                    Humanoid.Health.HitPart hitPart = isOverpene?
                        Humanoid.Health.HitPart.LIMB :
                        humanoid.health.GetHitPart(this, GetCollisionPoint());

                    GD.Print("hitPart : " + hitPart);

                    if (hitPart == Humanoid.Health.HitPart.NONE)
                    {
                        Position += velocity * (float)delta;
                    }
                    else
                    {
                        collidedList.Add(humanoid);
                        GetWoundEffect(humanoid, GetCollisionPoint());

                        bool overpene = humanoid.health.GetDamage(this, hitPart);
                        if (overpene)
                        {
                            isOverpene = true;
                            velocity *= 0.50f;
                            MoveAsVelocity(delta);
                        }
                        else
                            QueueFree(); // 충돌 시 Projectile 제거
                    }
                }
            }
            else if (collider is Wall wall)
            {
                //GD.Print($"{wall.Name}에 적중됨.");

                GetWoundEffect(wall, GetCollisionPoint());
                QueueFree(); // 충돌 시 Projectile 제거
            }
            else if (collider is Glass glass)
            {
                GetWoundEffect(glass, GetCollisionPoint());
                MoveAsVelocity(delta);
                //GD.Print($"{glass.Name}에 적중됨.");
                velocity *= 0.75f;
            }
        }
        else
        {
            MoveAsVelocity(delta);
            // Raycast 위치 업데이트 (투사체 이동 시뮬레이션)
        }


        VisibleProcess((float)delta);
        SetModulate();
    }

    //Visibility Code
    protected float visibility = 0f;
    void VisibleProcess(float delta)
    {
        const float getDelay = 0.05f, lossDelay = 0.05f;

        if (CheckLineOfSight() is Player)
            visibility += delta / getDelay;
        else
            visibility -= delta / lossDelay;

        visibility = visibility < 0f ? 0f : visibility > 1f ? 1f : visibility;

    }
    void SetModulate()
    {
        Modulate = new(1, 1, 1, visibility);
    }
    public Node CheckLineOfSight()
    {
        Player player = Player.player;

        if (player == null) return null;

        Vector2 from = GlobalPosition;           // Enemy 위치
        Vector2 to = player.GlobalPosition;      // Player 위치

        var spaceState = GetWorld2D().DirectSpaceState;

        var rayParams = new PhysicsRayQueryParameters2D
        {
            From = from,
            To = to,
        };

        var result = spaceState.IntersectRay(rayParams);

        if (result.Count > 0)
        {
            var collider = (Node)result["collider"];

            return collider;
        }
        else
            return null;
    }
    void MoveAsVelocity(double delta) {

        float airDrag = ammoStatus.adjustment.airDrag * (float)delta * 10f;
        velocity *= 1f-airDrag;
        Position += velocity * (float)delta;

        if (velocity.Length() < 1000f) QueueFree();
    }


    void GetWoundEffect(Node2D target, Vector2 collisionPoint)
    {
        Vector2 rand = Vector2.FromAngle((float)Random.Shared.NextDouble() * 360f) * 10f * (float)Random.Shared.NextDouble();

        Sprite2D sprite2D = new Sprite2D();
        sprite2D.Texture = ResourceLoader.Load("res://Asset/Particle/RadialAlphaGradient.png") as Texture2D;

        sprite2D.Modulate = Colors.Black;
        target.AddChild(sprite2D);
        sprite2D.GlobalPosition = collisionPoint + rand;
        sprite2D.GlobalScale = Vector2.One * 0.02f;
    }

    public override void _Draw()
    {
        if (!IsColliding())
            DrawLine(Vector2.Zero, TargetPosition, Colors.Yellow, 2);
    }
}
