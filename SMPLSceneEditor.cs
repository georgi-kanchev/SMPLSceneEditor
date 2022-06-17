global using System.Numerics;
global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;
global using SMPL;
global using SMPL.Tools;
global using Color = SFML.Graphics.Color;
global using Cursor = System.Windows.Forms.Cursor;

namespace SMPLSceneEditor
{
	public partial class FormWindow : Form
	{
		#region Fields
		private Dictionary<string, string> listBoxPlaceholderTexts = new()
		{
			{ "Children UIDs", "(select to focus)                                     " },
		};

		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene, confirmDeleteChildrenMsgShown, selectGameDirShown;
		private Vector2 prevFormsMousePos, prevMousePos, prevFormsMousePosGrid, selectStartPos, rightClickPos;
		private readonly List<string> selectedUIDs = new();
		private readonly Cursor[] editCursors = new Cursor[] { Cursors.NoMove2D, Cursors.Cross, Cursors.SizeAll };
		private readonly FileSystemWatcher assetsWatcher;
		private string? finalGameDir;
		#endregion

		#region Init
		public FormWindow()
		{
			InitializeComponent();
			UpdateThingPanel();

			WindowState = FormWindowState.Maximized;

			window = new(windowPicture.Handle);
			windowPicture.MouseWheel += OnSceneScroll;

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			SetView();

			editSelectionOptions.SelectedIndex = 0;

			Scene.CurrentScene = new MainScene();

			assetsWatcher = new(AppContext.BaseDirectory) { EnableRaisingEvents = true };

			AddThingProperty("Types", typeof(List<string>));
			AddThingProperty("Thing", null, rightLabel: true);
			AddThingProperty("Properties", null);
			AddThingProperty("UID", typeof(string));
			AddThingProperty("Old UID", typeof(string), readOnly: true);
			AddThingProperty("Parent UID", typeof(string));
			AddThingProperty("Children UIDs", typeof(List<string>));
			AddThingProperty("Update Order", typeof(int));
			AddThingProperty("", null); AddThingProperty("", null);
			AddThingProperty("Local Position", typeof(Vector2));
			AddThingProperty("Local Angle", typeof(float));
			AddThingProperty("Local Direction", typeof(Vector2), readOnly: true, smallNumericStep: true);
			AddThingProperty("Local Scale", typeof(float), smallNumericStep: true);

			AddThingProperty("Reset", typeof(Button), last: true); AddThingProperty("Apply", typeof(Button), last: true);
		}
		private void AddThingProperty(string name, Type? valueType, bool readOnly = false, bool rightLabel = false, bool smallNumericStep = false,
			bool last = false)
		{
			const string FONT = "Segoe UI";
			const float FONT_SIZE = 12f;

			var prop = default(Control);
			var label = new Label() { Text = name };
			SetDefault(label, FONT_SIZE + 3, reverseColors: true);
			label.Enabled = true;

			if(valueType == null)
			{
				label.TextAlign = rightLabel ? ContentAlignment.TopRight : ContentAlignment.TopLeft;
				editThingTable.Controls.Add(label);
				return;
			}

			if(valueType == typeof(string))
				prop = new TextBox();
			else if(valueType == typeof(int))
			{
				prop = new NumericUpDown();
				SetDefaultNumeric((NumericUpDown)prop, true);
			}
			else if(valueType == typeof(float))
			{
				prop = new NumericUpDown();
				SetDefaultNumeric((NumericUpDown)prop, false);
			}
			else if(valueType == typeof(List<string>))
			{
				prop = new ComboBox
				{
					DropDownStyle = ComboBoxStyle.DropDownList,
					FormattingEnabled = true,
				};
				var list = (ComboBox)prop;
				list.DropDown += new EventHandler(OnListThingDropDown);
				list.SelectedIndexChanged += new EventHandler(OnListSelectThing);
				list.DropDownClosed += new EventHandler(OnListThingDropDownClose);
			}
			else if(valueType == typeof(Button))
				prop = new Button() { Text = name };
			else if(valueType == typeof(Vector2))
			{
				prop = new TableLayoutPanel();
				var table = (TableLayoutPanel)prop;
				SetDefault(table);
				table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
				for(int i = 0; i < 2; i++)
					table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
				table.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
				table.ColumnCount = 2;
				table.RowCount = 1;
				var x = new NumericUpDown();
				var y = new NumericUpDown();
				x.BorderStyle = BorderStyle.None;
				y.BorderStyle = BorderStyle.None;
				SetDefault(x);
				SetDefault(y);
				SetDefaultNumeric(x, false);
				SetDefaultNumeric(y, false);
				table.Controls.Add(x);
				table.Controls.Add(y);
			}

			if(prop != null)
			{
				SetDefault(prop);
				label.ForeColor = prop.Enabled ? System.Drawing.Color.White : System.Drawing.Color.Gray;
			}

			if(last)
				editThingTable.Controls.Add(prop, 0, 24);
			else
				editThingTable.Controls.Add(prop);
			if(valueType != typeof(Button))
				editThingTable.Controls.Add(label);

			void SetDefault(Control control, float fontSize = FONT_SIZE, bool reverseColors = false)
			{
				var black = System.Drawing.Color.Black;
				var white = System.Drawing.Color.White;
				var isLabel = control is Label;

				if(isLabel)
					reverseColors = !reverseColors;

				control.Enabled = readOnly == false;
				control.Font = new System.Drawing.Font(FONT, fontSize, FontStyle.Regular, GraphicsUnit.Point);
				control.BackColor = reverseColors ? white : black;
				control.ForeColor = reverseColors ? black : white;
				control.Name = name;
				control.TabStop = false;
				control.Dock = DockStyle.Fill;
			}
			void SetDefaultNumeric(NumericUpDown numeric, bool isInt)
			{
				numeric.Increment = (decimal)(smallNumericStep ? 0.1 : 1);
				numeric.DecimalPlaces = isInt ? 0 : (smallNumericStep ? 3 : 1);
				numeric.Minimum = int.MinValue;
				numeric.Maximum = int.MaxValue;
			}
		}
		#endregion
		#region Update
		private void OnUpdate(object? sender, EventArgs e)
		{
			TryUpdateGameDir();

			window.Size = new((uint)windowPicture.Width, (uint)windowPicture.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.Clear();
			DrawGrid();
			TryShowMousePos();
			UpdateFPS();

			TrySelect();
			TryDestroy();

			ThingManager.UpdateAllThings();
			ThingManager.DrawAllVisuals(window);

			TryDrawSelection();

			window.Display();
		}
		private void UpdateThingPanel()
		{
			editThingTable.Visible = selectedUIDs.Count == 1;
			selectThingTip.Visible = selectedUIDs.Count != 1;

			if(editThingTable.Visible == false)
				return;

			var types = (ComboBox)Controls.Find("Types", true)[0];
			var uid = (TextBox)Controls.Find("UID", true)[0];
			var propOldUID = (TextBox)Controls.Find("Old UID", true)[0];
			var propParentUID = (TextBox)Controls.Find("Parent UID", true)[0];
			var propChildrenUIDs = (ComboBox)Controls.Find("Children UIDs", true)[0];
			var propUpdateOrder = (NumericUpDown)Controls.Find("Update Order", true)[0];
			var propLocalPos = (TableLayoutPanel)Controls.Find("Local Position", true)[0];
			var localX = (NumericUpDown)propLocalPos.Controls[0];
			var localY = (NumericUpDown)propLocalPos.Controls[1];
			var propLocalAng = (NumericUpDown)Controls.Find("Local Angle", true)[0];
			var propLocalSc = (NumericUpDown)Controls.Find("Local Scale", true)[0];
			var propLocalDir = (TableLayoutPanel)Controls.Find("Local Direction", true)[0];
			var localDirX = (NumericUpDown)propLocalDir.Controls[0];
			var localDirY = (NumericUpDown)propLocalDir.Controls[1];

			var uidStr = selectedUIDs[0];

			SetText(uid, uidStr);
			SetText(propOldUID, (string)ThingManager.Get(uidStr, "OldUID"), readOnly: true);
			SetText(propParentUID, (string)ThingManager.Get(uidStr, "ParentUID"));
			SetNumber(propUpdateOrder, (int)ThingManager.Get(uidStr, "UpdateOrder"));
			SetNumber(propLocalAng, ((float)ThingManager.Get(uidStr, "LocalAngle")).AngleTo360());
			SetNumber(propLocalSc, (float)ThingManager.Get(uidStr, "LocalScale"));

			var localPos = (Vector2)ThingManager.Get(uidStr, "LocalPosition");
			SetNumber(localX, localPos.X);
			SetNumber(localY, localPos.Y);
			var localDir = (Vector2)ThingManager.Get(uidStr, "LocalDirection");
			SetNumber(localDirX, localDir.X);
			SetNumber(localDirY, localDir.Y);

			ProcessThingList(types, "Types");
			ProcessThingList(propChildrenUIDs, "ChildrenUIDs");

			void ProcessThingList(ComboBox list, string propName)
			{
				list.Items.Clear();

				var propList = (List<string>)ThingManager.Get(uidStr, propName);
				list.Enabled = propList.Count > 0;

				if(list.Enabled && listBoxPlaceholderTexts.ContainsKey(list.Name))
					list.Items.Add(listBoxPlaceholderTexts[list.Name]);
				else if(list.Enabled == false)
					list.Items.Add("(none)");

				if(propList.Count > 0)
				{
					for(int i = 0; i < propList.Count; i++)
						list.Items.Add(propList[i]);
				}

				list.SelectedIndex = 0;
			}
			void SetText(TextBox text, string value, bool readOnly = false)
			{
				text.Enabled = readOnly == false;
				text.Text = value;
			}
			void SetNumber(NumericUpDown number, float value, bool readOnly = false)
			{
				number.Enabled = readOnly == false;
				number.Value = (decimal)value.Limit((float)number.Minimum, (float)number.Maximum);
			}
		}

		private void UpdateFPS()
		{
			SMPL.Tools.Time.Update();
			fps.Text = $"FPS [{SMPL.Tools.Time.FPS:F0}]";
		}
		private void DrawGrid()
		{
			if(gridThickness.Value == gridThickness.Minimum)
				return;

			var cellVerts = new VertexArray(PrimitiveType.Quads);
			var specialCellVerts = new VertexArray(PrimitiveType.Quads);
			var sz = new Vector2(windowPicture.Width, windowPicture.Height) * sceneSc;
			var thickness = (float)gridThickness.Value;
			var spacing = GetGridSpacing();
			var viewPos = window.GetView().Center;

			thickness *= sceneSc;
			thickness *= 0.5f;

			for(float i = 0; i <= sz.X * 4; i += spacing)
			{
				var x = viewPos.X - sz.X * 2 + i;
				var y = viewPos.Y;
				var top = new Vector2(x, y - sz.Y * 2).PointToGrid(new(spacing));
				var bot = new Vector2(x, y + sz.Y * 2).PointToGrid(new(spacing));
				var col = GetColor(top.X);
				var verts = GetVertexArray(top.X);

				verts.Append(new(top.PointMoveAtAngle(180, thickness, false).ToSFML(), col));
				verts.Append(new(top.PointMoveAtAngle(0, thickness, false).ToSFML(), col));
				verts.Append(new(bot.PointMoveAtAngle(0, thickness, false).ToSFML(), col));
				verts.Append(new(bot.PointMoveAtAngle(180, thickness, false).ToSFML(), col));
			}
			for(float i = 0; i <= sz.Y * 4; i += spacing)
			{
				var x = viewPos.X;
				var y = viewPos.Y - sz.Y * 2 + i;
				var left = new Vector2(x - sz.X * 2, y).PointToGrid(new(spacing));
				var right = new Vector2(x + sz.X * 2, y).PointToGrid(new(spacing));
				var col = GetColor(left.Y);
				var verts = GetVertexArray(left.Y);

				verts.Append(new(left.PointMoveAtAngle(270, thickness, false).ToSFML(), col));
				verts.Append(new(left.PointMoveAtAngle(90, thickness, false).ToSFML(), col));
				verts.Append(new(right.PointMoveAtAngle(90, thickness, false).ToSFML(), col));
				verts.Append(new(right.PointMoveAtAngle(270, thickness, false).ToSFML(), col));
			}

			window.Draw(cellVerts);
			window.Draw(specialCellVerts);

			Color GetColor(float coordinate)
			{
				if(coordinate == 0)
					return SFML.Graphics.Color.Yellow;
				else if(coordinate % 1000 == 0)
					return SFML.Graphics.Color.White;

				return new SFML.Graphics.Color(50, 50, 50);
			}
			VertexArray GetVertexArray(float coordinate)
			{
				return coordinate == 0 || coordinate % 1000 == 0 ? specialCellVerts : cellVerts;
			}
		}

		private void TryShowMousePos()
		{
			if(sceneMousePos.Visible == false)
				return;

			var gridSpacing = GetGridSpacing();
			var mousePos = GetMousePosition();
			var inGrid = mousePos.PointToGrid(new(gridSpacing)) + new Vector2(gridSpacing) * 0.5f;
			sceneMousePos.Text =
				$"Cursor [{(int)mousePos.X} {(int)mousePos.Y}]\n" +
				$"Grid [{(int)inGrid.X} {(int)inGrid.Y}]";
		}
		private void TryUpdateGameDir()
		{
			if(Directory.Exists(finalGameDir) || selectGameDirShown)
				return;

			selectGameDirShown = true;

			if(finalGameDir != null)
				MessageBox.Show("Find the game directory or select a new one.", "Game directory has changed!");

			if(gameDir.ShowDialog(this) != DialogResult.OK)
			{
				Application.Exit();
				return;
			}
			selectGameDirShown = false;
			finalGameDir = gameDir.SelectedPath;

			var assetsPath = finalGameDir + "\\Assets";
			Directory.CreateDirectory(assetsPath);

			assetsWatcher.Path = assetsPath;
			assetsWatcher.Deleted += OnAssetDelete;
			assetsWatcher.Created += OnAssetCreate;
			assetsWatcher.Renamed += OnAssetRename;

			var mirrorPath = GetMirrorAssetPath(assetsPath);
			if(Directory.Exists(mirrorPath))
				DeleteMirrorFiles(assetsPath);

			CopyMirrorFiles(assetsPath);
			MainScene.LoadAsset(mirrorPath);
		}
		private void TrySelect()
		{
			var left = Mouse.IsButtonPressed(Mouse.Button.Left);
			var click = left.Once("leftClick");

			if(isHoveringScene == false)
				return;

			var mousePos = GetMousePosition();
			var rawMousePos = Mouse.GetPosition(window);
			var uids = ThingManager.GetUIDs();
			var dragSelHitbox = GetDragSelectionHitbox();
			var clickedUIDs = new List<string>();
			var dist = selectStartPos.DistanceBetweenPoints(new(rawMousePos.X, rawMousePos.Y));
			var ctrl = Keyboard.IsKeyPressed(Keyboard.Key.LControl);
			var alt = Keyboard.IsKeyPressed(Keyboard.Key.LAlt);

			if(click)
			{
				if(ctrl == false && alt == false)
					selectedUIDs.Clear();

				var sum = 0;
				for(int i = 0; i < uids.Count; i++)
				{
					var uid = uids[i];
					var hitbox = GetHitbox(uid);
					if(hitbox == null)
						continue;

					TryTransformHitbox(uid);

					// don't count an object in a multi unselect cycle if it is not selected
					var unselectSpecialCase = alt && selectedUIDs.Contains(uid) == false;
					// same for opposite
					var oppositeSpecialCase = ctrl && selectedUIDs.Contains(uid);

					if(hitbox.ConvexContains(mousePos) && unselectSpecialCase == false && oppositeSpecialCase == false)
					{
						clickedUIDs.Add(uid);
						sum++;
					}
				}
				selectDepthIndex = clickedUIDs.Count == 0 ? -1 : (selectDepthIndex + 1).Limit(0, sum, Extensions.Limitation.Overflow);
			}
			else if(left && dist > 1)
			{
				if(ctrl == false && alt == false)
					selectedUIDs.Clear();
				for(int i = 0; i < uids.Count; i++)
				{
					var uid = uids[i];
					var hitbox = GetHitbox(uid);
					if(hitbox == null)
						continue;

					TryTransformHitbox(uid);

					if(dragSelHitbox != null && dragSelHitbox.ConvexContains(hitbox))
						SelectObject(uid);
				}
			}

			if(clickedUIDs.Count > 0)
				SelectObject(clickedUIDs[selectDepthIndex]);

			void SelectObject(string uid)
			{
				if(selectedUIDs.Contains(uid) == false)
					selectedUIDs.Add(uid);
				if(Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
					selectedUIDs.Remove(uid);
			}
		}
		private void TryDrawSelection()
		{
			for(int i = 0; i < selectedUIDs.Count; i++)
				Draw((Hitbox)ThingManager.Get(selectedUIDs[i], "Hitbox"));

			if(isDragSelecting)
				Draw(GetDragSelectionHitbox(), Keyboard.IsKeyPressed(Keyboard.Key.LAlt));

			void Draw(Hitbox? hitbox, bool unselect = false)
			{
				if(hitbox == null)
					return;

				var topL = hitbox.Lines[0].A;
				var topR = hitbox.Lines[0].B;
				var botR = hitbox.Lines[1].B;
				var botL = hitbox.Lines[2].B;
				var fillCol = unselect ? new Color(255, 180, 0, 100) : new Color(0, 180, 255, 100);
				var outCol = Color.White;
				var fill = new Vertex[]
				{
					new(topL.ToSFML(), fillCol),
					new(topR.ToSFML(), fillCol),
					new(botR.ToSFML(), fillCol),
					new(botL.ToSFML(), fillCol),
				};

				new Line(topL, topR).Draw(window, outCol, sceneSc * 4);
				new Line(topR, botR).Draw(window, outCol, sceneSc * 4);
				new Line(botR, botL).Draw(window, outCol, sceneSc * 4);
				new Line(botL, topL).Draw(window, outCol, sceneSc * 4);

				window.Draw(fill, PrimitiveType.Quads);
			}
		}
		private void TryDestroy()
		{
			var delClick = Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once("delete-selected-objects");

			if(confirmDeleteChildrenMsgShown == false && selectedUIDs.Count > 0 && delClick)
			{
				confirmDeleteChildrenMsgShown = true;
				var result = MessageBox.Show($"Also delete the children of the selected {{Things}}?", "Delete Selection", MessageBoxButtons.YesNoCancel);
				confirmDeleteChildrenMsgShown = false;
				var includeChildren = result == DialogResult.Yes;
				if(result == DialogResult.Cancel)
					return;

				var sel = new List<string>(selectedUIDs);
				for(int i = 0; i < sel.Count; i++)
				{
					var uid = selectedUIDs[i];

					selectedUIDs.Remove(uid);
					ThingManager.Destroy(uid, includeChildren);
				}
				UpdateThingPanel();
			}
		}
		private static void TryTransformHitbox(string uid)
		{
			if(ThingManager.HasGet(uid, "Hitbox") == false)
				return;

			var children = (List<string>)ThingManager.Get(uid, "ChildrenUIDs");
			var hitbox = (Hitbox)ThingManager.Get(uid, "Hitbox");
			hitbox.TransformLocalLines(uid);

			for(int i = 0; i < children.Count; i++)
				TryTransformHitbox(children[i]);
		}
		#endregion

		#region View
		private void SetView(Vector2 pos = default, float angle = 0, float scale = 1)
		{
			SetViewPosition(pos);
			SetViewAngle(angle);
			SetViewScale(scale);
		}
		private void SetViewPosition(Vector2 pos)
		{
			var view = window.GetView();
			view.Center = pos.ToSFML();
			window.SetView(view);
		}
		private void SetViewAngle(float angle)
		{
			var view = window.GetView();
			view.Rotation = angle;
			window.SetView(view);
		}
		private void SetViewScale(float scale)
		{
			sceneSc = scale.Limit(0.1f, 10f);
		}
		#endregion
		#region Get
		private static Hitbox? GetHitbox(string uid)
		{
			return ThingManager.HasGet(uid, "Hitbox") == false ? default : (Hitbox)ThingManager.Get(uid, "Hitbox");
		}
		private float GetGridSpacing()
		{
			return MathF.Max(gridSpacing.Text.ToNumber(), (float)gridSpacing.Minimum);
		}
		private Hitbox GetDragSelectionHitbox()
		{
			var mousePos = Mouse.GetPosition(window);
			var topLeft = new Vector2i((int)selectStartPos.X, (int)selectStartPos.Y);
			var botRight = new Vector2i(mousePos.X, mousePos.Y);
			var topRight = new Vector2i(botRight.X, topLeft.Y);
			var botLeft = new Vector2i(topLeft.X, botRight.Y);
			var tl = window.MapPixelToCoords(topLeft).ToSystem();
			var tr = window.MapPixelToCoords(topRight).ToSystem();
			var br = window.MapPixelToCoords(botRight).ToSystem();
			var bl = window.MapPixelToCoords(botLeft).ToSystem();

			return new Hitbox(tl, tr, br, bl, tl);
		}
		private Vector2 GetFormsMousePos()
		{
			var view = window.GetView();
			var scale = view.Size.ToSystem() / new Vector2(windowPicture.Width, windowPicture.Height);
			return new Vector2(MousePosition.X, MousePosition.Y) * scale;
		}
		private Vector2 GetMousePosition()
		{
			var mp = Mouse.GetPosition(window);
			var mp2 = window.MapPixelToCoords(new(mp.X, mp.Y), window.GetView());
			return new Vector2(mp2.X, mp2.Y);
		}
		#endregion
		#region Files
		private void CopyMirrorFiles(string path)
		{
			var targetPath = GetMirrorAssetPath(path);
			var targetDir = Path.GetDirectoryName(targetPath);

			if(string.IsNullOrWhiteSpace(targetDir) == false)
				Directory.CreateDirectory(targetDir);

			if(Path.HasExtension(path))
			{
				File.Copy(path, targetPath);
				return;
			}

			Directory.CreateDirectory(targetPath);

			var dirs = Directory.GetDirectories(path);
			var files = Directory.GetFiles(path);

			for(int i = 0; i < dirs?.Length; i++)
				CopyMirrorFiles(dirs[i]);
			for(int i = 0; i < files?.Length; i++)
				CopyMirrorFiles(files[i]);
		}
		private void DeleteMirrorFiles(string path)
		{
			var targetPath = GetMirrorAssetPath(path);
			var targetDir = Path.GetDirectoryName(targetPath);

			if(string.IsNullOrWhiteSpace(targetDir) == false)
				Directory.CreateDirectory(targetDir);

			if(Path.HasExtension(path))
			{
				File.Delete(targetPath);
				return;
			}

			var dirs = Directory.GetDirectories(path);
			var files = Directory.GetFiles(path);

			for(int i = 0; i < dirs?.Length; i++)
				DeleteMirrorFiles(dirs[i]);
			for(int i = 0; i < files?.Length; i++)
				DeleteMirrorFiles(files[i]);

			Directory.Delete(targetPath);
		}
		private string GetMirrorAssetPath(string path)
		{
			var targetPath = path;
			if(finalGameDir != null)
				targetPath = targetPath.Replace(finalGameDir + "\\", "");
			return targetPath;
		}
		#endregion
		#region Scene
		private void OnKeyDownObjectSearch(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Return || string.IsNullOrWhiteSpace(searchScene.Text))
				return;

			var bestGuessSymbolsMatch = 0;
			var bestGuess = default(string);
			var uids = ThingManager.GetUIDs();

			for(int i = 0; i < uids.Count; i++)
			{
				var curMatchingSymbols = 0;
				var uid = uids[i];

				for(int j = 0; j < uid.Length; j++)
				{
					if(searchScene.Text.Length <= j || uid[j] != searchScene.Text[j])
						break;

					curMatchingSymbols++;
				}

				if(curMatchingSymbols > bestGuessSymbolsMatch)
				{
					bestGuessSymbolsMatch = curMatchingSymbols;
					bestGuess = uid;
				}
			}

			if(bestGuess == null)
				return;

			searchScene.Text = "";
			selectedUIDs.Clear();
			selectedUIDs.Add(bestGuess);
			UpdateThingPanel();
			SetViewPosition((Vector2)ThingManager.Get(bestGuess, "Position"));
		}
		private void OnSaveClick(object sender, EventArgs e)
		{
			save.InitialDirectory = finalGameDir;
			if(save.ShowDialog(this) != DialogResult.OK)
				return;

			MainScene.SaveScene(save.FileName);

			static void CopyDirectory(string sourceDir, string destinationDir)
			{
				if(Directory.Exists(sourceDir) == false || Directory.Exists(destinationDir) == false)
					return;

				var dir = new DirectoryInfo(sourceDir);
				var dirs = dir.GetDirectories();

				Directory.CreateDirectory(destinationDir);

				foreach(var file in dir.GetFiles())
				{
					var targetFilePath = Path.Combine(destinationDir, file.Name);
					if(File.Exists(targetFilePath))
						File.Delete(targetFilePath);
					file.CopyTo(targetFilePath);
				}

				foreach(var subDir in dirs)
				{
					var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
					CopyDirectory(subDir.FullName, newDestinationDir);
				}
			}
		}
		private void OnLoadClick(object sender, EventArgs e)
		{
			if(load.ShowDialog(this) != DialogResult.OK)
				return;

			if(MessageBox.Show("Confirm? Any unsaved changes will be lost.", "Confirm Load", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;

			ThingManager.DestroyAll();

			Scene.CurrentScene = Scene.Load<MainScene>(load.FileName);
			MainScene.LoadAsset("Assets");
		}
		private void OnSceneScroll(object? sender, MouseEventArgs e)
		{
			var delta = e.Delta < 0 ? 0.1f : -0.1f;

			var pos = window.GetView().Center.ToSystem();
			var mousePos = GetMousePosition();
			var dist = pos.DistanceBetweenPoints(mousePos);

			if(selectedUIDs.Count == 0)
			{
				if(delta < 0)
				{
					pos = pos.PointMoveTowardPoint(mousePos, -dist * delta * 0.7f, false);
					SetViewPosition(pos);
				}
				SetViewScale(sceneSc + delta);
			}

			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var uid = selectedUIDs[i];
				var sc = (float)ThingManager.Get(uid, "Scale");

				ThingManager.Set(uid, "Scale", MathF.Max(sc + delta, 0.01f));
				TryTransformHitbox(uid);
			}

			UpdateThingPanel();
		}
		private void OnMouseLeaveScene(object sender, EventArgs e)
		{
			isHoveringScene = false;
			//sceneMousePos.Hide();
		}
		private void OnMouseEnterScene(object sender, EventArgs e)
		{
			isHoveringScene = true;
			//sceneMousePos.Show();
		}
		private void OnMouseMoveScene(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Middle)
			{
				var editIndex = editSelectionOptions.SelectedIndex;

				if(selectedUIDs.Count == 0)
				{
					var view = window.GetView();
					var pos = view.Center.ToSystem();

					if(editIndex == 0)
						SetViewPosition(Drag(pos, true));
					else if(editIndex == 1)
						SetViewAngle(DragAngle(pos, view.Rotation, true));
				}
				else
				{
					for(int i = 0; i < selectedUIDs.Count; i++)
					{
						var uid = selectedUIDs[i];
						var pos = (Vector2)ThingManager.Get(uid, "Position");
						var ang = (float)ThingManager.Get(uid, "Angle");


						if(editIndex == 0)
							ThingManager.Set(uid, "Position", Drag(pos, false, true));
						else if(editIndex == 1)
							ThingManager.Set(uid, "Angle", DragAngle(pos, ang));

						TryTransformHitbox(uid);
					}
				}

				Cursor.Current = editCursors[editIndex];
			}

			var gridSp = new Vector2((float)snap.Value);
			prevFormsMousePos = GetFormsMousePos();
			prevMousePos = GetMousePosition();
			prevFormsMousePosGrid = prevFormsMousePos.PointToGrid(gridSp) + gridSp * 0.5f;
		}
		private void OnMouseDownScene(object sender, MouseEventArgs e)
		{
			searchBox.Select();

			if(e.Button != MouseButtons.Left)
				return;

			isDragSelecting = true;
			var pos = Mouse.GetPosition(window);
			selectStartPos = new(pos.X, pos.Y);
		}
		private void OnMouseUpScene(object sender, MouseEventArgs e)
		{
			UpdateThingPanel();

			if(e.Button == MouseButtons.Right)
				rightClickPos = GetMousePosition();

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isDragSelecting = false;
		}
		#endregion
		#region SceneRightClick
		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
		}
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			var uid = ThingManager.CreateSprite("sprite");
			ThingManager.Set(uid, "Position", rightClickPos);
			ThingManager.Do(uid, "ApplyDefaultHitbox");
			ThingManager.Set(uid, "TexturePath", "Assets\\explosive.jpg");
			TryTransformHitbox(uid);

