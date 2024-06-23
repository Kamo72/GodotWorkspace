using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Node2D
{
	public Sprite2D sprite;

	public WeaponStatus weaponStatus = new WeaponStatus();

	public Node2D muzzleNode => this.FindByName("Muzzle") as Node2D;
	public Node2D magNode => this.FindByName("Mag") as Node2D;
	public Node2D magInsertNode => this.FindByName("MagInsert") as Node2D;
	public Node2D chamber => this.FindByName("Chamber") as Node2D;


	public Dictionary<string, Func<bool>> inputMap; 
	public bool isEquiped => GetParent() is Hands;

	public override void _Ready()
	{
        sprite = FindChild("Sprite2D") as Sprite2D;
		magNow = weaponStatus.mag;
		magMax = weaponStatus.mag;
    }

	public override void _Process(double delta)
	{
		LogicProcess(delta);
		base._Process(delta);
    }

    float cooldown = 0;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

	void LogicProcess(double delta)
	{
		if(!isEquiped) return; 

        if(inputMap["Fire"]())
            if (cooldown < 0 && magNow > 0)
                Fire();
		
		
		cooldown -= (float)delta;
		if(cooldown < 0) cooldown = -0.01f;
	}


	public int magMax, magNow;
	void Fire()
    {
		cooldown += (float)60 / weaponStatus.rpm;
		magNow--;

        Projectile proj = ResourceLoader.Load<PackedScene>("res://Prefab/projectile.tscn").Instantiate() as Projectile;
		proj.GlobalPosition = muzzleNode.GlobalPosition;
		proj.GlobalRotation = GlobalRotation;
		
		proj.speed = weaponStatus.muzzleSpeed;
		proj.damage = weaponStatus.damage;

        GetTree().Root.AddChild(proj);
    }
}


    #region [총기 기본 정보]
    public enum CaliberType
    {
        p38,
        p45,
        mm9x18,
        mm9x19,
        mm5p56x45,
        mm7p62x51,
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
        CLOSED_BOLT,    //폐쇄 노리쇠
        OPEN_BOLT,      //개방 노리쇠
        MANUAL_RELOAD,  //수동 약실 장전
        NONE,           //볼트 없음
    }
    public enum MagazineType
    {
        MAGAZINE,   //박스형 탄창 - 빠른 교체
        SYLINDER,   //탄창X 약실만 - 중절식, 리볼버 등 탄창 개념이 없음.
        TUBE,       //튜브형 탄창 - 전탄 소진 시, 약실 장전
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
            public struct HipData   //지향 사격
            {
                public struct HipStancelData    //자세 값
                {
                    /// <summary>
                    /// 지향 자세 회복 속도
                    /// </summary>
                    public float recovery;
                    /// <summary>
                    /// 지향 자세 정확도
                    /// </summary>
                    public float accuracy;
                    /// <summary>
                    ///  정확도 변인 [엄폐, 걷기]
                    /// </summary>
                    public (float crounch, float walk) accuracyAdjust;
                }
                /// <summary>
                /// 자세 값
                /// </summary>
                public HipStancelData stance;

                public struct HipRecoilData     //반동 값
                {
                    /// <summary>
                    /// 지향 반동 회복 속도
                    /// </summary>
                    public float recovery;
                    /// <summary>
                    /// 회복 속도 변인 [엄폐, 걷기]
                    /// </summary>
                    public (float crounch, float walk) recoveryAdjust;
                    /// <summary>
                    /// 지향 반동 크기
                    /// </summary>
                    public float strength;
                }
                /// <summary>
                /// 반동 값
                /// </summary>
                public HipRecoilData recoil;

                /// <summary>
                /// 트래깅 속도
                /// </summary>
                public float traggingSpeed;     
            };
            /// <summary>
            /// 지향 사격
            /// </summary>
            public HipData hip;

            public struct AdsData   //조준 사격
            {
                public struct AdsStancelData    //자세 값
                {
                    /// <summary>
                    /// 조준 자세 정확도
                    /// </summary>
                    public float accuracy;
                    /// <summary>
                    /// 정확도 변인 [엄폐, 걷기]
                    /// </summary>
                    public (float crounch, float walk) accuracyAdjust;
                }
                /// <summary>
                /// 자세 값
                /// </summary>
                public AdsStancelData stance;

                public struct AdsRecoilData    //반동 값
                {
                    /// <summary>
                    /// 조준점 반동 고정
                    /// </summary>
                    public Vector2 fix;   
                    /// <summary>
                    /// 조준점 반동 랜덤
                    /// </summary>
                    public Vector2 random;
                    /// <summary>
                    /// 조준점 반동 회복 속도
                    /// </summary>
                    public float recovery;
                    /// <summary>
                    /// 지향 반동 크기 변인
                    /// </summary>
                    public (float crounch, float walk) strengthAdjust;
                }
                /// <summary>
                /// 반동 값
                /// </summary>
                public AdsRecoilData recoil;

                /// <summary>
                /// 절대 명중률(거리 1000 기준)
                /// </summary>
                public float moa;

                /// <summary>
                /// adsData 라이브러리에서 adsData를 찾는 키값
                /// </summary>
                public string adsName;
            };
            /// <summary>
            /// 조준 사격
            /// </summary>
            public AdsData ads;

        }

        //행동 소요 시간 정보 
        public TimeData timeDt;
        public struct TimeData
        {
            public float adsTime;       //조준 속도
            public float sprintTime;    //질주 후 사격 전환 속도
            /// <summary>
            /// 재장전 속도 - 박스(분리 / 결합 (장전)) - 실린더(사출 / 장전 / 결합) - 내부(볼트 재낌 /장전) - (장전 준비 / (약실 장전) / 튜브 장전 )
            /// </summary>
            public (float, float, float) reloadTime;
            public float swapTime;      //무기 교체 속도
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
            public float RoundDelay =>  60f / roundPerMinute;
            public float roundPerMinute;
            public int chamberSize; //약실 크기
            public List<Type> magazineWhiteList;  //장착 가능한 탄창리스트
            public float muzzleVelocity;    //총구 속도
            public float effectiveRange;    //유효 사거리
            public float barrelLength;      //총기 전장
            public float loudness;          //소음 크기 (거리로 ex 10000)
        }

        public AttachData attachDt;
        public struct AttachData
        {
            //internal List<AttachSocket> socketList;
        }
    }
    public static class WeaponStatusEx
    {
        // internal static List<AttachSocket> CopyList(this AttachData attachDt) 
        // {
        //     List < AttachSocket >  list = new List<AttachSocket>(attachDt.socketList);

        //     for (int i = 0; i < list.Count; i++) 
        //     {
        //         AttachSocket socket = list[i];
        //         if (socket.attachment != null) 
        //         {
        //             AttachSocket originSocket = attachDt.socketList[i];
        //             Type originType = originSocket.attachment.GetType();
        //             var newItem = Activator.CreateInstance(originType);
        //             socket.attachment = newItem as IAttachment;
        //         }
        //     }

        //     return list;
        // }
    }
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
            WeaponDataLoad();
        }

        static Dictionary<string, WeaponStatus> weaponLib;
        static void WeaponDataLoad() 
        {
            //정적 생성자를 불러오는 역할?
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
