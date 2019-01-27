using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechanicsText : MonoBehaviour
{
    float delay = 0.1f;
    string fullText;
    string currenText = "";
    public Image image;

    public void SetText(string s)
    {
        fullText = s;
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length+1; i++)
        {
            currenText = fullText.Substring(0, i);
            GetComponent<Text>().text = currenText;
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(1f);
        EnableUI(false);
    }

    public void EnableUI(bool b)
    {
        image.gameObject.SetActive(b);
        gameObject.SetActive(b);
    }


}