			var uid2 = ThingManager.CreateSprite("sprite");
			ThingManager.Set(uid2, "Position", rightClickPos);
			ThingManager.Do(uid2, "ApplyDefaultHitbox");
			ThingManager.Set(uid2, "TexturePath", "Assets\\explosive.jpg");
			ThingManager.Set(uid2, "ParentUID", uid);
			TryTransformHitbox(uid2);
		}
		#endregion
		#region EditThingPanel
		private void OnListSelectThing(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;
			var uid = (string)list.SelectedItem;
			listBoxPlaceholderTexts.TryGetValue(list.Name, out var placeholder);

			if(list.Name == "Children UIDs" && uid != placeholder && ThingManager.Exists(uid))
			{
				SetViewPosition((Vector2)ThingManager.Get(uid, "Position"));
				selectedUIDs.Clear();
				selectedUIDs.Add(uid);
				UpdateThingPanel();
			}
		}
		private void OnListThingDropDown(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;

			if(listBoxPlaceholderTexts.ContainsKey(list.Name))
				list.Items.Remove(listBoxPlaceholderTexts[list.Name]);

			list.SelectedIndex = -1;
		}
		private void OnListThingDropDownClose(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;
			var hasPlaceholder = listBoxPlaceholderTexts.ContainsKey(list.Name);
			if(list.SelectedIndex != -1 && hasPlaceholder)
				return;

			if(hasPlaceholder)
			{
				var placeholder = listBoxPlaceholderTexts[list.Name];
				list.Items.Add(placeholder);
				list.SelectedItem = placeholder;
			}

			if(list.Name == "Types")
			{
				var types = (List<string>)ThingManager.Get(selectedUIDs[0], "Types");
				list.SelectedItem = types[0];
			}
		}
		#endregion
		#region Assets
		private void OnAssetRename(object sender, RenamedEventArgs e)
		{
			MainScene.UnloadAsset(e.OldFullPath);
			MainScene.LoadAsset(e.FullPath);
		}
		private void OnAssetCreate(object sender, FileSystemEventArgs e)
		{
			CopyMirrorFiles(e.FullPath);
			MainScene.LoadAsset(GetMirrorAssetPath(e.FullPath));
		}
		private void OnAssetDelete(object sender, FileSystemEventArgs e)
		{
			DeleteMirrorFiles(e.FullPath);
			MainScene.UnloadAsset(GetMirrorAssetPath(e.FullPath));
		}
		#endregion

