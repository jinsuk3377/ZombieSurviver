using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun gun; // 사용할 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private PlayerInput playerInput; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }
    
    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전

        //입력 감지는 playerInput에서 담당. 이 스크립트에서는 해당 스크립트를 불러와 사용하면 된다.

        //playerInput의 fire 변수가 true일 경우 -> 마우스 입력을 받았다는 뜻
        // gun 스크립트에 있는 Fire() 함수를 실행한다.
        if (playerInput.fire)
        {
            gun.Fire();
        }
        //R 버튼을 누르면(playerInput의 reload 변수) 탄창이 재장전된다. 
        else if (playerInput.reload)
        {
            //재장전 실행
            bool isSuccess = gun.Reload();
            //재장전에 성공했을 때(Reload 함수의 반환값이 true일 때) 플레이어의 재장전 애니메이션을 실행.

            if(isSuccess)
            {
                //트리거 파라미터인 "Reload"를 실행
                playerAnimator.SetTrigger("Reload");
            }
        }

        //총알 상태 체크
        UpdateUI();
    }

    // 탄약 UI 갱신
    private void UpdateUI() {
        if (gun != null && UIManager.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }

    // 애니메이터의 IK 갱신 => 총의 위치와 캐릭터의 팔 위치가 분리되어 있는데,
    // 스크립트로 동기화 시켜준다.
    private void OnAnimatorIK(int layerIndex) {

        //총의 위치와 팔의 위치를 잡는다.

        //총 본체의 기준점 (gun pivot)을 3D 모델의 오른쪽 팔꿈치 위치로 이동시킨다.
        //GetIKHintPosition : 특정 관절의 위치값을 가져오는 함수
        //아바타 애니메이터 상의 오른쪽 팔꿈치 관절의 이름 : AvatarIKHint.RightElbow

        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);

        //왼손의 위치와 회전값은 총의 왼손이 있어야 할 자리에 고정시킨다.
        //특정 관절의 위치를 다른 지점으로 옮기는 함수 : SetIKPositionWeight

        //가중치(우선순위) 설정
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f); // 위치 연동
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f); // 회전값 연동

        //총의 위치에 손을 갖다댄다.
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        //오른손 설정
        //가중치(우선순위) 설정
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f); // 위치 연동
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f); // 회전값 연동

        //총의 위치에 손을 갖다댄다.
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
    }


}