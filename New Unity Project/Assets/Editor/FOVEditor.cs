﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Editor 클래스에 접근하기 위해 명시해야 하는 네임스페이스
using UnityEditor;

//어떤 스크립트(클래스)를 사용자 정의 UI로 만들 것인지에 대한 정보를
//CustomEditor 어트리뷰트를 통해 Editor 클래스에 전달
[CustomEditor(typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //EnemyFOV 클래스 참조
        EnemyFOV fov=(EnemyFOV)target;

        //원주 위의 시작점의 좌표를 계산 (주어진 각도의 1/2)
        //viewAngle: 부채꼴의 사잇각 => 시작점의 각도는 (-viewAngle/2)
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        //원의 색상을 흰색으로 지정
        Handles.color = Color.white;

        //외곽선만 표현하는 원반 그리기
        //두번째 인자는 노멀벡터다.
        //Vertor3.up은 y축을 의미하기에 x축과 z축 평면에 원을 그린다는 의미이다.
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        //부채꼴 색상 지정
        Handles.color = new Color(1, 1, 1, 0.2f);

        //채워진 부채꼴 그리기
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);

        //시야각의 텍스트 표시
        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());
       
    }    
}
