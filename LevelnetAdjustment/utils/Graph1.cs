using System;
using System.Collections;
using System.Collections.Generic;
using LevelnetAdjustment.model;

namespace LevelnetAdjustment.utils {
    public partial class Graph1 {
        private static readonly int N = 100000;
        private static List<int>[] graph = new List<int>[N];
        private static List<int>[] cycles = new List<int>[N];
        private static int cyclenumber;

        private static void dfs_cycle(int u, int p, int[] color, int[] mark, int[] par) {
            if (color[u] == 2) {
                return;
            }

            if (color[u] == 1) {
                cyclenumber++;
                int cur = p;
                mark[cur] = cyclenumber;

                while (cur != u) {
                    cur = par[cur];
                    mark[cur] = cyclenumber;
                }
                return;
            }
            par[u] = p;

            color[u] = 1;

            foreach (int v in graph[u]) {
                if (v == par[u]) {
                    continue;
                }
                dfs_cycle(v, u, color, mark, par);
            }

            color[u] = 2;
        }

        private static void addEdge(int u, int v) {
            graph[u].Add(v);
            graph[v].Add(u);
        }

        private static List<string> Cycles(int edges, int[] mark) {
            for (int i = 1; i <= edges; i++) {
                if (mark[i] != 0) {
                    cycles[mark[i]].Add(i);
                }
            }

            List<string> loops = new List<string>();
            for (int i = 1; i <= cyclenumber; i++) {
                string s = i + ":";
                foreach (int x in cycles[i]) {
                    s += x + ",";
                }
                loops.Add(s);
            }

            return loops;
        }


        public static List<string> Drive(ArrayList point, List<ObservedData> od) {
            for (int i = 0; i < N; i++) {
                graph[i] = new List<int>();
                cycles[i] = new List<int>();
            }
            od.ForEach(p => {
                var startIdx = point.IndexOf(p.Start);
                var endIdx = point.IndexOf(p.End);
                addEdge(startIdx, endIdx);
            });

            int[] color = new int[N];
            int[] par = new int[N];

            int[] mark = new int[N];

            cyclenumber = 0;
            int edges = point.Count;

            dfs_cycle(1, 0, color, mark, par);

            return Cycles(edges, mark);
        }
    }

}
