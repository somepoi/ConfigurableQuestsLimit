using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoad : MonoBehaviour
{
	private void Start()
	{
		Invoke("LoadFromBootStrap", 2.5f);
	}

	public void LoadFromBootStrap()
	{
		SceneManager.LoadScene(1);
	}
}
