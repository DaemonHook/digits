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
    private void Start()
    {
        available = true;
    }

    public GameObject digitPF;

    private State curState;

    private List<Digit> digits;

    public Vector3 DigitScale => new(1.0f / curState.radius, 1.0f / curState.radius,
        1.0f / curState.radius);

    public float DigitRadius => DigitScale.x / 2f;

    public Queue<Step> stpQueue = new Queue<Step>();

    public bool available = true;

    public struct Step
    {
        public State startState;
        public Operation op;

        public Step(State s, Operation o)
        {
            startState = s;
            op = o;
        }
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
        for (int i = 0; i < curState.radius; i++)
        {
            for (int j = 0; j < curState.radius; j++)
            {
                if (curState.digits[i, j] == number)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        throw new Exception($"{number} not found in grid");
    }

    public void Init(State start)
    {
        curState = start;
        //stpQueue.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        //Debug.Log($"DigitRadius: {DigitRadius}");
        digits = new List<Digit>();

        var locDict = new Dictionary<int, Vector2Int>();
        for (int i = 0; i < start.radius; i++)
            for (int j = 0; j < start.radius; j++)
            {
                locDict.Add(start.digits[i, j], new Vector2Int(i, j));
            }

        //没有0
        for (int i = 1; i < curState.length; i++)
        {
            var newDigitGO = Instantiate(digitPF, transform);
            var d = newDigitGO.GetComponent<Digit>();
            d.Init(i, DigitScale, locDict[i]);
            digits.Add(d);
            var logicPos = GetLogicPos(i);
            var realPos = GetRealPos(logicPos);
            //Debug.Log($"{i} logic pos: {logicPos}, real pos: {realPos}");
            d.transform.localPosition = realPos;
        }
    }

    public void Clear()
    {
        curState = null;
        stpQueue.Clear();
        available = true;
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
            available = false;
            var stp = stpQueue.Dequeue();
            Debug.Log($"step queue dequeued! length: {stpQueue.Count}");
            Debug.Log($"grid doing {stp.op}");
            DoStep(stp);
        }
    }

    Step stp;
    private void MoveZero()
    {
        var toMovePos = stp.startState.zeroPos + Util.Delta[stp.op];
        Debug.Log($"moving from {stp.startState.zeroPos} to {toMovePos}");
        var digit = digits.Find((d) =>
        {
            return d != null && d.Position == toMovePos;
        });
        var dGO = digit.gameObject;

        var moveToRealPos = GetRealPos(stp.startState.zeroPos);
        var twe = dGO.transform.DOLocalMove(moveToRealPos, 1.0f);
        twe.OnComplete(() => { available = true; });
    }

    private void DoStep(Step stp)
    {
        this.stp = stp;
        Init(stp.startState);
        Invoke("MoveZero", 0.5f);

        //digitDic.Remove(toMovePos);
        //digitDic.Add(zeroPos, digit);
    }

    public void AddStep(Step stp)
    {
        stpQueue.Enqueue(stp);
        Debug.Log($"step queue enqueued! length: {stpQueue.Count}");
    }
}