using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimingGauge : MonoBehaviour
{
    public Image gaugeImage;
    public RectTransform cursorImage;
    public float cursorSpeed = 100f;

    public RectTransform successZoneImage; 

    public Button gaugeButton;
    public Image failImage;
    public Image failPanel;
    public Text successCountText;
    public Text resultCountText;

    public GameObject[] hideOnSuccess;
    public GameObject[] showOnSuccess;

    public RotateImage rotateImage;

    private float cursorRange;
    private bool movingRight = true;
    private int successCount = 0;
    private bool isRunning = true;

    private float successZoneStart;
    private float successZoneEnd;

    void Start()
    {
        gaugeButton.onClick.AddListener(OnGaugeButtonPressed);

        cursorRange = gaugeImage.rectTransform.rect.width / 2f;

        float zoneCenter = successZoneImage.anchoredPosition.x;
        float zoneWidth = successZoneImage.rect.width;

        successZoneStart = zoneCenter - zoneWidth / 2f;
        successZoneEnd   = zoneCenter + zoneWidth / 2f;

        StartCoroutine(MoveCursor());
        UpdateSuccessCount();
    }

    IEnumerator MoveCursor()
    {
        while (true)
        {
            if (!isRunning)
            {
                yield return null;
                continue;
            }

            float move = cursorSpeed * Time.deltaTime * (movingRight ? 1 : -1);
            cursorImage.anchoredPosition += new Vector2(move, 0);

            if (cursorImage.anchoredPosition.x >= cursorRange)
                movingRight = false;
            else if (cursorImage.anchoredPosition.x <= -cursorRange)
                movingRight = true;

            yield return null;
        }
    }

    void OnGaugeButtonPressed()
    {
        if (!isRunning) return;

        float pos = cursorImage.anchoredPosition.x;

        if (pos >= successZoneStart && pos <= successZoneEnd)
        {
            HandleSuccess();
        }
        else
        {
            HandleFailure();
        }
    }

    void HandleSuccess()
    {
        successCount++;
        cursorSpeed += 50f;
        rotateImage.rotationSpeed += -2f;

        UpdateSuccessCount();

        foreach (GameObject obj in hideOnSuccess) obj.SetActive(false);
        foreach (GameObject obj in showOnSuccess) obj.SetActive(true);

        StartCoroutine(ResetAfterDelay());
    }

    void HandleFailure()
    {
        isRunning = false;

        failImage?.gameObject.SetActive(true);
        failPanel?.gameObject.SetActive(true);
        resultCountText.text = $"{successCount}回アタックした";
        resultCountText.gameObject.SetActive(true);

        StartCoroutine(ReturnToTitleAfterDelay());
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        foreach (GameObject obj in hideOnSuccess) obj.SetActive(true);
        foreach (GameObject obj in showOnSuccess) obj.SetActive(false);

        isRunning = true;
    }

    IEnumerator ReturnToTitleAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("titleScene");
    }

    void UpdateSuccessCount()
    {
        successCountText.text = $"{successCount}";
    }
}
