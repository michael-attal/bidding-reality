using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUIScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;

    [SerializeField] private TMP_Text debugDisplay;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        StartCoroutine(BackendHandler.LoginUser(emailField.text, passwordField.text, res =>
        {
            Debug.Log($"Connected as {res.name}!");
            debugDisplay.text = $"Connected as {res.name}";
        }));
    }
}
