using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playfablogin : MonoBehaviour
{
	[Header("COSMETICS")]
	public static Playfablogin instance;

	public string MyPlayFabID;

	public string CatalogName;

	public List<GameObject> specialitems;

	public List<GameObject> disableitems;

	[Header("CURRENCY")]
	public string CurrencyName;

	public TextMeshPro currencyText;

	[SerializeField]
	public int coins;

	[Header("BANNED")]
	public string bannedscenename;

	[Header("TITLE DATA")]
	public TextMeshPro MOTDText;

	[Header("PLAYER DATA")]
	public TextMeshPro UserName;

	public string StartingUsername;

	public new string name;

	[SerializeField]
	public bool UpdateName;

	public void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		StartCoroutine(LoginRoutine());
	}

	private IEnumerator LoginRoutine()
	{
		while (true)
		{
			login();
			yield return new WaitForSeconds(3f);
		}
	}

	public void login()
	{
		PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
		{
			CustomId = SystemInfo.deviceUniqueIdentifier,
			CreateAccount = true,
			InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
			{
				GetPlayerProfile = true
			}
		}, OnLoginSuccess, OnError);
	}

	public void OnLoginSuccess(LoginResult result)
	{
		Debug.Log("logging in");
		PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), AccountInfoSuccess, OnError);
		GetVirtualCurrencies();
		GetMOTD();
		StopCoroutine(LoginRoutine());
	}

	public void AccountInfoSuccess(GetAccountInfoResult result)
	{
		MyPlayFabID = result.AccountInfo.PlayFabId;
		PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult getUserInventoryResult)
		{
			foreach (ItemInstance item in getUserInventoryResult.Inventory)
			{
				if (item.CatalogVersion == CatalogName)
				{
					for (int i = 0; i < specialitems.Count; i++)
					{
						if (specialitems[i].name == item.ItemId)
						{
							specialitems[i].SetActive(value: true);
						}
					}
					for (int j = 0; j < disableitems.Count; j++)
					{
						if (disableitems[j].name == item.ItemId)
						{
							disableitems[j].SetActive(value: false);
						}
					}
				}
			}
		}, delegate(PlayFabError error)
		{
			Debug.LogError(error.GenerateErrorReport());
		});
	}

	private async void Update()
	{
	}

	public void GetVirtualCurrencies()
	{
		PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
	}

	private void OnGetUserInventorySuccess(GetUserInventoryResult result)
	{
		coins = result.VirtualCurrency["HS"];
		currencyText.text = "You have " + coins + " " + CurrencyName;
	}

	private void OnError(PlayFabError error)
	{
		if (error.Error == PlayFabErrorCode.AccountBanned)
		{
			SceneManager.LoadScene(bannedscenename);
		}
	}

	public void GetMOTD()
	{
		PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), MOTDGot, OnError);
	}

	public void MOTDGot(GetTitleDataResult result)
	{
		if (result.Data == null || !result.Data.ContainsKey("MOTD"))
		{
			Debug.Log("No MOTD");
		}
		else
		{
			MOTDText.text = result.Data["MOTD"];
		}
	}
}
