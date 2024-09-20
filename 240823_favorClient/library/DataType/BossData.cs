using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.library.DataType
{
    public abstract class BossData
    {
        public string name;

        public string easyDes;
        public Description diffDes;

        public Func<PackedScene> getScene;



        public enum Type 
        {
            NONE,
            SCHADENFREUDE,
            KNIGHT,
        }
        public Type type;
    }
}
