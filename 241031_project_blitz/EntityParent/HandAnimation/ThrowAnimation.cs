using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MedStatus;


public partial class ThrowAnimation : HandAnimation
{
    public ThrowAnimation(Throwable throwable, string code)
    {
        this.throwable = throwable;
        throwStatus = ThrowStatus.Get(code);

        var sprite = new Sprite2D();
        sprite.Scale = Vector2.One * 2.5f;
        sprite.Texture = ResourceLoader.Load<Texture2D>(throwable.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(sprite);
        sprite.Position = Vector2.Zero;

    }
    public Throwable throwable;
    public ThrowStatus throwStatus;

    float equipRatio = 0f, usingRatio = 0f;
    float equipTime = 0.6f;
    float usingTime = 0.5f;
    float efficience = 0f;

    public bool isCanceling = false;
    public bool isThrowed = false, toReady = false;

    public void CancelUse() => isCanceling = true;

    public override void _Process(double delta)
    {
        GD.Print($"{Name} / equipRatio : {equipRatio} / usingRatio : {usingRatio} / isCanceling : {isCanceling}");

        //캔슬 시 장비해제 애니메이션으로
        float equipRatioDelta = (float)delta / equipTime
            * (!isCanceling ? 1f : -1f);
        equipRatio = Math.Clamp(equipRatio + equipRatioDelta, 0f, 1f);

        //[활성화 시작 시]
        if (equipRatio == 1f && usingRatio == 0f && !isCanceling)
            UseInit();

        //[활성화 중일 시]
        if (equipRatio == 1f && usingRatio != 0f)
            UseDuring((float)delta);

        //[활성화 종료 시]
        if (equipRatio == 1f && isCanceling)
            UseEnd();

        //[장비해제 성공 시]
        if (equipRatio == 0f && isCanceling)
            AnimEnd();

        if(isThrowed)
            CancelUse();

        float usingRatioDelta = (float)delta / usingTime
            * (equipRatio == 1f && !isCanceling && toReady ? 1f : -1f);
        usingRatio = Math.Clamp(usingRatio + usingRatioDelta, 0f, 1f);

    }

    void UseInit()
    {
        GD.Print("UseInit");
        //일회용 아이템 제거
        //if (usableMed.hasHealPoint == false)
        //    usableMed.onStorage?.RemoveItem(usableMed);
    }
    void UseDuring(float delta)
    {
        GD.Print("UseDuring");

        //충분히 회복한 경우를 판단하고 캔슬
        //if (usableMed.hasHealPoint)
        //    if (master.health.hpNow >= master.health.hpMax)
        //        CancelUse();

        //지속적인 효과 적용
        //foreach (var effect in effects)
        //    procedure(false, master, delta / medStatus.duration, effect);

        //회복 포인트가 있는 아이템의 경우
        //GD.Print("usableMed.hasHealPoint : " + usableMed.hasHealPoint);
        //if (usableMed.hasHealPoint)
        //{
        //    //충분히 회복한 경우를 판단하고 캔슬
        //    //GD.Print("master.health.hpNow >= master.health.hpMax : " + master.health.hpNow + "  " + master.health.hpMax + " >>> " + (master.health.hpNow >= master.health.hpMax));
        //    if (master.health.hpNow >= master.health.hpMax)
        //        CancelUse();

        //    //회복 포인트 감소
        //    usableMed.healPoint = (
        //        usableMed.healPoint.now - efficience * delta,
        //        usableMed.healPoint.max);

        //    //회복 포인트를 모두 소모했다면 제거
        //    if (usableMed.healPoint.now <= 0f)
        //    {
        //        CancelUse();
        //        usableMed.onStorage?.RemoveItem(usableMed);
        //    }
        //}

        if (usingRatio >= 1f)
            CancelUse();
    }
    void UseEnd()
    {
        GD.Print("UseEnd");
        //일시적인 효과 적용
        //foreach (var effect in effects)
        //    procedure(false, master, 0, effect);

    }
    void AnimEnd()
    {
        GD.Print("AnimEnd");
        master.UnEquip();
        this.QueueFree();
    }

    public void SetReady(bool toReady) => this.toReady = toReady;
    public void Throw(Vector2 tPos)
    {
        bool isThrowable = equipRatio >= 1f && toReady && !isCanceling && !isThrowed;

        if (!isThrowable) return;

        isThrowed = true;
        CancelUse();

        // ThrowableEntity 인스턴스 생성
        var grenadeScene = ResourceLoader.Load<PackedScene>("res://Prefab/Dynamic/ThrowableEntity.tscn");
        var grenadeInstance = grenadeScene.Instantiate<ThrowableEntity>();

        // 현재 위치에서 생성
        grenadeInstance.GlobalPosition = GlobalPosition;

        // 투척 방향 및 속도 적용
        Vector2 throwDirection = (tPos - GlobalPosition).Normalized();
        float throwPower = 500f; // 임의의 투척 속도 값 (조정 가능)
        grenadeInstance.LinearVelocity = throwDirection * throwPower;

        // ThrowStatus 적용
        grenadeInstance.throwStatus = throwStatus;

        // 월드에 추가
        GetParent().AddChild(grenadeInstance);
    }
}
