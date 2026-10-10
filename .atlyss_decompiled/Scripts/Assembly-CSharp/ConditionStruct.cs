using System;

[Serializable]
public struct ConditionStruct : IEquatable<ConditionStruct>
{
	public StatusEntity _senderEntity;

	public DamageType _attribute;

	public int _conditionID;

	public int _conditionDuration;

	public int _conditionPower;

	public float _conditionRepeatRate;

	public float _conditionPowerPercent;

	public readonly bool Equals(ConditionStruct other)
	{
		if (_senderEntity == other._senderEntity && _attribute == other._attribute && _conditionID == other._conditionID && _conditionDuration == other._conditionDuration && _conditionPower == other._conditionPower && _conditionRepeatRate == other._conditionRepeatRate)
		{
			return _conditionPowerPercent == other._conditionPowerPercent;
		}
		return false;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is ConditionStruct other)
		{
			return Equals(other);
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(_senderEntity, _attribute, _conditionID, _conditionDuration, _conditionPower, _conditionRepeatRate, _conditionPowerPercent);
	}
}
