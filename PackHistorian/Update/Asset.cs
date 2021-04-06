using System.Runtime.Serialization;

namespace PackTracker.Update
{
    [DataContract]
    public class Asset
    {
        [DataMember]
        public string name;

        [DataMember]
        public string browser_download_url;
    }
}
