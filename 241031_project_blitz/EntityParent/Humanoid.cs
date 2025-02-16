using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Godot.OpenXRHand;
using static Humanoid;
using static System.Net.Mime.MediaTypeNames;

public partial class Humanoid : RigidBody2D
{
    /* Movement */
    //이동 관련 변수
    public Vector2 moveVec = new Vector2(0, 0);
    //private float speed = 100f;  // 이동 속도 조절
    //private float inertia = 0.15f; // 관성 계수 조절

    //방향 관련 변수
    public float facingDir = 0f; // 현재 바라보는 방향 (라디안 단위)
    protected bool isRightSide => Math.Abs(facingDir) < MathF.PI / 2f;

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
    public HandAnimation equipped = null;
    public Weapon equippedWeapon => equipped as Weapon;
    public MedAnimation medAnimation => equipped as MedAnimation;

    //장비한 Weapon 객체 위상
    protected Vector2 handPos;
    protected float handRot;

    //장비 중인 무기 슬롯, Weapon 장비 딜레이
    public Inventory.EquipSlot nowEquip = null;
    protected Inventory.EquipSlot targetEquip = null;
    protected float equipValue = 0f;
    public bool isEquiped => equipValue >= 1f && nowEquip != null;

    //인벤토리 여부
    public bool isInventory = false;

    //상호작용
    protected List<IInteractable> interactables = new List<IInteractable>();
    public IInteractable interacting = null;


    //노이즈 객체
    private static FastNoiseLite noise = new FastNoiseLite();
    //랜덤 범위 뭐시기
    protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;


    protected string job = "";

    /* Initiate */
    public override void _Ready()
    {
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

    public override void _EnterTree()
    {
        inventory = new Inventory(this);
        movement = new Movement(this);
        health = new Health(this, 400);

        WorldManager.humanoids.Add(this);
    }
    public override void _ExitTree()
    {
        WorldManager.humanoids.Remove(this);
    }


    /* Process */
    public override void _PhysicsProcess(double delta)
    {
        if (GetParent() is Chunk chunk)
        {
            chunk.RemoveChild(this);
            chunk.GetParent().AddChild(this);
        }

        movement?.PhysicProcess(moveVec);
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
        intelligence?.Process((float)delta);
        movement?.Process((float)delta);
        health?.Process((float)delta);

        IntelligenceProcess((float)delta);
        InteractionProcess();
        WeaponEquipProcess((float)delta);
        WeaponTransformProcess((float)delta);
    }

    void InteractionProcess()
    {
        interactables = new List<IInteractable>();
        foreach (Node node in WorldManager.interactables)
            if(node is Node2D node2d)
                if (node is IInteractable interactable)
                {
                    if (node == this) continue;

                    float dist = (GlobalPosition - node2d.GlobalPosition).Length();

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
            (targetEquip == null && equippedWeapon.weaponItem != nowEquip.item)
            && equipped == null
            && equipped is not MedAnimation;

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
                equipped = null;
                nowEquip = targetEquip;

                if (targetEquip != null && targetEquip.item is WeaponItem wItem)
                {
                    EquipWeapon(wItem.GetWeapon());
                    WeaponTransformProcess(delta);
                }
            }
        }
    }

