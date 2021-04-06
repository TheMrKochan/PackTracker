namespace PackTracker.Storage
{
    internal interface IHistoryStorage
    {
        History Fetch();
        void Store(History History);
    }
}
