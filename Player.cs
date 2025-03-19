using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f; //SerializeField讓該變數能在編輯器調整，且只能在Player內存取
    [SerializeField] float jumpSpeed = 10f; //SerializeField讓該變數能在編輯器調整，且只能在Player內存取(彈跳速度)
    [SerializeField] int HP = 10; //設定血量初始為10
    [SerializeField] GameObject HpBar; //設定血量初始為10
    [SerializeField] Text scoreText; //層級的text
    [SerializeField] Text Game_paused; //遊戲暫停要顯示的text
    [SerializeField] GameObject replayButton; //重新開始的按鈕
    [SerializeField] Rigidbody2D rb; //控制物理行為，速度重力與力道等
    int score; //現在層數
    float scoreTime; //記錄過多久時間
    Animator anim;
    AudioSource deathSound; //裝載死亡音效的物件
    bool isPaused = false; // 追蹤遊戲是否暫停
    bool isDie = false; // 查看是否死亡
    public bool startgame = false;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("開始");
        score = 0;
        scoreTime = 0f;
        anim = GetComponent<Animator>();
        deathSound = GetComponent<AudioSource>(); //裝載死亡音效
        rb = GetComponent<Rigidbody2D>(); // 抓取 Rigidbody2D
        Game_paused.gameObject.SetActive(false); // 確保遊戲開始時暫停文字是隱藏的
        isDie = false;
        /*Debug.Log("startgame是：" + startgame);
        if (startgame == false)
        {
            Time.timeScale = 0f; //暫停遊戲時間
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (!startgame) return;

        if (startgame == false)
        {
            Time.timeScale = 0f; //暫停遊戲時間
        }

        if (isDie) return; //如果死亡狀態是true，就都不要執行

        // 按下空白鍵時，切換遊戲暫停狀態
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePause();
            Game_paused.gameObject.SetActive(isPaused); // 顯示或隱藏暫停文字
        }

        // 如果遊戲暫停，不執行移動邏輯
        if (isPaused) return;

        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) //如果按下方向右鍵、或是D鍵
        {
            transform.Translate(moveSpeed*Time.deltaTime, 0, 0);
            GetComponent<SpriteRenderer>().flipX = false;
            anim.SetBool("run", true); //設定為跑步動畫
            anim.SetBool("idle", false);
        }
        else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) //如果按下方向左鍵、或是A鍵
        {
            transform.Translate(-moveSpeed*Time.deltaTime, 0, 0);
            GetComponent<SpriteRenderer>().flipX = true;
            anim.SetBool("run", true); //跑步啟動
            anim.SetBool("idle", false); //待機取消
        }
        else
        {
            anim.SetBool("run", false); //跑步取消
            anim.SetBool("idle", true); //待機啟動
        }
        UpdateScore();
    }

    IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定時間
        gameObject.GetComponent<BoxCollider2D>().enabled = true; // 開啟碰撞器
    }

    // 切換遊戲暫停與恢復
    void TogglePause()
    {
        isPaused = !isPaused;

        if(isPaused)
        {
            Time.timeScale = 0; // 暫停遊戲
        }
        else
        {
            Time.timeScale = 1; // 恢復遊戲
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Normal") //如果是撞到類型Normal的物件(補血階梯)
        {
            if (!startgame) return;
            if(other.contacts[0].normal == new Vector2(0f, 1f)) //如果是碰觸到box上方
            {
                anim.ResetTrigger("hurt"); //重設，取消觸發
                anim.SetTrigger("null");
                ModifyHp(1); //撞到Normal +1血
                Debug.Log("播放音效");
                other.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if(other.gameObject.tag == "Nails") //如果是撞到類型Nails的物件(針刺階梯)
        {
            if(other.contacts[0].normal == new Vector2(0f, 1f)) //如果是碰觸到box上方
            {
                anim.ResetTrigger("null"); //重設，取消觸發
                anim.SetTrigger("hurt");
                ModifyHp(-3); //撞到Nails -3血
                other.gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else if (other.gameObject.tag == "Ceiling") //如果是撞到類型Ceiling的物件(天花板)
        {
            anim.ResetTrigger("null"); //重設，取消觸發
            anim.SetTrigger("hurt");
            ModifyHp(-3); //撞到Ceiling -3血
            gameObject.GetComponent<BoxCollider2D>().enabled = false; //碰撞後把掛載物件的box取消掉，讓角色掉下來
            other.gameObject.GetComponent<AudioSource>().Play();
            StartCoroutine(EnableColliderAfterDelay(0.37f)); // 啟動協程，0.3秒後重新開啟
        } 
        else if (other.gameObject.tag == "Trampoline") //如果是撞到類型Ceiling的物件(彈跳階梯)
        {
            if(other.contacts[0].normal == new Vector2(0f, 1f)) //如果是碰觸到box上方
            {
                anim.ResetTrigger("hurt"); //重設，取消觸發
                anim.SetTrigger("null");
                other.gameObject.GetComponent<AudioSource>().Play();
                // 清除舊的垂直速度，避免連續彈跳時速度累加過多
                rb.velocity = new Vector2(rb.velocity.x, 0);
                //AddForce()是公式：(方向 * 力量, 力量模式)，Vector2.up就是指上方，jumpSpeed則是力道，ForceMode2D.Impulse是瞬間施加力
                rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse); 
            }
        } 
    }

    void OnTriggerEnter2D(Collider2D other) //
    {
        if(other.gameObject.tag == "DeathLine") ////如果是撞到類型Floor2的物件
        {
            Die();
        }
    }

    void ModifyHp(int num)
    {
        HP += num;
        if(HP > 10)
        {
            HP = 10;
        }
        else if(HP <= 0)
        {
            HP = 0;
            Die();
        }
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        for(int i = 0; i<HpBar.transform.childCount; i++) //初始i是0，如果i小於HpBar的長度，就i+1(要遍歷整個HpBar的總長度)
        {
            if(HP > i) //如果生命小於i，就把第幾個子物件顯示出來，已達到顯示HP血量的功能
            {
                HpBar.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                HpBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void UpdateScore()
    {
        scoreTime += Time.deltaTime; //Time.deltaTime是讓Update呼叫一次才會增加一秒
        if(scoreTime > 2f)
        {
            score++;
            scoreTime = 0f;
            scoreText.text = "地下" + score.ToString() + "層";
        }
    }

    void Die()
    {
        isDie = true;
        deathSound.Play();
        Time.timeScale = 0f; //暫停遊戲時間
        replayButton.SetActive(true); //把重新開始按鈕打開
    }

    public void Replay() //點重新開始按鈕要執行的代碼
    {
        isDie = false; //重置死亡狀態
        Time.timeScale = 1f; //遊戲以一倍速開始執行
        SceneManager.LoadScene("SampleScene");
    }

    public void playgame() //點按鈕要執行的開始遊戲
    {
        startgame = true;
    }
}
