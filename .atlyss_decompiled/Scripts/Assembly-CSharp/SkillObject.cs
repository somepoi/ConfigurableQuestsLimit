using UnityEngine;

public class SkillObject : MonoBehaviour
{
	[Header("Runtime")]
	public ScriptableSkill _scriptSkill;

	public SkillObjectBehavior _skillObjBehavior;

	[Space]
	public Player _parentPlayer;

	public Creep _parentCreep;
}
