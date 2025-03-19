using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject[] floorPrefabs; //要創建階梯的列表
    private GameObject lastFloor; // 記錄前一次生成的階梯物件
    [SerializeField] float minDistanceX = 3f; // 設定 X 軸最小間距
    [SerializeField] float minDistanceY = 5f; // 設定 Y 軸最小間距

    public void SpawnFloor() //創建階梯
    {
        int r = Random.Range(0, floorPrefabs.Length); //隨機挑選從0到floorPrefabs列表長度中的數值

        Vector3 newPosition;
        if (lastFloor != null) //如果lastFloor不為空，在與下一個生成的物件做比較來生成
        {
            do //若距離不夠，則生成直到距離大於minDistance的物件為止
            {
                float randomX = Random.Range(-5.5f, 3.5f); // 隨機生成 X 軸位置
                newPosition = new Vector3(randomX, -8f, 0f); //這邊就是新生成物件的座標
            } 
            //這邊是計算新生成物件與上一個物件之間的距離，判斷若小於minDistance，則又從19行的do開始執行
            while (Mathf.Abs(lastFloor.transform.position.x - newPosition.x) < minDistanceX); 
            GameObject floor = Instantiate(floorPrefabs[r], transform); //這邊是創建floorPrefabs列表內第r的物件，transform是把該腳本掛載的物件當作生成物件的父物件
            floor.transform.position = newPosition; //生成物件的隨機位置
            lastFloor = floor;
        }
        else //如果lastFloor是空的，直接先生成物件
        {
            GameObject floor = Instantiate(floorPrefabs[r], transform); //這邊是創建floorPrefabs列表內第r的物件，transform是把該腳本掛載的物件當作生成物件的父物件
            floor.transform.position = new Vector3(Random.Range(-5.5f, 3.5f), -8f, 0f); //生成物件的隨機位置
            lastFloor = floor;
        }
    }
}
