using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//단순히 저장할 데이터의 멤버 변수만 정의한 클래스로서 
//MonoBehaviour로부터 상속받지 않는다.
//또한 다른 스크립트에서 쉽게 참조할 수 있게 DataInfo라는 네임스페이스로 지정한다.
//DataInfo 클래스에는 GameData와 Item이라는 두개의 클래스를 선언했다.
//GameData 클래스는 게임의 전반적인 정보를 저장할 클래스이고
//Item 클래스는 주인공이 취득한 아이템 정보를 저장할 클래스다.
namespace DataInfo
{
    [System.Serializable]
    public class GameData
    {
        public int killCount = 0;
        public float hp = 120.0f;
        public float damage = 25.0f;
        public float speed = 6.0f;
        public List<Item> equipItem = new List<Item>();
    }

    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP,SPEED,GRENADE,DAMAGE} //아이템 종류 선언
        public enum ItemCalc { INC_VALUE,PERCENT}       //계산 방식 선언
        public ItemType itemType;                       //아이템 종류
        public ItemCalc itemCalc;                       //아이템 적용시 계산 방식
        public string name;                             //아이템 이름
        public string desc;                             //아이템 소개
        public float value;                             //계산 값
    }
}


