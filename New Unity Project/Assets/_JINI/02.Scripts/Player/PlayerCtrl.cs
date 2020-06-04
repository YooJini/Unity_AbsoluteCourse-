using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//애니메이션 클립의 개수가 적을 때는 하나씩 변수로 선언한 다음 사용해도 무방하지만
//클립의 개수가 많을 때는 클래스로 선언하면 좀더 효율적으로 관리할 수 있음
//클래스는 System.Serializable이라는 어트리뷰트를 명시해야
//인스펙터 뷰에 노출됨

[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runR;
    public AnimationClip runL;

}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    private Transform tr;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 80.0f;

    //인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    //Animation 컴포넌트를 저장하기 위한 변수
    //실행시켰을 때 컴포넌트가 연결된 것을 확인했으면 
    [HideInInspector]  //더이상 인스펙터 뷰에 노출할 필요가 없음
    public Animation anim;

    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetup;
    }

    private void UpdateSetup()
    {
        moveSpeed = GameManager.instance.gameData.speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();
        //이외에 다른방법
        //1)anim.Play(playerAnim.idle.name);
        //2)anim.Play("Idle"); -> 명칭이 변경될 경우 소스코드를 수정해야하는 위험성 존재

        //불러온 데이터값을 moveSpeed에 적용
        moveSpeed = GameManager.instance.gameData.speed;

    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        Debug.Log("h="+ h.ToString());
        Debug.Log("v="+ v.ToString());

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate
        tr.Translate(moveDir.normalized*moveSpeed*Time.deltaTime,Space.Self);

        //Vector3.up축을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        //애니메이션의 변화가 부드럽게 이뤄질 수 있게 해주는 CrossFade 블렌딩 함수 
        if (v >= 0.1f) anim.CrossFade(playerAnim.runF.name, 0.3f);
        else if (v <= -0.1f) anim.CrossFade(playerAnim.runB.name, 0.3f);
        else if (h >= 0.1f) anim.CrossFade(playerAnim.runR.name, 0.3f);
        else if (h <= -0.1f) anim.CrossFade(playerAnim.runL.name, 0.3f);
        else anim.CrossFade(playerAnim.idle.name, 0.3f);

    }
}
