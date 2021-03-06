﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using DGI.Model;

namespace DGI.Controller
{
    public class GraphController
    {
        private const int ASCII_OFFSET = 48;

        GraphModel graph;
        public GraphModel Graph { get { return graph; } private set { graph = value; } }

        private static MainWindow mainWindowRef;

        public GraphController(MainWindow window, List<List<int>> adjList)
        {
            mainWindowRef = window;
            graph = new GraphModel(adjList);
            SetWorker();
        }
        public GraphController(MainWindow window, int[,] adjMatrix)
        {
            mainWindowRef = window;
            graph = new GraphModel(adjMatrix);
            SetWorker();
        }
        public GraphController()
        {
        }
        #region Asynchronous, background work
        BackgroundWorker backgroundWorker;

        public void SetWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.RunWorkerCompleted += Worker_Completed;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += Worker_doWork;
            backgroundWorker.ProgressChanged += Worker_ProgressChanged;
            backgroundWorker.RunWorkerAsync();

        }

        private void Worker_doWork(object sender, DoWorkEventArgs e)
        {
            int edges = 0;
            foreach (var row in graph.AdjacencyList)
                foreach (var item in row)
                    edges++;
            graph.EdgeCount = edges;
            backgroundWorker.ReportProgress(73);


            int n = graph.AdjacencyMatrix.GetLength(0);
            double oneStep = 28 / (double)n;
            for (int i = 0; i < n; i++)
            {
                var inOutEdg = CountInAndOutEdges(i);
                graph.AddVertice(i, inOutEdg.Item1, inOutEdg.Item2);

                if (i % 20 == 0) backgroundWorker.ReportProgress(72 + (int)(oneStep * i));
            }
            backgroundWorker.ReportProgress(100);
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker = null;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MainWindow.ChangeProgress(e.ProgressPercentage);
        }
        #endregion

        private Tuple<int, int> CountInAndOutEdges(int row)
        {
            int inEdg = 0;
            int outEdg = 0;
            for (int i = 0; i < graph.AdjacencyMatrix.GetLength(1); i++)
            {
                if (graph.AdjacencyMatrix[row, i] == 1) outEdg++;
                if (graph.AdjacencyMatrix[i, row] == 1) inEdg++;
            }
            return Tuple.Create(inEdg, outEdg);
        }

        public static async Task<GraphModel> LoadGraphAsync(string path)
        {
            StreamReader sr = new StreamReader(path);
            List<List<int>> adjList = new List<List<int>>();
            List<int> temp;
            string line;
            while (!sr.EndOfStream)
            {
                temp = new List<int>();
                line = await sr.ReadLineAsync();

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != ',')
                    {
                        temp.Add(line[i] - ASCII_OFFSET);
                    }
                }
                adjList.Add(temp);
            }

            return new GraphModel(adjList);
        }
        public static List<List<int>> LoadGraph(string path)
        {
            StreamReader sr = new StreamReader(path);
            List<List<int>> adjList = new List<List<int>>();
            List<int> temp;

            string text = sr.ReadToEnd();
            sr.Close();
            string[] line = text.Split('\n');
            int i = 0;
            foreach (var item in line)
            {
                if (i == line.Length - 1) break;
                i++;
                string[] single = item.Split(',');
                temp = new List<int>();
                foreach (var element in single)
                    if(element!= "\r" && element != " " && element != "")
                        temp.Add(Convert.ToInt32(element));
                adjList.Add(temp);
            }
            return adjList;
        }
        public static async Task SaveGraphAsync(GraphModel graph, string path)
        {
            StreamWriter sw = new StreamWriter(path);

            foreach (var row in graph.AdjacencyList)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    await sw.WriteAsync(row[i].ToString());
                    if(i != row.Count-1) await sw.WriteAsync(",");
                }
                await sw.WriteLineAsync();
            }
            sw.Close();
        }
    }
}
