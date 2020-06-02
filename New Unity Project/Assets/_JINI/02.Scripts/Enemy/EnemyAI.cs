
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyAI : MonoBehaviour
{
    //적 캐릭터의 상태를 표현하기 위한 열거형 변수 정의
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    //상태를 저장할 변수
    public State state = State.PATROL;

    //주인공의 위치를 저장할 변수
    private Transform playerTr;
    //적캐릭터의 위치를 저장할 변수
    private Transform enemyTr;
    //Animator 컴포넌트를 저장할 변수
    private Animator animator;

    //공격 사정거리 
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;

    //사망여부 판단 변수
    public bool isDie = false;

    //코루틴에서 사용할 지연시간 변수
    private WaitForSeconds ws;
    //이동을 제어하는 MoveAgent 클래스를 저장할 변수
    private MoveAgent moveAgent;
    //총알 발사를 제어하는 EnemyFire 클래스를 저장할 변수
    private EnemyFire enemyFire;

    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Awake()
    {
        //주인공 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공의 Transform 컴포넌트 추출
        if (player != null) 
            playerTr = player.GetComponent<Transform>();

        //적캐릭터의 Transform 컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        //애니메이터 컴포넌트 추출
        animator = GetComponent<Animator>();
        //이동을 제어하는 MoveAgent 컴포넌트 추출
        moveAgent = GetComponent<MoveAgent>();
        //총알 발사를 제어하는 EnemyFire 클래스 추출
        enemyFire = GetComponent<EnemyFire>();

        //코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);

        //Cycle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        //Speed 값을 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));

      
    }

    private void OnEnable()
    {
        //CheckState 코루틴 함수 실행
        StartCoroutine(CheckState());
        //Action 코루틴 함수 실행
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }
    //상태에 따라 적캐릭터의 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        //적캐릭터가 사망할 때까지 무한루프
        while(!isDie)
        {
            yield return ws;
            //상태에 따라 분기 처리
            switch(state)
            {
                case State.PATROL:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    if (enemyFire.isFire == false) enemyFire.isFire = true;

                    break;
                case State.DIE:
                    isDie = true;
                    enemyFire.isFire = false;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    //사망 애니메이션의 종류를 지정
                    animator.SetInteger(hashDieIdx,Random.Range(0,3));
                    //사망 애니메이션실행
                    animator.SetTrigger(hashDie);
                    //Capsule Collider 컴포넌트 비활성화 (죽은 뒤에도 혈흔이 발생하는 현상을 막기 위해)
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;

            }
        }
    }

    //적 캐릭터의 상태를 검사하는 코루틴 함수
    IEnumerator CheckState()
    {
       //적 캐릭터가 사망하기 전까지 도는 무한루프
       while(!isDie)
        {
            //상태가 사망이면 코루틴 함수를 종료시킴
            if (state == State.DIE) yield break;

            //주인공과 적캐릭터 간의 거리 계산
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);
            //float dist = (playerTr.position - enemyTr.position).sqrMagnitude;

            //공격 사정거리 이내인 경우
            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }//추적 사정거리 이내인 경우
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
                state = State.PATROL;

            //0.3초 대기하는 동안 제어권 양보
            yield return ws;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Speed 파라미터에 이동 속도 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //모든 코루틴 함수를 종료시킴
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
