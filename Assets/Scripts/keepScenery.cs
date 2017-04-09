using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class keepScenery : MonoBehaviour {

	public GameObject island,
		              water,
		              islandScenery,
		              waterScenery,
                      boat,
                      boatScenery;
	public ParticleSystem smoke,
		                  smokeScenery;
	public Light sceneLight,
                 lightScenery;
	Scene  currentScene;
	string sceneName;
	static bool sceneryStay = true;

	void Update()
	{
		currentScene = SceneManager.GetActiveScene ();
		sceneName    = currentScene.name;

		if (sceneName == "In Game")
		{
			sceneryStay = false;
		}
	}

	void Awake()
	{
		if (sceneryStay)
		{
			islandScenery = GameObject.Find ("ISLAND");

			if (islandScenery == null) {
				islandScenery = island;
				waterScenery = water;
				smokeScenery = smoke;
				boatScenery  = boat;
				lightScenery = sceneLight;
				islandScenery.name = "ISLAND";
				DontDestroyOnLoad (islandScenery);
				DontDestroyOnLoad (waterScenery);
				DontDestroyOnLoad (smokeScenery);
				DontDestroyOnLoad (boatScenery);
				DontDestroyOnLoad (lightScenery);
			} 
			else
			{
				if (island.name != "ISLAND")
				{
					Destroy (island);
					Destroy (water);
					Destroy (smoke);
					Destroy (boat);
					Destroy (sceneLight);
				}
			}
		}
    }
}