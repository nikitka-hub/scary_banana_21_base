using System.Collections.Generic;
using Photon.Pun;
using Photon.VR.Cosmetics;
using TMPro;
using UnityEngine;

namespace Photon.VR.Player
{
	public class PhotonVRPlayer : MonoBehaviourPun
	{
		[Header("Objects")]
		public Transform Head;

		public Transform Body;

		public Transform LeftHand;

		public Transform RightHand;

		[Tooltip("The objects that will get the colour of the player applied to them")]
		public List<MeshRenderer> ColourObjects;

		[Header("Cosmetics Parents")]
		public Transform HeadCosmetics;

		public Transform FaceCosmetics;

		public Transform BodyCosmetics;

		public Transform LeftHandCosmetics;

		public Transform RightHandCosmetics;

		[Header("Other")]
		public TextMeshPro NameText;

		public bool HideLocalPlayer = true;

		private void Awake()
		{
			if (base.photonView.IsMine)
			{
				PhotonVRManager.Manager.LocalPlayer = this;
				if (HideLocalPlayer)
				{
					Head.gameObject.SetActive(value: false);
					Body.gameObject.SetActive(value: false);
					RightHand.gameObject.SetActive(value: false);
					LeftHand.gameObject.SetActive(value: false);
					NameText.gameObject.SetActive(value: false);
				}
			}
			Object.DontDestroyOnLoad(base.gameObject);
			_RefreshPlayerValues();
		}

		private void Update()
		{
			if (base.photonView.IsMine)
			{
				Head.transform.position = PhotonVRManager.Manager.Head.transform.position;
				Head.transform.rotation = PhotonVRManager.Manager.Head.transform.rotation;
				RightHand.transform.position = PhotonVRManager.Manager.RightHand.transform.position;
				RightHand.transform.rotation = PhotonVRManager.Manager.RightHand.transform.rotation;
				LeftHand.transform.position = PhotonVRManager.Manager.LeftHand.transform.position;
				LeftHand.transform.rotation = PhotonVRManager.Manager.LeftHand.transform.rotation;
			}
		}

		public void RefreshPlayerValues()
		{
			base.photonView.RPC("RPCRefreshPlayerValues", RpcTarget.All);
		}

		[PunRPC]
		private void RPCRefreshPlayerValues()
		{
			_RefreshPlayerValues();
		}

		private void _RefreshPlayerValues()
		{
			if (NameText != null)
			{
				NameText.text = base.photonView.Owner.NickName;
			}
			foreach (MeshRenderer colourObject in ColourObjects)
			{
				if (colourObject != null)
				{
					colourObject.material.color = JsonUtility.FromJson<Color>((string)base.photonView.Owner.CustomProperties["Colour"]);
				}
			}
			PhotonVRCosmeticsData photonVRCosmeticsData = JsonUtility.FromJson<PhotonVRCosmeticsData>((string)base.photonView.Owner.CustomProperties["Cosmetics"]);
			if (HeadCosmetics != null)
			{
				foreach (Transform headCosmetic in HeadCosmetics)
				{
					if (headCosmetic.name != photonVRCosmeticsData.Head)
					{
						headCosmetic.gameObject.SetActive(value: false);
					}
					else
					{
						headCosmetic.gameObject.SetActive(value: true);
					}
				}
			}
			if (BodyCosmetics != null)
			{
				foreach (Transform item in BodyCosmetics.transform)
				{
					if (item.name != photonVRCosmeticsData.Body)
					{
						item.gameObject.SetActive(value: false);
					}
					else
					{
						item.gameObject.SetActive(value: true);
					}
				}
			}
			if (FaceCosmetics != null)
			{
				foreach (Transform item2 in FaceCosmetics.transform)
				{
					if (item2.name != photonVRCosmeticsData.Face)
					{
						item2.gameObject.SetActive(value: false);
					}
					else
					{
						item2.gameObject.SetActive(value: true);
					}
				}
			}
			if (LeftHandCosmetics != null)
			{
				foreach (Transform item3 in LeftHandCosmetics.transform)
				{
					if (item3.name != photonVRCosmeticsData.LeftHand)
					{
						item3.gameObject.SetActive(value: false);
					}
					else
					{
						item3.gameObject.SetActive(value: true);
					}
				}
			}
			if (!(RightHandCosmetics != null))
			{
				return;
			}
			foreach (Transform item4 in RightHandCosmetics.transform)
			{
				if (item4.name != photonVRCosmeticsData.RightHand)
				{
					item4.gameObject.SetActive(value: false);
				}
				else
				{
					item4.gameObject.SetActive(value: true);
				}
			}
		}
	}
}
