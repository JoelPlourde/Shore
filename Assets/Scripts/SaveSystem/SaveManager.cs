using System;
using System.IO;
using System.Linq;
using ItemSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Collections.Generic;

namespace SaveSystem {
	public class SaveManager : MonoBehaviour {

		public static SaveManager Instance;

		// The World Save is the state of the World when the scene was saved in the Editor.
		private Save _worldSave;

		private void Awake() {
			Instance = this;
		}

		/// <summary>
		/// On Start, after all components have been initialized, load the Save Files.
		/// </summary>
		/// <exception cref="UnityException"></exception>
		private void Start() {
			// Load the world save file.
			if (!LoadWorldSaveFile()) {
				throw new UnityException("Failed to load world save file, you must save the Scene first.");
			}

			// Load the most recent player save file.
			if (!LoadRecentPlayerSaveFile()) {

				// If no player save file was found, start a new game.
				NewGame();
			}
		}

		/// <summary>
		/// This method loads the World Save File for the current scene.
		/// </summary>
		/// <returns>Whether the operation was successful.</returns>
		private bool LoadWorldSaveFile() {
			string sceneName = SceneManager.GetActiveScene().name;
			string filename = "world-" + sceneName + ".json";

			Save save = LoadGameFile(filename);
			if (ReferenceEquals(save, null)) {
				return false;
			}

			_worldSave = save;
			return true;
		}

		/// <summary>
		/// This method loads the most recent player save file for the current scene.
		/// </summary>
		/// <returns>Whether the operation was successful.</returns>
		public bool LoadRecentPlayerSaveFile() {
			string path = Application.persistentDataPath;
			UnityEngine.Debug.Log("Looking for save files in: " + path);
			DirectoryInfo directory = new DirectoryInfo(path);
			FileInfo[] files = directory.GetFiles("*.json");

			if (files.Length == 0) {
				return false;
			}

			string sceneName = SceneManager.GetActiveScene().name;

			FileInfo mostRecentFile = null;
			foreach (FileInfo file in files) {
				if (!file.Name.Contains(sceneName)) {
					continue;
				}

				if (file.Name.Contains("world")) {
					continue;
				}

				if (ReferenceEquals(mostRecentFile, null)) {
					mostRecentFile = file;
					continue;
				}
				
				if (file.LastWriteTime > mostRecentFile.LastWriteTime) {
					mostRecentFile = file;
				}
			}

			if (ReferenceEquals(mostRecentFile, null)) {
				return false;
			}

			return LoadPlayerSaveFile(mostRecentFile.Name);
		}

		private void NewGame() {
			UnityEngine.Debug.Log("New Game!");
			Save save = new Save();

			save.ActorDtos.Add(new ActorDto());

			// Load all GameObject within the scene.
			GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

			foreach (GameObject gameObject in gameObjects) {
				// For each GameObject, find all components that implement ISaveable.
				ISaveable[] saveables = gameObject.GetComponents<ISaveable>();

				if (saveables.Length == 0) {
                	continue;
            	}

				foreach (ISaveable saveable in saveables) {
					saveable.Load(save);
				}
			}
			
			Squad.TeleportSquadToSpawnPoint();

			// Select the first actor in the squad.
			Squad.FirstSelected(out Actor actor);
			Squad.SelectActor(actor);
		}

		/// <summary>
		/// Load the player save file.
		/// </summary>
		/// <param name="filename">The filename</param>
		/// <returns>Whether the operation was successful</returns>
		private bool LoadPlayerSaveFile(string filename) {
			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();
			Save playerSave = LoadGameFile(filename);
			if (ReferenceEquals(playerSave, null)) {
				return false;
			}

			// Load all GameObject within the scene.
			GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

			foreach (GameObject gameObject in gameObjects) {
				// For each GameObject, find all components that implement ISaveable.
				ISaveable[] saveables = gameObject.GetComponents<ISaveable>();

				if (saveables.Length == 0) {
                	continue;
            	}

				foreach (ISaveable saveable in saveables) {
					if (saveable is IWorldSaveable worldSaveable) {
						if (worldSaveable.DetermineState(_worldSave, playerSave)) {
							// There has been a change in the world save, reload the player save.
							saveable.Load(playerSave);
						}
					} else {
						saveable.Load(playerSave);
					}
				}
			}

			// For each objects that are in the player save:
			foreach (KeyValuePair<string, WorldItemDto> worldItemDto in playerSave.WorldItemDtos.ToList()) {
				// Check if the worldItemDto is in the world save.
				if (!_worldSave.WorldItemDtos.ContainsKey(worldItemDto.Key)) {
					// If not, spawn it:
					ItemData itemData = ItemManager.Instance.GetItemData(worldItemDto.Value.ID);

					Item item = new Item(itemData, worldItemDto.Value.Amount);

					// Euler Angles to Quaternion
					Quaternion quaternion = Quaternion.Euler(worldItemDto.Value.Rotation);

					ItemManager.Instance.PlaceItemInWorld(item, worldItemDto.Value.Position, quaternion, false);
				}
			}

			stopwatch.Stop();
			UnityEngine.Debug.Log("Loaded player save in: " + stopwatch.ElapsedMilliseconds + "ms");

			return true;
		}

		/// <summary>
		/// Save the game.
		/// </summary>
		public void SaveGame() {
			// Create a new save object.
			Save save = new Save();

			// Load all GameObject within the scene.
			GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

			foreach (GameObject gameObject in gameObjects) {
				// For each GameObject, find all components that implement ISaveable.
				ISaveable[] saveables = gameObject.GetComponents<ISaveable>();

				if (saveables.Length == 0) {
                	continue;
            	}

				foreach (ISaveable saveable in saveables) {
					saveable.Save(save);
				}
			}

			// Create a filename based on the scene name and the current time.
			string sceneName = SceneManager.GetActiveScene().name;
			string filename = sceneName + "-" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".json";

			// Save the game to a file.
			SaveGameFile(save, filename);
		}

		/// <summary>
		/// Load the game from a file.
		/// </summary>
		/// <param name="filename">The filename</param>
		/// <returns>The save if found, else null.</returns>
		public static Save LoadGameFile(string filename) {
			Save save = new Save();

			string path = Application.persistentDataPath + "/" + filename;
			if (!File.Exists(path)) {
				return null;
			}

			StreamReader reader = new StreamReader(path);
			JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), save);
			reader.Close();

			return save;
		}

		/// <summary>
		/// Save the game to a file.
		/// </summary>
		/// <param name="save">The save</param>
		/// <param name="filename">The filename</param>
		public static void SaveGameFile(Save save, string filename) {
			string path = Application.persistentDataPath + "/";
			Directory.CreateDirectory(path);

			string absolutePath = Path.Combine(path, filename);
			string fileContents = JsonUtility.ToJson(save, true);

			File.WriteAllText(absolutePath, fileContents);

			UnityEngine.Debug.Log("Game saved to: " + absolutePath);
		}
	}
}
