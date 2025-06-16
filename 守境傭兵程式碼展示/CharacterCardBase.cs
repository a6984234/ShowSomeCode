using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class CharacterCardBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI classNameText;
    [SerializeField] protected Image classImage;
    [SerializeField] protected TextMeshProUGUI atkText;
    [SerializeField] protected TextMeshProUGUI defText;
    [SerializeField] protected TextMeshProUGUI hpText;
    [SerializeField] protected TextMeshProUGUI critText;

    protected CharacterData characterData;
    public CharacterData CharacterData => characterData;

    public virtual async void SetupCard(CharacterData data)
    {
        characterData = data;

        classNameText.text = data.Job.ToString();
        atkText.text = data.Atk.ToString();
        defText.text = data.Def.ToString();
        hpText.text = data.MaxHp.ToString();
        critText.text = data.Crit.ToString();

        string address = ClassIconAddress.GetAddress(data.Job);
        if (!string.IsNullOrEmpty(address) && classImage != null)
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
            Sprite sprite = await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                classImage.sprite = sprite;
            else
                Debug.LogWarning($"Failed to load class icon at address: {address}");
        }
        else
        {
            Debug.LogWarning($"No Addressable path mapped for job: {characterData.Job}");
        }
    }
}


