using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MonsterAI_RandomAnim : MonoBehaviour
{
    [Header("이동 반경/대기")]
    public float wanderRadius = 10f;     // 현재 위치 기준 랜덤 이동 반경
    public float minIdleTime = 0.8f;     // 목적지 도착 후 최소 대기
    public float maxIdleTime = 2.0f;     // 목적지 도착 후 최대 대기

    [Header("경로 재시도")]
    public int maxPickTries = 10;        // 목적지 고르기 재시도 횟수
    public float minNextDistance = 2.0f; // 너무 가까운 점은 제외

    [Header("애니메이터 파라미터")]
    public string speedParam = "Speed";  // Animator에 있는 파라미터 이름(숫자형)

    private NavMeshAgent agent;
    private Animator anim;
    private Vector3 startPos;
    private float idleUntil = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // 장애물 회피 품질(가시성 좋게)
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // 필요시 반경/속도 조절(맵 통로가 좁다면 radius를 약간 줄이세요)
        agent.radius = Mathf.Max(0.2f, agent.radius);
        startPos = transform.position;
    }

    void Start()
    {
        PickNewDestination();
    }

    void Update()
    {
        // 애니메이터: 이동속도를 파라미터로 전달 (Blend Tree용)
        float speed = agent.velocity.magnitude;
        if (anim && !string.IsNullOrEmpty(speedParam))
            anim.SetFloat(speedParam, speed);

        // 목적지 도착 판정
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.05f)
        {
            if (Time.time >= idleUntil)
                PickNewDestination(); // 대기 끝 → 새 목적지
        }

        // 경로가 막혔으면 다시 선택
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            PickNewDestination();
        }
    }

    void PickNewDestination()
    {
        // 도착 후 잠깐 쉬었다가 다음 목적지로
        idleUntil = Time.time + Random.Range(minIdleTime, maxIdleTime);

        for (int i = 0; i < maxPickTries; i++)
        {
            // 현재 위치 중심(또는 시작 위치 중심) 반경 내 랜덤 점
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir.y = 0f;
            Vector3 candidate = transform.position + randomDir;

            // NavMesh 위의 가장 가까운 점 샘플
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, wanderRadius, agent.areaMask))
            {
                // 너무 가까운 지점은 제외
                if (Vector3.Distance(transform.position, hit.position) < minNextDistance)
                    continue;

                // 시작점 기준 너무 멀어지지 않게(원한다면 유지)
                if (Vector3.Distance(startPos, hit.position) > wanderRadius * 1.2f)
                    continue;

                // 직선 광선으로 막혔는지 빠른 체크(선택)
                if (NavMesh.Raycast(transform.position, hit.position, out var raycastHit, agent.areaMask))
                    continue;

                agent.SetDestination(hit.position);
                return;
            }
        }

        // 실패 시: 제자리 대기 후 다시 시도
        idleUntil = Time.time + 1.0f;
    }
}
