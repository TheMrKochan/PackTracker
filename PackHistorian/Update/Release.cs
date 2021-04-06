using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PackTracker.Update
{
    [DataContract]
    public class Release
    {
        [DataMember]
        public string tag_name;

        [DataMember]
        public string name;

        [DataMember]
        public List<Asset> assets;

        [DataMember]
        public string body;
    }
}
