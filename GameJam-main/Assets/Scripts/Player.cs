using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;
    public int currentZone { get; set; } = 0;
    public int nextZone { get; private set; } = 0;

    public StateMachine stateMachine {  get; private set; }
    public IdleState idleState { get; private set; }
    public WalkState walkState { get; private set; }
    public PutOutFireState putOutFireState { get; private set; }
    public ShootState shootState { get; private set; }
    public FixFloorState fixFloorState { get; private set; }
    public FixSideState fixSideState { get; private set; }
    public FailState failState { get; private set; }

    private Vector3 mousePos;
    public Vector3 targetPos { get; private set; }

    private Vector3 potentialPos;

    [SerializeField] private GameObject dragedSailor;
    public float moveSpeed { get; private set; } = 5;
    public string currentTag { get; private set; }

    public NavMeshAgent navMeshAgent { get; private set; }

    public SkeletonAnimation skeletonAnimation {  get; private set; }
    public Animator animatorEffects;

    SoundManager soundManager;

    private bool isDragging = false;
    private BoxCollider playerCollider;
    private static Player currentlyDragging = null;
    private void Awake()
    {
        targetPos = transform.position;
        stateMachine = new StateMachine();
        idleState = new IdleState(this, stateMachine, "idle");
        walkState = new WalkState(this, stateMachine, "");
        putOutFireState = new PutOutFireState(this, stateMachine, "IsPutOutFire");
        shootState = new ShootState(this, stateMachine, "pushka");
        fixFloorState = new FixFloorState(this, stateMachine, "water");
        fixSideState = new FixSideState(this, stateMachine, "bort");
        failState = new FailState(this, stateMachine, "fail");

        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
       // animatorEffects = GetComponentInChildren<Animator>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        playerCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        stateMachine.Initialize(idleState);

        dragedSailor.SetActive(false);

        animatorEffects.gameObject.SetActive(false);

        navMeshAgent.updateRotation = false; 
        navMeshAgent.updateUpAxis = false;   
    }

    public Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }



    private void MouseDrag()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        potentialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePos);

        Plane plane = new Plane(Vector3.up, dragedSailor.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {            
            potentialPos = ray.GetPoint(distance);
            dragedSailor.transform.position = new Vector3(potentialPos.x, dragedSailor.transform.position.y, potentialPos.z);
            
        }


    }

    private void MouseUp()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        dragedSailor.SetActive(false);

        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;

        if (plane.Raycast(ray, out distance))
        {

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Collider collider = hitInfo.collider;
                if (collider.CompareTag("Gun") || collider.CompareTag("FloorHole") || collider.CompareTag("Fire") || collider.CompareTag("SideHole")) {
                    
                    if (ShipManager.Instance.CurrentState == ShipManager.ShipState.Madness)
                    {
                        ShipTask shipTask = hitInfo.collider.GetComponent<ShipTask>();
                        ShipTaskZone zone = shipTask.GetComponentInParent<ShipTaskZone>();
                        targetPos = ShipManager.Instance.GetInvertedShipTask(shipTask);
                        nextZone = ((int.Parse(hitInfo.collider.transform.parent.name[^1..]) - 1) 
                            + ShipManager.Instance.ShipTaskZones.Count / 2) % ShipManager.Instance.ShipTaskZones.Count;
                        if(targetPos == Vector3.zero)
                        {
                            targetPos = transform.position;
                        }
                        
                    }
                    else
                    {
                        targetPos = ray.GetPoint(distance);
                        nextZone = (int.Parse(hitInfo.collider.transform.parent.name[^1..])-1);
                    }

                    currentTag = hitInfo.collider.tag;
                    
                    if (!TaskAvailable()) return;

                    if (Vector3.Distance(transform.position, targetPos) > 0.1f)
                    {
                        stateMachine.ChangeState(walkState);
                    }
                }
                
                
            }
        }
    }

    private void Update()
    {
        stateMachine.currentState.Update();
        if (Mouse.current.leftButton.wasPressedThisFrame && currentlyDragging == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, playerLayerMask))
            {
                if (hitInfo.collider.gameObject == gameObject)
                {
                    isDragging = true;
                    currentlyDragging = this;
                    dragedSailor.SetActive(true);
                    mousePos = Input.mousePosition - GetMousePos();
                }
            }
        }
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            MouseDrag();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            currentlyDragging = null;
            MouseUp();
        }
    }

    public void ClearCurrentTask()
    {
        currentTag = "";
        nextZone = 0;
    }
    public bool TaskAvailable()
    {
        TaskType? type =
            currentTag == "Gun"       ? TaskType.Gun       :
            currentTag == "FloorHole" ? TaskType.FloorHole :
            currentTag == "SideHole"  ? TaskType.SideHole  :
            //currentTag == "Fire"      ? TaskType.Fire      :
                                        null               ;
        
        if (type == null) return false;

        return ShipManager.Instance.IsTaskActiveInZone((TaskType)type, nextZone);
    }
}

