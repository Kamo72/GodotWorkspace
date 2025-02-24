using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MedAnimation : HandAnimation
{
    public MedAnimation(UsableMed usableMed, string code) 
    {
        this.usableMed = usableMed;
        medStatus = MedStatus.Get(code);

        var sprite = new Sprite2D();
        sprite.Scale = Vector2.One * 2.5f;
        sprite.Texture = ResourceLoader.Load<Texture2D>(usableMed.status.textureRoot);
        sprite.TextureFilter = TextureFilterEnum.Nearest;
        AddChild(sprite);
        sprite.Position = Vector2.Zero;

        procedure = (isOnce, hum, delta, effectNode) =>
        {
            var actionNode = MedStatus.GetActionByEffect(effectNode.Item1);
            Action<Humanoid, float, float> action = actionNode.action;
            bool isOnceNode = actionNode.isOnce;
            float value = effectNode.Item2;

            if (effectNode.Item1 == MedStatus.Effect.HEALING)
                efficience = value / medStatus.duration;

            if (isOnce == isOnceNode) 
                action(hum, delta, value);
        };
    }

    public MedStatus medStatus;
    public UsableMed usableMed;

    float equipRatio = 0f, usingRatio = 0f;
    float equipTime => medStatus.delay;
    float usingTime => medStatus.duration;
    float efficience = 0f;

    Action<bool, Humanoid, float, (MedStatus.Effect, float)> procedure;
    List<(MedStatus.Effect, float)> effects => medStatus.effects;

    public bool isCanceling = false;

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
        if (equipRatio == 1f && usingRatio != 0f )
            UseDuring((float)delta);

        //[활성화 종료 시]
        if (equipRatio == 1f && isCanceling)
            UseEnd();

        //[장비해제 성공 시]
        if (equipRatio == 0f && isCanceling)
            AnimEnd();


        float usingRatioDelta = (float)delta / medStatus.duration
            * (equipRatio == 1f && !isCanceling ? 1f : -1f);
        usingRatio = Math.Clamp(usingRatio + usingRatioDelta, 0f, 1f);

    }
    
    void UseInit()
    {
        GD.Print("UseInit");
        //일회용 아이템 제거
        if (usableMed.hasHealPoint == false)
            usableMed.onStorage?.RemoveItem(usableMed);
    }
    void UseDuring(float delta)
    {
        GD.Print("UseDuring");

        //충분히 회복한 경우를 판단하고 캔슬
        if (usableMed.hasHealPoint)
            if (master.health.hpNow >= master.health.hpMax)
                CancelUse();

        //지속적인 효과 적용
        foreach (var effect in effects)
            procedure(false, master, delta/medStatus.duration, effect);

        //회복 포인트가 있는 아이템의 경우
        //GD.Print("usableMed.hasHealPoint : " + usableMed.hasHealPoint);
        if (usableMed.hasHealPoint)
        {
            //충분히 회복한 경우를 판단하고 캔슬
            //GD.Print("master.health.hpNow >= master.health.hpMax : " + master.health.hpNow + "  " + master.health.hpMax + " >>> " + (master.health.hpNow >= master.health.hpMax));
            if (master.health.hpNow >= master.health.hpMax)
                CancelUse();

            //회복 포인트 감소
            usableMed.healPoint = (
                usableMed.healPoint.now - efficience * delta,
                usableMed.healPoint.max);

            //회복 포인트를 모두 소모했다면 제거
            if (usableMed.healPoint.now <= 0f)
            {
                CancelUse();
                usableMed.onStorage?.RemoveItem(usableMed);
            }
        }

        if (usingRatio >= 1f)
            CancelUse();
    }
    void UseEnd()
    {
        GD.Print("UseEnd");
        //일시적인 효과 적용
        foreach (var effect in effects)
            procedure(false, master, 0, effect);

    }
    void AnimEnd()
    {
        GD.Print("AnimEnd");
        master.UnEquip();
        this.QueueFree();
    }
}
