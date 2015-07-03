namespace NerdDinner.Events {
    public class AddressChanged : IEventData {
        public string NewAddress { get; set; }
    }
}