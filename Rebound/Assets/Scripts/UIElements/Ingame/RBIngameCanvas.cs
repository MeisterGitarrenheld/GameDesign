using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBIngameCanvas : MonoBehaviour
{

    public static RBIngameCanvas Instance;

    public GameObject CountdownPrefab;
    public GameObject CountdownEndPrefab;

    public GameObject ItemBar;
    public GameObject SkillBar;

    public GameObject ItemPrefab;
    public GameObject SkillPrefab;

    private List<RBItemSlot> _itemSlots = new List<RBItemSlot>();
    private List<RBSkillSlot> _skillSlots = new List<RBSkillSlot>();

    private GameObject _currentCountdown;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //CreateSkillSlots();
        CreateItemSlots();
    }

    public void CreateSkillSlots()
    {
        foreach (var binding in RBAbilityActivityControl.Instance.GetAbilityBinding())
        {
            var slot = Instantiate(SkillPrefab, SkillBar.transform).GetComponent<RBSkillSlot>();
            slot.SetHotkey(binding.Key);
            _skillSlots.Add(slot);
            slot.UpdateSlot(binding.Key, binding.Value);
        }
    }

    public void CreateItemSlots()
    {
        foreach (var binding in RBPowerupActivityControl.Instance.GetPowerupKeyBindings())
        {
            var slot = Instantiate(ItemPrefab, ItemBar.transform).GetComponent<RBItemSlot>();
            slot.SetHotkey(binding.Key);
            _itemSlots.Add(slot);
            slot.UpdateSlot(binding.Key, binding.Value);
        }
    }

    public void SetCountdown(string text, bool end)
    {
        if (_currentCountdown != null)
            Destroy(_currentCountdown);

        if (end)
            _currentCountdown = Instantiate(CountdownEndPrefab, gameObject.transform);
        else
            _currentCountdown = Instantiate(CountdownPrefab, gameObject.transform);

        _currentCountdown.GetComponent<Text>().text = text;
    }
}
