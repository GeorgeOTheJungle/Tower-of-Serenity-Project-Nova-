using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerEntity : Entity
{
    [Header("Player Specific Visuals: "), Space(10)]
    [SerializeField] private GameObject commandsVisuals;

    [SerializeField] private float minimumPunchDistance = 0.1f;
    [SerializeField] private float punchMovementSpeed = 20.0f;

    private const string PUNCH_END_ANIMATION = "PunchEnd";
    private CombatNavigation combatNavigation;
    private CombatUI combatUI;
    public override void OnAwake()
    {
        combatNavigation = GetComponentInChildren<CombatNavigation>();
        combatUI = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<CombatUI>();
    }

    public override void OnCombatStart(GameState gameState)
    {
        currentSkill = null;
        switch (gameState)
        {
            case GameState.combatPreparation:
                entityStats = entityData.stats;
                break;
            case GameState.combatReady:
                StartCoroutine(DelayEntrance());
                StartCoroutine(TurnCommandsVisuals(true, 1.0f));
                break;
        }

        UpdateEntityStatsUI();
    }

    public override void OnEntityTurn()
    {
        if (entityStats.defenseBonus > 0.0f)
        {
            PlayAnimation(GUARD_HIT_ANIMATION);
            entityStats.defenseBonus = 0.0f;
        }
        currentSkill = null;
        StartCoroutine(TurnCommandsVisuals(true, 0.0f));
    }

    public override void OnStart()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
        Vector3 originalPlayerPosition = entityVisual.position;

    }

    public override void PerformAction(Skill skill)
    {
        currentSkill = skill;

        // Use resource
        UseResource();
        // Do visuals
        //if (currentSkill.skillName == "Punch") StartCoroutine(PunchMovement(CombatManager.Instance.GetEntityTransform(targetEntity)));
        //else
        PlayAnimation(currentSkill.animationKey);

        // Perform action
        if (currentSkill.baseDamage >= 0) combatNavigation.OnSkillSelected();
        else StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

  
    private IEnumerator PunchMovement(Transform target)
    {
        // Play travel animation here
        while(Vector3.Distance(entityVisual.position, target.position) > minimumPunchDistance)
        {
            entityVisual.position = Vector3.MoveTowards(entityVisual.position,target.position, punchMovementSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        PlayAnimation(PUNCH_END_ANIMATION);
    }

    public override void TargetEntity(int entitySlot) // Set entity to attack and deal damage
    {
        targetEntity = entitySlot;
        PerformAction(preSelectedSkill);
       // CombatManager.Instance.ActivateTargets();
    }
    public override void AttackEntity()
    {
        // Once the player targets the enemy
        // Perform action.
        // Deal damage.
        // Pass to next turn.
        CombatManager.Instance.entityList[targetEntity].OnDamageTaken(CalculateDamageDealt(), currentSkill.damageType);
    }

    protected override void UpdateEntityStatsUI()
    {
        combatUI.UpdateCombatStats();
    }

    public override void CombatUICleanUp()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    private void UseResource()
    {
        switch (currentSkill.resourceType)
        {
            case ResourceType.ammo:
                entityStats.ammo -= currentSkill.resourceAmount;
                if (entityStats.ammo <= 0)
                {
                    entityStats.ammo = 0;
                    // Here call the change of the command from shoot to hit.
                }
                break;
            case ResourceType.energy:
                entityStats.energy -= currentSkill.resourceAmount;
                break;
        }
        combatUI.UpdateCombatStats();
    }

    protected override void UpdateEntityUI(bool active)
    {
        commandsVisuals.SetActive(active);

        if(active) combatNavigation.StartCombatWindows();
    }

    #region Resource Methods

    public bool HasAmmo(int amount)
    {
        return entityStats.ammo >= amount;
    }

    public bool HasEnergy(int energy)
    {
        return entityStats.energy >= energy;
    }

    public override void MoveEntityToTarget()
    {
        StartCoroutine(PunchMovement(CombatManager.Instance.GetEntityTransform(targetEntity)));
    }
    #endregion

    // TODO MAKE THE PUNCH MOVEMENT NOT ANIMATION DEPENDANT.
}