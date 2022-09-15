using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Models
{
    public class Character
    {
        public int Id { get; set; }

        public String Name { get; set; } = "Frodo";

        public int HitPoints { get; set; } = 100;

        public int Strength { get; set; } = 10;

        public int Defence { get; set; } = 10;

        public int Inteligence { get; set; } = 10;

        public RpgClass Class { get; set; } = RpgClass.Knight;

        public User? User { get; set; }
    }
}