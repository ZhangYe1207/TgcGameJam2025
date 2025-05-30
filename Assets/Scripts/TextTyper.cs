using UnityEngine;
using TMPro;
using System.Collections;

public class TextTyper : MonoBehaviour
{
    public TextMeshProUGUI targetText;     // 目标文本组件
    private string fullText;     // 完整文本内容
    public float delay = 0.1f;  // 每个字符的延迟时间

    private void Start()
    {
        fullText = targetText.text;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        targetText.text = "";
        foreach (char c in fullText)
        {
            targetText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}