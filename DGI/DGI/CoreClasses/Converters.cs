﻿using DGI.Model;
using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DGI.CoreClasses
{
    public static class Converters
    {
        public static Graph GraphModelToMSAGLGraph(GraphModel sourceGraph)
        {
            Graph result = new Graph();
            int counter = 0;
            for (int i = 0; i < sourceGraph.AdjacencyList.Count; i++)
            {
                result.AddNode(i.ToString());
            }
            foreach (List<int> List in sourceGraph.AdjacencyList)
            {
                foreach (int value in List)
                {
                    result.AddEdge(counter.ToString(), value.ToString());
                }
                counter++;
            }
            return result;
        }
        public static int[,] ListToAdjacencyMatrix(List<List<int>> list)
        {
            if (list == null) return null;
            int size = list.Count;
            int[,] adjMtrx = new int[size, size];

            int line = 0;
            foreach (var row in list)
            {
                foreach (var item in row)
                    adjMtrx[line, item] = 1;
                line++;
            }
            return adjMtrx;
        }

        public static List<List<int>> AdjacencyMatrixToList(int[,] matrix)
        {
            if (matrix == null) return null;
            List<List<int>> list = new List<List<int>>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                list.Add(new List<int>());
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (matrix[i, j] == 1)
                        list[i].Add(j + 1);
            }
            return list;
        }

        /// <summary>
        /// Core method, which allow to compare 2 lists of Adjacency. If they are the same, graphs are isomorphic
        /// </summary>
        /// <param name="order"> New order list [0-based indexing system]</param>
        /// <param name="actual"> Actual list od adjacency [normal indexing system]</param>
        public static List<List<int>> NewAdjacencyListOrder(List<int> order, List<List<int>> actual)
        {
            List<List<int>> list = new List<List<int>>();
            for (int i = 0; i < actual.Count; i++)
            {
                list.Add(new List<int>());
                int orderN = order[i];
                int[] consideredLine = new int[actual[orderN].Count];
                int j = 0;
                foreach (var item in actual[orderN]) { consideredLine[j] = item; j++; }
                for (int k = 0; k < consideredLine.Length; k++)
                    for (int l = 0; l < order.Count; l++) if (order[l] == (consideredLine[k] - 1)) { list[i].Add(l + 1); break; }
            }
            return list;
        }
    }
}
