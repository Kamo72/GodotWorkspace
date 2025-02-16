using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class Humanoid
{
    public Movement movement;
    public class Movement
    {
        Humanoid master;
        public Movement(Humanoid master)
        {
            this.master = master;
        }

        private float speed { get {

                bool isWeaponEquipped = master.equippedWeapon != null;
                Func<WeaponStatus.MovementData> getMoveDt = () => master.equippedWeapon.status.moveDt;
                float speedRatio = isWeaponEquipped ?
                    Mathf.Lerp(getMoveDt().speed, 2.0f * getMoveDt().speedAdjust.sprint, sprintValue) :
                    Mathf.Lerp(1f, 2.0f, sprintValue);

                return 65f * speedRatio;
            
            } }  // 이동 속도 조절
        private float inertia = 0.15f; // 관성 계수 조절
        public float sprintValue = 0f;  // 질주 정도
        public bool sprintMaintain = false;

        public void PhysicProcess(Vector2 moveVec) 
        {
            // moveVec이 길이 1을 초과하면 normalize 처리
            if (moveVec.Length() > 1)
                moveVec = moveVec.Normalized();

            // 이동 처리: 관성을 적용해 이동
            master.LinearVelocity += moveVec * speed;
            master.LinearVelocity = master.LinearVelocity.Lerp(Vector2.Zero, inertia);
        }

        public void Process(float delta)
        {
            SprintProcess(delta);
            SoundProcess(delta);
        }


        void SprintProcess(float delta)
        {
            bool isSprint = master.intelligence.commandMap["Sprint"];

            if (master.intelligence.commandMap["SprintInit"])
                sprintMaintain = true;

            sprintMaintain = sprintMaintain && isSprint;

            if (master.health.isSprintable == false)
                sprintMaintain = false;

            //GD.Print("isSprint : " + isSprint);
            float transitionTime = master.equippedWeapon != null ? master.equippedWeapon.status.timeDt.sprintTime : 0.15f;

            sprintValue += (sprintMaintain ? 1 : -1) * delta / transitionTime;
            sprintValue = (float)Mathf.Clamp(sprintValue, 0f, 1f);
            //GD.Print(sprintValue);
        }

        float soundTime = 0f, soundMax = 1.7f;
        void SoundProcess(float delta) 
        {
            soundTime += master.LinearVelocity.Length() * delta / 100f;
            if (soundTime > soundMax)
            {
                Sound.MakeSelf(master, master.GlobalPosition, 150f, 0.3f, GetSoundRsc());

                //GD.Print("Sound!");
                soundTime -= soundMax;
            }

            if (master.LinearVelocity.Length() < 20f && soundTime > 0.1f)
            {
                Sound.MakeSelf(master, master.GlobalPosition, 150f, 0.3f, GetSoundRsc());   

                //GD.Print("Sound!");
                soundTime = 0f;
            }
        }

        string GetSoundRsc()
        {
            return $"res://Asset/SFX-Human/Footsteps-Essentials/Footsteps_Tile/Footsteps_Tile_Walk/Footsteps_Tile_Walk_0{Random.Shared.Next() % 8 + 1}.wav";
        }
    }
}