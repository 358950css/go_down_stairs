using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private Player player;
    [SerializeField] float moveSpeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && player.startgame)
        {
            transform.Translate(0, moveSpeed*Time.deltaTime, 0); //一直往上移動
            if(transform.position.y > 5.5f) //如果掛載物件的y軸超過6f
            {
                Destroy(gameObject); //刪除掛載該腳本的物件
                //呼叫FloorManager的生成方法
                transform.parent.GetComponent<FloorManager>().SpawnFloor(); //transform.parent是掛載物件的父物件，GetComponent<FloorManager>()是抓取該父物件的FloorManager腳本，.SpawnFloor是抓取該腳本的SpawnFloor函式
            }
        }
    }
}