    void WeaponTransformProcess(float delta)
    {
        if (equipped == null) return;
        //if (equippedWeapon == equipped) return;

        const float handDistance = 80f;
        float realDistance = (realAimPoint - GlobalPosition).Length();
        float allowedDistance = handDistance + 20f + (equippedWeapon == null? 30f : equippedWeapon.status.detailDt.muzzleDistance);
        float minDistance = (equippedWeapon == null ? 0f : equippedWeapon.status.detailDt.muzzleDistance) + 20f;

        Vector2 tPos = handPos;
        float tRot = handRot;
        float rotSider = isRightSide ? 1f : -1f;

        //근접 총기 접힘
        if (realDistance < minDistance)
        {
            //최소한의 길이 보장 X 총구 꺾임
            tPos = Vector2.FromAngle(facingDir) * handDistance / 4f;
            tRot = (realAimPoint - equipped.GlobalPosition).Angle() - 0.5f * 3.14f * rotSider;
        }
        else if (equippedWeapon != null && equippedWeapon.isReloading || isInventory)
        {
            //최소한의 길이 보장 X 총구 꺾임
            tPos = Vector2.FromAngle(facingDir) * handDistance / 2f;
            tRot = (realAimPoint - equipped.GlobalPosition).Angle() - 0.25f * 3.14f * rotSider;
        }
        else if (realDistance < allowedDistance)
        {
            //최소한의 길이 보장 X 총구 밀림
            tPos = Vector2.FromAngle(facingDir) * (handDistance - allowedDistance + realDistance);
            tRot = (realAimPoint - equipped.GlobalPosition).Angle();
        }
        else
        {
            //최소한의 길이 보장 O
            tPos = Vector2.FromAngle(facingDir) * handDistance;
            tRot = (realAimPoint - equipped.GlobalPosition).Angle();
        }

        //장비 진행에 따른 위상
        if (!isEquiped)
        {
            float ratio = MathF.Pow(1f - equipValue, 1f / 2f);

            tPos = tPos.Lerp(Vector2.FromAngle(facingDir) * handDistance / 2f , ratio);

            tRot = Mathf.LerpAngle(tRot,
                (realAimPoint - equipped.GlobalPosition).Angle() - 0.5f * 3.14f * rotSider,
                ratio);
        }
        
        //달리기에 따른 위상
        else if (movement.sprintValue > 0.01f)
        {
            float ratio = MathF.Pow(movement.sprintValue, 1f / 2f);

            tPos = tPos.Lerp(Vector2.FromAngle(facingDir) * handDistance / 2f , ratio);

            tRot = Mathf.LerpAngle(tRot,
                (realAimPoint - equippedWeapon.GlobalPosition).Angle() - 0.5f * 3.14f * rotSider,
                ratio);
        }

        handPos = handPos.Lerp(tPos, 0.1f);
        handRot = Mathf.LerpAngle(handRot, tRot, 0.1f);

        //무기 흔들림
        float tremblePower = (equipped == equippedWeapon? 10f : 150f);
        float trembleSpeed = (equipped == equippedWeapon ? 10f : 150f) + recoilVec.Length() / 30f;

        Vector2 trembleVec = new Vector2(
            noise.GetNoise2D(aimStableTime * trembleSpeed, 12435),
            noise.GetNoise2D(aimStableTime * trembleSpeed, 6559)
            ) * tremblePower;
        float trembleRot = noise.GetNoise2D(aimStableTime * trembleSpeed, 792) * tremblePower / 100f;

        //이동 기울임
        Vector2 moveVec = -LinearVelocity * 0.05f;

        //적용
        equipped.Rotation = handRot + trembleRot;
        equipped.Position = handPos + trembleVec + moveVec;
        equipped.Scale = new(1f, rotSider);
    }

