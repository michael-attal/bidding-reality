using System.Collections;
using UnityEngine;

public class BackendTestScript : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return BackendHandler.LoginUser("michael.attal06fr@gmail.com", "KaMK7gjqWRQQ", user =>
        {
            Debug.Log($"Logged in as: {user.name} with ID {user.id}");
        });
    }
}
