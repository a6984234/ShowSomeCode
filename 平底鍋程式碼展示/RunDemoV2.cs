using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunDemoV2 : MonoBehaviour
{
    // Start is called before the first frame update
    public float MoveSpeed;
    /// <summary>
    /// 現在的跑步速度
    /// </summary>
    [HideInInspector]
    public float NowSpeed;
    /// <summary>
    /// 玩家物件
    /// </summary>
    public GameObject Player;
    /// <summary>
    /// 攝影機移動速度
    /// </summary>
    public float CameraMoveSpeed;
    Vector3 CamaraPos;
    /// <summary>
    /// 攝影機y軸不動
    /// </summary>
    float Y_lock;
    /// <summary>
    /// 獲取此關卡總共有多少Item
    /// </summary>
    public float MaxItem;
    /// <summary>
    /// 已獲得的Item
    /// </summary>
    public float GetItem;
    public static RunDemoV2 runDemo;
    /// <summary>
    /// 遊玩狀態
    /// </summary>
    public static Status status;
    /// <summary>
    /// 開始攻擊Boss
    /// </summary>
    public bool StartHit;    
    public GameObject UI_InRunnig;
    public GameObject UI_HitBoss;
    void Start()
    {
        runDemo = this;
        NowSpeed = MoveSpeed;
        CamaraPos = Player.transform.position - Camera.main.transform.position;
        Y_lock = Camera.main.transform.position.y;
        LevelSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if (RunPlayer.PlayerDead)
        {
            NowSpeed -= 2 * Time.deltaTime;
            if (NowSpeed < 0) NowSpeed = 0;
        }
        else if (!RunPlayer.PlayerDead)
        {
            //玩家為死亡 同時也還沒有開始打Boss
            if (!StartHit)
                Player.transform.position += new Vector3(NowSpeed * Time.deltaTime, 0, 0);

        }

        //設定攝影機位置
        Vector3 nextpos = Player.transform.position - CamaraPos;
        nextpos = new Vector3(nextpos.x, Y_lock, nextpos.z);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, nextpos, CameraMoveSpeed);
        
    }
    /// <summary>
    /// 進行關卡參數設定
    /// </summary>
    void LevelSetting()
    {
        var obj =  GameObject.Find("SceneObject");
        var tagNames = obj.transform.GetComponentsInChildren<TagName>();
        MaxItem = 0;
        GetItem = 0;
        foreach(var p in tagNames)
        {
            if(p.m_TagName == "Item1")
            {
                MaxItem += 1;
            }
        }
    }
    /// <summary>
    /// 執行UI切換
    /// </summary>
    public void UI_Swich_HitBoss()
    {
        UI_InRunnig.SetActive(false);
        UI_HitBoss.SetActive(true);
    }
    /// <summary>
    /// 目前狀態
    /// </summary>
    public enum Status
    {
        Run,
        LastRun,
        HitBoss,
        wait
    }
}
