using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody2D[] rb;
    public float _moveForce = 2;
    public float _spinForce = 2;
    private float _waitTimeBeforeDestroy;
    [SerializeField] private float _maxWaitTime;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _enemyDyingAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        rb = gameObject.GetComponentsInChildren<Rigidbody2D>();
        _audioSource.PlayOneShot(_enemyDyingAudioClip, 0.6f);
        MoveParts();
    }


    // Update is called once per frame
    void Update()
    {
        WaitUntilDestroy();
        
    }

    void MoveParts()
    {
        for (int i = 0; i < rb.Length; i++)
        {
            rb[i].AddForce(new Vector2(Random.Range(-1,1), 1) * _moveForce , ForceMode2D.Impulse);
            rb[i].AddTorque(Random.Range(-1,1) * _spinForce , ForceMode2D.Impulse);
        }
    }
    void WaitUntilDestroy()
    {
        //after a while the bullet is destroyed 
        _waitTimeBeforeDestroy += Time.deltaTime;
        if (_waitTimeBeforeDestroy > _maxWaitTime)
        {
            Destroy(gameObject);
        }

    }


}
