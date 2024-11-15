using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class Humanoid
{
    public Health health;
    public class Health
    {
        Humanoid master;
        public Health(Humanoid master)
        {
            this.master = master;
        }
    }
}