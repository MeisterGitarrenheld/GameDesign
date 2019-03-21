using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBIngameCanvas : MonoBehaviour {

    public static RBIngameCanvas Instance;

    public GameObject ItemBar;
    public GameObject SkillBar;

    public GameObject ItemPrefab;
    public GameObject SkillPrefab;

    private List<RBItemSlot> _itemSlots = new List<RBItemSlot>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateItemSlots();
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
}
