using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreationPresenter : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField priceField;
    [SerializeField] private TMP_InputField dayField;
    [SerializeField] private TMP_InputField monthField;
    [SerializeField] private TMP_InputField yearField;
    [SerializeField] private TMP_Dropdown userDropdown;

    [SerializeField] private Button createButton;

    private Dictionary<int, string> indexToId = new Dictionary<int, string>();

    private IEnumerator Start()
    {
        yield return BackendHandler.GetUsers(users =>
        {
            var options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < users.Count; ++i)
            {
                indexToId[i] = users[i].id;
                options.Add(new TMP_Dropdown.OptionData(users[i].name));
            }

            userDropdown.ClearOptions();
            userDropdown.AddOptions(options);
        });
        
        createButton.onClick.AddListener(CreateItem);
    }

    void CreateItem()
    {
        DateTime endDate = DateTime.Parse($"{yearField.text}-{monthField.text}-{dayField.text}");
        
        BackendItem item = new BackendItem()
        {
            name = nameField.text,
            startPrice = float.Parse(priceField.text),
            endDate = endDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            userID = indexToId[userDropdown.value],
            isSold = false
        };
        BackendHandler.PostItem(item);
    }
}
