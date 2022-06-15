namespace SMPLSceneEditor
{
	public class MainScene : Scene
	{
		public MainScene()
		{
			FormWindow.DeleteFile(AssetsDirectory);
		}
		public static void LoadAsset(string path)
		{
			((MainScene)CurrentScene).LoadAssets(path);
		}
		public static void SaveScene()
		{
			((MainScene)CurrentScene).Save();
		}
		public static void SetSavePath(string path)
		{
			((MainScene)CurrentScene).SavePath = path;
		}
	}
}
