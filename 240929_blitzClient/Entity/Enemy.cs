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
    public partial class Enemy : Humanoid
    {
        //싱크 값들
        public static Noise noise = new FastNoiseLite();
        public override void _Ready()
        {
            base._Ready();
            seed = (float)Random.Shared.NextDouble() * 10000;
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }

        //액션 값 및 최소한의 정보
        protected float seed;
        protected Character target = null;
        protected float aggroRange = 2000f, attackRange = 300f;
        protected float attackTimeMax = 2f, attackTimeNow = -1f;
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
