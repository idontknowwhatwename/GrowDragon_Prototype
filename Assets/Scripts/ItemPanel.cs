using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemPanel : MonoBehaviour
{
    public Image itemImage;           // 상품 이미지
    public TMP_Text itemNameText;     // 상품 이름
    public TMP_Text itemPriceText;    // 상품 가격
    public TMP_Text itemDescText;     // 상품 설명
    public Button buyButton;          // 구매 버튼

    private int itemIndex;            // 매니저 전달용 인덱스
    private ShopCategoryManager manager; // 클릭 이벤트 전달용 매니저 참조

    public void Setup(ShopCategoryManager mgr, int index, ShopCategoryManager.ItemData item)
    {
        manager = mgr;                // 매니저 저장
        itemIndex = index;            // 아이템 인덱스 저장

        itemImage.sprite = item.image;
        itemNameText.text = item.name;
        itemPriceText.text = $"{item.CurrentPrice} G";
        itemDescText.text = item.CurrentDescription;

        // 클릭 이벤트 초기화 후 등록
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);
    }

    // 구매 버튼 클릭 시 매니저에게 알림 전달
    void OnClickBuy()
    {
        manager.OnBuyButtonClicked(itemIndex);
    }
}
