using Enums;
using Structs;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerEntity : Entity
{
    [Header("Player Specific Visuals: "), Space(10)]
    [SerializeField] private GameObject commandsVisuals;

    [SerializeField] private float minimumPunchDistance = 0.1f;
    [SerializeField] private float punchMovementSpeed = 20.0f;

    private PlayerSkill currentSkill;
    private PlayerSkill preSelectedSkill;

    private const string PUNCH_END_ANIMATION = "PunchEnd";
    private CombatNavigation combatNavigation;
    private CombatUI combatUI;
    private int lastAmmo;
    public override void OnAwake()
    {
        combatNavigation = GameObject.FindGameObjectWithTag("CombatNavigation").GetComponent<CombatNavigation>();
        commandsVisuals = combatNavigation.gameObject;
        combatUI = GameObject.FindGameObjectWithTag("CombatUI").GetComponent<CombatUI>();
    }

    public override void OnCombatStart(GameState gameState)
    {
        currentSkill = null;
        onFireEffect.RemoveEffect();
        entityState = EntityState.idle;
 
        switch (gameState)
        {
            case GameState.combatPreparation:
                //entityStats = entityData.stats;
                entityStats = PlayerStatsManager.Instance.GetPlayerStats();
                StartCoroutine(TurnCommandsVisuals(true, 0.0f));
                PlayAnimation(Constants.IDLE_OUT);
                break;
            case GameState.combatReady:
              
                StartCoroutine(DelayEntrance());

                break;
        }

        UpdateEntityStatsUI(false);
    }

    public override void OnEntityTurn()
    {
        if (entityState == EntityState.dead) return;
        if (entityStats.defenseBonus > 0.0f)
        {
            PlayAnimation(Constants.GUARD_END_ANIMATION);
            entityStats.defenseBonus = 0.0f;
        }
        if (buffDuration > 0) buffDuration--;
        if (buffDuration == 0) entityStats.buffBonus = 0.0f;
        CombatManager.Instance.IsPlayerTurn(true);
        entityState = EntityState.thinking;
        currentSkill = null;
        StartCoroutine(TurnCommandsVisuals(true, 0.0f));
    }

    public override void OnStart()
    {
        //StartCoroutine(TurnCommandsVisuals(false, 0.0f));
        Vector3 originalPlayerPosition = entityVisual.position;
    }

    public override void PerformAction(PlayerSkill skill)
    {
        currentSkill = skill;
        entityState = EntityState.acting;
        // Use resource
        UseResource();
        // Do visuals
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
        if(entitySlot != -1)
        {
            targetEntity = entitySlot;
        } else
        {
            targetEntity = -1;
        }

        PerformAction(preSelectedSkill);
    }
    public override void AttackEntity()
    {
        float damageMultiplier = currentSkill.damageType == DamageType.physical ? entityStats.physicalDamage : entityStats.magicDamage;

        int damage = CalculateDamageDealt(damageMultiplier,currentSkill.baseDamage);

        // Check if its crit or not
        bool isCrit = UnityEngine.Random.Range(0.0f, 1.0f) < entityStats.critRate;
        damage *= Mathf.CeilToInt(isCrit ? 2.5f : 1.0f);
        if (targetEntity == -1)
        {
            CombatManager.Instance.AttackAllEntities(damage, currentSkill.damageType, isCrit);
        } else
        {
            CombatManager.Instance.entityList[targetEntity].OnDamageTaken(damage, currentSkill.damageType,currentSkill.statusEffectType, isCrit);
        }

    }

    protected override void UpdateEntityStatsUI(bool healthHit)
    {
        combatUI.UpdateCombatStats(healthHit, false);
        if (lastAmmo < entityStats.ammo)
        {
            lastAmmo = entityStats.ammo;
            combatUI.OnReload();
        }
    }

    public override void CombatUICleanUp()
    {
        StartCoroutine(TurnCommandsVisuals(false, 0.0f));
    }

    protected override void UpdateEntityUI(bool active)
    {
        commandsVisuals.SetActive(active);

        if(active) combatNavigation.StartCombatWindows();
    }

    public override void MoveEntityToTarget()
    {
        StartCoroutine(PunchMovement(CombatManager.Instance.GetEntityTransform(targetEntity)));
    }
    public virtual void PreSelectSkill(PlayerSkill skill)
    {
        preSelectedSkill = skill;
    }

    public override void OnHeal()
    {
        // base.OnHeal(baseDamage);
        float baseHealing = currentSkill.baseDamage;
        OnResourceGain(ResourceType.health, CalculateHealing(Mathf.CeilToInt(baseHealing)), RegenStyle.None);
        base.OnHeal();
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

    private void UseResource()
    {
        switch (currentSkill.resourceType)
        {
            case ResourceType.ammo:
                lastAmmo = entityStats.ammo;
                entityStats.ammo -= currentSkill.resourceAmount;
                combatUI.OnShot();
                if (entityStats.ammo <= 0)
                {
                    entityStats.ammo = 0;
                    // Here call the change of the command from shoot to hit.
                }
                break;
            case ResourceType.energy:
                entityStats.energy -= currentSkill.resourceAmount;
                combatUI.UpdateCombatStats(false, true);
                break;
        }

    }

    public void GetResource(ResourceType resourceType,int amount)
    {
        if(currentSkill.skillName == "Punch")
        {
            amount *= 2;
        }

        switch(resourceType)
        {
            case ResourceType.ammo:
                entityStats.ammo += amount;
                if (entityStats.ammo > entityData.stats.ammo) entityStats.ammo = entityData.stats.ammo;

                break;
            case ResourceType.energy:
                entityStats.energy += amount;
                if (entityStats.energy > entityData.stats.energy) entityStats.energy = entityData.stats.energy;
                break;

        }
    }

    #endregion

    // TODO MAKE THE PUNCH MOVEMENT NOT ANIMATION DEPENDANT.
}
