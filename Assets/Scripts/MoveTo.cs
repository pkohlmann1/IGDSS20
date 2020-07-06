using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.ComponentModel;
using UnityEditor.UIElements;

public class MoveTo : MonoBehaviour
{
    #region Tile Navigation
    public Tile _curr;
    public Tile _next;
    public Tile _dest;
    #endregion

    #region References
    private Worker worker;
    private Animator animator;
    #endregion

    #region Movement States
    bool isMoving = false;
    bool atWork = false;
    private float progress;
    private float duration;
    public static float movementSpeed = 0.2f;
    private static float offsetHeight = 0.0f;
    #endregion

    #region Idle Timer
    static float idleTime = 3f;
    float idletimer = 0f;
    #endregion

    void Start()
    {

        worker = GetComponent<Worker>();
        _curr = worker._house._tile;
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.speed = movementSpeed;
    }


    void Update()
    {
        if (worker._job == null)
        {
            if (_curr != worker._house._tile) 
            {
                startRoute(worker._house._tile);
            }
        }
        else
        {
            if (!isMoving)
            {
                idletimer += Time.deltaTime;
                if (idletimer > idleTime)
                {
                    idletimer = 0f;
                    Tile goal = atWork ? worker._house._tile : worker._job._building._tile;
                    startRoute(goal);
                }
            }
            else
            {
                MoveToNeighbor();
            }
        }
        

    }
    void Next()
    {
        _curr = _next;
        if(_curr == _dest) 
        {
            _dest = null;
            _next = null;
            isMoving = false;
            atWork = worker._job._building._tile == _curr;
            animator.SetBool("isWalking", isMoving);
        }
        else 
        { 
            _next = _curr.routeTo[_dest];
            duration = _curr.travelTime[_next];
        }
    }

    void startRoute(Tile destination) 
    {
        UnityEngine.Debug.Assert(_curr.routeTo.ContainsKey(destination), "destination not in routing Table!", this);
        if (_curr.routeTo.ContainsKey(destination))
        {
            Debug.Log("going to " + destination.ToString(), this);
            _dest = destination;
            _next = _curr.routeTo[_dest];
            duration = _curr.travelTime[_next];
            isMoving = true;
            atWork = false;
            animator.SetBool("isWalking", isMoving);
        }
    }

    void MoveToNeighbor()
    {
        float delta = Time.deltaTime;
        progress += delta*movementSpeed;
       
        while (progress > duration && isMoving) 
        {
            progress -= duration;
            Next();
        }
        Transform c = _curr.gameObject.GetComponent(typeof(Transform)) as Transform;
        if (isMoving)
        {
            Debug.Assert(duration > progress,"progress exceeded travelduration after loop!", this);
            float along = progress / duration;
            Transform n = _next.gameObject.GetComponent(typeof(Transform)) as Transform;
            setPosition(c, n,along);
        }
        else 
        {
            gameObject.transform.position = c.position;
        }
        



    }
    private void setPosition(Transform c,Transform n,float along) 
    {
        float along_1 = 1f - along;
        float y = along < 0.5 ? c.position.y : n.position.y;//height
        y += offsetHeight;
        float x = (c.position.x * along_1) + (n.position.x * along);
        float z = (c.position.z * along_1) + (n.position.z * along);
        gameObject.transform.position = new Vector3(x, y, z);
        Vector3 pointer = Vector3.ProjectOnPlane(n.position-c.position, Vector3.up);
        gameObject.transform.rotation = Quaternion.LookRotation(pointer);
    }

    private void setPosition(Transform c,Transform n) 
    {
        gameObject.transform.position = c.position;
        Vector3 pointer = Vector3.ProjectOnPlane(n.position - c.position, Vector3.up);
        gameObject.transform.rotation = Quaternion.LookRotation(pointer);
    }
}