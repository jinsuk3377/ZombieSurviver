using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 적 AI를 구현한다
public class Enemy : LivingEntity {
    public LayerMask whatIsTarget; // 추적 대상 레이어

    private LivingEntity targetEntity; // 추적할 대상 => 플레이어 추적용
    private NavMeshAgent pathFinder; // 경로계산 AI 에이전트

    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    public AudioClip deathSound; // 사망시 재생할 소리
    public AudioClip hitSound; // 피격시 재생할 소리

    private Animator enemyAnimator; // 애니메이터 컴포넌트
    private AudioSource enemyAudioPlayer; // 오디오 소스 컴포넌트
    private Renderer enemyRenderer; // 렌더러 컴포넌트 => 좀비의 피부색 (좀비의 강함 정도)

    public float damage = 20f; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget // 읽기 전용 프로퍼티
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake() {
        // 컴포넌트 초기화
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();

        //좀비의 렌더러 컴포넌트는 좀비 몸에 달려있지 않고
        //자식 오브젝트에 들어 있다. => 자식 오브젝트로부터 getcomponent를 실행해야 한다.
        //자식 오브젝트에 딱 한 개의 불러올 컴포넌트가 있을 때에만 쓸 수 있는 함수
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor) {

        //새로운 좀비를 스폰할 때마다 실행할 함수.
        //좀비마다 서로 다른 스펙을 가지도록 만들 수 있다.

        //좀비의 체력 설정
        startingHealth = newHealth;
        //좀비의 대미지 설정
        damage = newDamage;
        //좀비의 이동 속도 설정 => 네비메쉬 변수 중 speed 에 설정
        pathFinder.speed = newSpeed;
        //좀비의 피부색 설정 => Renderer의 .material.color  에 설정 가능
        enemyRenderer.material.color = skinColor;

    }

    private void Start() {

        //오브젝트가 준비되자마자
        //네비게이션 기능을 시작 => UpdatePath에서 시작
        StartCoroutine(UpdatePath());

    }

