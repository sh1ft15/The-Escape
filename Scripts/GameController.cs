using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController instance = null;
    public GameObject player, camera, entrance, exit, light;
    public Text time_tag, game_over, life_tag, level_tag;
    public Button restart_button;
    public GameObject diamond;
    public GameObject[] playerSpawns;
    private GameObject currentPoint;
    public GameObject[] exitSpawns;
    private GameObject exitPoint;
    private GameObject pickedKey;
    public GameObject[] patterns;
    private GameObject currentPattern;
    private Vector3 playerToCamera;
    public static float timer, death_time, time_before_respawn;
    private string niceTime;
    private int minutes, seconds, life, level;
    private AudioSource audio;
    private Color origin_light;
    private bool is_dead, got_caught, has_escaped;
    private float increment, score;

    void Awake()
    {

        if (instance == null)
        {

            instance = this;
        }
        else if (instance != this)
        {


            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }


    void Start()
    {
        is_dead = false;
        life = 3;
        level = 1;
        increment = 0;
        audio = gameObject.GetComponent<AudioSource>();
        origin_light = light.GetComponent<Light>().color;
        RespawnPlayer();
        if (currentPattern)
        {
            currentPattern.SetActive(false);
        }

        currentPattern = patterns[level - 1];
        currentPattern.SetActive(true);
        level_tag.text = "Level " + level;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(life >= 0)
        {
            if (timer <= 0.0f)
            {
                this.IsDead("Time's Up!");

            }
            else if (got_caught)
            {
                timer += Time.deltaTime;
                this.IsDead("You have been caught!");
            }
            else if (has_escaped)
            {
                timer += Time.deltaTime;
                this.IsWin("You have escaped!");
            }
            else
            {
                timer -= Time.deltaTime;
                DisplayGUI();

            }
            
        }
        else
        {
            if (game_over.gameObject.activeSelf == false)
            {
                game_over.fontSize = 50;
                game_over.text = "Game Over !";
                game_over.gameObject.SetActive(true);
                restart_button.gameObject.SetActive(true);
            }

            if(player.gameObject.activeSelf == true)
            {
                player.SetActive(false);
            }
        }


    }

    void DisplayGUI()
    {
        minutes = Mathf.FloorToInt(timer / 60F);
        seconds = Mathf.FloorToInt(timer - minutes * 60);
        niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        time_tag.text = "Time : " + niceTime;

        if (timer <= 10.0f)
        {
            time_tag.color = new Color(time_tag.color.r, time_tag.color.g, time_tag.color.b, Mathf.Sin(Time.time * -8));
        }
        else
        {
            if(increment > 0)
            {
                time_tag.color = Color.green;
                increment -= 0.05f;
            }
            else
            {
                increment = 0;
                time_tag.color = Color.black;
            }
            
        }

        life_tag.text = "Life : " + life;
    }

    public void RespawnPlayer()
    {
        if (is_dead){
            this.life -= 1;
            this.is_dead = false;
            this.got_caught = false;
        }

        if (has_escaped){
            this.has_escaped = false;
        }

        timer = 15.0f;
        death_time = 0.0f;
        light.GetComponent<Light>().color = origin_light;
        game_over.gameObject.SetActive(false);
        restart_button.gameObject.SetActive(false);

        int index = Random.Range(0, playerSpawns.Length);
        currentPoint = playerSpawns[index];

        player.gameObject.SetActive(true);
        playerToCamera = camera.transform.position - player.transform.position;
        player.transform.position = currentPoint.transform.position;

        camera.transform.position = player.transform.position + playerToCamera;
        camera.GetComponent<CameraController>().SetOffset(playerToCamera);

        Vector3 pos = entrance.transform.position;
        pos.x = currentPoint.transform.position.x;
        entrance.transform.position = pos;

        exit.gameObject.SetActive(false);

        if (pickedKey){
            pickedKey.gameObject.SetActive(true);
        }
        
    }

    public void ObtainedKey(GameObject key)
    {
        exit.gameObject.SetActive(true);
        int index = Random.Range(0, exitSpawns.Length);
        exitPoint = exitSpawns[index];
        Vector3 pos = exit.transform.position;
        pos.z = exitPoint.transform.position.z;
        exit.transform.position = pos;

        key.gameObject.SetActive(false);
        this.pickedKey = key;
    }

    public void PanicMode(bool panic)
    {
        if (panic)
        {
            audio.pitch = 2.5f;
        }
        else
        {
            audio.pitch = 1.0f;
        }
        
    }

    public void IsDead(string text)
    {
        is_dead = true;

        if (player.gameObject.activeSelf == true)
        {
            player.gameObject.SetActive(false);
            light.GetComponent<Light>().color = Color.red;
        }

        if (game_over.gameObject.activeSelf == false)
        {
            game_over.fontSize = 30;
            game_over.gameObject.SetActive(true);
        }

        game_over.text = text + " Respawning in " + Mathf.Round(death_time);
        death_time += Time.deltaTime;

        if (death_time >= 3.0f)
        {
            this.RespawnPlayer();
        }

    }

    public void IsWin(string text)
    {
        if(level < 3)
        {
            level_tag.text = "Level " + level;
        }
        
        if (player.gameObject.activeSelf == true)
        {
            player.gameObject.SetActive(false);
            light.GetComponent<Light>().color = Color.green;
        }

        if (game_over.gameObject.activeSelf == false)
        {
            game_over.fontSize = 30;
            game_over.gameObject.SetActive(true);
        }

        game_over.text = "~" + text + "~\nBest Score : " + score + "\nRespawning in " + Mathf.Round(death_time);
        death_time += Time.deltaTime;

        if (death_time >= 3.0f)
        {
            
            if (level < 3)
            {
                this.currentPattern.SetActive(false);
                this.currentPattern = patterns[level - 1];
                this.currentPattern.SetActive(true);
                this.RespawnPlayer();
            }
            else
            {
                light.GetComponent<Light>().color = origin_light;
                this.game_over.fontSize = 50;
                this.game_over.text = "You Win!!";
                this.game_over.gameObject.SetActive(true);
                this.restart_button.gameObject.SetActive(true);
            }
            
        }

    }

    public void GotCaught()
    {
        this.got_caught = true;
    }


    public void RestartScene()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true); // this call onEnable()
    }

    public void OnEnable()
    {
        this.Start();
    }

    public void HasEscaped()
    {
        has_escaped = true;
        level += 1;

        score = Mathf.Ceil(timer * 1.0f);

    }

    public void PickedDiamond(GameObject diamond)
    {
        increment = diamond.GetComponent<DiamondController>().getIncrement();
        timer += increment;
        Destroy(diamond.gameObject);
    }
}
