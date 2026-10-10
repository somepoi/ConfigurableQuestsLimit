using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mirror;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public static class GeneratedNetworkCode
{
	public static TimeSnapshotMessage _Read_Mirror_002ETimeSnapshotMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002ETimeSnapshotMessage(NetworkWriter writer, TimeSnapshotMessage value)
	{
	}

	public static ReadyMessage _Read_Mirror_002EReadyMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002EReadyMessage(NetworkWriter writer, ReadyMessage value)
	{
	}

	public static NotReadyMessage _Read_Mirror_002ENotReadyMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002ENotReadyMessage(NetworkWriter writer, NotReadyMessage value)
	{
	}

	public static AddPlayerMessage _Read_Mirror_002EAddPlayerMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002EAddPlayerMessage(NetworkWriter writer, AddPlayerMessage value)
	{
	}

	public static SceneMessage _Read_Mirror_002ESceneMessage(NetworkReader reader)
	{
		return new SceneMessage
		{
			sceneName = reader.ReadString(),
			sceneOperation = _Read_Mirror_002ESceneOperation(reader),
			customHandling = reader.ReadBool()
		};
	}

	public static SceneOperation _Read_Mirror_002ESceneOperation(NetworkReader reader)
	{
		return (SceneOperation)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_Mirror_002ESceneMessage(NetworkWriter writer, SceneMessage value)
	{
		writer.WriteString(value.sceneName);
		_Write_Mirror_002ESceneOperation(writer, value.sceneOperation);
		writer.WriteBool(value.customHandling);
	}

	public static void _Write_Mirror_002ESceneOperation(NetworkWriter writer, SceneOperation value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static CommandMessage _Read_Mirror_002ECommandMessage(NetworkReader reader)
	{
		return new CommandMessage
		{
			netId = reader.ReadUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ECommandMessage(NetworkWriter writer, CommandMessage value)
	{
		writer.WriteUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static RpcMessage _Read_Mirror_002ERpcMessage(NetworkReader reader)
	{
		return new RpcMessage
		{
			netId = reader.ReadUInt(),
			componentIndex = NetworkReaderExtensions.ReadByte(reader),
			functionHash = reader.ReadUShort(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ERpcMessage(NetworkWriter writer, RpcMessage value)
	{
		writer.WriteUInt(value.netId);
		NetworkWriterExtensions.WriteByte(writer, value.componentIndex);
		writer.WriteUShort(value.functionHash);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static SpawnMessage _Read_Mirror_002ESpawnMessage(NetworkReader reader)
	{
		return new SpawnMessage
		{
			netId = reader.ReadUInt(),
			isLocalPlayer = reader.ReadBool(),
			isOwner = reader.ReadBool(),
			sceneId = reader.ReadULong(),
			assetId = reader.ReadUInt(),
			position = reader.ReadVector3(),
			rotation = reader.ReadQuaternion(),
			scale = reader.ReadVector3(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002ESpawnMessage(NetworkWriter writer, SpawnMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBool(value.isLocalPlayer);
		writer.WriteBool(value.isOwner);
		writer.WriteULong(value.sceneId);
		writer.WriteUInt(value.assetId);
		writer.WriteVector3(value.position);
		writer.WriteQuaternion(value.rotation);
		writer.WriteVector3(value.scale);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static ChangeOwnerMessage _Read_Mirror_002EChangeOwnerMessage(NetworkReader reader)
	{
		return new ChangeOwnerMessage
		{
			netId = reader.ReadUInt(),
			isOwner = reader.ReadBool(),
			isLocalPlayer = reader.ReadBool()
		};
	}

	public static void _Write_Mirror_002EChangeOwnerMessage(NetworkWriter writer, ChangeOwnerMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBool(value.isOwner);
		writer.WriteBool(value.isLocalPlayer);
	}

	public static ObjectSpawnStartedMessage _Read_Mirror_002EObjectSpawnStartedMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002EObjectSpawnStartedMessage(NetworkWriter writer, ObjectSpawnStartedMessage value)
	{
	}

	public static ObjectSpawnFinishedMessage _Read_Mirror_002EObjectSpawnFinishedMessage(NetworkReader reader)
	{
		return default;
	}

	public static void _Write_Mirror_002EObjectSpawnFinishedMessage(NetworkWriter writer, ObjectSpawnFinishedMessage value)
	{
	}

	public static ObjectDestroyMessage _Read_Mirror_002EObjectDestroyMessage(NetworkReader reader)
	{
		return new ObjectDestroyMessage
		{
			netId = reader.ReadUInt()
		};
	}

	public static void _Write_Mirror_002EObjectDestroyMessage(NetworkWriter writer, ObjectDestroyMessage value)
	{
		writer.WriteUInt(value.netId);
	}

	public static ObjectHideMessage _Read_Mirror_002EObjectHideMessage(NetworkReader reader)
	{
		return new ObjectHideMessage
		{
			netId = reader.ReadUInt()
		};
	}

	public static void _Write_Mirror_002EObjectHideMessage(NetworkWriter writer, ObjectHideMessage value)
	{
		writer.WriteUInt(value.netId);
	}

	public static EntityStateMessage _Read_Mirror_002EEntityStateMessage(NetworkReader reader)
	{
		return new EntityStateMessage
		{
			netId = reader.ReadUInt(),
			payload = reader.ReadBytesAndSizeSegment()
		};
	}

	public static void _Write_Mirror_002EEntityStateMessage(NetworkWriter writer, EntityStateMessage value)
	{
		writer.WriteUInt(value.netId);
		writer.WriteBytesAndSizeSegment(value.payload);
	}

	public static NetworkPingMessage _Read_Mirror_002ENetworkPingMessage(NetworkReader reader)
	{
		return new NetworkPingMessage
		{
			localTime = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPingMessage(NetworkWriter writer, NetworkPingMessage value)
	{
		writer.WriteDouble(value.localTime);
	}

	public static NetworkPongMessage _Read_Mirror_002ENetworkPongMessage(NetworkReader reader)
	{
		return new NetworkPongMessage
		{
			localTime = reader.ReadDouble()
		};
	}

	public static void _Write_Mirror_002ENetworkPongMessage(NetworkWriter writer, NetworkPongMessage value)
	{
		writer.WriteDouble(value.localTime);
	}

	public static PlayerProfileDataMessage _Read_PlayerProfileDataMessage(NetworkReader reader)
	{
		return new PlayerProfileDataMessage
		{
			_globalNickname = reader.ReadString(),
			_nickName = reader.ReadString(),
			_version = reader.ReadString(),
			_password = reader.ReadString(),
			_statsProfile = _Read_PlayerStats_Profile(reader),
			_appearanceProfile = _Read_PlayerAppearance_Profile(reader),
			_vanityStruct = _Read_EquipSyncStruct(reader),
			_currency = reader.ReadInt(),
			_isAltWeaponEquipped = reader.ReadBool(),
			_lockWeapon = reader.ReadBool(),
			_isNewCharacter = reader.ReadBool()
		};
	}

	public static PlayerStats_Profile _Read_PlayerStats_Profile(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		PlayerStats_Profile playerStats_Profile = new PlayerStats_Profile();
		playerStats_Profile._currentLevel = reader.ReadInt();
		playerStats_Profile._classID = reader.ReadString();
		playerStats_Profile._classTier = reader.ReadInt();
		playerStats_Profile._currentExp = reader.ReadInt();
		playerStats_Profile._skillPoints = reader.ReadInt();
		playerStats_Profile._attributePoints = reader.ReadInt();
		playerStats_Profile._attributes = _Read_AttributeStruct_005B_005D(reader);
		playerStats_Profile._professions = _Read_ProfessionStruct_005B_005D(reader);
		return playerStats_Profile;
	}

	public static AttributeStruct[] _Read_AttributeStruct_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<AttributeStruct>();
	}

	public static AttributeStruct _Read_AttributeStruct(NetworkReader reader)
	{
		return new AttributeStruct
		{
			_attributeName = reader.ReadString(),
			_attributeID = reader.ReadInt(),
			_attributeValue = reader.ReadInt()
		};
	}

	public static ProfessionStruct[] _Read_ProfessionStruct_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<ProfessionStruct>();
	}

	public static ProfessionStruct _Read_ProfessionStruct(NetworkReader reader)
	{
		return new ProfessionStruct
		{
			_professionID = reader.ReadInt(),
			_professionXP = reader.ReadInt(),
			_professionLvl = reader.ReadInt()
		};
	}

	public static PlayerAppearance_Profile _Read_PlayerAppearance_Profile(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		PlayerAppearance_Profile playerAppearance_Profile = new PlayerAppearance_Profile();
		playerAppearance_Profile._setRaceTag = reader.ReadString();
		playerAppearance_Profile._setEye = reader.ReadInt();
		playerAppearance_Profile._setMouth = reader.ReadInt();
		playerAppearance_Profile._setHairStyle = reader.ReadInt();
		playerAppearance_Profile._setEar = reader.ReadInt();
		playerAppearance_Profile._setTail = reader.ReadInt();
		playerAppearance_Profile._setMisc = reader.ReadInt();
		playerAppearance_Profile._displayBoobs = reader.ReadBool();
		playerAppearance_Profile._isLeftHanded = reader.ReadBool();
		playerAppearance_Profile._voicePitch = reader.ReadFloat();
		playerAppearance_Profile._helmDyeIndex = reader.ReadInt();
		playerAppearance_Profile._headWidth = reader.ReadFloat();
		playerAppearance_Profile._muzzleWeight = reader.ReadFloat();
		playerAppearance_Profile._heightWeight = reader.ReadFloat();
		playerAppearance_Profile._widthWeight = reader.ReadFloat();
		playerAppearance_Profile._torsoWeight = reader.ReadFloat();
		playerAppearance_Profile._armWeight = reader.ReadFloat();
		playerAppearance_Profile._boobWeight = reader.ReadFloat();
		playerAppearance_Profile._bellyWeight = reader.ReadFloat();
		playerAppearance_Profile._bottomWeight = reader.ReadFloat();
		playerAppearance_Profile._setTexture = reader.ReadInt();
		playerAppearance_Profile._hideHelm = reader.ReadBool();
		playerAppearance_Profile._hideCape = reader.ReadBool();
		playerAppearance_Profile._hideChest = reader.ReadBool();
		playerAppearance_Profile._hideLeggings = reader.ReadBool();
		playerAppearance_Profile._hideVanityHelm = reader.ReadBool();
		playerAppearance_Profile._hideVanityCape = reader.ReadBool();
		playerAppearance_Profile._hideVanityChest = reader.ReadBool();
		playerAppearance_Profile._hideVanityLeggings = reader.ReadBool();
		playerAppearance_Profile._hairColorProfile = _Read_ColorAdjustShader_Profile(reader);
		playerAppearance_Profile._hairIsBodyColor = reader.ReadBool();
		playerAppearance_Profile._miscColorProfile = _Read_ColorAdjustShader_Profile(reader);
		playerAppearance_Profile._setSkinColorProfile = _Read_ColorAdjustShader_Profile(reader);
		return playerAppearance_Profile;
	}

	public static ColorAdjustShader_Profile _Read_ColorAdjustShader_Profile(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		ColorAdjustShader_Profile colorAdjustShader_Profile = new ColorAdjustShader_Profile();
		colorAdjustShader_Profile._hue = reader.ReadFloat();
		colorAdjustShader_Profile._saturation = reader.ReadFloat();
		colorAdjustShader_Profile._brightness = reader.ReadFloat();
		colorAdjustShader_Profile._contrast = reader.ReadFloat();
		return colorAdjustShader_Profile;
	}

	public static EquipSyncStruct _Read_EquipSyncStruct(NetworkReader reader)
	{
		return new EquipSyncStruct
		{
			_helmTag = reader.ReadString(),
			_helmModifier = reader.ReadInt(),
			_chestTag = reader.ReadString(),
			_chestModifier = reader.ReadInt(),
			_leggingTag = reader.ReadString(),
			_leggingModifier = reader.ReadInt(),
			_capeTag = reader.ReadString(),
			_capeModifier = reader.ReadInt(),
			_ringTag = reader.ReadString(),
			_ringModifier = reader.ReadInt(),
			_weaponTag = reader.ReadString(),
			_weaponModifier = reader.ReadInt(),
			_damageOverride_mainWep = _Read_DamageType(reader),
			_useDamageOverride_mainWep = reader.ReadBool(),
			_altWeaponTag = reader.ReadString(),
			_altWeaponModifier = reader.ReadInt(),
			_damageOverride_altWep = _Read_DamageType(reader),
			_useDamageOverride_altWep = reader.ReadBool(),
			_shieldTag = reader.ReadString(),
			_shieldModifier = reader.ReadInt()
		};
	}

	public static DamageType _Read_DamageType(NetworkReader reader)
	{
		return (DamageType)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_PlayerProfileDataMessage(NetworkWriter writer, PlayerProfileDataMessage value)
	{
		writer.WriteString(value._globalNickname);
		writer.WriteString(value._nickName);
		writer.WriteString(value._version);
		writer.WriteString(value._password);
		_Write_PlayerStats_Profile(writer, value._statsProfile);
		_Write_PlayerAppearance_Profile(writer, value._appearanceProfile);
		_Write_EquipSyncStruct(writer, value._vanityStruct);
		writer.WriteInt(value._currency);
		writer.WriteBool(value._isAltWeaponEquipped);
		writer.WriteBool(value._lockWeapon);
		writer.WriteBool(value._isNewCharacter);
	}

	public static void _Write_PlayerStats_Profile(NetworkWriter writer, PlayerStats_Profile value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteInt(value._currentLevel);
		writer.WriteString(value._classID);
		writer.WriteInt(value._classTier);
		writer.WriteInt(value._currentExp);
		writer.WriteInt(value._skillPoints);
		writer.WriteInt(value._attributePoints);
		_Write_AttributeStruct_005B_005D(writer, value._attributes);
		_Write_ProfessionStruct_005B_005D(writer, value._professions);
	}

	public static void _Write_AttributeStruct_005B_005D(NetworkWriter writer, AttributeStruct[] value)
	{
		writer.WriteArray(value);
	}

	public static void _Write_AttributeStruct(NetworkWriter writer, AttributeStruct value)
	{
		writer.WriteString(value._attributeName);
		writer.WriteInt(value._attributeID);
		writer.WriteInt(value._attributeValue);
	}

	public static void _Write_ProfessionStruct_005B_005D(NetworkWriter writer, ProfessionStruct[] value)
	{
		writer.WriteArray(value);
	}

	public static void _Write_ProfessionStruct(NetworkWriter writer, ProfessionStruct value)
	{
		writer.WriteInt(value._professionID);
		writer.WriteInt(value._professionXP);
		writer.WriteInt(value._professionLvl);
	}

	public static void _Write_PlayerAppearance_Profile(NetworkWriter writer, PlayerAppearance_Profile value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteString(value._setRaceTag);
		writer.WriteInt(value._setEye);
		writer.WriteInt(value._setMouth);
		writer.WriteInt(value._setHairStyle);
		writer.WriteInt(value._setEar);
		writer.WriteInt(value._setTail);
		writer.WriteInt(value._setMisc);
		writer.WriteBool(value._displayBoobs);
		writer.WriteBool(value._isLeftHanded);
		writer.WriteFloat(value._voicePitch);
		writer.WriteInt(value._helmDyeIndex);
		writer.WriteFloat(value._headWidth);
		writer.WriteFloat(value._muzzleWeight);
		writer.WriteFloat(value._heightWeight);
		writer.WriteFloat(value._widthWeight);
		writer.WriteFloat(value._torsoWeight);
		writer.WriteFloat(value._armWeight);
		writer.WriteFloat(value._boobWeight);
		writer.WriteFloat(value._bellyWeight);
		writer.WriteFloat(value._bottomWeight);
		writer.WriteInt(value._setTexture);
		writer.WriteBool(value._hideHelm);
		writer.WriteBool(value._hideCape);
		writer.WriteBool(value._hideChest);
		writer.WriteBool(value._hideLeggings);
		writer.WriteBool(value._hideVanityHelm);
		writer.WriteBool(value._hideVanityCape);
		writer.WriteBool(value._hideVanityChest);
		writer.WriteBool(value._hideVanityLeggings);
		_Write_ColorAdjustShader_Profile(writer, value._hairColorProfile);
		writer.WriteBool(value._hairIsBodyColor);
		_Write_ColorAdjustShader_Profile(writer, value._miscColorProfile);
		_Write_ColorAdjustShader_Profile(writer, value._setSkinColorProfile);
	}

	public static void _Write_ColorAdjustShader_Profile(NetworkWriter writer, ColorAdjustShader_Profile value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteFloat(value._hue);
		writer.WriteFloat(value._saturation);
		writer.WriteFloat(value._brightness);
		writer.WriteFloat(value._contrast);
	}

	public static void _Write_EquipSyncStruct(NetworkWriter writer, EquipSyncStruct value)
	{
		writer.WriteString(value._helmTag);
		writer.WriteInt(value._helmModifier);
		writer.WriteString(value._chestTag);
		writer.WriteInt(value._chestModifier);
		writer.WriteString(value._leggingTag);
		writer.WriteInt(value._leggingModifier);
		writer.WriteString(value._capeTag);
		writer.WriteInt(value._capeModifier);
		writer.WriteString(value._ringTag);
		writer.WriteInt(value._ringModifier);
		writer.WriteString(value._weaponTag);
		writer.WriteInt(value._weaponModifier);
		_Write_DamageType(writer, value._damageOverride_mainWep);
		writer.WriteBool(value._useDamageOverride_mainWep);
		writer.WriteString(value._altWeaponTag);
		writer.WriteInt(value._altWeaponModifier);
		_Write_DamageType(writer, value._damageOverride_altWep);
		writer.WriteBool(value._useDamageOverride_altWep);
		writer.WriteString(value._shieldTag);
		writer.WriteInt(value._shieldModifier);
	}

	public static void _Write_DamageType(NetworkWriter writer, DamageType value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static ServerMessage _Read_ServerMessage(NetworkReader reader)
	{
		return new ServerMessage
		{
			servMsg = reader.ReadString()
		};
	}

	public static void _Write_ServerMessage(NetworkWriter writer, ServerMessage value)
	{
		writer.WriteString(value.servMsg);
	}

	public static void _Write_CreepCondition(NetworkWriter writer, CreepCondition value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static void _Write_StatStruct(NetworkWriter writer, StatStruct value)
	{
		writer.WriteInt(value._maxHealth);
		writer.WriteInt(value._maxMana);
		writer.WriteInt(value._maxStamina);
		writer.WriteInt(value._experience);
		writer.WriteInt(value._attackPower);
		writer.WriteInt(value._magicPower);
		writer.WriteInt(value._dexPower);
		writer.WriteFloat(value._criticalRate);
		writer.WriteFloat(value._magicCriticalRate);
		writer.WriteFloat(value._evasion);
		writer.WriteInt(value._defense);
		writer.WriteInt(value._magicDefense);
		writer.WriteInt(value._fireResist);
		writer.WriteInt(value._waterResist);
		writer.WriteInt(value._natureResist);
		writer.WriteInt(value._earthResist);
		writer.WriteInt(value._holyResist);
		writer.WriteInt(value._shadowResist);
	}

	public static CreepCondition _Read_CreepCondition(NetworkReader reader)
	{
		return (CreepCondition)NetworkReaderExtensions.ReadByte(reader);
	}

	public static StatStruct _Read_StatStruct(NetworkReader reader)
	{
		return new StatStruct
		{
			_maxHealth = reader.ReadInt(),
			_maxMana = reader.ReadInt(),
			_maxStamina = reader.ReadInt(),
			_experience = reader.ReadInt(),
			_attackPower = reader.ReadInt(),
			_magicPower = reader.ReadInt(),
			_dexPower = reader.ReadInt(),
			_criticalRate = reader.ReadFloat(),
			_magicCriticalRate = reader.ReadFloat(),
			_evasion = reader.ReadFloat(),
			_defense = reader.ReadInt(),
			_magicDefense = reader.ReadInt(),
			_fireResist = reader.ReadInt(),
			_waterResist = reader.ReadInt(),
			_natureResist = reader.ReadInt(),
			_earthResist = reader.ReadInt(),
			_holyResist = reader.ReadInt(),
			_shadowResist = reader.ReadInt()
		};
	}

	public static void _Write_WorldTime(NetworkWriter writer, WorldTime value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static void _Write_ClockSetting(NetworkWriter writer, ClockSetting value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static WorldTime _Read_WorldTime(NetworkReader reader)
	{
		return (WorldTime)NetworkReaderExtensions.ReadByte(reader);
	}

	public static ClockSetting _Read_ClockSetting(NetworkReader reader)
	{
		return (ClockSetting)NetworkReaderExtensions.ReadByte(reader);
	}

	public static ShopkeepItemStruct _Read_ShopkeepItemStruct(NetworkReader reader)
	{
		return new ShopkeepItemStruct
		{
			_itemName = reader.ReadString(),
			_itemData = _Read_ItemData(reader),
			_stockQuantity = reader.ReadInt(),
			_removeAtEmptyStock = reader.ReadBool(),
			_equipModifierID = reader.ReadInt(),
			_dedicatedValue = reader.ReadInt(),
			_useDedicatedValue = reader.ReadBool(),
			_specialItemCostName = reader.ReadString(),
			_specialItemCostQuantity = reader.ReadInt(),
			_isbuybackItem = reader.ReadBool(),
			_isGambleItem = reader.ReadBool(),
			_gambleValue = reader.ReadInt()
		};
	}

	public static ItemData _Read_ItemData(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		ItemData itemData = new ItemData();
		itemData._itemName = reader.ReadString();
		itemData._quantity = reader.ReadInt();
		itemData._maxQuantity = reader.ReadInt();
		itemData._slotNumber = reader.ReadInt();
		itemData._modifierID = reader.ReadInt();
		itemData._damageTypeOverride = _Read_DamageType(reader);
		itemData._useDamageTypeOverride = reader.ReadBool();
		itemData._isEquipped = reader.ReadBool();
		itemData._isAltWeapon = reader.ReadBool();
		return itemData;
	}

	public static void _Write_ShopkeepItemStruct(NetworkWriter writer, ShopkeepItemStruct value)
	{
		writer.WriteString(value._itemName);
		_Write_ItemData(writer, value._itemData);
		writer.WriteInt(value._stockQuantity);
		writer.WriteBool(value._removeAtEmptyStock);
		writer.WriteInt(value._equipModifierID);
		writer.WriteInt(value._dedicatedValue);
		writer.WriteBool(value._useDedicatedValue);
		writer.WriteString(value._specialItemCostName);
		writer.WriteInt(value._specialItemCostQuantity);
		writer.WriteBool(value._isbuybackItem);
		writer.WriteBool(value._isGambleItem);
		writer.WriteInt(value._gambleValue);
	}

	public static void _Write_ItemData(NetworkWriter writer, ItemData value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteString(value._itemName);
		writer.WriteInt(value._quantity);
		writer.WriteInt(value._maxQuantity);
		writer.WriteInt(value._slotNumber);
		writer.WriteInt(value._modifierID);
		_Write_DamageType(writer, value._damageTypeOverride);
		writer.WriteBool(value._useDamageTypeOverride);
		writer.WriteBool(value._isEquipped);
		writer.WriteBool(value._isAltWeapon);
	}

	public static void _Write_ZoneDifficulty(NetworkWriter writer, ZoneDifficulty value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static ZoneDifficulty _Read_ZoneDifficulty(NetworkReader reader)
	{
		return (ZoneDifficulty)NetworkReaderExtensions.ReadByte(reader);
	}

	public static ConditionStruct _Read_ConditionStruct(NetworkReader reader)
	{
		return new ConditionStruct
		{
			_senderEntity = reader.ReadNetworkBehaviour<StatusEntity>(),
			_attribute = _Read_DamageType(reader),
			_conditionID = reader.ReadInt(),
			_conditionDuration = reader.ReadInt(),
			_conditionPower = reader.ReadInt(),
			_conditionRepeatRate = reader.ReadFloat(),
			_conditionPowerPercent = reader.ReadFloat()
		};
	}

	public static void _Write_ConditionStruct(NetworkWriter writer, ConditionStruct value)
	{
		writer.WriteNetworkBehaviour(value._senderEntity);
		_Write_DamageType(writer, value._attribute);
		writer.WriteInt(value._conditionID);
		writer.WriteInt(value._conditionDuration);
		writer.WriteInt(value._conditionPower);
		writer.WriteFloat(value._conditionRepeatRate);
		writer.WriteFloat(value._conditionPowerPercent);
	}

	public static void _Write_DamageWeight(NetworkWriter writer, DamageWeight value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static DamageWeight _Read_DamageWeight(NetworkReader reader)
	{
		return (DamageWeight)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_FloatTextColor(NetworkWriter writer, FloatTextColor value)
	{
		writer.WriteInt((int)value);
	}

	public static FloatTextColor _Read_FloatTextColor(NetworkReader reader)
	{
		return (FloatTextColor)reader.ReadInt();
	}

	public static void _Write_ChatBehaviour_002FChatChannel(NetworkWriter writer, ChatBehaviour.ChatChannel value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static ChatBehaviour.ChatChannel _Read_ChatBehaviour_002FChatChannel(NetworkReader reader)
	{
		return (ChatBehaviour.ChatChannel)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_AtlyssSteamLobbyTag(NetworkWriter writer, AtlyssSteamLobbyTag value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static AtlyssSteamLobbyTag _Read_AtlyssSteamLobbyTag(NetworkReader reader)
	{
		return (AtlyssSteamLobbyTag)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_PartyInviteStatus(NetworkWriter writer, PartyInviteStatus value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static PartyInviteStatus _Read_PartyInviteStatus(NetworkReader reader)
	{
		return (PartyInviteStatus)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_GameCondition(NetworkWriter writer, GameCondition value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static void _Write_PlayerCondition(NetworkWriter writer, PlayerCondition value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static void _Write_ZoneType(NetworkWriter writer, ZoneType value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static GameCondition _Read_GameCondition(NetworkReader reader)
	{
		return (GameCondition)NetworkReaderExtensions.ReadByte(reader);
	}

	public static PlayerCondition _Read_PlayerCondition(NetworkReader reader)
	{
		return (PlayerCondition)NetworkReaderExtensions.ReadByte(reader);
	}

	public static ZoneType _Read_ZoneType(NetworkReader reader)
	{
		return (ZoneType)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_SkillStruct_005B_005D(NetworkWriter writer, SkillStruct[] value)
	{
		writer.WriteArray(value);
	}

	public static void _Write_SkillStruct(NetworkWriter writer, SkillStruct value)
	{
		if (value == null)
		{
			writer.WriteBool(value: false);
			return;
		}
		writer.WriteBool(value: true);
		writer.WriteString(value._skillName);
		writer.WriteInt(value._skillLibraryIndex);
		writer.WriteBool(value._skillUnlocked);
	}

	public static SkillStruct[] _Read_SkillStruct_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<SkillStruct>();
	}

	public static SkillStruct _Read_SkillStruct(NetworkReader reader)
	{
		if (!reader.ReadBool())
		{
			return null;
		}
		SkillStruct skillStruct = new SkillStruct();
		skillStruct._skillName = reader.ReadString();
		skillStruct._skillLibraryIndex = reader.ReadInt();
		skillStruct._skillUnlocked = reader.ReadBool();
		return skillStruct;
	}

	public static void _Write_WeaponSheatheCondition(NetworkWriter writer, WeaponSheatheCondition value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static WeaponSheatheCondition _Read_WeaponSheatheCondition(NetworkReader reader)
	{
		return (WeaponSheatheCondition)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_EquipSfx(NetworkWriter writer, EquipSfx value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static EquipSfx _Read_EquipSfx(NetworkReader reader)
	{
		return (EquipSfx)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_MovementLockType(NetworkWriter writer, MovementLockType value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static MovementLockType _Read_MovementLockType(NetworkReader reader)
	{
		return (MovementLockType)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_VersusFlag(NetworkWriter writer, VersusFlag value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static VersusFlag _Read_VersusFlag(NetworkReader reader)
	{
		return (VersusFlag)NetworkReaderExtensions.ReadByte(reader);
	}

	public static void _Write_System_002EString_005B_005D(NetworkWriter writer, string[] value)
	{
		writer.WriteArray(value);
	}

	public static string[] _Read_System_002EString_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<string>();
	}

	public static void _Write_System_002EInt32_005B_005D(NetworkWriter writer, int[] value)
	{
		writer.WriteArray(value);
	}

	public static int[] _Read_System_002EInt32_005B_005D(NetworkReader reader)
	{
		return reader.ReadArray<int>();
	}

	public static void _Write_PlayerAppearanceStruct(NetworkWriter writer, PlayerAppearanceStruct value)
	{
		writer.WriteString(value._setRaceTag);
		writer.WriteInt(value._eyeID);
		writer.WriteInt(value._mouthID);
		writer.WriteInt(value._hairStyleID);
		writer.WriteInt(value._earID);
		writer.WriteInt(value._tailID);
		writer.WriteInt(value._miscID);
		writer.WriteBool(value._displayBoobs);
		writer.WriteBool(value._isLeftHanded);
		writer.WriteInt(value._dyeIndex);
		writer.WriteFloat(value._headWidth);
		writer.WriteFloat(value._muzzleWeight);
		writer.WriteFloat(value._voicePitch);
		writer.WriteFloat(value._heightWeight);
		writer.WriteFloat(value._widthWeight);
		writer.WriteFloat(value._torsoWeight);
		writer.WriteFloat(value._armWeight);
		writer.WriteFloat(value._boobWeight);
		writer.WriteFloat(value._bellyWeight);
		writer.WriteFloat(value._bottomWeight);
		writer.WriteInt(value._textureID);
		writer.WriteFloat(value._bodyColorHue);
		writer.WriteFloat(value._bodyColorBrightness);
		writer.WriteFloat(value._bodyColorContrast);
		writer.WriteFloat(value._bodyColorSaturation);
		writer.WriteFloat(value._hairStyleHue);
		writer.WriteFloat(value._hairStyleBrightness);
		writer.WriteFloat(value._hairStyleContrast);
		writer.WriteFloat(value._hairStyleSaturation);
		writer.WriteBool(value._hairIsBodyColor);
		writer.WriteFloat(value._miscHue);
		writer.WriteFloat(value._miscBrightness);
		writer.WriteFloat(value._miscContrast);
		writer.WriteFloat(value._miscSaturation);
		writer.WriteBool(value._hideHelm);
		writer.WriteBool(value._hideCape);
		writer.WriteBool(value._hideChest);
		writer.WriteBool(value._hideLeggings);
		writer.WriteBool(value._hideVanityHelm);
		writer.WriteBool(value._hideVanityCape);
		writer.WriteBool(value._hideVanityChest);
		writer.WriteBool(value._hideVanityLeggings);
	}

	public static PlayerAppearanceStruct _Read_PlayerAppearanceStruct(NetworkReader reader)
	{
		return new PlayerAppearanceStruct
		{
			_setRaceTag = reader.ReadString(),
			_eyeID = reader.ReadInt(),
			_mouthID = reader.ReadInt(),
			_hairStyleID = reader.ReadInt(),
			_earID = reader.ReadInt(),
			_tailID = reader.ReadInt(),
			_miscID = reader.ReadInt(),
			_displayBoobs = reader.ReadBool(),
			_isLeftHanded = reader.ReadBool(),
			_dyeIndex = reader.ReadInt(),
			_headWidth = reader.ReadFloat(),
			_muzzleWeight = reader.ReadFloat(),
			_voicePitch = reader.ReadFloat(),
			_heightWeight = reader.ReadFloat(),
			_widthWeight = reader.ReadFloat(),
			_torsoWeight = reader.ReadFloat(),
			_armWeight = reader.ReadFloat(),
			_boobWeight = reader.ReadFloat(),
			_bellyWeight = reader.ReadFloat(),
			_bottomWeight = reader.ReadFloat(),
			_textureID = reader.ReadInt(),
			_bodyColorHue = reader.ReadFloat(),
			_bodyColorBrightness = reader.ReadFloat(),
			_bodyColorContrast = reader.ReadFloat(),
			_bodyColorSaturation = reader.ReadFloat(),
			_hairStyleHue = reader.ReadFloat(),
			_hairStyleBrightness = reader.ReadFloat(),
			_hairStyleContrast = reader.ReadFloat(),
			_hairStyleSaturation = reader.ReadFloat(),
			_hairIsBodyColor = reader.ReadBool(),
			_miscHue = reader.ReadFloat(),
			_miscBrightness = reader.ReadFloat(),
			_miscContrast = reader.ReadFloat(),
			_miscSaturation = reader.ReadFloat(),
			_hideHelm = reader.ReadBool(),
			_hideCape = reader.ReadBool(),
			_hideChest = reader.ReadBool(),
			_hideLeggings = reader.ReadBool(),
			_hideVanityHelm = reader.ReadBool(),
			_hideVanityCape = reader.ReadBool(),
			_hideVanityChest = reader.ReadBool(),
			_hideVanityLeggings = reader.ReadBool()
		};
	}

	public static void _Write_EquipCellTab(NetworkWriter writer, EquipCellTab value)
	{
		NetworkWriterExtensions.WriteByte(writer, (byte)value);
	}

	public static EquipCellTab _Read_EquipCellTab(NetworkReader reader)
	{
		return (EquipCellTab)NetworkReaderExtensions.ReadByte(reader);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void InitReadWriters()
	{
		Writer<byte>.write = NetworkWriterExtensions.WriteByte;
		Writer<byte?>.write = NetworkWriterExtensions.WriteByteNullable;
		Writer<sbyte>.write = NetworkWriterExtensions.WriteSByte;
		Writer<sbyte?>.write = NetworkWriterExtensions.WriteSByteNullable;
		Writer<char>.write = NetworkWriterExtensions.WriteChar;
		Writer<char?>.write = NetworkWriterExtensions.WriteCharNullable;
		Writer<bool>.write = NetworkWriterExtensions.WriteBool;
		Writer<bool?>.write = NetworkWriterExtensions.WriteBoolNullable;
		Writer<short>.write = NetworkWriterExtensions.WriteShort;
		Writer<short?>.write = NetworkWriterExtensions.WriteShortNullable;
		Writer<ushort>.write = NetworkWriterExtensions.WriteUShort;
		Writer<ushort?>.write = NetworkWriterExtensions.WriteUShortNullable;
		Writer<int>.write = NetworkWriterExtensions.WriteInt;
		Writer<int?>.write = NetworkWriterExtensions.WriteIntNullable;
		Writer<uint>.write = NetworkWriterExtensions.WriteUInt;
		Writer<uint?>.write = NetworkWriterExtensions.WriteUIntNullable;
		Writer<long>.write = NetworkWriterExtensions.WriteLong;
		Writer<long?>.write = NetworkWriterExtensions.WriteLongNullable;
		Writer<ulong>.write = NetworkWriterExtensions.WriteULong;
		Writer<ulong?>.write = NetworkWriterExtensions.WriteULongNullable;
		Writer<float>.write = NetworkWriterExtensions.WriteFloat;
		Writer<float?>.write = NetworkWriterExtensions.WriteFloatNullable;
		Writer<double>.write = NetworkWriterExtensions.WriteDouble;
		Writer<double?>.write = NetworkWriterExtensions.WriteDoubleNullable;
		Writer<decimal>.write = NetworkWriterExtensions.WriteDecimal;
		Writer<decimal?>.write = NetworkWriterExtensions.WriteDecimalNullable;
		Writer<string>.write = NetworkWriterExtensions.WriteString;
		Writer<ArraySegment<byte>>.write = NetworkWriterExtensions.WriteBytesAndSizeSegment;
		Writer<byte[]>.write = NetworkWriterExtensions.WriteBytesAndSize;
		Writer<Vector2>.write = NetworkWriterExtensions.WriteVector2;
		Writer<Vector2?>.write = NetworkWriterExtensions.WriteVector2Nullable;
		Writer<Vector3>.write = NetworkWriterExtensions.WriteVector3;
		Writer<Vector3?>.write = NetworkWriterExtensions.WriteVector3Nullable;
		Writer<Vector4>.write = NetworkWriterExtensions.WriteVector4;
		Writer<Vector4?>.write = NetworkWriterExtensions.WriteVector4Nullable;
		Writer<Vector2Int>.write = NetworkWriterExtensions.WriteVector2Int;
		Writer<Vector2Int?>.write = NetworkWriterExtensions.WriteVector2IntNullable;
		Writer<Vector3Int>.write = NetworkWriterExtensions.WriteVector3Int;
		Writer<Vector3Int?>.write = NetworkWriterExtensions.WriteVector3IntNullable;
		Writer<Color>.write = NetworkWriterExtensions.WriteColor;
		Writer<Color?>.write = NetworkWriterExtensions.WriteColorNullable;
		Writer<Color32>.write = NetworkWriterExtensions.WriteColor32;
		Writer<Color32?>.write = NetworkWriterExtensions.WriteColor32Nullable;
		Writer<Quaternion>.write = NetworkWriterExtensions.WriteQuaternion;
		Writer<Quaternion?>.write = NetworkWriterExtensions.WriteQuaternionNullable;
		Writer<Rect>.write = NetworkWriterExtensions.WriteRect;
		Writer<Rect?>.write = NetworkWriterExtensions.WriteRectNullable;
		Writer<Plane>.write = NetworkWriterExtensions.WritePlane;
		Writer<Plane?>.write = NetworkWriterExtensions.WritePlaneNullable;
		Writer<Ray>.write = NetworkWriterExtensions.WriteRay;
		Writer<Ray?>.write = NetworkWriterExtensions.WriteRayNullable;
		Writer<Matrix4x4>.write = NetworkWriterExtensions.WriteMatrix4x4;
		Writer<Matrix4x4?>.write = NetworkWriterExtensions.WriteMatrix4x4Nullable;
		Writer<Guid>.write = NetworkWriterExtensions.WriteGuid;
		Writer<Guid?>.write = NetworkWriterExtensions.WriteGuidNullable;
		Writer<NetworkIdentity>.write = NetworkWriterExtensions.WriteNetworkIdentity;
		Writer<NetworkBehaviour>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<Transform>.write = NetworkWriterExtensions.WriteTransform;
		Writer<GameObject>.write = NetworkWriterExtensions.WriteGameObject;
		Writer<Uri>.write = NetworkWriterExtensions.WriteUri;
		Writer<Texture2D>.write = NetworkWriterExtensions.WriteTexture2D;
		Writer<Sprite>.write = NetworkWriterExtensions.WriteSprite;
		Writer<DateTime>.write = NetworkWriterExtensions.WriteDateTime;
		Writer<DateTime?>.write = NetworkWriterExtensions.WriteDateTimeNullable;
		Writer<TimeSnapshotMessage>.write = _Write_Mirror_002ETimeSnapshotMessage;
		Writer<ReadyMessage>.write = _Write_Mirror_002EReadyMessage;
		Writer<NotReadyMessage>.write = _Write_Mirror_002ENotReadyMessage;
		Writer<AddPlayerMessage>.write = _Write_Mirror_002EAddPlayerMessage;
		Writer<SceneMessage>.write = _Write_Mirror_002ESceneMessage;
		Writer<SceneOperation>.write = _Write_Mirror_002ESceneOperation;
		Writer<CommandMessage>.write = _Write_Mirror_002ECommandMessage;
		Writer<RpcMessage>.write = _Write_Mirror_002ERpcMessage;
		Writer<SpawnMessage>.write = _Write_Mirror_002ESpawnMessage;
		Writer<ChangeOwnerMessage>.write = _Write_Mirror_002EChangeOwnerMessage;
		Writer<ObjectSpawnStartedMessage>.write = _Write_Mirror_002EObjectSpawnStartedMessage;
		Writer<ObjectSpawnFinishedMessage>.write = _Write_Mirror_002EObjectSpawnFinishedMessage;
		Writer<ObjectDestroyMessage>.write = _Write_Mirror_002EObjectDestroyMessage;
		Writer<ObjectHideMessage>.write = _Write_Mirror_002EObjectHideMessage;
		Writer<EntityStateMessage>.write = _Write_Mirror_002EEntityStateMessage;
		Writer<NetworkPingMessage>.write = _Write_Mirror_002ENetworkPingMessage;
		Writer<NetworkPongMessage>.write = _Write_Mirror_002ENetworkPongMessage;
		Writer<PlayerProfileDataMessage>.write = _Write_PlayerProfileDataMessage;
		Writer<PlayerStats_Profile>.write = _Write_PlayerStats_Profile;
		Writer<AttributeStruct[]>.write = _Write_AttributeStruct_005B_005D;
		Writer<AttributeStruct>.write = _Write_AttributeStruct;
		Writer<ProfessionStruct[]>.write = _Write_ProfessionStruct_005B_005D;
		Writer<ProfessionStruct>.write = _Write_ProfessionStruct;
		Writer<PlayerAppearance_Profile>.write = _Write_PlayerAppearance_Profile;
		Writer<ColorAdjustShader_Profile>.write = _Write_ColorAdjustShader_Profile;
		Writer<EquipSyncStruct>.write = _Write_EquipSyncStruct;
		Writer<DamageType>.write = _Write_DamageType;
		Writer<ServerMessage>.write = _Write_ServerMessage;
		Writer<CreepCondition>.write = _Write_CreepCondition;
		Writer<StatStruct>.write = _Write_StatStruct;
		Writer<WorldTime>.write = _Write_WorldTime;
		Writer<ClockSetting>.write = _Write_ClockSetting;
		Writer<ShopkeepItemStruct>.write = _Write_ShopkeepItemStruct;
		Writer<ItemData>.write = _Write_ItemData;
		Writer<Player>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<ZoneDifficulty>.write = _Write_ZoneDifficulty;
		Writer<ConditionStruct>.write = _Write_ConditionStruct;
		Writer<StatusEntity>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<DamageWeight>.write = _Write_DamageWeight;
		Writer<FloatTextColor>.write = _Write_FloatTextColor;
		Writer<ChatBehaviour.ChatChannel>.write = _Write_ChatBehaviour_002FChatChannel;
		Writer<AtlyssSteamLobbyTag>.write = _Write_AtlyssSteamLobbyTag;
		Writer<PartyInviteStatus>.write = _Write_PartyInviteStatus;
		Writer<GameCondition>.write = _Write_GameCondition;
		Writer<PlayerCondition>.write = _Write_PlayerCondition;
		Writer<ZoneType>.write = _Write_ZoneType;
		Writer<SkillStruct[]>.write = _Write_SkillStruct_005B_005D;
		Writer<SkillStruct>.write = _Write_SkillStruct;
		Writer<WeaponSheatheCondition>.write = _Write_WeaponSheatheCondition;
		Writer<EquipSfx>.write = _Write_EquipSfx;
		Writer<PushBlock>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<Portal>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<ItemDropEntity_Chest>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<Net_ItemObject>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<NetNPC>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<MovementLockType>.write = _Write_MovementLockType;
		Writer<VersusFlag>.write = _Write_VersusFlag;
		Writer<string[]>.write = _Write_System_002EString_005B_005D;
		Writer<int[]>.write = _Write_System_002EInt32_005B_005D;
		Writer<ResourceEntity>.write = NetworkWriterExtensions.WriteNetworkBehaviour;
		Writer<PlayerAppearanceStruct>.write = _Write_PlayerAppearanceStruct;
		Writer<EquipCellTab>.write = _Write_EquipCellTab;
		Reader<byte>.read = NetworkReaderExtensions.ReadByte;
		Reader<byte?>.read = NetworkReaderExtensions.ReadByteNullable;
		Reader<sbyte>.read = NetworkReaderExtensions.ReadSByte;
		Reader<sbyte?>.read = NetworkReaderExtensions.ReadSByteNullable;
		Reader<char>.read = NetworkReaderExtensions.ReadChar;
		Reader<char?>.read = NetworkReaderExtensions.ReadCharNullable;
		Reader<bool>.read = NetworkReaderExtensions.ReadBool;
		Reader<bool?>.read = NetworkReaderExtensions.ReadBoolNullable;
		Reader<short>.read = NetworkReaderExtensions.ReadShort;
		Reader<short?>.read = NetworkReaderExtensions.ReadShortNullable;
		Reader<ushort>.read = NetworkReaderExtensions.ReadUShort;
		Reader<ushort?>.read = NetworkReaderExtensions.ReadUShortNullable;
		Reader<int>.read = NetworkReaderExtensions.ReadInt;
		Reader<int?>.read = NetworkReaderExtensions.ReadIntNullable;
		Reader<uint>.read = NetworkReaderExtensions.ReadUInt;
		Reader<uint?>.read = NetworkReaderExtensions.ReadUIntNullable;
		Reader<long>.read = NetworkReaderExtensions.ReadLong;
		Reader<long?>.read = NetworkReaderExtensions.ReadLongNullable;
		Reader<ulong>.read = NetworkReaderExtensions.ReadULong;
		Reader<ulong?>.read = NetworkReaderExtensions.ReadULongNullable;
		Reader<float>.read = NetworkReaderExtensions.ReadFloat;
		Reader<float?>.read = NetworkReaderExtensions.ReadFloatNullable;
		Reader<double>.read = NetworkReaderExtensions.ReadDouble;
		Reader<double?>.read = NetworkReaderExtensions.ReadDoubleNullable;
		Reader<decimal>.read = NetworkReaderExtensions.ReadDecimal;
		Reader<decimal?>.read = NetworkReaderExtensions.ReadDecimalNullable;
		Reader<string>.read = NetworkReaderExtensions.ReadString;
		Reader<byte[]>.read = NetworkReaderExtensions.ReadBytesAndSize;
		Reader<ArraySegment<byte>>.read = NetworkReaderExtensions.ReadBytesAndSizeSegment;
		Reader<Vector2>.read = NetworkReaderExtensions.ReadVector2;
		Reader<Vector2?>.read = NetworkReaderExtensions.ReadVector2Nullable;
		Reader<Vector3>.read = NetworkReaderExtensions.ReadVector3;
		Reader<Vector3?>.read = NetworkReaderExtensions.ReadVector3Nullable;
		Reader<Vector4>.read = NetworkReaderExtensions.ReadVector4;
		Reader<Vector4?>.read = NetworkReaderExtensions.ReadVector4Nullable;
		Reader<Vector2Int>.read = NetworkReaderExtensions.ReadVector2Int;
		Reader<Vector2Int?>.read = NetworkReaderExtensions.ReadVector2IntNullable;
		Reader<Vector3Int>.read = NetworkReaderExtensions.ReadVector3Int;
		Reader<Vector3Int?>.read = NetworkReaderExtensions.ReadVector3IntNullable;
		Reader<Color>.read = NetworkReaderExtensions.ReadColor;
		Reader<Color?>.read = NetworkReaderExtensions.ReadColorNullable;
		Reader<Color32>.read = NetworkReaderExtensions.ReadColor32;
		Reader<Color32?>.read = NetworkReaderExtensions.ReadColor32Nullable;
		Reader<Quaternion>.read = NetworkReaderExtensions.ReadQuaternion;
		Reader<Quaternion?>.read = NetworkReaderExtensions.ReadQuaternionNullable;
		Reader<Rect>.read = NetworkReaderExtensions.ReadRect;
		Reader<Rect?>.read = NetworkReaderExtensions.ReadRectNullable;
		Reader<Plane>.read = NetworkReaderExtensions.ReadPlane;
		Reader<Plane?>.read = NetworkReaderExtensions.ReadPlaneNullable;
		Reader<Ray>.read = NetworkReaderExtensions.ReadRay;
		Reader<Ray?>.read = NetworkReaderExtensions.ReadRayNullable;
		Reader<Matrix4x4>.read = NetworkReaderExtensions.ReadMatrix4x4;
		Reader<Matrix4x4?>.read = NetworkReaderExtensions.ReadMatrix4x4Nullable;
		Reader<Guid>.read = NetworkReaderExtensions.ReadGuid;
		Reader<Guid?>.read = NetworkReaderExtensions.ReadGuidNullable;
		Reader<NetworkIdentity>.read = NetworkReaderExtensions.ReadNetworkIdentity;
		Reader<NetworkBehaviour>.read = NetworkReaderExtensions.ReadNetworkBehaviour;
		Reader<NetworkBehaviourSyncVar>.read = NetworkReaderExtensions.ReadNetworkBehaviourSyncVar;
		Reader<Transform>.read = NetworkReaderExtensions.ReadTransform;
		Reader<GameObject>.read = NetworkReaderExtensions.ReadGameObject;
		Reader<Uri>.read = NetworkReaderExtensions.ReadUri;
		Reader<Texture2D>.read = NetworkReaderExtensions.ReadTexture2D;
		Reader<Sprite>.read = NetworkReaderExtensions.ReadSprite;
		Reader<DateTime>.read = NetworkReaderExtensions.ReadDateTime;
		Reader<DateTime?>.read = NetworkReaderExtensions.ReadDateTimeNullable;
		Reader<TimeSnapshotMessage>.read = _Read_Mirror_002ETimeSnapshotMessage;
		Reader<ReadyMessage>.read = _Read_Mirror_002EReadyMessage;
		Reader<NotReadyMessage>.read = _Read_Mirror_002ENotReadyMessage;
		Reader<AddPlayerMessage>.read = _Read_Mirror_002EAddPlayerMessage;
		Reader<SceneMessage>.read = _Read_Mirror_002ESceneMessage;
		Reader<SceneOperation>.read = _Read_Mirror_002ESceneOperation;
		Reader<CommandMessage>.read = _Read_Mirror_002ECommandMessage;
		Reader<RpcMessage>.read = _Read_Mirror_002ERpcMessage;
		Reader<SpawnMessage>.read = _Read_Mirror_002ESpawnMessage;
		Reader<ChangeOwnerMessage>.read = _Read_Mirror_002EChangeOwnerMessage;
		Reader<ObjectSpawnStartedMessage>.read = _Read_Mirror_002EObjectSpawnStartedMessage;
		Reader<ObjectSpawnFinishedMessage>.read = _Read_Mirror_002EObjectSpawnFinishedMessage;
		Reader<ObjectDestroyMessage>.read = _Read_Mirror_002EObjectDestroyMessage;
		Reader<ObjectHideMessage>.read = _Read_Mirror_002EObjectHideMessage;
		Reader<EntityStateMessage>.read = _Read_Mirror_002EEntityStateMessage;
		Reader<NetworkPingMessage>.read = _Read_Mirror_002ENetworkPingMessage;
		Reader<NetworkPongMessage>.read = _Read_Mirror_002ENetworkPongMessage;
		Reader<PlayerProfileDataMessage>.read = _Read_PlayerProfileDataMessage;
		Reader<PlayerStats_Profile>.read = _Read_PlayerStats_Profile;
		Reader<AttributeStruct[]>.read = _Read_AttributeStruct_005B_005D;
		Reader<AttributeStruct>.read = _Read_AttributeStruct;
		Reader<ProfessionStruct[]>.read = _Read_ProfessionStruct_005B_005D;
		Reader<ProfessionStruct>.read = _Read_ProfessionStruct;
		Reader<PlayerAppearance_Profile>.read = _Read_PlayerAppearance_Profile;
		Reader<ColorAdjustShader_Profile>.read = _Read_ColorAdjustShader_Profile;
		Reader<EquipSyncStruct>.read = _Read_EquipSyncStruct;
		Reader<DamageType>.read = _Read_DamageType;
		Reader<ServerMessage>.read = _Read_ServerMessage;
		Reader<CreepCondition>.read = _Read_CreepCondition;
		Reader<StatStruct>.read = _Read_StatStruct;
		Reader<WorldTime>.read = _Read_WorldTime;
		Reader<ClockSetting>.read = _Read_ClockSetting;
		Reader<ShopkeepItemStruct>.read = _Read_ShopkeepItemStruct;
		Reader<ItemData>.read = _Read_ItemData;
		Reader<Player>.read = NetworkReaderExtensions.ReadNetworkBehaviour<Player>;
		Reader<ZoneDifficulty>.read = _Read_ZoneDifficulty;
		Reader<ConditionStruct>.read = _Read_ConditionStruct;
		Reader<StatusEntity>.read = NetworkReaderExtensions.ReadNetworkBehaviour<StatusEntity>;
		Reader<DamageWeight>.read = _Read_DamageWeight;
		Reader<FloatTextColor>.read = _Read_FloatTextColor;
		Reader<ChatBehaviour.ChatChannel>.read = _Read_ChatBehaviour_002FChatChannel;
		Reader<AtlyssSteamLobbyTag>.read = _Read_AtlyssSteamLobbyTag;
		Reader<PartyInviteStatus>.read = _Read_PartyInviteStatus;
		Reader<GameCondition>.read = _Read_GameCondition;
		Reader<PlayerCondition>.read = _Read_PlayerCondition;
		Reader<ZoneType>.read = _Read_ZoneType;
		Reader<SkillStruct[]>.read = _Read_SkillStruct_005B_005D;
		Reader<SkillStruct>.read = _Read_SkillStruct;
		Reader<WeaponSheatheCondition>.read = _Read_WeaponSheatheCondition;
		Reader<EquipSfx>.read = _Read_EquipSfx;
		Reader<PushBlock>.read = NetworkReaderExtensions.ReadNetworkBehaviour<PushBlock>;
		Reader<Portal>.read = NetworkReaderExtensions.ReadNetworkBehaviour<Portal>;
		Reader<ItemDropEntity_Chest>.read = NetworkReaderExtensions.ReadNetworkBehaviour<ItemDropEntity_Chest>;
		Reader<Net_ItemObject>.read = NetworkReaderExtensions.ReadNetworkBehaviour<Net_ItemObject>;
		Reader<NetNPC>.read = NetworkReaderExtensions.ReadNetworkBehaviour<NetNPC>;
		Reader<MovementLockType>.read = _Read_MovementLockType;
		Reader<VersusFlag>.read = _Read_VersusFlag;
		Reader<string[]>.read = _Read_System_002EString_005B_005D;
		Reader<int[]>.read = _Read_System_002EInt32_005B_005D;
		Reader<ResourceEntity>.read = NetworkReaderExtensions.ReadNetworkBehaviour<ResourceEntity>;
		Reader<PlayerAppearanceStruct>.read = _Read_PlayerAppearanceStruct;
		Reader<EquipCellTab>.read = _Read_EquipCellTab;
	}
}
