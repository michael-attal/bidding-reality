using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreenPresenter : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button registerButton;
    
    [SerializeField] private GameObject lobbiesUI;

    private void Start()
        => registerButton.onClick.AddListener(Register);

    private void Register()
    {
        BackendUser user = new()
        {
            name = nameField.text,
            email = emailField.text,
            password = passwordField.text,
            role = "player"
        };
        BackendHandler.PostUser(user, res =>
        {
            gameObject.SetActive(false);
            lobbiesUI.SetActive(true);
        });
    }
}
