using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour {

    private NavMeshAgent agent;
    private Vector3 origin_pos, last_seen;
    private bool detected;
    private Animation anim;
    private float action_time;
    private GameObject controller;

	// Use this for initialization
	void Start () {
        agent = gameObject.GetComponent<NavMeshAgent>();
        origin_pos = transform.position;
        last_seen = Vector3.zero;
        anim = gameObject.GetComponent<Animation>();
        detected = false;
        action_time = 0;
        controller = GameObject.FindGameObjectWithTag("GameController");
    }
	

    void Update(){
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, fwd, out hit, 14)){

            switch (hit.transform.gameObject.tag){
                case "Player":
                    last_seen = hit.transform.position;
                    detected = true;
                    hit.transform.gameObject.GetComponent<PlayerController>().SetDetected();
                    break;
                case "Wall":
                    // last_seen = origin_pos;
                    detected = false;
                    break;
            }
        }
        else
        {
            detected = false;
        }


        if (detected == true)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        // seeing something .. 
        if (last_seen != Vector3.zero)
        {   
            // reach last seen or not
            if (transform.position.x == last_seen.x)
            {
                StartScan();
                last_seen = Vector3.zero;
                action_time = 0;
            }
            else
            {
                StopScan();
                action_time += Time.deltaTime;

                if (action_time >= 1)
                {
                    agent.destination = last_seen;
                }
            }
            
        }

        // last_seen reset or on initial
        if (last_seen == Vector3.zero)
        {
           
            if (transform.position.x != origin_pos.x)
            {
                action_time += Time.deltaTime;

                if (action_time >= 10)
                {
                    StopScan();
                    agent.destination = origin_pos;
                }
            }
            else
            {
                StartScan();
                action_time = 0;
            }


        }


        // Debug.Log("Action Time : " + action_time);

    }
    

    void StartScan() {
        gameObject.GetComponent<Renderer>().material.color = Color.black;
        gameObject.GetComponent<LineRenderer>().enabled = true;

        if (anim["EnemyAnim2"])
        {
            anim["EnemyAnim2"].speed = 1;
        }
        else
        {
            anim["EnemyAnim"].speed = 1;
        }
        
    }

    void StopScan() {
        gameObject.GetComponent<LineRenderer>().enabled = false;

        if (anim["EnemyAnim2"])
        {
            anim["EnemyAnim2"].speed = 0;
        }
        else
        {
            anim["EnemyAnim"].speed = 0;
        }

    }


    //bool test = transform.position.x == last_seen.x;
    //Debug.Log(transform.position +" == "+ last_seen + " = "+ test);

    //if (transform.position.x == origin_pos.x){

    //    transform.rotation = Quaternion.Slerp(transform.rotation, origin_rot, Time.deltaTime * 4);

    //}

    // Debug.DrawLine(transform.position, hit.point, Color.green);
}
