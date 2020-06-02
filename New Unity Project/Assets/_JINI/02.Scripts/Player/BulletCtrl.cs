using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력\
    public float damage = 20.0f;
    //총알 발사 속도
    public float speed = 1000.0f;
    // Start is called before the first frame update
    void Start()
    {
        //해당 게임오브젝트의 로컬 좌표를 기준으로 힘을 주기 위해 AddForce() 인자로 transform.forward를 넣거나
        //AddRelativeForce() 사용, 인자로 Vector3.forward 넣기
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
