using _favorClient.library.DataType;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public partial class Humanoid : CharacterBody2D
    {
        //RPC ID
        public int rpcOwner = 1;

        //스텟
        public (float max, float now) health = (500, 500);
        public (float original, float now) speed = (0.85f, 0.85f);
        public bool isAlive => health.now > 0;

        //싱크값
        protected Vector2 syncPos = Vector2.Zero;
        protected float healthSync = 500f, syncRot;

        //핸드 값
        public Node2D hands => GetNode("./Hands") as Node2D;

        public override void _EnterTree()
        {
            GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(rpcOwner);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
            {
                Vector2 velocity = Velocity;

                //살아있고 의식 있음
                if (isAlive)
                {
                    if (Input.IsMouseButtonPressed(MouseButton.Left))
                        Rpc("DoAttackMain", GlobalPosition, GetGlobalMousePosition());
                    if (Input.IsMouseButtonPressed(MouseButton.Right))
                        Rpc("DoAttackSub", GlobalPosition, GetGlobalMousePosition());

                    if (Input.IsKeyPressed(Key.Space))
                        Rpc("DoDodge", GlobalPosition, GetGlobalMousePosition());

                    if (Input.IsKeyPressed(Key.Q))
                        Rpc("DoSkillMain", GlobalPosition, GetGlobalMousePosition());
                    if (Input.IsKeyPressed(Key.E))
                        Rpc("DoSkillSub", GlobalPosition, GetGlobalMousePosition());
                    if (Input.IsKeyPressed(Key.R))
                        Rpc("DoUltimate", GlobalPosition, GetGlobalMousePosition());

                    ActionProcess((float)delta);

                    float rotation = (GetGlobalMousePosition() - hands.GlobalPosition).Angle();
                    hands.GlobalRotation = rotation;

                    //이동불가 상태가 아님
                    {
                        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");

                        if (direction != Vector2.Zero)
                            velocity += direction * speed.now;

                        else
                            velocity = new(Mathf.MoveToward(velocity.X, 0, speed.now), Mathf.MoveToward(velocity.Y, 0, speed.now));

                        velocity *= 0.95f;
                    }
                }

                //이동불가 상태가 아님
                Velocity = velocity;
                var collision = MoveAndCollide(Velocity);
                if (collision != null)
                    if (collision.GetCollider() != null)
                    {
                        var collider = collision.GetCollider();
                        if (collider is Boss boss || collider is Character character)
                        {
                            Velocity += -(((Node2D)collider).GlobalPosition - this.GlobalPosition).Normalized() * 10f;
                            GlobalPosition += -(((Node2D)collider).GlobalPosition - this.GlobalPosition).Normalized() * 1f;
                        }
                    }

                syncPos = GlobalPosition;
                syncRot = hands.GlobalRotation;
                healthSync = health.now;
            }
            else
            {
                GlobalPosition = GlobalPosition.Lerp(syncPos, .1f);
                hands.GlobalRotation = Mathf.Lerp(hands.GlobalRotation, syncRot, .1f);
                health.now = healthSync;
            }

        }

        //기본 상호작용 함수
        public bool GetDamage(Damage damage)
        {
            if (isAlive == false) return false;
            //if (shield.now > 0)
            //{
            //    if (damage.damage > shield.now)
            //    {
            //        damage.damage -= shield.now;
            //        shield.now = 0f;
            //    }
            //    else 
            //    {
            //        shield.now -= damage.damage;
            //        damage.damage = 0f;
            //    }
            //}

            if (damage.damage > health.now)
            {
                damage.damage -= health.now;
                health.now = 0f;
                DoDead();
            }
            else
            {
                health.now -= damage.damage;
                damage.damage = 0f;
            }


            return true;
        }
        public bool GetHeal(float heal)
        {
            if (isAlive == false) return false;
            if (health.now >= health.max) return false;


            health.now = Mathf.Min(health.now + heal, health.max);

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
            this.Modulate = new Color(1f, 1f, 1f);
        }

        //액션 프로세스
        protected (string state, float now, float max) action = ("idle", 0, 0);
        protected virtual void ActionProcess(float delta) {
            action.now -= delta;
        }

        protected virtual void OnLMB(bool justPressed, bool isPressing) { }
        protected virtual void OnRMB(bool justPressed, bool isPressing) { }
        protected virtual void OnInteract(bool justPressed, bool isPressing) { }
        protected virtual void OnReload(bool justPressed, bool isPressing) { }

    }
}
