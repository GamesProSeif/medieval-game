using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class PlayerController : MonoBehaviour
{

    Vector3 PlayerMovementInput;

    private Rigidbody Playerbody;
    private GameObject ground;
    private Collider playerCollider;
    public TextMeshProUGUI text;
    public AudioSource playeraudio;

    private float speed = 5f;
    private float JumpForce = 10f;
    private bool isGrounded = false;


    /// <summary>
    /// stats
    /// </summary>
    /// 
    [SerializeField]private int health;
    [SerializeField]private int strength;
    [SerializeField]private int Exp = 1;
    


    // Start is called before the first frame update
    void Start()
    {
        Playerbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        isGrounded = true;
        text.gameObject.SetActive(false);
        playeraudio.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        MovePlayer();
       
    }
    void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * speed;
        Playerbody.velocity = new Vector3(MoveVector.x, Playerbody.velocity.y, MoveVector.z);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Playerbody.AddForce(JumpForce * Vector3.up, ForceMode.Impulse);
            isGrounded=false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
        else if(collision.gameObject.CompareTag("Exp"))
        {
            Destroy(collision.gameObject);
            Exp++;
            text.gameObject.SetActive(true);
            playeraudio.Play();
        }
                }


}



