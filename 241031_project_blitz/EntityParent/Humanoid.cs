using Godot;
using System;
using System.Collections.Generic;
using static Godot.OpenXRHand;
using static Humanoid;

public partial class Humanoid : RigidBody2D
{
    /* Movement */
    //이동 관련 변수
    public Vector2 moveVec = new Vector2(0, 0);
    private float speed = 100f;  // 이동 속도 조절
    private float inertia = 0.15f; // 관성 계수 조절

    //방향 관련 변수
    protected float facingDir = 0f; // 현재 바라보는 방향 (라디안 단위)
    protected bool isRightSide => Math.Abs(facingDir) < MathF.PI / 2f;


    /* Health */
    //체력
    public float healthNow;
    public float healthMax;


    /* Aim */
    // 조준점 관련 변수
    public Vector2 virtualAimPoint;
    private Vector2 recoilVec = Vector2.Zero;
    private float aimLength => (virtualAimPoint - GlobalPosition).Length();

    //최종 조준점 연산
    public Vector2 realAimPoint { get
        {
            Vector2 aimPoint = virtualAimPoint;

            aimPoint += (recoilVec * aimLength / 1000 - GlobalPosition)
                .Rotated(recoilVec.Length() * randFloat / 5000);
            
            aimPoint += GlobalPosition;

            
            float aimStatble = equippedWeapon == null? 0f : equippedWeapon.status.aimDt.stance; // 조준 안정 계수

            float radius = aimStatble * (1f + recoilVec.Length() / 100f);
            float noiseX = noise.GetNoise2D(aimStableTime * 10f, 6945) * radius;
            float noiseY = noise.GetNoise2D(aimStableTime * 10f + 100, 1235) * radius;
            aimPoint += new Vector2(noiseX, noiseY)
                * aimLength / 1000;

            return aimPoint;
        } }

    //지연 반동
    protected Vector2 delayedRecoilVec = Vector2.Zero;
    protected float delayedRecoilRatio = 0.9f;

    //조준 안정
    protected float aimStableTime = 0f;

    /* Hands */
    //장비한 Weapon 객체
    protected Weapon equippedWeapon;

    //장비한 Weapon 객체 위상
    protected Vector2 handPos;
    protected float handRot;

    //장비 중인 무기 슬롯, Weapon 장비 딜레이
    protected Inventory.EquipSlot nowEquip = null;
    protected Inventory.EquipSlot targetEquip = null;
    protected float equipValue = 0f;
    public bool isEquiped => equipValue >= 1f && nowEquip != null;

    //인벤토리 여부
    public bool isInventory = false;

    //상호작용
    protected List<Interactable> interactables = new List<Interactable>();
    public Interactable interacting = null;


    //노이즈 객체
    private static FastNoiseLite noise = new FastNoiseLite();
    //랜덤 범위 뭐시기
    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;


    /* Initiate */
    public override void _Ready()
    {
        healthMax = 100f; // 최대 체력 초기화
        healthNow = healthMax; // 현재 체력을 최대 체력으로 설정

        inventory = new Inventory(this);

        // Collision 생성 및 설정
        var collision = new CollisionShape2D();
        collision.Shape = new CircleShape2D() { Radius = 35 };
        collision.Position = Vector2.Zero;
        AddChild(collision);

        GravityScale = 0;
        LockRotation = true;

        SetCollisionMaskValue(1, true);
        SetCollisionLayerValue(1, true); // Humanoid 레이어
        SetCollisionMaskValue(2, true);  // Projectile과 충돌 감지
        SetCollisionMaskValue(3, true);  // Wall과 충돌 감지


        // 초기 가상 조준점 설정
        virtualAimPoint = GetGlobalMousePosition();
        handPos = GlobalPosition;
    }


    /* Process */
    public override void _PhysicsProcess(double delta)
    {
        // moveVec이 길이 1을 초과하면 normalize 처리
        if (moveVec.Length() > 1)
            moveVec = moveVec.Normalized();

        // 이동 처리: 관성을 적용해 이동
        LinearVelocity += moveVec * speed;
        LinearVelocity = LinearVelocity.Lerp(Vector2.Zero, inertia);

        WeaponTransformProcess();
    }

    public override void _Process(double delta)
    {
        QueueRedraw();

        // 가상 조준점 업데이트
        virtualAimPoint = virtualAimPoint.Lerp(GetGlobalMousePosition(),
            equippedWeapon == null? 0.3f : equippedWeapon.status.aimDt.traggingSpeed);

        float recoilRecover = 0.1f; //반동 회복
        // 실제 반동 벡터 업데이트
        recoilVec += delayedRecoilVec * (1f - delayedRecoilRatio);
        recoilVec *= (1f - recoilRecover);

        // 지연 반동 벡터 업데이트
        delayedRecoilVec *= delayedRecoilRatio;

        //조준 안정
        float aimStableStrength = equippedWeapon == null? 1f : equippedWeapon.status.aimDt.stance;
        aimStableTime += (float)delta * (1 + recoilVec.Length()/100) * 3f * aimStableStrength/50f;

        InteractionProcess();
        WeaponEquipProcess((float)delta);
    }

    void InteractionProcess()
    {
        interactables = new List<Interactable>();
        Godot.Collections.Array<Node> nodes = this.GetTree().Root.GetChild(0).FindByName("World").GetChildren();
        
        foreach (Node node in nodes)
            if (node is Interactable interactable)
            {
                float dist = (GlobalPosition - interactable.GlobalPosition).Length();

                if (dist < interactable.interactableRange)
                    interactables.Add(interactable);
            }

    }