    private void Update() {

        //좀비 애니메이션 프로퍼티 중 "HasTarget" 은 값이 true가 되면
        //좀비가 달리는 애니메이션이 실행된다.
        //실시간으로 타겟이 있는지 여부를 체크해서, 있으면 애니메이션 상태를 true로 전환한다.
        //타겟 존재 여부는 hasTarget 프로퍼티로 체크
        // => hasTarget이 true면 애니메이션의 프로퍼티도 true, false면 같이 false
        enemyAnimator.SetBool("HasTarget", hasTarget);

    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath() {

        //네비게이션 기능을 활용해 타겟을 특정하고 쫓아다니는 함수
        
        //좀비가 살아있는 동안 무한 루프로 작동
        while(dead == false)
        {
            //쫓아야 하는 타겟이 있을 경우 : 네비게이션을 작동.
            //목적지를 타겟(targetEntity)의 위치로 선정 (SetDestination)
            if (hasTarget)
            {
                //네비게이션 작동 = 정지 상태 해제
                pathFinder.isStopped = false;
                //목적지를 타겟의 위치로 선정
                pathFinder.SetDestination(targetEntity.transform.position);
            }
            else
            {
                //쫓아야 할 타겟이 없는 경우 :
                //네비게이션 중지.
                pathFinder.isStopped = true;
                //타겟을 감시하는 센서를 발동. => 특정 범위 내에서 살아있는 모든 생명체(LivingEntity)를 찾는다.

                //특정 범위 내에 있는 모든 생명체의 콜라이더를 저장할 지역 변수를 선언.
                //Physics.OverlapSphere : 구체 모양으로 보이지 않는 영역을 만들고, 그 내부의 오브젝트를 탐색하는 기능
                //Physics.OverlapSphere(구체의 중심점, 구체의 반지름(범위),
                //누구를 대상으로 검색할건지-특정 레이어만 감지하도록 설정이 가능 => whatIsTarget 활용)
                //whatIsTarget의 레이러를 Player로 하면 Player 캐릭터만 감지한다.
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);

                //감지한 콜라이더를 모두 순회하여 살아있는 생명체를 찾는다.
                for(int i=0; i<colliders.Length; i++)
                {
                    //i번째 콜라이더 내에 들어있는 LivingEntity 컴포넌트를 갖고온다.
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                    //해당 livingEntity가 비어있는지 1차 확인, 있을 경우 livingEntity의 대상이 살아있는지 체크
                    if(livingEntity != null && livingEntity.dead == false)
                    {
                        //대상이 살아있을 경우에 타겟을 이 대상으로 지정.
                        //for을 즉시 정지.
                        targetEntity = livingEntity;
                        break;
                    }

                }
            }

            //이프문 처리가 끝나면 주기적으로 대기 후 다시 반복한다.
            yield return new WaitForSeconds(0.25f);

        }

    }

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) 
    {
        if(dead == false)
        {
            //총에 맞는 파티클을 재생.
            // => 파티클이 터질 위치 결정 (hitPoint로 받아온다.)
            // => 파티클이 터질 방향 결정 (hitNormal)
            // => 파티클 재생

            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //죽지 않은 상태에서 피격당했을 경우 효과음 재생

            enemyAudioPlayer.PlayOneShot(hitSound);
        }
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        //좀비는 콜라이더를 갖고 있기 때문에 쓰러진 이후에 다른 좀비들의 이동을 방해할 수 있다.
        //방해하지 못하게 하기 위해 좀비가 갖고 있는 모든 콜라이더를 비활성화 시킨다.
        //GetComponent를 복수형으로 사용할 수 있다. -> 끝에 s붙이기
        //Collider 클래스는 Box, Capsule 모양 상관없이 모든 콜라이더를 담을 수 있다.
       
        Collider[] enemyColliders = GetComponents<Collider>(); // 좀비의 박스 콜라이더, 캡슐 콜라이더가 담긴다.

        //반복문 돌려서 모든 콜라이더를 비활성화 시킨다.
        foreach(Collider collider in enemyColliders)
        {
            collider.enabled = false;
        }

        //네비게이션 작동 중지
        pathFinder.isStopped = false;
        //내비매쉬 에이전트 컴포넌트도 완전히 비활성화 시킨다.
        pathFinder.enabled = false;

        //사망 애니메이션 실행
        //좀비 애니메이터의 Die 트리거를 활성화한다.
        enemyAnimator.SetTrigger("Die");

        //사망 효과음 재생
        enemyAudioPlayer.PlayOneShot(deathSound);

    }

    private void OnTriggerStay(Collider other) {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행   

        //isTrigger 처리가 된 박스 콜라이더 내에 추적 대상(targetEntity)가 있을 경우 해당 대상을 공격한다.

        // 1. 본인이 사망 상태가 아니다.
        // 2. 공격 쿨타임이 지난 상태여야 한다. (현재 시각이 마지막 공격 시간 + 쿨타임 보다 커야 한다.)
        if (dead == false && Time.time >= lastAttackTime + timeBetAttack)
        {
            //상대 공격을 시도
            //트리거 범위 내의 대상에게서 LivingEntity를 가져오려 시도 (targetEntity와 비교하기 위해서)
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();

            //만약 attackTarget에 어떤 값을 불러오는데 성공했다면 (=부딪힌 상대에게 LivingEntity 컴포넌트가 있다는 뜻)
            //내 추적대상(targetEntity) 과 동일 대상인지 체크
            if(attackTarget != null && attackTarget == targetEntity)
            {
                //동일 대상까지 일치하면
                //최근 공격 시간을 갱신
                lastAttackTime = Time.time;
                //공격
                //함수에 넣기 위한 때리는 위치, 방향 계산
                //ClosestPoint : 콜라이더에 들어온 부위 중에서 가장 가까운 곳의 점 위치
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                //방향 : 벡터 계산식 = 출발지점 좌표 - 목표지점 좌표
                Vector3 hitNormal = transform.position - other.transform.position;

                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }

        }

    }
}