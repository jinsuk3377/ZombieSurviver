using UnityEngine;

// 총알을 충전하는 아이템
public class AmmoPack : MonoBehaviour, IItem {
    public int ammo = 30; // 충전할 총알 수

    public void Use(GameObject target) {
        Debug.Log("총알 아이템");

        //획득 시 획득한 만큼 총알이 충전된다.
        //target이 플레이어가 될 예정이기 때문에
        // 플레이어 오브젝트로부터 총알 갯수에 어떻게 접근하는지?
        // Gun 클래스의 ammoRemain 에 어떻게 접근할지

        //1. Player 오브젝트의 PlayerShooter에 접근
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();

        //PlayerShooter 의 참조에 실패했을 경우에는 다른 함수를 실행하면 안 된다.
        //PlayerShooter의 참조에 성공해도, 내부의 gun에 접근 실패했을 경우에도 함수의 실행을 막아야 한다.
        if (playerShooter != null && playerShooter.gun != null)
        {
            //접근에 성공했을 경우 (예외처리)
            //총알의 전체 갯수에 ammo 만큼 더해준다.
            playerShooter.gun.ammoRemain += ammo;

            //먹은 아이템은 파괴
            Destroy(gameObject);
        }


    }
}