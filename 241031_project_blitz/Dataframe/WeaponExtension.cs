using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;



#region [총기 기본 정보]
public enum CaliberType
{
    //권총탄
    p38, //.38 Special
    p45, //.45AVP
    mm7p62x25, //토카레프
    mm9x18, //마카로프
    mm9x19, //파라벨룸
    mm9x21, //베레스크 smg에서 쓴데요

    //소형소총탄
    mm4p6x30, //HK 46
    mm5p7x28, //FN 57

    //소구경 소총탄
    mm5p45x39, //5.45
    mm5p56x45, //5.56

    //아음속탄
    p300,   //.300 blackout
    mm9x39, //9x39mm

    //중형소총탄
    mm7p62x39,  //7.62x39
    mm7p62x51,  //7.62x51 .308

    //대구경탄
    mm7p62x54R,  //7.62x54R
    p50, //.50 BMG

    //산탄
    g12, //12 gage
}
public enum SelectorType
{
    SEMI,
    AUTO,
    BURST2,
    BURST3
}
public enum MechanismType
{
    CLOSED_BOLT,    //폐쇄 노리쇠 - M4A1
    OPEN_BOLT,      //개방 노리쇠 - PPSH
    MANUAL_RELOAD,  //수동 장전 - 더블액션, 펌프 액션 등
    NONE,           //볼트 없음 - 싱글 액션, 브레이크 액션 등
}
public enum MagazineType
{
    MAGAZINE,   //박스형 탄창 - 빠른 교체
    SYLINDER,   //탄창X 약실만 - 중절식, 리볼버 등 탄창 개념이 없음.
    TUBE,       //튜브형 탄창 - 트랩도어 장전(전탄 소진 시, 약실 장전 후, 트랩도어 장전)
    INTERNAL,   //내장형 탄창 - 장전 시 볼트 재낌    
}
public enum BoltLockerType
{
    ACTIVATE,       //전탄 소진 시 노리쇠 후퇴 고정 ex M4A1
    ONLY_MANUAL,    //수동으로만 노리쇠 후퇴 고정 ex MP5
    LOCK_TO_FIRE,   //볼트를 잠궈야 사격 가능 ex karabiner 98
    NONE,           //노리쇠 후퇴 고정 불가 ex AK47
}

public struct WeaponStatus
{
    public WeaponStatus(WeaponStatus status)
    {
        this.typeDt = status.typeDt;
        this.aimDt = status.aimDt;
        this.timeDt = status.timeDt;
        this.moveDt = status.moveDt;
        this.detailDt = status.detailDt;
        this.attachDt = status.attachDt;
    }
    public WeaponStatus(TypeData typeData, AimData aimData, TimeData timeData, MovementData movementData, DetailData detailData, AttachData attachData)
    {
        this.typeDt = typeData;
        this.aimDt = aimData;
        this.timeDt = timeData;
        this.moveDt = movementData;
        this.detailDt = detailData;
        this.attachDt = attachData;
    }


    //무장 유형 정보
    public TypeData typeDt;
    public struct TypeData
    {
        public TypeData(MechanismType mechanismType, MagazineType magazineType, BoltLockerType boltLockerType, List<SelectorType> selectorList, CaliberType caliberType)
        {
            this.mechanismType = mechanismType;
            this.magazineType = magazineType;
            this.boltLockerType = boltLockerType;
            this.selectorList = selectorList;
            this.caliberType = caliberType;
        }

        public MechanismType mechanismType;     //작동 방시 
        public MagazineType magazineType;       //탄창 방식
        public BoltLockerType boltLockerType;   //노리쇠멈치 방식
        public List<SelectorType> selectorList; //조정간
        public CaliberType caliberType;         //구경
    }

    //조준 정보
    public AimData aimDt;
    public struct AimData
    {
        /// <summary>
        /// 조준안정
        /// </summary>
        public float stance;

        /// <summary>
        /// 반동 세기
        /// </summary>
        public float strength;

        /// <summary>
        /// 조준점 속도
        /// </summary>
        public float traggingSpeed;
        
        /// <summary>
        /// 반동 회복 속도
        /// </summary>
        public float recovery;

