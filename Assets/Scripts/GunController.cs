using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun[] allGuns;

    public TextMeshProUGUI gunNameText;
    public float gunNameDisplayTime = 1f;

    List<Gun> equippedGuns = new List<Gun>();
    int currentWeaponIndex = 0;

    void Start()
    {
        EquipStartingGun();
    }

    void EquipStartingGun()
    {
        EquipGun(allGuns[0]);
    }

    public void EquipGun(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < allGuns.Length)
        {
            currentWeaponIndex = weaponIndex;
            EquipGun(allGuns[currentWeaponIndex]);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        Gun newGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        newGun.transform.parent = weaponHold;
        equippedGuns.Add(newGun);
        gunNameText.text = newGun.name;
        StartCoroutine(HideGunNameText());

        foreach (Gun gun in equippedGuns)
        {
            gun.gameObject.SetActive(false);
        }

        equippedGuns[currentWeaponIndex].gameObject.SetActive(true);
    }

    IEnumerator HideGunNameText()
    {
        yield return new WaitForSeconds(gunNameDisplayTime);
        gunNameText.text = "";
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equippedGuns.Count > 0)
        {
            equippedGuns[currentWeaponIndex].Aim(aimPoint);
        }
    }

    public void OnTriggerHold()
    {
        if (equippedGuns.Count > 0)
        {
            equippedGuns[currentWeaponIndex].OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equippedGuns.Count > 0)
        {
            equippedGuns[currentWeaponIndex].OnTriggerRelease();
        }
    }

    public float GunHeight
    {
        get { return weaponHold.position.y; }
    }

    public void Reload()
    {
        if (equippedGuns.Count > 0)
        {
            equippedGuns[currentWeaponIndex].Reload();
        }
    }

    public void SwitchWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % equippedGuns.Count;

        for (int i = 0; i < equippedGuns.Count; i++)
        {
            equippedGuns[i].gameObject.SetActive(i == currentWeaponIndex);
        }
        gunNameText.text = equippedGuns[currentWeaponIndex].name;
        StartCoroutine(HideGunNameText());
    }
}
