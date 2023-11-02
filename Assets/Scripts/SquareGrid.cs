using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SquareGrid : MonoBehaviour
{
    private void Awake()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public GameObject digit;

    [Header("正方形边长")] public int Radius;

    [Header("问题规模")] public int Length;

    [FormerlySerializedAs("State")] [Header("当前状态")]
    public int[] Input;

    private int[,] grid;

    private List<GameObject> digitGOs;
    private List<Digit> digits;

    public Vector3 DigitScale => new(1.0f / Radius, 1.0f / Radius, 1.0f / Radius);

    public float DigitRadius => DigitScale.x / 2f;

    private Vector2Int zeroPos;

    // public Vector2 

    public Vector2 GetDigitPos(Vector2Int logicPos)
    {
        return new Vector2(
            -0.5f + DigitRadius + 2 * DigitRadius * logicPos.x,
            -0.5f + DigitRadius + 2 * DigitRadius * logicPos.y
        );
    }

    public Vector2Int GetCurrentPos(int number)
    {
        for (int i = 0; i < Radius; i++)
        {
            for (int j = 0; j < Radius; j++)
            {
                if (grid[i, j] == number)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        throw new Exception($"{number} not found in grid");
    }

    public void Init()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
        Debug.Log($"DigitRadius: {DigitRadius}");
        digits = new List<Digit>();
        digitGOs = new List<GameObject>();
        digits.Add(null);
        digitGOs.Add(null);
        grid = new int[Radius, Radius];
        int cnt = 0;
        for (int i = 0; i < Radius; i++)
        {
            for (int j = 0; j < Radius; j++)
            {
                grid[i, j] = Input[cnt++];
                if (grid[i, j] == 0)
                {
                    zeroPos = new Vector2Int(i, j);
                }
            }
        }

        //没有0
        for (int i = 1; i < Length; i++)
        {
            var newDigitGO = Instantiate(digit, transform);
            var d = newDigitGO.GetComponent<Digit>();
            digitGOs.Add(newDigitGO);
            digits.Add(d);
            d.Init(i, DigitScale);
            var logicPos = GetCurrentPos(i);
            var realPos = GetDigitPos(logicPos);
            Debug.Log($"{i} logic pos: {logicPos}, real pos: {realPos}");
            d.transform.localPosition = realPos;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}