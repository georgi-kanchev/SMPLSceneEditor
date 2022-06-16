namespace SMPLSceneEditor
{
	public class MainScene : Scene
	{
		public static void LoadAsset(string path)
		{
			((MainScene)CurrentScene).LoadAssets(path);
		}
		public static void UnloadAsset(string path)
		{
			((MainScene)CurrentScene).UnloadAssets(path);
		}
		public static void SaveScene(string path)
		{
			((MainScene)CurrentScene).Save(path);
		}
	}
}
