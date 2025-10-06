using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ItemPanel : MonoBehaviour
{
    public Image itemImage;           // ��ǰ �̹���
    public TMP_Text itemNameText;     // ��ǰ �̸�
    public TMP_Text itemPriceText;    // ��ǰ ����
    public TMP_Text itemDescText;     // ��ǰ ����
    public Button buyButton;          // ���� ��ư

    private int itemIndex;            // �Ŵ��� ���޿� �ε���
    private ShopCategoryManager manager; // Ŭ�� �̺�Ʈ ���޿� �Ŵ��� ����

    public void Setup(ShopCategoryManager mgr, int index, ShopCategoryManager.ItemData item)
    {
        manager = mgr;                // �Ŵ��� ����
        itemIndex = index;            // ������ �ε��� ����

        itemImage.sprite = item.image;
        itemNameText.text = item.name;
        itemPriceText.text = $"{item.CurrentPrice} G";
        itemDescText.text = item.CurrentDescription;

        // Ŭ�� �̺�Ʈ �ʱ�ȭ �� ���
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnClickBuy);
    }

    // ���� ��ư Ŭ�� �� �Ŵ������� �˸� ����
    void OnClickBuy()
    {
        manager.OnBuyButtonClicked(itemIndex);
    }
}
