
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid : CharacterBody2D
{
    public Aim aim;
    public class Aim
    {
        public Aim(Humanoid master) 
        {
            this.master = master;
        }
        public Humanoid master;

        public float healthNow;
        public float healthMax;

        // 조준점 관련 변수
        private Vector2 virtualAimPoint;
        protected Vector2 delayedRecoilVec = Vector2.Zero;
        private Vector2 recoilVec = Vector2.Zero;
        private float aimLength => (virtualAimPoint - master.GlobalPosition).Length();

        private static FastNoiseLite noise = new FastNoiseLite();

        public Vector2 realAimPoint
        {
            get
            {
                Vector2 aimPoint = virtualAimPoint;

                aimPoint += (recoilVec * aimLength / 1000 - master.GlobalPosition)
                    .Rotated(recoilVec.Length() * randFloat / 5000);

                aimPoint += master.GlobalPosition;

                // SimpleNoise를 기반으로 조준점 흔들림 계산
                float radius = 100f * (1f + recoilVec.Length() / 100f);
                float noiseX = noise.GetNoise2D(aimStableTime * 10f, 6945) * radius;
                float noiseY = noise.GetNoise2D(aimStableTime * 10f + 100, 1235) * radius; // 서로 다른 노이즈 값 생성
                aimPoint += new Vector2(noiseX, noiseY)
                    * aimLength / 1000;

                return aimPoint;
            }
        }


        protected float delayedRecoilRatio = 0.9f;
        protected float aimStableTime = 0f;

        protected float randFloat => ((float)Random.Shared.NextDouble() - 0.5f) * 2f;

        private Vector2 handPos;
        private float handRot;

        public void Process(float delta) 
        {
        
        }
    }
}
