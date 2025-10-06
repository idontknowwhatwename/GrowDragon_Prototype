using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ShopManager : MonoBehaviour
{



    // ���� �̵� �ӵ�
    [SerializeField] private float moveSpeed = 2500f;

    [SerializeField] private GameObject ShopScreen; // ���� ��ũ�� ������Ʈ
    [SerializeField] private GameObject ScrollView; // ���� ��ũ�Ѻ� ������Ʈ

    // RectTransform�� ĳ���� �θ� �� ������ GetComponent ȣ���� ���� �� ����.
    // ���� �𸣰ڴµ� transform.localPosition���� �̰� ���°� ����
    private RectTransform shopScreenRect;   // ���� ��ũ�� RectTransform
    private RectTransform scrollViewRect;   // ���� ��ũ�Ѻ� RectTransform
    [SerializeField] private RectTransform[] buttons; // ī�װ� ��ư 6��

    // ���� ���� ���� üũ
    private bool ShopOpen = false;

    // ���� ��ũ��, ��ũ�Ѻ� ����/�ݱ� ��ǥ Y��
    private float ScreenNScrollOpenY = -117f;
    private float ScreenNScrollCloseY = -670f;

    // ���� ��ư ����/�ݱ� ��ǥ Y��
    private float ButtonOpenY = -365;
    private float ButtonCloseY = -918f;

    // ���� ���� �ݱ� ��ư �̹���
    [SerializeField] private Sprite[] ShopButtonSprites;    // ��ư�� ��������Ʈ �迭 (����, ����)
    [SerializeField] private Image ShopButtonImage;         // ��ư�� Image ������Ʈ

    // ���� ���� ���� �ڷ�ƾ ���� (�ߺ� ���� ������)
    private Coroutine moveCoroutine;

    // ī�װ� ��ư ����
    [SerializeField]  public Image[] ButtonsImage;  // ī�װ� ��ư �̹��� �迭
    private int currentButtonIndex = 0; // ���� ���õ� ��ư �ε���


    private void Start()
    {
        // UI ������Ʈ�� RectTransform�� ���� -> �̰� ������ ĳ��
        shopScreenRect = ShopScreen.GetComponent<RectTransform>();
        scrollViewRect = ScrollView.GetComponent<RectTransform>();

        // �ʱ� ��ư �̹��� ���� (���� ����)
        ShopButtonImage.sprite = ShopButtonSprites[0];
    }

    // ���� ��ư�� Ŭ�� ��
    public void OnClickShopButton()
    {
        // ���� ���� ���� ����
        ShopOpen = !ShopOpen;

        // �ڷ�ƾ �ߺ� ����
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        // ��ư �̹��� ����
        ShopButtonImage.sprite = ShopOpen ? ShopButtonSprites[1] : ShopButtonSprites[0];

        // ��ǥ Y�� ����
        float ScreenNScrollTargetY = ShopOpen ? ScreenNScrollOpenY : ScreenNScrollCloseY;
        float ButtonTargetY = ShopOpen ? ButtonOpenY : ButtonCloseY;

        // �ڷ�ƾ ����, ����, (����x, ��ǥy) �� ����
        moveCoroutine = StartCoroutine(MoveShopScreen(ScreenNScrollTargetY, ButtonTargetY));
    }

    // ī�װ� ��ư Ŭ�� ��
    public void OnClickCategoryButton(int index)
    {
        // ���õǾ��ִ� ��ư ���� �������
        ButtonsImage[currentButtonIndex].color = new Color32(196, 196, 196, 110);   // ȸ��
        // ���� ���õ� ī�װ� ��ȣ ����
        currentButtonIndex = index;              
        // ���õ� ��ư ���� ����
        ButtonsImage[currentButtonIndex].color = new Color32(255, 255, 255, 255); // ���

        // ShopCategoryManager ��ũ��Ʈ�� ��ũ�Ѻ� ���� ���� ��û (���� ����)
        ShopCategoryManager.instance.ShowCategory(index);

    }


    // ���� ��ũ���� ��ǥ ��ġ�� �̵���Ű�� �ڷ�ƾ (����x, ��ǥy) �� �޾ƿ�
    private IEnumerator MoveShopScreen(float screenNScrollTargetY, float buttonTargetY)
    {
        bool moving = true;

        while (moving)
        {
            moving = false;

            // 1) ShopScreen �̵�
            Vector2 screenCurrent = shopScreenRect.anchoredPosition;
            Vector2 screenNext = Vector2.MoveTowards(screenCurrent, new Vector2(screenCurrent.x, screenNScrollTargetY), moveSpeed * Time.deltaTime);
            shopScreenRect.anchoredPosition = screenNext;

            // ��ǥ ��ġ�� ���� �������� �ʾ����� ��� �̵�
            // Mathf.Abs(currentY - targetY): y�ุ ���
            // Distance(current, target) : ���� �Ÿ��� ��ȯ, x, y ��ü �Ÿ� ���
            if (Mathf.Abs(shopScreenRect.anchoredPosition.y - screenNScrollTargetY) > 0.01f)
                moving = true;


            // 2) Scroll View �̵� (��ũ���� ����)
            Vector2 scrollViewCurrent = scrollViewRect.anchoredPosition;
            Vector2 scrollViewNext = Vector2.MoveTowards(scrollViewCurrent, new Vector2(scrollViewCurrent.x, screenNScrollTargetY), moveSpeed * Time.deltaTime);
            scrollViewRect.anchoredPosition = scrollViewNext;

            if (Mathf.Abs(scrollViewRect.anchoredPosition.y - screenNScrollTargetY) > 0.01f)
                moving = true;


            // 3) ��ư�� �̵�
            foreach (var b in buttons)
            {
                RectTransform buttonRect = b;
                Vector2 buttonCurrent = buttonRect.anchoredPosition;
                Vector2 buttonNext = Vector2.MoveTowards(buttonCurrent, new Vector2(buttonCurrent.x, buttonTargetY), moveSpeed * Time.deltaTime);
                buttonRect.anchoredPosition = buttonNext;

                // ��ǥ ��ġ�� ���� �������� �ʾ����� ��� �̵�
                if (Mathf.Abs(buttonRect.anchoredPosition.y - buttonTargetY) > 0.01f)
                    moving = true;
            }

            yield return null;
        }

        // ������ ����
        shopScreenRect.anchoredPosition = new Vector2(shopScreenRect.anchoredPosition.x, screenNScrollTargetY);
        scrollViewRect.anchoredPosition = new Vector2(scrollViewRect.anchoredPosition.x, screenNScrollTargetY);
        foreach (var b in buttons)
            b.anchoredPosition = new Vector2(b.anchoredPosition.x, buttonTargetY);

    }


}