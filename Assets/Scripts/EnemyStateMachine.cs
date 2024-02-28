using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Vector3 target;
    public FPS_Controller player;

    #region Patrol Variables

    [SerializeField] private GameObject[] waypoints;

    public int waypointIndex = 0;

    #endregion

    #region Chase Variables

    public int chaseDistance;

    #endregion

    public enum EnemyStates
    {
        patrol,
        chase,
        search,
        attack,
        retreat
    }

    public EnemyStates enemyState;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<FPS_Controller>();
        InitWaypoints();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyStates.patrol;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyState);

        switch (enemyState)
        {
            case EnemyStates.patrol:
                target = waypoints[waypointIndex].transform.position;
                navMeshAgent.SetDestination(target);
                if (Vector3.Distance(transform.position, target) < 1)
                {
                    //Debug.Log("Reached Waypoint!");
                    UpdateWaypoint();
                }

                if (Vector3.Distance(transform.position, player.transform.position) < chaseDistance)
                {
                    enemyState = EnemyStates.chase;
                }

                    break; 
            case EnemyStates.chase:
                target = player.transform.position;
                navMeshAgent.SetDestination(target);
                break;
            case EnemyStates.search:
                //
                //
                break;
            case EnemyStates.attack:
                //
                //
                break;
            case EnemyStates.retreat:
                //
                //
                break;
        }
    }

    #region Patrol Methods

    public void UpdateWaypoint()
    {
        waypointIndex++;
        if (waypointIndex > waypoints.Length - 1)
        {
            waypointIndex = 0;
        }
    }

    public void InitWaypoints()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
    }

    #endregion

}
