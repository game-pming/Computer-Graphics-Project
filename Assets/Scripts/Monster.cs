using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public int curHp;
    public float speed;
    public Transform target;
    public Player player;

    public bool isDead;
    bool isChase;
    public bool inRange;

    CapsuleCollider col;
    Rigidbody rig;
    Animator monAnim;
    NavMeshAgent nav;
    Material mat;

    // Start is called before the first frame update
    void Awake()
    {
        
        col = GetComponent<CapsuleCollider>();
        rig = GetComponent<Rigidbody>();
        monAnim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;   

        nav.isStopped = true;
        
    }

    void Update()
    {
        Vector3 lookVec = target.position - transform.position;
        lookVec.y = 0;
        transform.forward = -lookVec;
        if (nav.enabled && isChase) nav.SetDestination(target.position);

    }

    void FixedUpdate()
    {
        if (isChase && !isDead)
        {
            rig.velocity = Vector3.zero;
            rig.angularVelocity = Vector3.zero;
        }
        
    }

    public void FollowTarget()
    {
        isChase = true;
        nav.isStopped = !isChase;
        monAnim.SetBool("isWalk", true);
    }

    public void FollowTargetOut()
    {
        isChase = false;
        nav.isStopped = !isChase;
        monAnim.SetBool("isWalk", false);
    }

    // Update is called once per frame
    public void Attack()
    {
        monAnim.SetTrigger("doAttack");
    }

    public void Dead()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cake"))
        {
            curHp -= Random.Range(30, 50);
            if (curHp < 0)
            {
                curHp = 0;
                isDead = true;
            }

            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage()
    {
        if (!isDead)
        {
            mat.color = Color.red;
        }
        isChase = false;
        Vector3 forVec = (transform.position - target.position).normalized;
        rig.AddForce(forVec * 10 + Vector3.up * 10, ForceMode.Impulse);
        yield return new WaitForSeconds(0.3f);

        if (!isDead)
        {
            mat.color = Color.white;

            isChase = true;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 9;
            monAnim.SetTrigger("doDie");
            nav.enabled = false;
            rig.isKinematic = false;
            Destroy(gameObject, 3f);
            player.score += 100;
        }
        
    }
}
