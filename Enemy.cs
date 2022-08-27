using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Enemy : MonoBehaviour , Enemy_GetHit 
{
    public float Hp = 10;
    public float shake;
    public ParticleSystem HitEffect;
    float NowHp;
    Image HPImg;
    public int AtkTime { get; set; } = 3;

    // Start is called before the first frame update
    private  void Start()
    {
        HPImg = GetComponentInChildren<Image>();
        NowHp = Hp;
        HitManager.hitManager.enemy = this;
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    Tweener tweener;

    #region Interface Enemy_GetHit

    public void GetHit()
    {

        HpReduce();        
        if (tweener == null)
            tweener = transform.DOShakePosition(0.3f, shake).OnComplete(() => tweener = null);
        //技能迴圈
        foreach(var p in RunPlayer.Skill)
        {
            //2連擊
            if(p == "Skill_DoubleHit")
            {
                var skilltext =  Instantiate(HitManager.s_skillText);
                skilltext.SetActive(true);
                skilltext.transform.position = transform.position + new Vector3(0, 1, 0);
                skilltext.transform.DOMoveY(2, 3);
                skilltext.GetComponentInChildren<Text>().DOColor(new Color(0, 0, 0, 0), 3).OnComplete(() => Destroy(skilltext.gameObject)) ;

                StartCoroutine(enumerator());
                IEnumerator enumerator()
                {
                    yield return new WaitForSeconds(0.2f);
                    HpReduce();
               
                }
            }
        }
        void HpReduce()
        {
            Hp -= RunPlayer.runPlayer.M_data.Atk;
            HPImg.fillAmount = Hp / NowHp;
            HitEffect.Play();

            var dmg =  Instantiate(HitManager.hitManager.JumpDmg);
            dmg.transform.position = transform.position;
            var nextpos = new Vector3(dmg.transform.position.x + Random.Range(-1f, 1f), dmg.transform.position.y + Random.Range(1f, 2f), 0);
            dmg.transform.DOMove(nextpos, 0.5f);
            dmg.GetComponentInChildren<Text>().DOFade(0, 2f);
            dmg.GetComponentInChildren<Text>().text = "-" + RunPlayer.runPlayer.M_data.Atk.ToString();
            if (Hp<= 0)
            {
                //RunManager.runManager.EnemyDead();
                Destroy(gameObject);
            }
        }
    }
    Tween playshake;
    public void EnemyHit()
    {
        var effect = Instantiate(HitEffect);
        effect.transform.position = RunPlayer.runPlayer.transform.position;
        effect.Play();
        Destroy(effect.gameObject, 3f);
        if (playshake == null)
            playshake = RunPlayer.runPlayer.transform.DOShakePosition(0.3f, shake).OnComplete(() => playshake = null);
        HitManager.PlayerHpReduce(1);
    }

    #endregion Interface Enemy_GetHit
}
public interface Enemy_GetHit
{
    /// <summary>
    /// 敵人攻擊冷卻時間
    /// </summary>
    int AtkTime { get; set; }
    /// <summary>
    /// 敵人受到攻擊
    /// </summary>
    void GetHit();
    /// <summary>
    /// 敵人攻擊
    /// </summary>
    void EnemyHit();
}

     
