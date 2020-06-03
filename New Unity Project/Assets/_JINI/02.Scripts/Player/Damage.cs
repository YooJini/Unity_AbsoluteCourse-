
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private const string bulletTag = "BULLET";
    private const string enemyTag = "ENEMY";

    private float initHp = 100.0f;
    public float currHp;

    //BloodScreen 텍스처를 저장하기 위한 변수
    public Image bloodScreen;

    //Hp바 이미지를 저장하기 위한 변수
    public Image hpBar;

    //생명 게이지의 처음 색상(녹색)
    private readonly Color initColor = new Vector4(0, 1.0f, 0.0f, 1.0f);
    private Color currColor;

    //델리게이트 및 이벤트 선언
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // Start is called before the first frame update
    void Start()
    {
        currHp = initHp;

        //생명 게이지의 초기 색상 설정
        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        //충돌한 Collider의 태그가 BULLET이면 Player의 currHp 차감
        if(other.tag==bulletTag)
        {
            Destroy(other.gameObject);

            //혈흔 효과를 표현할 코루틴 함수 호출
            StartCoroutine(ShowBloodScreen());

            currHp -= 5.0f;
            Debug.Log("Player HP= " + currHp.ToString());

            //생명 게이지의 색상 및 크기 변경 함수 호출
            DisplayHpbar();

            //Player의 생명이 0이하이면 사망처리
            if(currHp<=0.0f)
            {
                PlayerDie();
            }
        }
    }

    private void DisplayHpbar()
    {
        //생명 수치가 50%일 때까지는 녹색에서 노란색으로 변경
        if (currHp / initHp > 0.5f)
            currColor.r = (1 - (currHp / initHp)) * 2.0f;
        //생명 수치가 0%일 때까지는 노란색에서 빨간색으로 변경
        else
            currColor.g = (currHp / initHp) * 2.0f;

        //Hp바의 색상 변경
        hpBar.color = currColor;
        //Hp바의 크기 변경
        hpBar.fillAmount = (currHp / initHp);
    }

    IEnumerator ShowBloodScreen()
    {
        //BloodScreen 텍스처의 알파값을 불규칙하게 변경
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //BloodScreen 텍스처의 색상을 모두 0으로 변경
        bloodScreen.color = Color.clear;
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
