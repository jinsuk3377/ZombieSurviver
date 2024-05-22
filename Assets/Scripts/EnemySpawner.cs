using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour {
    public Enemy enemyPrefab; // 생성할 적 AI

    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float damageMax = 40f; // 최대 공격력
    public float damageMin = 20f; // 최소 공격력

    public float healthMax = 200f; // 최대 체력
    public float healthMin = 100f; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int wave; // 현재 웨이브

    private void Update() 
    {
        //좀비를 생성해야 하는 타이밍인지를 실시간으로 체크
        // 1. 게임 오버 상태가 아니다.
        
        if (GameManager.instance != null && GameManager.instance.isGameover == true)
        {
            //게임 오버 상태일 때에는 즉시 함수를 끝낸다.
            return;
        }

        //2.맵 위에 에너미의 수가 0인지(모두 물리쳤는지)
        if(enemies.Count <= 0)
        {
            SpawnWave();
        }
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, enemies.Count);
    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave() {
        //실행할 때마다 게임이 난이도가 조금씩 높아지게 하는 역할

        //웨이브를 1 증가시킨다.
        wave++;

        //적을 생성 : 현재 웨이브의 x 1.5배의 수만큼 좀비를 생성 (소수일 경우에는 반올림)
        //특정 소수를 반올림 해주는 함수 : Mathf.RoundToInt(반올림 할 수)
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        //spawnCount의 수만큼 좀비를 생성 =>CreateEnemy 를 실행
        for(int i=0; i<spawnCount; i++)
        {
            //적의 강함 정도는 랜덤으로 결정 (0~1 사이의 값)
            float enemyIntensity = Random.Range(0f,1f);
            CreateEnemy(enemyIntensity);
        }

    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy(float intensity) {
        //intensity : 적이 얼마나 강한지

        //Lerp : 선형 보간 : a와 b 사이의 수 중에서 n% 지점에 위치한 수가 무엇인가를 구하는 수학 공식

        //intensity로 받아온 값에 해당하는 체력 구하기

        float health = Mathf.Lerp(healthMin, healthMax, intensity);

        //좀비의 대미지
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);

        //좀비의 이동속도
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);

        //좀비의 피부색 : 색은 Color.Lerp로 지정한다.
        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity);

        //좀비 생성 => 좀비를 어디서 스폰하는지 설정
        // 스폰 포인트 4개 중 1곳에서 랜덤으로 스폰.
        int randomSpawn = Random.Range(0, spawnPoints.Length);

        Transform spawnPoint = spawnPoints[randomSpawn];
        //좀비 생성 => 좀비 생성 후에 해당 정보를 변수에 임시로 저장 (좀비 파워를 정하기 위해)
        // => 좀비 프리팹을 스폰 포인트에서 스폰 포인트의 회전값으로 소환한다.
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //만들어진 좀비의 세기를 결정 => 위에 구했던 체력, 대미지, 이동 속도, 피부 색 등을 적용
        enemy.Setup(health, damage, speed, skinColor);

        //좀비를 현재 맵 위에 있는 좀비 리스트에 추가 => 추후 관리를 위함
        enemies.Add(enemy);

        //이 에너미가 죽으면 일으킬 행동을 Action (이벤트 함수)에 추가. 
        // => 에너미가 죽을 때 일어날 행동을 예약

        //지금 소환된 에너미가 죽으면
        //1. 이 스크립트의 에너미 리스트에서 삭제. => 리스트에서 어떤 요소 제거 함수 : Remove
        enemy.onDeath += () => enemies.Remove(enemy);
        //2. 이 에너미의 오브젝트를 10초 후 파괴
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);


        //3. 게임의 점수를 상승. (보류)
        enemy.onDeath += () => GameManager.instance.AddScore(100);

    }
}