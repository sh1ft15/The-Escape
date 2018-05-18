using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public AudioClip key_sound;
    private Vector3 moveDirection = Vector3.zero;
    public GameObject gm;
    private float limit_time = 0.0f;
    private bool detected = false;
    private AudioSource audio;
    

    private void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (detected)
        {
            ReduceSpeed();
            gm.GetComponent<GameController>().PanicMode(true);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;
            gm.GetComponent<GameController>().PanicMode(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            gm.GetComponent<GameController>().GotCaught();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key"){

            gm.GetComponent<GameController>().ObtainedKey(other.gameObject);
            audio.PlayOneShot(key_sound);

        }else if (other.gameObject.tag == "Exit"){

            gm.GetComponent<GameController>().HasEscaped();

        }else if (other.gameObject.tag == "Diamond"){


            gm.GetComponent<GameController>().PickedDiamond(other.gameObject);
          
        }
    }

    public void ReduceSpeed()
    {
        this.speed = 1;
        this.limit_time += 1;

        if(limit_time >= 80.0f)
        {
            this.limit_time = 0;
            this.speed = 6.0F;
            this.detected = false;
        }

    }

    public void SetDetected()
    {
        this.detected = true;
        gameObject.GetComponent<Renderer>().material.color = Color.green;
    }
}
