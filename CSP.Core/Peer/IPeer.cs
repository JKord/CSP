#region namespace
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using Persistence;
#endregion

namespace CSP.Core.Peer
{
    [ServiceContract]
    interface IPeer
    {
        [OperationContract]
        Stream ConnectToStream();

        [OperationContract]
        void RestartFlow();

        [OperationContract]
        void GetFlow(string RID, int begin, long length, Dictionary<string, float> nodes);

        [OperationContract]
        void StopFlow();

        [OperationContract]
        void Configure(string udpPort, string kademliaPort);

        [OperationContract]
        bool StoreFile(string filename);

        [OperationContract]
        IList<KademliaResource> SearchFor(string queryString);
    }
}