    void WeaponEquipProcess(float delta)
    {
        bool isNotCorrectEquiped =
            (equippedWeapon != null && equippedWeapon.weaponItem != null &&
            nowEquip != null && nowEquip.item != null)
            &&
            (targetEquip == null && equippedWeapon.weaponItem != nowEquip.item);

        //GD.Print($"WeaponEquipProcess : {equipValue} isNotCorrectEquiped : {isNotCorrectEquiped}" +
        //    $"\n{(equippedWeapon == null? "no Equipped" : equippedWeapon.weaponItem.status.shortName)}" +
        //    $"\n{(nowEquip == null ? "null" : nowEquip.item == null ? "item is null!" : nowEquip.item.status.shortName)}" +
        //    $"\n{(targetEquip == null ? "null" : targetEquip.item == null ? "item is null!" : targetEquip.item.status.shortName)}");


        float equipDelay = 0.65f;
        bool isInvalidEquiped = nowEquip == null || equippedWeapon == null;


        //장비 중인 무기가 없을시 있는거 아무거나 장비
        //추후 삭제 가능?
        if (isInvalidEquiped)
            if (inventory.firstWeapon.item != null)
                targetEquip = inventory.firstWeapon;
            else if (inventory.secondWeapon.item != null)
                targetEquip = inventory.secondWeapon;
            else if (inventory.subWeapon.item != null)
                targetEquip = inventory.subWeapon;
            else
                targetEquip = null;

        if (isNotCorrectEquiped)
            targetEquip = null;

        //현재 무기를 장비 중
        else if (targetEquip == nowEquip && targetEquip != null)
            if (!isEquiped)
                equipValue += delta / equipDelay;

        //다른 무기로 교체 중
        if (targetEquip != nowEquip)
        {
            equipValue -= delta / equipDelay;
            //장비 해제에 대한 처리
            if (equipValue < 0)
            {
                equipValue = 0;
                equippedWeapon?.QueueFree();
                equippedWeapon = null;
                nowEquip = targetEquip;

                if (targetEquip != null && targetEquip.item is WeaponItem wItem)
                {
                    EquipWeapon(wItem.GetWeapon());
                    WeaponTransformProcess();
                }
            }
        }
    }

    void WeaponTransformProcess()
    {
        if (equippedWeapon == null)
            return;

        const float handDistance = 80f;
        float realDistance = (realAimPoint - GlobalPosition).Length();
        float allowedDistance = equippedWeapon.status.detailDt.muzzleDistance + handDistance + 20f;
        float minDistance = equippedWeapon.status.detailDt.muzzleDistance + 20f;

        Vector2 tPos = handPos;
        float tRot = handRot;
        float rotSider = isRightSide ? 1f : -1f;

        //근접 총기 접힘
        if (realDistance < minDistance)
        {
            //최소한의 길이 보장 X 총구 꺾임
            tPos = Vector2.FromAngle(facingDir) * handDistance / 4f;
            tRot = (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.5f * 3.14f * rotSider;
        }
        else if (equippedWeapon.isReloading || isInventory)
        {
            //최소한의 길이 보장 X 총구 꺾임
            tPos = Vector2.FromAngle(facingDir) * handDistance / 2f;
            tRot = (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.25f * 3.14f * rotSider;
        }
        else if (realDistance < allowedDistance)
        {
            //최소한의 길이 보장 X 총구 밀림
            tPos = Vector2.FromAngle(facingDir) * (handDistance - allowedDistance + realDistance);
            tRot = (realAimPoint - equippedWeapon.GlobalPosition).Angle();
        }
        else
        {
            //최소한의 길이 보장 O
            tPos = Vector2.FromAngle(facingDir) * handDistance;
            tRot = (realAimPoint - equippedWeapon.GlobalPosition).Angle();
        }

        //장비 진행에 따른 위상
        if (!isEquiped)
        {
            float ratio = MathF.Pow(1f - equipValue, 1f / 2f);

            tPos = tPos.Lerp(Vector2.FromAngle(facingDir) * -handDistance / 2f * rotSider, ratio);

            tRot = Mathf.LerpAngle(tRot,
                (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.5f * 3.14f * rotSider,
                ratio);
        }

        handPos = handPos.Lerp(tPos, 0.1f);
        handRot = Mathf.LerpAngle(handRot, tRot, 0.1f);

        //무기 흔들림
        float tremblePower = 10f;
        float trembleSpeed = 10f + recoilVec.Length() / 30f;

        Vector2 trembleVec = new Vector2(
            noise.GetNoise2D(aimStableTime * trembleSpeed, 12435),
            noise.GetNoise2D(aimStableTime * trembleSpeed, 6559)
            ) * tremblePower;
        float trembleRot = noise.GetNoise2D(aimStableTime * trembleSpeed, 792) * tremblePower / 100f;

        //이동 기울임
        Vector2 moveVec = -LinearVelocity * 0.05f;

        //적용
        equippedWeapon.Rotation = handRot + trembleRot;
        equippedWeapon.Position = handPos + trembleVec + moveVec;
        equippedWeapon.Scale = new(1f, rotSider);
    }

    /* Updater */
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
    }

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
        handPos += Vector2.FromAngle(facingDir) * -recoilVec.Length() * 0.3f;
    }


    //TEMPORARY
    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 35, Colors.AliceBlue);
    }
}
