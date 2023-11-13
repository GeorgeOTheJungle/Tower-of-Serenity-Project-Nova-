using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatNavigation : MonoBehaviour
{
    public static CombatNavigation Instance;
    [Header("Action Window"), Space(10)]
    [SerializeField] private GameObject actionSelectedButton;
    [SerializeField] private GameObject actionsWindow;
    [SerializeField] private GameObject[] menuWindows;
    [SerializeField] private Button[] firstSelected;

    [Header("Command Window"), Space(10)]
    [SerializeField] private GameObject commandSelectedButton;
    [SerializeField] private GameObject commandsWindow;
    [Space(10)]
    [SerializeField] private GameObject shootCommandGO;
    [SerializeField] private GameObject punchCommandGO;

    [Space(10)]
    [SerializeField] private int lastWindow; // READ ONLY

    [SerializeField] private TextMeshPro actionText;

    private PlayerEntity playerEntity;

    private void Awake()
    {
        Instance = this;
        playerEntity = GetComponentInParent<PlayerEntity>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            HandleReturnToLastWindow();
        }
    }
    int buttonIndex;
    int currentWindow;
    public void OpenWindow(int menuIndex)
    {
        
        currentWindow = menuIndex;
        buttonIndex = menuIndex;
        foreach (GameObject menu in menuWindows)
        {
            menu.SetActive(false);
        }

        menuWindows[menuIndex].SetActive(true);

        // Check if is attack window
        if(menuIndex == 1)
        {
            commandsWindow.SetActive(true);
            actionsWindow.SetActive(false);

            //Check player ammo
            if (playerEntity.HasAmmo(1) == false)
            {
                shootCommandGO.SetActive(false);
                punchCommandGO.SetActive(true);
                buttonIndex++;
            }
            else
            {
                shootCommandGO.SetActive(true);
                punchCommandGO.SetActive(false);

            }
        }

        if (menuIndex != 2) firstSelected[buttonIndex].Select();
        else firstSelected[3].Select();
    }

    private void HandleReturnToLastWindow()
    {
        currentWindow--;
        if (currentWindow < 0) currentWindow = 0;
        OpenWindow(currentWindow);
    }

    public void StartCombatWindows()
    {
        OpenWindow(0);
        actionText.SetText("Act");
        actionText.gameObject.SetActive(true);
    }

    public void OnSkillSelected()
    {
        foreach (GameObject menu in menuWindows)
        {
            menu.SetActive(false);
        }
        actionText.SetText(string.Empty);
        actionText.gameObject.SetActive(false);
    }

    public void UpdateActionText(string text)
    {
        if (actionText.IsActive() == false) return;
        actionText.SetText(text);
    }

  
    public void HideAllWindows()
    {
        for(int i = 0;i < menuWindows.Length; i++)
        {
            if (menuWindows[i].activeSelf) lastWindow = i;
            menuWindows[i].SetActive(false);  
        }
    }
}