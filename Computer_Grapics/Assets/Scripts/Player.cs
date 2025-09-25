using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Transform cam;

    public float speed;
    public float hp;
    public float jumpPower;

    public float maxSpeed;
    public float maxHp;
    public float maxJumpPower;

    float hAxis;
    float vAxis;

    bool spaceDown;
    bool shiftDown;

    Vector3 lookingVec;
    Vector3 moveVec;

    bool isMove;
    bool isJump;
    bool isRun;

    Animator anim;
    Rigidbody rig;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        looking();
        Move();
        Jump();
    }

    void InputKey()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        spaceDown = Input.GetButton("Jump");
        shiftDown = Input.GetButtonDown("Run");
    }

    void looking()
    {
        if(moveVec != Vector3.zero)
        {
            lookingVec = cam.forward;
            lookingVec.y = 0f;
            transform.forward = lookingVec;
        }
        
    }

    void Move()
    {
        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();

        moveVec = forward * vAxis + right * hAxis;

        transform.position += moveVec * speed * (isRun ? 1f : 0.4f) * Time.deltaTime;


        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", shiftDown);
    }

    void Jump()
    {
        if (spaceDown && !isJump)
        {
            rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");

            Debug.Log(rig);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }
}