        /// <summary>
        /// 절대 탄퍼짐
        /// </summary>
        public float moa;
    }

    //행동 소요 시간 정보 
    public TimeData timeDt;
    public struct TimeData
    {
        public float adsTime;       //조준 속도
        public float sprintTime;    //질주 후 사격 전환  속도
        public float swapTime;      //무기 교체 속도

        /// <summary>
        /// 재장전 속도 - 
        /// 박스(분리 / 결합 준비 / 결합)
        /// 실린더(분리 / 장전1회 / 결합)
        /// 내부(후퇴고정 / 장전 / 전진)
        /// 튜브(장전 준비 / (약실 장전) / 튜브 장전 )
        /// </summary>
        public (float, float, float) reloadTime;

        /// <summary>
        ///총기 확인의 선후 딜레이
        /// </summary>
        public (float, float) inspectTime;

        /// <summary>
        ///노리쇠 조작 시간 (전진, 후퇴전진, 후퇴고정)
        /// </summary>
        public (float, float, float) boltTime;

    }

    //이동 속도 정보
    public MovementData moveDt;
    public struct MovementData
    {
        /// <summary>
        /// 기본 이동 속도 배율
        /// </summary>
        public float speed;
        /// <summary>
        /// 이속 변인 (엄폐, 조준, 질주)
        /// </summary>
        public (float crounch, float ads, float sprint) speedAdjust;
    }

    //상세 정보
    public DetailData detailDt;
    public struct DetailData
    {
        public float RoundDelay => 60f / roundPerMinute;
        public float roundPerMinute;
        public int chamberSize; //약실 크기
        public List<string> magazineWhiteList;  //장착 가능한 탄창리스트
        public float muzzleVelocity;    //총구 속도
        public float effectiveRange;    //유효 사거리
        public float muzzleDistance;      //총기 전장
        public float loudness;          //소음 크기 (거리로 ex 10000)

    }

    //부착물 정보
    public AttachData attachDt;
    public struct AttachData
    {
        //internal List<AttachSocket> socketList;
    }

}
//public static class WeaponStatusEx
//{
//    // internal static List<AttachSocket> CopyList(this AttachData attachDt) 
//    // {
//    //     List < AttachSocket >  list = new List<AttachSocket>(attachDt.socketList);

//    //     for (int i = 0; i < list.Count; i++) 
//    //     {
//    //         AttachSocket socket = list[i];
//    //         if (socket.attachment != null) 
//    //         {
//    //             AttachSocket originSocket = attachDt.socketList[i];
//    //             Type originType = originSocket.attachment.GetType();
//    //             var newItem = Activator.CreateInstance(originType);
//    //             socket.attachment = newItem as IAttachment;
//    //         }
//    //     }

//    //     return list;
//    // }
//}
#endregion

#region [총기 옵션 정보]

public enum WeaponAdjustType
{
    PROS,   //장
    CONS,   //단
    NONE,   //?
}
public struct WeaponAdjust
{
    public WeaponAdjust(string description, WeaponAdjustType adjustType, Func<WeaponStatus, WeaponStatus> adjustFun)
    {
        this.description = description;
        this.adjustType = adjustType;
        this.adjustFun = adjustFun;
    }

    public string description;
    public WeaponAdjustType adjustType;
    public Func<WeaponStatus, WeaponStatus> adjustFun;
}

#endregion

#region [총기 데이터셋 제공자]
internal static class WeaponLibrary
{
    static WeaponLibrary()
    {
        weaponLib = new Dictionary<string, WeaponStatus>();
        //WeaponDataLoad();
    }

    static Dictionary<string, WeaponStatus> weaponLib;
    static void WeaponDataLoad()
    {
        //정적 생성자를 불러오는 역할?
        new K2();
        new K2C1();
        new AKS_74U();
        new MP_133();
        new M4A1();
    }
    public static WeaponStatus Get(string weaponName)
    {
        return weaponLib[weaponName];
    }
    public static void Set(string weaponName, WeaponStatus weaponStatus)
    {
        if (weaponLib.ContainsKey(weaponName)) throw new Exception("weaponLib - 중복된 키 삽입!");
        weaponLib.Add(weaponName, weaponStatus);
    }
}

#endregion


