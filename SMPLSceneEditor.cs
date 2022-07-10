global using System.Collections.ObjectModel;
global using System.Numerics;
global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;
global using SMPL;
global using SMPL.Tools;
global using Color = SFML.Graphics.Color;
global using BlendMode = SMPL.Thing.BlendMode;
global using Cursor = System.Windows.Forms.Cursor;

namespace SMPLSceneEditor
{
	public partial class FormWindow : Form
	{
		#region Fields
		private const string NO_ASSETS_MSG = "In order to use Assets, save this Scene or load another.\n\n" +
			"Upon doing that, a Scene file, alongside an Asset folder will be present in the provided game directory. " +
			"Fill that folder with Assets and point the Things in the Scene toward them.";
		private const string LOADING_ASSETS = "Processing assets...", LOADING_SCENE = "Loading Scene...";
		private const float HITBOX_POINT_EDIT_MAX_DIST = 20;

		private CheckBox? editHitbox, paintTile;
		private Control? waitingPickThingControl, tilePaletteCol;
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1f;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene;
		private Vector2 prevFormsMousePos, prevMousePos, prevFormsMousePosGrid, selectStartPos, rightClickPos;
		private readonly List<string> selectedUIDs = new();
		private readonly List<int> selectedHitboxPointIndexesA = new(), selectedHitboxPointIndexesB = new();
		private readonly Cursor[] editCursors = new Cursor[] { Cursors.NoMove2D, Cursors.Cross, Cursors.SizeAll };
		private FileSystemWatcher? assetsWatcher, assetsFolderWatcher;
		private string pickThingProperty = "", scenePath = "", loadingDescr = "", tilePaletteUID = "";
		private readonly Dictionary<string, TableLayoutPanel> editThingTableTypes = new();
		private readonly Dictionary<string, string> listBoxPlaceholderTexts = new()
		{
			{ "PropChildrenUIDs", "(select to focus)                                     " },
			{ "PropTypes", "(select to edit)                                     " },
			{ "PropCameraUIDs", "(select to focus)                                     " },
		};
		private readonly Dictionary<string, Color> typeColors = new()
		{
			{ "Camera", Color.Red },
			{ "Light", Color.Yellow },
			{ "Audio", Color.Magenta },
			{ "Text", new(150, 150, 150) },
			{ "Sprite", Color.White },
			{ "NinePatch", Color.White },
			{ "Tilemap", new(100, 60, 20) },
		};

		private Color bgCol = Color.Black, bbCol = Color.Cyan, selCol = new(0, 180, 255, 100), hitCol = new(0, 255, 0),
			gridCol = new(50, 50, 50), gridCol0 = Color.Yellow, gridCol1000 = Color.White;
		#endregion

