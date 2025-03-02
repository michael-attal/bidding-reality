using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaymentScreenPresenter : MonoBehaviour
{
    [SerializeField] private TMP_InputField numberField;
    [SerializeField] private TMP_InputField cvcField;
    [SerializeField] private TMP_InputField monthField;
    [SerializeField] private TMP_InputField yearField;
    [SerializeField] private Button submitButton;

    private void Start()
        => submitButton.onClick.AddListener(Submit);

    private void Submit()
    {
        StartCoroutine(BackendHandler.PostCardToken(
            numberField.text,
            cvcField.text,
            monthField.text,
            yearField.text
        ));
    }
}
