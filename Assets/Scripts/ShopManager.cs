using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ShopManager : MonoBehaviour
{

    // 상점 이동 속도
    [SerializeField] private float moveSpeed = 2500f;
    
    [SerializeField] private GameObject ShopScreen; // 상점 스크린 오브젝트
    [SerializeField] private GameObject ScrollView; // 상점 스크롤뷰 오브젝트

    // RectTransform을 캐시해 두면 매 프레임 GetComponent 호출을 피할 수 있음.
    // 뭔지 모르겠는데 transform.localPosition보다 이거 쓰는게 좋대
    private RectTransform shopScreenRect;   // 상점 스크린 RectTransform
    private RectTransform scrollViewRect;   // 상점 스크롤뷰 RectTransform
    [SerializeField] private RectTransform[] buttons; // 카테고리 버튼 6개

    // 상점 열림 상태 체크
    private bool ShopOpen = false;

    // 상점 스크린, 스크롤뷰 열기/닫기 목표 Y값
    private float ScreenNScrollOpenY = -117f;
    private float ScreenNScrollCloseY = -670f;

    // 상점 버튼 열기/닫기 목표 Y값
    private float ButtonOpenY = -365;
    private float ButtonCloseY = -918f;

    // 상점 열고 닫기 버튼 이미지
    [SerializeField] private Sprite[] ShopButtonSprites;    // 버튼의 스프라이트 배열 (닫힘, 열림)
    [SerializeField] private Image ShopButtonImage;         // 버튼의 Image 컴포넌트

    // 현재 실행 중인 코루틴 저장 (중복 실행 방지용)
    private Coroutine moveCoroutine;

    // 카테고리 버튼 관련
    [SerializeField]  public Image[] ButtonsImage;  // 카테고리 버튼 이미지 배열
    private int currentButtonIndex = 0; // 현재 선택된 버튼 인덱스


    private void Start()
    {
        // UI 오브젝트는 RectTransform을 가짐 -> 이걸 가져와 캐시
        shopScreenRect = ShopScreen.GetComponent<RectTransform>();
        scrollViewRect = ScrollView.GetComponent<RectTransform>();

        // 초기 버튼 이미지 설정 (닫힘 상태)
        ShopButtonImage.sprite = ShopButtonSprites[0];
    }

    // 상점 버튼에 클릭 시
    public void OnClickShopButton()
    {
        // 상점 열림 상태 반전
        ShopOpen = !ShopOpen;

        // 코루틴 중복 방지
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);

        // 버튼 이미지 변경
        ShopButtonImage.sprite = ShopOpen ? ShopButtonSprites[1] : ShopButtonSprites[0];

        // 목표 Y값 설정
        float ScreenNScrollTargetY = ShopOpen ? ScreenNScrollOpenY : ScreenNScrollCloseY;
        float ButtonTargetY = ShopOpen ? ButtonOpenY : ButtonCloseY;

        // 코루틴 시작, 저장, (현재x, 목표y) 값 전달
        moveCoroutine = StartCoroutine(MoveShopScreen(ScreenNScrollTargetY, ButtonTargetY));
    }

    // 카테고리 버튼 클릭 시
    public void OnClickCategoryButton(int index)
    {
        // 선택되어있던 버튼 색상 원래대로
        ButtonsImage[currentButtonIndex].color = new Color32(196, 196, 196, 110);   // 회색
        // 현재 선택된 카테고리 번호 저장
        currentButtonIndex = index;              
        // 선택된 버튼 색상 변경
        ButtonsImage[currentButtonIndex].color = new Color32(255, 255, 255, 255); // 흰색

        // ShopCategoryManager 스크립트로 스크롤뷰 내용 변경 요청 (츄가할 예정)

    }


    // 상점 스크린을 목표 위치로 이동시키는 코루틴 (현재x, 목표y) 값 받아옴
    private IEnumerator MoveShopScreen(float screenNScrollTargetY, float buttonTargetY)
    {
        bool moving = true;

        while (moving)
        {
            moving = false;

            // 1) ShopScreen 이동
            Vector2 screenCurrent = shopScreenRect.anchoredPosition;
            Vector2 screenNext = Vector2.MoveTowards(screenCurrent, new Vector2(screenCurrent.x, screenNScrollTargetY), moveSpeed * Time.deltaTime);
            shopScreenRect.anchoredPosition = screenNext;

            // 목표 위치에 거의 도달하지 않았으면 계속 이동
            // Mathf.Abs(currentY - targetY): y축만 고려
            // Distance(current, target) : 직선 거리를 반환, x, y 전체 거리 고려
            if (Mathf.Abs(shopScreenRect.anchoredPosition.y - screenNScrollTargetY) > 0.01f)
                moving = true;


            // 2) Scroll View 이동 (스크린과 동일)
            Vector2 scrollViewCurrent = scrollViewRect.anchoredPosition;
            Vector2 scrollViewNext = Vector2.MoveTowards(scrollViewCurrent, new Vector2(screenCurrent.x, screenNScrollTargetY), moveSpeed * Time.deltaTime);
            scrollViewRect.anchoredPosition = scrollViewNext;

            if (Mathf.Abs(scrollViewRect.anchoredPosition.y - screenNScrollTargetY) > 0.01f)
                moving = true;


            // 3) 버튼들 이동
            for (int i = 0; i < buttons.Length; i++)
            {
                RectTransform buttonRect = buttons[i];
                Vector2 buttonCurrent = buttonRect.anchoredPosition;
                Vector2 buttonNext = Vector2.MoveTowards(buttonCurrent, new Vector2(buttonCurrent.x, buttonTargetY), moveSpeed * Time.deltaTime);
                buttonRect.anchoredPosition = buttonNext;

                // 목표 위치에 거의 도달하지 않았으면 계속 이동
                if (Vector2.Distance(buttonRect.anchoredPosition, new Vector2(buttonCurrent.x, buttonTargetY)) > 0.01f)
                    moving = true;
            }

            yield return null;
        }

        // 마지막 보정
        shopScreenRect.anchoredPosition = new Vector2(shopScreenRect.anchoredPosition.x, screenNScrollTargetY);
        scrollViewRect.anchoredPosition = new Vector2(scrollViewRect.anchoredPosition.x, screenNScrollTargetY);
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].anchoredPosition = new Vector2(buttons[i].anchoredPosition.x, buttonTargetY);

    }


}