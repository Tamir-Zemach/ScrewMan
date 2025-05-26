using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Anim : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }
     public void DestoryAfterAnim()
    {
        Destroy(gameObject);
    }

}