		#region Utility
		private Vector2 Drag(Vector2 point, bool reverse = false, bool snapToGrid = false)
		{
			var view = window.GetView();
			var prev = snapToGrid ? prevFormsMousePosGrid : prevFormsMousePos;
			var pos = GetFormsMousePos();
			var gridSp = new Vector2((float)snap.Value);

			if(snapToGrid)
				pos = pos.PointToGrid(gridSp) + gridSp * 0.5f;

			var dist = prev.DistanceBetweenPoints(pos);
			var ang = prev.AngleBetweenPoints(pos);

			if(reverse)
				dist *= -1;

			return dist == 0 ? point : point.PointMoveAtAngle(view.Rotation + ang, dist, false);
		}
		private float DragAngle(Vector2 point, float angle, bool reverse = false)
		{
			var snapValue = (float)snap.Value;
			var curAng = point.AngleBetweenPoints(GetMousePosition());
			var prevAng = point.AngleBetweenPoints(prevMousePos);
			curAng = AngToGrid(curAng, snapValue);
			prevAng = AngToGrid(prevAng, snapValue);
			var delta = curAng - prevAng;
			return angle + (reverse ? -delta : delta);
		}

		private static float AngToGrid(float ang, float gridSz)
		{
			return new Vector2(ang).PointToGrid(new(gridSz)).X;
		}
		#endregion
	}
}