using System;
using System.Collections.Generic;
using System.Linq;

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
        public Node preNode;

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
            preNode = null;
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

    public State endingState;

    public SearchMethod method;

    public Func<IEnumerable<SearchResult>> NextStepFunction;
    
    
    public Search(State startState, SearchMethod method)
    {
        initialNode = new Node(startState);
        int r = startState.radius;
        int[] d = new int[r * r];
        for (int i = 0; i < r * r; i++)
        {
            d[i] = i;
        }

        endingState = new State(r * r, d);
        this.method = method;
        switch (this.method)
        {
            case SearchMethod.BFS:
                this.NextStepFunction = BFSSearch;
                break;
            case SearchMethod.DFS:
                this.NextStepFunction = DFSSearch;
                break;
            case SearchMethod.ASTAR1:
                this.NextStepFunction = AStarSearch1;
                break;
            case SearchMethod.ASTAR2:
                this.NextStepFunction = AStarSearch2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

    private IEnumerable<SearchResult> BFSSearch()
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

    private IEnumerable<SearchResult> DFSSearch()
    {
        visitedSet.Clear();
        Stack<Node> s = new Stack<Node>();
        s.Push(initialNode);
        visitedSet.Add(initialNode.state);
        while (s.Count > 0)
        {
            Node cur = s.Pop();
            bool isEndingState = cur.state.Equals(endingState);

            foreach (var op in operations)
            {
                if (cur.state.CanDoOperation(op))
                {
                    Node newNode = new Node(cur, op);
                    if (!visitedSet.Contains(newNode.state))
                    {
                        visitedSet.Add(newNode.state);
                        s.Push(newNode);
                    }
                }
            }

            yield return new SearchResult
                { SearchSucceed = isEndingState, SearchCanProceed = true, CurSearchingNode = cur };
        }

        yield return new SearchResult { SearchSucceed = false, SearchCanProceed = false, CurSearchingNode = null };
    }

    /// <summary>
    /// 启发函数1：不在位置上的数字数量
    /// </summary>
    private static int NotInPositionCount(State s)
    {
        int r = s.radius;
        var digits = s.digits;
        int cnt = 0;
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < r; j++)
            {
                int a = i * r + j;
                if (a != digits[i, j]) cnt++;
            }
        }

        return cnt;
    }

    /// <summary>
    /// 启发函数2：数字到其终点位置的曼哈顿距离和
    /// </summary>
    private static int ManhattanDistanceSum(State s)
    {
        int r = s.radius;
        var digits = s.digits;
        int cnt = 0;
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < r; j++)
            {
                int k = digits[i, j];
                int kx = k / r;
                int ky = k % r;
                cnt += Math.Abs(i - kx) + Math.Abs(j - ky);
            }
        }

        return cnt;
    }

    private class AStarComp1 : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            int xv = NotInPositionCount(x.state);
            int yv = NotInPositionCount(y.state);
            var c = xv.CompareTo(yv);
            if (c != 0)
            {
                return c;
            }
            else
            {
                return x.state.ToString().CompareTo(y.state.ToString());
            }
        }
    }
    
    private class AStarComp2 : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            int xv = ManhattanDistanceSum(x.state);
            int yv = ManhattanDistanceSum(y.state);
            var c = xv.CompareTo(yv);
            if (c != 0)
            {
                return c;
            }
            else
            {
                return x.state.ToString().CompareTo(y.state.ToString());
            }
        }
    }

    private IEnumerable<SearchResult> AStarSearch1()
    {
        visitedSet.Clear();
        SortedSet<Node> ss = new SortedSet<Node>(new AStarComp1());
        ss.Add(initialNode);
        visitedSet.Add(initialNode.state);
        while (ss.Count > 0)
        {
            Node cur = ss.First();
            ss.Remove(cur);
            
            bool isEndingState = cur.state.Equals(endingState);

            foreach (var op in operations)
            {
                if (cur.state.CanDoOperation(op))
                {
                    Node newNode = new Node(cur, op);
                    if (!visitedSet.Contains(newNode.state))
                    {
                        visitedSet.Add(newNode.state);
                        ss.Add(newNode);
                    }
                }
            }

            yield return new SearchResult
                { SearchSucceed = isEndingState, SearchCanProceed = true, CurSearchingNode = cur };
        }

        yield return new SearchResult { SearchSucceed = false, SearchCanProceed = false, CurSearchingNode = null };
    }
    
    private IEnumerable<SearchResult> AStarSearch2()
    {
        visitedSet.Clear();
        SortedSet<Node> ss = new SortedSet<Node>(new AStarComp1());
        ss.Add(initialNode);
        visitedSet.Add(initialNode.state);
        while (ss.Count > 0)
        {
            Node cur = ss.First();
            ss.Remove(cur);
            
            bool isEndingState = cur.state.Equals(endingState);

            foreach (var op in operations)
            {
                if (cur.state.CanDoOperation(op))
                {
                    Node newNode = new Node(cur, op);
                    if (!visitedSet.Contains(newNode.state))
                    {
                        visitedSet.Add(newNode.state);
                        ss.Add(newNode);
                    }
                }
            }

            yield return new SearchResult
                { SearchSucceed = isEndingState, SearchCanProceed = true, CurSearchingNode = cur };
        }

        yield return new SearchResult { SearchSucceed = false, SearchCanProceed = false, CurSearchingNode = null };
    }
}