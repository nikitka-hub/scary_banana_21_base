using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
	private void Start()
	{
		Login();
	}

	private void Login()
	{
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
		{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true
		}, OnSuccess, OnError);
	}

	private void OnSuccess(LoginResult result)
	{
		Debug.Log("Successful login/account create!");
		PlayFabPlayerLoggedIn();
		string displayName = PlayerPrefs.GetString("username");
		PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
		{
			DisplayName = displayName
		}, delegate
		{
			Debug.Log("Display Name Changed!");
		}, delegate(PlayFabError error)
		{
			Debug.Log("Error");
			Debug.Log(error.ErrorDetails);
		});
	}

	private void OnError(PlayFabError error)
	{
		Debug.Log("Error while logging in/creating account!");
		if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			Debug.Log("PLAYER IS BANNED");
			SceneManager.LoadScene("Bans");
		}
		Debug.Log(error.GenerateErrorReport());
	}

	public virtual void PlayFabPlayerLoggedIn()
	{
	}
}
