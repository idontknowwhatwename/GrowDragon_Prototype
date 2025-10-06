using System.Collections.Generic;
using UnityEngine;


public class ShopCategoryManager : MonoBehaviour
{
    public static ShopCategoryManager instance = null;

    private void Awake()
    {
        // 이미 인스턴스가 있으면 자신을 파괴 (중복 방지)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // ScrollView Content
    public Transform contentParent;

    // 패널 프리팹
    public GameObject itemPanelPrefab;

    // 카테고리별 상품 데이터
    public List<ItemData> accessories;
    public List<ItemData> foodAndMedicine;
    public List<ItemData> inAppPurchase;
    public List<ItemData> toys;
    public List<ItemData> adopt;
    public List<ItemData> touch;

    // 현재 선택된 카테고리
    private List<ItemData> currentCategory;

    // 생성된 패널 목록
    private List<GameObject> currentPanels = new List<GameObject>();

    // 카테고리 선택 시 목록 출력
    public void ShowCategory(int categoryIndex)
    {
        // 1) 기존 패널 제거
        // foreach : 컬렉션(리스트, 배열 등)의 각 요소를 순회
        // var : 컴파일러가 변수 타입을 자동으로 추론
        foreach (var p in currentPanels)
            Destroy(p);
        currentPanels.Clear();

        // 2) 선택된 카테고리 설정
        switch (categoryIndex)
        {
            case 0: currentCategory = accessories; break;
            case 1: currentCategory = foodAndMedicine; break;
            case 2: currentCategory = inAppPurchase; break;
            case 3: currentCategory = toys; break;
            case 4: currentCategory = adopt; break;
            case 5: currentCategory = touch; break;
            default: currentCategory = null; break;
        }

        if (currentCategory == null) return;

        // 3) 패널 생성
        for (int i = 0; i < currentCategory.Count; i++)
        {
            var item = currentCategory[i];                                // 상품 데이터
            GameObject panel = Instantiate(itemPanelPrefab, contentParent); // 패널 생성
            var ui = panel.GetComponent<ItemPanel>();                     // ItemPanel 스크립트 가져오기

            // 패널 초기화
            ui.Setup(this, i, item);
            currentPanels.Add(panel);                                     // 리스트에 추가
        }
    }

    // 구매 버튼 클릭 시
    public void OnBuyButtonClicked(int itemIndex)
    {
        if (currentCategory == null || itemIndex >= currentCategory.Count) return;

        var item = currentCategory[itemIndex];

        // 예시: 재화 확인 후 구매/업그레이드
        if (CanAfford(item.CurrentPrice))
        {
            Pay(item.CurrentPrice);   // 재화 차감
            item.level++;             // 레벨 업

            Debug.Log($"{item.name} 업그레이드 완료! 현재 레벨: {item.level}");

            // UI 갱신
            var panel = currentPanels[itemIndex].GetComponent<ItemPanel>();
            panel.Setup(this, itemIndex, item);
        }
        else
        {
            Debug.Log("재화가 부족합니다!");
        }
    }

    // 임시 재화 처리
    bool CanAfford(int price) => true;
    void Pay(int price) { /* 재화 차감 처리 */ }

    [System.Serializable]
    public class ItemData
    {
        public Sprite image;            // 상품 이미지
        public string name;             // 상품 이름
        public int basePrice;           // 기본 가격
        public string baseDescription;  // 기본 설명
        public bool canUpgrade;         // 업그레이드 가능 여부

        public int level = 1;           // 현재 레벨

        // 현재 가격 계산
        public int CurrentPrice => canUpgrade ? basePrice * level : basePrice;

        // 현재 설명 계산
        public string CurrentDescription => canUpgrade
            ? $"{baseDescription}\nLv.{level} → Lv.{level + 1}"
            : baseDescription;
    }
}
