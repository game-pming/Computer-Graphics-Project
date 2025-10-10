
using UnityEngine.XR;

using System.Collections; 
using System.Collections.Generic; 
using Unity.VisualScripting; 
using UnityEditor; 
using UnityEngine; 
using UnityEngine.UI; 

public class Player : MonoBehaviour
{
    Transform cam;
    public float speed;
    public float attackSpeed;
    public int hp;
    public int score;
    public int coin;

    public float jumpPower;
    public float rotationSpeed;
    public float maxSpeed;
    public float maxHp;
    public float maxJumpPower;
    public float followRad;

    int cakeCnt;
    float waitAttack;
    float hAxis;
    float vAxis;

    bool spaceDown;
    bool shiftDown;
    bool FDown;
    bool fireReady;

    Vector3 moveVec;
    Vector3 attackVec;
    Vector3 reloadVec;
    Vector3 throwVec;


    bool isInvincible;
    bool isDead;
    bool isMove;
    bool isJump;
    bool isAttack;
    bool isReload;
    bool hasCakes;

    Animator anim;
    Rigidbody rig;
    Monster mon;
    Monster colMon;
    SkinnedMeshRenderer[] meshes;

    public GameObject equipedCake;
    public GameObject throwCake;
    public Transform cakePos;
    public Text cakeCntTxt;

    void Awake()
    {
        cakeCnt = 5;
        hasCakes = true;
        anim = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Debug.Log(hp);
        if (isDead) return;
        InputKey();
        looking();
        Move();
        Jump();
        Attack();
    }


    void LateUpdate()
    {
        cakeCntTxt.text = string.Format("{0:n0}", cakeCnt);
    }

    void InputKey()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        spaceDown = Input.GetButtonDown("Jump");
        shiftDown = Input.GetButton("Run");
        FDown = Input.GetButton("Fire1");
    }

    void looking()
    {
        if (moveVec != Vector3.zero && !isReload)
        {
            transform.forward = moveVec;
        }
    }

    void Move()
    {
        if (isReload) return;
        Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = cam.right; right.y = 0f; right.Normalize();
        moveVec = forward * vAxis + right * hAxis;
        transform.position += moveVec * speed * (shiftDown ? 1f : 0.4f) * Time.deltaTime;
        anim.SetBool("isWalk", moveVec != Vector3.zero);
        anim.SetBool("isRun", shiftDown);
    }

    void Jump()
    {
        if (spaceDown && !isJump && !isReload)
        {
            rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            mon = other.GetComponent<Monster>();
            if (!mon.isDead) mon.FollowTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            mon = other.GetComponent<Monster>();
            if (!mon.isDead) mon.FollowTargetOut();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }

        if (collision.gameObject.tag == "Monster")
        {
            if (isInvincible) return;  // 公利捞搁 单固瘤 公矫

            colMon = collision.gameObject.GetComponent<Monster>();
            colMon.Attack();
            hp -= 10;
            if (hp <= 0)
            {
                isDead = true;
                hp = 0;
                Die();
            }
            if (isDead) return;

            StartCoroutine(OnDamaged());
        }
    }

    IEnumerator OnDamaged()
    {
        isInvincible = true; // 公利 矫累
        Vector3 hitVec = (transform.position - colMon.gameObject.transform.position).normalized;
        rig.AddForce(hitVec * 10 + Vector3.up * 2, ForceMode.Impulse);

        foreach (SkinnedMeshRenderer mesh in meshes)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.3f);

        foreach (SkinnedMeshRenderer mesh in meshes)
            mesh.material.color = Color.white;

        yield return new WaitForSeconds(1f); // 公利 瘤加 矫埃

        rig.velocity = Vector3.zero;
        rig.angularVelocity = Vector3.zero;

        isInvincible = false; // 公利 辆丰
    }

    void Die()
    {
        foreach (SkinnedMeshRenderer mesh in meshes) { mesh.material.color = Color.gray; }
        anim.SetTrigger("doDie");
        gameObject.layer = 7;
    }

    void Reload()
    {
        if (!isAttack && !isReload)
        {
            isReload = true;
            Vector3 reloadVec = -cam.forward; reloadVec.y = 0;
            transform.forward = reloadVec;
            anim.SetTrigger("doReload");
            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        isReload = false;
        cakeCnt = 5;
        hasCakes = true;
        reloadVec = cam.forward; reloadVec.y = 0;
        transform.forward = reloadVec;
    }

    void Attack()
    {
        waitAttack += Time.deltaTime;
        fireReady = waitAttack >= attackSpeed;
        if (FDown && hasCakes && !isAttack && fireReady)
        {
            attackVec = cam.forward; attackVec.y = 0;
            transform.forward = attackVec;
            anim.SetTrigger("doAttack");
            GameObject InstCake = Instantiate(throwCake, cakePos.position, cakePos.rotation);
            Rigidbody cakeRig = InstCake.GetComponent<Rigidbody>();
            throwVec = cam.forward.normalized; throwVec.y = 0;
            cakeRig.AddForce(throwVec * 30 + Vector3.up.normalized * 5, ForceMode.Impulse);
            cakeCnt--;
            waitAttack = 0f;
        }
        isAttack = true;
        Invoke("AttackOut", 0.3f);
    }

    void AttackOut()
    {
        isAttack = false;
        if (cakeCnt > 0) hasCakes = true;
        else
        {
            hasCakes = false;
            Reload();
        }
    }
}