    bool noIntelligenceErrored = false;
    void IntelligenceProcess(float delta)
    {
        if (intelligence == null ) {
            if (!noIntelligenceErrored)
            {
                GD.PushError($"[ERROR] {Name} : 해당 Humanoid에게 등록된 Intelligence가 없습니다!");
                noIntelligenceErrored = true;
            }
            return;
        }
        
        //이동
        moveVec = intelligence.vectorMap["MoveVec"];

        // 마우스 위치를 얻어 Player의 위치로부터 방향 계산
        var mousePosition = intelligence.vectorMap["AimPos"];
        var direction = mousePosition - GlobalPosition;
        var angle = direction.Angle();

        // 바라보는 방향 설정
        facingDir = angle;


        // 가상 조준점 업데이트
        virtualAimPoint = virtualAimPoint.Lerp(intelligence.vectorMap["AimPos"],
            equippedWeapon == null ? 0.3f : equippedWeapon.status.aimDt.traggingSpeed);

        float recoilRecover = 0.1f; //반동 회복
        // 실제 반동 벡터 업데이트
        recoilVec += delayedRecoilVec * (1f - delayedRecoilRatio);
        recoilVec *= (1f - recoilRecover);

        // 지연 반동 벡터 업데이트
        delayedRecoilVec *= delayedRecoilRatio;

        //조준 안정
        float aimStableStrength = equippedWeapon == null ? 1f : equippedWeapon.status.aimDt.stance;
        aimStableTime += (float)delta * (1 + recoilVec.Length() / 100) * 3f * aimStableStrength / 50f;

        // 재장전 수행
        if (intelligence.commandMap["Reload"])
            equippedWeapon?.Reload();

        if (intelligence.commandMap["Inventory"])
        {
            isInventory = !isInventory;
            InventoryPage.instance?.ResetOtherPanel();

            if (this == Player.player)
            {
                Control mainUI = GetTree().Root.FindByName("MainUi") as Control;
                mainUI.Visible = !mainUI.Visible;
            }
        }

        if (intelligence.commandMap["Interact"])
        {
            GD.PushWarning("Interaction Call : " + interactables.Count);
            if (interactables.Count > 0)
                interactables[0].Interacted(this);
        }

        if (intelligence.commandMap["FirstWeapon"])
            if (inventory.firstWeapon.item != null)
                targetEquip = inventory.firstWeapon;

        if (intelligence.commandMap["SecondWeapon"])
            if (inventory.secondWeapon.item != null)
                targetEquip = inventory.secondWeapon;

        if (intelligence.commandMap["SubWeapon"])
            if (inventory.subWeapon.item != null)
                targetEquip = inventory.subWeapon;

        if (intelligence.commandMap["Fire"])
        {
            if (equippedWeapon == null) return;
            if (equippedWeapon.IsBusy())
            {
                if (equippedWeapon.isReloading)
                    equippedWeapon.DisruptReload();
                return;
            }

            bool isShoot = equippedWeapon.Shoot();
            if (isShoot)
                OnShoot();
        }
        if (intelligence.commandMap["FireReleased"])
            equippedWeapon?.SetRealease();
    }

    /* Updater */
    public void EquipWeapon(Weapon weapon)
    {
        // 무기를 장착하여 자식 노드에 추가
        if (equipped != null)
            if(equipped.GetParent() != this)
                RemoveChild(equipped);

        equipped = weapon;
        AddChild(equipped);
        equipValue = 0f;

        // Weapon 위치 및 초기 회전 설정
        equipped.Position = Vector2.Zero;
    }

    public async void EquipMed(MedAnimation med)
    {
        // 무기를 장착하여 자식 노드에 추가
        if (equipped != null)
        {
            if (equipped is Weapon weapon)
            {
                targetEquip = null;
                await ToSignal(GetTree().CreateTimer(weapon.status.timeDt.swapTime), "timeout");
            }
            RemoveChild(equipped);
        }
        equipped = med;
        AddChild(equipped);

        // Weapon 위치 및 초기 회전 설정
        equipped.Position = Vector2.Zero;
    }

    public void UnEquip()
    {
        // 무기를 장착하여 자식 노드에 추가
        if (equipped != null)
        {
            RemoveChild(equipped);
            equipped = null;
        }

        if (targetEquip != null && targetEquip.item is WeaponItem wItem)
            EquipWeapon(wItem.GetWeapon());
    }

    private void OnDead()
    {
        GD.Print($"{Name}가 사망했습니다.");

        //시체 생성
        Body body = ResourceLoader.Load<PackedScene>("res://Prefab/Dynamic/body.tscn").Instantiate() as Body;
        inventory.master = null;
        body.Initiate( inventory );
 

        //기타 추가적인 정보 전달
        GetParent().AddChild(body);
        body.Position = Position;

        if (CameraManager.current.target == this) 
            CameraManager.current.target = null;

        QueueFree(); // 객체 삭제
    }

    // 발사 시 호출하는 함수 (발사 성공 시)
    public void OnShoot()
    {
        // 지연 반동 벡터에 반동 추가
        float recoilValue = 100f; //반동 크기
        delayedRecoilVec += Vector2.FromAngle(randFloat * 2 * (float)Math.PI) * recoilValue;
        handPos += Vector2.FromAngle(facingDir) * -recoilVec.Length() * 0.3f;


        if (CameraManager.current.target == this)
            CameraManager.current.ApplyRecoil(equippedWeapon.status.aimDt.strength);
        //if (CameraManager.current.target == this)
        //    CameraManager.current.ApplyRecoil(100);
    }


    //TEMPORARY
    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 35, Colors.AliceBlue);
    }
}
