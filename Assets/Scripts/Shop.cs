using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;
using System.Collections;



public class Shop : MonoBehaviour
{

    private float moveSpeed = 2500f;

    [SerializeField]
    public GameObject ShopScreen;


    public void OnClickShopButton()
    {
        StartCoroutine(MoveShopScreenUp());

        Debug.Log("Shop button clicked!");

    }

    private IEnumerator MoveShopScreenUp()
    {
        while (ShopScreen.transform.position.y < 130)
        {
            ShopScreen.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }
    }
}
