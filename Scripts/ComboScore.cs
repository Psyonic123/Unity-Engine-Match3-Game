using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ComboScore : MonoBehaviour
{
    Text score;
    public int combo_count = 0;


    // Start is called before the first frame update
    void Start()
    {

        score = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        score.text = combo_count.ToString();
    }
}
