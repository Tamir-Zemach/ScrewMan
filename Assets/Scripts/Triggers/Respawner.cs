
using UnityEngine;

public class Respawner : MonoBehaviour
{
    private GameObject _player;
    private PlayerStateChecker PlayerStateChecker; 
    private PlayerMovement PlayerMovement;
    private Rigidbody2D _playerRB;
    public Transform _respawnPos;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        PlayerMovement = _player.gameObject.GetComponent<PlayerMovement>();
        PlayerStateChecker = _player.gameObject.GetComponent<PlayerStateChecker>();
      
        _playerRB = _player.GetComponent<Rigidbody2D>();
        _respawnPos = transform.Find("Respawn_Point");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _player.transform.position = _respawnPos.transform.position;
             PlayerStateChecker.isDashingDown = false;
             PlayerMovement.ResetVelocity();
        }
    }
}
