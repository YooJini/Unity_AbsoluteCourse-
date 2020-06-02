using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float initHp = 100.0f;
    public float currHp;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // Start is called before the first frame update
    void Start()
    {
        currHp = initHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        //충돌한 Collider의 태그가 BULLET이면 Player의 currHp 차감
        if(other.tag==bulletTag)
        {
            Destroy(other.gameObject);

            currHp -= 5.0f;
            Debug.Log("Player HP= " + currHp.ToString());

            //Player의 생명이 0이하이면 사망처리
            if(currHp<=0.0f)
            {
                PlayerDie();
            }
        }
    }

    //Player 사망처리 루틴
    private void PlayerDie()
    {
        //아래 주석처리된 로직으로 구현하면 적캐릭터의 수가 늘어날 경우 비효율적이다.
        //따라서 델리게이트 이용
        OnPlayerDie();

       //Debug.Log("Player Die!");
       //
       ////ENEMY 태그로 지정된 모든 적캐릭터 추출, 배열에 저장
       //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
       //
       ////배열의 처음부터 순회하면서 적 캐릭터의 OnPlayerDie 함수 호출
       ////SendMessage: 특정 게임오브젝트에 포함된 스크립트를 하나씩 검색해 호출하려는 함수가 있으면 실행하라고 메시지 전달
       ////만약 호출한 함수가 해당 게임오브젝트에 포함된 스크립트에 없다면 오류 메시지 반환
       ////SendMessageOptions: 위의 오류 메시지를 리턴 받을 것인지 여부를 결정함 
       //for(int i=0;i<enemies.Length;i++)
       //{
       //    enemies[i].SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
       //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
