using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public abstract partial class Intelligence
    {
        public Memory memory;
        public class Memory
        {
            /* Memory
             * 기억 능력을 담당하는 객체입니다.
             * Perception에서 얻은 정보를 이전의 기억과 조합해 유의미한 자료로 저장해
             * 다른 객체들이 이용할 수 있게 합니다.
             */
            #region 기본
            public Intelligence intelligence;
            public Humanoid master => intelligence.master;
            public Memory(Intelligence intelligence)
            {
                this.intelligence = intelligence;
                this.squadList = new() { intelligence.master };
            }
            #endregion

            #region 분대
            public List<Humanoid> squadList;
            public void AddSquad(Humanoid humanoid)
            {
                squadList.Add(humanoid);

                if (humanoid.intelligence != null)
                    humanoid.intelligence.memory.squadList = squadList;
            }
            public void DelSquad(Humanoid humanoid) 
            {
                squadList.Remove(humanoid);
            }
            #endregion
            
            #region 적 정보
            //적에 대한 예측
            public struct Prediction 
            {
                public Humanoid humanoid;   //대상
                public float pastTime;  //정보의 최신화 정도
                public Vector2 pos;  //추정 위치
                public float accuracy;  //정확도
            }

            public List<Prediction> enemyList = new();
            #endregion

            #region 파밍 정보
            public List<(bool lootable, IInteractable iInter)> lootList = new();
            #endregion

            public virtual void Process(float delta)
            {
                Perception perception = intelligence.perception;
                var foundLoots = perception.loots;
                var foundThreats = perception.threats;

            }


        }
    }
}
