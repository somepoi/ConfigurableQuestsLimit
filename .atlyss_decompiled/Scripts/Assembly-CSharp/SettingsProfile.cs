using System;
using UnityEngine;

[Serializable]
public class SettingsProfile
{
	[Header("Video Settings")]
	public int _screenMode;

	public int _cursorIndex;

	public bool _setResolution;

	public bool _isHardwareCursor;

	public int _textureFilterSetting;

	public int _antiAliasingSetting;

	public int _textureQualitySetting;

	public bool _anisotropicTextureSetting = true;

	public bool _isVsync = true;

	public float _cameraFieldOfView = 90f;

	public float _cameraSmoothing = 15f;

	public float _cameraHoriz;

	public float _cameraVert;

	public int _cameraRenderDistanceValue = 2;

	public bool _cameraBitCrushEnabled;

	public bool _cameraUnderwaterShaderEnabled = true;

	public bool _cameraShakeEnabled = true;

	public bool _disableWeaponGlow;

	public bool _disableJiggleBones;

	public bool _clearUnderClothes;

	public bool _disableGibs;

	[Header("Audio Settings")]
	public float _masterVolume = 1f;

	public float _gameVolume = 1f;

	public float _guiVolume = 0.85f;

	public float _ambienceVolume = 0.65f;

	public float _musicVolume = 0.75f;

	public float _voiceVolume = 1f;

	public bool _muteApplication;

	public bool _muteMusic;

	public bool _fadeMusicOutOfWindow = true;

	[Header("Input Settings")]
	public int _setGamepadType;

	public float _cameraSensitivity = 5f;

	public bool _invertXAxis;

	public bool _invertYAxis;

	public bool _analogDirectionMode;

	public int _inputStrafeMode;

	public bool _inputStrafeWhileHoldingWeapon = true;

	public bool _inputStrafeWhileMoveAttack = true;

	public bool _inputStrafeWhileCastingOffensiveSkill = true;

	public bool _detectedGamepadOnMenu;

	[Header("Network / UI Settings")]
	public bool _enableUnicodeCharacters;

	public bool _displayGlobalNameTags = true;

	public bool _networkDisplayHostTag;

	public bool _networkDisplayLocalPlayerNameTag;

	public bool _hideDungeonMinimap;

	public bool _flagPvPOnEnterMap;

	public bool _persistDisconnectHotkey;

	public bool _hideFpsCounter;

	public bool _hidePingCounter;

	public bool _hideStatPointCounter;

	public bool _hideSkillPointCounter;

	public bool _hideQuestTracker;

	public bool _hideDamageValueIcons;

	public bool _hidePlayerInstanceCounter;

	public bool _fadeChatText = true;

	public bool _fadeGameFeed = true;

	public bool _hideNpcHeadIcons;

	public bool _displaySlotCoolDownCounters;

	public bool _dontShowDisclaimer;

	[Header("Last Character")]
	public int _lastCharacterFile;

	public int _lastCharacterPage;
}
