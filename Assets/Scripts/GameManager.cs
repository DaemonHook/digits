using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float ScreenWidth = Screen.width;
    public float ScreenHeight = Screen.height;
    public InputField inputField;
    public SquareGrid Grid;

    public TMP_Text Message;
    public TMP_Dropdown ModeDropDown;
    public TMP_Dropdown AlgorithmDropDown;
    
    /// <summary>
    /// 8数码还是15数码
    /// </summary>
    public int ProblemSize = 9;

    public enum GameMode
    {
        Visiable,   //可视模式
        Speed       //测速模式
    }

    public GameMode mode = GameMode.Visiable;

    public enum AlgoType
    {
        BFS,    //深度优先
        DFS,    //广度优先
        ASTAR1, //A星算法1
        ASTAR2, //A星算法2
        RANDOM, //随机
    }

    public AlgoType algoType = AlgoType.BFS;
    
    public int[] initialInput;
    
    private string inputCache = "";

    private void Awake()
    {
        inputField.onValueChanged.AddListener(InputHook);
    }

    public void InputHook(string input)
    {
        inputCache = input;
        Debug.Log($"input: {inputCache}");
    }

    bool CheckInput(out int[] arr)
    {
        string[] ss = inputCache.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (ss.Length != ProblemSize)
        {
            ShowMsg($"输入数字个数不对，应为{ProblemSize}");
        }
        arr = new int[ss.Length];
        int count = 0;
        foreach (var s in ss)
        {
            try
            {
                arr[count] = int.Parse(s);
            }
            catch (Exception ex)
            {
                ShowMsg("输入的数字格式不对");
                return false;
            }
        }

        return true;
    }

    public void ShowMsg(string msg)
    {
        Message.text = msg;
    }
    
    public void OnStartButton()
    {
        if (CheckInput(out initialInput))
        {
            
        }
    }

    public void OnResetButton()
    {
        
    }
    
    public void StartSolving()
    {
        
    }
    
    void Start()
    {
        // Grid.Init();
    }

    // Update is called once per frame
    void Update()
    {
    }
}