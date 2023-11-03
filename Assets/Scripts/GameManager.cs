using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public InputField inputField;
    public SquareGrid Grid;

    public TMP_Text Message;
    public TMP_Dropdown ModeDropDown;
    public TMP_Dropdown AlgorithmDropDown;
    public TMP_Dropdown ProblemSizeDropDown;

    // public 
    private bool solving = false;

    // public Button StartButton;
    // public Button ResetButton;

    /// <summary>
    /// 8数码还是15数码
    /// </summary>
    public int ProblemSize;

    public enum GameMode
    {
        Visiable, //可视模式
        Speed //测速模式
    }

    public GameMode mode = GameMode.Visiable;

    public enum AlgoType
    {
        BFS, //深度优先
        DFS, //广度优先
        ASTAR1, //A星算法1
        ASTAR2, //A星算法2
    }

    public AlgoType algoType = AlgoType.BFS;

    public int[] initialInput;

    private string inputCache = "";

    private void Awake()
    {
        inputField.onValueChanged.AddListener(InputHook);
        ModeDropDown.onValueChanged.AddListener(ModeHook);
        AlgorithmDropDown.onValueChanged.AddListener(AlgoHook);
        ProblemSizeDropDown.onValueChanged.AddListener(SizeHook);
    }

    public void InputHook(string input)
    {
        inputCache = input;
        Debug.Log($"input: {inputCache}");
    }

    public void ModeHook(int i)
    {
        if (solving)
        {
            ShowMsg("当前更改重置后生效");
        }

        if (i == 0)
        {
            mode = GameMode.Visiable;
        }
        else
        {
            mode = GameMode.Speed;
        }

        Debug.Log($"当前模式：{mode}");
    }

    public void AlgoHook(int i)
    {
        if (solving)
        {
            ShowMsg("当前更改重置后生效");
        }

        algoType = (AlgoType)i;
        Debug.Log($"当前算法：{algoType}");
    }

    public void SizeHook(int i)
    {
        if (solving)
        {
            ShowMsg("当前更改重置后生效");
        }

        if (i == 0)
        {
            ProblemSize = 9;
        }
        else
        {
            ProblemSize = 16;
        }

        Debug.Log($"当前问题规模：{ProblemSize}");
    }

    bool CheckInput(out int[] arr)
    {
        arr = null;
        string[] ss = inputCache.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (ss.Length != ProblemSize)
        {
            ShowMsg($"输入数字个数不对，应为{ProblemSize}");
            return false;
        }

        arr = new int[ss.Length];
        int count = 0;
        foreach (var s in ss)
        {
            try
            {
                arr[count] = int.Parse(s);
                if (arr[count] < 0 || arr[count] >= ProblemSize)
                {
                    ShowMsg("输入的数字超出了范围");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                ShowMsg("输入的数字格式不对");
                return false;
            }

            count++;
        }

        for (int i = 0; i < ProblemSize; i++)
        {
            if (Array.IndexOf(arr, i) == -1)
            {
                ShowMsg("输入的数字不全");
                return false;
            }
        }

        if (!CanBeSolved(arr))
        {
            ShowMsg("此输入无解，请重新输入！");
        }

        StringBuilder sb = new StringBuilder();
        foreach (var i in arr)
        {
            sb.Append(i.ToString() + ' ');
        }

        Debug.Log($"input string: [{sb.ToString()}]");
        return true;
    }

    /// <summary>
    /// 求数组的逆序数
    /// </summary>
    private int RevNumber(int[] ints)
    {
        int cnt = 0;
        {
            for (int j = ints.Length - 1; j >= 0; j--)
                for (int i = 0; i < j; i++)
                    if (ints[i] > ints[j] && ints[i] != 0 && ints[j] != 0)
                        cnt++;
        }
        return cnt;
    }

    /// <summary>
    /// 根据理论，只有逆序数的奇偶与终点相同的输入才有解
    /// 终点的逆序数为0
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool CanBeSolved(int[] input)
    {
        int revn = RevNumber(input);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            sb.Append(input[i].ToString() + ",");
        }
        Debug.Log($"{sb}的逆序数为{revn}");
        return revn % 2 == 0;
    }

    public void ShowMsg(string msg)
    {
        Message.text = msg;
    }

    public void OnStartButton()
    {

        Debug.Log("OnStartButton");
        if (CheckInput(out initialInput))
        {
            ShowMsg("输入成功，正在为您求解");
            solving = true;

            // Grid.Init(ProblemSize, initialInput);
            Search.SearchMethod method;
            switch (algoType)
            {
                case AlgoType.BFS:
                    method = Search.SearchMethod.BFS;
                    break;
                case AlgoType.DFS:
                    method = Search.SearchMethod.DFS;
                    break;
                case AlgoType.ASTAR1:
                    method = Search.SearchMethod.ASTAR1;
                    break;
                case AlgoType.ASTAR2:
                    method = Search.SearchMethod.ASTAR2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SearchEngine = new Search(new State(ProblemSize, initialInput), method);

            if (mode == GameMode.Speed)
            {
                //abort = false;
                //threadDone = false;
                ShowMsg("计算中，请耐心等待");
                //var thread = new Thread(CalculatingThread);
                //thread.Start();
                TT.Instance.StartThread(SearchEngine);
            }
            else
            {
                ShowMsg("同步展示搜索过程……");
            }

            //nodeCount = 0;
            // //TEST
            // Grid.AddOperation(Operation.UP);
            // Grid.AddOperation(Operation.UP);
            // StartCoroutine(GetNextSearchStatus());
        }
    }

    public void OnResetButton()
    {
        Debug.Log("OnResetButton");
        ShowMsg("已重置");
        Grid.Clear();
        TT.Instance.Reset();
        SearchEngine = null;
        solving = false;
        //abort = true;
    }

    public void OnRandomButton()
    {
        List<int> lst = new List<int>();
        for (int i = 0; i < ProblemSize; i++)
        {
            lst.Add(i);
        }

        Shuffle(lst);
        while (!CanBeSolved(lst.ToArray()))
        {
            Shuffle(lst);
        }

        StringBuilder sb = new StringBuilder();
        foreach (int i in lst)
        {
            sb.Append(i.ToString() + ' ');
        }
        inputCache = sb.ToString();
        inputField.text = inputCache;
    }

    /// <summary>
    /// Knuth洗牌算法
    /// </summary>
    private void Shuffle(List<int> lst)
    {
        for (int i = lst.Count - 1; i >= 0; i--)
        {
            var r = new System.Random();
            var j = r.Next(0, i + 1);
            (lst[i], lst[j]) = (lst[j], lst[i]);
        }
    }


    void Start()
    {

    }

    public static Search SearchEngine;

    //private static int nodeCount = 0;

    //private static Search.SearchResult result;

    //private static bool threadDone = false;

    //private static bool abort = false;

    //private static float startTime;

    //private void CalculatingThread()
    //{
    //    threadDone = false;
    //    startTime = Time.time;
    //    for (; ; )
    //    {
    //        if (abort) return;
    //        SearchEngine.RefreshNext();
    //        nodeCount++;
    //        var res = SearchEngine.CurResult;
    //        if (res.SearchSucceed == true || res.SearchCanProceed == false)
    //        {
    //            result = res;
    //            break;
    //        }

    //    }
    //    threadDone = true;
    //}


    // Update is called once per frame
    void Update()
    {
        if (solving)
        {
            if (mode == GameMode.Visiable)
            {
                if (Grid.available)
                {
                    SearchEngine.RefreshNext();
                    var res = SearchEngine.CurResult;
                    //Debug.Log($"curRes: {res.CurSearchingNode.state}");
                    if (!res.SearchCanProceed)
                    {
                        ShowMsg("搜索不能继续，如果可能，请将截图提交至github issue");
                        solving = false;
                    }
                    var node = res.CurSearchingNode;
                    if (node.preNode == null)
                    {
                        Grid.Clear();
                        Grid.Init(node.state);
                    }
                    else
                    {
                        Grid.AddStep(new SquareGrid.Step(node.preNode.state, node.preOp));
                    }
                    //nodeCount++;
                    if (res.SearchSucceed)
                    {
                        solving = false;
                        ShowMsg($"搜索成功，{res.CurSearchingNode.state}步完成");
                    }
                }
            }
            else
            {
                Debug.Log($"node count: {TT.nodeCount}");
                if (TT.done)
                {
                    solving = false;
                    TT.Instance.Stop();
                    if (TT.failed)
                    {
                        ShowMsg("搜索不能继续，如果可能，请将截图提交至github issue");
                    }
                    else
                    {
                        var r = TT.result;
                        var node = r.CurSearchingNode;
                        List<Search.Node> lst = new List<Search.Node>
                        {
                            node
                        };
                        while (node.preNode != null)
                        {
                            lst.Add(node.preNode);
                            node = node.preNode;
                        }
                        Grid.Clear();
                        Grid.available = true;
                        if (lst.Count <= 1)
                        {
                            Grid.Init(lst[lst.Count - 1].state);
                        }
                        ShowMsg($"搜索成功，用时{Time.time - TT.startTime}秒，共搜索了{TT.nodeCount}个节点，" +
                            $"现在展示找到的路径，路径长度{lst.Count}");
                        for (int i = lst.Count - 2; i >= 0; i--)
                        {
                            Grid.AddStep(new SquareGrid.Step(lst[i].preNode.state, lst[i].preOp));
                        }
                    }
                }
            }
        }
    }
}