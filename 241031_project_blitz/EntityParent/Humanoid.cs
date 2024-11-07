using Godot;
using System;
using System.Collections.Generic;

public partial class Humanoid : RigidBody2D
{
    public Vector2 moveVec = new Vector2(0, 0);
    private float speed = 100f;  // 이동 속도 조절
    private float inertia = 0.15f; // 관성 계수 조절

    protected Weapon equippedWeapon;
    protected float facingDir = 0f; // 현재 바라보는 방향 (라디안 단위)

    public float healthNow;
    public float healthMax;

    // 조준점 관련 변수
    public Vector2 virtualAimPoint;
    protected Vector2 delayedRecoilVec = Vector2.Zero;
    private Vector2 recoilVec = Vector2.Zero;
    private float aimLength => (virtualAimPoint - GlobalPosition).Length();

    private static FastNoiseLite noise = new FastNoiseLite();

    public Vector2 realAimPoint { get
        {
            Vector2 aimPoint = virtualAimPoint;

            aimPoint += (recoilVec * aimLength / 1000 - GlobalPosition)
                .Rotated(recoilVec.Length() * randFloat / 5000);
            
            aimPoint += GlobalPosition;

            float aimStatble = 50f; // 조준 안정 계수

            float radius = aimStatble * (1f + recoilVec.Length() / 100f);
            float noiseX = noise.GetNoise2D(aimStableTime * 10f, 6945) * radius;
            float noiseY = noise.GetNoise2D(aimStableTime * 10f + 100, 1235) * radius;
            aimPoint += new Vector2(noiseX, noiseY)
                * aimLength / 1000;

            return aimPoint;
        } }


    protected float delayedRecoilRatio = 0.9f; //지연 반동 계수
    protected float aimStableTime = 0f;

    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;

    protected Vector2 handPos;
    protected float handRot;

    public override void _Ready()
    {
        healthMax = 100f; // 최대 체력 초기화
        healthNow = healthMax; // 현재 체력을 최대 체력으로 설정

        // Collision 생성 및 설정
        var collision = new CollisionShape2D();
        collision.Shape = new CircleShape2D() { Radius = 35 };
        collision.Position = Vector2.Zero;
        AddChild(collision);

        GravityScale = 0;
        LockRotation = true;

        // 무기 초기 장착 (기본 무기 생성 및 장착)
        EquipWeapon(new Weapon(Weapon.Code.K2)); // 임시로 무기 장착 (rpm, damage, muzzleSpeed 설정)


        SetCollisionMaskValue(1, true);
        SetCollisionLayerValue(1, true); // Humanoid 레이어
        SetCollisionMaskValue(2, true);  // Projectile과 충돌 감지
        SetCollisionMaskValue(3, true);  // Wall과 충돌 감지


        // 초기 가상 조준점 설정
        virtualAimPoint = GetGlobalMousePosition();
        handPos = GlobalPosition;
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

    public override void _Process(double delta)
    {
        QueueRedraw();

        // 가상 조준점 업데이트
        virtualAimPoint = virtualAimPoint.Lerp(GetGlobalMousePosition(), 0.3f);

        float recoilRecover = 0.1f; //반동 회복
        // 실제 반동 벡터 업데이트
        recoilVec += delayedRecoilVec * (1f - delayedRecoilRatio);
        recoilVec *= (1f - recoilRecover);

        // 지연 반동 벡터 업데이트
        delayedRecoilVec *= delayedRecoilRatio;

        //조준 안정
        aimStableTime += (float)delta * (1 + recoilVec.Length()/100) * 3f;

        InteractionProcess();
    }

    private void UpdateWeaponRotation()
    {
        const float handDistance = 80f;

        if (equippedWeapon != null)
        {
            float realDistance = (realAimPoint - GlobalPosition).Length();
            float allowedDistance = equippedWeapon.status.muzzleDistance + handDistance + 20f;
            float minDistance = equippedWeapon.status.muzzleDistance + 20f;

            if (realDistance < minDistance)
            {
                //최소한의 길이 보장 X 총구 꺾임
                handPos = handPos.Lerp(Vector2.FromAngle(facingDir) * handDistance / 4f, 0.1f);
                handRot = Mathf.LerpAngle(handRot, (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.5f * 3.14f, 0.1f);
            }
            else if (equippedWeapon.isReloading)
            {//최소한의 길이 보장 X 총구 꺾임
                handPos = handPos.Lerp(Vector2.FromAngle(facingDir) * handDistance / 2f, 0.1f);
                handRot = Mathf.LerpAngle(handRot, (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.25f * 3.14f, 0.1f);

            }
            else if (realDistance < allowedDistance)
            {
                //최소한의 길이 보장 X 총구 밀림
                handPos = handPos.Lerp(Vector2.FromAngle(facingDir) * (handDistance - allowedDistance + realDistance), 0.1f);
                handRot = Mathf.LerpAngle(handRot, (realAimPoint - equippedWeapon.GlobalPosition).Angle(), 0.1f);
            }
            else
            {
                //최소한의 길이 보장 O
                handPos = handPos.Lerp(Vector2.FromAngle(facingDir) * handDistance, 0.1f);
                handRot = Mathf.LerpAngle(handRot, (realAimPoint - equippedWeapon.GlobalPosition).Angle(), 0.1f);
            }

            equippedWeapon.Rotation = handRot;
            equippedWeapon.Position = handPos;
        }
    }

    public void SetFacingDirection(float direction)
    {
        // 바라보는 방향 설정
        facingDir = direction;
        UpdateWeaponRotation();
    }

    protected List<Interactable> interactables = new List<Interactable>();
    void InteractionProcess()
    {
        interactables = new List<Interactable>();
        Godot.Collections.Array<Node> nodes = this.GetTree().Root.GetChild(0).GetChildren();

        foreach (Node node in nodes)
            if (node is Interactable interactable)
            {
                float dist = (GlobalPosition - interactable.GlobalPosition).Length();
                if (dist < interactable.interactableRange)
                    interactables.Add(interactable);
            }

    }


    // 데미지 처리 함수
    public void GetDamage(float damage)
    {
        healthNow -= damage;
        GD.Print($"{Name}가 {damage}의 피해를 입어 현재 체력: {healthNow}");

        // 체력이 0 이하이면 사망 처리
        if (healthNow <= 0)
        {
            OnDead();
        }
    }

    // 사망 처리 함수
    private void OnDead()
    {
        GD.Print($"{Name}가 사망했습니다.");
        QueueFree(); // 객체 삭제
    }

    // 발사 시 호출하는 함수 (발사 성공 시)
    public void OnShoot()
    {
        // 지연 반동 벡터에 반동 추가
        float recoilValue = 100f; //반동 크기
        delayedRecoilVec += Vector2.FromAngle(randFloat * 2 * (float)Math.PI) * recoilValue;
    }


    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 35, Colors.AliceBlue);

    }
}
