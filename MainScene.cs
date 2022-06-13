namespace SMPLSceneEditor
{
	public class MainScene : Scene
	{
		public static void Load(string path)
		{
			((MainScene)CurrentScene).LoadAssets(path);
		}
	}
}
