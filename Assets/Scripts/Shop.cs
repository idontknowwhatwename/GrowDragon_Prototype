using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
public class Shop : MonoBehaviour
{

    // 상점 이동 속도 (인스펙터에서 조정 가능)
    [SerializeField]
    private float moveSpeed = 2500f;

    // 상점 스크린 오브젝트
    [SerializeField]
    private GameObject ShopScreen;

    // RectTransform을 캐시해 두면 매 프레임 GetComponent 호출을 피할 수 있음.
    // 뭔지 모르겠는데 transform.localPosition보다 이거 쓰는게 좋대
    private RectTransform shopScreenRect;
    [SerializeField] private RectTransform[] buttons; // 이동할 버튼 5개

    // 상점 열림 상태 체크
    private bool ShopOpen = false;

    // 상점 스크린 열기/닫기 목표 Y값
    private float ScreenOpenY = -117f;
    private float ScreenCloseY = -670f;

    // 상점 스크린 열기/닫기 목표 Y값
    private float ButtonOpenY = -365;
    private float ButtonCloseY = -918f;

    // 상점 버튼 이미지
    [SerializeField] private Sprite[] ShopButtonSprites;    // 버튼의 스프라이트 배열 (닫힘, 열림)
    [SerializeField] private Image ShopButtonImage;         // 버튼의 Image 컴포넌트

    // 현재 실행 중인 코루틴 저장 (중복 실행 방지용)
    private Coroutine moveCoroutine;

    private void Start()
    {
        // UI 오브젝트는 RectTransform을 가짐 -> 이걸 가져와 캐시
        shopScreenRect = ShopScreen.GetComponent<RectTransform>();

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
        float ScreenTargetY = ShopOpen ? ScreenOpenY : ScreenCloseY;
        float ButtonTargetY = ShopOpen ? ButtonOpenY : ButtonCloseY;

        // 코루틴 시작, 저장, (현재x, 목표y) 값 전달
        moveCoroutine = StartCoroutine(MoveShopScreen(ScreenTargetY, ButtonTargetY));
    }

    // 상점 스크린을 목표 위치로 이동시키는 코루틴 (현재x, 목표y) 값 받아옴
    private IEnumerator MoveShopScreen(float screenTargetY, float buttonTargetY)
    {
        bool moving = true;

        while (moving)
        {
            moving = false;

            // 1) ShopScreen 이동
            Vector2 screenCurrent = shopScreenRect.anchoredPosition;
            Vector2 screenNext = Vector2.MoveTowards(screenCurrent, new Vector2(screenCurrent.x, screenTargetY), moveSpeed * Time.deltaTime);
            shopScreenRect.anchoredPosition = screenNext;

            // 목표 위치에 거의 도달하지 않았으면 계속 이동
            // Mathf.Abs(currentY - targetY): y축만 고려
            // Distance(current, target) : 직선 거리를 반환, x, y 전체 거리 고려
            if (Vector2.Distance(shopScreenRect.anchoredPosition, new Vector2(screenCurrent.x, screenTargetY)) > 0.01f)
                moving = true;

            // 2) 버튼들 이동
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
        shopScreenRect.anchoredPosition = new Vector2(shopScreenRect.anchoredPosition.x, screenTargetY);
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].anchoredPosition = new Vector2(buttons[i].anchoredPosition.x, buttonTargetY);

    }

}