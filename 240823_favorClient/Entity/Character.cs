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

        public float healthMax = 800, health = 800;
        public float speed = 0.85f;
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

                //Fire 
                if (Input.IsActionJustPressed("fire"))
                {
                    Rpc("Fire", GetNode<Node2D>("GunRotation").RotationDegrees, GetNode<Node2D>("GunRotation/BulletSpawn").GlobalPosition);
                }

                Node2D gun = GetNode<Node2D>("GunRotation");
                float rotation = (GetGlobalMousePosition() - gun.GlobalPosition).Angle();
                gun.GlobalRotation = rotation;

                //Move 
                Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
                if (direction != Vector2.Zero)
                {
                    velocity = direction * speed;
                }
                else
                {
                    velocity.X = Mathf.MoveToward(velocity.X, 0, speed);
                    velocity.Y = Mathf.MoveToward(velocity.Y, 0, speed);
                }

                Velocity = velocity;
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





        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoAttackMain() { }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoAttackSub() { }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoDodge() { }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoSkillMain() { }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoSkillSub() { }
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        protected virtual void DoUltimate() { }








    }
}
