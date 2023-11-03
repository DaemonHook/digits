using UnityEngine;
using System.Threading;
using System;

public class TT : MonoBehaviour
{
    public static TT Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static Search SearchEngine;

    public static int nodeCount = 0;

    public static Search.SearchResult result;

    public static bool done = false;

    public static bool failed = false;

    public static bool abort = false;

    public static float startTime;


    private void CalculatingThread()
    {
        done = false;
        failed = false;
        while (!abort)
        {
            SearchEngine.RefreshNext();
            nodeCount++;
            var res = SearchEngine.CurResult;
            if (res.SearchSucceed == true || res.SearchCanProceed == false)
            {
                if (res.SearchCanProceed == false)
                {
                    failed = true;
                }
                result = res;
                break;
            }

        }
        done = true;

    }

    private Thread th;

    public void StartThread(Search searchEngine)
    {
        startTime = Time.time;
        nodeCount = 0;
        SearchEngine = searchEngine;
        th = new Thread(CalculatingThread);
        th.Start();
    }

    public void Stop()
    {
        abort = true;
    }


    private void OnDestroy()
    {
        th.Abort();
    }

    public void Reset()
    {
        if (th != null && th.IsAlive)
        {
            th.Abort();
        }

        th = null;
        done = false;

        failed = false;

        abort = false;
        SearchEngine = null;

        nodeCount = 0;
    }
}