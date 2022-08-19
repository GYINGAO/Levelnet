using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelnetAdjustment.utils {
    public class VertexNode {
        public VertexNode(int num, string name) {
            Id = num;
            Data = name;
        }
        public int Id { get; set; }          //结点序号
        public string Data { get; set; }         // 结点信息
        public EdgeNode FirstEdge { get; set; }  // 第一条边

        // 构造函数及Setter Getter省略
    }

    // 边表结点
    public class EdgeNode {
        public EdgeNode(int end) {
            Adjvex = end;
        }
        public int Adjvex { get; set; }    // 每条边的下一结点
        public EdgeNode Next { get; set; }      // 下一个边结点

        // 构造函数及Setter Getter省略
    }

    // 邻接表表示的图
    public partial class Graph1 {
        public int NumVertex { get; set; }       // 图中结点数量
        public VertexNode[] VertexNodes { get; set; }// 邻接表中表头结点集合
        public bool[] Vst { get; set; }           // 结点访问标记
        public int[] Pre { get; set; }           // 父结点记录数组，用于回溯，初始值为-1——表示没有父结点
        public int RingFinded { get; set; }      // 标记是否已经找到环

        // 构造函数及Setter Getter省略
    }

    /// <summary>
    /// 无向图中的环 计算算法
    /// </summary>
    public class DFSAlgorithm {
        public int numVertex;
        public int numEdge;
        public VertexNode[] vertexNodes;
        public bool[] vst;
        public int[] pre;
        public bool ringFinded;
        public int[] numLayerNode;
        public int[] nodeLyaer;

        public DFSAlgorithm(VertexNode[] vertexNodes) {
            this.numEdge = 0;
            this.numVertex = vertexNodes.Length;
            this.vertexNodes = vertexNodes;
            vst = new bool[numVertex];
            pre = new int[numVertex];

            for (int i = 0; i < pre.Length; i++) {
                pre[i] = -1;
            }
        }

        public void insertEdge(int start, int end) {
            VertexNode vertexNode = vertexNodes[start];
            EdgeNode edgeNode = new EdgeNode(end);

            EdgeNode firstEdgeNode = vertexNode.FirstEdge;
            if (firstEdgeNode == null) {
                vertexNode.FirstEdge = edgeNode;
            }
            else {
                edgeNode.Next = firstEdgeNode;
                vertexNode.FirstEdge = edgeNode;
            }
        }

        public void dfs(int root) {
            VertexNode vertexNode = this.vertexNodes[root];
            vst[root] = true;
            Console.WriteLine(vertexNode.Data + " ");

            EdgeNode currentEdgeNode = vertexNode.FirstEdge;

            while (currentEdgeNode != null) {
                int vertexNodeIndex = currentEdgeNode.Adjvex;
                if (vst[vertexNodeIndex] == false) {
                    dfs(vertexNodeIndex);
                }
                currentEdgeNode = currentEdgeNode.Next;
            }
        }

        // 通过DFS找环
        public void findRing(int root) {
            vst[root] = true; // 标记为已访问

            VertexNode vertexNode = vertexNodes[root];       // 当前表头结点
            EdgeNode currentEdgeNode = vertexNode.FirstEdge;

            while (currentEdgeNode != null && ringFinded == false) { // 遍历边结点，当找到环时结束
                int vertexNodeIndex = currentEdgeNode.Adjvex;

                if (vst[vertexNodeIndex] == false) {
                    pre[vertexNodeIndex] = root;    // 记录父结点
                    findRing(vertexNodeIndex);              // 递归搜索子结点
                }
                else if (pre[root] != vertexNodeIndex) {
                    ringFinded = true;
                    backPath(root);
                    break;
                }

                currentEdgeNode = currentEdgeNode.Next;     // 搜索下一表头结点
            }
        }

        // 回溯路径
        public void backPath(int index) {
            Console.WriteLine("起点{0}的闭合环:", index);
            while (index != -1) {
                Console.Write(vertexNodes[index].Data + "->");
                index = pre[index];
            }
        }
    }

}
