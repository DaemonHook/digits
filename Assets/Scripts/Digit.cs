using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Digit : MonoBehaviour
{
    private TextMeshPro text;
    private SpriteRenderer sr;
    
    public static string[] ColorStrings =
    {
        "#444693",
        "#444693",
        "#D71345",
        "#005831",
        "#F58220",
        "#7BBFEA",
        "#EA66A6",
        "#7C8577",
        "#bed742",
        "#008792",
        "#deab8a",
        "#367459",
        "#70A19F",
        "#2E3A1F",
        "#d1c7b7",
        "#b3424a",
        "#b3424a",
    };

    public void Init(int number, Vector3 scale)
    {
        text.text = number.ToString();
        transform.localScale = scale;
        Color c = Color.black;
        ColorUtility.TryParseHtmlString(ColorStrings[number], out c);
        sr.color = c;
    }

    private void Awake()
    {
        text = transform.Find("Text (TMP)").GetComponent<TextMeshPro>();
        sr = GetComponent<SpriteRenderer>();
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
