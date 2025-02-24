using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MedStatus;

public partial class Throwable : Item, IUsable
{
    public string code;
    static Throwable()
    {
        AmmoLibrary.Set("3mm_fragment", new()
        {
            caliber = CaliberType.fragment,
            adjustment = new()
            {
                accuracyRatio = 1f,
                recoilRatio = 1f,
                speedRatio = 1f,
                airDrag = 0.6f,
            },
            lethality = new()
            {
                damage = 15,
                bleeding = 16,
                suppress = 25f,
                pellitCount = 1,
                pierceLevel = 3.0f,
            },
            tracer = new()
            {
                isTraced = false,
                color= new(1f,1f,0f,1f),
                radius = 0f,
            }
        });

    }
    public Throwable(string throwCode)
    {
        this.code = throwCode;
    }

    public ThrowStatus throwStatus => ThrowStatus.Get(code);

    public ThrowAnimation GetAnimation()
    {
        ThrowAnimation anim = new ThrowAnimation(this, code);
        return anim;
    }
}


public struct ThrowStatus 
{
    public static ThrowStatus Get(string code) => throwStatusDic[code];
    public static bool IsValidCode(string code) => throwStatusDic.ContainsKey(code);
    public static void Set(string code, ThrowStatus throwStatus) => throwStatusDic[code] = throwStatus;

    static Dictionary<string, ThrowStatus> throwStatusDic = new Dictionary<string, ThrowStatus>();

    public string code; //코드
    public (float value, float distance) damage;
    public (string ammoCode, int count)? fragment;
    public float loudness;
    public float fuseDelay; //신관 길이
    public bool isImpact;   //충격 신관 여부
}