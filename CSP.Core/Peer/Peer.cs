#region namespace
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TransportService;
using CSP.Core.Util;
using Persistence;
#endregion

namespace CSP.Core.Peer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Peer : IDisposable
    {
        private TransportProtocol transportLayer;
        private Stream localStream;
        private ServiceHost[] svcHosts = new ServiceHost[3];
        private bool single;
        private string btpNode;
        private Repository trackRep;
        private string peerAddress;
        private Uri transportAddress;
        private Uri kademliaAddress;

        #region Properties

        public Dictionary<string, string> ConfOptions { get; set; }

        public int ChunkLength
        {
            get
            {
                if (transportLayer != null)
                {
                    return transportLayer.ChunkLength;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        public Peer(bool single = false, string btpNode = "")
        {
            Logger.Log("Initializing peer structure");
            this.ConfOptions = new Dictionary<string, string>();
            this.localStream = new MemoryStream();
            this.single = single;
            this.btpNode = btpNode;
            AppSettingsReader asr = new AppSettingsReader();
            Persistence.RepositoryConfiguration conf = new Persistence.RepositoryConfiguration(new { data_dir = (string)asr.GetValue("TrackRepository", typeof(string)) });
            this.trackRep = Persistence.RepositoryFactory.GetRepositoryInstance("Raven", conf);

            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] listaIP = IPHost.AddressList;
            foreach (IPAddress ip in listaIP)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    this.peerAddress = ip.ToString();
                    break;
                }
            }
        }

        private void calculateAddresses()
        {
            string udpPort = this.ConfOptions["udpPort"];
            string kademliaPort = this.ConfOptions["kadPort"];
            this.transportAddress = new Uri("soap.udp://" + this.peerAddress + ":" + udpPort + "/transport_protocol");
            this.kademliaAddress = new Uri("soap.udp://" + this.peerAddress + ":" + kademliaPort + "/kademlia");
        }

        public void RunLayers(bool withoutInterface = false)
        {
            Logger.Log("Running layers...");
            this.calculateAddresses();
            Exception kadStartExp = null;
            Exception transportStartExp = null;
            Exception interfaceStartExp = null;
            Thread kadThread = kadThread = new Thread(new ThreadStart(() => {
                try
                {
                    this.runKademliaLayer(single, btpNode, ref svcHosts[0]);
                }
                catch (Exception e)
                {
                    kadStartExp = e;
                }
            }));
            Thread transportThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    this.runTransportLayer(ref svcHosts[1]);
                }
                catch (Exception e)
                {
                    transportStartExp = e;
                }
            }));
            if (!withoutInterface)
            {
                Thread interfaceThread = new Thread(new ThreadStart(() => {
                    try
                    {
                        this.runInterfaceLayer(ref svcHosts[2]);
                    }
                    catch (Exception e)
                    {
                        interfaceStartExp = e;
                    }
                }));
                interfaceThread.Start();
                interfaceThread.Join();
                if (interfaceStartExp != null)
                {
                    throw interfaceStartExp;
                }
            }
            kadThread.Start();
            kadThread.Join();
            if (kadStartExp != null)
            {
                throw kadStartExp;
            }
            transportThread.Start();
            transportThread.Join();
            if (transportStartExp != null)
            {
                throw transportStartExp;
            }
        }

        #region layersInitialization
        /// <summary>
        /// Method (usually runned like a thread) that privately hosts the interface service.
        /// </summary>
        /// <param name="svcHost">a service host object where is stored newly initalized host.</param>
        private void runInterfaceLayer(ref ServiceHost svcHost)
        {
            Logger.Log("Running Interface Layer.");
            ServiceHost host = new ServiceHost(this);
            try
            {
                host.Open();
            }
            catch (AddressAlreadyInUseException aaiue)
            {
                Logger.Log("Unable to Connect as a Server because there is already one on this machine", aaiue);
            }
            foreach (Uri uri in host.BaseAddresses)
            {
                Logger.Log(uri.ToString());
            }
            svcHost = host;
        }

        /// <summary>
        /// Method that runs the transport layer
        /// </summary>
        /// <param name="svcHost">a service host object where is stored newly initalized host.</param>
        private void runTransportLayer(ref ServiceHost svcHost)
        {
            Logger.Log("Running Transport Layer.");
            TransportProtocol tsp = new TransportProtocol(transportAddress, this.trackRep);
            this.transportLayer = tsp;
            ServiceHost host = new ServiceHost(tsp, transportAddress);
            try
            {
                host.Open();
                #region Output dispatchers listening
                foreach (Uri uri in host.BaseAddresses)
                {
                    Logger.Log(uri.ToString());
                }
                Logger.Log("Number of dispatchers listening : " + host.ChannelDispatchers.Count);
                foreach (System.ServiceModel.Dispatcher.ChannelDispatcher dispatcher in host.ChannelDispatchers)
                {
                    Logger.Log(dispatcher.Listener.Uri.ToString());
                }
                #endregion
            }
            catch (AddressAlreadyInUseException aaiue)
            {
                Logger.Log("Unable to Connect as a Server because there is already one on this machine", aaiue);
                throw aaiue;
            }
            svcHost = host;
        }

        /// <summary>
        /// Method that runs the kademlia layer
        /// </summary>
        /// <param name="single">if true indicates that the kademlia layer have to do a single start</param>
        /// <param name="btpNode">if indicated, it represents the node suggested to do bootstrap</param>
        /// <param name="svcHost">a service host object where is stored newly initalized host.</param>
        private void runKademliaLayer(bool single, string btpNode, ref ServiceHost svcHost)
        {
            Logger.Log("Running Kademlia layer.");

            ServiceHost kadHost = new ServiceHost(null, kademliaAddress);
            try
            {
                kadHost.Open();
            }
            catch (AddressAlreadyInUseException aaiue)
            {
                Logger.Log("Unable to Connect as a Server because there is already one on this machine", aaiue);
                throw aaiue;
            }            
            svcHost = kadHost;
        }
        #endregion

        #region interface

        public Stream ConnectToStream()
        {
            Logger.Log("Returning stream to requestor.");
            return this.localStream;
        }

        public void RestartFlow()
        {
            if (transportLayer == null)
            {
                Thread transportThread = new Thread(new ThreadStart(() => this.runTransportLayer(ref svcHosts[1])));
                transportThread.Start();
                transportThread.Join();
            }
            this.transportLayer.ReStart();
        }

        public void GetFlow(string RID, int begin, long length, Dictionary<string, float> nodes)
        {
            this.GetFlow(RID, begin, length, nodes, null);
        }

        public void GetFlow(string RID, int begin, long length, Dictionary<string, float> nodes, Stream stream = null)
        {
            Logger.Log("Beginning to get flow from the network");
            Stream handlingStream = stream;
            if (handlingStream == null)
            {
                handlingStream = this.localStream;
            }
            if (transportLayer == null)
            {
                Thread transportThread = new Thread(new ThreadStart(() => this.runTransportLayer(ref svcHosts[1])));
                transportThread.Start();
                transportThread.Join();
            }
            this.transportLayer.Start(RID, begin, length, nodes, handlingStream);
        }

        public void StopFlow()
        {
            Logger.Log("Stop flow.");
            if (this.transportLayer != null)
            {
                this.transportLayer.Stop();
            }
        }

        #endregion

        public void Dispose()
        {
            Logger.Log("Disposing Peer");
            foreach (ServiceHost svcHost in this.svcHosts)
            {
                if (svcHost != null)
                    svcHost.Close();
            }
            this.trackRep.Dispose();
        }
    }
}
