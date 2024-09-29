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
    public partial class Boss : Enemy
    {
        public new BossData.Type type = BossData.Type.NONE;

        public int phase = 0; //0번은 준비, 1페이즈, 2페이즈 등등

        public override void GetDamage(EDamage damage)
        {
            base.GetDamage(damage);
        }

        protected override void OnHit()
        {
            base.OnHit();
        }
        protected override void OnDown()
        {
            base.OnDown();

        }
        protected override void OnStagger()
        {
            base.OnStagger();

        }


        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);
            LoadRoom();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                ProcessOnAuthority((float)delta);


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

        protected override void ProcessOnAuthority(float delta) { }


        public virtual void LoadRoom() { }
        public virtual void Finished() { }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if(IngameManager.boss != this)
                    IngameManager.boss = null;

            base.Dispose(disposing);
        }
    }
}