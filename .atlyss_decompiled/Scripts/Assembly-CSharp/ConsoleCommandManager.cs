using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class ConsoleCommandManager : MonoBehaviour
{
	private HostConsole _hostConsole;

	private bool _isEmittingShutdown;

	private void Awake()
	{
		_hostConsole = GetComponent<HostConsole>();
	}

	private void Update()
	{
		if (!NetworkServer.active)
		{
			StopAllCoroutines();
			_isEmittingShutdown = false;
		}
	}

	public void Init_ConsoleCommand(string _name, string _output)
	{
		int.TryParse(_output, out var result);
		if (!int.TryParse(_output, out result))
		{
			result = -1;
		}
		string message = "'/" + _name + "' is invalid command.";
		string message2 = "Invalid output.";
		switch (_name)
		{
		case "help":
		{
			string message4 = "Commands: \n/shutdown - shuts down the server. Add a number output to apply a countdown. (ex: /shutdown 5)\n/cancelsd - cancels an ongoing server shutdown counter. Must have a countdown going.\n/starthost - initalizes the server. Server instance must be shut down to initalize.\n/kick - kicks a _gamepadIsDetected client. Must have a output of the connection ID number. (ex: /kick 2)\n/ban - same as kick, but also bans the client's address from connecting to the server. (ex: /ban 2)\n/clearbanlist - clears the list of bans saved in your host settings profile.\n/clients - displays all clients that are _gamepadIsDetected to the server with Connnection IDs.\n/maplist - displays all loaded map instances on the server.\n/clear - clears console log.";
			_hostConsole.New_LogMessage(message4);
			break;
		}
		case "shutdown":
			if (result > 0)
			{
				if (!_isEmittingShutdown)
				{
					StartCoroutine(ShutDown_Countdown(result));
				}
				else
				{
					_hostConsole.New_LogMessage("Server already undergoing shutdown phase.");
				}
			}
			else
			{
				_hostConsole.Init_ServerDisconnect();
			}
			break;
		case "cancelsd":
			if (!_isEmittingShutdown)
			{
				_hostConsole.New_LogMessage("Server is not undergoing a shutdown.");
				break;
			}
			_hostConsole.Init_ServerMessage("Server shutdown cancelled.");
			_isEmittingShutdown = false;
			break;
		case "starthost":
			if (NetworkServer.active)
			{
				_hostConsole.New_LogMessage("Server is already running.");
			}
			else
			{
				AtlyssNetworkManager._current.StartServer();
			}
			break;
		case "kick":
		{
			if (_output == string.Empty)
			{
				_hostConsole.New_LogMessage(message2);
				break;
			}
			if (result <= -1)
			{
				_hostConsole.New_LogMessage(message2);
				break;
			}
			if (NetworkServer.connections.Count <= 0)
			{
				_hostConsole.New_LogMessage("There are no _gamepadIsDetected clients.");
				break;
			}
			for (int l = 0; l < _hostConsole._peerListEntries.Count; l++)
			{
				if (_hostConsole._peerListEntries[l]._dataID == result)
				{
					if (_hostConsole._peerListEntries[l]._peerPlayer._isHostPlayer)
					{
						_hostConsole.New_LogMessage("Cannot kick the host!");
					}
					else
					{
						Init_KickClient(_hostConsole._peerListEntries[l], result);
					}
					break;
				}
				_hostConsole.New_LogMessage($"connectionID {result} does not exist.");
			}
			break;
		}
		case "ban":
		{
			if (_output == string.Empty)
			{
				_hostConsole.New_LogMessage(message2);
				break;
			}
			if (result <= -1)
			{
				_hostConsole.New_LogMessage(message2);
				break;
			}
			if (NetworkServer.connections.Count <= 0)
			{
				_hostConsole.New_LogMessage("There are no _gamepadIsDetected clients.");
				break;
			}
			for (int j = 0; j < _hostConsole._peerListEntries.Count; j++)
			{
				if (_hostConsole._peerListEntries[j]._dataID == result)
				{
					if (_hostConsole._peerListEntries[j]._peerPlayer._isHostPlayer)
					{
						_hostConsole.New_LogMessage("Cannot ban the host!");
					}
					else
					{
						Init_BanClient(_hostConsole._peerListEntries[j], result);
					}
					break;
				}
				_hostConsole.New_LogMessage($"connectionID {result} does not exist.");
			}
			break;
		}
		case "clearbanlist":
			if (AtlyssNetworkManager._current._bannedClientList.Count <= 0)
			{
				_hostConsole.New_LogMessage("There are no banned clients on the list.");
			}
			else
			{
				Init_ClearBanList();
			}
			break;
		case "clients":
		{
			if (NetworkServer.connections.Count <= 0)
			{
				_hostConsole.New_LogMessage("There are no _gamepadIsDetected clients.");
				break;
			}
			_hostConsole.New_LogMessage($"{_hostConsole._peerListEntries.Count} _gamepadIsDetected clients found.");
			for (int k = 0; k < _hostConsole._peerListEntries.Count; k++)
			{
				string message5 = _hostConsole._peerListEntries[k]._dataNameText.text ?? "";
				_hostConsole.New_LogMessage(message5);
			}
			break;
		}
		case "maplist":
		{
			if (_hostConsole._instanceListEntries.Count <= 0)
			{
				_hostConsole.New_LogMessage("There are no loaded map instances.");
				break;
			}
			_hostConsole.New_LogMessage($"{_hostConsole._instanceListEntries.Count} map instances found.");
			for (int i = 0; i < _hostConsole._instanceListEntries.Count; i++)
			{
				string message3 = _hostConsole._instanceListEntries[i]._dataNameText.text ?? "";
				_hostConsole.New_LogMessage(message3);
			}
			break;
		}
		case "savelog":
			_hostConsole.Close_HostConsole();
			break;
		case "clear":
			_hostConsole.Clear_Log();
			Console.Clear();
			break;
		default:
			_hostConsole.New_LogMessage(message);
			break;
		}
	}

	public void Init_ClearBanList()
	{
		AtlyssNetworkManager._current._bannedClientList.Clear();
		ProfileDataManager._current.Save_HostSettingsData();
		_hostConsole.New_LogMessage("Cleared banlist.");
	}

	public void Init_BanClient(HC_PeerListEntry _peerEntryIndex, int _conID)
	{
		string servMsg = _peerEntryIndex._peerPlayer._nickname + " is banned from the server.";
		NetworkServer.SendToAll(new ServerMessage
		{
			servMsg = servMsg
		});
		BannedClientParameter item = new BannedClientParameter
		{
			_characterNickname = _peerEntryIndex._peerPlayer._nickname,
			_address = _peerEntryIndex._peerPlayer.connectionToClient.address,
			_steamID = _peerEntryIndex._peerPlayer._steamID
		};
		AtlyssNetworkManager._current._bannedClientList.Add(item);
		_hostConsole.New_LogMessage($"Banned connectionID {_conID}.");
		ProfileDataManager._current.Save_HostSettingsData();
		Init_KickClient(_peerEntryIndex, _conID);
	}

	public void Init_KickClient(HC_PeerListEntry _peerEntryIndex, int _conID)
	{
		string servMsg = _peerEntryIndex._peerPlayer._nickname + " was kicked from the server.";
		NetworkServer.SendToAll(new ServerMessage
		{
			servMsg = servMsg
		});
		_hostConsole.New_LogMessage($"Kicked connectionID {_conID}.");
		NetworkServer.connections[_conID].Disconnect();
	}

	public void Init_ButtonShutdown()
	{
		StartCoroutine(ShutDown_Countdown(20));
	}

	private IEnumerator ShutDown_Countdown(int _timer)
	{
		_isEmittingShutdown = true;
		int _currentTimer = _timer;
		_hostConsole.Init_ServerMessage($"The server will be shutting down soon (Set Timer: {_currentTimer}).");
		yield return new WaitForSeconds(1f);
		do
		{
			if (!_isEmittingShutdown)
			{
				yield break;
			}
			if (_currentTimer <= 10)
			{
				_hostConsole.Init_ServerMessage($"Server shut down in {_currentTimer} seconds...");
			}
			else if (_currentTimer % 10 == 0)
			{
				_hostConsole.Init_ServerMessage($"Server shut down in {_currentTimer} seconds...");
			}
			_currentTimer--;
			yield return new WaitForSeconds(1f);
		}
		while (_currentTimer > -1);
		if (_isEmittingShutdown)
		{
			_hostConsole.Init_ServerDisconnect();
			_isEmittingShutdown = false;
		}
	}
}
