namespace EventsSystem
{
    public interface ICancellable
    {
        bool Cancelled { get; set; }
    }
}