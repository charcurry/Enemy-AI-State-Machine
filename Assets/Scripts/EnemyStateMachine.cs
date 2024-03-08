using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    // General variables used in most if not all States.
    private NavMeshAgent navMeshAgent;
    public Vector3 target;
    private FPS_Controller player;
    public TextMeshProUGUI currentStateText;

    // The rule of thumb im using for the variables is that for the most part, they are self contained, but
    // variables such as chaseDistance will be used in other States as if the enemy is in the retreat state
    // but the player gets close, the enemy will need to start chasing the player. This is how I have all of
    // the variables & methods laid out.
    #region Patrol Variables

    [SerializeField] private GameObject[] waypoints;
    public int waypointIndex = 0;
    public Material patrol;

    #endregion

    #region Chase Variables

    public int chaseDistance;
    public Material chase;

    #endregion

    #region Search Variables

    public Vector3 lastPlayerPosition;
    public float searchTimerDuration;
    [SerializeField] private float searchTimer;
    public Material search;

    #endregion

    #region Attack Variables

    public int attackDistance;
    public float attackTimerDuration;
    [SerializeField] private float attackTimer;
    public Material attack;

    #endregion

    #region Retreat Variables

    public Material retreat;

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
        player = FindObjectOfType<FPS_Controller>();
        InitWaypoints();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyState = EnemyStates.patrol;
        searchTimer = searchTimerDuration;
        attackTimer = attackTimerDuration;
    }

    // Update is called once per frame
    void Update()
    {
        currentStateText.text = "Current State: " + enemyState.ToString();

        switch (enemyState)
        {
            case EnemyStates.patrol:
                GetComponent<Renderer>().material = patrol;
                target = waypoints[waypointIndex].transform.position;
                navMeshAgent.SetDestination(target);
                if (Vector3.Distance(transform.position, target) < 1)
                {
                    UpdateWaypoint();
                }

                if (Vector3.Distance(transform.position, player.transform.position) < chaseDistance)
                {
                    enemyState = EnemyStates.chase;
                }
                break;
                
            case EnemyStates.chase:
                GetComponent<Renderer>().material = chase;
                target = player.transform.position;
                navMeshAgent.SetDestination(target);
                lastPlayerPosition = player.transform.position;
                if (Vector3.Distance(transform.position, target) > chaseDistance)
                {
                    enemyState = EnemyStates.search;
                }
                if (Vector3.Distance(transform.position, target) < attackDistance)
                {
                    enemyState= EnemyStates.attack;
                }
                break;

            case EnemyStates.search:
                GetComponent<Renderer>().material = search;
                target = lastPlayerPosition;
                navMeshAgent.SetDestination(target);
                searchTimer -= Time.deltaTime;
                if (Vector3.Distance(transform.position, player.transform.position) < chaseDistance)
                {
                    enemyState = EnemyStates.chase;
                    searchTimer = searchTimerDuration;
                }
                if (searchTimer <= 0f)
                {
                    enemyState = EnemyStates.retreat;
                    searchTimer = searchTimerDuration;
                }
                break;

            case EnemyStates.attack:
                GetComponent<Renderer>().material = attack;
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
                    {
                        enemyState = EnemyStates.chase;
                        attackTimer = attackTimerDuration;
                    }
                    else
                    {
                        attackTimer = attackTimerDuration;
                    }
                }
                break;

            case EnemyStates.retreat:
                GetComponent<Renderer>().material = retreat;
                target = GetClosestWaypoint().transform.position;
                navMeshAgent.SetDestination(target);
                if (Vector3.Distance(transform.position, target) < 1)
                {
                    enemyState = EnemyStates.patrol;
                }
                if (Vector3.Distance(transform.position, player.transform.position) < chaseDistance)
                {
                    enemyState = EnemyStates.chase;
                }
                break;
        }
    }

    // These are the only two States that really needed external methods to make everything look cleaner.
    // They are only used in/pertain to the specified State. 
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

    #region Retreat Methods

    public GameObject GetClosestWaypoint()
    {
        if (waypoints.Length != 0)
        {
            GameObject closestWaypoint = waypoints[0];
            float closestDistance = Vector3.Distance(transform.position, closestWaypoint.transform.position);

            for (int i = 1; i < waypoints.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, waypoints[i].transform.position);
                if (distance < closestDistance)
                {
                    closestWaypoint = waypoints[i];
                    closestDistance = distance;
                }
            }

            return closestWaypoint;
        }
        return null;
    }

    #endregion

}
