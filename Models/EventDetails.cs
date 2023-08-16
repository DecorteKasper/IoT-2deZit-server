using System;

namespace IoT_2deZit_server.Models
{
    public class EventDetails
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string City { get; set; }
        public string CreatorEmail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
