using _favorClient.Entity;
using _favorClient.library.DataType;
using _favorClient.System.Ingame;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;
using static System.Collections.Specialized.BitVector32;

namespace _favorClient.EntityImplemented.BossKnight
{
    public partial class BossKnight : Boss
    {
        [Export]
        PackedScene projNormalSwift;
        [Export]
        PackedScene projNormalThrust;

        AnimationPlayer anim => hands.GetNode<AnimationPlayer>("AnimationPlayer");

        public BossKnight()
        {
            type = library.DataType.BossData.Type.KNIGHT;
            speed = (4, 4);
        }

        public override void LoadRoom()
        {
            //TODO
        }

       
        
        protected override void ProcessOnAuthority(float delta)
        {
            //GD.Print($"[{action.type}] {action.now}/{action.max}");
            
            passedTime += delta;
            action.now -= delta;
            attackTimeNow -= delta;

            switch (action.type)
            {
                case "idle":
                    {
                        //타겟 탐색
                        if (target == null)
                        {
                            toMovePos = GlobalPosition;

                            float disClosest = 999999f;
                            foreach (var charac in IngameManager.characters)
                            {
                                if (charac == null) continue;

                                float dis = (GlobalPosition - charac.GlobalPosition).Length();

                                if (dis < disClosest && dis < aggroRange)
                                {
                                    disClosest = dis;
                                    target = charac;
                                }
                            }
                        }
                        else
                        {
                            float dis = (GlobalPosition - target.GlobalPosition).Length();
                            Vector2 dir = (GlobalPosition - target.GlobalPosition).Normalized();
                            //공격 범위 밖
                            if (dis > attackRange)
                            {
                                toMovePos = GlobalPosition + dir * (dis - attackRange);
                            }
                            //공격 불가
                            else if (attackTimeNow > 0f)
                            {
                                toMovePos = GlobalPosition + new Vector2(
                                    noise.GetNoise3D(seed + passedTime, GlobalPosition.X, GlobalPosition.Y),
                                    noise.GetNoise3D(seed + passedTime, GlobalPosition.Y, GlobalPosition.X + 1231)) * 150;
                            }
                            //공격 가능
                            else
                            {
                                float value = (float)Random.Shared.NextDouble();

                                if (value < 0.4f && attackTimeNow < 0)
                                    SetAction("normalSwift", 1.4f);

                                else if (value < 0.7f)
                                    SetAction("dodge", 0.5f);

                                else if (value < 1.1f && attackTimeNow < 0)
                                    SetAction("normalThrust", 1.9f);
                            }

                        }

                        if ((toMovePos - GlobalPosition).Length() > 50)
                        {
                            hands.GlobalRotation = (target.GlobalPosition - GlobalPosition).Angle() + noise.GetNoise1D(passedTime * 3 + seed +134) * 5
                                * (2 / 360 * (float)Math.PI);
                            
                            hands.Position = new Vector2(
                                noise.GetNoise2D(passedTime * 3 + seed, GlobalPosition.Length()),
                                noise.GetNoise2D(passedTime * 3 + seed, GlobalPosition.Length() + 1241)
                                ) * 20;

                            Velocity += (GlobalPosition - toMovePos).Normalized() * speed.now;
                            Velocity *= 0.95f;
                        }
                    }
                    break;
                case "normalSwift":
                    {
                        if (action.now + delta >= action.max && action.now < action.max)
                        {
                            anim.CurrentAnimation = "normalSwift";
                            attackTimeNow = attackTimeMax;
                        }

                        if (action.now + delta >= 0.9f && action.now < 0.9f)
                            Rpc("InstantiateNomalSwift",
                                GlobalPosition + Vector2.FromAngle(hands.GlobalRotation) * 00f,
                                hands.GlobalRotation);

                        if (action.now + delta >= 0f && action.now < 0f)
                        {
                            hands.Scale = new(1, -hands.Scale.Y);
                            anim.CurrentAnimation = "idle";
                            SetAction("idle");
                        }

                    } break;
                case "normalThrust":
                    {
                        if (action.now + delta >= action.max && action.now < action.max)
                        {
                            anim.CurrentAnimation = "normalThrust";
                            attackTimeNow = attackTimeMax;
                        }

                        if (action.now + delta >= 1.1f && action.now < 1.1f)
                            Rpc("InstantiateNomalThrust",
                                GlobalPosition + Vector2.FromAngle(hands.GlobalRotation) * 00f,
                                hands.GlobalRotation);

                        if (action.now + delta >= 0f && action.now < 0f)
                        {
                            hands.Scale = new(1, -hands.Scale.Y);
                            anim.CurrentAnimation = "idle";
                            SetAction("idle");
                        }
                    }
                    break;
                case "normal_dodge":
                    {
                        if (action.now + delta >= action.max && action.now < action.max)
                        {
                            rot = (toMovePos - GlobalPosition).Angle();
                        }
                        if (0 < action.now && action.now < action.max) 
                        {
                            Velocity = Vector2.FromAngle(rot) * action.now / action.max * 10;
                        }
                        if (action.now + delta >= 0f && action.now < 0f)
                        {
                            SetAction("idle");
                        }
                    }
                    break;

            }
            
        }
        float rot = 0f;

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected void InstantiateNomalSwift(Vector2 pos, float angle) 
        {
            var pp = projNormalSwift.Instantiate<Node2D>() as PProjectile;
            pp.GlobalPosition = pos;
            pp.GlobalRotation = angle;
            pp.SetOwner(this, true);

            IngameManager.instance.AddChild(pp);
            GD.PushWarning("InstantiateNomalSwift called {pos} {angle}");
        }

        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected void InstantiateNomalThrust(Vector2 pos, float angle)
        {
            var pp = projNormalSwift.Instantiate<Node2D>() as PProjectile;
            pp.GlobalPosition = pos;
            pp.GlobalRotation = angle;
            pp.SetOwner(this, true);

            IngameManager.instance.AddChild(pp);
            GD.PushWarning("InstantiateNomalSwift called {pos} {angle}");
        }
    }
}
