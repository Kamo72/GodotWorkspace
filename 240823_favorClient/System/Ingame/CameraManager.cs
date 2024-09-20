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

        public Vector2 oriPos, resultPos;
        float shakingValue;

        public CameraManager() { }

        public void SetTarget(Character character) {
            target = character;
        }

        public override void _Process(double delta)
        {
            Vector2 tPos = (target.Position + GetGlobalMousePosition() * 0.4f) / 1.4f;

            oriPos = (oriPos + tPos * 0.1f) / 1.1f;

            ShakingProcess();
        }


        void ShakingProcess()
        {
            shakingValue *= 0.96f;

            resultPos = oriPos + new Vector2(
                (float)(Random.Shared.NextDouble() - 0.5f) * shakingValue * 10f,
                (float)(Random.Shared.NextDouble() - 0.5f) * shakingValue * 10f
                );
            GlobalPosition = resultPos;

            GlobalRotation *= 0.96f;
        }

        public void GetShaken(float power)
        {
            shakingValue += power;
            GlobalRotation = power * (float)(Random.Shared.NextDouble() - 0.5f);
        }


    }
}
