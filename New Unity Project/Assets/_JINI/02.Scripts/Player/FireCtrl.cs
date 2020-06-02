using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        //FirePos하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        //AudioSource 컴포넌트 추출
        _audio = GetComponent<AudioSource>();

        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼을 클릭했을 때 Fire함수 호출
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
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
    }

    void FireSfx()
    {
        //현재 들고있는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        //사운드 발생
        _audio.PlayOneShot(_sfx, 1.0f);
    }
}
