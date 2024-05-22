using UnityEngine;
using UnityEngine.AI; // 내비메쉬 관련 코드

// 주기적으로 아이템을 플레이어 근처에 생성하는 스크립트
public class ItemSpawner : MonoBehaviour {
    public GameObject[] items; // 생성할 아이템들
    public Transform playerTransform; // 플레이어의 트랜스폼

    public float maxDistance = 5f; // 플레이어 위치로부터 아이템이 배치될 최대 반경

    public float timeBetSpawnMax = 7f; // 최대 시간 간격
    public float timeBetSpawnMin = 2f; // 최소 시간 간격
    private float timeBetSpawn; // 생성 간격

    private float lastSpawnTime; // 마지막 생성 시점

    private void Start() {
        // 생성 간격과 마지막 생성 시점 초기화
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0;
    }

    // 주기적으로 아이템 생성 처리 실행
    private void Update() {

        //현재 시간과 생성 주기를 비교
        //현재 시각이 생성 주기 이상의 시간이 지났을 때
        // + 플레이어 캐릭터가 있을 때
        if(Time.time > lastSpawnTime + timeBetSpawn && playerTransform != null)
        {
            // 아이템을 생성     //아이템 생성 간격은 매번 달라진다.
            // => 아이템 생성 산격을 재설정
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            // => 마지막 생성 시간을 설정
            lastSpawnTime = Time.time;
            // => 아이템 생성
            Spawn();

        }
      
    }

    // 실제 아이템 생성 처리
    private void Spawn() {

        //플레이어 주변의 랜덤한 위치에 아이템을 스폰한다.
        //GetRandomPointOnNavMesh 함수 이용

        Vector3 itemSpawnPoint = GetRandomPointOnNavMesh(playerTransform.position, maxDistance);

        //생성 지점의 y축 값을 상승해서 공중에 살짝 떠오르게 만든다.
        itemSpawnPoint += new Vector3(0, 0.5f, 0);

        //아이템 중 1개를 리스트 중 무작위로 골라
        int random = Random.Range(0, items.Length);
        GameObject spawnObject = items[random];

        //itemSpawnPoint 위치에 생성한다.
        GameObject item =  Instantiate(spawnObject, itemSpawnPoint, Quaternion.identity);

        //만든 아이템을 5초 뒤 파괴한다.
        Destroy(item, 5f);
    }

    // 내비메시 위의 랜덤한 위치를 반환하는 메서드
    // center를 중심으로 distance 반경 안에서 랜덤한 위치를 찾는다
    private Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance) {

        // Random.insideUnitSphere : 특정 중심점을 기준으로
        // 구 형체 내의 랜덤한 좌표값을 반환하는 함수
        //Random.insideUnitSphere * 반경 + 중심점

        Vector3 randomPosition = Random.insideUnitSphere * distance + center;

        //좀비들이 걸어다니는 경로이자 플레이어가 걸어다닐 수 있는 길은
        //내비매시의 위다. (내비매시 위만 걸어다닐 수 있다.)
        //위에서 구한 랜덤 좌표에서 가장 가까이 위치한
        //내비메쉬의 좌표값을 구한다.
        // => SamplePosition

        //내비매쉬 위의 위치값을 저장할 변수 선언
        NavMeshHit hit;

        //NavMesh.SamplePosition(내비매쉬 좌표를 구할 기존 포지션, out hit(구한 내비매시 좌표를 저장할 변수),
        //내비매쉬 반경,
        //내비매시의 어떤 레이어를 감지할지)
        NavMesh.SamplePosition(randomPosition, out hit, distance, NavMesh.AllAreas);

        //hit에 변수의 값이 저장된다.
        // 찾은 점 반환
        return hit.position;
    }
}