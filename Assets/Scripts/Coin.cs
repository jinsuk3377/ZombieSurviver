using UnityEngine;

// 게임 점수를 증가시키는 아이템
public class Coin : MonoBehaviour, IItem {
    public int score = 200; // 증가할 점수

    public void Use(GameObject target) {
        //게임 점수가 score만큼 상승
        GameManager.instance.AddScore(score);

        // 먹은 아이템은 파괴한다.
        Destroy(gameObject);

    }
}