using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public abstract partial class Intelligence
    {
        public Pathfinder pathfinder;
        public class Pathfinder
        {
            /* Pathfinder
             * 이동 경로를 찾는 객체입니다.
             * 적절한 경로를 찾기 위해 이동 비용을 계산하고 적용하는 절차를 수행합니다.
             */
            public Intelligence intelligence;
            public Humanoid master => intelligence.master;
            public Pathfinder(Intelligence intelligence)
            {
                this.intelligence = intelligence;
            }

            public virtual void Process(float delta)
            {

            }
        }
    }
}
