using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class contact : MonoBehaviour

{


    [SerializeField] private TextMeshProUGUI Hittext;
    private int Count;
    // Start is called before the first frame update
    void Start()
    {
        Count = 0;
        Hittext.text = "Hit 0";
    }

    // Update is called once per frame
    void Update()
    {
        Hittext.text = "Hit " + Count;
    }

    void OnCollisionEnter(Collision other)
    {
        string name = other.gameObject.name;

        if (name.Length >= 4 && name.Substring(0, 4) == "wall")
        {
            Count++;
        }
    }
}
