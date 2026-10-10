using Mirror;

public struct PlayerProfileDataMessage : NetworkMessage
{
	public string _globalNickname;

	public string _nickName;

	public string _version;

	public string _password;

	public PlayerStats_Profile _statsProfile;

	public PlayerAppearance_Profile _appearanceProfile;

	public EquipSyncStruct _vanityStruct;

	public int _currency;

	public bool _isAltWeaponEquipped;

	public bool _lockWeapon;

	public bool _isNewCharacter;
}
