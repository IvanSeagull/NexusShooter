using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    private WeaponsManager weaponsManager;
    private float ammoEffectivenessMultiplier = 1.0f;

    [System.Serializable]
    public class Ammo
    {
        public GunData.AmmoType type;
        public int maxAmount;
        public int currAmount;
    }

    public List<Ammo> ammoInventory = new List<Ammo>();

    void Awake()
    {
        weaponsManager = GameObject.FindFirstObjectByType<WeaponsManager>();
        if (weaponsManager == null)
        {
            Debug.LogError("Weapons manager not found.");
        }
    }

    void Start()
    {
        foreach (GunData.AmmoType type in System.Enum.GetValues(typeof(GunData.AmmoType)))
        {
            Ammo ammo = new Ammo();
            ammo.type = type;
            ammo.maxAmount = 100;
            ammo.currAmount = 50;
            ammoInventory.Add(ammo);
        }
    }

    public void SetAmmoEffectivenessMultiplier(float multiplier)
    {
        ammoEffectivenessMultiplier = multiplier;
    }

    public void AddAmmo(GunData.AmmoType type, int amount)
    {
        int adjustedAmount = Mathf.RoundToInt(amount * ammoEffectivenessMultiplier);
        for (int i = 0; i < ammoInventory.Count; i++)
        {
            if (ammoInventory[i].type == type)
            {
                ammoInventory[i].currAmount = Mathf.Min(ammoInventory[i].currAmount + adjustedAmount, ammoInventory[i].maxAmount);
                weaponsManager.UpdateAmmoPanel();
                return;
            }
        }

        Debug.LogError("Could not add ammo");
    }

    public void UseAmmo(GunData.AmmoType type, int amount)
    {
        for (int i = 0; i < ammoInventory.Count; i++)
        {
            if (ammoInventory[i].type == type)
            {
                ammoInventory[i].currAmount = Mathf.Max(ammoInventory[i].currAmount - amount, 0);
                return;
            }
        }

        Debug.LogError("Could not use ammo");
    }

    public int GetAmmo(GunData.AmmoType type)
    {
        for (int i = 0; i < ammoInventory.Count; i++)
        {
            if (ammoInventory[i].type == type)
            {
                return ammoInventory[i].currAmount;
            }
        }

        return 0;
    }
}
