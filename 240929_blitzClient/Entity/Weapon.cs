using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Weapon : Node2D
    {
        //{
        //    get {
        //        return GetNode("./Magazine") as Magazine;
        //    }
        //    set {
        //        if (magazine != null)
        //        {
        //            magazine.QueueFree();
        //            AddChild(value);
        //            value.Position = magPos.Position;
        //        }
        //    }
        //}   //탄창
        [Export]
        private Node2D rightHand;   //오른쪽손
        [Export]
        private Node2D leftHand;    //왼쪽손
        [Export]
        private Node2D muzzle;  //총구
        [Export]
        private Node2D ejecter; //약실
        [Export]
        private Node2D bolt; //볼트
        [Export]
        private Node2D magPos;  //탄창 접합부

        [Export]
        private AnimationPlayer anim;
        [Export]
        private PackedScene projPrefab;
        [Export]
        private PackedScene magPrefab;

        public Humanoid equiped
        {
            get
            {
                Node2D hands = GetParent() as Node2D;
                Humanoid humanoid = hands.GetParent() as Humanoid;

                return humanoid;
            }
        }


        public string dataName = "M4A1";
        public WeaponStatus wStat => WeaponStatus.statLib[dataName];
        public Magazine magazine = new(){ magCount = (30,30) };

        public (string type, float now, float max) action = ("idle", -1f, -1f);
        public float aimValue = 0f;

        public override void _Ready() { }

        Func<(string type, float now, float max), float, float, bool> JustPassed =
            (action, delta, t) => action.now + delta >= action.max * t && action.max * t > action.now;
        public override void _Process(double delta)
        {
            action.now -= (float)delta;
            firePast += (float)delta;

            switch (action.type)
            {
                case "idle": {
                        //조준
                        aimValue += input.onAim ? (float)delta / wStat.time.aim : -(float)delta / wStat.time.aim;
                        aimValue = Math.Clamp(aimValue, 0f, 1f);

                        if (input.justFire || (input.onFire && wStat.mechanism.isAuto))
                            if(firePast > fireDelay)
                                Fire();

                        if(input.reload)
                            action = ("reload", wStat.time.reload, wStat.time.reload);

                    } break;
                case "reload": {
                        aimValue = Math.Clamp(aimValue - (float)delta / wStat.time.aim, 0f, 1f);

                        if (JustPassed(action, (float)delta, 0.7f))
                            magazine.magCount.now = 0;


                        if (JustPassed(action, (float)delta, 0.1f))
                            Reload();

                        if (action.now <= 0)
                            if(inChamber == true)
                                action = ("idle", -1, -1);
                            else
                                action = ("charging", 0.4f, 0.4f);
                    } break;
                case "charging":
                    {
                        if (action.now <= 0)
                        {
                            inChamber = magazine.GetFeed();
                            action = ("idle", -1, -1);
                        }
                    } break;
            }

        }

        public void Setinput((bool justFire, bool onFire, bool onAim, bool reload) input) 
        {
            this.input = input;
        }

        public (bool justFire, bool onFire, bool onAim, bool reload) input = (false, false, false, false);

        public float rpm => wStat.projectile.rpm;
        public float fireDelay => 60f / rpm;
        float firePast = 0f;

        protected bool inChamber = false;
        public float muzzleSpeed = 10.0f;
        
        public void Fire()
        {
            if (inChamber == false)
                return;

            inChamber = false;

            //투사체 발사
            Projectile proj = projPrefab.Instantiate<RigidBody2D>() as Projectile;
            proj.GlobalPosition = muzzle.GlobalPosition;
            proj.GlobalRotation = muzzle.GlobalRotation;
            proj.speed = muzzleSpeed;

            firePast = 0f;

            //피드
            inChamber = magazine.GetFeed();
        }


        public void Reload() 
        {
            magazine.magCount.now = magazine.magCount.mag;
        }

        //public void DettachMag() 
        //{
        //    magazine = null;
        //}
        //public void AttachMag() 
        //{
        //    magazine = GetMagazine();
        //}
        //protected Magazine GetMagazine() {
        //    Magazine mag = magPrefab.Instantiate<Magazine>();
        //    mag.magCount = (30, 30);
        //    //mag.sprite2D.Texture = ResourceLoader.Load<Texture2D>("");
        //    return mag;
        //}
    }
}
