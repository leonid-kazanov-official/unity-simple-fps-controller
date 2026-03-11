using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStaminaController : MonoBehaviour
{
    #region Fields
    [Header("Stamina Seetings")]
    [SerializeField] private float maxStamina;
    [Range(0, 1)][SerializeField] private float runStaminaThresholdProcents;

    [SerializeField] private float staminaFrameRestoraytion;
    [SerializeField] private float staminaFrameSpending;

    public bool StaminaIsRecovering { get; private set; }
    private bool lastRecoveringState;

    [Header("Components")]
    [SerializeField] private PlayerMovementController playerMovementController;

    private float currentStamina;
    public float CurrentStamina
    {
        get { return currentStamina; }
        private set { currentStamina = Math.Clamp(value, 0, maxStamina); }
    }

    private float currentStaminaProgress;
    public float CurrentStaminaProgress
    {
        get { return currentStaminaProgress; }
        private set { currentStaminaProgress = Math.Clamp(value, 0, 1); }
    }
    #endregion



    #region Unity Methods
    void Awake()
    {
        GetComponents();
        CurrentStamina = maxStamina;
    }

    void Update()
    {
        PlayerStaminaUsage();
        StaminaProgressCalculator();

        if (StaminaIsRecovering != lastRecoveringState)
        {
            if (StaminaIsRecovering == true) { playerMovementController.DisableCanSprint(); }
            else { playerMovementController.EnableCanSprint(); }
            lastRecoveringState = StaminaIsRecovering;
        }

    }
    #endregion



    #region Custom Methods
    void StaminaProgressCalculator()
    {
        CurrentStaminaProgress = currentStamina / maxStamina;
    }
    void PlayerStaminaUsage()
    {
        if (playerMovementController.IsSprinting == false)
        {
            float value = staminaFrameRestoraytion * Time.deltaTime;
            StaminaIncrease(value);
        }
        else
        {
            float value = staminaFrameSpending * Time.deltaTime;
            StaminaDecrease(value);
        }

        CheckSprintAbility();
    }
    void CheckSprintAbility()
    {
        if (currentStamina == 0)
        {
            StaminaIsRecovering = true;
        }
        if (currentStamina > maxStamina * runStaminaThresholdProcents)
        {
            StaminaIsRecovering = false;
        }
    }
    void GetComponents()
    {
        // PlayerMovementController
        if (playerMovementController == null)
        {
            playerMovementController = gameObject.GetComponent<PlayerMovementController>();
            if (playerMovementController == null)
            {
                Debug.LogError("Can't find PlayerMovementController component, please add it in playerMovementController field");
            }
        }
    }

    void StaminaIncrease(float value)
    {
        CurrentStamina += value;
    }
    void StaminaDecrease(float value)
    {
        CurrentStamina -= value;
    }
    #endregion
}
