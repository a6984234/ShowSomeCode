using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class HitManager : MonoBehaviour
{
    public static HitManager hitManager;
    #region UI物件設定
    public Text text;//已棄用
    public Text ComboText; //連擊Text
    /// <summary>
    /// 敵人相關函式
    /// </summary>
    public Enemy_GetHit enemy;
    /// <summary>
    /// 技能文字,棄用
    /// </summary>
    public GameObject SkillText;

    public static GameObject s_skillText;
    string SaveText;
    [InspectorName("攻擊所需指令數")]
    public int HitCount = 1;
    public static int Combo;
    
    #region 動畫相關參數
    public GameObject Arrow;
    public Transform ArrowParent;
    public List<ArrowsData> arrowsDatas;
    public GameObject UI_effect;
    #endregion
    #region 一些效果
    /// <summary>
    /// 敵人被攻擊時 跳出傷害值
    /// </summary>
    public GameObject JumpDmg;
    #endregion 一些效果
    // Start is called before the first frame update
    void Start()
    {
        hitManager = this;
        s_skillText = SkillText;       
        InitArrows();
        //初始化指令
        void InitArrows()
        {
            for(int i  = 1; i < arrowsDatas.Count -1; i++)
            {                
                var arrow = Instantiate(Arrow, ArrowParent);
                arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowsDatas[i].ArrowsPos.x, 0);
                //Debug.Log(arrow.transform.localPosition);
                arrow.transform.localScale = arrowsDatas[i].Size;
                arrowsDatas[i].ArrowObj = arrow;
                var x =  Random.Range(0, 4);
                //右
                if (x == 0)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 0);
                    arrowsDatas[i].ArrowDriect = "⇨";
                }
                //上
                else if (x == 1)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 90);
                    arrowsDatas[i].ArrowDriect = "⇧";
                }
                //下
                else if (x == 2)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, -90);
                    arrowsDatas[i].ArrowDriect = "⇩";
                }
                //左
                else if (x == 3)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
                    arrowsDatas[i].ArrowDriect = "⇦";
                }
          
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //如果已經進入打Boss的階段 , 開啟輸入指令的相關程式碼
        if (RunDemoV2.status == RunDemoV2.Status.HitBoss)
        {
            PCInput();
            EnemyAtk();
            MoustInput();
            MobileInput();
        }
        ComboText.text = Combo.ToString();
    }
    /// <summary>
    /// 電腦輸入
    /// </summary>
    void PCInput()
    {        
        if (Input.GetKeyDown(KeyCode.W))
        {
            But_UP();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            But_Left();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            But_Down();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            But_Right();
        }
    }
    /// <summary>
    /// 使用按鈕控制
    /// </summary>
    public void ButtonInput(string str)
    {
        if (RunManager.status == RunManager.Status.HitBoss)
        {

            if (str == "up")
            {
                But_UP();
            }
            if (str == "left")
            {
                But_Left();
            }
            if (str == "down")
            {
                But_Down();
            }
            if (str == "right")
            {
                But_Right();
            }
        }   
    }
    #region 手機判斷用參數

    Vector2 StarPos;
    Vector2 EndPos;
    #endregion 手機判斷用參數
    /// <summary>
    /// 手機端操控指令輸入判斷
    /// </summary>   
    void MobileInput()
    {
        if(Input.touchCount == 1)
        {
            //開始滑動位置
            if(Input.touches[0].phase == TouchPhase.Began)
            {
                StarPos = Input.touches[0].position;
            }
            //結束滑動位置
            if(Input.touches[0].phase == TouchPhase.Ended)
            {
                EndPos = Input.touches[0].position;
                //判斷上下左右
                var x = EndPos.x - StarPos.x;
                var y = EndPos.y - StarPos.y;
                var absX = Mathf.Abs(x);
                var absy = Mathf.Abs(y);
                if(absX > absy)
                {
                    //右滑
                    if(x > 0)
                    {
                        But_Right();
                    }
                    //左滑
                    else if (x < 0)
                    {
                        But_Left();
                    }
                }
                else if (absX< absy)
                {
                    //上滑
                    if(y > 0)
                    {
                        But_UP();
                    }
                    //下滑
                    else if(y < 0)
                    {
                        But_Down();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 電腦滑鼠輸入
    /// </summary>
    void MoustInput()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor) return;
        //開始滑動位置
        if (Input.GetMouseButtonDown(0))
        {
            StarPos = Input.mousePosition;
        }
        //結束滑動位置
        if (Input.GetMouseButtonUp(0))
        {
            EndPos = Input.mousePosition;
            //判斷上下左右
            var x = EndPos.x - StarPos.x;
            var y = EndPos.y - StarPos.y;
            var absX = Mathf.Abs(x);
            var absy = Mathf.Abs(y);
            if (absX > absy)
            {
                //右滑
                if (x > 0)
                {
                    But_Right();
                }
                //左滑
                else if (x < 0)
                {
                    But_Left();
                }
            }
            else if (absX < absy)
            {
                //上滑
                if (y > 0)
                {
                    But_UP();
                }
                //下滑
                else if (y < 0)
                {
                    But_Down();
                }
            }
        }

    }
    #region 敵人攻擊相關函式
    /// <summary>
    /// 下次攻擊時間
    /// </summary>
    public static float EnemyAtk_Time;
    /// <summary>
    /// 敵人攻擊呼叫端
    /// </summary>
    void EnemyAtk()
    {
        if (Time.time > EnemyAtk_Time)
        {
            EnemyAtk_Time = enemy.AtkTime + Time.time;
            enemy.EnemyHit();
        }
    }
    /// <summary>
    /// 設定敵人第一次的攻擊間隔
    /// </summary>
    /// <param name="Time"></param>
    public void SetEnemyAtkFirstTime()
    {
        EnemyAtk_Time = Time.time + enemy.AtkTime;
    }
    /// <summary>
    /// 玩家血量減少
    /// </summary>
    /// <param name="Dmage">傳入傷害量</param>
    public static void PlayerHpReduce(int Dmage)
    {
        int Realdamage = Dmage - RunPlayer.runPlayer.M_data.Def;
        if (Realdamage <= 0) Realdamage = 1;
        RunPlayer.runPlayer.M_data.Hp -= Realdamage;
    }
    #endregion 敵人攻擊相關函式
    #region 輸入
    public void But_Left()
    {
        SaveText += "⇦";
        TextUpdate();
    }
    public void But_Right()
    {
        SaveText += "⇨";
        TextUpdate();
    }
    public void But_UP()
    {
        SaveText += "⇧";
        TextUpdate();
    }
    public void But_Down()
    {
        SaveText += "⇩";
        TextUpdate();
    }
    #endregion 輸入
    /// <summary>
    /// 玩家以輸入指令數量 
    /// </summary>
    int m_Hitcount;
    /// <summary>
    /// 執行更新指令
    /// </summary>
    void TextUpdate()
    {
        if(SaveText == arrowsDatas[1].ArrowDriect)
        {
            SaveText = "";
            m_Hitcount += 1;
            Combo += 1;
            AddNewArrow();
            //播放粒子特效
            var effect = Instantiate(UI_effect, ArrowParent);
            effect.transform.position = arrowsDatas[1].ArrowObj.transform.position;
            effect.transform.GetComponent<ParticleSystem>().Play();
            Destroy(effect.gameObject, 2f);
            //進行移動
            for (int  i =1; i< arrowsDatas.Count ; i++)
            {
                var m_arrow = arrowsDatas[i];
                var arrowL = arrowsDatas[i - 1];
                var next_pos = arrowsDatas[i - 1].ArrowsPos;
                var next_scale = arrowsDatas[i - 1].Size;
                //arrow.GetComponent<RectTransform>().anchoredPosition
                m_arrow.ArrowObj.GetComponent<RectTransform>().DOAnchorPos(next_pos, 0.5f);
                m_arrow.ArrowObj.transform.DOScale(next_scale, 0.5f);
                var des_obj = m_arrow.ArrowObj;
                if (i == 1)
                {
                    //m_arrow.ArrowObj.GetComponent<Image>().DOFade(0, 0.5f).OnComplete(() => Destroy(des_obj));
                    Destroy(des_obj);
                }
                arrowL.ArrowObj = m_arrow.ArrowObj;
                arrowL.ArrowDriect = m_arrow.ArrowDriect;                
            }
            if (m_Hitcount >= HitCount)
            {
                AtkEnemy();
                m_Hitcount = 0;
            }
            //增加新的Arrow
            void AddNewArrow()
            {                
                var arrow = Instantiate(Arrow, ArrowParent);
                arrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(arrowsDatas.Last().ArrowsPos.x, 0);
                var x = Random.Range(0, 4);
                arrow.transform.localScale = arrowsDatas.Last().Size;
                arrow.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                arrow.GetComponent<Image>().DOFade(1, 0.5f);
                arrowsDatas.Last().ArrowObj = arrow;
                //右
                if (x == 0)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 0);
                    arrowsDatas.Last().ArrowDriect = "⇨";
                }
                //上
                else if (x == 1)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 90);
                    arrowsDatas.Last().ArrowDriect = "⇧";
                }
                //下
                else if (x == 2)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, -90);
                    arrowsDatas.Last().ArrowDriect = "⇩";
                }
                //左
                else if (x == 3)
                {
                    arrow.transform.localEulerAngles = new Vector3(0, 0, 180);
                    arrowsDatas.Last().ArrowDriect = "⇦";
                }
            }
        }
        else
        {
            Combo = 0;
            SaveText = "";
        }
        return; // 以下棄用  , 可參考
        if(text.text.Substring(0,1) == SaveText)
        {
            text.text = text.text.Remove(0, 1);
            text.text += ADDNewOrder();
            
            SaveText = "";
            m_Hitcount += 1;
            Combo += 1;
            if (m_Hitcount >= HitCount)
            {
                AtkEnemy();
                m_Hitcount = 0;
            }
        }
        //按錯指令
        else       
        {
            Combo = 0;
            SaveText = "";
        }
        /*
        Debug.Log($"{SaveText} , {text.text.Substring(0, SaveText.Length)}");

        if (SaveText != text.text.Substring(0, SaveText.Length))
        {
            SaveText = SaveText.Remove(SaveText.Length - 1);
        }
        //更新文字 完成指令
        if (SaveText == text.text)
        {
            TextChange();
            AtkEnemy();
        }
        Debug.Log(SaveText);
        */
    }
    /// <summary>
    /// 更新指令
    /// </summary>
    string ADDNewOrder()
    {
        var x = Random.Range(0, 4);
        if (x == 0)
        {
            return"⇦";

        }
        else if (x == 1)
        {
            return "⇧";

        }
        else if (x == 2)
        {
            return "⇨";

        }
        else if (x == 3)
        {
            return "⇩";
        }
        return "";
    }
    /// <summary>
    /// 攻擊敵人
    /// </summary>
    void AtkEnemy()
    {
        enemy.GetHit();
    }
    [System.Serializable]
    public class ArrowsData
    {
        public Vector3 ArrowsPos;

        public Vector3 Size;
        /// <summary>
        /// 物件
        /// </summary>
        public GameObject ArrowObj;
        /// <summary>
        /// 紀錄方向
        /// </summary>
        public string ArrowDriect;
    }
}
