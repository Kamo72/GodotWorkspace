using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public abstract partial class Intelligence
    {
        public Tactical tactical;
        public class Tactical
        {
            /* Tactical
             * Spacefinder, Memory, Perception 등으로부터 얻은 정보와 전투 성향, 보유 자원 등을 이용해서 
             * 취할 전술적 행동을 결정하고, 이를 Command로 변경하거나, PathFinder에게 넘기는 객체.
             */
            #region 기본
            public Intelligence intelligence;
            public Humanoid master => intelligence.master;
            public Tactical(Intelligence intelligence)
            {
                this.intelligence = intelligence;
            }
            #endregion

            #region 경보 단계
            public enum AlertLevel
            {
                UNCONSCIOUS,//무의식
                PEACEFUL,   //일상
                PATROL,  //순찰

                WARNING,  //약 경계
                ALERT,  //강 경계

                ENGAGING, //교전 중
                SPOTTED, //적 포착
            }
            public AlertLevel alertLevel = AlertLevel.PATROL;
            public bool IsOrUpper(AlertLevel compare) => (int)alertLevel >= (int)compare;
            public bool IsOrLower(AlertLevel compare) => (int)alertLevel <= (int)compare;

            public virtual void Process(float delta)
            {
                //TODO
                //decisionTree.Classify(intelligence);
            }

            #endregion

            #region 의사 결정 트리
            class DecisionNode
            {
                public string Condition { get; set; }
                public List<(Func<object, bool> Decider, DecisionNode Node)> DecisionPairs { get; private set; } = new();
                public DecisionNode DefaultDecider { get; private set; }

                // 노드 구성 메서드
                public DecisionNode AddDecision(Func<object, bool> decider, DecisionNode node)
                {
                    DecisionPairs.Add((decider, node));
                    return this; // 체이닝을 위해 this 반환
                }
                public DecisionNode AddDecision(Func<object, bool> decider, string Condition)
                {
                    DecisionPairs.Add((decider, new DecisionNode() { Condition = Condition }));
                    return this; // 체이닝을 위해 this 반환
                }

                public DecisionNode SetDefault(DecisionNode node)
                {
                    DefaultDecider = node;
                    return this; // 체이닝을 위해 this 반환
                }
                public DecisionNode SetDefault(string Condition)
                {
                    DefaultDecider = new DecisionNode() { Condition = Condition };
                    return this; // 체이닝을 위해 this 반환
                }

                public string Classify(object data)
                {
                    foreach (var pair in DecisionPairs)
                    {
                        if (pair.Decider != null && pair.Node != null && pair.Decider(data))
                            return pair.Node.Classify(data);
                    }

                    return DefaultDecider?.Classify(data) ?? Condition;
                }
            }
            
            DecisionNode decisionTree = new DecisionNode()
            .AddDecision(data => (int)data > 75,
                new DecisionNode()
                    .AddDecision(data => (int)data > 90, "Class S")
                    .SetDefault("Class A")
            )
            .AddDecision(data => (int)data > 50, "Class B")
            .AddDecision(data => (int)data > 25, "Class C")
            .SetDefault("Class D");
            #endregion


            /* END */
        }
    }
}
