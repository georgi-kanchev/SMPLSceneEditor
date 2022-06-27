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
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1f;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene, confirmDeleteChildrenMsgShown, selectGameDirShown, saveSceneShown, quitting;
		private Vector2 prevFormsMousePos, prevMousePos, prevFormsMousePosGrid, selectStartPos, rightClickPos;
		private readonly List<string> selectedUIDs = new();
		private readonly Cursor[] editCursors = new Cursor[] { Cursors.NoMove2D, Cursors.Cross, Cursors.SizeAll };
		private readonly FileSystemWatcher assetsWatcher, assetsFolderWatcher;
		private string prevAssetsMirrorPath = "Assets", assetsPath = "Assets", finalGameDir = "", pickThingProperty = "";
		private readonly Dictionary<string, TableLayoutPanel> editThingTableTypes = new();
		private readonly Dictionary<string, string> listBoxPlaceholderTexts = new()
		{
			{ "PropChildrenUIDs", "(select to focus)                                     " },
			{ "PropTypes", "(select to edit)                                     " },
		};
		private readonly Dictionary<string, Color> nonVisualTypeColors = new()
		{
			{ "Camera", Color.Red },
			{ "Light", Color.Yellow },
		};
		#endregion

		#region Init
		public FormWindow()
		{
			InitializeComponent();
			UpdateThingPanel();

			DeleteCache();

			WindowState = FormWindowState.Maximized;
			window = new(windowPicture.Handle);
			window.SetVerticalSyncEnabled(false);
			windowPicture.MouseWheel += OnSceneScroll;

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			FormClosing += OnAppQuitting;

			editSelectionOptions.SelectedIndex = 0;
			Scene.CurrentScene = new MainScene();

			// these directories are changed upon updating the game dir
			assetsWatcher = new("C:\\") { EnableRaisingEvents = true };
			assetsFolderWatcher = new("C:\\") { EnableRaisingEvents = true };
			assetsFolderWatcher.Renamed += OnAssetFolderRename;
			assetsWatcher.Deleted += OnAssetDelete;
			assetsWatcher.Created += OnAssetCreate;
			assetsWatcher.Renamed += OnAssetRename;

			InitTables();
			SetView();

			var desktopRes = Screen.PrimaryScreen.Bounds.Size;
			ThingManager.CreateCamera(Scene.MAIN_CAMERA_UID, new(desktopRes.Width, desktopRes.Height));
		}

		private void InitTables()
		{
			var thing = CreateDefaultTable("tableThing");
			var sprite = CreateDefaultTable("tableSprite");
			var visual = CreateDefaultTable("tableVisual");
			var light = CreateDefaultTable("tableLight");
			var camera = CreateDefaultTable("tableCamera");
			var types = thingTypesTable;

			AddThingProperty(types, "Types", "PropTypes", typeof(List<string>));

			AddPropsThing();
			AddPropsSprite();
			AddPropsVisual();
			AddPropsLight();
			AddPropsCamera();

			rightTable.Controls.Add(thing, 0, 1);

			editThingTableTypes[thing.Name] = thing;
			editThingTableTypes[sprite.Name] = sprite;
			editThingTableTypes[visual.Name] = visual;
			editThingTableTypes[light.Name] = light;
			editThingTableTypes[camera.Name] = camera;

			TableLayoutPanel CreateDefaultTable(string name)
			{
				var result = new TableLayoutPanel
				{
					ColumnCount = 2,
					RowCount = 24,
					Dock = DockStyle.Fill,
					Name = name
				};
				for(int i = 0; i < 2; i++)
					result.ColumnStyles.Add(new(SizeType.Percent, 50));
				for(int i = 0; i < 24; i++)
					result.RowStyles.Add(new(SizeType.Percent, 100 / 24f));

				return result;
			}
			void AddPropsThing()
			{
				AddThingProperty(thing, "Self", rightLabel: true); AddThingProperty(thing, "Properties", null);
				AddThingProperty(thing, "UID", "PropUID", typeof(string));
				AddThingProperty(thing, "Old UID", "PropOldUID", typeof(string), readOnly: true);
				AddThingProperty(thing, "Age", "PropAgeTimer", typeof(string), readOnly: true);
				AddThingProperty(thing, "Update Order", "PropUpdateOrder", typeof(int));
				AddThingProperty(thing, "Position", "PropPosition", typeof(Vector2));
				AddThingProperty(thing, "Angle", "PropAngle", typeof(float));
				AddThingProperty(thing, "Direction", "PropDirection", typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(thing, "Scale", "PropScale", typeof(float), smallNumericStep: true);
				AddThingProperty(thing, ""); AddThingProperty(thing, "");
				AddThingProperty(thing, "Parent", rightLabel: true); AddThingProperty(thing, "Properties");
				AddThingProperty(thing, "Parent UID", "PropParentUID", typeof(string));
				AddThingProperty(thing, "Parent Old UID", "PropParentOldUID", typeof(string), readOnly: true);
				AddThingProperty(thing, "Children UIDs", "PropChildrenUIDs", typeof(List<string>));
				AddThingProperty(thing, "Local Position", "PropLocalPosition", typeof(Vector2));
				AddThingProperty(thing, "Local Angle", "PropLocalAngle", typeof(float));
				AddThingProperty(thing, "Local Direction", "PropLocalDirection", typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(thing, "Local Scale", "PropLocalScale", typeof(float), smallNumericStep: true);

			}
			void AddPropsSprite()
			{
				AddThingProperty(sprite, "Local Size", "PropLocalSize", typeof(Vector2));
				AddThingProperty(sprite, "Size", "PropSize", typeof(Vector2));
				AddThingProperty(sprite, ""); AddThingProperty(sprite, "");
				AddThingProperty(sprite, "Origin Unit", "PropOriginUnit", typeof(Vector2), smallNumericStep: true);
				AddThingProperty(sprite, "Origin", "PropOrigin", typeof(Vector2));
				AddThingProperty(sprite, ""); AddThingProperty(sprite, "");
				AddThingProperty(sprite, "Texture Coordinates Unit A", "PropTexCoordsUnitA", typeof(Vector2), labelSizeOffset: -3, smallNumericStep: true);
				AddThingProperty(sprite, "Texture Coordinates Unit B", "PropTexCoordsUnitB", typeof(Vector2), labelSizeOffset: -3, smallNumericStep: true);
			}
			void AddPropsVisual()
			{
				AddThingProperty(visual, "Is Hidden", "PropIsHidden", typeof(bool));
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Depth", "PropDepth", typeof(int));
				AddThingProperty(visual, "Tint", "PropTint", typeof(Color));
				AddThingProperty(visual, "Blend Mode", "PropBlendMode", typeof(ThingManager.BlendModes));
				AddThingProperty(visual, "Effect", "PropEffect", typeof(ThingManager.Effects));
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Texture Path", "PropTexturePath", typeof(string));
				AddThingProperty(visual, "Has Smooth Texture", "PropHasSmoothTexture", typeof(bool));
				AddThingProperty(visual, "Has Repeated Texture", "PropHasRepeatedTexture", typeof(bool));
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Camera UIDs", "PropCameraUIDs", typeof(List<string>));
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Hitbox", "PropHitbox", typeof(Hitbox));
			}
			void AddPropsLight()
			{
				AddThingProperty(light, "Color", "PropColor", typeof(Color));
			}
			void AddPropsCamera()
			{
				AddThingProperty(camera, "Resolution", "PropResolution", typeof(Vector2), readOnly: true);
				AddThingProperty(camera, "Is Smooth", "PropIsSmooth", typeof(bool));
			}
		}
		private void AddThingProperty(TableLayoutPanel table, string label, string? propName = null, Type? valueType = null, bool readOnly = false,
			bool rightLabel = false, bool smallNumericStep = false, bool last = false, float labelSizeOffset = 3)
		{
			const string FONT = "Segoe UI";
			const float FONT_SIZE = 12f;

			var prop = default(Control);
			var lab = new Label() { Text = label };
			SetDefault(lab, FONT_SIZE + labelSizeOffset, reverseColors: true);
			lab.Enabled = true;
			lab.TextAlign = ContentAlignment.MiddleLeft;

			if(valueType == null)
			{
				lab.TextAlign = rightLabel ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
				table.Controls.Add(lab);
				return;
			}

			if(valueType == typeof(string))
			{
				prop = new TextBox();
				prop.TextChanged += OnTextBoxChange;
			}
			else if(valueType == typeof(bool) || valueType == typeof(Hitbox))
			{
				prop = new CheckBox();
				((CheckBox)prop).CheckedChanged += OnCheckBoxChange;
			}
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
				prop = CreateList();
			else if(valueType == typeof(Button))
				prop = new Button() { Text = label };
			else if(valueType == typeof(Vector2))
				prop = CreateMultipleValuesTable(2, false);
			else if(valueType == typeof(Color))
				prop = CreateMultipleValuesTable(4, true);
			else if(valueType.IsEnum)
				prop = CreateList();

			if(prop != null)
			{
				SetDefault(prop);
				lab.ForeColor = prop.Enabled ? System.Drawing.Color.White : System.Drawing.Color.Gray;

				if(valueType == typeof(Hitbox))
				{
					prop.BackColor = System.Drawing.Color.DarkGreen;
					prop.Text = "Edit";
				}
			}

			if(last)
				table.Controls.Add(prop, 0, 24);
			else
				table.Controls.Add(prop);

			if(valueType != typeof(Button))
				table.Controls.Add(lab);

			if(propName != null)
			{
				if(propName.Contains("Path"))
				{
					var btn = new Button() { Text = "Assets", Dock = DockStyle.Right };
					btn.Click += PickAsset;
					lab.Controls.Add(btn);
				}
				else if(readOnly == false && valueType == typeof(string) && propName != "PropUID" && propName.Contains("UID"))
				{
					var btn = new Button() { Text = "Things", Dock = DockStyle.Right, Width = 80 };
					btn.Click += PickThing;
					lab.Controls.Add(btn);
				}
				else if(valueType == typeof(Color))
				{
					var btn = new Button() { Text = "Colors", Dock = DockStyle.Right, Width = 80 };
					btn.Click += PickColor;
					lab.Controls.Add(btn);
				}
			}

			void PickThing(object? sender, EventArgs e)
			{
				var uids = ThingManager.GetUIDs();

				thingsList.MaximumSize = new(thingsList.MaximumSize.Width, 300);
				thingsList.Items.Clear();
				for(int i = 0; i < uids.Count; i++)
					thingsList.Items.Add(uids[i]);

				if(sender == null)
					return;
				var control = (Control)sender;

				pickThingProperty = propName.Replace("Prop", "");
				// show twice cuz first time has wrong position
				Show();
				Show();

				var t = thingsList;

				void Show() => thingsList.Show(this, control.PointToScreen(new(-thingsList.Width, -control.Height / 2)));
			}
			void PickColor(object? sender, EventArgs e)
			{
				if(pickColor.ShowDialog() != DialogResult.OK)
					return;

				var c = pickColor.Color;
				var property = propName.Replace("Prop", "");
				var uid = selectedUIDs[0];
				var col = (Color)ThingManager.Get(uid, property);

				ThingManager.Set(uid, property, new Color(c.R, c.G, c.B, col.A));
				UpdateThingPanel();
			}
			void PickAsset(object? sender, EventArgs e)
			{
				pickAsset.InitialDirectory = assetsPath;
				if(pickAsset.ShowDialog() != DialogResult.OK)
					return;

				var asset = pickAsset.FileName;
				var property = propName.Replace("Prop", "");
				var uid = selectedUIDs[0];

				ThingManager.Set(uid, property, GetMirrorAssetPath(asset));
				UpdateThingPanel();
			}
			void SetDefault(Control control, float fontSize = FONT_SIZE, bool reverseColors = false)
			{
				var black = System.Drawing.Color.Black;
				var white = System.Drawing.Color.White;

				if(control is Label)
					reverseColors = !reverseColors;

				control.Enabled = readOnly == false;
				control.Font = new System.Drawing.Font(FONT, fontSize, FontStyle.Regular, GraphicsUnit.Point);
				control.BackColor = reverseColors ? white : black;
				control.ForeColor = reverseColors ? black : white;
				control.Name = propName;
				control.TabStop = false;
				control.Dock = DockStyle.Fill;
				control.TabStop = true;

				if(control is CheckBox c)
					c.CheckAlign = ContentAlignment.MiddleRight;
			}
			void SetDefaultNumeric(NumericUpDown numeric, bool isInt)
			{
				numeric.Increment = (decimal)(smallNumericStep ? 0.1 : 1);
				numeric.DecimalPlaces = isInt ? 0 : (smallNumericStep ? 3 : 1);
				numeric.Minimum = int.MinValue;
				numeric.Maximum = int.MaxValue;
				numeric.ValueChanged += OnNumericChange;
			}
			TableLayoutPanel CreateMultipleValuesTable(int columns, bool isInt)
			{
				var vecTable = new TableLayoutPanel();
				SetDefault(vecTable);
				vecTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
				for(int i = 0; i < columns; i++)
					vecTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
				vecTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
				vecTable.ColumnCount = columns;
				vecTable.RowCount = 1;
				for(int i = 0; i < columns; i++)
				{
					var col = new NumericUpDown { BorderStyle = BorderStyle.None };
					SetDefault(col);
					SetDefaultNumeric(col, isInt);
					col.Name += i.ToString();
					vecTable.Controls.Add(col);
				}

				return vecTable;
			}
			ComboBox CreateList()
			{
				var list = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
				list.DropDown += new EventHandler(OnListThingDropDown);
				list.SelectedIndexChanged += new EventHandler(OnListSelectThing);
				list.DropDownClosed += new EventHandler(OnListThingDropDownClose);
				return list;
			}
		}
		#endregion
		#region Update
		private void OnUpdate(object? sender, EventArgs e)
		{
			SMPL.Tools.Time.Update();
			TryUpdateGameDir();

			window.Size = new((uint)windowPicture.Width, (uint)windowPicture.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.Clear();
			DrawGrid();
			UpdateSceneValues();

			TrySelect();

			ThingManager.UpdateAllThings();
			ThingManager.DrawAllVisuals(window);
			DrawAllNonVisuals();

			TryDrawSelection();

			window.Display();

			TryDestroy();
			TryCtrlS();
		}
		private void UpdateThingPanel()
		{
			rightTable.Visible = selectedUIDs.Count == 1;
			selectThingTip.Visible = selectedUIDs.Count != 1;

			if(rightTable.Visible == false)
				return;

			var uid = selectedUIDs[0];
			var props = ThingManager.GetPropertiesInfo(uid);

			for(int i = 0; i < props.Count; i++)
			{
				var propName = props[i].Name;
				var controlName = $"Prop{propName}";
				var type = props[i].Type;

				var c = Controls.Find(controlName, true);
				if(c == null || c.Length == 0)
					continue;

				var control = c[0];
				var readOnly = props[i].HasSetter == false;

				if(type == "String")
					SetText((TextBox)control, (string)Get(), readOnly);
				else if(type == "Int32")
					SetNumber((NumericUpDown)control, (int)Get(), readOnly);
				else if(type == "Boolean")
					SetTick((CheckBox)control, (bool)Get(), readOnly);
				else if(type == "Single")
					SetNumber((NumericUpDown)control, (float)Get(), readOnly);
				else if(type == "List<String>")
					ProcessThingList((ComboBox)control, propName);
				else if(type == "Vector2")
				{
					var table = (TableLayoutPanel)control;
					var vec = (Vector2)Get();
					SetNumber((NumericUpDown)table.Controls[0], vec.X);
					SetNumber((NumericUpDown)table.Controls[1], vec.Y);
				}
				else if(type == "Color")
				{
					var table = (TableLayoutPanel)control;
					var col = (Color)Get();
					SetNumber((NumericUpDown)table.Controls[0], col.R);
					SetNumber((NumericUpDown)table.Controls[1], col.G);
					SetNumber((NumericUpDown)table.Controls[2], col.B);
					SetNumber((NumericUpDown)table.Controls[3], col.A);
					control.BackColor = System.Drawing.Color.FromArgb(255, col.R, col.G, col.B);
				}
				else if(type == "BlendModes")
					ProcessEnumList((ComboBox)control, typeof(ThingManager.BlendModes), propName);
				else if(type == "Effects")
					ProcessEnumList((ComboBox)control, typeof(ThingManager.Effects), propName);
				else if(type == "Hitbox")
				{
					var tick = (CheckBox)control;
					tick.BackColor = System.Drawing.Color.YellowGreen;
					tick.ForeColor = System.Drawing.Color.Black;
				}

				object Get() => ThingManager.Get(uid, propName);
			}

			void ProcessThingList(ComboBox list, string propName)
			{
				list.Items.Clear();

				var propList = (List<string>)ThingManager.Get(uid, propName);
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
			void ProcessEnumList(ComboBox list, Type enumType, string propName)
			{
				if(enumType.IsEnum == false)
					return;

				var names = Enum.GetNames(enumType);
				list.Items.Clear();
				for(int i = 0; i < names.Length; i++)
					list.Items.Add(names[i]);

				list.SelectedIndex = (int)ThingManager.Get(uid, propName);
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
			void SetTick(CheckBox tick, bool value, bool readOnly = false)
			{
				tick.Enabled = readOnly == false;
				tick.Checked = value;
				tick.BackColor = tick.Checked ? System.Drawing.Color.Green : System.Drawing.Color.Red;
			}
		}
		private void DrawAllNonVisuals()
		{
			var uids = ThingManager.GetUIDs();
			for(int i = 0; i < uids.Count; i++)
			{
				var uid = uids[i];
				var type = ((List<string>)ThingManager.Get(uid, "Types"))[0];

				if(nonVisualTypeColors.ContainsKey(type) == false)
					continue;

				var boundingBox = (Hitbox)ThingManager.Get(uid, "BoundingBox");
				boundingBox.Draw(window, nonVisualTypeColors[type], sceneSc * 4);
			}
		}

		private void ResetThingPanel()
		{
			if(rightTable.Controls.ContainsKey("tableThing"))
				return;
			rightTable.Controls.Clear();
			rightTable.Controls.Add(thingTypesTable);
			rightTable.Controls.Add(editThingTableTypes["tableThing"]);
		}

		private void UpdateSceneValues()
		{
			var gridSpacing = GetGridSpacing();
			var mousePos = GetMousePosition();
			var inGrid = mousePos.PointToGrid(new(gridSpacing)) + new Vector2(gridSpacing) * 0.5f;
			sceneValues.Text =
				$"FPS [{SMPL.Tools.Time.FPS:F0}]\n" +
				$"Cursor [{(int)mousePos.X} {(int)mousePos.Y}]\n" +
				$"Grid [{(int)inGrid.X} {(int)inGrid.Y}]";
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

		private void TryCtrlS()
		{
			if(saveSceneShown == false && ActiveForm == this &&
				Keyboard.IsKeyPressed(Keyboard.Key.LControl) && Keyboard.IsKeyPressed(Keyboard.Key.S).Once("ctrl-s-save-scene"))
				TrySaveScene();
		}
		private void TrySaveScene()
		{
			saveSceneShown = true;
			if(string.IsNullOrWhiteSpace(sceneName.Text) || MainScene.SaveScene(Path.Join(finalGameDir, sceneName.Text + ".scene")) == false)
			{
				if(quitting)
					return;

				MessageBox.Show(this, "Enter a valid Scene name before saving.", "Failed to Save!");
				saveSceneShown = false;
				return;
			}
			saveSceneShown = false;

			var prevSceneName = prevAssetsMirrorPath.Replace(" Assets", "");
			var prevScenePath = finalGameDir + "\\" + prevSceneName + ".scene";
			if(sceneName.Text != prevSceneName && File.Exists(prevScenePath))
				File.Delete(prevScenePath);

			var path = sceneName.Text + " Assets";
			assetsPath = finalGameDir + "\\" + path;
			if(Directory.Exists(prevAssetsMirrorPath) && prevAssetsMirrorPath != path)
			{
				Directory.Move(prevAssetsMirrorPath, path);

				if(Directory.Exists(assetsPath) == false)
					Directory.Move(finalGameDir + "\\" + prevAssetsMirrorPath, assetsPath);
			}
			else
				Directory.CreateDirectory(assetsPath);
			prevAssetsMirrorPath = path;
			assetsFolderWatcher.Path = finalGameDir;
			assetsFolderWatcher.Filter = path;
			assetsWatcher.Path = assetsPath;
		}
		private void TryUpdateGameDir()
		{
			if(Directory.Exists(finalGameDir) || selectGameDirShown)
				return;

			selectGameDirShown = true;

			if(string.IsNullOrWhiteSpace(finalGameDir) == false)
				MessageBox.Show("Find the game directory or select a new one.", "Game directory has changed!");

			while(gameDir.ShowDialog(this) != DialogResult.OK) { }

			selectGameDirShown = false;
			finalGameDir = gameDir.SelectedPath;

			var name = sceneName.Text;
			name += string.IsNullOrWhiteSpace(name) ? "" : " ";
			var assetsPath = finalGameDir + "\\" + name + "Assets";
			if(string.IsNullOrWhiteSpace(name))
				return;

			Directory.CreateDirectory(assetsPath);

			assetsFolderWatcher.Path = finalGameDir;
			assetsFolderWatcher.Filter = name + "Assets";
			assetsWatcher.Path = assetsPath;

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
					var hitbox = GetBoundingBox(uid);
					if(hitbox == null)
						continue;

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
			else if(left && dist > 5)
			{
				if(ctrl == false && alt == false)
					selectedUIDs.Clear();
				for(int i = 0; i < uids.Count; i++)
				{
					var uid = uids[i];
					var hitbox = GetBoundingBox(uid);
					if(hitbox == null)
						continue;

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
				Draw((Hitbox)ThingManager.Get(selectedUIDs[i], "BoundingBox"));

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

				for(int i = 0; i < selectedUIDs.Count; i++)
				{
					var pos = (Vector2)ThingManager.Get(selectedUIDs[i], "Position");
					pos.DrawPoint(window, Color.Red, sceneSc * 4);
				}

				window.Draw(fill, PrimitiveType.Quads);
			}
		}
		private void TryDestroy()
		{
			var delClick = Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once("delete-selected-objects");

			if(ActiveForm == this && confirmDeleteChildrenMsgShown == false && selectedUIDs.Count > 0 && delClick)
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
		#endregion

		#region View
		private void SetView(Vector2 pos = default, float angle = 0, float scale = 1.5f)
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

		private void FocusThing(string uid)
		{
			SetViewPosition((Vector2)ThingManager.Get(uid, "Position"));

			var bb = (Hitbox)ThingManager.Get(uid, "BoundingBox");
			var dist1 = bb.Lines[0].A.DistanceBetweenPoints(bb.Lines[0].B);
			var dist2 = bb.Lines[1].A.DistanceBetweenPoints(bb.Lines[1].B);
			var scale = (dist1 > dist2 ? dist1 : dist2) / window.Size.X * 2f;
			SetViewScale(scale + 0.5f);
		}
		#endregion
		#region Get
		private static Hitbox? GetBoundingBox(string uid)
		{
			return ThingManager.HasGetter(uid, "BoundingBox") == false ? default : (Hitbox)ThingManager.Get(uid, "BoundingBox");
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
				if(File.Exists(targetPath) && IsFileLocked(targetPath) == false)
					File.Delete(targetPath);

				if(File.Exists(path))
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
			path = path.Replace(AppContext.BaseDirectory, "");

			if(Directory.Exists(Path.GetDirectoryName(path)) == false)
				return;

			var targetPath = GetMirrorAssetPath(path);
			var targetDir = Path.GetDirectoryName(targetPath);

			if(string.IsNullOrWhiteSpace(targetDir) == false)
				Directory.CreateDirectory(targetDir);

			DeleteFiles(targetPath);
		}
		private void DeleteFiles(string path)
		{
			if(Path.HasExtension(path))
			{
				File.Delete(path);
				return;
			}

			var dirs = Directory.GetDirectories(path);
			var files = Directory.GetFiles(path);

			for(int i = 0; i < dirs?.Length; i++)
				DeleteFiles(dirs[i]);
			for(int i = 0; i < files?.Length; i++)
				DeleteFiles(files[i]);

			Directory.Delete(path);
		}
		private string GetMirrorAssetPath(string path)
		{
			var targetPath = path;
			if(finalGameDir != null)
				targetPath = targetPath.Replace(finalGameDir + "\\", "");
			return targetPath;
		}
		private void DeleteCache()
		{
			var dirs = Directory.GetDirectories(AppContext.BaseDirectory);
			for(int i = 0; i < dirs.Length; i++)
				if(Path.GetFileName(dirs[i]) != "runtimes")
					DeleteFiles(dirs[i]);
		}
		#endregion
		#region Scene
		private void OnKeyDownObjectSearch(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Return || string.IsNullOrWhiteSpace(searchScene.Text))
				return;

			var prio = 0;
			var bestGuess = default(string);
			var uids = ThingManager.GetUIDs();

			for(int i = 0; i < uids.Count; i++)
			{
				var uid = uids[i];

				if(searchScene.Text == uid)
				{
					bestGuess = uid;
					break;
				}

				var contains = uid.Contains(searchScene.Text);
				var lowerContains = uid.ToLower().Contains(searchScene.Text.ToLower());
				var sameLength = uid.Length == searchScene.Text.Length;

				if(contains && sameLength)
				{
					prio = 2;
					bestGuess = uid;
				}
				else if(prio < 2 && lowerContains && sameLength)
				{
					prio = 1;
					bestGuess = uid;
				}
				else if(prio < 1 && (lowerContains || sameLength))
				{
					prio = 1;
					bestGuess = uid;
				}
			}

			if(bestGuess == null)
				return;

			searchScene.Text = "";
			selectedUIDs.Clear();
			selectedUIDs.Add(bestGuess);
			UpdateThingPanel();
			FocusThing(bestGuess);
		}
		private void OnSaveClick(object sender, EventArgs e)
		{
			TrySaveScene();
		}
		private void OnLoadClick(object sender, EventArgs e)
		{
			if(load.ShowDialog(this) != DialogResult.OK)
				return;

			if(MessageBox.Show("Confirm? Any unsaved changes will be lost.", "Confirm Load", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;

			ThingManager.DestroyAll();

			var scene = Scene.Load<MainScene>(load.FileName);
			if(scene == null)
			{
				MessageBox.Show("Could not load the selected Scene. The file may be altered or corrupt.", "Failed to Load");
				return;
			}

			sceneName.Text = Path.GetFileNameWithoutExtension(load.FileName);
			Scene.CurrentScene = scene;
			var name = sceneName.Text;
			name += string.IsNullOrWhiteSpace(name) ? "" : " ";
			var mirrorPath = name + "Assets";
			var path = finalGameDir + "\\" + mirrorPath;
			while(IsFileLocked(path)) { }
			CopyMirrorFiles(path);
			MainScene.LoadAsset(mirrorPath);
			assetsPath = finalGameDir + "\\" + mirrorPath;
			prevAssetsMirrorPath = mirrorPath;

			assetsFolderWatcher.Filter = mirrorPath;
			assetsFolderWatcher.Path = finalGameDir;
			assetsWatcher.Path = assetsPath;
		}
		private void OnSceneScroll(object? sender, MouseEventArgs e)
		{
			var delta = e.Delta / 1200f * -1f;
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

				ThingManager.Set(uid, "Scale", sc + delta);
			}

			UpdateThingPanel();
		}
		private void OnMouseLeaveScene(object sender, EventArgs e)
		{
			isHoveringScene = false;
		}
		private void OnMouseEnterScene(object sender, EventArgs e)
		{
			isHoveringScene = true;
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
			if(e.Button == MouseButtons.Left)
				ResetThingPanel();

			UpdateThingPanel();

			if(e.Button == MouseButtons.Right)
				rightClickPos = GetMousePosition();

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isDragSelecting = false;
		}
		#endregion
		#region SceneRightClick
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			var uid = ThingManager.CreateSprite("Sprite");
			ThingManager.Set(uid, "Position", rightClickPos);
			var cams = (List<string>)ThingManager.Get(uid, "CameraUIDs");
			cams.Add("camera");
		}
		private void OnSceneRightClickMenuCreateLight(object sender, EventArgs e)
		{
			var uid = ThingManager.CreateLight("Light");
			ThingManager.Set(uid, "Position", rightClickPos);
		}
		private void OnSceneRightclickMenuCreateCamera(object sender, EventArgs e)
		{
			var res = new Vector2();
			var minRes = new Vector2(1, 1);
			var maxRes = new Vector2(7680, 4320);
			var input = GetInput();

			while(InputIsInvalid(input, out res))
				input = GetInput();

			if(input == "")
				return;

			if(res.X.IsBetween(minRes.X, maxRes.X) == false ||
				res.Y.IsBetween(minRes.Y, maxRes.Y) == false)
			{
				Error();
				return;
			}

			var uid = ThingManager.CreateCamera("Camera", res);
			ThingManager.Set(uid, "Position", rightClickPos);

			string GetInput()
			{
				return FormWindow.GetInput(
					"Camera Resolution",
					"Provide the desired Camera resolution.\nExample: '1920 1080', '600 600'",
					"1000 1000");
			}
			bool InputIsInvalid(string input, out Vector2 resolution)
			{
				resolution = new();
				if(string.IsNullOrWhiteSpace(input))
					return false;

				var split = input.Split(' ');
				if(split.Length != 2 || split[0].IsNumber() == false || split[1].IsNumber() == false)
				{
					Error();
					return true;
				}

				resolution = new(split[0].ToNumber(), split[1].ToNumber());
				return false;
			}
			void Error()
			{
				MessageBox.Show($"The provided Camera resolution is invalid.\n" +
					$"A valid resolution is between [{minRes.X} {minRes.Y}] and [{maxRes.X} {maxRes.Y}].");
			}
		}

		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
		}
		private void OnSceneRightClickMenuDeselect(object sender, EventArgs e)
		{
			selectedUIDs.Clear();
			ResetThingPanel();
		}
		#endregion
		#region EditThingPanel
		private void OnListSelectThing(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;
			var selectedItem = (string)list.SelectedItem;
			listBoxPlaceholderTexts.TryGetValue(list.Name, out var placeholder);

			if(selectedItem == placeholder)
				return;

			if(list.Name == "PropChildrenUIDs" && ThingManager.Exists(selectedItem))
			{
				FocusThing(selectedItem);
				selectedUIDs.Clear();
				selectedUIDs.Add(selectedItem);
				UpdateThingPanel();
			}
			else if(list.Name == "PropTypes")
			{
				var table = editThingTableTypes[$"table{list.SelectedItem}"];

				list.Items.Add(placeholder);
				list.SelectedItem = placeholder;

				rightTable.Controls.Clear();
				rightTable.Controls.Add(thingTypesTable);
				rightTable.Controls.Add(table);
				UpdateThingPanel();
			}
			else if(list.Name == "PropBlendMode")
			{
				var index = list.SelectedIndex;
				ThingManager.Set(selectedUIDs[0], "BlendMode", (ThingManager.BlendModes)index);
			}
			else if(list.Name == "PropEffect")
			{
				var index = list.SelectedIndex;
				ThingManager.Set(selectedUIDs[0], "Effect", (ThingManager.Effects)index);
			}
		}
		private void OnListThingDropDown(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;

			if(listBoxPlaceholderTexts.ContainsKey(list.Name))
			{
				list.Items.Remove(listBoxPlaceholderTexts[list.Name]);
				list.SelectedIndex = -1;
			}
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
		}

		private void OnTextBoxChange(object? sender, EventArgs e)
		{
			if(sender == null || ((Control)sender).Focused == false) // ignore if it isn't the user changing the value
				return;
			var textBox = (TextBox)sender;
			var propName = textBox.Name["Prop".Length..];
			var uid = selectedUIDs[0];

			var prop = ThingManager.GetPropertyInfo(uid, propName);
			if(prop.HasSetter)
				ThingManager.Set(uid, propName, textBox.Text);

			// if the uid was changed, update the selection and hitbox uid
			if(propName == "UID" && string.IsNullOrWhiteSpace(textBox.Text) == false)
			{
				uid = textBox.Text;
				selectedUIDs[0] = uid;
			}

			UpdateThingPanel();
		}
		private void OnCheckBoxChange(object? sender, EventArgs e)
		{
			if(sender == null || ((Control)sender).Focused == false) // ignore if it isn't the user changing the value
				return;

			var checkBox = (CheckBox)sender;
			var propName = checkBox.Name["Prop".Length..];
			var uid = selectedUIDs[0];

			if(propName == "Hitbox")
			{

			}
			else
				ThingManager.Set(uid, propName, checkBox.Checked);
			UpdateThingPanel();
		}
		private void OnNumericChange(object? sender, EventArgs e)
		{
			if(sender == null || ((Control)sender).Focused == false) // ignore if it isn't the user changing the value
				return;

			var numeric = (NumericUpDown)sender;
			var propName = numeric.Name["Prop".Length..];
			var vecColIndex = 0;
			if(numeric.Parent.Name.StartsWith("Prop")) // is vector or color
			{
				vecColIndex = (int)propName[^1].ToString().ToNumber();
				propName = propName[..^1];
			}

			var uid = selectedUIDs[0];
			var propType = ThingManager.GetPropertyInfo(uid, propName).Type;
			var valueFloat = (float)numeric.Value;
			var valueInt = (int)numeric.Value;

			if(propType == "Vector2")
			{
				var vec = (Vector2)ThingManager.Get(uid, propName);
				if(vecColIndex == 0) vec.X = valueFloat;
				else if(vecColIndex == 1) vec.Y = valueFloat;
				ThingManager.Set(uid, propName, vec);
			}
			else if(propType == "Color")
			{
				var col = (Color)ThingManager.Get(uid, propName);
				var val = (byte)valueInt;
				if(vecColIndex == 0) col.R = val;
				else if(vecColIndex == 1) col.G = val;
				else if(vecColIndex == 2) col.B = val;
				else if(vecColIndex == 3) col.A = val;
				ThingManager.Set(uid, propName, col);
			}
			else if(propType == "Int32")
				ThingManager.Set(uid, propName, valueInt);
			else if(propType == "Single")
				ThingManager.Set(uid, propName, valueFloat);

			UpdateThingPanel();
		}

		private void OnThingListPick(object sender, ToolStripItemClickedEventArgs e)
		{
			ThingManager.Set(selectedUIDs[0], pickThingProperty, e.ClickedItem.Text);
			UpdateThingPanel();
		}
		#endregion
		#region Assets
		private void OnAssetRename(object sender, RenamedEventArgs e)
		{
			DeleteMirrorFiles(e.OldFullPath);
			CopyMirrorFiles(e.FullPath);

			MainScene.UnloadAsset(GetMirrorAssetPath(e.OldFullPath));
			MainScene.LoadAsset(GetMirrorAssetPath(e.FullPath));
		}
		private void OnAssetCreate(object sender, FileSystemEventArgs e)
		{
			// bigger files need time to be created/copied/moved, this event triggers before the file is fully there, this prevents that
			while(IsFileLocked(e.FullPath)) { }

			CopyMirrorFiles(e.FullPath);
			MainScene.LoadAsset(GetMirrorAssetPath(e.FullPath));
		}
		private void OnAssetDelete(object sender, FileSystemEventArgs e)
		{
			DeleteMirrorFiles(e.FullPath);
			MainScene.UnloadAsset(GetMirrorAssetPath(e.FullPath));
		}

		private void OnAssetFolderRename(object sender, RenamedEventArgs e)
		{
			if(e.FullPath != assetsPath)
				Directory.Move(e.FullPath, assetsPath);
		}
		private void OnAppQuitting(object? sender, FormClosingEventArgs e)
		{
			var result = MessageBox.Show("Save Scene before quitting?", "Quit", MessageBoxButtons.YesNoCancel);
			if(result == DialogResult.Cancel)
			{
				e.Cancel = true;
				return;
			}
			else if(result == DialogResult.Yes)
			{
				DeleteCache();
				quitting = true;
				TrySaveScene();
			}
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
		private static string GetInput(string title, string text, string defaultInput = "")
		{
			var split = text.Split('\n');
			var spacing = new Vector2i(50, 20);
			var btnSz = new Vector2i(70, 25);
			var textBoxSz = new Vector2i(80, 25);
			var sz = new Vector2i(400, spacing.Y * 5 + btnSz.Y + split.Length * 15);
			var prompt = new Form()
			{
				Width = sz.X,
				Height = sz.Y,
				FormBorderStyle = FormBorderStyle.FixedToolWindow,
				Text = title,
				StartPosition = FormStartPosition.CenterScreen
			};
			var textLabel = new Label()
			{
				Left = spacing.X,
				Top = spacing.Y,
				Text = text,
				Width = sz.X - spacing.X * 2,
				Height = sz.Y - btnSz.Y - spacing.Y * 4
			};
			var textBox = new TextBox()
			{
				Left = spacing.X,
				Top = spacing.Y + textLabel.Height,
				Width = sz.X - btnSz.X - spacing.X * 3,
				Height = textBoxSz.Y,
				Text = defaultInput
			};
			var button = new Button()
			{
				Text = "OK",
				Left = sz.X - btnSz.X - spacing.X,
				Width = btnSz.X,
				Height = btnSz.Y,
				Top = spacing.Y + textLabel.Height,
				DialogResult = DialogResult.OK
			};
			button.Click += (sender, e) => { prompt.Close(); };
			prompt.Controls.Add(textBox);
			prompt.Controls.Add(button);
			prompt.Controls.Add(textLabel);
			prompt.AcceptButton = button;

			return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
		}
		private static bool IsFileLocked(string file)
		{
			try
			{
				if(Path.HasExtension(file) == false)
				{
					var dirs = Directory.GetDirectories(file);
					var files = Directory.GetFiles(file);

					for(int i = 0; i < dirs.Length; i++)
						if(IsFileLocked(dirs[i]))
							return true;

					for(int i = 0; i < files.Length; i++)
						if(IsFileLocked(files[i]))
							return true;

					return false;
				}

				using var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
				stream.Close();
			}
			catch(IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				return true;
			}

			//file is not locked
			return false;
		}
		#endregion
	}
}