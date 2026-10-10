using UnityEngine;

namespace Mirror.Examples.AdditiveLevels;

public class PhysicsSimulator : MonoBehaviour
{
	private PhysicsScene physicsScene;

	private bool simulatePhysicsScene;

	private void Awake()
	{
		if (NetworkServer.active)
		{
			physicsScene = base.gameObject.scene.GetPhysicsScene();
			simulatePhysicsScene = physicsScene.IsValid() && physicsScene != Physics.defaultPhysicsScene;
		}
		else
		{
			base.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		if (NetworkServer.active && simulatePhysicsScene)
		{
			physicsScene.Simulate(Time.fixedDeltaTime);
		}
	}
}
