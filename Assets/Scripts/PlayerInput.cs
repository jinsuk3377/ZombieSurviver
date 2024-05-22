using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class PlayerInput : MonoBehaviour {
    public string moveAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    public string rotateAxiName = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string fireButtsonName = "Fire1"; // 발사를 위한 입력 버튼 이름 - 마우스 좌클릭
    public string reloadButtonName = "Reload"; // 재장전을 위한 입력 버튼 이름 - R키 클릭

    // 값 할당은 내부에서만 가능
    public float move { get; private set; } // 감지된  회전 입력값
    public bool fire { get; private set; } // 감지된 움직임 입력값
    public float rotate { get; private set; } // 감지된발사 입력값
    public bool reload { get; private set; } // 감지된 재장전 입력값


    // 매프레임 사용자 입력을 감지
    private void Update() {

        //게임 오버 시에( + 게임 매니저의 인스턴스가 존재하지 않을 때) 는 입력값을 받아오지 않는다.
        if(GameManager.instance != null  && GameManager.instance.isGameover)
        {
            //모든 입력값을 기본값으로 초기화한다.
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;

            //아래의 함수를 실행하지 않는다.
            // => 함수를 여기서 종료한다.
            return;
        }

        //move에다 moveAxisName에 해당하는 키를 눌렀을 경우의 변하는 값을 저장.
        move = Input.GetAxis(moveAxisName);
        rotate = Input.GetAxis(rotateAxiName);

        //GetAxis : 방향을 받아오는 함수
        //GetButton : 버튼의 클릭 여부를 받아오는 함수. 버튼을 누르고 있는 동안 반복 실행.
        //GetButtonDown : 버튼이 클릭되는 순간에 1회만 실행. 누르고 있어도 반복 실행이 되지 않는다.
        fire = Input.GetButton(fireButtsonName);
        reload = Input.GetButtonDown(reloadButtonName);

    }
}