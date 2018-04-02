namespace Actor.Server
{
    public interface IListenerService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IContextComm GetCommunicationContext();
        void Close();
    }
}
