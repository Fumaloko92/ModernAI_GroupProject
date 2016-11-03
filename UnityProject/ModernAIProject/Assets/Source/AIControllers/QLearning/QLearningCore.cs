using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class QLearningCore : AIController
{
    private HashSet<GameObject> statesAvailable;
    private HashSet<GameObject> connectionsPlaceholders;
    private Executor executor;
    private QTable<Vector3> qTable;
    private bool initialized;
    private int count;
    void Awake()
    {
        statesAvailable = new HashSet<GameObject>();
        connectionsPlaceholders = new HashSet<GameObject>();
        destination = transform.position;
        initialized = false;
        executor = GameObject.FindGameObjectWithTag("Executor").GetComponent<Executor>();
    }
    void Start()
    {
        InitializeAgent();
        
        qTable = new QTable<Vector3>(world.Grid);
        count = 0;
        Initialize();
        /*ThreadStart childref = new ThreadStart(RunQLearningLoop);
        Thread childThread = new Thread(childref);
        childThread.Start();*/
        //  RunQLearningLoop();
    }

    private void Initialize()
    {
        if (count<10 && statesAvailable.Count > 0)
        {
            targetResource = world.GetRandomResource(executor.MaterialForDestination);

            if (targetResource != null)
            {
                targetPosition = targetResource.GetPositionInWorld();
                transform.position = qTable.GetRandomState();
                destination = GetNextState(transform.position);
                initialized = true;
            }
            else
            {
                activeAgent = false;
            }
        }
    }
    private void RunQLearningLoop()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 currentState = qTable.GetRandomState();
            do
            {
                Vector3 nextState = GetNextState(currentState);
                if (Mathf.Abs(Vector3.Distance(currentState, targetPosition)) < Mathf.Abs(Vector3.Distance(currentState, nextState)))
                    break;
                currentState = nextState;
            } while (true);
            Debug.Log("Destination reached!");

        }
        Debug.Log("QLearningLoop finished!");
    }

    void FixedUpdate()
    {
        if (activeAgent)
        {
            if (!initialized)
                Initialize();
            else
            {
                myAgent.SetDestination(destination);
                if (myAgent.remainingDistance < 0.05)
                {
                    Vector3 currentState = FindClosestStateAvailable().transform.position;
                    transform.position = currentState;
                    destination = GetNextState(currentState);
                    if (Mathf.Abs(Vector3.Distance(currentState, targetPosition)) < Mathf.Abs(Vector3.Distance(currentState, destination)))
                    {
                        count++;

                        collectedResources.Add(targetResource);
                        world.RemoveFromResourcePool(targetResource);
                        initialized = false;
                        Initialize();
                        Debug.Log("Destination reached![" + count + "]");
                    }
                    else
                        myAgent.SetDestination(destination);
                }
            }
        }
        //  Debug.Log(qTable.ToString());
    }

    private GameObject FindClosestStateAvailable()
    {
        float min_distance = float.MaxValue;
        GameObject min = null;
        foreach (GameObject v in statesAvailable)
        {
            float distance = Mathf.Abs(Vector3.Distance(transform.position, v.transform.position));
            if (min_distance > distance)
            {
                min = v;
                min_distance = distance;
            }
        }
        return min;
    }

    void OnTriggerEnter(Collider col)
    {
        statesAvailable.Add(col.gameObject);
    }


    void OnTriggerExit(Collider col)
    {
        statesAvailable.Remove(col.gameObject);
    }

    private Vector3 GetNextState(Vector3 position)
    {
        Vector3 nextState = qTable.GetNextState(position);
        qTable.UpdateQValues(position, nextState, 1, RewardFunction(position, nextState));
        UpdateConnectionPlaceholder(position, nextState, qTable.GetCostFromStateToState(position, nextState));
        return nextState;
    }

    private void UpdateConnectionPlaceholder(Vector3 from, Vector3 to, float value)
    {
        Vector3 midPoint = Vector3.Lerp(from, to, 0.25f);
        bool found = false;
        foreach (GameObject g in connectionsPlaceholders)
        {
            if (g.transform.position == midPoint)
            {
                g.name = value.ToString("n2");
                found = true;
            }
        }
        if (!found)
        {
            GameObject g = (GameObject)Instantiate(executor.ConnectionValuePlaceholder, midPoint, Quaternion.identity);
            g.name = value.ToString("n2");
            connectionsPlaceholders.Add(g);
        }
    }

    private float RewardFunction(Vector3 currentState, Vector3 nextState)
    {
        if (nextState == targetPosition)
            return float.MaxValue;
        float oldDistance, newDistance;
        oldDistance = Mathf.Abs(Vector3.Distance(currentState, targetPosition));
        newDistance = Mathf.Abs(Vector3.Distance(nextState, targetPosition));
        float reward;
        reward = (oldDistance - newDistance) / newDistance;
        return reward* newDistance;
    }
}
