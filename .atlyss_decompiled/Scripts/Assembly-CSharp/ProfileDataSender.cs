using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using Steamworks;
using UnityEngine;

public class ProfileDataSender : MonoBehaviour
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass9_0
	{
		public PlayerProfileDataMessage _message;

		public NetworkConnectionToClient _conn;

		public string _address;
	}

	[CompilerGenerated]
	private sealed class _003CAssign_PlayerData_003Ed__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public PlayerProfileDataMessage _message;

		public NetworkConnectionToClient _conn;

		private _003C_003Ec__DisplayClass9_0 _003C_003E8__1;

		public ProfileDataSender _003C_003E4__this;

		private GameObject _003C_newPlayerObject_003E5__2;

		private Player _003C_player_003E5__3;

		private PlayerInventory _003C_pInventory_003E5__4;

		private PlayerCombat _003C_pCombat_003E5__5;

		private PlayerStats _003C_pStats_003E5__6;

		private PlayerVisual _003C_pVisual_003E5__7;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CAssign_PlayerData_003Ed__9(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			ProfileDataSender profileDataSender = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003C_003E8__1._message = _message;
				_003C_003E8__1._conn = _conn;
				_003C_003E8__1._address = _003C_003E8__1._conn.address;
				if (!_003CAssign_PlayerData_003Eg__Can_ConnectToHost_007C9_0(ref _003C_003E8__1))
				{
					return false;
				}
				_003C_003E2__current = new WaitForSeconds(0.25f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				_003C_newPlayerObject_003E5__2 = UnityEngine.Object.Instantiate(profileDataSender._nM.playerPrefab);
				_003C_player_003E5__3 = _003C_newPlayerObject_003E5__2.GetComponent<Player>();
				_003C_pInventory_003E5__4 = _003C_player_003E5__3._pInventory;
				_003C_pCombat_003E5__5 = _003C_player_003E5__3._pCombat;
				_003C_pStats_003E5__6 = _003C_player_003E5__3._pStats;
				_003C_pVisual_003E5__7 = _003C_player_003E5__3._pVisual;
				_ = _003C_player_003E5__3._chatBehaviour;
				_003C_player_003E5__3.Network_steamID = _003C_003E8__1._conn.address;
				_003C_player_003E5__3.Network_currentGameCondition = GameCondition.LOADING_GAME;
				_003C_player_003E5__3.Network_currentPlayerCondition = PlayerCondition.CONNECTING;
				NetworkServer.AddPlayerForConnection(_003C_003E8__1._conn, _003C_newPlayerObject_003E5__2);
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 2;
				return true;
			case 2:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				profileDataSender.Assign_PlayerStats(_003C_player_003E5__3, _003C_pStats_003E5__6, _003C_003E8__1._message);
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 3;
				return true;
			case 3:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				profileDataSender.Assign_PlayerVisual(_003C_pVisual_003E5__7, _003C_003E8__1._message);
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 4;
				return true;
			case 4:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				profileDataSender.Assign_PlayerInventory(_003C_pInventory_003E5__4, _003C_003E8__1._message);
				_003C_pCombat_003E5__5.Network_isUsingAltWeapon = _003C_003E8__1._message._isAltWeaponEquipped;
				_003C_pCombat_003E5__5.Network_lockWeapon = _003C_003E8__1._message._lockWeapon;
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 5;
				return true;
			case 5:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				if ((bool)_003C_pStats_003E5__6)
				{
					_003C_pStats_003E5__6.Apply_StatCalculations();
					_003C_003E2__current = new WaitForSeconds(0.45f);
					_003C_003E1__state = 6;
					return true;
				}
				return false;
			case 6:
				_003C_003E1__state = -1;
				if (_003C_003E8__1._conn == null)
				{
					return false;
				}
				if (!_003C_newPlayerObject_003E5__2)
				{
					return false;
				}
				_003C_003E2__current = new WaitForSeconds(0.45f);
				_003C_003E1__state = 7;
				return true;
			case 7:
				_003C_003E1__state = -1;
				HostConsole._current.Create_PeerListEntry(_003C_003E8__1._conn.connectionId, _003C_player_003E5__3);
				if (!AtlyssNetworkManager._current._soloMode)
				{
					HostConsole._current.Init_ServerMessage(_003C_player_003E5__3._nickname + " connected.");
				}
				_003C_newPlayerObject_003E5__2.name = $"{_003C_player_003E5__3._nickname} (connID: {_003C_003E8__1._conn.connectionId})";
				if (_003C_003E8__1._message._isNewCharacter)
				{
					_003C_003E2__current = new WaitForSeconds(5f);
					_003C_003E1__state = 8;
					return true;
				}
				break;
			case 8:
				_003C_003E1__state = -1;
				break;
			}
			if (profileDataSender._nM._mapInstanceManager._loadedInstances.Count <= 0)
			{
				profileDataSender._nM._mapInstanceManager.Init_LoadSubScene(profileDataSender._nM._mapInstanceManager._hubMap);
			}
			profileDataSender._nM._mapInstanceManager.Init_SendPeer_ToScene(_003C_player_003E5__3, string.Empty, profileDataSender._nM._mapInstanceManager._hubMap, profileDataSender._nM._mapInstanceManager._hubMainSpawnPointTag, ZoneDifficulty.NORMAL);
			if (SteamManager.Initialized && _003C_player_003E5__3.isLocalPlayer && _003C_player_003E5__3._isHostPlayer)
			{
				_003C_player_003E5__3.Network_steamID = SteamUser.GetSteamID().ToString();
			}
			return false;
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[CompilerGenerated]
	private sealed class _003CInit_ClientConnectionToServer_003Ed__6 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public ProfileDataSender _003C_003E4__this;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CInit_ClientConnectionToServer_003Ed__6(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			ProfileDataSender profileDataSender = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				goto IL_0046;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0046;
			case 2:
				_003C_003E1__state = -1;
				profileDataSender.OnSend_CharacterFileData();
				_003C_003E2__current = new WaitForSeconds(1.5f);
				_003C_003E1__state = 3;
				return true;
			case 3:
				{
					_003C_003E1__state = -1;
					profileDataSender._isSendingData = false;
					return false;
				}
				IL_0046:
				if (!NetworkClient.ready)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003E2__current = new WaitForSeconds(0.35f);
				_003C_003E1__state = 2;
				return true;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	private AtlyssNetworkManager _nM;

	private bool _isSendingData;

	private BitArray connids_handled = new BitArray(128);

	private void Awake()
	{
		_nM = GetComponent<AtlyssNetworkManager>();
	}

	public void Init_RegisterProfileMessage()
	{
		NetworkServer.RegisterHandler<PlayerProfileDataMessage>(OnRecieve_PlayerProfileData);
	}

	[ClientCallback]
	public void Client_SendCharacterFileData()
	{
		if (NetworkClient.active && !_isSendingData)
		{
			StartCoroutine(Init_ClientConnectionToServer());
			LoadSceneManager._current.Init_LoadScreen();
			_isSendingData = true;
		}
	}

	[IteratorStateMachine(typeof(_003CInit_ClientConnectionToServer_003Ed__6))]
	[ClientCallback]
	private IEnumerator Init_ClientConnectionToServer()
	{
		if (!NetworkClient.active)
		{
			return null;
		}
		return new _003CInit_ClientConnectionToServer_003Ed__6(0)
		{
			_003C_003E4__this = this
		};
	}

	[ClientCallback]
	private void OnSend_CharacterFileData()
	{
		if (NetworkClient.active)
		{
			ProfileDataManager current = ProfileDataManager._current;
			PlayerProfileDataMessage message = default;
			CharacterFile characterFile = new CharacterFile
			{
				_nickName = current._characterFile._nickName,
				_version = current._characterFile._version,
				_isNewCharacter = current._characterFile._isNewCharacter,
				_currency = current._characterFile._currency,
				_isAltWeaponEquipped = current._characterFile._isAltWeaponEquipped,
				_isWeaponLocked = current._characterFile._isWeaponLocked,
				_vanityStruct = current._characterFile._vanityStruct,
				_statsProfile = current._characterFile._statsProfile,
				_appearanceProfile = current._characterFile._appearanceProfile
			};
			message._nickName = characterFile._nickName;
			message._version = characterFile._version;
			message._isNewCharacter = characterFile._isNewCharacter;
			message._currency = characterFile._currency;
			message._isAltWeaponEquipped = characterFile._isAltWeaponEquipped;
			message._lockWeapon = characterFile._isWeaponLocked;
			message._vanityStruct = characterFile._vanityStruct;
			message._statsProfile = characterFile._statsProfile;
			message._appearanceProfile = characterFile._appearanceProfile;
			message._globalNickname = MainMenuManager._current._characterSelectManager._globalNickanmeInput.text;
			message._password = AtlyssNetworkManager._current._sentPassword;
			NetworkClient.connection.Send(message);
		}
	}

	private void OnRecieve_PlayerProfileData(NetworkConnectionToClient _conn, PlayerProfileDataMessage _message)
	{
		int connectionId = _conn.connectionId;
		if (connectionId >= connids_handled.Length)
		{
			connids_handled.Length *= 2;
		}
		if (!connids_handled[connectionId])
		{
			connids_handled[connectionId] = true;
			if (NetworkServer.active)
			{
				StartCoroutine(Assign_PlayerData(_conn, _message));
			}
		}
	}

	[IteratorStateMachine(typeof(_003CAssign_PlayerData_003Ed__9))]
	[ServerCallback]
	private IEnumerator Assign_PlayerData(NetworkConnectionToClient _conn, PlayerProfileDataMessage _message)
	{
		if (!NetworkServer.active)
		{
			return null;
		}
		return new _003CAssign_PlayerData_003Ed__9(0)
		{
			_003C_003E4__this = this,
			_conn = _conn,
			_message = _message
		};
	}

	[ServerCallback]
	private void Assign_PlayerStats(Player _player, PlayerStats _pStats, PlayerProfileDataMessage _message)
	{
		if (!NetworkServer.active || !_pStats)
		{
			return;
		}
		PlayerStats_Profile statsProfile = _message._statsProfile;
		string _setGlobalNickname = _message._globalNickname;
		string _setNickName = _message._nickName;
		if (Check_IfInvalidName(_setGlobalNickname))
		{
			_setGlobalNickname = "";
		}
		if (Check_IfInvalidName(_setNickName))
		{
			_setNickName = "Null";
		}
		_player.Network_globalNickname = _setGlobalNickname;
		_player.Network_nickname = _setNickName;
		int num = statsProfile._currentLevel;
		int num2 = statsProfile._currentExp;
		if (num > GameManager._current._statLogics._maxMainLevel)
		{
			num = GameManager._current._statLogics._maxMainLevel;
		}
		if (num <= 0)
		{
			num = 1;
		}
		if ((float)num2 >= GameManager._current._statLogics._experienceCurve.Evaluate(num))
		{
			num2 = (int)GameManager._current._statLogics._experienceCurve.Evaluate(num) - 5;
		}
		_pStats.Network_currentLevel = num;
		_pStats.Network_currentExp = num2;
		if (statsProfile._classID != string.Empty)
		{
			_pStats.Network_syncClass = statsProfile._classID;
		}
		_pStats.Network_syncClassTier = statsProfile._classTier;
		int num3 = statsProfile._attributePoints;
		int num4 = (num - 1) * GameManager._current._statLogics._attributePointsEachLevel;
		num4 += 6;
		if (num3 > num4)
		{
			num3 = num4;
			if (num == 1)
			{
				num3 = 6;
			}
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		_pStats.Network_currentAttributePoints = num3;
		_pStats.Init_AttributeListing();
		int num5 = 0;
		bool flag = false;
		for (int i = 0; i < _message._statsProfile._attributes.Length; i++)
		{
			num5 += _message._statsProfile._attributes[i]._attributeValue;
		}
		for (int j = 0; j < _message._statsProfile._attributes.Length; j++)
		{
			int num6 = _message._statsProfile._attributes[j]._attributeValue;
			if (num6 < 1)
			{
				num6 = 1;
			}
			if (flag)
			{
				num6 = 1;
			}
			if (num5 > num4 + 4 && !flag)
			{
				num6 = num4;
				num5 = 0;
				flag = true;
			}
			AttributeStruct value = new AttributeStruct
			{
				_attributeID = _message._statsProfile._attributes[j]._attributeID,
				_attributeName = _message._statsProfile._attributes[j]._attributeName,
				_attributeValue = num6
			};
			_pStats._syncAttributes[j] = value;
		}
		bool flag2 = false;
		if (_message._version != Application.version && GameManager._current._resetAttributePointsOnNewVersion)
		{
			for (int k = 0; k < GameManager._current._versionIgnoreResetList.Length; k++)
			{
				if (_message._version == GameManager._current._versionIgnoreResetList[k])
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				_pStats.Server_ResetAttributePoints();
			}
		}
		_pStats.Init_ProfessionListing();
		if (_message._statsProfile._professions == null || _message._statsProfile._professions.Length == 0)
		{
			return;
		}
		for (int l = 0; l < _message._statsProfile._professions.Length; l++)
		{
			if (!(GameManager._current._statLogics._scriptableProfessions[l] == null))
			{
				ScriptableProfession scriptableProfession = GameManager._current._statLogics._scriptableProfessions[l];
				ProfessionStruct value2 = new ProfessionStruct
				{
					_professionID = l,
					_professionLvl = _message._statsProfile._professions[l]._professionLvl,
					_professionXP = _message._statsProfile._professions[l]._professionXP
				};
				if (value2._professionLvl <= 0)
				{
					value2._professionLvl = 1;
				}
				else if (value2._professionLvl > scriptableProfession._maxProfessionLevel)
				{
					value2._professionLvl = scriptableProfession._maxProfessionLevel;
				}
				if (value2._professionXP < 0)
				{
					value2._professionXP = 0;
				}
				_pStats._syncProfessions[l] = value2;
			}
		}
		bool Check_IfInvalidName(string _input)
		{
			if (GameManager._current.ContainsUnicodeCharacter(_input))
			{
				return true;
			}
			if (string.IsNullOrWhiteSpace(_input))
			{
				return true;
			}
			if (_setGlobalNickname.Contains('@'))
			{
				return true;
			}
			if (_setGlobalNickname.Contains('<'))
			{
				return true;
			}
			if (_setNickName.Contains('>'))
			{
				return true;
			}
			if (_setGlobalNickname.Contains('/'))
			{
				return true;
			}
			if (_setGlobalNickname.Length > 18)
			{
				return true;
			}
			return false;
		}
	}

	[ServerCallback]
	private void Assign_PlayerInventory(PlayerInventory _pInventory, PlayerProfileDataMessage _message)
	{
		if (NetworkServer.active && (bool)_pInventory)
		{
			_pInventory.Network_heldCurrency = _message._currency;
			_pInventory.GetComponent<PlayerEquipment>().Network_vanitySyncStruct = _message._vanityStruct;
		}
	}

	[ServerCallback]
	private void Assign_PlayerVisual(PlayerVisual _pVisual, PlayerProfileDataMessage _message)
	{
		if (!NetworkServer.active || !_pVisual)
		{
			return;
		}
		PlayerAppearance_Profile appearanceProfile = _message._appearanceProfile;
		ScriptablePlayerRace scriptablePlayerRace = GameManager._current.Locate_PlayerRace(_message._appearanceProfile._setRaceTag);
		if (!scriptablePlayerRace)
		{
			UnityEngine.Debug.LogError("Scriptable Race File not found in Database: " + _message._appearanceProfile._setRaceTag + ". Is it missing?");
			return;
		}
		PlayerAppearance_Profile playerAppearance_Profile = scriptablePlayerRace.Init_ParamsCheck(appearanceProfile);
		PlayerAppearanceStruct network_playerAppearanceStruct = new PlayerAppearanceStruct
		{
			_setRaceTag = playerAppearance_Profile._setRaceTag,
			_textureID = playerAppearance_Profile._setTexture,
			_bodyColorHue = playerAppearance_Profile._setSkinColorProfile._hue,
			_bodyColorBrightness = playerAppearance_Profile._setSkinColorProfile._brightness,
			_bodyColorContrast = playerAppearance_Profile._setSkinColorProfile._contrast,
			_bodyColorSaturation = playerAppearance_Profile._setSkinColorProfile._saturation,
			_hairStyleHue = playerAppearance_Profile._hairColorProfile._hue,
			_hairStyleBrightness = playerAppearance_Profile._hairColorProfile._brightness,
			_hairStyleContrast = playerAppearance_Profile._hairColorProfile._contrast,
			_hairStyleSaturation = playerAppearance_Profile._hairColorProfile._saturation,
			_hairIsBodyColor = playerAppearance_Profile._hairIsBodyColor,
			_miscHue = playerAppearance_Profile._miscColorProfile._hue,
			_miscBrightness = playerAppearance_Profile._miscColorProfile._brightness,
			_miscContrast = playerAppearance_Profile._miscColorProfile._contrast,
			_miscSaturation = playerAppearance_Profile._miscColorProfile._saturation,
			_dyeIndex = playerAppearance_Profile._helmDyeIndex,
			_headWidth = playerAppearance_Profile._headWidth,
			_muzzleWeight = playerAppearance_Profile._muzzleWeight,
			_hairStyleID = appearanceProfile._setHairStyle,
			_miscID = appearanceProfile._setMisc,
			_mouthID = appearanceProfile._setMouth,
			_earID = appearanceProfile._setEar,
			_eyeID = appearanceProfile._setEye,
			_voicePitch = playerAppearance_Profile._voicePitch,
			_heightWeight = playerAppearance_Profile._heightWeight,
			_widthWeight = playerAppearance_Profile._widthWeight,
			_torsoWeight = playerAppearance_Profile._torsoWeight,
			_displayBoobs = playerAppearance_Profile._displayBoobs,
			_boobWeight = playerAppearance_Profile._boobWeight,
			_armWeight = playerAppearance_Profile._armWeight,
			_bellyWeight = playerAppearance_Profile._bellyWeight,
			_bottomWeight = playerAppearance_Profile._bottomWeight,
			_isLeftHanded = playerAppearance_Profile._isLeftHanded,
			_tailID = appearanceProfile._setTail,
			_hideHelm = appearanceProfile._hideHelm,
			_hideCape = appearanceProfile._hideCape,
			_hideChest = appearanceProfile._hideChest,
			_hideLeggings = appearanceProfile._hideLeggings,
			_hideVanityHelm = appearanceProfile._hideVanityHelm,
			_hideVanityCape = appearanceProfile._hideVanityCape,
			_hideVanityChest = appearanceProfile._hideVanityChest,
			_hideVanityLeggings = appearanceProfile._hideVanityLeggings
		};
		_pVisual.Network_playerAppearanceStruct = network_playerAppearanceStruct;
		if ((bool)_pVisual.GetComponent<PlayerSound>())
		{
			_pVisual.GetComponent<PlayerSound>().Network_voicePitch = appearanceProfile._voicePitch;
		}
	}

	[CompilerGenerated]
	internal static bool _003CAssign_PlayerData_003Eg__Can_ConnectToHost_007C9_0(ref _003C_003Ec__DisplayClass9_0 P_0)
	{
		if (AtlyssNetworkManager._current._soloMode)
		{
			return true;
		}
		if (P_0._message._version == string.Empty)
		{
			HostConsole._current.New_LogMessage($"kicked connection ID {P_0._conn.connectionId} - no version key sent.");
			P_0._conn.Disconnect();
			return false;
		}
		if (SteamManager.Initialized)
		{
			for (int i = 0; i < AtlyssNetworkManager._current._bannedClientList.Count; i++)
			{
				if (AtlyssNetworkManager._current._bannedClientList[i]._steamID == P_0._address)
				{
					HostConsole._current.New_LogMessage($"kicked connection ID {P_0._conn.connectionId} - banned.");
					P_0._conn.Disconnect();
					return false;
				}
			}
			ulong.TryParse(P_0._address, out var result);
			if (SteamFriends.HasFriend(new CSteamID(result), EFriendFlags.k_EFriendFlagImmediate))
			{
				return true;
			}
		}
		bool flag = true;
		if (AtlyssNetworkManager._current._steamworksMode && LobbyListManager._current._lobbyTypeDropdown.value != 0)
		{
			flag = false;
		}
		if (flag && P_0._message._password != AtlyssNetworkManager._current._serverPassword && AtlyssNetworkManager._current._serverPassword != string.Empty)
		{
			HostConsole._current.New_LogMessage($"kicked connection ID {P_0._conn.connectionId} - incorrect password.");
			P_0._conn.Disconnect();
			return false;
		}
		return true;
	}
}
