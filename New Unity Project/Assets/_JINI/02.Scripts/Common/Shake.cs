﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    //쉐이크 효과를 줄 카메라의 Transform을 저장할 변수
    public Transform shakeCamera;
    //회전시킬 것인지를 판단할 변수
    public bool shakeRotate = false;
    //초기 좌표와 회전값을 저장할 변수
    private Vector3 originPos;
    private Quaternion originRot;

    // Start is called before the first frame update
    void Start()
    {
        //카메라의 초기값을 저장
        originPos = shakeCamera.localPosition;
        originRot = shakeCamera.localRotation;
    }

    public IEnumerator ShakeCamera(float duration=0.05f, float magnitudePos=0.03f, float magnitudeRot=0.1f)
    {
        //지나간 시간을 누적할 변수
        float passTime = 0.0f;

        //진동 시간 동안 루프를 순회함
        while(passTime<duration)
        {
            //불규칙한 위치 산출
            //반경이 1인 구체 내부의 3차원 좌표값을 불규칙하게 반환
            Vector3 shakePos = Random.insideUnitSphere;
            //카메라의 위치 변경
            shakeCamera.localPosition = shakePos * magnitudePos;

            //불규칙한 회전을 사용할 경우
            if(shakeRotate)
            {
                //불규칙한 회전값을 펄린 노이즈 함수를 이용해 추출
                //0과 1사이의 난수를 발생시키지만 일반 난수 발생기와는 다르게 연속성이 있는 난수를 발생시킨다.
                //무작위한 터레인 또는 텍스처, 구름등을 생성할 때 유용하게 사용된다
                Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                //카메라의 회전값을 변경
                shakeCamera.localRotation = Quaternion.Euler(shakeRot);
            }
            //진동 시간을 누적
            passTime += Time.deltaTime;

            yield return null;
        }

        //진동이 끝난 후 카메라의 초기값으로 설정
        shakeCamera.localPosition = originPos;
        shakeCamera.localRotation = originRot;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
