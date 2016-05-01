#region namespace
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using NChordLib;
using CSP.Core.Util;
#endregion

namespace CSP.Core
{
    public class ChordTopoMap
    {
        #region Fields & Properties

        private const int MaxNodeLimit = 6000;

        private List<ChordNode> nodeList;
        private int[,] hopCountMap;

        #endregion

        #region Properties

        public ChordNode SeedNode { get; set; }
        public List<ChordNode> NodeList {
            get { return nodeList; }
            set { nodeList = value; }
        }

        #endregion

        #region Connection

        public void Connect(String hostname, int port)
        {
            SeedNode = new ChordNode(hostname, port);
            GenerateMap(SeedNode, out nodeList, out hopCountMap);
        }

        public void Close()
        {

        }

        #endregion

        #region Shared Functionality

        public void RunSortTask()
        {
            // nodeList = GetNodeList(SeedNode);

            int[] array = { 2,4,6,8,345,34,3463,6546,56,3,45,45,44,6578,768,76987,97,68,76,9,76,976,98,69,67,22,976,8,76,9,74,56,67,56,765,7,54876,345,96,9,45,67,6,756,8 };

            Dictionary<string, object> inputData = new Dictionary<string, object>();
            inputData.Add("array", array);

            JSRun.Execute("prog/sort.js", inputData);
        }

        private static void GenerateMap(ChordNode seedNode, out List<ChordNode> nodeList, out int[,] hopcountMap)
        {
            nodeList = GetNodeList(seedNode);
            hopcountMap = new int[nodeList.Count, nodeList.Count];
            nodeList.Sort(); // sort by nodeId

            int y = 0;
            foreach (ChordNode sourceNode in nodeList)
            {
                int x = 0;
                foreach (ChordNode destinationNode in nodeList)
                {
                    ChordServer.CallFindSuccessor(sourceNode, destinationNode.ID, 3, 0, out hopcountMap[y, x]);
                    x++;
                }
                y++;
            }
        }

        private static List<ChordNode> GetNodeList(ChordNode seedNode)
        {
            List<ChordNode> nodeList = new List<ChordNode>();
            nodeList.Add(seedNode);

            ChordNode currNode = ChordServer.GetSuccessor(seedNode);
            int i = 0;
            while (seedNode.ID != currNode.ID && i < MaxNodeLimit)
            {
                currNode = ChordServer.GetSuccessor(currNode);
                nodeList.Add(currNode);
                i++;
            }

            return nodeList;
        }

        #endregion

        #region Printing

        public void GenerateCsv() => ChordTopoMap.GenerateCsv(hopCountMap, nodeList, "map.csv");
        public void GenerateImage() => ChordTopoMap.GenerateImage(hopCountMap, nodeList, "map.jpg");

        private static void GenerateCsv(int[,] hopcountMap, List<ChordNode> nodeList, string outputFilename)
        {
            StreamWriter fileWriter = File.CreateText(outputFilename);
            for (int y = 0; y < nodeList.Count; y++)
            {
                string hopCounts = string.Empty;
                for (int x = 0; x < nodeList.Count; x++)
                {
                    hopCounts += hopcountMap[y, x];
                    if (x != nodeList.Count - 1)
                    {
                        hopCounts += ",";
                    }
                }
                fileWriter.WriteLine(hopCounts);
            }

            fileWriter.Close();
        }

        private static void GenerateImage(int[,] hopcountMap, List<ChordNode> nodeList, string outputFileName)
        {
            Bitmap mapImage = new Bitmap(nodeList.Count, nodeList.Count);
            for (int y = 0; y < nodeList.Count; y++)
            {
                for (int x = 0; x < nodeList.Count; x++)
                {
                    Color pixelColor = Color.White;
                    if (hopcountMap[y, x] <= ColorMap.Length)
                        pixelColor = ColorMap[hopcountMap[y, x]];
                    mapImage.SetPixel(x, y, pixelColor);
                }
            }

            mapImage.Save(outputFileName, ImageFormat.Jpeg);
        }

        private static Color[] ColorMap =
        {
            Color.Black,
            Color.LightGray,
            Color.Gray,
            Color.LightBlue,
            Color.Blue,
            Color.Navy,
            Color.LightGreen,
            Color.Green,
            Color.LightYellow, 
            Color.Yellow,
            Color.Red,
            Color.Maroon,
            Color.Purple,
            Color.Plum
        };

        #endregion
    }
}