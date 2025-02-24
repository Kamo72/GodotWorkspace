
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Godot;

public abstract class Magazine : Item
{
    public Magazine(string magazineCode)
    {
        this.magazineCode = magazineCode;
        magStatus = magStatusOrigin;
        ammoStack = new Stack<Ammo> { };
    }
    public Magazine(string magazineCode, Type ammoType) : this(magazineCode)
    {
        if (ammoType == null) return;

        //입력한 탄 유형으로 탄약 채우기
        bool isAmmo = TypeEx.IsChildByParent(ammoType, typeof(Ammo));

        if (isAmmo == false) return;

        try
        {
            while (true)
            {
                Ammo ammo = Activator.CreateInstance(ammoType) as Ammo;
                ammo.stackNow = 9999;
                AmmoPush(ammo);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + e.StackTrace);
        }
    }

    public string magazineCode;
    public MagazineStatus magStatusOrigin { get { return MagazineLibrary.Get(magazineCode); } }
    public MagazineStatus magStatus;

    #region [탄 관리]
    Stack<Ammo> ammoStack; //탄창 내 탄약 <스택>

    public int ammoCount => ammoStack.Count;

    public int AmmoPrice => ammoPrice;
    int ammoPrice = 0;
    public bool AmmoPush(Ammo ammo)
    {
        //예외
        if (magStatus.whiteList.Contains(ammo.ammoStatus.caliber) == false) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "호환되지 않는 탄종" + $"{ammo.ammoStatus.caliber.ToString()} != {magStatus.whiteList.ToArray()}");
        if (ammoStack.Count >= magStatus.ammoSize) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄창이 가득 찼음");
        if (ammo.stackNow <= 0) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄약의 Stack값이 정상적이지 않음");

        //GD.Print("Ammo Push " + ammoPrice);
        //처리
        ammo.stackNow--;
        Ammo splitted = Activator.CreateInstance(ammo.GetType()) as Ammo;
        ammoPrice += splitted.GetPrice();
        ammoStack.Push(splitted);

        return true;
    }
    public Ammo AmmoPop()
    {
        if (ammoStack.Count <= 0) return null;

        //GD.Print("Ammo Pop " + ammoPrice);
        var ammo = ammoStack.Pop();
        ammoPrice -= ammo.GetPrice();
        return ammo;
    }
    public Ammo AmmoPeek()
    {
        if (ammoStack.Count <= 0) return null;

        return ammoStack.Peek();
    }
    #endregion
}

public abstract class Ammo : Item, IStackable
{
    public Ammo(string ammoCode)
    {
        this.ammoCode = ammoCode;
        ammoStatus = AmmoLibrary.Get(ammoCode);
    }

    public bool isUsed = false;
    public string ammoCode;

    public AmmoStatus ammoStatus;

    //IStackable
    public int stackNow { get; set; }
    public int stackMax { get; set; }
}

#region [탄창&탄약 기본 정보]
public struct MagazineStatus
{
    public MagazineStatus(int ammoSize, List<CaliberType> whiteList, List<WeaponAdjust> adjusts)
    {
        this.ammoSize = ammoSize;
        this.whiteList = whiteList;
        this.adjusts = adjusts;
    }

    public int ammoSize; //탄창 크기
    public List<CaliberType> whiteList; //허용 탄종
    public List<WeaponAdjust> adjusts;  //스텟 보정

}

public struct AmmoStatus
{
    public CaliberType caliber; //구경

    public struct Lethality
    {
        public float damage;    //피해량
        public float pierceLevel;   //관통계수
        public float suppress;  //저지력
        public float bleeding;  //출혈 발생
        public int pellitCount; //펠릿 갯수
    }
    public Lethality lethality;

    public struct Adjustment
    {
        public float accuracyRatio; //정확도 배율
        public float recoilRatio;   //반동 배율
        public float speedRatio;    //탄속 배율
        public float airDrag;       //공기저항 값
    }
    public Adjustment adjustment;

    public struct Tracer
    {
        public bool isTraced;  //예광탄 여부
        public float radius;   //예광탄 크기
        public Color color;   //예광탄 색
    }
    public Tracer tracer;
}
#endregion

#region [탄창&탄약 데이터셋 제공자]
public static class MagazineLibrary
{
    static MagazineLibrary()
    {
        magazineLib = new Dictionary<string, MagazineStatus>();
    }

    static Dictionary<string, MagazineStatus> magazineLib;

    public static MagazineStatus Get(string magazineName)
    {
        return magazineLib[magazineName];
    }
    public static void Set(string magazineName, MagazineStatus magazineStatus)
    {
        if (magazineLib.ContainsKey(magazineName)) throw new Exception("magazineLib - 중복된 키 삽입!");
        magazineLib.Add(magazineName, magazineStatus);
    }
}
public static class AmmoLibrary
{
    static AmmoLibrary()
    {
        ammoLib = new Dictionary<string, AmmoStatus>();
    }

    static Dictionary<string, AmmoStatus> ammoLib;

    public static AmmoStatus Get(string magazineName)
    {
        return ammoLib[magazineName];
    }
    public static void Set(string magazineName, AmmoStatus magazineStatus)
    {
        if (ammoLib.ContainsKey(magazineName)) throw new Exception("ammoLib - 중복된 키 삽입!");
        ammoLib.Add(magazineName, magazineStatus);
    }
}
#endregion