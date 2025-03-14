using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextOutLine : MonoBehaviour
{
    [Header(" 0 : 글자 색 1 : 외곽선 색")]
    TextMeshProUGUI text;
    [SerializeField] bool drawTop, useTextColor = false;
    [SerializeField] float size = 0.35f;
    [SerializeField] Color[] color = new Color[2] { Color.black, Color.white };

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        Set();
    }
    [ContextMenu("글자 외곽선 적용")]
    public void Set()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.outlineWidth = size;
        if (!useTextColor) text.color = color[0];
        text.outlineColor = color[1];
        text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, size);

        if (drawTop) text.fontMaterial.renderQueue = 3001;
    }
}
