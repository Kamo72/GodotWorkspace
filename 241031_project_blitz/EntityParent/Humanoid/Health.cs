using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.RenderingDevice;
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
        public float spNow = 100, spMax = 100, spReg = 10, spRed = 20;  //스테미나
        public float bleeding = 0, bleedingRatio;      //출혈
        public float epNow = 200f, epMax = 200f, epRed = -0.05f;  //에너지
        public float wpNow = 100f, wpMax = 100f, wpRed = -0.05f;  //수분
        public float concussionTime = 0;    //뇌진탕

        public bool isConcussion => concussionTime > 0; //뇌진탕 여부
        public bool isStarvation => epNow <= 0;  //기아 여부
        public bool isDehydration => wpNow <= 0; //탈수 여부
        public bool isSprintable => spNow > 0;  //질주 가능 여부

        public enum HitPart
        {
            HEAD,
            THORAX,
            LIMB,
            NONE,
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
        public void GetHemostasis(float hemostasis)
        {
            bleeding -= hemostasis;

            if (bleeding <= 0f)
                bleeding = 0f;
        }

        public void GetHeal(float heal)
        {
            hpNow += heal;

            if (hpNow > hpMax)
                hpNow = hpMax;
        }

        public void Process(float delta) 
        {
            DigestingProcess(delta);
            StaminaProcess(delta);
            BleedingProcess(delta);
        }


        public HitPart GetHitPart(Projectile projectile, Vector2 collisionPoint)
        {
            AmmoStatus ammoStatus = projectile.ammoStatus;

            //헤드 판정
            float collisionRadius = 35f;
            float headDistance = (projectile.aimPos - master.Position).Length();
            if (headDistance < collisionRadius)
            {
                GetWoundEffect(collisionPoint);
                return HitPart.HEAD;
            }

            //판정 데이터
            float effectiveRange = projectile.weaponStatus.detailDt.effectiveRange;
            float aimLength = (projectile.startPos - projectile.aimPos).Length();
            float hitLength = (projectile.startPos - collisionPoint).Length();
            float errorDistance = Mathf.Abs(aimLength - hitLength);

            //초과 사거리 조준 여부
            bool tryOutOfRange = aimLength > effectiveRange;


            //
            (float inner, float outer) judgeLen = (300, 300);

            if (tryOutOfRange)//초과 사거리 조준
            {
                float overLen = aimLength - effectiveRange;

                float ratio = 1f / ((200f + overLen) / 200f);
                //0 100 200 300...
                //1 2/3 1/2 2/5

                //GD.Print("effectiveRange : " + effectiveRange);
                judgeLen = (
                    judgeLen.inner * MathF.Pow(ratio, 1.1f),
                    judgeLen.outer * MathF.Pow(ratio, 0.9f) + 70
                    );
            }
            else//유효 사거리 조준
            {
                float ratio = aimLength / effectiveRange;
                //er = 150
                //0 50 100 150
                //0 1/3 2/3 1
                //GD.Print("effectiveRange : " + effectiveRange);
                judgeLen = (
                    judgeLen.inner * ratio,
                    judgeLen.outer * MathF.Pow(ratio, 1.1f)
                    );
            }

            #region  DEBUG
            //Line2D line2D = new Line2D();
            //WorldManager.instance.AddChild(line2D);
            //line2D.Position = Vector2.Zero;
            //line2D.Points = new Vector2[2] {
            //    projectile.aimPos + Vector2.FromAngle(projectile.direction) * judgeLen.outer,
            //    projectile.aimPos - Vector2.FromAngle(projectile.direction) * judgeLen.inner};
            //line2D.DefaultColor = Colors.Blue;

            //line2D = new Line2D();
            //WorldManager.instance.AddChild(line2D);
            //line2D.Position = Vector2.Zero;
            //line2D.Points = new Vector2[2] {
            //    projectile.aimPos + Vector2.FromAngle(projectile.direction) * judgeLen.outer * 0.5f,
            //    projectile.aimPos - Vector2.FromAngle(projectile.direction) * judgeLen.inner * 0.5f};
            //line2D.DefaultColor = Colors.Red;

            //line2D = new Line2D();
            //WorldManager.instance.AddChild(line2D);
            //line2D.Position = Vector2.Zero;
            //line2D.Points = new Vector2[2] {
            //    collisionPoint,
            //    collisionPoint + Vector2.One  
            //};
            //line2D.DefaultColor = Colors.Black;


            //GD.Print("에임 초과 명중 : " + (hitLength > aimLength));
            //GD.Print("값 : " + (hitLength <= aimLength? errorDistance-70 : errorDistance));
            //GD.Print("기준 : " + (hitLength > aimLength ? judgeLen.outer : judgeLen.inner));


            #endregion  DEBUG


            HitPart result = HitPart.NONE;

            float hitDist = hitLength > aimLength ? errorDistance : errorDistance - 70;
            float checkDist = hitLength > aimLength? judgeLen.outer : judgeLen.inner;

            if (hitDist < checkDist * 0.5f)
                result = HitPart.THORAX;
            else if (hitDist < checkDist)
                result = HitPart.LIMB;


            if (result != HitPart.NONE)
                GetWoundEffect(collisionPoint);

            return result;

        }

        void GetWoundEffect(Vector2 collisionPoint)
        {
            Vector2 rand = Vector2.FromAngle((float)Random.Shared.NextDouble() * 360f) * 10f * (float)Random.Shared.NextDouble();

            Sprite2D sprite2D = new Sprite2D();
            sprite2D.Texture = ResourceLoader.Load("res://Asset/Particle/RadialAlphaGradient.png") as Texture2D;
            sprite2D.Scale = Vector2.One * 0.02f;
            sprite2D.Modulate = Colors.DarkRed;
            master.AddChild(sprite2D);
            sprite2D.GlobalPosition = collisionPoint + rand;
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