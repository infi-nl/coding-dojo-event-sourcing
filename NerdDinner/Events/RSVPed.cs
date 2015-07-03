namespace NerdDinner.Events
{
    public class RSVPed : IEventData
    {
        public string Name { get; set; }
        public string FriendlyName { get; set; }
    }
}