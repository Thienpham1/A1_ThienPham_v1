using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StatManager : MonoBehaviour
{
    [Header("NPC Chat")]
    public TextMeshProUGUI npcChatText;
    private Coroutine reactionCoroutine;
    public GameObject swordObject;
    public GameObject armorObject;
    public GameObject potionObject;

  
   [Header("UI Text References")]
    public TextMeshProUGUI playerStatText;
    public TextMeshProUGUI bossStatText;
    public TextMeshProUGUI itemInfoText;

    private bool hpUpgraded = false;
    private bool atkUpgraded = false;
    private bool defUpgraded = false;

    [Header("UI Panels")]
    public GameObject buyButton;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    public int pHP, pAtk, pDef;
    public int bHP, bAtk, bDef;

    public int PlayerLast, BossLast;

    private string currentGoalItem = ""; 
    private string currentAdviceText = "";
    private ItemData selectedItem;
    private bool skipMessageRequested = false;

    void Start()
    {
        GenerateStats();
        UpdateNPCChat();
        resultPanel.SetActive(false);
        buyButton.SetActive(false);
        
    }

 

    void GenerateStats()
    {
        bHP = Random.Range(150, 200);
        bAtk = Random.Range(30, 45);
        bDef = Random.Range(20, 30);

        float mod = Random.Range(0.7f, 0.9f);
        pHP = Mathf.RoundToInt(bHP * mod);
        pAtk = Mathf.RoundToInt(bAtk * mod);
        pDef = Mathf.RoundToInt(bDef * mod);

        // Initially white or default color
        playerStatText.color = Color.white; 
        UpdateUI();
    }

   

   public void BuyItem()
    {
        if (selectedItem == null) return;

       
        if (selectedItem.healthBonus > 0)
        {
            pHP += selectedItem.healthBonus;
            hpUpgraded = true;
        }
        
        if (selectedItem.attackBonus > 0)
        {
            pAtk += selectedItem.attackBonus;
            atkUpgraded = true;
        }

        if (selectedItem.defenseBonus > 0)
        {
            pDef += selectedItem.defenseBonus;
            defUpgraded = true;
        }

        PlayerLast = pHP + pDef - bAtk;
        BossLast = bHP + bDef - pAtk;

        UpdateUI();
        UpdateNPCChat();
    }
    void UpdateUI()
    {
        
        string greenStart = "<color=#00FF00>";
        string endTag = "</color>";

        
        string hpDisplay = hpUpgraded ? $"{greenStart}{pHP}{endTag}" : pHP.ToString();
        string atkDisplay = atkUpgraded ? $"{greenStart}{pAtk}{endTag}" : pAtk.ToString();
        string defDisplay = defUpgraded ? $"{greenStart}{pDef}{endTag}" : pDef.ToString();

        playerStatText.text = $"<b>PLAYER</b>\n" +
                              $"HP: {hpDisplay}\n" +
                              $"ATK: {atkDisplay}\n" +
                              $"DEF: {defDisplay}";

        bossStatText.text = $"<b>BOSS</b>\nHP: {bHP}\nATK: {bAtk}\nDEF: {bDef}";
    }

    public void StartBattle()
    {
        int playerFinal = (pHP + pDef) - bAtk;
        int bossFinal = (bHP + bDef) - pAtk;

        resultPanel.SetActive(true);
        resultText.text = (playerFinal > bossFinal) ? "VICTORY!" : "DEFEAT!";
        resultText.color = (playerFinal > bossFinal) ? Color.green : Color.red;
    }

    public void Replay() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void Exit() => Application.Quit();

    public void UpdateNPCChat()
    {
       // Calculate Gaps
        bool needsHP = pHP < bHP;
        bool needsAtk = pAtk < bAtk;
        bool needsDef = pDef < bDef;
       

        // Reset visibility of all items first
        swordObject.SetActive(pAtk <= bAtk + 1);
        armorObject.SetActive(pDef <= bDef + 1);
        potionObject.SetActive(pHP <= bHP + 1);

      
        // One-at-a-time Logic (Priority: HP > ATK > DEF)
            if ((pHP + pDef - bAtk) <= (bHP + bDef - pAtk))
            {
                if ((pHP <= bHP + 1) && (pAtk <= bAtk + 1) && (pDef <= bDef + 1))
                {
                    currentGoalItem = "Any";
                    currentAdviceText = "Hello Hero! You are currently weaker than your opponent in every aspect. Buy whatever you want, then I will give you more advice.";
                    
                }
                else if (needsHP)
                {
                    currentGoalItem = "potion";
                    currentAdviceText = "NPC: You look pale. You should buy a <b>Health Potion</b>.";
                }
                else if (needsAtk)
                {
                    currentGoalItem = "sword";
                    currentAdviceText = "You won't leave a scratch on him! Get a <b>Sword</b>.";
                    
                }
                else if (needsDef)
                {
                    currentGoalItem = "armor";
                    currentAdviceText = "That boss hits hard. Better grab some <b>Armor</b>.";
                    
                }
                
            }
            else
            {
                currentAdviceText = "You are strong enough my hero!";
                currentGoalItem = "None";
            }

            if (reactionCoroutine == null)
        {
            npcChatText.text = currentAdviceText;
            npcChatText.color = Color.white;
        }   
       
    }

    public void SelectItem(ItemData data)
    {
        selectedItem = data;
        buyButton.SetActive(true);

       
        if (reactionCoroutine != null) StopCoroutine(reactionCoroutine);
        reactionCoroutine = StartCoroutine(ShowReaction(data));
    }

    public void OnChatBoxClicked()
    {
        skipMessageRequested = true;
    }
    IEnumerator ShowReaction(ItemData data)
    {
        bool isCorrect = data.itemName.Contains(currentGoalItem);

        if ((pHP + pDef - bAtk) <= (bHP + bDef - pAtk))
        {
            if (!isCorrect && currentGoalItem != "None" && currentGoalItem != "Any")
            {
                npcChatText.text = "NPC: Okay! You have the right to choose whatever you want.";
                npcChatText.color = Color.yellow;
            }
            else if(currentGoalItem == "Any")
            {
                npcChatText.text = "NPC: Good choice!";
                npcChatText.color = Color.white;
            }
            else
            {
                npcChatText.text = $"NPC: Good choice! The {data.itemName} will help.";
                npcChatText.color = Color.cyan;
            }
        }
        else
        {
            currentAdviceText = "NPC: You're ready! Stop wasting money and FIGHT!";
            currentGoalItem = "None";
        }
        
        yield return new WaitForSeconds(1f);
       /* float timer = 0;
        while (!skipMessageRequested && timer < 5f) 
        {
            timer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        */
        npcChatText.text = currentAdviceText;
        npcChatText.color = Color.white;
        
        reactionCoroutine = null; 
    }
    
    
}