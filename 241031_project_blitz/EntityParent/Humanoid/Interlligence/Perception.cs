using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public abstract partial class Intelligence
    {
        public Perception perception;
        public class Perception
        {
            /* Perception
             * 인지 능력을 담당하는 객체입니다. 감각기관으로 정보를 수집합니다.
             * 시각, 청각, 촉각 등을 이용해 정보를 얻습니다.
             */
            #region 기본
            public Intelligence intelligence;
            public Humanoid master => intelligence.master;
            public Perception(Intelligence intelligence)
            {
                this.intelligence = intelligence;
            }
            #endregion

            #region 감각 능력
            protected ( //시야 범위
                (float range, float angle) peace,
                (float range, float angle) combat) sightArea = ((600, 60), (1200, 120));

            protected float visiblePower = 100f;    //시각 능력 - 높을 수록 좋음 기본 100
            protected float hearingPower = 100f;    //청각 능력 - 높을 수록 좋음 기본 100
            protected float peacefulAdjust = 0.5f;  //평화 상태 능력 조정값 기본 50%
            protected bool inCombat = false;
            #endregion

            #region 저장공간
            public struct Threaten
            {
                public enum Type
                {
                    SOUND,  //청각 정보
                    VISUAL, //시각 정보
                    DAMAGE, //피격
                }

                public Humanoid humanoid; //위협 원천
                public Vector2 position;    //위협 위치
                public Type type;   //위협 유형
                public float accuracy;  //정확도
                public float danger;    //위험도
            }

            public List<Threaten> damageThreatsBuffer;
            public List<Threaten> threats;
            public List<IInteractable> loots;
            #endregion

            public virtual void Process(float delta)
            {
                threats = new List<Threaten>();

                VisionProcess(delta);
                HearingProcess(delta);
            }

            List<Sound> getSoundList => new();
            void HearingProcess(float delta)
            {
                List<Sound> sounds = getSoundList;

                //소리 객체 확인
                foreach (var target in sounds)
                {
                    float dist = (master.GlobalPosition - target.GlobalPosition).Length();
                    float distRatio = 1f - dist / target.MaxDistance;

                    if (target.source == null)
                        continue;

                    if (dist <= target.MaxDistance)
                        continue;

                    threats.Add(new Threaten()
                    {
                        accuracy = distRatio,
                        danger = target.danger,
                        humanoid = target.source,
                        position = target.GlobalPosition,
                        type = Threaten.Type.SOUND,
                    });
                }
            }

            List<Humanoid> getHumanoidList => new();
            void VisionProcess(float delta)
            {
                List<Humanoid> humanoids = getHumanoidList;

                List<Humanoid> newList = new();
                var nowSight = inCombat ? sightArea.combat : sightArea.peace;
                Npc npc = master as Npc;

                //거리 확인
                foreach (var target in humanoids)
                {
                    float dist = (master.GlobalPosition - target.GlobalPosition).Length();

                    if (dist <= nowSight.range)
                        continue;

                    newList.Add(target);
                }
                humanoids = newList;
                newList = new();

                //각도 확인
                foreach (var target in humanoids)
                {
                    Vector2 vecTarget = master.GlobalPosition - target.GlobalPosition,
                        vecAim = master.GlobalPosition - master.realAimPoint;

                    if (Mathf.Abs(vecTarget.AngleTo(vecAim)) < Mathf.DegToRad(nowSight.angle))
                        continue;

                    newList.Add(target);
                }
                humanoids = newList;
                newList = new();

                //장애물 확인
                foreach (var target in humanoids)
                {
                    var vision = npc.CheckLineOfSight(target);

                    if (!vision.isVisible)
                        continue;

                    threats.Add(new Threaten()
                    {
                        accuracy = vision.visibility,
                        danger = 1f,
                        humanoid = target,
                        position = target.GlobalPosition,
                        type = Threaten.Type.VISUAL,
                    });
                }
            }

            List<IInteractable> getLootable => new();
            void LootableProcess(float delta)
            {
                List<IInteractable> lootables = getLootable;

                //TODO

                //loots.Add(null);
            }

            public void GetDamageThreat(Projectile proj)
            {
                //TODO
            }
        }
    }
}
