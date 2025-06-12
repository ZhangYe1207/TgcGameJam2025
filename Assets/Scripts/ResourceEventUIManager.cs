using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ResourceEventUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Text titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image eventImage;
    [SerializeField] private Button[] resultButtons = new Button[3];
    [SerializeField] private Text[] resultTexts = new Text[3];
    [SerializeField] private Image[] resultImages = new Image[3];
    [SerializeField] private PromptUIManager promptManager;

    private RandomEvent currentEvent;
    private RandomEventHandler randomEventHandler;

    private void Awake()
    {
        // Hide the panel initially
        eventPanel.SetActive(false);
        
        // Add click listeners to result buttons
        for (int i = 0; i < resultButtons.Length; i++)
        {
            int buttonIndex = i; // Create a local copy for the lambda
            resultButtons[i].onClick.AddListener(() => OnResultSelected(buttonIndex));
        }
    }

    public void SetRandomEventHandler(RandomEventHandler randomEventHandler) 
    {
        this.randomEventHandler = randomEventHandler;
    }

    public void ShowEvent(RandomEvent randomEvent)
    {
        currentEvent = randomEvent;
        
        // Update UI elements
        titleText.text = randomEvent.title;
        descriptionText.text = randomEvent.description;
        if (randomEvent.eventImage != null)
        {
            eventImage.sprite = randomEvent.eventImage;
            eventImage.gameObject.SetActive(true);
        }
        else
        {
            eventImage.gameObject.SetActive(false);
        }

        // Update result buttons
        int resultCount = Mathf.Min(randomEvent.results.Length, 3);
        for (int i = 0; i < resultButtons.Length; i++)
        {
            if (i < resultCount)
            {
                resultButtons[i].gameObject.SetActive(true);
                resultTexts[i].text = randomEvent.results[i].description;
                
                // Update result image
                if (randomEvent.results[i].resultImage != null)
                {
                    resultImages[i].sprite = randomEvent.results[i].resultImage;
                    resultImages[i].gameObject.SetActive(true);
                }
                else
                {
                    resultImages[i].gameObject.SetActive(false);
                }
            }
            else
            {
                resultButtons[i].gameObject.SetActive(false);
                resultImages[i].gameObject.SetActive(false);
            }
        }

        eventPanel.SetActive(true);
    }

    private void OnResultSelected(int resultIndex)
    {
        if (currentEvent == null || resultIndex >= currentEvent.results.Length)
            return;

        EventResult selectedResult = currentEvent.results[resultIndex];
        
        // Show confirmation prompt
        promptManager.ShowConfirmCancelPrompt(
            $"Are you sure you want to choose this option?\n\n{selectedResult.description}",
            () => OnResultConfirmed(resultIndex),
            () => { } // Cancel callback is empty as we just want to keep the event panel open
        );
    }

    private void OnResultConfirmed(int resultIndex)
    {
        randomEventHandler.HandleResourceEvent(currentEvent.results[resultIndex]);
        randomEventHandler.ConfirmEvent();
        // Hide the event panel after selection
        HideEvent();
    }

    public void HideEvent()
    {
        eventPanel.SetActive(false);
        currentEvent = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
