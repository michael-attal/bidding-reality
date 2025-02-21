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
    
    [SerializeField] private GameObject lobbiesUI;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        StartCoroutine(BackendHandler.LoginUser(emailField.text, passwordField.text, res =>
        {
            Debug.Log($"Connected as {res.name}!");
            gameObject.SetActive(false);
            lobbiesUI.SetActive(true);
        }));
    }
}
