using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technical_Solution
{
    internal class Event
    {
        private readonly int eventID;
        private readonly int routeID;
        private readonly string date;
        private readonly string time;
        private readonly string location;
        private readonly byte[] routeMap;

        public Event(int eventID, int routeID, string date, string time, string location, byte[] routeMap)
        {
            this.eventID = eventID;
            this.routeID = routeID;
            this.date = date;
            this.time = time;
            this.location = location;
            this.routeMap = routeMap;
        }

        public int GetEventID()
        {
            return eventID;
        }

        public int GetRouteID()
        {
            return routeID;
        }

        public string GetDate()
        {
            return date;
        }

        public string GetTime()
        {
            return time;
        }

        public string GetLocation()
        {
            return location;
        }

        public byte[] GetRouteMap()
        {
            return routeMap;
        }
    }
}
