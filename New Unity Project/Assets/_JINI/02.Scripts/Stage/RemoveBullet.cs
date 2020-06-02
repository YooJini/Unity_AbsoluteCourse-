using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //스파크 프리팹을 저장할 변수
    public GameObject sparkEffect;

    //충돌이 시작할 때 발생하는 이벤트
    private void OnCollisionEnter(Collision collision)
    {
        //충돌한 게임오브젝트의 태그값 비교
        if(collision.collider.tag=="BULLET")
        {
            //스파크 효과 함수 호출
            ShowEffect(collision);
            //충돌한 게임오브젝트 삭제
            Destroy(collision.gameObject);
        }
    }
    void ShowEffect(Collision collision)
    {
        //충돌 지점의 정보를 추출
        //두 객체가 충돌했을 때 접점은 여러개가 될 수 있어서 배열타입으로 결과값을 반환함
        //그래서 우리는 첫번째 충돌지점을 사용하기 위해 contacts[0]로 접근
        ContactPoint contact = collision.contacts[0];

        //법선 벡터가 이루는 회전각도를 추출
        //normal: 법선
        //Oncollision함수가 벽에서 호출되기 때문에
        //충돌한 상대편의 법선벡터가 반환됨
        //따라서 마이너스를 곱해야 벽 입장에서의 법선벡터가 됨
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        //스파크 효과를 생성
        GameObject spark = Instantiate(sparkEffect, contact.point, rot);

        //드럼통이 폭발하면서 처음 드럼통과 총알이 충돌한 위치에 생성됐던 스파크 효과는 
        //그자리에 있으므로 조금 어색하다.
        //따라서 스파크 효과를 생성하고 부모객체를 드럼통으로 설정하면
        //드럼통과 같이 이동하게 된다.

        //스파크 효과의 부모를 드럼통 또는 벽으로 설정
        spark.transform.SetParent(this.transform);
    }
}
