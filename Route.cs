using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class Route
    {
        private readonly int id;
        private readonly string name;
        private readonly string difficulty;
        private readonly byte[] routeImage;

        public Route(int id, string name, string difficulty, byte[] routeImage)
        {
            this.id = id;
            this.name = name;
            this.difficulty = difficulty;
            this.routeImage = routeImage;
        }

        public int GetID()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        public string GetDifficulty()
        {
            return difficulty;
        }

        public byte[] GetRouteImage()
        {
            return routeImage;
        }
    }
}
