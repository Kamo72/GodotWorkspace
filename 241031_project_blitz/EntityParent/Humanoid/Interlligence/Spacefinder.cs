using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Humanoid
{
    public abstract partial class Intelligence
    {
        public Spacefinder spacefinder;
        public class Spacefinder
        {
            /* Spacefinder
             * 주변의 공간들을 파악하고, 각 공간들의 연결성을 바탕으로
             * 위험한 공간, 안전한 공간, 확인한 공간, 적이 있을 법한 공간 등을 파악하고, 점수를 매기는 등의 행동을 취함.
             */
            public Intelligence intelligence;
            public Humanoid master => intelligence.master;
            public Spacefinder(Intelligence intelligence)
            {
                this.intelligence = intelligence;
            }

            public virtual void Process(float delta)
            {

            }
        }
    }
}
