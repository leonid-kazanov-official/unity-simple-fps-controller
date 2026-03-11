using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class UIStaminaBar : MonoBehaviour
{
    #region Fields

    [Header("Stamina Bar Settings")]
    [SerializeField] private Color hightStaminaColor;
    [SerializeField] private Color mediumStaminaColor;
    [SerializeField] private Color lowStaminaColor;
    [SerializeField] private float fadeOutDuration;

    [Header("Components")]
    [SerializeField] private Image staminaBarImage;
    [SerializeField] private PlayerStaminaController playerStaminaController;
    [SerializeField] private RectTransform staminaBarRectTransform;
    [SerializeField] private PlayerMovementController playerMovementController;

    #endregion



    #region Cash Fields
    private Color currentBarColor;
    private Coroutine fadeOutCoroutine;
    private bool fadeOutAlreadyPlayed;
    #endregion



    #region Unity Methods
    void Awake()
    {
        GetComponents();
    }

    void Update()
    {
        ColorUpdate();
        SizeUpdate();
        if (playerMovementController.IsSprinting)
        {
            if (fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }
            SetAplha(1f);
            fadeOutAlreadyPlayed = false;
        }
        else
        {
            if (fadeOutCoroutine == null && fadeOutAlreadyPlayed == false)
            {
                fadeOutCoroutine = StartCoroutine(FadeOut());
            }
        }
    }
    #endregion



    #region Custom Methods
    void SizeUpdate()
    {
        staminaBarRectTransform.localScale = new UnityEngine.Vector3(playerStaminaController.CurrentStaminaProgress, 1f, 1f);
    }

    void ColorUpdate()
    {
        if (playerStaminaController.CurrentStaminaProgress >= 0.5)
        {
            float normilizedProgress = (playerStaminaController.CurrentStaminaProgress - 0.5f) * 2;
            currentBarColor = Color.Lerp(mediumStaminaColor, hightStaminaColor, normilizedProgress);
        }
        else
        {
            float normilizedProgress = playerStaminaController.CurrentStaminaProgress * 2;
            currentBarColor = Color.Lerp(lowStaminaColor, mediumStaminaColor, normilizedProgress);
        }

        float currentAplha = staminaBarImage.color.a;
        currentBarColor.a = currentAplha;
        staminaBarImage.color = currentBarColor;
    }
    void SetAplha(float alpha)
    {
        Color color = staminaBarImage.color;
        color.a = alpha;
        staminaBarImage.color = color;
    }
    #endregion




    #region Coroutines
    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float alpha;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            SetAplha(alpha);
            yield return null;
        }
        fadeOutCoroutine = null;
        fadeOutAlreadyPlayed = true;

    }
    #endregion



    #region GetComponents Method
    void GetComponents()
    {
        // StaminaBar's Image
        if (staminaBarImage == null)
        {
            staminaBarImage = gameObject.GetComponent<Image>();
            if (staminaBarImage == null)
            {
                Debug.LogError("Can't find StaminaBar's Image component, please add it in playerMovementController field");
            }
        }

        // PlayerMovementController
        if (playerMovementController == null)
        {
            playerMovementController = gameObject.GetComponent<PlayerMovementController>();
            if (playerMovementController == null)
            {
                Debug.LogError("Can't find PlayerMovementController component, please add it in playerMovementController field");
            }
        }

        // PlayerStaminaController
        if (playerStaminaController == null)
        {
            playerStaminaController = gameObject.GetComponent<PlayerStaminaController>();
            if (playerStaminaController == null)
            {
                Debug.LogError("Can't find PlayerStaminaController component, please add it in playerMovementController field");
            }
        }

        //StaminaBars's RectTransform
        if (staminaBarRectTransform == null)
        {
            staminaBarRectTransform = staminaBarImage.GetComponent<RectTransform>();
            if (staminaBarRectTransform == null)
            {
                Debug.LogError("Can't find StaminaBar's RectTransform component, please add it in playerMovementController field");
            }
        }
    }
    #endregion

}
