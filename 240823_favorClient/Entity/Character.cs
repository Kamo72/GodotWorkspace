using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Character : CharacterBody2D
    {
        [Export]
        Label nameTxt;

        public CharacterData.Type type = CharacterData.Type.NONE;

        public CharacterData data => CharacterData.GetByType(type);

        public int id;

        public (float now, float dur) shield = (0f, -1f);
        public (float max, float now) health = (800, 800);
        public (float max, float now, bool cancelable)? casting = null; 
        public (float original, float now) speed = (0.85f, 0.85f);
        public float stunDur = -1f, unstoppableDur = -1f, rootDur = -1f, undamagableDur = -1f,
            undyingDur = -1f, silenceDur = -1f;

        public bool isAlive => health.now > 0;
        public bool isRooted => isAlive && (rootDur< 0 || isUnstoppable);
        public bool isConcious => isAlive && (stunDur < 0 || isUnstoppable);
        public bool isCastable => isAlive && (isConcious && silenceDur < 0f) || isUnstoppable;
        public bool isUnstoppable => isAlive && unstoppableDur> 0f;
        public bool isDamagable => isAlive && undamagableDur > 0f;
        public bool isKillable => isAlive && undyingDur > 0f;
        public bool isCasting => isAlive && casting != null;

        private Vector2 syncPos = Vector2.Zero;
        private float syncRotation = 0f;

        public override void _Ready()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                Vector2 velocity = Velocity;

                //살아있고 의식 있음
                if (isAlive && isConcious)
                {
                    //캐스팅 중이지 않거나 캔슬 가능함
                    if (!isCasting || casting.Value.cancelable)
                    {

                        if (Input.IsMouseButtonPressed(MouseButton.Left))
                            Rpc("DoAttackMain", GlobalPosition, GetGlobalMousePosition());
                        if (Input.IsMouseButtonPressed(MouseButton.Right))
                            Rpc("DoAttackSub", GlobalPosition, GetGlobalMousePosition());

                        //이동불가 상태가 아님
                        if (!isRooted)
                            if (Input.IsKeyPressed(Key.Space))
                                Rpc("DoDodge", GlobalPosition, GetGlobalMousePosition());

                        //침묵 상태가 아님
                        if (isCastable)
                        {
                            if (Input.IsKeyPressed(Key.Q))
                                Rpc("DoSkillMain", GlobalPosition, GetGlobalMousePosition());
                            if (Input.IsKeyPressed(Key.E))
                                Rpc("DoSkillSub", GlobalPosition, GetGlobalMousePosition());
                            if (Input.IsKeyPressed(Key.R))
                                Rpc("DoUltimate", GlobalPosition, GetGlobalMousePosition());
                        }
                    }
                    action.time += (float)delta;
                    ActionProcess();

                    //Node2D gun = GetNode<Node2D>("GunRotation");
                    //float rotation = (GetGlobalMousePosition() - gun.GlobalPosition).Angle();
                    //gun.GlobalRotation = rotation;

                    //이동불가 상태가 아님
                    if (!isRooted)
                    {
                        //Move 
                        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
                        if (direction != Vector2.Zero)
                        {
                            velocity = direction * speed.now;
                        }
                        else
                        {
                            velocity.X = Mathf.MoveToward(velocity.X, 0, speed.now);
                            velocity.Y = Mathf.MoveToward(velocity.Y, 0, speed.now);
                        }
                    }
                }
                
                //캐스팅이 불가능하다면 캐스팅 중단
                if (isCastable == false && isCasting)
                    CancelCasting();

                //캐스팅이 불가능하다면 캐스팅 중단
                if (isCastable == false && isCasting)
                    CancelCasting();


                Velocity = velocity;

                shield.dur -= (float)delta;
                if (shield.dur < 0f)
                    shield.now = 0f;

                //이동불가 상태가 아님
                if (!isRooted)
                    MoveAndSlide();

                syncPos = GlobalPosition;
                syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
            }
            else
            {
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                GetNode<Node2D>("GunRotation").RotationDegrees = Mathf.Lerp(GetNode<Node2D>("GunRotation").RotationDegrees, syncRotation, .1f);
            }
        }

        public void SetupPlayer(string name)
        {
            //TODO
            nameTxt.Text = name;
        }



        public bool GetDamage(PDamage damage) 
        {
            if (isAlive == false) return false;
            if (isDamagable) 
            {
                if (shield.now > 0)
                {
                    if (damage.damage > shield.now)
                    {
                        damage.damage -= shield.now;
                        shield.now = 0f;
                    }
                    else 
                    {
                        shield.now -= damage.damage;
                        damage.damage = 0f;
                    }
                }

                if (damage.damage > health.now)
                {
                    damage.damage -= health.now;
                    health.now = isKillable? 0f : 1f;

                    if (isKillable) DoDead();
                }
                else
                {
                    health.now -= damage.damage;
                    damage.damage = 0f;
                }
            }

            if (isUnstoppable == false)
                damage.CC(this);
            
            return true;
        }
        public bool GetHeal(float heal)
        {
            if (isAlive == false) return false;
            if (health.now >= health.max) return false;


            health.now = Mathf.Min(health.now + heal, health.max);

            return true;
        }
        public bool GetShield(float shield, float dur)
        {
            if (isAlive == false) return false;

            this.shield = (this.shield.now + shield, MathF.Max(this.shield.dur, dur));

            return true;
        }
        public void DoDead() 
        {
            health.now = -1;
            action = ("idle", 0, 0);
            this.Modulate = new Color(0.5f, 0.5f, 0.5f);
        }
        public void DoRevive(float health)
        {
            this.health.now = 1;
            GetHeal(health);
            action = ("idle", 0, 0);
            this.Modulate = new Color(1f,1f,1f);
        }
        public void CancelCasting() 
        {
            if (casting.HasValue == false) return;
            if (isCasting == false) return;

            casting = null;
            
        }

        protected (string state, int idx, float time) action = ("idle", 0, 0);
        protected virtual void ActionProcess() { }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoAttackMain(Vector2 from, Vector2 to) { }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoAttackSub(Vector2 from, Vector2 to) { }
        
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoDodge(Vector2 from, Vector2 to) { }
        
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoSkillMain(Vector2 from, Vector2 to) { }
        
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoSkillSub(Vector2 from, Vector2 to) { }
        
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoUltimate(Vector2 from, Vector2 to) { }

    }


}
