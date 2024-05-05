using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGChangeColor : MonoBehaviour
{
    private SpriteRenderer sp;
    private Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black};

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        InvokeRepeating("X",0,20f);
    }

    private void X()
    {
        StartCoroutine(ChangeColor());
    }
    IEnumerator ChangeColor()
    {
        Color targetColor = colors[Random.Range(0, colors.Length)];
        Debug.Log("Changing color" + targetColor);

        while (true)
        {
            float t = 0f;

            while (t < 1.0f)
            {
                sp.color = Color.Lerp(sp.color, targetColor, t);
                t += Time.deltaTime/1000f;
                yield return null;
            }

            sp.color = targetColor;
        }
    }
}
