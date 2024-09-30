using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public struct WeaponStatus
    {
        public static Dictionary<string, WeaponStatus> statLib = new(){
            { "M4A1", new (){
                projectile = (800, 10, 45),
                magazine = (30, "?"),
                time = (2.1f, 0.6f, 0.4f),
                mechanism = (true, false),
        } },
        };


        public WeaponStatus()
        {
            projectile = (800, 10, 45);
            magazine = (30, "?");
            time = (2.1f, 0.6f, 0.4f);
            mechanism = (true, false);
        }

        public (float rpm, float muzzleSpeed, float damage) projectile;
        public (int size, string spriteRoot) magazine;
        public (float reload, float feed, float aim) time;
        public (bool isAuto, bool isOpen) mechanism;
    }
}
