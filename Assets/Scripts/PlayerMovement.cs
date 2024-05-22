using System.Collections.Generic;
using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도


    private PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디
    private Animator playerAnimator; // 플레이어 캐릭터의 애니메이터

    private void Start() {
        // 사용할 컴포넌트들의 참조를 가져오기
        // => GetComponent
        //playerInput, playerRigidbody, playerAnimator

        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // FixedUpdate는 물리 갱신 주기 (0.02) 에 맞춰 실행됨
    private void FixedUpdate() {
        // 물리 갱신 주기마다 움직임, 회전, 애니메이션 처리 실행
        Move();

        Rotate();

        //애니메이션 처리
        //playerInput의 move 변수값을 그대로 애니메이터의 move 파라미터에 대입한다.
        playerAnimator.SetFloat("Move", playerInput.move);
    }

    // 입력값에 따라 캐릭터를 앞뒤로 움직임
    private void Move() {
        //캐릭터가 매 프레임마다 움직일 거리 계산.
        // playerInput의 move의 값 (-1~1사이의 값) 에다 일정한 시간 단위, 속도에 비례하게 움직인다.
        // 플레이어가 움직이는 방향은 오브젝트 기준의 정면방향으로 설정 (transform.forward)
        Vector3 moveDistance = playerInput.move * Time.deltaTime * moveSpeed * transform.forward;

        //리지드바디를 움직여서 게임 오브젝트의 위치 변경. => 원래 있던 장소를 기준으로 상대적으로 이동
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() {
        //playerInput의 rotate 값을 받아와, 일정한 시간 단위, 회전 속도에 비례하게 회전한다.
        float turn = playerInput.rotate * Time.deltaTime * rotateSpeed;

        //리지드바디를 이용해 게임 오브젝트의 회전상태 변경.
        //회전값은 Quaternion 이라는 자료형 이용. 

        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0);
    }
}