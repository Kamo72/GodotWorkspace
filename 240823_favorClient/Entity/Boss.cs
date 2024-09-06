using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Boss : CharacterBody2D
    {
        public int phase = 0; //0번은 준비, 1페이즈, 2페이즈 등등
        public float nowHealth = 10000, nowStagger = 10000;
        public float maxHealth = 10000, maxStagger = 10000;

        public float stunDuration = -1f;
        public bool isStunned = false, isInvincible = false, isHittable = true;

        private Vector2 syncPos = Vector2.Zero;
        private float syncRotation = 0f;

        public void GetDamage(BDamage damage)
        {
            if(isHittable == false) return;

            OnHit();

            if (isInvincible) return;

            nowHealth -= damage.damage;
            nowStagger-= damage.stagger;

            if (nowHealth < 0) OnDown();
            if (nowStagger < 0) OnStagger();
        }

        protected virtual void OnHit()
        {

        }
        protected virtual void OnDown()
        {

        }
        protected virtual void OnStagger()
        {
            isStunned = true;
            stunDuration = 5;
        }


        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
        }
        public override void _Process(double delta)
        {
            stunDuration -= (float)delta;
            if (isStunned)
                if (stunDuration < 0)
                    isStunned = false;
        }
        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                Vector2 velocity = Velocity;

                ProcessOnAuthority();

                Velocity = velocity;
                MoveAndSlide();
                syncPos = GlobalPosition;
                //syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
            }
            else
            {
                ProcessOutOfAuthority();
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                //GetNode<Node2D>("GunRotation").RotationDegrees = Mathf.Lerp(GetNode<Node2D>("GunRotation").RotationDegrees, syncRotation, .1f);
            }
        }

        protected virtual void ProcessOnAuthority() { }
        protected virtual void ProcessOutOfAuthority() { }


        public virtual void LoadRoom()
        {
            GD.Print();
        }
        public virtual void Finished() { }
    }
}