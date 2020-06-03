﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

//총알 발사와 재장전 오디오 클립을 저장할 구조체
[System.Serializable]
public struct PlayerSfx
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    //무기 타입
    public enum WeaponType
    {
        RIFLE=0,
        SHOTGUN
    }

    //주인공이 현재 들고있는 무기를 저장할 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    //총알 프리팹
    public GameObject bullet;
    //총알 발사 좌표
    public Transform firePos;
    //탄피 추출 파티클
    public ParticleSystem cartridge;
    //총구 화염 파티클
    private ParticleSystem muzzleFlash;
    //오디오소스 컴포넌트를 저장할 변수
    private AudioSource _audio;

    //오디오클립을 저장할 변수
    public PlayerSfx playerSfx;

    //쉐이크 클래스를 저장할 변수
    private Shake shake;

    //탄창 이미지 Image UI
    public Image magazineImg;
    //남은 총알 수 Text UI
    public Text magazineText;

    //최대 총알 수
    public int maxBullet = 10;
    //남은 총알 수
    public int remainingBullet = 10;

    //재장전 시간
    public float reloadTime = 2.0f;
    //재장전 여부를 판단할 변수
    private bool isReloading = false;

    // Start is called before the first frame update
    void Start()
    {
        //탄창 이미지 초기화
        magazineImg.fillAmount = 1.0f;
        //남은 총알 수 갱신
        UpdateBulletText();

        //FirePos하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        //AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();

        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    private void UpdateBulletText()
    {
        //(남은 총알 수) / (최대 총알 수) 표시
        magazineText.text = string.Format("<color=#ff0000>{0}</color>/{1}", remainingBullet, maxBullet);
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼을 클릭했을 때 Fire함수 호출
        if(!isReloading && Input.GetMouseButtonDown(0))
        {
            //총알 수 하나 감소
            --remainingBullet;
            Fire();

            //남은 총알이 없을 경우 재장전 코루틴 호출
            if (remainingBullet == 0) StartCoroutine(Reloading());
        }
    }

   IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1.0f);

        //재장전 오디오의 길이 +0.3초 동안 대기
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        //각종 변수값 초기화
        isReloading = false;
        magazineImg.fillAmount = 1.0f;
        remainingBullet = maxBullet;
        //남은총알 수 갱신
        UpdateBulletText();

    }

    void Fire()
    {
        //쉐이크 효과 호출
        StartCoroutine(shake.ShakeCamera(0.1f,0.2f,0.5f));
        //Bullet프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
        //탄피 추출 파티클 실행
        cartridge.Play();
        //총구 화염 파티클 실행
        muzzleFlash.Play();
        //사운드 발생
        FireSfx();

        //재장전 이미지의 fillAmount 속성값 지정
        magazineImg.fillAmount = (float)remainingBullet / (float)maxBullet;
        //남은 총알 수 갱신
        UpdateBulletText();
    }

    void FireSfx()
    {
        //현재 들고있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        //사운드 발생
        _audio.PlayOneShot(_sfx, 1.0f);
    }
}
