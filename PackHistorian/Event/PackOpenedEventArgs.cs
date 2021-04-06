using PackTracker.Entity;

namespace PackTracker.Event
{
    internal class PackOpenedEventArgs
    {
        public Pack Pack { get; }

        public PackOpenedEventArgs(Pack Pack)
        {
            this.Pack = Pack;
        }
    }
}
