using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fps : MonoBehaviour
{
    private Text _text;

    private void Awake() => _text = GetComponent<Text>();

    private void Update() => _text.text = $"{1f / Time.deltaTime}";
}
