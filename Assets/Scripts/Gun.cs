using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State {
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }

    public State state { get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력
    private float fireDistance = 50f; // 사정거리

    public int ammoRemain = 100; // 남은 전체 탄약
    public int magCapacity = 25; // 탄창 용량
    public int magAmmo; // 현재 탄창에 남아있는 탄약


    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간
    private float lastFireTime; // 총을 마지막으로 발사한 시점


    private void Awake() {
        // 사용할 컴포넌트들의 참조를 가져오기
        // bulletLineRenderer, gunAudioPlayer 참조 넣어주기 (Getcomponent)
        bulletLineRenderer = GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();

        //그 외 초기화 : 라인 렌더러 설정 초기화
        //선을 그리는데 사용할 점을 기본적으로 0개로 설정했으니,
        // 프로그램이 시작되는 동시에 점을 2개로 늘려준다.
        bulletLineRenderer.positionCount = 2;

        //점이 2개가 되어 선이 그어지면 화면에 바로 보이기 때문에
        //보이지 않도록 컴포넌트를 비활성화한다.
        bulletLineRenderer.enabled = false;

    }

    private void OnEnable() {
        // 총 상태 초기화
        //1. 현재 탄창이 가득 차 있어야 한다.
        //=> 현재 탄창에 들어있는 탄알의 양이 탄창 용량과 같아야 한다.
        magAmmo = magCapacity;

        //2. 총의 현재 상태가 '총을 쏠 준비가 되었다' 상태여야 한다.
        state = State.Ready;

        //3. 마지막으로 총을 쏜 시점은 0초전이어야 한다. (총 쏜 시간 초기화)
        lastFireTime = 0;
    }

    // 발사 시도
    public void Fire() {
        //발사가 가능한 상태가 언제인가?
        // => 마지막 총 발사 시점에서 총알을 발사할 수 있는 간격 만큼의 시간이 흘렀는지?
        // => 총의 상태가 '총을 쏠 준비가 되었다' 상태인지
        //Time.time : 현재 시각
        if ((Time.time >= lastFireTime + timeBetFire) && (state == State.Ready))
        {
            //마지막으로 총을 쏜 시간을 갱신
            lastFireTime = Time.time;

            //총을 쏠 준비가 되면
            Shot();
        }

    }

    // 실제 발사 처리
    private void Shot() {
        //실제로 총이 발사되면 실행할 함수

        //총이 맞는 위치를 어떻게 계산하는가? => 레이캐스트(눈에 보이지 않는 광선) 사용.
        //보이지 않는 광선을 직선으로 쏴, 광선이 닿는 위치에 좀비가 있을 경우
        //좀비가 피격당한 것으로 계산

        //충돌 정보를 저장할 레이캐스트 선언
        RaycastHit hit;

        //탄알이 맞은 장소를 저장할 변수 선언
        Vector3 hitPostion = Vector3.zero; //(0,0,0)

        //Raycast(광선의 시작 지점, 광선이 뻗어나갈 방향, 충돌 정보(out hit), 광선의 사정거리)
        // 광선의 시작 지점 : 총구 위치
        // 광선이 뻗어나갈 방향 : 총구로부터 정면
        // 광선의 사정거리 : 총알의 사정거리
        if(Physics.Raycast(fireTransform.position , fireTransform.forward ,out hit, fireDistance))
        {
            //if 내부의 함수 : 광선이 어딘가에 부딪혔을 때 실행

            //광선이 무언가에 부딪힌 경우
            // => 총알에 부딪힌 물체가 살아있는 생물인지 체크 

            // => 부딪힌 물체의 정보를 저장하기 위해 변수 선언
            //자료형? : 대미지를 입을 수 있는 모든 물체는 IDamageable 인터페이스를 지니고 있기 때문에,
            // 인터페이스를 변수형으로 받아온다.

            //부딪힌 물체가 IDamageable 인터페이스를 상속받고 있을 경우 해당 값을 참조한다.
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            //부딪힌 물체가 IDamageable 을 갖고 오지 못하면? => 대미지를 입을 수 없는 상대라는 뜻.(생명체가 아니다)
            if (target != null)
            {
                //IDamageable을 갖고 있는 상대에게 아래 함수를 실행한다. (=target이 비어있지 않을 경우)
                //IDamageable을 가진 물체는 반드시 OnDamage 함수를 갖고 있기 때문에
                //타겟으로부터 해당 함수를 실행한다.
                //OnDamage(대미지, 맞은 위치, 맞은 방향)
                target.OnDamage(damage, hit.point, hit.normal);
            }

            // 부딪힌 물체가 좀비인지 체크
            // 좀비가 맞다면 대미지를 준다.

            //좀비 여부와 관계 없이 광선이 어디에 부딪혔는지 위치를 저장.
            // => 광선 선 그리는 용도
            hitPostion = hit.point;
        }
        else
        {
            //광선이 어디에도 부딪히지 않았을 때
            //광선이 부딪힌 위치 : 탄알이 최대 사정거리까지 날아갔을 때의 위치값
            //   => 출발 위치 + 총구의 정면 방향 * 사정거리 
            hitPostion = fireTransform.position + fireTransform.forward * fireDistance;
        }

        //광선이 어디에 맞든 맞지 않든 상관없이 해야하는 동작
        //총알 발사 이펙트 시작
        StartCoroutine(ShotEffect(hitPostion));

        //현재 남은 탄알의 수 1개 감소
        magAmmo--;

        //만약 탄창에 남은 탄알의 수가 0개일 경우
        //총의 상태를 Empty로 바꿔준다.
        if(magAmmo <= 0)
        {
            state = State.Empty;
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    // hitPositin이 총알이 닿는 위치
    private IEnumerator ShotEffect(Vector3 hitPosition) {

        //시각적 효과
        //총구 화염 효과를 재생
        muzzleFlashEffect.Play();
        //탄피 배출 효과도 재생
        shellEjectEffect.Play();
        //gun에 달린 audioSource를 활용해 총격 소리도 1회 재생 (PlayOneShot)
        gunAudioPlayer.PlayOneShot(shotClip);


        //총이 발사되는 궤적을 라인렌더러를 통해서 그려준다.
        //선의 시작점 : 총구
        //라인렌더러.SetPosition(몇 번째 점인지, 그 점의 위치값)
        bulletLineRenderer.SetPosition(0, fireTransform.position);

        //선의 끝점(=1번째 점) : 총알이 닿는 목적지 = 매개변수 hitPosition으로 받아온다.
        bulletLineRenderer.SetPosition(1, hitPosition);

        //라인 렌더러 컴포넌트를 활성화 한다.
        bulletLineRenderer.enabled = true;

        //선이 보이도록 잠깐 대기 (0.03초) (new WaitForSecond)
        yield return new WaitForSeconds(0.03f);

        //라인 렌더러를 비활성화 한다.
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload() {

        //불가능한 경우에 false를 반환
        //재장전이 불가능한 경우
        //1. 재장전이 이미 진행중일 때 (총의 상태 체크)
        //2. 남은 전체 탄환의 갯수가 0 이하일 때
        //3. 현재 탄창에 탄알이 가득 차 있을 때
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }
        else
        {
            //재장전이 가능한 경우에 
            //ReloadRoutine 코루틴 함수를 실행
            //true를 반환
            StartCoroutine(ReloadRoutine());
            return true;
        }
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {

        //재장전 쿨타임 동안에는 다른 행동을 못하도록
        //총의 상태를 '재장전 중이다' 로 변경.
        state = State.Reloading;

        //--재장전 행동--//
        //재장전 효과음 재생 (1번)
        gunAudioPlayer.PlayOneShot(reloadClip);
        //재장전에 걸리는 소요시간만큼 대기
        yield return new WaitForSeconds(reloadTime);

        //------------------------------------


        //대기가 끝나면 탄창에 탄알을 채운다. (계산 필요)
        //채워야 할 탄창의 갯수를 저장할 지역변수 선언

        //현재 탄창에 몇 개의 탄알을 채워야 하는지 계산
        //(ex : 3발이 남은 상태에서 재장전 시 22발을 재장전 한다. => 탄창의 용량 - 현재 남은 탄창)
        int ammoToFill = magCapacity - magAmmo;

        //주의 : 남은 전체 탄환의 갯수가 채우려고 하는 갯수보다 작을 경우
        //재장전하는 총알의 갯수를 남은 전체 탄환의 갯수에 맞춰준다.
        if(ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }

        //이후에 탄환 채우기
        //현재 탄환에 재장전할 탄환 갯수 더해주기
        magAmmo += ammoToFill;

        //남은 전체 탄환에서는 재장전한 탄환의 갯수만큼 뺀다.
        ammoRemain -= ammoToFill;

        //재장전 행동 끝
        //총의 상태를 '총 발사 가능' 상태로 전환
        state = State.Ready;
    }
}