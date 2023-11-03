using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
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
        
        StringBuilder sb = new StringBuilder();
        foreach (var i in arr)
        {
            sb.Append(i.ToString() + ' ');
        }

        Debug.Log($"input string: [{sb.ToString()}]");
        return true;
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
        solving = false;
    }

    public Search SearchEngine;

    void Start()
    {
        // Grid.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (solving)
        {
            if (mode == GameMode.Visiable)
            {
                
            }
            else
            {
                
            }
        }
    }
}