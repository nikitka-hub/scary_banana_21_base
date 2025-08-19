using UnityEngine;

public static class TargetPoint
{
	public static Vector3 FitToTheGround(Vector3 origin, LayerMask mask, float offset, float length, float radius)
	{
		if (Physics.SphereCast(origin + Vector3.up * offset, radius, Vector3.down, out var hitInfo, length, mask))
		{
			return hitInfo.point;
		}
		return origin;
	}

	public static bool IsValidStepPoint(Vector3 point, LayerMask mask, float offset, float length, float radius)
	{
		return Physics.CheckSphere(point + Vector3.up * offset, radius, mask);
	}
}
