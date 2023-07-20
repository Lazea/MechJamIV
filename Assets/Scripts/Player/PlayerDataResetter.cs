using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataResetter : MonoBehaviour
{
    public PlayerData playerData;

    // Start is called before the first frame update
    void Awake()
    {
        playerData.ResetData();
        playerData.ResetCredits();
        playerData.ResetKillCount();
        playerData.ResetStageCount();
        playerData.ResetWeaponData();
    }
}
