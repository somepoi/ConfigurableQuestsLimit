using UnityEngine;

[CreateAssetMenu(fileName = "castEffect_", menuName = "ScriptableObjects/Skill/Cast Effect Collection", order = 1)]
public class CastEffectCollection : ScriptableObject
{
	[Header("Cast Effect Collection")]
	public GameObject _chargeSkillEffect;

	public GameObject _castSkillEffect;

	[Header("Ground Glyph Display")]
	public Texture _channelGroundGlyphTexture;

	public float _channelGroundGlyphRotationSpeed = 1.25f;

	public float _groundGlyphSpotAngleSize = 35.5f;

	public float _groundGlyphIntensity = 85.5f;

	public Color _groundGlyphColor = Color.white;

	[Header("Cast Effect Audio")]
	public AudioClip _channelSound;

	public AudioClip _castSound;

	public float _channelSoundPitch = 1f;

	public float _castSoundPitch = 1f;
}
