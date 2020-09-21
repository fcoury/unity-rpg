using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed = 10f;
    public Animator myAnim;
    public static PlayerController instance;
    public string areaTransitionName;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    public Vector3 moveTo = Vector3.zero;
    public bool moveToEnabled;

    public bool canMove = true;

    void Start()
    {
        Debug.Log("Position: " + gameObject.transform.position);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (moveToEnabled) {
            Debug.Log(instance + " - " + this + " - Position: " + transform.position + " Move to: " + moveTo);
            transform.position = moveTo;
            Debug.Log("B:" + transform.position.x + " / " + Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x));
            moveToEnabled = false;
            moveTo = Vector3.zero;
            return;
        }

        float horizMove = Input.GetAxisRaw("Horizontal");
        float vertMove = Input.GetAxisRaw("Vertical");

        if (canMove)
        {
            theRB.velocity = new Vector2(horizMove * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed);
        }
        else
        {
            theRB.velocity = Vector2.zero;
        }

        myAnim.SetFloat("moveX", theRB.velocity.x);
        myAnim.SetFloat("moveY", theRB.velocity.y);

        if (horizMove == 1 || horizMove == -1 || vertMove == 1 || vertMove == -1)
        {
            if (canMove)
            {
                myAnim.SetFloat("lastMoveX", horizMove);
                myAnim.SetFloat("lastMoveY", vertMove);
            }
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
    }

    public void MoveTo(Vector3 destination)
    {
        moveTo = destination;
        moveToEnabled = true;
    }

    public void SetBounds(Vector3 botLeft, Vector3 topRight)
    {
        bottomLeftLimit = botLeft + new Vector3(.5f, 1f, 0);
        topRightLimit = topRight - new Vector3(.5f, 1f, 0);
    }
}
