//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Enemy Create Info")]       //인스펙터뷰에서 가독성을 높여줌
    //적 캐릭터가 출현할 위치를 담을 배열
    public Transform[] points;
    //적 캐릭터 프리팹을 저장할 변수
    public GameObject enemy;
    //적 캐릭터를 생성할 주기
    public float createTime = 2.0f;
    //적 캐릭터의 최대 생성 개수
    public int maxEnemy = 10;
    //게임 종료 여부를 판단할 변수
    public bool isGameOver = false;

    //싱글턴에 접근하기 위한 Static 변수 선언
    public static GameManager instance = null;

    [Header("Object Pool")]
    //생성할 총알 프리팹
    public GameObject bulletPrefab;
    //오브젝트 풀에 생성할 개수
    public int maxPool = 10;
    public List<GameObject> bulletPool = new List<GameObject>();

    //일시정지 여부를 판단하는 변수
    private bool isPaused;
    //Inventory의 Canvas Group 컴포넌트를 저장할 변수
    public CanvasGroup inventoryCG;

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        //instance에 할당된 클래스의 인스턴스가 다를 경우 새로 생성된 클래스를 의미함
        else if (instance != this)
            Destroy(this.gameObject);
        //다른 씬으로 넘어가더라도 삭제하지 않고 유지함
        DontDestroyOnLoad(this.gameObject);
        //오브젝트 풀링 생성함수 호출
        CreatePooling();
    }

   
    private void Start()
    {
        //처음 인벤토리를 비활성화
        OnInventoryOpen(false);

        //하이러키 뷰의 SpawnPointGroup을 찾아 하위에 있는 모든 트랜스폼 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        if(points.Length>0)
        {
            StartCoroutine(this.CreateEnemy());
        }
    }

    //인벤토리를 활성화/비활성화 하는 함수
    public void OnInventoryOpen(bool isOpened)
    {
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }

    //적 캐릭터를 생성하는 코루틴 함수
    IEnumerator CreateEnemy()
    {
        //게임 종료시까지 무한루프
        while (!isGameOver)
        {
            //현재 생성된 적 캐릭터의 개수 산출
            int enemyCount = (int)GameObject.FindGameObjectsWithTag("ENEMY").Length;

            //적 캐릭터의 최대 생성 개수보다 작을 때만 적 캐릭터 생성
            if (enemyCount < maxEnemy)
            {
                //적 캐릭터의 생성 주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = UnityEngine.Random.Range(1, points.Length);
                //적 캐릭터의 동적 생성
                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else
                yield return null;
        }
    }

    //오브젝트 풀에 총알을 생성하는 함수
    private void CreatePooling()
    {
        //총알을 생성해 차일드화할 페어런트 게임오브젝트를 생성
        GameObject objectPools = new GameObject("ObjectPools");

        //풀링 개수만큼 미리 총알을 생성
        for(int i=0;i<maxPool;i++)
        {
            //동적으로 생성한 Bullet프리팹이 하이러키 뷰에 쭉 나열되는 것을 방지하기 위해
            //스크립트에서 생성한 ObjectPools 게임오브젝트 하위에 생성한다.
            var obj = Instantiate<GameObject>(bulletPrefab, objectPools.transform);//생성할 프리팹, 특정 게임오브젝트의 transform => 그 게임오브젝트 하위에 생성됨
            obj.name = "Bullet_" + i.ToString("00");
            //비활성화
            obj.SetActive(false);
            //리스트에 생성한 총알 추가
            bulletPool.Add(obj);
        }
    }

    public GameObject GetBullet()
    {
        for(int i=0;i<bulletPool.Count;i++)
        {
            //비활성화 여부로 사용 가능한 총알인지를 판단
            if (bulletPool[i].activeSelf == false)
                return bulletPool[i];
        }
        return null;
    }

    public void OnPauseClick()
    {
        //일시정지 값을 토글시킴
        isPaused = !isPaused;
        //Time Scale이 0이면 정지, 1이면 정상 속도
        Time.timeScale = (isPaused) ? 0.0f : 1.0f;

        //Time.timeScale을 0으로 설정했다고 하더라도 주인공 캐릭터에 추가된 스크립트 중 마우스 클릭과 같은 이벤트는 동작한다.
        //즉, 일시정지한 상태에서 마우스 클릭을 하면 총알이 생성되고 사운드가 발생한다.
        //따라서 주인공 캐릭터에 있는 모든 스크립트를 추출한 후 일시정지 여부에 따라서 모두 활성화/비활성화 한다.

        //주인공 객체를 추출
        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공 캐릭터에 추가된 모든 스크립트를 추출함
        var scripts = playerObj.GetComponents<MonoBehaviour>();
        //주인공 캐릭터의 모든 스크립트를 활성화/비활성화
        foreach(var script in scripts)
        {
            script.enabled = !isPaused;
        }

        //Time.timeScale을 0으로 설정했을 때 마우스 클릭과 터치 이벤트는 여전히 정상적인 동작을 한다.
        //따라서 일시 정지가 됐을 때 일부 UI(무기교체버튼)가 클릭되지 않도록 구현
        //보통 일시정지됐을 때 비활성화해야 하는 각종 버튼은 하나의 빈 게임오브젝트 하위에 넣어두고 
        //부모 게임오브젝트 하나의 Canvas Group만 추가해 구현한다.
        //이 경우에 부모 게임오브젝트에 있는 Canvas Group의 속성을 수정하면 하위에 있는 모든 UI항목에 그 설정값이 적용된다.
        var canvasGroup = GameObject.Find("Panel - Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }
}
