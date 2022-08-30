using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static HummingBirdWebApi;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

public class Select_UI_Punch : MonoBehaviour
{
    /// <summary>
    /// 公司名稱
    /// </summary>
    Text CompanyName;
    /// <summary>
    /// 打卡機名稱
    /// </summary>
    Text PunchName;
    /// <summary>
    /// 打卡機紀錄
    /// </summary>
    GameObject PunchBeacon;
    /// <summary>
    /// 預錄物件 , beacon
    /// </summary>
    public GameObject beacon;
    Objs_In_Use In_Use;
    /// <summary>
    /// 顯示今天日期
    /// </summary>
    public Text T_Date_Day;
    /// <summary>
    /// 顯示現在時間
    /// </summary>
    public Text T_Date_Time;
    /// <summary>
    /// 打卡記錄生成位置的父物件
    /// </summary>
    public GameObject RecordParent;
    /// <summary>
    /// 預錄物件 , 打卡紀錄
    /// </summary>
    public GameObject PunchRecord;
    /// <summary>
    /// 背景圖片
    /// </summary>
    public Image Panal;    
    public Client_Manager client_Manager;
    /// <summary>
    /// 聲音 按下與放開
    /// </summary>
    public AudioClip Press, Put;
    /// <summary>
    /// 動畫參數
    /// </summary>
    public float bigsize, movetime,shaketime, shakex, shakey;
    /// <summary>
    /// 三角定位判斷使用 , 是否在範圍內
    /// </summary>
    public int RssiCheck = 70;
    /// <summary>
    /// 能否打卡
    /// </summary>
    public bool CanPunch;
    /// <summary>
    /// 打卡紐文字 
    /// </summary>
    public Text ButtonText;
    /// <summary>
    /// 更新鈕打卡文字
    /// </summary>
    public Text RecordUpdate;
    #region 打卡本地檔案 相關
    /// <summary>
    /// 一天只能最多打卡30次
    /// </summary>
    static string PunchTimeSave
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                string path = Application.persistentDataPath + @"\PunchTimeSave";
                return path;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                string path = Application.persistentDataPath + @"/PunchTimeSave";
                return path;
            }
            else
            {
                string path = Application.persistentDataPath + @"\PunchTimeSave";
                return path;
            }
        }
    }
    class PunchTimeClass
    {
        public DateTime Date { get; set; }
        public int PunchTime { get; set; }
    }
    /// <summary>
    /// 判斷當天打卡次數是否超過30次
    /// </summary>
    bool PunchTimeCheck
    {
        get {
            if (File.Exists(PunchTimeSave))
            {
                using (StreamReader reader = new StreamReader(PunchTimeSave))
                {
                    var data = JsonConvert.DeserializeObject<PunchTimeClass>(reader.ReadToEnd());
                    if (data.Date.Date == DateTime.Now.Date)
                    {
                        //如果今日的打卡次數少於30 就仍然可以打卡
                        if (data.PunchTime < 30)
                            return true;
                        //超過就沒辦法打卡
                        return false;
                        

                    }
                    //如果不是今天的日期 也可以打卡
                    else
                    {
                        return true;
                    }
                }
            }
            //沒有檔案 , 寫入檔案 並回傳可以打卡
            else
            {
                using (StreamWriter writer = new StreamWriter(PunchTimeSave))
                {
                    var data = new PunchTimeClass
                    {
                        Date = DateTime.Now,
                        PunchTime = 0
                    };
                    writer.Write(JsonConvert.SerializeObject(data));
                    return true;
                }
            }

            }
    }
    //紀錄寫入次數
    void AddPunchTime()
    {
        PunchTimeClass data = null ;
        using (StreamReader reader = new StreamReader(PunchTimeSave))
        {
             data = JsonConvert.DeserializeObject<PunchTimeClass>(reader.ReadToEnd());
            if(data.Date.Date != DateTime.Now.Date)
            {
                data.Date = DateTime.Now;
                data.PunchTime = 1;
            }
            else
            {
                data.PunchTime += 1;
            }
        }
        using (StreamWriter writer = new StreamWriter(PunchTimeSave))
        {
            writer.Write(JsonConvert.SerializeObject(data));
        }
    }
    #endregion 打卡本地檔案 相關
    /// <summary>
    /// 當下顯示的公司資料
    /// </summary>
    public static Table_HbbCompany Select_Company;
    // Start is called before the first frame update
    void Start()
    {
        In_Use = GetComponent<Objs_In_Use>();
        CompanyName = In_Use.GetObj("CompanyName").GetComponent<Text>();
        PunchName = In_Use.GetObj("PunchName").GetComponent<Text>();
        PunchBeacon = In_Use.GetObj("PunchBeacon");
        CompanyName.text = Select_Company.full_co_name;
        PunchName.text = Select_Company.co_name + "打卡機";
        DataDeletCheck();
        InitRecord();
        //之後就不用經過網路也可以進入打卡頁面 , 但是上傳資料的時候會在判斷Token是否有問題
        Select_Company.CanPunch = true;
        Debug.LogWarning(JsonConvert.SerializeObject(Select_Company));
        Debug.LogWarning(JsonConvert.SerializeObject(Client_Manager.userData));
        Client_Manager.WriteUserData();

        StartCoroutine(enumerator());
        //更新打卡按鈕
        IEnumerator enumerator()
        {
            Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));//重新排序
                                                                                                         //如果間隔沒超過30秒
            if (Client_Manager.userData.WorkRecord.Count > 0)
            {
                TimeSpan timeSpan = DateTime.Now.Subtract(PunchTime);
                if (Math.Abs(timeSpan.TotalSeconds) < 30)
                {
                    var t = 30 - Math.Abs(timeSpan.TotalSeconds);
                    ButtonText.text = $"Beacon打卡 ({t.ToString("0")})";
                }
                else
                {
                    ButtonText.text = $"Beacon打卡";
                }
            }
            //更新鈕 到數計算
            TimeSpan timeSpan2 = DateTime.Now.Subtract(UpdateTime);
            if (Math.Abs(timeSpan2.TotalSeconds) < 30)
            {
                var t = 30 - Math.Abs(timeSpan2.TotalSeconds);
                RecordUpdate.text = $"({t.ToString("0")})";
            }
            else
            {
                RecordUpdate.text = $"";
            }
            yield return new WaitForSeconds(1);            
            StartCoroutine(enumerator());                 
        }     
    }
    /// <summary>
    /// 手機將app縮小時 清空所有beacon紀錄
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        Client_Manager.beaconDatas.Clear();
    }
    // Update is called once per frame
    void Update()
    {

        T_Date_Day.text = DateTime.Now.ToString("yyyy年MM月dd日") + $" {toZh_tw_Date()}";
        T_Date_Time.text = DateTime.Now.ToString("HH:mm:ss");
        for(int i = 2; i < PunchBeacon.transform.childCount; i++)
        {
            Destroy(PunchBeacon.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < Client_Manager.beaconDatas.Count; i++)
        {
            var data = Client_Manager.beaconDatas[i];
            //通用型才能打卡
            if (data.bc_company == Select_Company.co_code && data.bc_major == 0)
            {
                var obj = Instantiate(beacon, PunchBeacon.transform);
                obj.GetComponent<Text>().text = $"HB{data.bc_company}-{data.bc_major}-{data.bc_minor} RSSI -{data.Rssi} dBm 電量:{data.Electricity}%";//HBB-1-101-1 RSSI -73 dBm
                Canvas.ForceUpdateCanvases();
                if (CanPunch)
                    PunchBeacon.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
            }
        }
        //測試人員將永遠會有一顆Beacon
        if (Client_Manager.userData.HbbUser.user_beacon == 1)
        {
            var obj = Instantiate(beacon, PunchBeacon.transform);
            obj.GetComponent<Text>().text = $"HB{Select_Company.co_code}-{0}-{1} RSSI -{73} dBm";//HBB-1-101-1 RSSI -73 dBm
            Canvas.ForceUpdateCanvases();
            if (CanPunch)
                PunchBeacon.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }
        if (PunchBeacon.transform.childCount == 2)
        {
            for(int i = 0; i < PunchBeacon.transform.childCount; i++)
            {
                PunchBeacon.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < PunchBeacon.transform.childCount; i++)
            {
                PunchBeacon.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        //如果公司有啟用三角定位
        if (Select_Company.co_position == 1)
        {
            ///3顆以上啟動三角定位
            if (PunchBeacon.transform.childCount >= 5)
            {
                CanPunch = false;
                int Getbeacon = 0;
                for (int i = 0; i < Client_Manager.beaconDatas.Count; i++)
                {
                    var data = Client_Manager.beaconDatas[i];
                    //通用型才能打卡
                    if (data.bc_company == Select_Company.co_code && data.bc_major == 0)
                    {
                        if (Math.Abs(data.Rssi) < Math.Abs(RssiCheck))
                        {
                            Getbeacon += 1;
                            //Debug.LogWarning(data.Rssi + "|" + Getbeacon);
                        }
                    }
                }
                if (Getbeacon >= 3)
                {
                    CanPunch = true;
                }
            }
            if (CanPunch)
            {
                PunchBeacon.SetActive(true);
            }
            else
                PunchBeacon.SetActive(false);
        }
        else
            CanPunch = true;
             
    }
    /// <summary>
    /// 每30才能打卡一次
    /// </summary>
    static DateTime PunchTime = DateTime.Now.AddSeconds(-60);
    /// <summary>
    /// 執行員工打卡
    /// </summary>
    public void StaffPunch()
    {
        Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));//重新排序

        #region 打卡前判斷
        //判斷公司已被停止使用        
        if (Select_Company.co_status == 1)
        {
            Client_Manager.UI_ERROR("公司已停止使用蜂鳥服務");
            return;
        }
        //判斷當天打卡次數是否超過30次
        if (!PunchTimeCheck)
        {
            Client_Manager.UI_ERROR("已超過當天打卡次數");
            return;
        }
        //如果間隔沒超過30秒
        if (Client_Manager.userData.WorkRecord.Count > 0)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(PunchTime);
            if (Math.Abs(timeSpan.TotalSeconds) < 30)
            {
                Client_Manager.UI_ERROR("打卡間隔至少30秒");
                return;
            }
        }
        PunchTime = DateTime.Now;

        Table_PunchShift punchshift = new Table_PunchShift();
        if (!string.IsNullOrEmpty(GetIPAddress()))
        {
            //有網路時打卡都會更新班別資訊
            Client_Manager.userData.PunchShift.Clear();
            //判斷是否是打卡人員
            var _punchshift = Client_Manager.userData.PunchShift.Where(x => { if (x.co_gu_id == Select_Company.gu_id) return true; return false; }).ToList();
            if (_punchshift.Count <= 0)
            {
                //Client_Manager.userData.PunchShift = GetPunchShift(Client_Manager.userData.HbbUser);
                var data = GetPunchShift(Client_Manager.userData.HbbUser);
                if (data == null)
                {
                    Client_Manager.UI_ERROR("無法打卡！\n\n公司未幫你加入打卡人員，\n無法知道你的班別。\n請聯繫公司管理人員。");
                    return;
                }
                else
                {
                    punchshift = data;
                    Client_Manager.userData.PunchShift.Add(data);
                }
            }
        }
        else
        {
            var _punchshift = Client_Manager.userData.PunchShift.Where(x => { if (x.co_gu_id == Select_Company.gu_id) return true; return false; }).ToList();
            if (_punchshift.Count <= 0)
            {
                //Client_Manager.userData.PunchShift = GetPunchShift(Client_Manager.userData.HbbUser);
                Client_Manager.UI_ERROR("無網路與班別資料");              
            }
            else
            {
                punchshift = _punchshift[0];
            }
        }
  
        AddPunchTime();
        if(PunchBeacon.transform.childCount <=2)
        {
             Client_Manager.UI_ERROR("附近沒有Beacon");
            return;
        }
        if (!CanPunch)
        {
            Client_Manager.UI_ERROR("還沒進公司!");
            return;
        }
#endregion 打卡前判斷

        //Debug.LogWarning(PunchCheck().ToString());
        try
        {
            if (!string.IsNullOrEmpty(GetIPAddress()))
            {
                Task.Run(() =>
                {
                    //Debug.LogWarning("Connect api");
                    Debug.LogWarning(JsonConvert.SerializeObject(Client_Manager.userData.PunchShift));
                    Table_WorkRecord Record = InsPunchRecord(punchshift);
                    var data = HummingBirdWebApi.PunchRecord(Record, out string Result);

                    if (data != null)
                    {
                        Action action = () =>
                        {
                            data.Send = true;
                            Client_Manager.userData.WorkRecord.Add(data);
                            Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));//重新排序
                            Client_Manager.WriteUserData();
                            var obj = Instantiate(PunchRecord, RecordParent.transform);

                            obj.transform.localScale = Vector3.one;
                            var text = obj.transform.Find("Text").gameObject;
                            obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)}";
                            obj.transform.SetSiblingIndex(0);
                            TextAni(text);
                            if (RecordParent.transform.childCount > 4)
                            {
                                Destroy(RecordParent.transform.GetChild(4).gameObject);
                            }
                            //Canvas.ForceUpdateCanvases();
                            //RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
                            //如果Token錯誤
                            if (Result.Contains("fail"))
                            {
                                Client_Manager.OpenUI(client_Manager.UI_Login);
                                Client_Manager.UI_ERROR("資料比對失敗,請重新登入");
                                Client_Manager.userData = new UserData();
                                Client_Manager.WriteUserData();
                            }
                        };                                              
                        Client_Manager.actions.Add(action);
                    }
                    else
                    {
                        //員工離職
                        if(Result.Contains("staff"))
                        {
                            Client_Manager.UI_ERROR("你已離職,無法打卡b");
                        }
                    }

                });
            }
            else
            {
                Debug.LogWarning("上傳資料失敗 寫入至本地端等待上傳");
                Table_WorkRecord data = InsPunchRecord(punchshift);
                Client_Manager.userData.WorkRecord.Add(data);
                Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));//重新排序
                PunchCheck();
                Client_Manager.WriteUserData();
                var obj = Instantiate(PunchRecord, RecordParent.transform);
                obj.transform.localScale = Vector3.one;

                //obj.transform.DOScale(Vector3.one , movetime).OnComplete(() => { obj.transform.DOShakeScale(shaketime, new Vector3(shakex, shakey, 0)); });

                //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
                //     $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>未上傳</color>";
                var text = obj.transform.Find("Text").gameObject;
                obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>未上傳</color>";
                obj.transform.SetSiblingIndex(0);
                TextAni(text);
                if (RecordParent.transform.childCount > 4)
                {
                    Destroy(RecordParent.transform.GetChild(4).gameObject);
                }
                Canvas.ForceUpdateCanvases();
                RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("上傳資料失敗 寫入至本地端等待上傳 error " + e.Message);
            Table_WorkRecord data = new Table_WorkRecord
            {
                wkr_time = DateTime.Now,
                wkr_status = (int)PunchCheck()
            };
            Client_Manager.userData.WorkRecord.Add(data);
            Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));//重新排序
            PunchCheck();
            Client_Manager.WriteUserData();
            var obj = Instantiate(PunchRecord, RecordParent.transform);
            obj.transform.localScale = Vector3.one ;
            //obj.transform.DOScale(Vector3.one, movetime).OnComplete(() => { obj.transform.DOShakeScale(shaketime, new Vector3(shakex, shakey, 0)); });
            

            //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
              //     $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>未上傳</color>";

            var text = obj.transform.Find("Text").gameObject;
            obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>未上傳</color>";
            obj.transform.SetSiblingIndex(0);

            if (RecordParent.transform.childCount > 4)
            {
                Destroy(RecordParent.transform.GetChild(4).gameObject);
            }
            Canvas.ForceUpdateCanvases();
            RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }
    }
    /// <summary>
    /// 打卡按鈕按下 , 執行一些動畫反饋
    /// </summary>
    public void btn_PunchDown()
    {
        Panal.color = new Color32(255, 71, 0, 165);
        GameObject.Find("Background").GetComponent<PunchColorChenge>().PunchDown();
        Camera.main.transform.GetComponent<AudioSource>().PlayOneShot(Press);
    }
    /// <summary>
    /// 打卡按鈕放開 , 執行一些動畫反饋
    /// </summary>
    public void btn_PunchUp()
    {
        Panal.color = new Color32(255, 167, 0, 165);
        GameObject.Find("Background").GetComponent<PunchColorChenge>().PunchUp();
        Camera.main.transform.GetComponent<AudioSource>().PlayOneShot(Put);
    }
    /// <summary>
    ///  打卡文字動畫
    /// </summary>
    /// <param name="text"></param>
    void TextAni(GameObject text)
    {
        RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        RecordParent.GetComponent<VerticalLayoutGroup>().enabled = true;
        Canvas.ForceUpdateCanvases();
        RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        RecordParent.GetComponent<VerticalLayoutGroup>().enabled = false;
        var Pos = text.transform.localPosition;
        Debug.LogWarning(Pos.ToString());
            
        text.transform.localPosition += new Vector3(Screen.width, 0, 0);
        Debug.LogWarning(text.transform.localPosition);
        text.transform.DOLocalMove(Pos, movetime).SetEase(Ease.OutBack).OnComplete(()=> {
            RecordParent.GetComponent<VerticalLayoutGroup>().enabled = true;
            RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
            Canvas.ForceUpdateCanvases();
        });
    }
    /// <summary>
    /// 判斷目前是打什麼卡 , 還有是否異常判斷
    /// </summary>
    PunchMode PunchCheck()
    {
        var user_shift = Client_Manager.userData.PunchShift.Where(x => { if (x.co_gu_id == Select_Company.gu_id) return true; return false; }).First(); //Client_Manager.userData.PunchShift;

        DateTime StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, user_shift.shift_start.Hour, user_shift.shift_start.Minute, 0, 0);
        DateTime EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, user_shift.shift_end.Hour, user_shift.shift_end.Minute, 0, 0);
        if (StartTime > EndTime)
            EndTime = EndTime.AddDays(1);
        var Records = Client_Manager.userData.WorkRecord.Where(x =>
       {
            //獲取今天的打卡記錄
            if (x.wkr_group == user_shift.wkr_group() && x.wkr_status!=(int)PunchMode.none)
           {
               return true;
           }
           return false;
       }).ToList();

        if (DateTime.Now > StartTime.AddHours(-4) && DateTime.Now < EndTime.AddHours(4))
        {
            if (Records.Count > user_shift.PunchCount())
            {
                return PunchMode.none;
            }
            else
            {
                if (user_shift.PunchCount() == 2)
                {
                    if (Records.Count == 0)
                        return PunchMode.@in;
                    else if (Records.Count == 1)
                        return PunchMode.@out;
                }
                else
                {
                    if (Records.Count == 0)
                        return PunchMode.@in;
                    else if (Records.Count == 1)
                        return PunchMode.PunRest_Start;
                    else if (Records.Count == 2)
                        return PunchMode.PunRest_End;
                    else if (Records.Count == 3)
                        return PunchMode.@out;
                }
            }     
        }
        return PunchMode.none;
    }
    /// <summary>
    /// 產生正確的打卡記錄
    /// </summary>
    /// <param name="_PunchShift"></param>
    /// <returns></returns>
    Table_WorkRecord InsPunchRecord(Table_PunchShift _PunchShift)
    {
        Table_WorkRecord table_WorkRecord = new Table_WorkRecord
        {
            wkr_time = DateTime.Now,
            shift_gu_id = _PunchShift.gu_id,
            wkr_group = _PunchShift.wkr_group(),
            wkr_status = (int)PunchCheck(),
            shift_name = _PunchShift.shift_name,
            co_gu_id = Select_Company.gu_id,
            shift_start = _PunchShift.shift_start,
            shift_end = _PunchShift.shift_end,
            rest_start = _PunchShift.rest_start,
            rest_end = _PunchShift.rest_end
        };
        return table_WorkRecord;
    }
    /// <summary>
    ///  判斷打卡狀態
    /// </summary>
    /// <param name="table_WorkRecord"></param>
    /// <returns></returns>
    string TxtMode (Table_WorkRecord table_WorkRecord)
    {
        if(table_WorkRecord.wkr_status == (int)PunchMode.@in)
        {
            return "上班";
        }
        else if (table_WorkRecord.wkr_status == (int)PunchMode.@out)
        {
            return "下班";
        }
        else if (table_WorkRecord.wkr_status == (int)PunchMode.PunRest_Start)
        {
            return "休息";
        }
        else if (table_WorkRecord.wkr_status == (int)PunchMode.PunRest_End)
        {
            return "休息結束";
        }
        else
        {
            return "異常";
        }
    }
    /// <summary>
    /// 打卡記錄只保存3個月
    /// </summary>
    void DataDeletCheck()
    {
        foreach(var p  in Client_Manager.userData.WorkRecord)
        {
            if(p.wkr_time < DateTime.Now.AddDays(-90))
            {
                Client_Manager.userData.WorkRecord.Remove(p);
            }                            
        }
    }
    static DateTime UpdateTime;
    /// <summary>
    /// 更新按鈕
    /// </summary>
    public void Btn_UpdateRecord()
    {      
        TimeSpan timeSpan = DateTime.Now.Subtract(UpdateTime);
        //增加時間限制
        if (Math.Abs(timeSpan.TotalSeconds) > 30)
        {
            if (Initrecord != null) return;
            for (int i = 0; i < RecordParent.transform.childCount; i++)
            {
                Destroy(RecordParent.transform.GetChild(i).gameObject);
            }
            StartCoroutine(enumerator());
            UpdateTime = DateTime.Now;
        }
        IEnumerator enumerator()
        {
            yield return new WaitForEndOfFrame();
            InitRecord();
        }
    }
    Task Initrecord;
    /// <summary>
    /// 讀取本地打卡記錄 , 檢查是否有資料尚未上傳
    /// </summary>
    public void InitRecord()
    {
        //獲取打卡紀錄 , 未上傳的資料要保留 不能覆蓋
        if (PingConnect)
        {
            if (Initrecord != null) return;

            Initrecord =  Task.Run(() =>
            {
                var PunchRecord = GetUserPunchRecord(Client_Manager.userData);
                foreach (var p in PunchRecord)
                    p.Send = true;
                return PunchRecord;
            }).ContinueWith((t) =>
            {

                List<Table_WorkRecord> SaveData = new List<Table_WorkRecord>(); //用來存放未上傳的資料
                foreach (var p in Client_Manager.userData.WorkRecord)
                {
                    if (!p.Send)
                        SaveData.Add(p);
                }
                Client_Manager.userData.WorkRecord = t.Result;
                foreach (var p in SaveData)
                {
                    Client_Manager.userData.WorkRecord.Add(p);
                }
                //將未上傳的資料嘗試上傳
                bool TokenFail = false;
                foreach (var p in Client_Manager.userData.WorkRecord)
                {
                    if (!p.Send)
                    {
                        var Result = HummingBirdWebApi.PunchRecord(p);
                        //資料已上傳 但是token錯誤
                        if (Result.Contains("token"))
                        {
                            p.Send = true;
                            TokenFail = true;
                        }
                        //完成上傳
                        else if (Result.Contains("pass"))
                        {
                            p.Send = true;
                        }

                    }
                }
                if (TokenFail)
                {
                    Client_Manager.OpenUI(client_Manager.UI_Login);
                    Client_Manager.UI_ERROR("資料比對失敗,請重新登入");
                    Client_Manager.userData = new UserData();
                    Client_Manager.WriteUserData();
                }
                Client_Manager.WriteUserData();
            }).ContinueWith((t) =>
            {
                Action action = () =>
                {
                    //進行排序
                    Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));                  
                    var WorkRecord = Client_Manager.userData.WorkRecord.Where(x => { if (x.co_gu_id == Select_Company.gu_id) return true; return false; }).ToList();
                    if (WorkRecord.Count < 4)
                    {
                        for (int i = 0; i < WorkRecord.Count; i++)
                        {
                            var data = WorkRecord[i];
                            var obj = Instantiate(PunchRecord, RecordParent.transform);
                            string Send = "";
                            if (!data.Send)
                                Send = "未上傳";
                            //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
                            //  $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                            var text = obj.transform.Find("Text").gameObject;
                            obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            var data = WorkRecord[i];
                            var obj = Instantiate(PunchRecord, RecordParent.transform);
                            string Send = "";
                            if (!data.Send)
                                Send = "未上傳";
                            //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
                            //  $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                            var text = obj.transform.Find("Text").gameObject;
                            obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";

                        }
                    }
                    Canvas.ForceUpdateCanvases();
                    RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
                };
                Client_Manager.actions.Add(action);
                Task.Delay(1000).ContinueWith((t2) => { Initrecord = null; });
                
            });
        }
        //沒有網路直接顯示本地資料
        else
        {
            //進行排序
            Client_Manager.userData.WorkRecord.Sort((x, y) => -DateTime.Compare(x.wkr_time, y.wkr_time));
            var WorkRecord = Client_Manager.userData.WorkRecord.Where(x => { if (x.co_gu_id == Select_Company.gu_id) return true; return false; }).ToList();
            if (WorkRecord.Count < 4)
            {
                for (int i = 0; i < WorkRecord.Count; i++)
                {
                    var data = WorkRecord[i];
                    var obj = Instantiate(PunchRecord, RecordParent.transform);
                    string Send = "";
                    if (!data.Send)
                        Send = "未上傳";
                    //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
                    //  $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                    var text = obj.transform.Find("Text").gameObject;
                    obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    var data = WorkRecord[i];
                    var obj = Instantiate(PunchRecord, RecordParent.transform);
                    string Send = "";
                    if (!data.Send)
                        Send = "未上傳";
                    //obj.GetComponent<Text>().text = "----------------------------------------------------------------" +
                    //  $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";
                    var text = obj.transform.Find("Text").gameObject;
                    obj.transform.Find("Text").GetComponent<Text>().text = $"\n{data.wkr_time.ToString("MM月dd日hh點mm分ss秒")} {TxtMode(data)} <color=red>{Send}</color>";

                }
            }
            Canvas.ForceUpdateCanvases();
            RecordParent.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
        }
    }
    /// <summary>
    /// 顯示今天禮拜幾
    /// </summary>
    /// <returns></returns>
    string toZh_tw_Date()
    {
        string m_DayofWeek = "禮拜";
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                m_DayofWeek += "日";
                break;
            case DayOfWeek.Monday:
                m_DayofWeek += "一";
                break;
            case DayOfWeek.Tuesday:
                m_DayofWeek += "二";
                break;
            case DayOfWeek.Wednesday:
                m_DayofWeek += "三";
                break;
            case DayOfWeek.Thursday:
                m_DayofWeek += "四";
                break;
            case DayOfWeek.Friday:
                m_DayofWeek += "五";
                break;
            case DayOfWeek.Saturday:
                m_DayofWeek += "六";
                break;
        }
        return m_DayofWeek;
    }
}
