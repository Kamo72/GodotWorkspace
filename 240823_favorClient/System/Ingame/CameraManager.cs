using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _favorClient.Entity;
using Godot;

namespace _favorClient.System.Ingame
{
    public partial class CameraManager : Camera2D
    {
        public Character target = null;


        public CameraManager() { }

        public void SetTarget(Character character) {
            target = character;

        }

        public override void _Process(double delta)
        {

            Vector2 tPos = (target.Position + GetGlobalMousePosition() * 0.4f) / 1.4f;

            GlobalPosition = (GlobalPosition + tPos * 0.1f) / 1.1f;


            base._Process(delta);
        }


    }
}
