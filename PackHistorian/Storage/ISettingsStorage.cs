namespace PackTracker.Storage
{
    internal interface ISettingsStorage
    {
        Settings Fetch();
        void Store(Settings History);
    }
}
