using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeartsToggle : MonoBehaviour
{
    [SerializeField] GameObject[] hearts;
    [SerializeField] float delay = 0.2f;
    public bool _finishedAnimation = false;

    private void Start()
    {
        StartCoroutine(TurnOnHealth());
    }


    public IEnumerator TurnOnHealth()
    {
        foreach (var heart in hearts)
        {
            yield return new WaitForSeconds(delay);
            heart.SetActive(true);
        }
        _finishedAnimation = true;
    }
}
