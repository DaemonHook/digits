using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

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

    public GameObject digitPF;

    private State startState;

    private List<GameObject> digitGOs;
    private List<Digit> digits;
    private Dictionary<Vector2Int, Digit> digitDic = new Dictionary<Vector2Int, Digit>();

    public Vector3 DigitScale => new(1.0f / startState.radius, 1.0f / startState.radius,
        1.0f / startState.radius);

    public float DigitRadius => DigitScale.x / 2f;

    private Vector2Int zeroPos;

    public Queue<Step> stpQueue = new Queue<Step>();

    public bool available;
    
    public struct Step
    {
        public State startState;
        public Operation op;
    }

    private Vector2 GetRealPos(Vector2Int logicPos)
    {
        return new Vector2(
            -0.5f + DigitRadius + 2 * DigitRadius * logicPos.x,
            -0.5f + DigitRadius + 2 * DigitRadius * logicPos.y
        );
    }

    private Vector2Int GetLogicPos(int number)
    {
        for (int i = 0; i < startState.radius; i++)
        {
            for (int j = 0; j < startState.radius; j++)
            {
                if (startState.digits[i, j] == number)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        throw new Exception($"{number} not found in grid");
    }

    public void Init(State start)
    {
        available = true;
        startState = start;
        stpQueue.Clear();
        digitDic.Clear();
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

        //没有0
        for (int i = 1; i < startState.length; i++)
        {
            var newDigitGO = Instantiate(digitPF, transform);
            var d = newDigitGO.GetComponent<Digit>();
            digitGOs.Add(newDigitGO);
            digits.Add(d);
            d.Init(i, DigitScale);
            var logicPos = GetLogicPos(i);
            digitDic.Add(logicPos, d);
            var realPos = GetRealPos(logicPos);
            Debug.Log($"{i} logic pos: {logicPos}, real pos: {realPos}");
            d.transform.localPosition = realPos;
        }
    }

    public void Clear()
    {
        available = true;
        stpQueue.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (available && stpQueue.Count > 0)
        {
            var stp = stpQueue.Dequeue();
            Debug.Log($"grid doing {stp.op}");
            DoStep(stp);
        }
    }
    
    private void DoStep(Step stp)
    {
        Init(stp.startState);
        var toMovePos = zeroPos + Util.Delta[stp.op];
        var digit = digitDic[toMovePos];
        var dGO = digit.gameObject;
        var moveToRealPos = GetRealPos(zeroPos);
        available = false;
        var twe = dGO.transform.DOLocalMove(moveToRealPos, 1.0f);
        twe.OnComplete(() => { available = true; });
        digitDic.Remove(toMovePos);
        digitDic.Add(zeroPos, digit);
        zeroPos = toMovePos;
    }

    public void AddStep(Step stp)
    {
        stpQueue.Enqueue(stp);
    }
}