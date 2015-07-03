namespace NerdDinner.Events
{
    public class RSVPed : IEvent
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
    }
}