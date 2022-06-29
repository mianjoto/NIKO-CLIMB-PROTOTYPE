using System;
using UnityEngine;

public class ShieldUIManager : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [SerializeField] ShieldManager sm;
    
    [SerializeField] GameObject shieldIconPrefab;
    [SerializeField] Sprite brokenShieldSprite;
    GameObject[] Shields;
    int[] validShields;

    public event EventHandler OnShieldAbsorbHit;

    private void Start() {
        InitializeShieldUI();
        sm = gm.Player.GetComponent<PlayerCombat>().Shield.GetComponent<ShieldManager>();
        sm.OnShieldAbsorbHit += ShieldUIManager_OnShieldAbsorbHit;
    }

    private void InitializeShieldUI() {
        // Add shield icons to the UI
        Shields = new GameObject[gm.numberOfShields];
        for (int i = 0; i < Shields.Length; i++)
        {
            Vector2 pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            GameObject instance = Instantiate(shieldIconPrefab, pos, Quaternion.identity);
            instance.transform.SetParent(transform, worldPositionStays: false);
            instance.name = "Shield" + (i+1);
            Shields[i] = instance;
        }
        validShields = new int[Shields.Length];
        Array.Fill(validShields, 1);
    }

    private void ShieldUIManager_OnShieldAbsorbHit(object sender, EventArgs e) {
        // Find first valid shield, searching from right to left
        int validShieldIndex = findValidShieldIndex();
        if (validShieldIndex == -1) 
        {
            return;
        }

        // Replace shield with broken shield
        Shields[validShieldIndex].GetComponent<SpriteRenderer>().sprite = brokenShieldSprite; 

        validShields[validShieldIndex] = 0;
        Debug.Log("Lost shield of index " + validShieldIndex);
    }

    public int findValidShieldIndex()
    {
        for (int i = validShields.Length-1; i >= 0; i--)
        {
            if (validShields[i] == 1)
            {
                return i;
            }
        }
        return -1;
    }


    public void addShield()
    {
        for (int i = 0; i <= validShields.Length; i++)
        {
            if (validShields[i] == 0)
            {
                validShields[i] = 1;
                Shields[i].GetComponent<SpriteRenderer>().sprite = shieldIconPrefab.GetComponent<SpriteRenderer>().sprite;
                return;
            }
        }
        return;
    }

}
