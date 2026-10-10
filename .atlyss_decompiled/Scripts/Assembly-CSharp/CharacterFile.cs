using System;

[Serializable]
public class CharacterFile
{
	public string _nickName;

	public string _version;

	public int _currency;

	public PlayerAppearance_Profile _appearanceProfile;

	public PlayerStats_Profile _statsProfile;

	public PlayerCasting_SkillsProfile _skillsProfile;

	public QuestProgressProfile _questProgressProfile;

	public ItemData[] _inventoryProfile;

	public EquipSyncStruct _vanityStruct;

	public string[] _quickItemSlotProfile = new string[5];

	public string[] _dialogIntroProfile = new string[0];

	public string[] _waypointAttunementProfile = new string[0];

	public bool[] _questTrackToggles = new bool[5];

	public bool _isWeaponLocked;

	public bool _isAltWeaponEquipped;

	public bool _isEmptySlot;

	public bool _isNewCharacter = true;
}
