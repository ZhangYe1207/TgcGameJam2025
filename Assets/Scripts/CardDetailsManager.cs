using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardDetailsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject CardDetailsUI;
    public Image background;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public TextMeshProUGUI details;
    public Vector3 offset;

    public void Show(Card cardData, Vector3 position) {
        name.text = cardData.cardName;
        description.text = cardData.description;
        details.text = cardData.details;
        CardDetailsUI.SetActive(true);
        CardDetailsUI.transform.position = position + offset;
    }

    public void Hide() {
        CardDetailsUI.SetActive(false);
    }

}