		#region Init
		public FormWindow()
		{
			InitializeComponent();
			UpdateThingPanel();

			WindowState = FormWindowState.Maximized;
			window = new(windowPicture.Handle);
			window.SetVerticalSyncEnabled(false);
			windowPicture.MouseWheel += OnSceneScroll;

			loading.Parent = this;

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			FormClosing += OnAppQuitting;

			editSelectionOptions.SelectedIndex = 0;
			Scene.CurrentScene = new MainScene();

			InitTables();
			TryResetThingPanel();
			SetView();

			var desktopRes = Screen.PrimaryScreen.Bounds.Size;
			Thing.CreateCamera(Scene.MAIN_CAMERA_UID, new(desktopRes.Width, desktopRes.Height));
		}
		private void InitTables()
		{
			var scene = CreateDefaultTable("tableScene");
			var thing = CreateDefaultTable("tableThing");
			var sprite = CreateDefaultTable("tableSprite");
			var visual = CreateDefaultTable("tableVisual");
			var light = CreateDefaultTable("tableLight");
			var camera = CreateDefaultTable("tableCamera");
			var text = CreateDefaultTable("tableText");
			var ninePatch = CreateDefaultTable("tableNinePatch");
			var audio = CreateDefaultTable("tableAudio");
			var tilemap = CreateDefaultTable("tableTilemap");
			var types = thingTypesTable;

			types.Hide();
			AddThingProperty(types, "Types", Thing.Property.THING_TYPES, typeof(ReadOnlyCollection<string>));

			AddPropsScene();
			AddPropsThing();
			AddPropsSprite();
			AddPropsVisual();
			AddThingProperty(light, "Color", Thing.Property.LIGHT_COLOR, typeof(Color));
			AddPropsCamera();
			AddPropsText();
			AddThingProperty(ninePatch, "Border Size", Thing.Property.NINE_PATCH_BORDER_SIZE, typeof(float));
			AddPropsAudio();
			AddPropsTilemap();

			editThingTableTypes[scene.Name] = scene;
			editThingTableTypes[thing.Name] = thing;
			editThingTableTypes[sprite.Name] = sprite;
			editThingTableTypes[visual.Name] = visual;
			editThingTableTypes[light.Name] = light;
			editThingTableTypes[camera.Name] = camera;
			editThingTableTypes[text.Name] = text;
			editThingTableTypes[ninePatch.Name] = ninePatch;
			editThingTableTypes[audio.Name] = audio;
			editThingTableTypes[tilemap.Name] = tilemap;

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
			void AddSpace(TableLayoutPanel table)
			{
				AddThingProperty(table, ""); AddThingProperty(table, "");
			}
			void AddPropsScene()
			{
				AddThingProperty(scene, "SMPL Scene", rightLabel: true); AddThingProperty(scene, "Editor Colors");
				AddThingProperty(scene, "Background", "SceneBackgroundColor", typeof(Color));
				AddThingProperty(scene, "Grid", "SceneGridColor", typeof(Color));
				AddThingProperty(scene, "Grid 0", "SceneGrid0Color", typeof(Color));
				AddThingProperty(scene, "Grid 1000", "SceneGrid1000Color", typeof(Color));
				AddSpace(scene);
				AddThingProperty(scene, "Bound Box", "SceneBoundingBoxColor", typeof(Color));
				AddThingProperty(scene, "Select", "SceneSelectColor", typeof(Color));
				AddSpace(scene);
				AddThingProperty(scene, "Sprite", "SceneSpriteColor", typeof(Color));
				AddThingProperty(scene, "NinePatch", "SceneNinePatchColor", typeof(Color));
				AddThingProperty(scene, "Text", "SceneTextColor", typeof(Color));
				AddThingProperty(scene, "Tilemap", "SceneTilemapColor", typeof(Color));
				AddThingProperty(scene, "Camera", "SceneCameraColor", typeof(Color));
				AddThingProperty(scene, "Hitbox", "SceneHitboxColor", typeof(Color));
				AddThingProperty(scene, "Light", "SceneLightColor", typeof(Color));
				AddThingProperty(scene, "Audio", "SceneAudioColor", typeof(Color));
			}
			void AddPropsThing()
			{
				AddThingProperty(thing, "UID", Thing.Property.THING_UID, typeof(string));
				AddThingProperty(thing, "Old UID", Thing.Property.THING_OLD_UID, typeof(string), readOnly: true);
				AddThingProperty(thing, "Age (Seconds)", Thing.Property.THING_AGE, typeof(float), readOnly: true);
				AddSpace(thing);
				AddThingProperty(thing, "Position", Thing.Property.THING_POSITION, typeof(Vector2));
				AddThingProperty(thing, "Angle", Thing.Property.THING_ANGLE, typeof(float));
				AddThingProperty(thing, "Direction", Thing.Property.THING_DIRECTION, typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(thing, "Scale", Thing.Property.THING_SCALE, typeof(float), smallNumericStep: true);
				AddSpace(thing);
				AddThingProperty(thing, "Parent UID", Thing.Property.THING_PARENT_UID, typeof(string));
				AddThingProperty(thing, "Parent Old UID", Thing.Property.THING_PARENT_OLD_UID, typeof(string), readOnly: true);
				AddThingProperty(thing, "Children UIDs", Thing.Property.THING_CHILDREN_UIDS, typeof(ReadOnlyCollection<string>), thingList: true);
				AddSpace(thing);
				AddThingProperty(thing, "Local Position", Thing.Property.THING_LOCAL_POSITION, typeof(Vector2));
				AddThingProperty(thing, "Local Angle", Thing.Property.THING_LOCAL_ANGLE, typeof(float));
				AddThingProperty(thing, "Local Direction", Thing.Property.THING_LOCAL_DIRECTION, typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(thing, "Local Scale", Thing.Property.THING_LOCAL_SCALE, typeof(float), smallNumericStep: true);

			}
			void AddPropsSprite()
			{
				AddThingProperty(sprite, "Local Size", Thing.Property.SPRITE_LOCAL_SIZE, typeof(Vector2));
				AddThingProperty(sprite, "Size", Thing.Property.SPRITE_SIZE, typeof(Vector2));
				AddSpace(sprite);
				AddThingProperty(sprite, "Origin Unit", Thing.Property.SPRITE_ORIGIN_UNIT, typeof(Vector2), smallNumericStep: true);
				AddThingProperty(sprite, "Origin", Thing.Property.SPRITE_ORIGIN, typeof(Vector2));
				AddSpace(sprite);
				AddThingProperty(sprite, "Texture Coordinates Unit A", Thing.Property.SPRITE_TEX_COORD_UNIT_A, typeof(Vector2), labelSizeOffset: -3, smallNumericStep: true);
				AddThingProperty(sprite, "Texture Coordinates Unit B", Thing.Property.SPRITE_TEX_COORD_UNIT_B, typeof(Vector2), labelSizeOffset: -3, smallNumericStep: true);
				AddThingProperty(sprite, "Texture Coordinates A", Thing.Property.SPRITE_TEX_COORD_A, typeof(Vector2), labelSizeOffset: 0, smallNumericStep: true);
				AddThingProperty(sprite, "Texture Coordinates B", Thing.Property.SPRITE_TEX_COORD_B, typeof(Vector2), labelSizeOffset: 0, smallNumericStep: true);
			}
			void AddPropsVisual()
			{
				AddThingProperty(visual, "Texture Path", Thing.Property.VISUAL_TEXTURE_PATH, typeof(string));
				AddThingProperty(visual, "Tint", Thing.Property.VISUAL_TINT, typeof(Color));
				AddThingProperty(visual, "Depth", Thing.Property.VISUAL_DEPTH, typeof(int));
				AddThingProperty(visual, "Hitbox", Thing.Property.THING_HITBOX, typeof(string), readOnly: true);
				AddSpace(visual);
				AddThingProperty(visual, "Is Hidden", Thing.Property.VISUAL_IS_HIDDEN, typeof(bool));
				AddThingProperty(visual, "Is Repeated", Thing.Property.VISUAL_IS_REPEATED, typeof(bool));
				AddThingProperty(visual, "Is Smooth", Thing.Property.VISUAL_IS_SMOOTH, typeof(bool));
				AddSpace(visual);
				AddThingProperty(visual, "Effect", Thing.Property.VISUAL_EFFECT, typeof(Thing.Effect));
				AddThingProperty(visual, "Blend Mode", Thing.Property.VISUAL_BLEND_MODE, typeof(BlendMode));
				AddSpace(visual);
				AddThingProperty(visual, "Camera UIDs", Thing.Property.VISUAL_CAMERA_UIDS, typeof(List<string>), thingList: true);
			}
			void AddPropsCamera()
			{
				AddThingProperty(camera, "Resolution", Thing.Property.CAMERA_RESOLUTION, typeof(Vector2), readOnly: true);
				AddSpace(camera);
				AddThingProperty(camera, "Is Smooth", Thing.Property.CAMERA_IS_SMOOTH, typeof(bool));
			}
			void AddPropsText()
			{
				AddThingProperty(text, "Font Path", Thing.Property.TEXT_FONT_PATH, typeof(string));
				AddThingProperty(text, "Value", Thing.Property.TEXT_VALUE, typeof(string));
				AddThingProperty(text, "Color", Thing.Property.TEXT_COLOR, typeof(Color));
				AddThingProperty(text, "Style", Thing.Property.TEXT_STYLE, typeof(Text.Styles));
				AddSpace(text);
				AddThingProperty(text, "Origin Unit", Thing.Property.TEXT_ORIGIN_UNIT, typeof(Vector2));
				AddThingProperty(text, "Symbol Size", Thing.Property.TEXT_SYMBOL_SIZE, typeof(int));
				AddThingProperty(text, "Symbol Space", Thing.Property.TEXT_SYMBOL_SPACE, typeof(float));
				AddThingProperty(text, "Line Space", Thing.Property.TEXT_LINE_SPACE, typeof(float));
				AddSpace(text);
				AddThingProperty(text, "Outline Color", Thing.Property.TEXT_OUTLINE_COLOR, typeof(Color));
				AddThingProperty(text, "Outline Size", Thing.Property.TEXT_OUTLINE_SIZE, typeof(float));
			}
			void AddPropsAudio()
			{
				AddThingProperty(audio, "Path", Thing.Property.AUDIO_PATH, typeof(string));
				AddSpace(audio);
				AddThingProperty(audio, "Status", Thing.Property.AUDIO_STATUS, typeof(Thing.AudioStatus));
				AddThingProperty(audio, "Duration", Thing.Property.AUDIO_DURATION, typeof(float), readOnly: true);
				AddThingProperty(audio, "Progress (Seconds)", Thing.Property.AUDIO_PROGRESS, typeof(float));
				AddThingProperty(audio, "Progress Unit", Thing.Property.AUDIO_PROGRESS_UNIT, typeof(float), smallNumericStep: true);
				AddSpace(audio);
				AddThingProperty(audio, "Volume Unit", Thing.Property.AUDIO_VOLUME_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(audio, "Is Looping", Thing.Property.AUDIO_IS_LOOPING, typeof(bool));
				AddThingProperty(audio, "Is Global", Thing.Property.AUDIO_IS_GLOBAL, typeof(bool));
				AddThingProperty(audio, "Pitch Unit", Thing.Property.AUDIO_PITCH_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(audio, "Distance Fade", Thing.Property.AUDIO_FADE, typeof(float));
			}
			void AddPropsTilemap()
			{
				AddThingProperty(tilemap, "Tile Size", Thing.Property.TILEMAP_TILE_SIZE, typeof(Vector2));
				AddThingProperty(tilemap, "Tile Gap", Thing.Property.TILEMAP_TILE_GAP, typeof(Vector2));
				AddSpace(tilemap);
				AddThingProperty(tilemap, "Tile Brush", Thing.Property.TILEMAP_TILE_PALETTE, typeof(Dictionary<string, Thing.Tile>));
				AddThingProperty(tilemap, "Tiles", Thing.Property.TILEMAP_TILE_COUNT, typeof(int), readOnly: true);
			}
		}
		private void AddThingProperty(TableLayoutPanel table, string label, string? propName = null, Type? valueType = null, bool readOnly = false,
			bool rightLabel = false, bool smallNumericStep = false, bool last = false, float labelSizeOffset = 3,
			bool thingList = false, bool uniqueList = true)
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
			else if(valueType == typeof(List<string>) || valueType.IsEnum ||
				valueType == typeof(ReadOnlyCollection<string>) || valueType == typeof(Dictionary<string, Thing.Tile>))
				prop = CreateList();
			else if(valueType == typeof(Button))
				prop = new Button() { Text = label };
			else if(valueType == typeof(Vector2))
				prop = CreateMultipleValuesTable(2, false);
			else if(valueType == typeof(Color))
				prop = CreateMultipleValuesTable(4, true);

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

			if(propName == Thing.Property.THING_HITBOX)
			{
				editHitbox = new CheckBox() { Text = "Edit", Dock = DockStyle.Left };
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.ForeColor = System.Drawing.Color.White;
				lab.Controls.Add(editHitbox);
			}

			if(valueType == typeof(int) && propName == "TileCount")
			{
				paintTile = new CheckBox() { Text = "Paint", Dock = DockStyle.Left, Width = 75 };
				paintTile.CheckedChanged += OnPaintTileCheck;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.ForeColor = System.Drawing.Color.White;
				lab.Controls.Add(paintTile);
			}

			if(propName == null || readOnly)
				return;

			if(propName.Contains("Path"))
			{
				var btn = new Button() { Text = "Assets", Dock = DockStyle.Left };
				btn.Click += PickAsset;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			else if(valueType == typeof(string) && propName != Thing.Property.THING_UID && propName.Contains("UID"))
			{
				var btn = new Button() { Text = "Things", Dock = DockStyle.Left, Width = 80 };
				btn.Click += PickThingList;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			else if(valueType == typeof(Color))
			{
				var btn = new Button() { Text = "Colors", Dock = DockStyle.Left, Width = 80 };
				btn.Click += PickColor;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			else if(valueType == typeof(List<string>))
			{
				var btn = new Button() { Text = "Items", Dock = DockStyle.Left, Width = 70 };
				btn.Click += EditTextList;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			else if(valueType == typeof(Thing.Effect))
			{
				var btn = new Button() { Text = "Uniforms", Dock = DockStyle.Left, Width = 100 };
				btn.Click += SetUniform;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			else if(valueType == typeof(Dictionary<string, Thing.Tile>))
			{
				var btn = new Button() { Text = "Palette", Dock = DockStyle.Left, Width = 80 };
				btn.Click += EditTilePalette;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}

			void PickThingList(object? sender, EventArgs e)
			{
				if(sender == null)
					return;
				var control = (Control)sender;
				PickThing(control, propName);
			}
			void PickColor(object? sender, EventArgs e)
			{
				if(sender == null || pickColor.ShowDialog() != DialogResult.OK)
					return;

				var c = pickColor.Color;
				var control = (Control)sender;
				var parName = control.Parent.Name;

				if(parName.StartsWith("PropScene"))
				{
					var cols = typeColors;
					switch(parName)
					{
						case "PropSceneBackgroundColor": bgCol = C(bgCol); break;
						case "PropSceneBoundingBoxColor": bbCol = C(bbCol); break;
						case "PropSceneSelectColor": selCol = C(selCol); break;
						case "PropSceneCameraColor": cols["Camera"] = C(cols["Camera"]); break;
						case "PropSceneHitboxColor": hitCol = C(hitCol); break;
						case "PropSceneLightColor": cols["Light"] = C(cols["Light"]); break;
						case "PropSceneSpriteColor": cols["Sprite"] = C(cols["Sprite"]); break;
						case "PropSceneNinePatchColor": cols["NinePatch"] = C(cols["NinePatch"]); break;
						case "PropSceneGridColor": gridCol = C(gridCol); break;
						case "PropSceneGrid0Color": gridCol0 = C(gridCol0); break;
						case "PropSceneGrid1000Color": gridCol1000 = C(gridCol1000); break;
						case "PropSceneAudioColor": cols["Audio"] = C(cols["Audio"]); break;
						case "PropSceneTilemapColor": cols["Tilemap"] = C(cols["Tilemap"]); break;
					}

					TryResetThingPanel();
					return;

					Color C(Color col) => new(c.R, c.G, c.B, col.A);
				}
				else if(parName.StartsWith("PropTilePalette"))
				{
					if(tilePaletteCol != null)
					{
						SetControlNumber((NumericUpDown)tilePaletteCol.Controls[0], c.R);
						SetControlNumber((NumericUpDown)tilePaletteCol.Controls[1], c.G);
						SetControlNumber((NumericUpDown)tilePaletteCol.Controls[2], c.B);
						SetControlNumber((NumericUpDown)tilePaletteCol.Controls[3], c.A);
						tilePaletteCol.BackColor = c;
					}
					return;
				}

				var uid = selectedUIDs[0];
				var col = (Color)Thing.Get(uid, propName);

				Thing.Set(uid, propName, new Color(c.R, c.G, c.B, col.A));
				UpdateThingPanel();
			}
			void PickAsset(object? sender, EventArgs e)
			{
				var assetsPath = GetAssetsPath();
				pickAsset.InitialDirectory = assetsPath;
				if(string.IsNullOrWhiteSpace(assetsPath))
				{
					MessageBox.Show(this, NO_ASSETS_MSG, "No Assets Found");
					return;
				}

				if(pickAsset.ShowDialog() != DialogResult.OK)
					return;

				var asset = pickAsset.FileName;
				var uid = selectedUIDs[0];

				Thing.Set(uid, propName, GetMirrorAssetPath(asset));
				UpdateThingPanel();
			}
			void EditTextList(object? sender, EventArgs e)
			{
				EditList("Edit List", (List<string>)Thing.Get(selectedUIDs[0], propName), thingList, uniqueList);
				UpdateThingPanel();
			}
			void SetUniform(object? sender, EventArgs e)
			{
				var uid = selectedUIDs[0];
				var effect = (Thing.Effect)Thing.Get(uid, Thing.Property.VISUAL_EFFECT);
				if(effect == Thing.Effect.None)
					return;
				var code = (Thing.CodeGLSL)Thing.CallGet(selectedUIDs[0], "GetEffectCode", effect);
				var frag = string.IsNullOrWhiteSpace(code.FragmentUniforms) ? "" : $"- Fragment Uniforms:{code.FragmentUniforms}";
				var vert = string.IsNullOrWhiteSpace(code.VertexUniforms) ? "" : $"- Vertex Uniforms:{code.VertexUniforms}";

				var input = GetInput($"Set {effect} Uniform", "- Syntax examples:\n" +
					"BoolName true\n" +
					"IntName 3\n" +
					"FloatName 0.5\n" +
					"Vec2Name 0.3 0.1\n" +
					"Vec3Name 25.0 13.5 2.0\n" +
					"Vec4Name 0.2 0.9 0.3 1.0\n" +
					"ColorName 1.0 0.8 0.2 1.0\n" +
					"ArrayName[ArrayIndex] ..." +
					$"\n\n{frag}{vert}");

				if(string.IsNullOrWhiteSpace(input))
					return;

				var a = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if(a[1] == "true")
					Thing.CallVoid(uid, "SetShaderBool", a[0], true);
				else if(a[1] == "false")
					Thing.CallVoid(uid, "SetShaderBool", a[0], false);
				else if(a.Length == 2 && a[1].IsNumber())
				{
					var n = a[1].ToNumber();
					Thing.CallVoid(uid, "SetShaderFloat", a[0], n);
					Thing.CallVoid(uid, "SetShaderInt", a[0], (int)n);
				}
				else if(a.Length == 3 && a[1].IsNumber() && a[2].IsNumber())
					Thing.CallVoid(uid, "SetShaderVector2", a[0], new Vector2(a[1].ToNumber(), a[2].ToNumber()));
				else if(a.Length == 4 && a[1].IsNumber() && a[2].IsNumber() && a[3].IsNumber())
					Thing.CallVoid(uid, "SetShaderVector3", a[0], new Vector3(a[1].ToNumber(), a[2].ToNumber(), a[3].ToNumber()));
				else if(a.Length == 5 && a[1].IsNumber() && a[2].IsNumber() && a[3].IsNumber() && a[4].IsNumber())
					Thing.CallVoid(uid, "SetShaderVector4", a[0], new Vector4(a[1].ToNumber(), a[2].ToNumber(), a[3].ToNumber(), a[4].ToNumber()));
				else
					MessageBox.Show(this, $"The {effect} uniform was not set due to invalid input.", $"Set {effect} Uniform Failed");
			}
			void EditTilePalette(object? sender, EventArgs e)
			{
				var palette = (Dictionary<string, Thing.Tile>)Thing.Get(selectedUIDs[0], Thing.Property.TILEMAP_TILE_PALETTE);
				var sz = new Vector2i(W + 120, H + 350);
				var window = new Form()
				{
					Text = "Edit Palette",
					Width = sz.X,
					Height = sz.Y,
					FormBorderStyle = FormBorderStyle.FixedToolWindow,
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White,
					StartPosition = FormStartPosition.CenterScreen
				};
				var listBox = new ListBox()
				{
					Left = SPACING_X,
					Top = SPACING_Y,
					Width = sz.X - SPACING_X * 2,
					Height = sz.Y / 3,
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White
				};
				var table = new TableLayoutPanel
				{
					Top = listBox.Height + SPACING_Y,
					Left = SPACING_X,
					Width = sz.X - SPACING_X * 2,
					Height = 185,
					ColumnCount = 2,
					RowCount = 4,
					Name = "PropTilePalette",
				};
				var textBox = new TextBox()
				{
					Left = SPACING_X,
					Top = table.Height + listBox.Height + SPACING_Y * 2,
					Width = sz.X - BUTTON_W * 2 - SPACING_X * 3,
					Height = TEXTBOX_H,
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White,
				};
				var buttonSet = new Button()
				{
					Top = table.Height + listBox.Height + SPACING_Y * 2,
					Left = sz.X - BUTTON_W * 2 - SPACING_X,
					Width = BUTTON_W,
					Height = BUTTON_H,
					Text = "Set",
				};
				var buttonRemove = new Button()
				{
					Top = table.Height + listBox.Height + SPACING_Y * 2,
					Left = sz.X - BUTTON_W - SPACING_X,
					Width = BUTTON_W,
					Height = BUTTON_H,
					Text = "Remove",
				};

				AddThingProperty(table, "Tile", rightLabel: true); AddThingProperty(table, "Properties");
				AddThingProperty(table, "Indecies Texture Coordinate", "TilePaletteTexCoord", valueType: typeof(Vector2), labelSizeOffset: -1);
				AddThingProperty(table, "Color", "TilePaletteColor", valueType: typeof(Color));
				AddThingProperty(table, "Depth", "TilePaletteDepth", valueType: typeof(int));
				AddThingProperty(table, "Indecies Size", "TilePaletteSize", valueType: typeof(Vector2));

				var texCoord = table.Controls[2];
				var color = table.Controls[4];
				var depth = table.Controls[6];
				var size = table.Controls[8];
				tilePaletteCol = color;

				color.BackColor = System.Drawing.Color.White;
				((NumericUpDown)color.Controls[0]).Value = 255;
				((NumericUpDown)color.Controls[1]).Value = 255;
				((NumericUpDown)color.Controls[2]).Value = 255;
				((NumericUpDown)color.Controls[3]).Value = 255;
				((NumericUpDown)size.Controls[0]).Value = 1;
				((NumericUpDown)size.Controls[1]).Value = 1;

				foreach(var kvp in palette)
					listBox.Items.Add(kvp.Key);

				if(palette.Count > 0 && prop != null)
					listBox.SelectedIndex = ((ComboBox)prop).SelectedIndex;

				window.FormClosed += (sender, e) =>
				{
					UpdateThingPanel();
					if(prop != null)
						((ComboBox)prop).SelectedIndex = listBox.SelectedIndex;
				};
				buttonSet.Click += (sender, e) =>
				{
					if(string.IsNullOrWhiteSpace(textBox.Text) != false)
						return;

					if(listBox.Items.Contains(textBox.Text) == false)
						listBox.Items.Add(textBox.Text);

					palette[textBox.Text] = new(
						new((float)((NumericUpDown)texCoord.Controls[0]).Value, (float)((NumericUpDown)texCoord.Controls[1]).Value),
						(int)((NumericUpDown)depth).Value,
						(float)((NumericUpDown)size.Controls[0]).Value,
						(float)((NumericUpDown)size.Controls[1]).Value,
						(byte)((NumericUpDown)color.Controls[0]).Value,
						(byte)((NumericUpDown)color.Controls[1]).Value,
						(byte)((NumericUpDown)color.Controls[2]).Value,
						(byte)((NumericUpDown)color.Controls[3]).Value);
					listBox.SelectedItem = textBox.Text;
				};
				buttonRemove.Click += (sender, e) =>
				{
					palette.Remove((string)listBox.SelectedItem);
					listBox.Items.Remove(listBox.SelectedItem);
					listBox.SelectedIndex = listBox.Items.Count == 0 ? -1 : 0;
				};
				listBox.SelectedIndexChanged += (sender, e) =>
				{
					if(listBox.SelectedIndex == -1)
						return;

					var uid = (string)listBox.SelectedItem;
					var tile = palette[uid];
					var texCoord = table.Controls[2];
					var color = table.Controls[4];
					var size = table.Controls[8];
					SetControlNumber((NumericUpDown)texCoord.Controls[0], tile.IndeciesTexCoord.X);
					SetControlNumber((NumericUpDown)texCoord.Controls[1], tile.IndeciesTexCoord.Y);
					SetControlNumber((NumericUpDown)color.Controls[0], tile.Color.R);
					SetControlNumber((NumericUpDown)color.Controls[1], tile.Color.G);
					SetControlNumber((NumericUpDown)color.Controls[2], tile.Color.B);
					SetControlNumber((NumericUpDown)color.Controls[3], tile.Color.A);
					SetControlNumber((NumericUpDown)depth, tile.Depth);
					color.BackColor = System.Drawing.Color.FromArgb(255, tile.Color.R, tile.Color.G, tile.Color.B);
					SetControlNumber((NumericUpDown)size.Controls[0], tile.IndeciesSize.X);
					SetControlNumber((NumericUpDown)size.Controls[1], tile.IndeciesSize.Y);

					textBox.Text = uid;
					tilePaletteUID = uid;
				};

				for(int i = 0; i < 2; i++)
					table.ColumnStyles.Add(new(SizeType.Percent, 50));
				for(int i = 0; i < 5; i++)
					table.RowStyles.Add(new(SizeType.Percent, 100 / 24f));

				window.Controls.Add(table);
				window.Controls.Add(listBox);
				window.Controls.Add(textBox);
				window.Controls.Add(buttonSet);
				window.Controls.Add(buttonRemove);
				window.ShowDialog(this);
			}
			void OnPaintTileCheck(object? sender, EventArgs e)
			{
				var palette = (Dictionary<string, Thing.Tile>)Thing.Get(selectedUIDs[0], "TilePalette");
				if(paintTile.Checked && palette.Count == 0)
					MessageBox.Show(this, "The Tile Palette is empty. Add tiles before painting.", "Paint Tile");
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
				control.Name = $"Prop{propName}";
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
				list.SelectedIndexChanged += new EventHandler(OnListSelectItem);
				list.DropDownClosed += new EventHandler(OnListThingDropDownClose);
				return list;
			}
		}
		#endregion
		#region Update
		private void OnUpdate(object? sender, EventArgs e)
		{
			loading.Visible = loadingDescr != "";
			loading.Text = loadingDescr;
			windowSplit.Visible = loading.Visible == false;

			SMPL.Tools.Time.Update();

			window.Size = new((uint)windowPicture.Width, (uint)windowPicture.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.Clear(bgCol);
			DrawGrid();
			UpdateSceneValues();

			TrySelect();
			TryPaintTile();

			Game.UpdateEngine(window);
			DrawAllNonVisuals();

			TryDrawSelection();

			window.Display();

			TryDestroy();
			TryCtrlS();
		}
		private void UpdateThingPanel()
		{
			if(selectedUIDs.Count != 1)
				return;

			var uid = selectedUIDs[0];
			var props = Thing.Info.GetProperties(uid);

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

				switch(type)
				{
					case "String": SetText((TextBox)control, (string)Get(), readOnly); break;
					case "Int32": SetControlNumber((NumericUpDown)control, (int)Get(), readOnly); break;
					case "Boolean": SetTick((CheckBox)control, (bool)Get(), readOnly); break;
					case "Single": SetControlNumber((NumericUpDown)control, (float)Get(), readOnly); break;
					case "List<String>": ProcessList((ComboBox)control, (List<string>)Thing.Get(uid, propName)); break;
					case "ReadOnlyCollection<String>": ProcessList((ComboBox)control, (ReadOnlyCollection<string>)Thing.Get(uid, propName)); break;
					case "BlendMode": ProcessEnumList((ComboBox)control, typeof(Thing.BlendMode), propName); break;
					case "Styles": ProcessEnumList((ComboBox)control, typeof(Text.Styles), propName); break;
					case "Effect": ProcessEnumList((ComboBox)control, typeof(Thing.Effect), propName); break;
					case "AudioStatus": ProcessEnumList((ComboBox)control, typeof(Thing.AudioStatus), propName); break;
					case "Hitbox": SetText((TextBox)control, $"{((Hitbox)Thing.Get(uid, propName)).Lines.Count} Lines", readOnly); break;
					case "Dictionary<String, Tile>": ProcessList((ComboBox)control, ((Dictionary<string, Thing.Tile>)Thing.Get(uid, Thing.Property.TILEMAP_TILE_PALETTE)).Keys); break;
					case "Vector2":
						{
							var table = (TableLayoutPanel)control;
							var vec = (Vector2)Get();
							SetControlNumber((NumericUpDown)table.Controls[0], vec.X);
							SetControlNumber((NumericUpDown)table.Controls[1], vec.Y);
							break;
						}
					case "Color":
						{
							var table = (TableLayoutPanel)control;
							var col = (Color)Get();
							SetControlNumber((NumericUpDown)table.Controls[0], col.R);
							SetControlNumber((NumericUpDown)table.Controls[1], col.G);
							SetControlNumber((NumericUpDown)table.Controls[2], col.B);
							SetControlNumber((NumericUpDown)table.Controls[3], col.A);
							control.BackColor = System.Drawing.Color.FromArgb(255, col.R, col.G, col.B);
							break;
						}
				}

				object Get() => Thing.Get(uid, propName);
			}

			void ProcessEnumList(ComboBox list, Type enumType, string propName)
			{
				if(enumType.IsEnum == false)
					return;

				var names = Enum.GetNames(enumType);
				list.Items.Clear();
				for(int i = 0; i < names.Length; i++)
					list.Items.Add(names[i]);

				list.SelectedIndex = (int)Thing.Get(uid, propName);
			}
			void SetText(TextBox text, string value, bool readOnly = false)
			{
				text.Enabled = readOnly == false;
				text.Text = value;
			}
			void SetTick(CheckBox tick, bool value, bool readOnly = false)
			{
				tick.Enabled = readOnly == false;
				tick.Checked = value;
				tick.BackColor = tick.Checked ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkRed;
			}
		}
		private void SetControlNumber(NumericUpDown number, float value, bool readOnly = false)
		{
			number.Enabled = readOnly == false;
			number.Value = (decimal)value.Limit((float)number.Minimum, (float)number.Maximum);
		}

		private void DrawAllNonVisuals()
		{
			var uids = Thing.GetUIDs();
			for(int i = 0; i < uids.Count; i++)
			{
				var uid = uids[i];
				var type = ((ReadOnlyCollection<string>)Thing.Get(uid, Thing.Property.THING_TYPES))[0];

				if(typeColors.ContainsKey(type) == false)
					continue;

				var boundingBox = (Hitbox)Thing.Get(uid, Thing.Property.THING_BOUNDING_BOX);
				boundingBox.Draw(window, typeColors[type], sceneSc * 4);
			}
		}
		private void ProcessList<T>(ComboBox control, ICollection<T> list)
		{
			control.Items.Clear();
			control.Enabled = list.Count > 0;

			TryAddPlaceholderToList(control);

			if(list.Count > 0)
				foreach(var item in list)
					control.Items.Add(item);

			control.SelectedIndex = 0;
		}
		private void TryAddPlaceholderToList(ComboBox control)
		{
			if(control.Enabled && listBoxPlaceholderTexts.ContainsKey(control.Name))
				control.Items.Add(listBoxPlaceholderTexts[control.Name]);
			else if(control.Enabled == false)
				control.Items.Add("(none)");
		}
		private void AddHitboxLine(string uid, List<Line> line)
		{
			var hitbox = (Hitbox)Thing.Get(uid, Thing.Property.THING_HITBOX);
			for(int i = 0; i < line.Count; i++)
				hitbox.LocalLines.Add(new Line(Local(line[i].A), Local(line[i].B)));
			UpdateThingPanel();

			Vector2 Local(Vector2 global) => (Vector2)Thing.CallGet(uid, Thing.Method.THING_GET_LOCAL_POSITION_FROM_SELF, global);
		}
		private void TrySelectThing(string uid)
		{
			if(selectedUIDs.Contains(uid))
				return;

			selectedUIDs.Add(uid);
			TryResetThingPanel();
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
					return gridCol0;
				else if(coordinate % 1000 == 0)
					return gridCol1000;

				return gridCol;
			}
			VertexArray GetVertexArray(float coordinate)
			{
				return coordinate == 0 || coordinate % 1000 == 0 ? specialCellVerts : cellVerts;
			}
		}
		private void TryPaintTile()
		{
			if(paintTile == null || paintTile.Checked == false || selectedUIDs.Count == 0)
				return;

			var uid = selectedUIDs[0];
			var tileIndecies = Thing.CallGet(uid, Thing.Method.TILEMAP_GET_TILE_INDECIES, GetMousePosition());

			if(Mouse.IsButtonPressed(Mouse.Button.Left))
			{

			}
		}
		private void TryResetThingPanel()
		{
			if((editHitbox != null && editHitbox.Checked) ||
				(paintTile != null && paintTile.Checked))
				return;

			var table = editThingTableTypes[selectedUIDs.Count == 1 ? "tableThing" : "tableScene"];
			if(rightTable.Controls.Contains(table) == false)
			{
				rightTable.Controls.Clear();
				thingTypesTable.Visible = selectedUIDs.Count == 1;
				rightTable.Controls.Add(thingTypesTable);
				rightTable.Controls.Add(table, 0, 1);
			}

			if(selectedUIDs.Count == 0)
			{
				SetColor("PropSceneBackgroundColor", bgCol);
				SetColor("PropSceneBoundingBoxColor", bbCol);
				SetColor("PropSceneSelectColor", selCol);
				SetColor("PropSceneCameraColor", typeColors["Camera"]);
				SetColor("PropSceneHitboxColor", hitCol);
				SetColor("PropSceneGridColor", gridCol);
				SetColor("PropSceneGrid0Color", gridCol0);
				SetColor("PropSceneGrid1000Color", gridCol1000);
				SetColor("PropSceneLightColor", typeColors["Light"]);
				SetColor("PropSceneAudioColor", typeColors["Audio"]);
				SetColor("PropSceneTextColor", typeColors["Text"]);
				SetColor("PropSceneSpriteColor", typeColors["Sprite"]);
				SetColor("PropSceneNinePatchColor", typeColors["NinePatch"]);
				SetColor("PropSceneTilemapColor", typeColors["Tilemap"]);

				void SetColor(string name, Color col)
				{
					var table = (TableLayoutPanel)editThingTableTypes["tableScene"].Controls.Find(name, true)[0];
					SetNumber((NumericUpDown)table.Controls[0], col.R);
					SetNumber((NumericUpDown)table.Controls[1], col.G);
					SetNumber((NumericUpDown)table.Controls[2], col.B);
					SetNumber((NumericUpDown)table.Controls[3], col.A);
					table.BackColor = System.Drawing.Color.FromArgb(255, col.R, col.G, col.B);
				}
				void SetNumber(NumericUpDown number, float value, bool readOnly = false)
				{
					number.Enabled = readOnly == false;
					number.Value = (decimal)value.Limit((float)number.Minimum, (float)number.Maximum);
				}
			}
		}
		private void TrySelect()
		{
			var left = Mouse.IsButtonPressed(Mouse.Button.Left);
			var click = left.Once("leftClick");

			if(isHoveringScene == false || (paintTile != null && paintTile.Checked))
				return;

			var mousePos = GetMousePosition();
			var rawMousePos = Mouse.GetPosition(window);
			var uids = Thing.GetUIDs();
			var dragSelHitbox = GetDragSelectionHitbox();
			var clickedUIDs = new List<string>();
			var dist = selectStartPos.DistanceBetweenPoints(new(rawMousePos.X, rawMousePos.Y));
			var ctrl = Keyboard.IsKeyPressed(Keyboard.Key.LControl);
			var alt = Keyboard.IsKeyPressed(Keyboard.Key.LAlt);
			var drag = left && dist > sceneSc * 5f;
			var editingHitbox = editHitbox != null && editHitbox.Checked;

			if(editingHitbox)
			{
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.THING_HITBOX);
				if(hitbox == null)
					return;

				if(click)
				{
					TryDeselect();

					var pointSelectDist = sceneSc * HITBOX_POINT_EDIT_MAX_DIST * 0.75f;
					for(int i = 0; i < hitbox.Lines.Count; i++)
					{
						if(mousePos.DistanceBetweenPoints(hitbox.Lines[i].A) < pointSelectDist)
						{
							SelectHitboxPointIndexA(i);
							break; // prevents multi selection with click (the only way to separate overlapping points)
						}
						else if(mousePos.DistanceBetweenPoints(hitbox.Lines[i].B) < pointSelectDist)
						{
							SelectHitboxPointIndexB(i);
							break; // prevents multi selection with click (the only way to separate overlapping points)
						}
					}
				}
				else if(drag && dragSelHitbox != null)
				{
					TryDeselect();

					for(int i = 0; i < hitbox.Lines.Count; i++)
					{
						if(dragSelHitbox.ConvexContains(hitbox.Lines[i].A))
							SelectHitboxPointIndexA(i);
						if(dragSelHitbox.ConvexContains(hitbox.Lines[i].B))
							SelectHitboxPointIndexB(i);
					}
				}
				return;

				void TryDeselect()
				{
					if(ctrl || alt)
						return;

					selectedHitboxPointIndexesA.Clear();
					selectedHitboxPointIndexesB.Clear();
				}
			}

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
			else if(drag)
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
						TrySelectOrDeselectThing(uid);
				}
			}

			if(clickedUIDs.Count > 0)
				TrySelectOrDeselectThing(clickedUIDs[selectDepthIndex]);

			void TrySelectOrDeselectThing(string uid)
			{
				if(selectedUIDs.Contains(uid) == false)
					selectedUIDs.Add(uid);
				if(Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
					selectedUIDs.Remove(uid);
			}
			void SelectHitboxPointIndexA(int index)
			{
				if(selectedHitboxPointIndexesA.Contains(index) == false)
					selectedHitboxPointIndexesA.Add(index);
				if(Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
					selectedHitboxPointIndexesA.Remove(index);
			}
			void SelectHitboxPointIndexB(int index)
			{
				if(selectedHitboxPointIndexesB.Contains(index) == false)
					selectedHitboxPointIndexesB.Add(index);
				if(Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
					selectedHitboxPointIndexesB.Remove(index);
			}
		}
		private void TryDrawSelection()
		{
			if(paintTile != null && paintTile.Checked)
				return;

			var hitboxSelectCol = hitCol;
			var r = hitCol.R / 2;
			var g = hitCol.G / 2;
			var b = hitCol.B / 2;
			var hitboxDeselectedCol = new Color((byte)r, (byte)g, (byte)b);

			if(isDragSelecting)
				DrawBoundingBox(GetDragSelectionHitbox(), Keyboard.IsKeyPressed(Keyboard.Key.LAlt));

			if(selectedUIDs.Count == 0)
				return;

			for(int i = 0; i < selectedUIDs.Count; i++)
				DrawBoundingBox((Hitbox)Thing.Get(selectedUIDs[i], Thing.Property.THING_BOUNDING_BOX));

			var ptsA = selectedHitboxPointIndexesA;
			var ptsB = selectedHitboxPointIndexesB;
			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[i], Thing.Property.THING_HITBOX);
				for(int j = 0; j < hitbox.Lines.Count; j++)
				{
					var col = ptsA.Contains(j) || ptsB.Contains(j) ? hitboxSelectCol : hitboxDeselectedCol;
					hitbox.Lines[j].Draw(window, col, sceneSc * 4);
				}
			}

			if(editHitbox == null || editHitbox.Checked == false)
				return;

			var lines = ((Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.THING_HITBOX)).Lines;
			var sz = sceneSc * HITBOX_POINT_EDIT_MAX_DIST;

			for(int j = 0; j < lines.Count; j++)
			{
				if(ptsA.Contains(j) == false)
					lines[j].A.DrawPoint(window, hitboxDeselectedCol, sz);
				if(ptsB.Contains(j) == false)
					lines[j].B.DrawPoint(window, hitboxDeselectedCol, sz);
			}
			// always draw selected points on top
			for(int j = 0; j < lines.Count; j++)
			{
				if(ptsA.Contains(j))
					lines[j].A.DrawPoint(window, hitboxSelectCol, sz);
				if(ptsB.Contains(j))
					lines[j].B.DrawPoint(window, hitboxSelectCol, sz);
			}

			void DrawBoundingBox(Hitbox? boundingBox, bool unselect = false)
			{
				if(boundingBox == null)
					return;

				var topL = boundingBox.Lines[0].A;
				var topR = boundingBox.Lines[0].B;
				var botR = boundingBox.Lines[1].B;
				var botL = boundingBox.Lines[2].B;
				var r = 255 - selCol.R;
				var g = 255 - selCol.G;
				var b = 255 - selCol.B;
				var fillCol = unselect ? new Color((byte)r, (byte)g, (byte)b, 100) : selCol;
				var fill = new Vertex[]
				{
					new(topL.ToSFML(), fillCol),
					new(topR.ToSFML(), fillCol),
					new(botR.ToSFML(), fillCol),
					new(botL.ToSFML(), fillCol),
				};

				boundingBox.Draw(window, bbCol, sceneSc * 4);

				for(int i = 0; i < selectedUIDs.Count; i++)
				{
					var pos = (Vector2)Thing.Get(selectedUIDs[i], Thing.Property.THING_POSITION);
					pos.DrawPoint(window, Color.Red, sceneSc * 4);
				}

				window.Draw(fill, PrimitiveType.Quads);
			}
		}
		private void TryDestroy()
		{
			if(ActiveForm != this || (paintTile != null && paintTile.Checked))
				return;

			var delClick = Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once("delete-selected-objects");

			if(delClick == false)
				return;

			if(editHitbox != null && editHitbox.Checked)
			{
				// work on copy lists since removing from the main list changes other indexes in the main list but not the selectedPts lists
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.THING_HITBOX);
				var lines = new List<Line>(hitbox.LocalLines);
				var ptsA = new List<int>(selectedHitboxPointIndexesA);
				var ptsB = new List<int>(selectedHitboxPointIndexesB);

				for(int i = 0; i < ptsA.Count; i++)
					Remove(ptsA[i]);
				for(int i = 0; i < ptsB.Count; i++)
					Remove(ptsB[i]);

				hitbox.LocalLines.Clear();
				for(int i = 0; i < lines.Count; i++)
					if(lines[i].A.IsNaN() == false && lines[i].B.IsNaN() == false)
						hitbox.LocalLines.Add(lines[i]);

				selectedHitboxPointIndexesA.Clear();
				selectedHitboxPointIndexesB.Clear();
				UpdateThingPanel();
				return;

				void Remove(int index) => lines[index] = new Line(new Vector2().NaN(), new Vector2().NaN());
			}

			if(selectedUIDs.Count > 0)
			{
				loop.Stop();
				var result = MessageBox.Show("Also delete the children of the selected Things?", "Delete Selection", MessageBoxButtons.YesNoCancel);
				loop.Start();
				var includeChildren = result == DialogResult.Yes;
				if(result == DialogResult.Cancel)
					return;

				var sel = new List<string>(selectedUIDs);
				for(int i = 0; i < sel.Count; i++)
				{
					var uid = sel[i];

					selectedUIDs.Remove(uid);
					Thing.Destroy(uid, includeChildren);
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
			if(Thing.Exists(uid) == false)
				return;

			SetViewPosition((Vector2)Thing.Get(uid, Thing.Property.THING_POSITION));

			var bb = (Hitbox)Thing.Get(uid, Thing.Property.THING_BOUNDING_BOX);
			var dist1 = bb.Lines[0].A.DistanceBetweenPoints(bb.Lines[0].B);
			var dist2 = bb.Lines[1].A.DistanceBetweenPoints(bb.Lines[1].B);
			var scale = (dist1 > dist2 ? dist1 : dist2) / window.Size.X * 2f;
			SetViewScale(scale + 0.5f);
		}
		#endregion
		#region Get
		private static Hitbox? GetBoundingBox(string uid)
		{
			return Thing.Info.HasGetter(uid, Thing.Property.THING_BOUNDING_BOX) == false ? default : (Hitbox)Thing.Get(uid, Thing.Property.THING_BOUNDING_BOX);
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
		private void TryCtrlS()
		{
			if(ActiveForm == this && Keyboard.IsKeyPressed(Keyboard.Key.LControl) &&
				Keyboard.IsKeyPressed(Keyboard.Key.S).Once("ctrl-s-save-scene"))
				TrySave();
		}
		private void TrySave()
		{
			if(File.Exists(scenePath))
				Save();
			else
				SaveAs();
		}
		private void SaveAs()
		{
			if(save.ShowDialog(this) != DialogResult.OK)
				return;

			Save();
		}
		private void Save()
		{
			scenePath = string.IsNullOrWhiteSpace(save.FileName) ? load.FileName : save.FileName;
			Loading("Saving Scene...");
			MainScene.SaveScene(scenePath);
			Loading("");

			TrackAssets();
		}

		private void TrackAssets()
		{
			var assetsPath = GetAssetsPath();
			Directory.CreateDirectory(assetsPath);

			if(assetsWatcher == null)
			{
				assetsWatcher = new(AppContext.BaseDirectory) { EnableRaisingEvents = true, IncludeSubdirectories = true };
				assetsWatcher.Deleted += OnAssetDelete;
				assetsWatcher.Created += OnAssetCreate;
				assetsWatcher.Renamed += OnAssetRename;
			}
			if(assetsFolderWatcher == null)
			{
				assetsFolderWatcher = new(AppContext.BaseDirectory) { EnableRaisingEvents = true };
				assetsFolderWatcher.Renamed += OnAssetFolderRename;
			}

			assetsFolderWatcher.Path = GetGameDirectory();
			assetsFolderWatcher.Filter = Path.GetFileNameWithoutExtension(assetsPath);
			assetsWatcher.Path = assetsPath;
		}
		private string GetAssetsPath()
		{
			var sceneName = Path.GetFileNameWithoutExtension(scenePath);
			return string.IsNullOrWhiteSpace(scenePath) ? "" : GetGameDirectory() + "\\" + sceneName + " Assets";
		}
		private string GetGameDirectory()
		{
			var dir = Path.GetDirectoryName(scenePath);
			return string.IsNullOrWhiteSpace(dir) ? "" : dir;
		}
		private string GetMirrorAssetPath(string path)
		{
			var targetPath = path;
			var gameDir = GetGameDirectory();
			if(string.IsNullOrWhiteSpace(gameDir) == false)
				targetPath = targetPath.Replace(gameDir + "\\", "");

			return targetPath;
		}
		private void CopyMirrorFiles(string path)
		{
			while(FileIsLocked(path))
				Thread.Sleep(100);

			var targetPath = GetMirrorAssetPath(path);
			var targetDir = Path.GetDirectoryName(targetPath);

			if(string.IsNullOrWhiteSpace(targetDir) == false)
				Directory.CreateDirectory(targetDir);

			if(Path.HasExtension(path))
			{
				if(File.Exists(targetPath) && FileIsLocked(targetPath) == false)
					File.Delete(targetPath);

				if(File.Exists(path) && File.Exists(targetPath) == false)
					File.Copy(path, targetPath);
				return;
			}

			Directory.CreateDirectory(targetPath);
			Directory.CreateDirectory(path);

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
			while(FileIsLocked(path))
				Thread.Sleep(100);

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
		private void DeleteCache()
		{
			var dirs = Directory.GetDirectories(AppContext.BaseDirectory);
			for(int i = 0; i < dirs.Length; i++)
				if(Path.GetFileName(dirs[i]) != "runtimes")
					DeleteFiles(dirs[i]);
		}
		private static bool FileIsLocked(string file)
		{
			try
			{
				if(File.Exists(file) == false)
					return false;
				if(Path.HasExtension(file) == false)
				{
					var dirs = Directory.GetDirectories(file);
					var files = Directory.GetFiles(file);

					for(int i = 0; i < dirs.Length; i++)
						if(FileIsLocked(dirs[i]))
							return true;

					for(int i = 0; i < files.Length; i++)
						if(FileIsLocked(files[i]))
							return true;

					return false;
				}

				using var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
				stream.Close();
			}
			catch(Exception)
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
		#region Scene
		private void OnKeyDownObjectSearch(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Return || string.IsNullOrWhiteSpace(searchScene.Text) || (editHitbox != null && editHitbox.Checked) ||
				(paintTile != null && paintTile.Checked))
				return;

			var prio = 0;
			var bestGuess = default(string);
			var uids = Thing.GetUIDs();

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
			TrySelectThing(bestGuess);
			UpdateThingPanel();
			FocusThing(bestGuess);
		}
		private void OnSaveAsClick(object sender, EventArgs e)
		{
			SaveAs();
		}
		private void OnSaveClick(object sender, EventArgs e)
		{
			TrySave();
		}
		private void OnLoadClick(object sender, EventArgs e)
		{
			load.InitialDirectory = GetGameDirectory();
			if(load.ShowDialog(this) != DialogResult.OK)
				return;

			if(MessageBox.Show("Confirm? Any unsaved changes will be lost.", "Confirm Load", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;

			scenePath = load.FileName;
			var assetsPath = GetAssetsPath();

			Loading(LOADING_ASSETS);
			CopyMirrorFiles(assetsPath);
			Loading(LOADING_SCENE);
			Thing.DestroyAll();

			var scene = Scene.Load<MainScene>(load.FileName);
			if(scene == null)
			{
				MessageBox.Show("Could not load Scene. The file may be altered or corrupt.", "Load Scene Failed");
				return;
			}

			Scene.CurrentScene = scene;
			MainScene.LoadAsset(GetMirrorAssetPath(assetsPath));

			TrackAssets();

			selectedUIDs.Clear();
			SetView();
			Loading();
		}
		private void OnSceneScroll(object? sender, MouseEventArgs e)
		{
			var delta = e.Delta / 1200f * -1f;
			var pos = window.GetView().Center.ToSystem();
			var mousePos = GetMousePosition();
			var dist = pos.DistanceBetweenPoints(mousePos);
			var editingHitbox = editHitbox != null && editHitbox.Checked;
			var ptsA = selectedHitboxPointIndexesA;
			var ptsB = selectedHitboxPointIndexesB;

			if(selectedUIDs.Count == 0 || (editingHitbox && ptsA.Count == 0 && ptsB.Count == 0) ||
				(paintTile != null && paintTile.Checked))
			{
				if(delta < 0)
				{
					pos = pos.PointMoveTowardPoint(mousePos, -dist * delta * 0.7f, false);
					SetViewPosition(pos);
				}
				SetViewScale(sceneSc + delta);
				return;
			}
			if(editingHitbox)
			{
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.THING_HITBOX);

				for(int i = 0; i < ptsA.Count; i++)
				{
					var line = hitbox.LocalLines[ptsA[i]];
					line.A += line.A * delta;
					hitbox.LocalLines[ptsA[i]] = line;
				}
				for(int i = 0; i < ptsB.Count; i++)
				{
					var line = hitbox.LocalLines[ptsB[i]];
					line.B += line.B * delta;
					hitbox.LocalLines[ptsB[i]] = line;
				}
				return;
			}
			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var uid = selectedUIDs[i];
				var sc = (float)Thing.Get(uid, Thing.Property.THING_SCALE);

				Thing.Set(uid, Thing.Property.THING_SCALE, sc + delta);
			}

			UpdateThingPanel();
		}
		private void OnMouseLeaveScene(object sender, EventArgs e)
		{
			isHoveringScene = false;
			isDragSelecting = false;
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
				var editHitboxPts = editHitbox != null && editHitbox.Checked;
				var ptsA = selectedHitboxPointIndexesA;
				var ptsB = selectedHitboxPointIndexesB;

				if(selectedUIDs.Count == 0 || (editHitboxPts && ptsA.Count == 0 && ptsB.Count == 0) ||
					(paintTile != null && paintTile.Checked))
				{
					var view = window.GetView();
					var pos = view.Center.ToSystem();

					if(editIndex == 0)
						SetViewPosition(Drag(pos, true));
					else if(editIndex == 1)
						SetViewAngle(DragAngle(pos, view.Rotation, true));
				}
				else if(editHitboxPts)
				{
					var uid = selectedUIDs[0];
					var hitbox = (Hitbox)Thing.Get(uid, Thing.Property.THING_HITBOX);
					var sc = (float)Thing.Get(uid, Thing.Property.THING_SCALE);

					if(ptsA.Count == 0 && ptsB.Count == 0)
						return;

					if(editIndex == 0)
					{
						for(int i = 0; i < ptsA.Count; i++)
						{
							var line = hitbox.LocalLines[ptsA[i]];
							hitbox.LocalLines[ptsA[i]] = new(TrySnap(Drag(line.A, false, true, sc), ptsA[i]), line.B);
						}
						for(int i = 0; i < ptsB.Count; i++)
						{
							var line = hitbox.LocalLines[ptsB[i]];
							hitbox.LocalLines[ptsB[i]] = new(line.A, TrySnap(Drag(line.B, false, true, sc), ptsB[i]));
						}

						Vector2 TrySnap(Vector2 draggedPos, int index)
						{
							var snapValue = (float)snap.Value;
							if(snapValue == 0)
								return draggedPos;

							for(int i = 0; i < hitbox.LocalLines.Count; i++)
							{
								if(i == index) // shouldn't be the same line
									continue;

								if(hitbox.LocalLines[i].A.DistanceBetweenPoints(draggedPos) < snapValue)
									return hitbox.LocalLines[i].A;
								else if(hitbox.LocalLines[i].B.DistanceBetweenPoints(draggedPos) < snapValue)
									return hitbox.LocalLines[i].B;
							}
							return draggedPos;
						}
					}
					else if(editIndex == 1)
					{
						var thingPos = (Vector2)Thing.Get(uid, Thing.Property.THING_POSITION);
						var a = thingPos.AngleBetweenPoints(GetMousePosition());
						var ang = DragAngle(thingPos, a);

						for(int i = 0; i < ptsA.Count; i++)
						{
							var line = hitbox.LocalLines[ptsA[i]];
							var matrix =
								Matrix3x2.CreateTranslation(line.A) *
								Matrix3x2.CreateRotation((ang - a).DegreesToRadians());

							hitbox.LocalLines[ptsA[i]] = new(matrix.Translation, line.B);
						}
						for(int i = 0; i < ptsB.Count; i++)
						{
							var line = hitbox.LocalLines[ptsB[i]];
							var matrix =
								Matrix3x2.CreateTranslation(line.B) *
								Matrix3x2.CreateRotation((ang - a).DegreesToRadians());

							hitbox.LocalLines[ptsB[i]] = new(line.A, matrix.Translation);
						}
					}
				}
				else
					for(int i = 0; i < selectedUIDs.Count; i++)
					{
						var uid = selectedUIDs[i];
						var pos = (Vector2)Thing.Get(uid, Thing.Property.THING_POSITION);
						var ang = (float)Thing.Get(uid, Thing.Property.THING_ANGLE);


						if(editIndex == 0)
							Thing.Set(uid, Thing.Property.THING_POSITION, Drag(pos, false, true));
						else if(editIndex == 1)
							Thing.Set(uid, Thing.Property.THING_ANGLE, DragAngle(pos, ang));
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

			if(e.Button == MouseButtons.Right)
				windowPicture.ContextMenuStrip = editHitbox != null && editHitbox.Checked ? sceneRightClickMenuHitbox : sceneRightClickMenu;

			if(e.Button != MouseButtons.Left)
				return;

			isDragSelecting = true;
			var pos = Mouse.GetPosition(window);
			selectStartPos = new(pos.X, pos.Y);
		}
		private void OnMouseUpScene(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
				TryResetThingPanel();

			UpdateThingPanel();

			if(e.Button == MouseButtons.Right)
			{
				rightClickPos = GetMousePosition();

				if(paintTile != null && paintTile.Checked)
					windowPicture.ContextMenuStrip.Hide();
			}

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isDragSelecting = false;
		}
		#endregion
		#region SceneRightClick
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			var uid = Thing.CreateSprite("Sprite", null);
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
		}
		private void OnSceneRightClickMenuCreateLight(object sender, EventArgs e)
		{
			var uid = Thing.CreateLight("Light", Color.White);
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
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

			var uid = Thing.CreateCamera("Camera", res);
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);

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
		private void OnSceneRightclickMenuCreateText(object sender, EventArgs e)
		{
			var result = DialogResult.None;
			if(string.IsNullOrWhiteSpace(GetAssetsPath()))
				MessageBox.Show(this, "This Text will be invisible without a Font.\n\n" + NO_ASSETS_MSG, "Create Text");
			else if(MessageBox.Show(this, "Pick a Font?", "Create Text", MessageBoxButtons.YesNo) == DialogResult.Yes)
				result = pickAsset.ShowDialog();

			var uid = Thing.CreateText("Text", result != DialogResult.OK ? null : GetMirrorAssetPath(pickAsset.FileName));
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
		}
		private void OnSceneRightclickMenuCreateNinePatch(object sender, EventArgs e)
		{
			var uid = Thing.CreateNinePatch("NinePatch", null);
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
		}
		private void OnSceneRightclickMenuCreateTilemap(object sender, EventArgs e)
		{
			var uid = Thing.CreateTilemap("Tilemap", null);
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
		}
		private void OnSceneRightclickMenuCreateAudio(object sender, EventArgs e)
		{
			var result = DialogResult.None;
			if(string.IsNullOrWhiteSpace(GetAssetsPath()))
				MessageBox.Show(this, "This Audio will be silent without an Audio file.\n\n" + NO_ASSETS_MSG, "Create Audio");
			else if(MessageBox.Show(this, "Pick an Audio file?", "Create Audio", MessageBoxButtons.YesNo) == DialogResult.Yes)
				result = pickAsset.ShowDialog();

			var uid = Thing.CreateAudio("Audio", result != DialogResult.OK ? null : GetMirrorAssetPath(pickAsset.FileName));
			Thing.Set(uid, Thing.Property.THING_POSITION, rightClickPos);
		}

		private void OnSceneRightclickMenuCreateHitboxLine(object sender, EventArgs e)
		{
			var off = new Vector2(50f * sceneSc, 0);
			AddHitboxLine(selectedUIDs[0], new List<Line>() { new(rightClickPos - off, rightClickPos + off) });
		}
		private void OnSceneRightclickMenuCreateHitboxSquare(object sender, EventArgs e)
		{
			var off = new Vector2(50f) * sceneSc;
			var uid = selectedUIDs[0];
			var p = rightClickPos;
			var lines = new List<Line>()
			{
				new(p + new Vector2(-off.X, -off.Y), p + new Vector2(off.X, -off.Y)),
				new(p + new Vector2(off.X, -off.Y), p + new Vector2(off.X, off.Y)),
				new(p + new Vector2(off.X, off.Y), p + new Vector2(-off.X, off.Y)),
				new(p + new Vector2(-off.X, off.Y), p + new Vector2(-off.X, -off.Y)),
			};
			AddHitboxLine(uid, lines);
		}
		private void OnSceneRightclickMenuCreateHitboxCircle(object sender, EventArgs e)
		{
			var uid = selectedUIDs[0];
			var radius = 50f * sceneSc;
			var angStep = 360f / 8f;
			var lines = new List<Line>();

			for(int i = 0; i < 8; i++)
			{
				var p = rightClickPos.PointMoveAtAngle(angStep * i, radius, false);
				var p1 = rightClickPos.PointMoveAtAngle(angStep * (i - 1), radius, false);
				lines.Add(new(p, p1));
			}
			AddHitboxLine(uid, lines);
		}

		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
		}
		private void OnSceneRightClickMenuDeselect(object sender, EventArgs e)
		{
			if(editHitbox != null && editHitbox.Checked)
			{
				selectedHitboxPointIndexesA.Clear();
				selectedHitboxPointIndexesB.Clear();
				return;
			}

			selectedUIDs.Clear();
			TryResetThingPanel();
		}

		#endregion

		#region EditThingPanel
		private void OnListSelectItem(object? sender, EventArgs e)
		{
			if(sender == null)
				return;

			var list = (ComboBox)sender;
			var selectedItem = list.SelectedItem == null ? "" : list.SelectedItem.ToString();
			listBoxPlaceholderTexts.TryGetValue(list.Name, out var placeholder);

			if(selectedItem == null || selectedItem == placeholder)
				return;

			if(list.Name.Contains("UIDs"))
			{
				if(Thing.Exists(selectedItem) == false)
				{
					TryAddPlaceholderToList(list);
					list.SelectedItem = placeholder;
					return;
				}

				FocusThing(selectedItem);
				selectedUIDs.Clear();
				TrySelectThing(selectedItem);

				TryResetThingPanel();
				UpdateThingPanel();
			}
			else if(list.Name == $"Prop{Thing.Property.THING_TYPES}")
			{
				var table = editThingTableTypes[$"table{list.SelectedItem}"];

				TryAddPlaceholderToList(list);
				list.SelectedItem = placeholder;

				rightTable.Controls.Clear();
				rightTable.Controls.Add(thingTypesTable);
				rightTable.Controls.Add(table);

				UpdateThingPanel();
			}
			else if(list.Name == $"Prop{Thing.Property.VISUAL_BLEND_MODE}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.VISUAL_BLEND_MODE, (BlendMode)index);
			}
			else if(list.Name == $"Prop{Thing.Property.VISUAL_EFFECT}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.VISUAL_EFFECT, (Thing.Effect)index);
			}
			else if(list.Name == $"Prop{Thing.Property.TEXT_STYLE}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.TEXT_STYLE, (Text.Styles)index);
			}
			else if(list.Name == $"Prop{Thing.Property.AUDIO_STATUS}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.AUDIO_STATUS, (Thing.AudioStatus)index);
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

			if((list.SelectedIndex != -1 && hasPlaceholder))
				return;

			if(hasPlaceholder)
			{
				var placeholder = listBoxPlaceholderTexts[list.Name];
				TryAddPlaceholderToList(list);
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

			var prop = Thing.Info.GetProperty(uid, propName);
			if(prop.HasSetter)
				Thing.Set(uid, propName, textBox.Text);

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

			if(propName == Thing.Property.THING_HITBOX)
			{

			}
			else
				Thing.Set(uid, propName, checkBox.Checked);
			UpdateThingPanel();
		}
		private void OnNumericChange(object? sender, EventArgs e)
		{
			if(sender == null || ((Control)sender).Focused == false) // ignore if it isn't the user changing the value
				return;

			var numeric = (NumericUpDown)sender;
			var propName = numeric.Name["Prop".Length..];
			var vecColIndex = 0;
			var parName = numeric.Parent.Name;
			if(parName.StartsWith("Prop") || parName.StartsWith("Scene")) // is vector or color
			{
				vecColIndex = (int)propName[^1].ToString().ToNumber();
				propName = propName[..^1];
			}

			var valueFloat = (float)numeric.Value;
			var valueInt = (int)numeric.Value;

			if(parName.StartsWith("PropScene"))
			{
				var cols = typeColors;
				switch(parName)
				{
					case "PropSceneBackgroundColor": bgCol = SetColor(bgCol); break;
					case "PropSceneBoundingBoxColor": bbCol = SetColor(bbCol); break;
					case "PropSceneGridColor": gridCol = SetColor(gridCol); break;
					case "PropSceneGrid0Color": gridCol0 = SetColor(gridCol0); break;
					case "PropSceneGrid1000Color": gridCol1000 = SetColor(gridCol1000); break;
					case "PropSceneSelectColor": selCol = SetColor(selCol); break;
					case "PropSceneCameraColor": cols["Camera"] = SetColor(cols["Camera"]); break;
					case "PropSceneHitboxColor": hitCol = SetColor(hitCol); break;
					case "PropSceneLightColor": cols["Light"] = SetColor(cols["Light"]); break;
					case "PropSceneAudioColor": cols["Audio"] = SetColor(cols["Audio"]); break;
					case "PropSceneTextColor": cols["Text"] = SetColor(cols["Text"]); break;
					case "PropSceneSpriteColor": cols["Sprite"] = SetColor(cols["Sprite"]); break;
					case "PropSceneNinePatchColor": cols["NinePatch"] = SetColor(cols["NinePatch"]); break;
					case "PropSceneTilemapColor": cols["Tilemap"] = SetColor(cols["Tilemap"]); break;
				}

				TryResetThingPanel();
				return;

				Color SetColor(Color col)
				{
					if(vecColIndex == 0)
						col.R = (byte)valueInt;
					else if(vecColIndex == 1)
						col.G = (byte)valueInt;
					else if(vecColIndex == 2)
						col.B = (byte)valueInt;
					else if(vecColIndex == 3)
						col.A = (byte)valueInt;
					return col;
				}
			}

			if(parName.StartsWith("PropTilePalette"))
			{
				if(tilePaletteCol != null)
				{
					var r = (NumericUpDown)tilePaletteCol.Controls[0];
					var g = (NumericUpDown)tilePaletteCol.Controls[1];
					var b = (NumericUpDown)tilePaletteCol.Controls[2];
					var a = (NumericUpDown)tilePaletteCol.Controls[3];

					r.Value = ((int)r.Value).Limit(0, 256, Extensions.Limitation.Overflow);
					g.Value = ((int)g.Value).Limit(0, 256, Extensions.Limitation.Overflow);
					b.Value = ((int)b.Value).Limit(0, 256, Extensions.Limitation.Overflow);
					a.Value = ((int)a.Value).Limit(0, 256, Extensions.Limitation.Overflow);

					tilePaletteCol.BackColor = System.Drawing.Color.FromArgb(255, (int)r.Value, (int)g.Value, (int)b.Value);
				}
				return;
			}

			var uid = selectedUIDs[0];
			var propType = Thing.Info.GetProperty(uid, propName).Type;

			if(propType == "Vector2")
			{
				var vec = (Vector2)Thing.Get(uid, propName);
				if(vecColIndex == 0) vec.X = valueFloat;
				else if(vecColIndex == 1) vec.Y = valueFloat;
				Thing.Set(uid, propName, vec);
			}
			else if(propType == "Color")
			{
				var col = (Color)Thing.Get(uid, propName);
				var val = (byte)valueInt;
				if(vecColIndex == 0) col.R = val;
				else if(vecColIndex == 1) col.G = val;
				else if(vecColIndex == 2) col.B = val;
				else if(vecColIndex == 3) col.A = val;
				Thing.Set(uid, propName, col);
			}
			else if(propType == "Int32")
				Thing.Set(uid, propName, valueInt);
			else if(propType == "Single")
				Thing.Set(uid, propName, valueFloat);

			UpdateThingPanel();
		}
		private void OnThingListPick(object sender, ToolStripItemClickedEventArgs e)
		{
			if(pickThingProperty != "")
			{
				Thing.Set(selectedUIDs[0], pickThingProperty, e.ClickedItem.Text);
				UpdateThingPanel();
				return;
			}

			if(waitingPickThingControl != null)
				waitingPickThingControl.Text = e.ClickedItem.Text;
		}
		#endregion
		#region Assets
		private void OnAssetRename(object sender, RenamedEventArgs e)
		{
			MainScene.UnloadAsset(GetMirrorAssetPath(e.OldFullPath));
			DeleteMirrorFiles(e.OldFullPath);
			CopyMirrorFiles(e.FullPath);
			MainScene.LoadAsset(GetMirrorAssetPath(e.FullPath));
		}
		private void OnAssetCreate(object sender, FileSystemEventArgs e)
		{
			Loading(LOADING_ASSETS);
			CopyMirrorFiles(e.FullPath);
			MainScene.LoadAsset(GetMirrorAssetPath(e.FullPath));
			Loading();
		}
		private void OnAssetDelete(object sender, FileSystemEventArgs e)
		{
			Loading(LOADING_ASSETS);
			MainScene.UnloadAsset(GetMirrorAssetPath(e.FullPath));
			DeleteMirrorFiles(e.FullPath);
			Loading();
		}

		private void OnAssetFolderRename(object sender, RenamedEventArgs e)
		{
			//if(e.FullPath != assetsPath)
			//	Directory.Move(e.FullPath, assetsPath);
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
				TrySave();

			selectedUIDs.Clear();
			Loading("Deleting cached assets...");
			Thing.DestroyAll();
			MainScene.UnloadAssets();
			DeleteCache();
			Loading("");
		}
		#endregion

		#region Utility
		const int SPACING_X = 50, SPACING_Y = 20, BUTTON_W = 70, BUTTON_H = 25, TEXTBOX_H = 25,
			W = 400, H = SPACING_Y * 5 + BUTTON_H;

		private Vector2 Drag(Vector2 point, bool reverse = false, bool snapToGrid = false, float scale = 1f)
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

			return dist == 0 ? point : point.PointMoveAtAngle(view.Rotation + ang, dist / scale, false);
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
			var sz = new Vector2i(W, H + text.Split('\n').Length * 15);
			var window = new Form()
			{
				Width = sz.X,
				Height = sz.Y,
				FormBorderStyle = FormBorderStyle.FixedToolWindow,
				Text = title,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White,
				StartPosition = FormStartPosition.CenterScreen
			};
			var textLabel = new Label()
			{
				Left = SPACING_X,
				Top = SPACING_Y,
				Text = text,
				Width = sz.X - SPACING_X * 2 - SPACING_X / 4,
				Height = sz.Y - BUTTON_H - SPACING_Y * 4
			};
			var textBox = new TextBox()
			{
				Left = SPACING_X,
				Top = SPACING_Y + textLabel.Height,
				Width = sz.X - BUTTON_W - SPACING_X * 3,
				Height = TEXTBOX_H,
				Text = defaultInput,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White
			};
			var button = new Button()
			{
				Text = "OK",
				Left = sz.X - BUTTON_W - SPACING_X - SPACING_X / 4,
				Width = BUTTON_W,
				Height = BUTTON_H,
				Top = SPACING_Y + textLabel.Height,
				DialogResult = DialogResult.OK
			};
			button.Click += (sender, e) => { window.Close(); };
			window.Controls.Add(textBox);
			window.Controls.Add(button);
			window.Controls.Add(textLabel);
			window.AcceptButton = button;

			return window.ShowDialog() == DialogResult.OK ? textBox.Text : "";
		}
		private void EditList(string title, List<string> list, bool thingList, bool unique)
		{
			var sz = new Vector2i(W + W / (thingList ? 2 : 4), H + 100);
			var window = new Form()
			{
				Width = sz.X,
				Height = sz.Y,
				FormBorderStyle = FormBorderStyle.FixedToolWindow,
				Text = title,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White,
				StartPosition = FormStartPosition.CenterScreen
			};
			var listBox = new ListBox()
			{
				Left = SPACING_X,
				Top = SPACING_Y,
				Width = sz.X - SPACING_X * 2 - SPACING_X / 4,
				Height = sz.Y - BUTTON_H - SPACING_Y * 4,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White
			};
			var textBox = new TextBox()
			{
				Left = SPACING_X + (thingList ? BUTTON_W : 0),
				Top = SPACING_Y + listBox.Height + 5,
				Width = sz.X - BUTTON_W * (thingList ? 4 : 3) - SPACING_X * 3,
				Height = TEXTBOX_H,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White
			};
			var buttonOk = new Button()
			{
				Top = SPACING_Y + listBox.Height + 5,
				Left = sz.X - BUTTON_W - SPACING_X - SPACING_X / 4,
				Width = BUTTON_W,
				Height = BUTTON_H,
				Text = "OK",
				DialogResult = DialogResult.OK
			};
			var buttonAdd = new Button()
			{
				Top = SPACING_Y + listBox.Height + 5,
				Left = sz.X - BUTTON_W * 3 - SPACING_X * 2,
				Width = BUTTON_W,
				Height = BUTTON_H,
				Text = "Add",
			};
			var buttonRemove = new Button()
			{
				Top = SPACING_Y + listBox.Height + 5,
				Left = sz.X - BUTTON_W * 2 - SPACING_X * 2,
				Width = BUTTON_W,
				Height = BUTTON_H,
				Text = "Remove",
			};
			for(int i = 0; i < list.Count; i++)
				listBox.Items.Add(list[i]);

			buttonOk.Click += (sender, e) => { window.Close(); };
			buttonAdd.Click += (sender, e) =>
			{
				if(string.IsNullOrWhiteSpace(textBox.Text) != false || (unique && listBox.Items.Contains(textBox.Text)))
					return;

				listBox.Items.Add(textBox.Text);
				listBox.SelectedItem = textBox.Text;
				textBox.Text = "";
			};
			buttonRemove.Click += (sender, e) => { listBox.Items.Remove(listBox.SelectedItem); };
			listBox.SelectedIndex = listBox.Items.Count == 0 ? -1 : 0;
			window.Controls.Add(textBox);
			window.Controls.Add(buttonOk);
			window.Controls.Add(buttonAdd);
			window.Controls.Add(buttonRemove);
			window.Controls.Add(listBox);
			window.AcceptButton = buttonOk;

			if(thingList)
			{
				var buttonThings = new Button()
				{
					Top = SPACING_Y + listBox.Height + 5,
					Left = SPACING_X,
					Width = BUTTON_W,
					Height = BUTTON_H,
					Text = "Things",
				};
				buttonThings.Click += (sender, e) =>
				{
					waitingPickThingControl = textBox;
					PickThing(buttonThings);
				};
				window.Controls.Add(buttonThings);
			}

			if(window.ShowDialog() != DialogResult.OK)
				return;

			list.Clear();
			foreach(string item in listBox.Items)
				list.Add(item);
		}
		private void PickThing(Control control, string property = "")
		{
			var uids = Thing.GetUIDs();

			thingsList.MaximumSize = new(thingsList.MaximumSize.Width, 300);
			thingsList.Items.Clear();
			for(int i = 0; i < uids.Count; i++)
				thingsList.Items.Add(uids[i]);

			pickThingProperty = property;
			// show twice cuz first time has wrong position
			Show();
			Show();

			void Show() => thingsList.Show(this, control.PointToScreen(new(-thingsList.Width, -control.Height / 2)));
		}
		private void Loading(string description = "")
		{
			loadingDescr = description;
			Application.DoEvents();
		}
		#endregion
	}
}