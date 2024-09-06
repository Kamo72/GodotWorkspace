using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _favorClient.Entity;
using Godot;

namespace _favorClient.System.Ingame
{
    public partial class CameraManager : Camera2D
    {
        public Character target = null;


        public CameraManager() { }
    }
}
