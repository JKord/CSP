using NChordLib;
using CSP.Core.Util;

namespace CSP.Core
{
    public class Ring
    {
        private ChordInstance instance;

        public void Create(int port)
        {
            ChordServer.LocalNode = new ChordNode(System.Net.Dns.GetHostName(), port);

            if (ChordServer.RegisterService(port))
            {
                instance = ChordServer.GetInstance(ChordServer.LocalNode);
                instance.Join(null, ChordServer.LocalNode.Host, ChordServer.LocalNode.PortNumber);
            }
            Logger.Log("Ring Create");
        }

        public void Close()
        {
            instance.Depart();
            Logger.Log("Ring Close");
        }

        public void NodeInfo(bool extended = false)
        {
            ChordNode successor = instance.Successor;
            ChordNode predecessor = instance.Predecessor;
            ChordFingerTable fingerTable = instance.FingerTable;
            ChordNode[] successorCache = instance.SuccessorCache;

            string successorString, predecessorString, successorCacheString, fingerTableString;
            successorString = (successor != null)? successorString = successor.ToString() : "NULL";
            predecessorString = ((predecessor != null)) ? predecessor.ToString() : "NULL";

            successorCacheString = "SUCCESSOR CACHE:";
            for (int i = 0; i < successorCache.Length; i++)
            {
                successorCacheString +=
                    string.Format("\r{0}: ", i)
                    + ((successorCache[i] != null)? successorCache[i].ToString() : "NULL");
            }

            fingerTableString = "FINGER TABLE:";
            for (int i = 0; i < fingerTable.Length; i++)
            {
                fingerTableString +=
                    string.Format("\r{0:x8}: ", fingerTable.StartValues[i])
                    + ((fingerTable.Successors[i] != null)? fingerTable.Successors[i].ToString() : "NULL");
            }

            Logger.Log("NODE INFORMATION:\rSuccessor: {1}\rLocal Node: {0}\rPredecessor: {2}\r", ChordServer.LocalNode, successorString, predecessorString);

            if (extended)
            {
                Logger.Log(successorCacheString);
                Logger.Log(fingerTableString);
            }
        }
    }
}
