using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Bar : MonoBehaviour
{

    [SerializeField] private Slider _slider;

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthBar(float CuerrentHealth, float MaxHealth)
    {
        _slider.value = CuerrentHealth / MaxHealth;
    }
}
