using System.Collections.Generic;
using UnityEngine;


public class ShopCategoryManager : MonoBehaviour
{
    public static ShopCategoryManager instance = null;

    private void Awake()
    {
        // �̹� �ν��Ͻ��� ������ �ڽ��� �ı� (�ߺ� ����)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // ScrollView Content
    public Transform contentParent;

    // �г� ������
    public GameObject itemPanelPrefab;

    // ī�װ��� ��ǰ ������
    public List<ItemData> accessories;
    public List<ItemData> foodAndMedicine;
    public List<ItemData> inAppPurchase;
    public List<ItemData> toys;
    public List<ItemData> adopt;
    public List<ItemData> touch;

    // ���� ���õ� ī�װ�
    private List<ItemData> currentCategory;

    // ������ �г� ���
    private List<GameObject> currentPanels = new List<GameObject>();

    // ī�װ� ���� �� ��� ���
    public void ShowCategory(int categoryIndex)
    {
        // 1) ���� �г� ����
        // foreach : �÷���(����Ʈ, �迭 ��)�� �� ��Ҹ� ��ȸ
        // var : �����Ϸ��� ���� Ÿ���� �ڵ����� �߷�
        foreach (var p in currentPanels)
            Destroy(p);
        currentPanels.Clear();

        // 2) ���õ� ī�װ� ����
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

        // 3) �г� ����
        for (int i = 0; i < currentCategory.Count; i++)
        {
            var item = currentCategory[i];                                // ��ǰ ������
            GameObject panel = Instantiate(itemPanelPrefab, contentParent); // �г� ����
            var ui = panel.GetComponent<ItemPanel>();                     // ItemPanel ��ũ��Ʈ ��������

            // �г� �ʱ�ȭ
            ui.Setup(this, i, item);
            currentPanels.Add(panel);                                     // ����Ʈ�� �߰�
        }
    }

    // ���� ��ư Ŭ�� ��
    public void OnBuyButtonClicked(int itemIndex)
    {
        if (currentCategory == null || itemIndex >= currentCategory.Count) return;

        var item = currentCategory[itemIndex];

        // ����: ��ȭ Ȯ�� �� ����/���׷��̵�
        if (CanAfford(item.CurrentPrice))
        {
            Pay(item.CurrentPrice);   // ��ȭ ����
            item.level++;             // ���� ��

            Debug.Log($"{item.name} ���׷��̵� �Ϸ�! ���� ����: {item.level}");

            // UI ����
            var panel = currentPanels[itemIndex].GetComponent<ItemPanel>();
            panel.Setup(this, itemIndex, item);
        }
        else
        {
            Debug.Log("��ȭ�� �����մϴ�!");
        }
    }

    // �ӽ� ��ȭ ó��
    bool CanAfford(int price) => true;
    void Pay(int price) { /* ��ȭ ���� ó�� */ }

    [System.Serializable]
    public class ItemData
    {
        public Sprite image;            // ��ǰ �̹���
        public string name;             // ��ǰ �̸�
        public int basePrice;           // �⺻ ����
        public string baseDescription;  // �⺻ ����
        public bool canUpgrade;         // ���׷��̵� ���� ����

        public int level = 1;           // ���� ����

        // ���� ���� ���
        public int CurrentPrice => canUpgrade ? basePrice * level : basePrice;

        // ���� ���� ���
        public string CurrentDescription => canUpgrade
            ? $"{baseDescription}\nLv.{level} �� Lv.{level + 1}"
            : baseDescription;
    }
}
