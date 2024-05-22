using UnityEditorInternal;
using UnityEngine;

// 체력을 회복하는 아이템
public class HealthPack : MonoBehaviour, IItem {
    public float health = 50; // 체력을 회복할 수치

    public void Use(GameObject target) {
        //획득한 플레이어의 체력을 회복한다.

        //Living Entity의 RestoreHealth 함수를 불러와 사용.
        LivingEntity livingEntity = target.GetComponent<LivingEntity>();

        //함수를 잘 불러올 수 있는지 예외 체크
        if(livingEntity != null)
        {
            //함수를 불러오는데 성공하면
            //체력을 회복한다.
            livingEntity.RestoreHealth(health);
            Debug.Log(PostProcessTest.State.a);     
                }

        //아이템 본인을 파괴.
        Destroy(gameObject);
    }
}