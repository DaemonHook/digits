using System;
using System.Collections.Generic;

public class Search
{
    /// <summary>
    /// 搜索方式
    /// </summary>
    public enum SearchMethod
    {
        BFS,
        DFS,
        ASTAR1,
        ASTAR2,
        RANDOM
    }

    /// <summary>
    /// 搜索树上的节点
    /// </summary>
    public class Node
    {
        public readonly State state;
        public List<Node> children;
        public List<Operation> opts;

        /// <summary>
        /// 上一个节点（母节点）
        /// </summary>
        public Node preNode = null;

        /// <summary>
        /// 上一个节点通过什么操作达到此节点
        /// </summary>
        public Operation preOp;

        /// <summary>
        /// 搜索链至此的长度
        /// </summary>
        public readonly int depth;

        /// <summary>
        /// 产生初始节点
        /// </summary>
        public Node(State s)
        {
            state = new State(s);
            children = new List<Node>();
            opts = new List<Operation>();
            depth = 1;
        }

        /// <summary>
        /// 通过op操作产生
        /// </summary>
        public Node(Node pre, Operation op)
        {
            state = new State(pre.state);
            state.DoOperation(op);
            preNode = pre;
            depth = pre.depth + 1;
        }
    }

    public readonly Node initialNode;

    public readonly State endingState;

    public SearchMethod method;

    public Search(State startState, State endingState, SearchMethod method)
    {
        initialNode = new Node(startState);
        this.endingState = endingState;
        this.method = method;
    }

    // public Node EndingNode = null;
    //
    // public bool IsEnd
    // {
    //     get { return EndingNode != null; }
    // }

    private static readonly Operation[] operations = ((Operation[])Enum.GetValues(typeof(Operation)));

    public struct SearchResult
    {
        public bool SearchSucceed;
        public bool SearchCanProceed;
        public Node CurSearchingNode;
    }

    private readonly HashSet<State> visitedSet = new HashSet<State>();

    private IEnumerator<SearchResult> BFSSearch()
    {
        visitedSet.Clear();
        Queue<Node> q = new Queue<Node>();
        q.Enqueue(initialNode);
        visitedSet.Add(initialNode.state);
        while (q.Count > 0)
        {
            Node cur = q.Dequeue();
            bool isEndingState = cur.state.Equals(endingState);

            foreach (var op in operations)
            {
                if (cur.state.CanDoOperation(op))
                {
                    Node newNode = new Node(cur, op);
                    if (!visitedSet.Contains(newNode.state))
                    {
                        visitedSet.Add(newNode.state);
                        q.Enqueue(newNode);
                    }
                }
            }

            yield return new SearchResult
                { SearchSucceed = isEndingState, SearchCanProceed = true, CurSearchingNode = cur };
        }

        yield return new SearchResult { SearchSucceed = false, SearchCanProceed = false, CurSearchingNode = null };
    }
}