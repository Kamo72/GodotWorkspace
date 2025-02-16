using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class UsableMed : Item, IUsable
{
    public string code;

    public UsableMed(string code)
    {
        this.code = code;
        SetValue();
    }

    public MedStatus medStatus => MedStatus.GetByCode(code);

    public MedAnimation GetMedAnimation() 
    {
        MedAnimation anim = new MedAnimation(this, code);
        return anim;
    }

    void SetValue()
    {
        if (MedStatus.IsValidCode(code) == false) return;

        bool result = false;

        medStatus.effects.ForEach(e => {
            if (e.effect == MedStatus.Effect.HEALING) {
                result = true;
                GD.Print("this is healpoint med");
                healPoint = (e.ratio, e.ratio);
            }
        });

        hasHealPoint = result;
        healDuration = medStatus.duration;
    }

    public bool hasHealPoint = false;
    public (float now, float max) healPoint = (0,0);
    public float healDuration = 0f;

    public void ReduceHealPoint(float point)
    {
        healPoint.now -= point;

        if (healPoint.now < point)
            onStorage?.RemoveItem(this);
    }
}

public struct MedStatus
{
    public static MedStatus GetByCode(string code) => medStatusDic[code];
    public static bool IsValidCode(string code) => medStatusDic.ContainsKey(code);
    public static void Set(string code, MedStatus medStatus) => medStatusDic[code] = medStatus;

    static Dictionary<string, MedStatus> medStatusDic = new Dictionary<string, MedStatus>();

    public enum Effect
    {
        HEMOSTASIS, //지혈
        HEALING,    //체력 회복
        DOT,   //지속 피해
        SUICIDE,    //자살
    }
    public static (bool isOnce, Action<Humanoid, float, float> action) GetActionByEffect(Effect effect) => effectActionDic[effect];
    static Dictionary<Effect, (bool isOnce, Action<Humanoid, float, float> action)> effectActionDic = new()
    {
        { Effect.HEMOSTASIS, (false, (Humanoid humanoid, float ratio, float delta)=>{
            humanoid.health.GetHemostasis(ratio * delta);
        }) },
        { Effect.HEALING, (false, (Humanoid humanoid, float ratio, float delta)=>{
            humanoid.health.GetHeal(ratio * delta);
        }) },
        { Effect.DOT, (false, (Humanoid humanoid, float ratio, float delta)=>{
            humanoid.health.GetDamage(ratio * delta);
        }) },
        { Effect.SUICIDE, (true, (Humanoid humanoid, float ratio, float delta)=>{
            humanoid.health.GetDamage(10000f);
        }) },
    };

    public MedStatus(string code, float delay, float duration, bool isMovable, List<(Effect effect, float ratio)> effects) 
    {
        this.code = code;
        this.delay = delay;
        this.duration = duration;
        this.isMovable = isMovable;
        this.effects = effects;
    }

    public string code;
    public float delay;
    public float duration;
    public bool isMovable;
    public List<(Effect effect, float ratio)> effects;
}