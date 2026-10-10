using UnityEngine;

[CreateAssetMenu(fileName = "dialogData_", menuName = "ScriptableObjects/Game Logic/Dialog Data", order = 0)]
public class ScriptableDialogData : ScriptableObject
{
	public string _nameTag = string.Empty;

	[Space]
	public DialogBranch[] _introductionBranches;

	public DialogBranch[] _dialogBranches;

	[Header("Shopkeep Parameters")]
	public ScriptableShopkeep _scriptableShopkeep;

	public string[] _shopkeepResponses;

	public string[] _shopkeepRejections;

	[Header("Quest Giver Parameters")]
	public ScriptableQuest[] _scriptableQuests;

	public string[] _questAcceptResponses;

	public string[] _questCompleteResponses;

	[Header("NPC Audio")]
	public AudioClip[] _voxSounds;

	public float _voxSpeed = 0.185f;

	private void OnValidate()
	{
		for (int i = 0; i < _dialogBranches.Length; i++)
		{
			_dialogBranches[i]._dialogIndexTag = $"Dialog Branch Index #{i}";
			for (int j = 0; j < _dialogBranches[i].dialogs.Length; j++)
			{
				string text = _dialogBranches[i].dialogs[j]._dialogInput;
				if (text.Length > 30)
				{
					text = text.Substring(0, 30) + "...";
				}
				_dialogBranches[i].dialogs[j]._dialogKey = text ?? "";
			}
		}
	}
}
