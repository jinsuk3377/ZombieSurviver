using UnityEngine;
using UnityEngine.UI; // UI 관련 코드

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : LivingEntity {
    public Slider healthSlider; // 체력을 표시할 UI 슬라이더

    public AudioClip deathClip; // 사망 소리
    public AudioClip hitClip; // 피격 소리
    public AudioClip itemPickupClip; // 아이템 습득 소리

    private AudioSource playerAudioPlayer; // 플레이어 소리 재생기
    private Animator playerAnimator; // 플레이어의 애니메이터

    private PlayerMovement playerMovement; // 플레이어 움직임 컴포넌트
    private PlayerShooter playerShooter; // 플레이어 슈터 컴포넌트

    private void Awake() {
        // 사용할 컴포넌트를 가져오기
        //playerAudioPlayer, playerAnimator, playerMovement, playerShooter
        playerAudioPlayer = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooter = GetComponent<PlayerShooter>();
    }

    protected override void OnEnable() {
        // LivingEntity의 OnEnable() 실행 (상태 초기화)
        base.OnEnable(); // 사망 상태 초기화, 체력 초기화

        //만약의 오류를 대비한 작업
        //체력 슬라이더 활성화 => 오브젝트의 활성화
        healthSlider.gameObject.SetActive(true);

        //체력 슬라이더의 최댓값을 체력의 기본값으로 설정
        healthSlider.maxValue = startingHealth;// 슬라이더의 최댓값

        //체력 슬라이더의 현재값을 체력의 현재값으로 설정
        healthSlider.value = health;// 슬라이더의 현재값

        //플레이어의 행동을 담당하는 playerMovement, playerShooter가 꺼져있을 경우를 대비하여
        //이 자리에서 활성화 => 스크립트의 활성화
        playerMovement.enabled = true;
        playerShooter.enabled = true;
    }

    // 체력 회복
    public override void RestoreHealth(float newHealth) {
        // LivingEntity의 RestoreHealth() 실행 (체력 증가)
        base.RestoreHealth(newHealth);

        //기능은 부모 클래스에 이미 모두 구현되어 있다.
        //체력 슬라이더에 업데이트된 체력 연동.
        healthSlider.value = health;

    }

    // 데미지 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitDirection) {

        //맞는 순간에 죽지 않았다면
        if(dead == false)
        {
            //피격 효과음 1회 재생
            playerAudioPlayer.PlayOneShot(hitClip);
        }

        // LivingEntity의 OnDamage() 실행(데미지 적용)
        base.OnDamage(damage, hitPoint, hitDirection); // 체력 감소, 사망 코드 구현

        //체력 슬라이더 업데이트
        healthSlider.value = health;
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die() 실행(사망 적용)
        base.Die(); // OnDeath 함수 실행, dead 값 true로 전환

        //죽은 이후
        //체력 슬라이더 오브젝트 비활성화
        healthSlider.gameObject.SetActive(false);

        //사망 효과음 재생
        playerAudioPlayer.PlayOneShot(deathClip);
        //사망 애니메이션 실행 => 애니메이터의 Die 트리거 활성화
        playerAnimator.SetTrigger("Die");

        //playerMovement, playerShooter의 업데이트 함수가 실행되는 것을 막기 위해서
        //두 스크립트 비활성화
        playerMovement.enabled = false;
        playerShooter.enabled = false;

    }

    private void OnTriggerEnter(Collider other) {
        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리

        //플레이어가 사망 상태가 아닐 때에만 아이템 획득이 가능하다.
        if(dead == false)
        {
            //사망하지 않았을 경우
            //충돌 물체가 아이템인지 확인.
            //=> 부딪힌 상대가 iItem 인터페이스를 참조하고 있는지 확인. (Getcomponent)
            IItem getItem = other.GetComponent<IItem>();

            //부딪힌 물체가 아이템이 맞을 경우
            // => getItem이 null이 아니다.
            if(getItem != null)
            {
                //해당 아이템을 사용한다.
                getItem.Use(gameObject);
                //아이템 사용 효과음 재생
                playerAudioPlayer.PlayOneShot(itemPickupClip);
            }


        }


    }
}