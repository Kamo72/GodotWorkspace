using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


public partial class Humanoid
{
    public Health health;
    public class Health
    {
        Humanoid master;
        public Health(Humanoid master, float health)
        {
            this.master = master;
            hpMax = health;
            hpNow = health;
        }

        public float hpNow, hpMax;  //최대 체력
        public float spNow = 100, spMax = 100, spReg = 6, spRed = 4;  //스테미나
        public float bleeding = 0, bleedingRatio;      //출혈
        public float epNow = 200f, epMax = 200f, epRed = -0.05f;  //에너지
        public float wpNow = 100f, wpMax = 100f, wpRed = -0.05f;  //수분
        public float concussionTime = 0;    //뇌진탕

        public bool isConcussion => concussionTime > 0; //뇌진탕 여부
        public bool isStarvation => epNow > 0;  //기아 여부
        public bool isDehydration => wpNow > 0; //탈수 여부
        public bool isSprintable => spNow > 0;  //질주 가능 여부

        public enum HitPart
        {
            HEAD,
            THORAX,
            LIMB,
        }

        public void GetDamage(Projectile projectile, HitPart hitPart)
        {
            var damage = projectile.ammoStatus.lethality.damage;
            hpNow -= damage;

            Sound.MakeSelf(master, master.GlobalPosition, 400f, 0.1f, soundDamageFlesh, 2);

            if (CameraManager.current.target == master)
                CameraManager.current.ApplyRecoil(damage * 10f);

            if (hpNow <= 0f)
                master.OnDead();
        }
        public void GetDamage(float damage)
        {
            hpNow -= damage;

            if (hpNow <= 0f)
                master.OnDead();
        }

        public void Process(float delta) 
        {
            DigestingProcess(delta);
            StaminaProcess(delta);
            BleedingProcess(delta);
        }



        void DigestingProcess(float delta)
        {
            epNow = Math.Clamp(epNow + epRed * delta, 0f, epMax);
            wpNow = Math.Clamp(wpNow + wpRed * delta, 0f, wpMax);

            if (isStarvation)
                GetDamage(1f * delta);

            if (isDehydration)
                GetDamage(1f * delta);
        }
        void StaminaProcess(float delta)
        {
            var diff = (master.movement.sprintValue > 0? -spRed : spReg);
            spNow = Math.Clamp(spNow + diff * delta, 0f, spMax);
            //GD.Print($"SP : {spNow} / {spMax} => {isSprintable}");
        }
        void BleedingProcess(float delta)
        {
            float damage = bleeding * bleedingRatio * delta;
            GetDamage(damage);
            bleeding -= damage;
        }

        void DoDead()
        {

            GD.Print($"{master.Name}가 사망했습니다.");

            //시체 생성
            Body body = ResourceLoader.Load<PackedScene>("res://Prefab/Dynamic/body.tscn").Instantiate() as Body;
            master.inventory.master = null;
            body.Initiate(master.inventory);


            //기타 추가적인 정보 전달
            master.GetParent().AddChild(body);
            body.Position = master.Position;

            if (CameraManager.current.target == master)
                CameraManager.current.target = null;

            master.QueueFree(); // 객체 삭제
        }

        string soundDamageFlesh => "res://Asset/SFX-Damage/GutsAndGoreHorrorSFXPackVol1/GutsAndGore_055.wav";
    }
}