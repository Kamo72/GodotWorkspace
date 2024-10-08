using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _favorClient.Entity
{
    public struct Magazine
    {
        public Magazine() 
        {
        
        }

        public (int now, int mag) magCount = (30, 30);
        //public Sprite2D sprite2D => GetNode("./Sprite2D") as Sprite2D;

        public bool GetFeed() 
        {
            return magCount.now-- > 0;
        }
    }
}
