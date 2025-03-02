using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XRMultiplayer;

public class ConnectionUIScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    
    [SerializeField] private GameObject lobbiesUI;
    [SerializeField] private GameObject registerUI;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            registerUI.SetActive(true);
        });
    }

    private void OnLoginClicked()
    {
        StartCoroutine(BackendHandler.LoginUser(emailField.text, passwordField.text, res =>
        {
            gameObject.SetActive(false);
            lobbiesUI.SetActive(true);
        }));
    }
}
