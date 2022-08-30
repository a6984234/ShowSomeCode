using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.U2D;

#if UNITY_EDITOR
[CustomEditor(typeof( SceneObject))]
public class SceneObject_Editer : Editor
{
    /// <summary>
    /// 關卡讀寫入功能實作位置
    /// </summary>
    public override void OnInspectorGUI()
    {
        //SplineControlPoint
        base.OnInspectorGUI();
        var myClass = (SceneObject)target;
        string savePath = Application.dataPath + $"/SavelevelPath";
        //執行儲存關卡，檔案將寫入savePath資料夾
        if (GUILayout.Button("Save"))
        {       
            List<ObjDataSave> objDataSaves = new List<ObjDataSave>();
            myClass.Objects = new List<GameObject>();
            for (int i  =0; i <  myClass.transform.childCount; i++)
            {
                var obj = myClass.transform.GetChild(i).gameObject;
                myClass.Objects.Add(myClass.transform.GetChild(i).gameObject);
                var tagName =  obj.GetComponent<TagName>().m_TagName;
                ObjDataSave objDataSave = new ObjDataSave
                {
                    obj = $"SceneObject/{tagName}",
                    pos = obj.transform.position,
                    scale = obj.transform.localScale,
                    Rotation = obj.transform.eulerAngles,                                        
                };
                if (obj.TryGetComponent<SpriteShapeController>(out var ssc))
                    objDataSave.m_ControlPoints = obj.GetComponent<SpriteShapeController>().spline.m_ControlPoints;
                objDataSaves.Add(objDataSave);
              
            }              
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using(StreamWriter writer = new StreamWriter(savePath+$"/{myClass.SaveName}.lv"))
            {
                //Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(objDataSaves));
                writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(objDataSaves));
            }
            Debug.Log("SAVE!");
            
        }
        //設定讀取的位置
        if (GUILayout.Button("讀取位置"))
        {
            myClass.LoadPath =  EditorUtility.OpenFilePanel("選取檔案位置", savePath, "lv");
        }
        ///執行讀取
        if (GUILayout.Button("讀取關卡資料"))
        {
            using (StreamReader reader = new StreamReader(myClass.LoadPath))
            {
                string data = reader.ReadToEnd();
                var objdata = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjDataSave>>(data);
                delet();
                foreach (var p in objdata)
                {
                    var obj = Instantiate(Resources.Load<GameObject>(p.obj));
                    obj.transform.parent = myClass.transform;
                    obj.transform.position = p.pos;
                    obj.transform.localScale = p.scale;
                    obj.transform.eulerAngles = p.Rotation;
                    if (obj.TryGetComponent<SpriteShapeController>(out var ssc))
                        ssc.spline.m_ControlPoints = p.m_ControlPoints;
                    
                }
                Debug.Log("Load!");
            }
        }
        void delet()
        {
            for (int i = 0; i < myClass.transform.childCount; i++)
            {
                DestroyImmediate(myClass.transform.GetChild(i).gameObject);
                //Destroy(myClass.transform.GetChild(i).gameObject);
            }
            if (myClass.transform.childCount > 0)
                delet();
        }
    }
}
public class ObjDataSave
{
    /// <summary>
    /// 使用物件
    /// </summary>
    public string obj;
    /// <summary>
    /// 世界位置
    /// </summary>
    public Vector3 pos;
    /// <summary>
    /// 大小
    /// </summary>
    public Vector3 scale;

    /// <summary>
    /// transform.rotation
    /// </summary>
    public Vector3 Rotation;    
    /// <summary>
    /// 儲存任意2d變形物件
    /// </summary>
     public List<SplineControlPoint> m_ControlPoints ;
}
#endif