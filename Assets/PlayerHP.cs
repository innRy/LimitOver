using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private int hp;

    [SerializeField] private TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        hpText.text = hp.ToString();
    }

    // Update is called once per frame
    
    public void TakeDamage(int damage)
    {
        hp -= damage;

        hpText.text= hp.ToString();

        if (hp<= 0)
        {
            hp=0;

            hpText.text = hp.ToString();

            Destroy(gameObject);
        }
    }
}
