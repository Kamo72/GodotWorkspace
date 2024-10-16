using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
                resources = ("", ""),
        } },
        };

        public WeaponStatus()
        {
            projectile = (800, 10, 45);
            magazine = (30, "?");
            time = (2.1f, 0.6f, 0.4f);
            mechanism = (true, false);
            resources = ("", "");
        }

        public (float rpm, float muzzleSpeed, float damage) projectile;
        public (int size, string spriteRoot) magazine;
        public (float reload, float feed, float aim) time;
        public (bool isAuto, bool isOpen) mechanism;
        public (string main, string sound) resources;

        public Node2D Instantiate()
        {
            Node2D result = null;
            try {
                PackedScene packedScene = ResourceLoader.Load<PackedScene>(resources.main);
                result = packedScene.Instantiate() as Node2D;
            }
            catch (Exception e)
            {
                GD.PushError(e);
            }


            return result;

        }
    }
}
