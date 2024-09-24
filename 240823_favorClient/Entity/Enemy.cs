using _favorClient.library.DataType;
using _favorClient.System.Ingame;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Enemy : CharacterBody2D
    {
        public BossData.Type type = BossData.Type.NONE;

        public int phase = 0; //0번은 준비, 1페이즈, 2페이즈 등등
        public float nowHealth = 10000, nowStagger = 10000;
        public float maxHealth = 10000, maxStagger = 10000;

        public float stunDuration = -1f;
        public bool isStunned = false, isInvincible = false, isHittable = true;


        public void GetDamage(BDamage damage)
        {
            if (isHittable == false) return;

            OnHit();

            if (isInvincible) return;

            nowHealth -= damage.damage;
            nowStagger -= damage.stagger;

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

        protected Vector2 syncPos = Vector2.Zero;
        protected float staggerSync = 10000, healthSync = 10000, syncRot;

        public Node2D hands => GetNode("./Hands") as Node2D;


        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
            LoadRoom();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                Vector2 velocity = Velocity;

                ProcessOnAuthority();

                Velocity = velocity;
                MoveAndSlide();


                if (isStunned)
                    if (stunDuration - (float)delta < 0 && 0 < stunDuration)
                    {
                        isStunned = false;
                        nowStagger = maxStagger;
                    }

                stunDuration -= (float)delta;


                syncPos = GlobalPosition;
                syncRot = hands.GlobalRotation;
                healthSync = nowHealth;
                staggerSync = nowStagger;
            }
            else
            {
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                hands.GlobalRotation = Mathf.Lerp(hands.GlobalRotation, syncRot, .1f);
                nowHealth = healthSync;
                nowStagger = staggerSync;
            }
        }

        protected virtual void ProcessOnAuthority() { }


        public virtual void LoadRoom() { }
        public virtual void Finished() { }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
