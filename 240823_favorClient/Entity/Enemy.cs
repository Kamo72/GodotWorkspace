using _favorClient.library.DataType;
using _favorClient.System.Ingame;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public enum StunStrenth
    {
        NONE,
        LIGHT,
        HEAVY
    }

    public partial class Enemy : CharacterBody2D
    {
        //종류 값
        public EnemyData.Type type = EnemyData.Type.NONE;

        //체력, 스테커, 속도
        public float nowHealth = 10000, maxHealth = 10000;
        public float nowStagger = 10000, maxStagger = 10000;
        public (float ori, float now) speed = (0.85f, 0.85f);

        //경직
        public float stunDuration = -1f;
        public bool isStunned = false, isInvincible = false, isHittable = true;
        public StunStrenth stunStr = StunStrenth.NONE;

        public virtual void GetDamage(EDamage damage)
        {
            if (isHittable == false) return;

            OnHit();

            if (isInvincible) return;

            nowHealth -= damage.damage;
            nowStagger -= damage.stagger;

            if (nowHealth < 0) OnDown();
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

        public Node2D hands => GetNode("./Hands") as Node2D;

        //싱크 값들
        protected Vector2 syncPos = Vector2.Zero;
        protected float staggerSync = 10000, healthSync = 10000, syncRot;

        public static Noise noise = new FastNoiseLite();
        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(1);

            seed = (float)Random.Shared.NextDouble() * 10000;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                Vector2 velocity = Velocity;

                ProcessOnAuthority((float)delta);

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

        //액션 값 및 최소한의 정보
        protected float seed;
        protected Character target = null;
        protected float aggroRange = 2000f, attackRange = 300f;
        protected float attackTimeMax = 2f, attackTimeNow = -1f;
        protected (string type, float now, float max) action = ("idle", -1f, -1f);
        protected Vector2 toMovePos = new Vector2();
        protected float passedTime = 0f;
        
        //액션 관리 프로세스
        protected virtual void ProcessOnAuthority(float delta) {}

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected void SetAction(string type, float max = -1f, float now = -1f) =>
            action = (type, max, max < 0f ? now : max);
        
        //해제
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
