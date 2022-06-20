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
			{ "PropChildrenUIDs", "(select to focus)                                     " },
			{ "PropTypes", "(select to edit)                                     " },
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
		private readonly Dictionary<string, TableLayoutPanel> editThingTableTypes = new();
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

			InitTables();
		}
		private void InitTables()
		{
			var thing = CreateDefaultTable("tableThing");
			var sprite = CreateDefaultTable("tableSprite");
			var visual = CreateDefaultTable("tableVisual");
			var types = thingTypesTable;

			AddThingProperty(types, "Types", "PropTypes", typeof(List<string>));

			AddPropsThing();
			AddPropsSprite();
			AddPropsVisual();

			rightTable.Controls.Add(thing, 0, 1);

			editThingTableTypes[thing.Name] = thing;
			editThingTableTypes[sprite.Name] = sprite;
			editThingTableTypes[visual.Name] = visual;

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
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Texture Path", "PropTexturePath", typeof(string));
				AddThingProperty(visual, "Shader Path", "PropShaderPath", typeof(string));
				AddThingProperty(visual, ""); AddThingProperty(visual, "");
				AddThingProperty(visual, "Camera UID", "PropCameraUID", typeof(string));
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
			else if(valueType == typeof(bool))
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
			}

			if(last)
				table.Controls.Add(prop, 0, 24);
			else
				table.Controls.Add(prop);

			if(valueType != typeof(Button))
				table.Controls.Add(lab);

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
					control.BackColor = System.Drawing.Color.FromArgb(col.A, col.R, col.G, col.B);
				}
				else if(type == "BlendModes")
					ProcessEnumList((ComboBox)control, typeof(ThingManager.BlendModes), propName);

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
			if(ThingManager.HasGetter(uid, "Hitbox") == false)
				return;

			var children = (List<string>)ThingManager.Get(uid, "ChildrenUIDs");
			var hitbox = (Hitbox)ThingManager.Get(uid, "Hitbox");
			ThingManager.CallVoid(uid, "ApplyDefaultHitbox");
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
			return ThingManager.HasGetter(uid, "Hitbox") == false ? default : (Hitbox)ThingManager.Get(uid, "Hitbox");
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
			ThingManager.CallVoid(uid, "ApplyDefaultHitbox");
			TryTransformHitbox(uid);
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
				SetViewPosition((Vector2)ThingManager.Get(selectedItem, "Position"));
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
			if(sender == null)
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

			TryTransformHitbox(uid);
			UpdateThingPanel();


		}
		private void OnCheckBoxChange(object? sender, EventArgs e)
		{
			if(sender == null)
				return;
			var checkBox = (CheckBox)sender;
			var propName = checkBox.Name["Prop".Length..];
			var uid = selectedUIDs[0];
			ThingManager.Set(uid, propName, checkBox.Checked);
			TryTransformHitbox(uid);
			UpdateThingPanel();
		}
		private void OnNumericChange(object? sender, EventArgs e)
		{
			if(sender == null)
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

			TryTransformHitbox(uid);
			UpdateThingPanel();
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