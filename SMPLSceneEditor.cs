global using System.Collections.ObjectModel;
global using System.Numerics;

global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;

global using SMPL;
global using SMPL.Tools;

global using static SFML.Window.Keyboard;

global using Color = SFML.Graphics.Color;
global using BlendMode = SMPL.Thing.BlendMode;
global using Extensions = SMPL.Tools.Extensions;
global using Cursor = System.Windows.Forms.Cursor;

using Application = System.Windows.Forms.Application;
using Text = SFML.Graphics.Text;

namespace SMPLSceneEditor
{
	public partial class FormWindow : Form
	{
		#region Fields
		const string FONT = "Segoe UI";
		const float FONT_SIZE = 12f;
		private const string NO_ASSETS_MSG = "In order to use Assets, save this Scene or load another.\n\n" +
			"Upon doing that, a Scene file, alongside an Asset folder will be present in the provided game directory. " +
			"Fill that folder with Assets and point the Things in the Scene toward them.";
		private const string LOADING_ASSETS = "Processing assets...", LOADING_SCENE = "Loading Scene...";
		private const float HITBOX_POINT_EDIT_MAX_DIST = 20;

		private TextBox? spriteStackCreateTexPath, spriteStackCreateObjPath, cubeSideTexPath;
		private readonly Label tilePaletteHoveredIndexesLabel;
		private CheckBox? editHitbox, paintTile;
		private Control? waitingPickThingControl, tilePaletteCol, pickThingResult;
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1f;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene, isSpriteStackCreationOpen, isTyping;
		private Vector2 prevFormsMousePos, prevMousePos, prevFormsMousePosGrid, selectStartPos, createPosition, tilePaintRightClickPos = new Vector2().NaN();
		private readonly List<string> selectedUIDs = new();
		private readonly List<int> selectedHitboxPointIndexesA = new(), selectedHitboxPointIndexesB = new();
		private readonly Cursor[] editCursors = new Cursor[] { Cursors.NoMove2D, Cursors.Cross, Cursors.SizeAll };
		private FileSystemWatcher? assetsWatcher, assetsFolderWatcher;
		private string pickThingProperty = "", scenePath = "", loadingDescr = "", tilePaletteUID = "", tilePaletteHoveredIndexes = "", lastCreatedTexStackName = "";
		private readonly Dictionary<string, TableLayoutPanel> editThingTableTypes = new();
		private readonly Dictionary<string, string> listBoxPlaceholderTexts = new()
		{
			{ $"Prop{Thing.Property.CHILDREN_UIDS}", "(select to focus)                                     " },
			{ $"Prop{Thing.Property.TYPES}", "(select to edit)                                     " },
			{ $"Prop{Thing.Property.UI_LIST_MULTISELECT_SELECTION_UIDS}", "(select to focus)                                     " },
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
			{ "SpriteStack", Color.White },
			{ "Cloth", Color.White },
			{ "Cube", Color.White },
			{ "Button", new(50, 150, 180) },
			{ "Textbox", new(50, 150, 180) },
		};
		private readonly Dictionary<Key[], Action> hotkeys = new();

		private Color bgCol = Color.Black, bbCol = Color.Cyan, selCol = new(0, 180, 255, 100), hitCol = new(0, 255, 0),
			gridCol = new(50, 50, 50), gridCol0 = Color.Yellow, gridCol1000 = Color.White;
		#endregion

		#region Init
		public FormWindow()
		{
			DeleteCache();
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

			InitHotkeys();
			InitTables();
			TryResetThingPanel();
			SetView();

			Scene.CurrentScene = new("editor");

			var desktopRes = Screen.PrimaryScreen.Bounds.Size;
			Thing.CreateCamera(Scene.MAIN_CAMERA_UID, new(desktopRes.Width, desktopRes.Height));

			tilePaletteHoveredIndexesLabel = new();
		}

		private void InitHotkeys()
		{
			hotkeys.Add(new Key[] { Key.H, Key.L }, AddLineToHitbox);
			hotkeys.Add(new Key[] { Key.H, Key.S }, AddSquareToHitbox);
			hotkeys.Add(new Key[] { Key.H, Key.C }, AddCircleToHitbox);

			hotkeys.Add(new Key[] { Key.LControl, Key.S }, TrySaveScene);
			hotkeys.Add(new Key[] { Key.LControl, Key.L }, TryLoadScene);

			hotkeys.Add(new Key[] { Key.LControl, Key.LAlt, Key.P }, TryUnloadTextureStack);
			hotkeys.Add(new Key[] { Key.LControl, Key.P }, TryCreateTextureStack);

			hotkeys.Add(new Key[] { Key.LControl, Key.C }, TryDuplicateSelection);
			hotkeys.Add(new Key[] { Key.LControl, Key.D }, DeselectAll);
			hotkeys.Add(new Key[] { Key.Delete }, TryDestroySelection);
			hotkeys.Add(new Key[] { Key.Space }, ResetView);

			hotkeys.Add(new Key[] { Key.C, Key.S }, CreateSprite);
			hotkeys.Add(new Key[] { Key.C, Key.T }, CreateText);
			hotkeys.Add(new Key[] { Key.C, Key.N }, CreateNinePatch);
			hotkeys.Add(new Key[] { Key.C, Key.I }, CreateLight);
			hotkeys.Add(new Key[] { Key.C, Key.R }, CreateCamera);
			hotkeys.Add(new Key[] { Key.C, Key.M }, CreateTilemap);
			hotkeys.Add(new Key[] { Key.C, Key.A }, CreateAudio);
			hotkeys.Add(new Key[] { Key.C, Key.H }, CreateCloth);
			hotkeys.Add(new Key[] { Key.C, Key.P }, CreateSpriteStack);
			hotkeys.Add(new Key[] { Key.C, Key.Q }, CreateCube);

			hotkeys.Add(new Key[] { Key.U, Key.B }, CreateUIButton);

		}
		private void InitTables()
		{
			CreateTable("tableScene", AddPropsScene);
			CreateTable("tableThing", AddPropsThing);
			CreateTable("tableSprite", AddPropsSprite);
			CreateTable("tableVisual", AddPropsVisual);
			CreateTable("tableLight", AddPropsLight);
			CreateTable("tableCamera", AddPropsCamera);
			CreateTable("tableText", AddPropsText);
			CreateTable("tableNinePatch", AddPropsNinePatch);
			CreateTable("tableAudio", AddPropsAudio);
			CreateTable("tableTilemap", AddPropsTilemap);
			CreateTable("tableCloth", AddPropsCloth);
			CreateTable("tablePseudo3D", AddPropsPseudo3D);
			CreateTable("tableCube", AddPropsCube);
			CreateTable("tableButton", AddPropsButton);
			CreateTable("tableTextbox", AddPropsTextbox);
			CreateTable("tableTextButton", AddPropsTextButton);
			CreateTable("tableInputbox", AddPropsInputbox);
			CreateTable("tableCheckbox", AddPropsCheckbox);
			CreateTable("tableProgressBar", AddPropsProgressBar);
			CreateTable("tableScrollBar", AddPropsScrollBar);
			CreateTable("tableSlider", AddPropsSlider);
			CreateTable("tableList", AddPropsList);
			CreateTable("tableListDropdown", AddPropsListDropdown);
			CreateTable("tableListCarousel", AddPropsListCarousel);
			CreateTable("tableListMultiselect", AddPropsListMultiselect);

			var types = thingTypesTable;
			types.Hide();
			AddThingProperty(types, "Types", Thing.Property.TYPES, typeof(ReadOnlyCollection<string>));

			TableLayoutPanel CreateTable(string name, Action<TableLayoutPanel> addPropertiesMethod)
			{
				var table = CreateDefaultTable(name);
				addPropertiesMethod?.Invoke(table);
				editThingTableTypes[table.Name] = table;
				return table;
			}
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
			void AddPropsScene(TableLayoutPanel table)
			{
				AddThingProperty(table, "SMPL Scene", rightLabel: true); AddThingProperty(table, "Editor Colors");
				AddThingProperty(table, "Background", "SceneBackgroundColor", typeof(Color));
				AddThingProperty(table, "Grid", "SceneGridColor", typeof(Color));
				AddThingProperty(table, "Grid 0", "SceneGrid0Color", typeof(Color));
				AddThingProperty(table, "Grid 1000", "SceneGrid1000Color", typeof(Color));
				AddSpace(table);
				AddThingProperty(table, "Selection Box", "SceneBoundingBoxColor", typeof(Color), labelSizeOffset: 2);
				AddThingProperty(table, "Selection Fill", "SceneSelectColor", typeof(Color));
				AddSpace(table);
				AddThingProperty(table, "Sprite", "SceneSpriteColor", typeof(Color));
				AddThingProperty(table, "Text", "SceneTextColor", typeof(Color));
				AddThingProperty(table, "NinePatch", "SceneNinePatchColor", typeof(Color));
				AddThingProperty(table, "Tilemap", "SceneTilemapColor", typeof(Color));
				AddThingProperty(table, "Camera", "SceneCameraColor", typeof(Color));
				AddThingProperty(table, "Hitbox", "SceneHitboxColor", typeof(Color));
				AddThingProperty(table, "Light", "SceneLightColor", typeof(Color));
				AddThingProperty(table, "Audio", "SceneAudioColor", typeof(Color));
				AddThingProperty(table, "Cloth", "SceneClothColor", typeof(Color));
				AddThingProperty(table, "SpriteStack", "SceneSpriteStackColor", typeof(Color));
				AddThingProperty(table, "Cube", "SceneCubeColor", typeof(Color));
			}
			void AddPropsThing(TableLayoutPanel table)
			{
				AddThingProperty(table, "UID", Thing.Property.UID, typeof(string));
				AddThingProperty(table, "Old UID", Thing.Property.OLD_UID, typeof(string), readOnly: true);
				AddThingProperty(table, "Numeric UID", Thing.Property.NUMERIC_UID, typeof(int), readOnly: true);
				AddThingProperty(table, "Tags", Thing.Property.TAGS, typeof(List<string>));
				AddThingProperty(table, "Age (Seconds)", Thing.Property.AGE, typeof(float), readOnly: true);
				AddSpace(table);
				AddThingProperty(table, "Position", Thing.Property.POSITION, typeof(Vector2));
				AddThingProperty(table, "Angle", Thing.Property.ANGLE, typeof(float));
				AddThingProperty(table, "Direction", Thing.Property.DIRECTION, typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(table, "Scale", Thing.Property.SCALE, typeof(float), smallNumericStep: true);
				AddSpace(table);
				AddThingProperty(table, "Parent UID", Thing.Property.PARENT_UID, typeof(string));
				AddThingProperty(table, "Parent Old UID", Thing.Property.PARENT_OLD_UID, typeof(string), readOnly: true);
				AddThingProperty(table, "Children UIDs", Thing.Property.CHILDREN_UIDS, typeof(ReadOnlyCollection<string>), thingList: true);
				AddSpace(table);
				AddThingProperty(table, "Local Position", Thing.Property.LOCAL_POSITION, typeof(Vector2));
				AddThingProperty(table, "Local Angle", Thing.Property.LOCAL_ANGLE, typeof(float));
				AddThingProperty(table, "Local Direction", Thing.Property.LOCAL_DIRECTION, typeof(Vector2), readOnly: true, smallNumericStep: true);
				AddThingProperty(table, "Local Scale", Thing.Property.LOCAL_SCALE, typeof(float), smallNumericStep: true);
				AddSpace(table);
				AddThingProperty(table, "Hitbox", Thing.Property.HITBOX, typeof(string), readOnly: true);
			}
			void AddPropsSprite(TableLayoutPanel table)
			{
				AddThingProperty(table, "Local Size", Thing.Property.SPRITE_LOCAL_SIZE, typeof(Vector2));
				AddThingProperty(table, "Size", Thing.Property.SPRITE_SIZE, typeof(Vector2));
				AddSpace(table);
				AddThingProperty(table, "Origin Unit", Thing.Property.SPRITE_ORIGIN_UNIT, typeof(Vector2), smallNumericStep: true);
				AddThingProperty(table, "Origin", Thing.Property.SPRITE_ORIGIN, typeof(Vector2));
				AddSpace(table);
				AddThingProperty(table, "Texture Coordinate Unit A", Thing.Property.SPRITE_TEX_COORD_UNIT_A, typeof(Vector2), labelSizeOffset: -1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate Unit B", Thing.Property.SPRITE_TEX_COORD_UNIT_B, typeof(Vector2), labelSizeOffset: -1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate A", Thing.Property.SPRITE_TEX_COORD_A, typeof(Vector2), labelSizeOffset: 1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate B", Thing.Property.SPRITE_TEX_COORD_B, typeof(Vector2), labelSizeOffset: 1, smallNumericStep: true);
			}
			void AddPropsVisual(TableLayoutPanel table)
			{
				AddThingProperty(table, "Texture Path", Thing.Property.VISUAL_TEXTURE_PATH, typeof(string));
				AddThingProperty(table, "Tint", Thing.Property.VISUAL_TINT, typeof(Color));
				AddThingProperty(table, "Order", Thing.Property.VISUAL_ORDER, typeof(int));
				AddSpace(table);
				AddThingProperty(table, "Is Hidden", Thing.Property.VISUAL_IS_HIDDEN, typeof(bool));
				AddThingProperty(table, "Is Repeated", Thing.Property.VISUAL_IS_REPEATED, typeof(bool));
				AddThingProperty(table, "Is Smooth", Thing.Property.VISUAL_IS_SMOOTH, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Effect", Thing.Property.VISUAL_EFFECT, typeof(Thing.Effect));
				AddThingProperty(table, "Blend Mode", Thing.Property.VISUAL_BLEND_MODE, typeof(BlendMode));
				AddSpace(table);
				AddThingProperty(table, "Camera Tag", Thing.Property.VISUAL_CAMERA_TAG, typeof(string), thingList: true);
			}
			void AddPropsCamera(TableLayoutPanel table)
			{
				AddThingProperty(table, "Resolution", Thing.Property.CAMERA_RESOLUTION, typeof(Vector2), readOnly: true);
				AddSpace(table);
				AddThingProperty(table, "Is Smooth", Thing.Property.CAMERA_IS_SMOOTH, typeof(bool));
			}
			void AddPropsText(TableLayoutPanel table)
			{
				AddThingProperty(table, "Font Path", Thing.Property.TEXT_FONT_PATH, typeof(string));
				AddThingProperty(table, "Value", Thing.Property.TEXT_VALUE, typeof(string));
				AddThingProperty(table, "Color", Thing.Property.TEXT_COLOR, typeof(Color));
				AddThingProperty(table, "Style", Thing.Property.TEXT_STYLE, typeof(Text.Styles), readOnly: true);
				AddSpace(table);
				AddThingProperty(table, "Origin Unit", Thing.Property.TEXT_ORIGIN_UNIT, typeof(Vector2));
				AddThingProperty(table, "Symbol Size", Thing.Property.TEXT_SYMBOL_SIZE, typeof(int));
				AddThingProperty(table, "Symbol Space", Thing.Property.TEXT_SYMBOL_SPACE, typeof(float));
				AddThingProperty(table, "Line Space", Thing.Property.TEXT_LINE_SPACE, typeof(float));
				AddSpace(table);
				AddThingProperty(table, "Outline Color", Thing.Property.TEXT_OUTLINE_COLOR, typeof(Color));
				AddThingProperty(table, "Outline Size", Thing.Property.TEXT_OUTLINE_SIZE, typeof(float));
			}
			void AddPropsAudio(TableLayoutPanel table)
			{
				AddThingProperty(table, "Path", Thing.Property.AUDIO_PATH, typeof(string));
				AddSpace(table);
				AddThingProperty(table, "Status", Thing.Property.AUDIO_STATUS, typeof(Thing.AudioStatus));
				AddThingProperty(table, "Duration", Thing.Property.AUDIO_DURATION, typeof(float), readOnly: true);
				AddThingProperty(table, "Progress (Seconds)", Thing.Property.AUDIO_PROGRESS, typeof(float));
				AddThingProperty(table, "Progress Unit", Thing.Property.AUDIO_PROGRESS_UNIT, typeof(float), smallNumericStep: true);
				AddSpace(table);
				AddThingProperty(table, "Volume Unit", Thing.Property.AUDIO_VOLUME_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(table, "Is Looping", Thing.Property.AUDIO_IS_LOOPING, typeof(bool));
				AddThingProperty(table, "Is Global", Thing.Property.AUDIO_IS_GLOBAL, typeof(bool));
				AddThingProperty(table, "Pitch Unit", Thing.Property.AUDIO_PITCH_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(table, "Distance Fade", Thing.Property.AUDIO_FADE, typeof(float));
			}
			void AddPropsTilemap(TableLayoutPanel table)
			{
				AddThingProperty(table, "Tile Size", Thing.Property.TILEMAP_TILE_SIZE, typeof(Vector2));
				AddThingProperty(table, "Tile Gap", Thing.Property.TILEMAP_TILE_GAP, typeof(Vector2));
				AddSpace(table);
				AddThingProperty(table, "Tile Brush", Thing.Property.TILEMAP_TILE_PALETTE, typeof(Dictionary<string, Thing.Tile>));
				AddThingProperty(table, "Tiles", Thing.Property.TILEMAP_TILE_COUNT, typeof(int), readOnly: true);
			}
			void AddPropsCloth(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Simulating", Thing.Property.CLOTH_IS_SIMULATING, typeof(bool));
				AddThingProperty(table, "Has Threads", Thing.Property.CLOTH_HAS_THREADS, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Break Threshold", Thing.Property.CLOTH_BREAK_THRESHOLD, typeof(float));
				AddThingProperty(table, "Gravity", Thing.Property.CLOTH_GRAVITY, typeof(Vector2));
				AddThingProperty(table, "Force", Thing.Property.CLOTH_FORCE, typeof(Vector2));
				AddSpace(table);
				AddThingProperty(table, "Texture Coordinate Unit A", Thing.Property.CLOTH_TEX_COORD_UNIT_A, typeof(Vector2), labelSizeOffset: -1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate Unit B", Thing.Property.CLOTH_TEX_COORD_UNIT_B, typeof(Vector2), labelSizeOffset: -1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate A", Thing.Property.CLOTH_TEX_COORD_A, typeof(Vector2), labelSizeOffset: 1, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate B", Thing.Property.CLOTH_TEX_COORD_B, typeof(Vector2), labelSizeOffset: 1, smallNumericStep: true);
				AddSpace(table);
				AddButton(table, "Add Pin", Pin);
				AddButton(table, "Remove Pin", Unpin);

				void Pin(object? sender, EventArgs e)
				{
					var index = GetVector2("Add Pin", "Provide the indexes to be pinned.", 0, int.MaxValue, 0, int.MaxValue);
					Thing.CallVoid(selectedUIDs[0], Thing.Method.CLOTH_PIN, index, true);
				}
				void Unpin(object? sender, EventArgs e)
				{
					var index = GetVector2("Remove Pin", "Provide the indexes to be unpinned.", 0, int.MaxValue, 0, int.MaxValue);
					Thing.CallVoid(selectedUIDs[0], Thing.Method.CLOTH_PIN, index, false);
				}
			}
			void AddPropsPseudo3D(TableLayoutPanel table)
			{
				AddThingProperty(table, "Tilt", Thing.Property.PSEUDO_3D_TILT, typeof(float));
				AddThingProperty(table, "Depth", Thing.Property.PSEUDO_3D_DEPTH, typeof(float));
				AddSpace(table);
				AddThingProperty(table, "Local Size", Thing.Property.PSEUDO_3D_LOCAL_SIZE, typeof(Vector2));
				AddThingProperty(table, "Size", Thing.Property.PSEUDO_3D_SIZE, typeof(Vector2));
				AddSpace(table);
				AddThingProperty(table, "Origin Unit", Thing.Property.PSEUDO_3D_ORIGIN_UNIT, typeof(Vector2), smallNumericStep: true);
				AddThingProperty(table, "Origin", Thing.Property.PSEUDO_3D_ORIGIN, typeof(Vector2));
			}
			void AddPropsCube(TableLayoutPanel table)
			{
				AddThingProperty(table, "Perspective Unit", Thing.Property.CUBE_PERSPECTIVE_UNIT, typeof(float), smallNumericStep: true);
				AddSpace(table);
				AddThingProperty(table, "Side Far", Thing.Property.CUBE_SIDE_FAR, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
				AddThingProperty(table, "Side Top", Thing.Property.CUBE_SIDE_TOP, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
				AddThingProperty(table, "Side Left", Thing.Property.CUBE_SIDE_LEFT, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
				AddThingProperty(table, "Side Right", Thing.Property.CUBE_SIDE_RIGHT, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
				AddThingProperty(table, "Side Bottom", Thing.Property.CUBE_SIDE_BOTTOM, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
				AddThingProperty(table, "Side Near", Thing.Property.CUBE_SIDE_NEAR, typeof(Thing.CubeSide), readOnly: true, labelSizeOffset: 2);
			}
			void AddPropsLight(TableLayoutPanel table)
			{
				AddThingProperty(table, "Color", Thing.Property.LIGHT_COLOR, typeof(Color));
			}
			void AddPropsNinePatch(TableLayoutPanel table)
			{
				AddThingProperty(table, "Border Size", Thing.Property.NINE_PATCH_BORDER_SIZE, typeof(float));
			}
			void AddPropsButton(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Disabled", Thing.Property.UI_BUTTON_IS_DISABLED, typeof(bool));
				AddThingProperty(table, "Is Draggable", Thing.Property.UI_BUTTON_IS_DRAGGABLE, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Hold Delay", Thing.Property.UI_BUTTON_HOLD_DELAY, typeof(float));
				AddThingProperty(table, "Hold Trigger Speed", Thing.Property.UI_BUTTON_HOLD_TRIGGER_SPEED, typeof(float), labelSizeOffset: 2);
			}
			void AddPropsTextbox(TableLayoutPanel table)
			{
				AddThingProperty(table, "Backgr. Color", Thing.Property.UI_TEXTBOX_BACKGROUND_COLOR, typeof(Color), labelSizeOffset: 2);
				AddThingProperty(table, "Camera UID", Thing.Property.UI_TEXTBOX_CAMERA_UID, typeof(string));
				AddThingProperty(table, "Alignment", Thing.Property.UI_TEXTBOX_ALIGNMENT, typeof(SMPL.UI.Thing.TextboxAlignment));
				AddSpace(table);
				AddThingProperty(table, "ShadowOffset", Thing.Property.UI_TEXTBOX_SHADOW_OFFSET, typeof(Vector2), smallNumericStep: true);
				AddThingProperty(table, "ShadowColor", Thing.Property.UI_TEXTBOX_SHADOW_COLOR, typeof(Color), labelSizeOffset: 2);
			}
			void AddPropsTextButton(TableLayoutPanel table)
			{
				AddThingProperty(table, "Text UID", Thing.Property.UI_TEXT_BUTTON_TEXT_UID, typeof(string));
				AddSpace(table);
				AddThingProperty(table, "Is Hyperlink", Thing.Property.UI_TEXT_BUTTON_IS_HYPERLINK, typeof(bool));
			}
			void AddPropsInputbox(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Disabled", Thing.Property.UI_INPUTBOX_IS_DISABLED, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Cursor Color", Thing.Property.UI_INPUTBOX_CURSOR_COLOR, typeof(Color));
				AddThingProperty(table, "Cursor Position Index", Thing.Property.UI_INPUTBOX_CURSOR_POSITION_INDEX, typeof(int), labelSizeOffset: 2);
				AddSpace(table);
				AddThingProperty(table, "Placeholder Value", Thing.Property.UI_INPUTBOX_PLACEHOLDER_VALUE, typeof(string));
				AddThingProperty(table, "Placeh. Color", Thing.Property.UI_INPUTBOX_PLACEHOLDER_COLOR, typeof(Color), labelSizeOffset: 2);
			}
			void AddPropsCheckbox(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Checked", Thing.Property.UI_CHECKBOX_IS_CHECKED, typeof(bool));
			}
			void AddPropsProgressBar(TableLayoutPanel table)
			{
				AddThingProperty(table, "Range A", Thing.Property.UI_PROGRESS_BAR_RANGE_A, typeof(float));
				AddThingProperty(table, "Range B", Thing.Property.UI_PROGRESS_BAR_RANGE_B, typeof(float));
				AddSpace(table);
				AddThingProperty(table, "Progress Unit", Thing.Property.UI_PROGRESS_BAR_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(table, "Value", Thing.Property.UI_PROGRESS_BAR_VALUE, typeof(float));
				AddSpace(table);
				AddThingProperty(table, "Max Length", Thing.Property.UI_PROGRESS_BAR_MAX_LENGTH, typeof(float));
			}
			void AddPropsScrollBar(TableLayoutPanel table)
			{
				AddThingProperty(table, "Button ^ UID", Thing.Property.UI_SCROLL_BAR_BUTTON_UP_UID, typeof(string), labelSizeOffset: 2);
				AddThingProperty(table, "Button V UID", Thing.Property.UI_SCROLL_BAR_BUTTON_DOWN_UID, typeof(string), labelSizeOffset: 2);
				AddSpace(table);
				AddThingProperty(table, "Step", Thing.Property.UI_SCROLL_BAR_STEP, typeof(float), smallNumericStep: true);
				AddThingProperty(table, "Step Unit", Thing.Property.UI_SCROLL_BAR_STEP_UNIT, typeof(float), smallNumericStep: true);
			}
			void AddPropsSlider(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Disabled", Thing.Property.UI_SLIDER_IS_DISABLED, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Length Unit", Thing.Property.UI_SLIDER_LENGTH_UNIT, typeof(float), smallNumericStep: true);
				AddThingProperty(table, "Length", Thing.Property.UI_SLIDER_LENGTH, typeof(float));
				AddSpace(table);
				AddThingProperty(table, "Progress Col.", Thing.Property.UI_SLIDER_PROGRESS_COLOR, typeof(Color));
				AddThingProperty(table, "Empty Col.", Thing.Property.UI_SLIDER_EMPTY_COLOR, typeof(Color));
			}
			void AddPropsList(TableLayoutPanel table)
			{
				AddThingProperty(table, "Buttons Tag", Thing.Property.UI_LIST_BUTTONS_TAG, typeof(string));
				AddSpace(table);
				AddThingProperty(table, "Visible Btn Count Max", Thing.Property.UI_LIST_VISIBLE_BUTTON_COUNT_MAX, typeof(int), labelSizeOffset: 2);
				AddThingProperty(table, "Visible Btn Count Current", Thing.Property.UI_LIST_VISIBLE_BUTTON_COUNT_CURRENT,
					typeof(int), labelSizeOffset: 0, readOnly: true);
				AddSpace(table);
				AddThingProperty(table, "Button Spacing", Thing.Property.UI_LIST_BUTTON_SPACING, typeof(float));
				AddThingProperty(table, "Button Width", Thing.Property.UI_LIST_BUTTON_WIDTH, typeof(float));
				AddThingProperty(table, "Button Height", Thing.Property.UI_LIST_BUTTON_HEIGHT, typeof(float), readOnly: true);
			}
			void AddPropsListDropdown(TableLayoutPanel table)
			{
				AddThingProperty(table, "Btn Show UID", Thing.Property.UI_LIST_DROPDOWN_BUTTON_SHOW_UID, typeof(string), labelSizeOffset: 2);
				AddSpace(table);
				AddThingProperty(table, "Selection Index", Thing.Property.UI_LIST_DROPDOWN_SELECTION_INDEX, typeof(int));
				AddThingProperty(table, "Selection UID", Thing.Property.UI_LIST_DROPDOWN_SELECTION_UID, typeof(string), readOnly: true);
			}
			void AddPropsListCarousel(TableLayoutPanel table)
			{
				AddThingProperty(table, "Is Repeating", Thing.Property.UI_LIST_CAROUSEL_IS_REPEATING, typeof(bool));
				AddSpace(table);
				AddThingProperty(table, "Selection Index", Thing.Property.UI_LIST_CAROUSEL_SELECTION_INDEX, typeof(int));
				AddThingProperty(table, "Selection UID", Thing.Property.UI_LIST_CAROUSEL_SELECTION_UID, typeof(string), readOnly: true);
			}
			void AddPropsListMultiselect(TableLayoutPanel table)
			{
				AddThingProperty(table, "Selection Indexes", Thing.Property.UI_LIST_MULTISELECT_SELECTION_INDEXES, typeof(ReadOnlyCollection<int>));
				AddThingProperty(table, "Selection UIDs", Thing.Property.UI_LIST_MULTISELECT_SELECTION_UIDS, typeof(ReadOnlyCollection<string>));
			}

			void AddSpace(TableLayoutPanel table)
			{
				AddThingProperty(table, ""); AddThingProperty(table, "");
			}
			void AddButton(TableLayoutPanel table, string text, EventHandler clickMethod)
			{
				var btn = new Button()
				{
					Text = text,
					Dock = DockStyle.Fill,
					ForeColor = System.Drawing.Color.White,
					Font = new System.Drawing.Font(FONT, FONT_SIZE, FontStyle.Regular, GraphicsUnit.Point)
				};
				btn.Click += clickMethod;
				table.Controls.Add(btn);
			}
		}
		private void AddThingProperty(TableLayoutPanel table, string label, string? propName = null, Type? valueType = null, bool readOnly = false,
			bool rightLabel = false, bool smallNumericStep = false, bool last = false, float labelSizeOffset = 3, bool thingList = false, bool uniqueList = true)
		{
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

			var valueTypeIsFlag = valueType.IsDefined(typeof(FlagsAttribute), false);
			if(valueType == typeof(string) || (valueType.IsEnum && valueTypeIsFlag) || valueType == typeof(Thing.CubeSide))
			{
				prop = new TextBox();
				prop.TextChanged += OnTextBoxChange;
				SetFocusHotkeyPrevention(prop);
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
				SetFocusHotkeyPrevention(prop);
			}
			else if(valueType == typeof(float))
			{
				prop = new NumericUpDown();
				SetDefaultNumeric((NumericUpDown)prop, false);
				SetFocusHotkeyPrevention(prop);
			}
			else if(valueType == typeof(List<string>) || (valueType.IsEnum && valueTypeIsFlag == false) ||
				valueType == typeof(ReadOnlyCollection<string>) || valueType == typeof(ReadOnlyCollection<int>) ||
				valueType == typeof(Dictionary<string, Thing.Tile>))
				prop = CreateList();
			else if(valueType == typeof(Button))
				prop = new Button() { Text = label };
			else if(valueType == typeof(Vector2))
				prop = CreateMultipleValuesTable(2, false);
			else if(valueType == typeof(Vector3))
				prop = CreateMultipleValuesTable(3, false);
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

			if(propName == Thing.Property.HITBOX)
			{
				editHitbox = new CheckBox() { Text = "Edit", Dock = DockStyle.Left };
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.ForeColor = System.Drawing.Color.White;
				lab.Controls.Add(editHitbox);
			}
			else if(valueType == typeof(int) && propName == "TileCount")
			{
				paintTile = new CheckBox() { Text = "Paint", Dock = DockStyle.Left, Width = 75 };
				paintTile.CheckedChanged += OnPaintTileCheck;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.ForeColor = System.Drawing.Color.White;
				lab.Controls.Add(paintTile);
			}
			else if(valueType.IsEnum && valueType.IsDefined(typeof(FlagsAttribute), false))
			{
				CreateButton("Options", SelectFlagsEnum, 90);
				lab.ForeColor = System.Drawing.Color.White;
			}
			else if(valueType == typeof(Thing.CubeSide))
			{
				CreateButton("Modify", CustomizeCubeSide, 90);
				lab.ForeColor = System.Drawing.Color.White;
			}

			if(propName == null || readOnly)
				return;

			if(propName.Contains("Path") && propName.Contains("Paths") == false)
				CreateButton("Assets", PickAsset);
			else if(valueType == typeof(string))
			{
				if(propName != Thing.Property.UID && propName.Contains("UID"))
					CreateButton("Things", PickThingList);
				else if(propName == Thing.Property.TEXT_VALUE)
					CreateButton("Text", GetBigText);
			}
			else if(valueType == typeof(Color))
				CreateButton("Colors", PickColor);
			else if(valueType == typeof(List<string>))
				CreateButton("Items", EditTextList);
			else if(valueType == typeof(Thing.Effect))
				CreateButton("Uniforms", SetUniform, 100);
			else if(valueType == typeof(Dictionary<string, Thing.Tile>))
				CreateButton("Palette", EditTilePalette);

			void CreateButton(string text, EventHandler clickMethod, int width = 80)
			{
				var btn = new Button() { Text = text, Dock = DockStyle.Left, Width = width };
				btn.Click += clickMethod;
				lab.TextAlign = ContentAlignment.MiddleRight;
				lab.Controls.Add(btn);
			}
			void SelectFlagsEnum(object? sender, EventArgs e)
			{
				var list = new ListBox()
				{
					Left = SPACING_X,
					Width = W - (int)(SPACING_X * 2.5f),
					Height = H - SPACING_Y * 2,
					Top = SPACING_Y,
					SelectionMode = SelectionMode.MultiSimple
				};
				var sz = new Vector2i(W, H);
				var window = new Form()
				{
					Width = sz.X,
					Height = list.Height + SPACING_Y * 4,
					FormBorderStyle = FormBorderStyle.FixedToolWindow,
					Text = "Select One or Multiple Options",
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White,
					StartPosition = FormStartPosition.CenterScreen
				};
				window.Controls.Add(list);

				var names = Enum.GetNames(valueType);
				for(int i = 0; i < names.Length; i++)
					list.Items.Add(names[i]);

				window.ShowDialog();

				var items = list.SelectedItems;
				var values = new List<int>();
				for(int i = 0; i < items.Count; i++)
					values.Add((int)Enum.Parse(valueType, (string)items[i]));
				var sum = values.Sum();

				var result = Enum.ToObject(valueType, sum);
				Thing.Set(selectedUIDs[0], propName, result);
			}
			void PickThingList(object? sender, EventArgs e)
			{
				if(sender == null)
					return;
				var control = (Control)sender;
				PickThing(control, null, propName);
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
						case "PropSceneClothColor": cols["Cloth"] = C(cols["Cloth"]); break;
						case "PropSceneSpriteStackColor": cols["SpriteStack"] = C(cols["SpriteStack"]); break;
						case "PropSceneCubeColor": cols["Cube"] = C(cols["Cube"]); break;
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

				var asset = GetMirrorAssetPath(pickAsset.FileName);
				if(propName.Contains(nameof(Thing.CubeSide)) && cubeSideTexPath != null)
				{
					cubeSideTexPath.Text = asset;
					return;
				}
				else if(isSpriteStackCreationOpen)
				{
					if(propName == "SpriteStackTexturePath" && spriteStackCreateTexPath != null)
						spriteStackCreateTexPath.Text = asset;
					else if(propName == "SpriteStackObjPath" && spriteStackCreateObjPath != null)
						spriteStackCreateObjPath.Text = asset;

					return;
				}

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
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_BOOL, a[0], true);
				else if(a[1] == "false")
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_BOOL, a[0], false);
				else if(a.Length == 2 && a[1].IsNumber())
				{
					var n = a[1].ToNumber();
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_FLOAT, a[0], n);
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_INT, a[0], (int)n);
				}
				else if(a.Length == 3 && a[1].IsNumber() && a[2].IsNumber())
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_VECTOR2, a[0], new Vector2(a[1].ToNumber(), a[2].ToNumber()));
				else if(a.Length == 4 && a[1].IsNumber() && a[2].IsNumber() && a[3].IsNumber())
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_VECTOR3, a[0], new Vector3(a[1].ToNumber(), a[2].ToNumber(), a[3].ToNumber()));
				else if(a.Length == 5 && a[1].IsNumber() && a[2].IsNumber() && a[3].IsNumber() && a[4].IsNumber())
					Thing.CallVoid(uid, Thing.Method.VISUAL_SET_EFFECT_VECTOR4, a[0], new Vector4(a[1].ToNumber(), a[2].ToNumber(), a[3].ToNumber(), a[4].ToNumber()));
				else
					MessageBox.Show(this, $"The {effect} uniform was not set due to invalid input.", $"Set {effect} Uniform Failed");
			}
			void EditTilePalette(object? sender, EventArgs e)
			{
				var palette = (Dictionary<string, Thing.Tile>)Thing.Get(selectedUIDs[0], Thing.Property.TILEMAP_TILE_PALETTE);
				var sz = new Vector2i(W + 120, H + 375);
				var window = new Form()
				{
					Text = "Edit Palette",
					Width = sz.X * 2,
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
					Height = sz.Y / 2,
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White
				};
				var table = new TableLayoutPanel
				{
					Top = listBox.Height + SPACING_Y,
					Left = SPACING_X,
					Width = sz.X - SPACING_X * 2,
					Height = 115,
					ColumnCount = 2,
					RowCount = 3,
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
				var image = new PictureBox()
				{
					Width = 0,
					Height = 0,
					Top = SPACING_Y * 2,
					Left = window.Size.Width / 2,
					BorderStyle = BorderStyle.Fixed3D
				};
				var tileWindow = new RenderWindow(image.Handle) { Size = new((uint)sz.X - (uint)(SPACING_X * 1.5f), (uint)(sz.Y - SPACING_Y * 5.5f)) };
				var view = tileWindow.GetView();
				var zoom = 1f;
				var windowSize = new Vector2(tileWindow.Size.X, tileWindow.Size.Y);
				var tileSz = (Vector2)Thing.Get(selectedUIDs[0], Thing.Property.TILEMAP_TILE_SIZE);
				var tileGap = (Vector2)Thing.Get(selectedUIDs[0], Thing.Property.TILEMAP_TILE_GAP);

				tilePaletteHoveredIndexesLabel.AutoSize = true;
				tilePaletteHoveredIndexesLabel.Dock = DockStyle.Right;
				tilePaletteHoveredIndexesLabel.TextAlign = ContentAlignment.TopRight;
				tilePaletteHoveredIndexesLabel.Font = new System.Drawing.Font("Arial", 24);

				tileWindow.MouseWheelScrolled += OnScroll;

				AddThingProperty(table, "Tile", rightLabel: true); AddThingProperty(table, "Properties");
				AddThingProperty(table, "Indecies", "TilePaletteTexCoord", valueType: typeof(Vector2));
				AddThingProperty(table, "Color", "TilePaletteColor", valueType: typeof(Color));

				var texCoord = table.Controls[2];
				var color = table.Controls[4];
				tilePaletteCol = color;

				color.BackColor = System.Drawing.Color.White;
				((NumericUpDown)color.Controls[0]).Value = 255;
				((NumericUpDown)color.Controls[1]).Value = 255;
				((NumericUpDown)color.Controls[2]).Value = 255;
				((NumericUpDown)color.Controls[3]).Value = 255;

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
					SetControlNumber((NumericUpDown)texCoord.Controls[0], tile.Indexes.X);
					SetControlNumber((NumericUpDown)texCoord.Controls[1], tile.Indexes.Y);
					SetControlNumber((NumericUpDown)color.Controls[0], tile.Color.R);
					SetControlNumber((NumericUpDown)color.Controls[1], tile.Color.G);
					SetControlNumber((NumericUpDown)color.Controls[2], tile.Color.B);
					SetControlNumber((NumericUpDown)color.Controls[3], tile.Color.A);
					color.BackColor = System.Drawing.Color.FromArgb(255, tile.Color.R, tile.Color.G, tile.Color.B);

					textBox.Text = uid;

					if(window.Visible)
						tilePaletteUID = uid;
				};

				for(int i = 0; i < 2; i++)
					table.ColumnStyles.Add(new(SizeType.Percent, 50));
				for(int i = 0; i < 3; i++)
					table.RowStyles.Add(new(SizeType.Percent, 50));

				window.Controls.AddRange(new Control[] { table, listBox, textBox, buttonSet, buttonRemove, image, tilePaletteHoveredIndexesLabel });

				var running = true;
				var thread = new Thread(UpdateWindow) { Name = "TileSelectWindow" };
				thread.Start();

				window.ShowDialog(this);
				running = false;

				tileWindow.Close();
				tileWindow.Dispose();

				void UpdateWindow()
				{
					var path = (string)Thing.Get(selectedUIDs[0], Thing.Property.VISUAL_TEXTURE_PATH);
					if(path == null)
						return;

					var texture = new Texture(path);
					var sz = texture.Size;
					var verts = new Vertex[]
					{
						new(new(0, 0), Color.White, new(0, 0)),
						new(new(sz.X, 0), Color.White, new(sz.X, 0)),
						new(new(sz.X, sz.Y), Color.White, new(sz.X, sz.Y)),
						new(new(0, sz.Y), Color.White, new(0, sz.Y)),
					};
					var prevMousePos = new Vector2i();
					while(running)
					{
						tileWindow.DispatchEvents();
						tileWindow.Clear(bgCol);

						var mousePos = Mouse.GetPosition();
						var indexPos = (tileWindow.MapPixelToCoords(Mouse.GetPosition(tileWindow), view).ToSystem()).PointToGrid(tileSz + tileGap).ToSFML();
						var hoveredIndexes = (indexPos.ToSystem() / (tileSz + tileGap));
						tilePaletteHoveredIndexes = $"X:{hoveredIndexes.X} Y:{hoveredIndexes.Y}";

						var lines = new List<Line> { new Line(indexPos.ToSystem(), new(indexPos.X + tileSz.X, indexPos.Y)) };
						lines.Add(new Line(lines[0].B, indexPos.ToSystem() + tileSz));
						lines.Add(new Line(lines[1].B, new(indexPos.X, indexPos.Y + tileSz.Y)));
						lines.Add(new Line(lines[2].B, lines[0].A));

						var hoveredTileVerts = new Vertex[]
						{
							new(indexPos, selCol),
							new(new(indexPos.X + tileSz.X, indexPos.Y), selCol),
							new(indexPos + tileSz.ToSFML(), selCol),
							new(new(indexPos.X, indexPos.Y + tileSz.Y), selCol),
						};

						if(Mouse.IsButtonPressed(Mouse.Button.Middle))
						{
							var delta = prevMousePos - mousePos;
							view.Center += new Vector2f(delta.X * zoom, delta.Y * zoom);
						}
						view.Center = new Vector2f(view.Center.X.Limit(0, texture.Size.X), view.Center.Y.Limit(0, texture.Size.Y));
						view.Size = (windowSize * zoom).ToSFML();

						if(running == false)
							return;
						tileWindow.SetView(view);

						tileWindow.Draw(verts, PrimitiveType.Quads, new(SFML.Graphics.BlendMode.Alpha, Transform.Identity, texture, null));
						lines.Draw(tileWindow, bbCol, zoom * 4);
						tileWindow.Draw(hoveredTileVerts, PrimitiveType.Quads);
						tileWindow.Display();
						prevMousePos = mousePos;
					}
					texture.Dispose();
				}
				void OnScroll(object? sender, MouseWheelScrollEventArgs e)
				{
					var way = e.Delta < 0 ? 1f : -1f;
					zoom += zoom * 0.05f * way;
					zoom = zoom.Limit(0.05f, 3f);
					var pos = view.Center.ToSystem();
					view.Center = pos.PointPercentTowardPoint(tileWindow.MapPixelToCoords(Mouse.GetPosition(tileWindow), view).ToSystem(), new(5)).ToSFML();
				}
			}
			void OnPaintTileCheck(object? sender, EventArgs e)
			{
				if(selectedUIDs.Count == 0)
					return;

				var palette = (Dictionary<string, Thing.Tile>)Thing.Get(selectedUIDs[0], "TilePalette");
				if(paintTile.Checked && palette.Count == 0)
					MessageBox.Show(this, "The Tile Palette is empty. Add tiles before painting.", "Paint Tile");
			}
			void CustomizeCubeSide(object? sender, EventArgs e)
			{
				const int TABLE_HEIGHT = 150;
				var sz = new Vector2i(W + SPACING_X * 4, H + TABLE_HEIGHT);
				var window = new Form()
				{
					Width = sz.X,
					Height = sz.Y,
					FormBorderStyle = FormBorderStyle.FixedToolWindow,
					Text = $"Modify {propName?.Replace("Side", "")} Cube Side",
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White,
					StartPosition = FormStartPosition.CenterScreen
				};
				var table = new TableLayoutPanel
				{
					Top = SPACING_Y,
					Left = SPACING_X,
					Width = sz.X - SPACING_X * 2 - SPACING_X / 2,
					Height = TABLE_HEIGHT,
					ColumnCount = 2,
					RowCount = 6,
					Name = "PropModifyCubeSide"
				};
				var button = new Button()
				{
					Text = "OK",
					Top = table.Height + SPACING_Y * 2,
					Left = sz.X - BUTTON_W - SPACING_X - SPACING_X / 4,
					Width = BUTTON_W,
					Height = BUTTON_H,
					DialogResult = DialogResult.OK
				};
				button.Click += (sender, e) => { window.Close(); };
				window.Controls.Add(table);
				window.Controls.Add(button);
				window.AcceptButton = button;

				for(int i = 0; i < 2; i++)
					table.ColumnStyles.Add(new(SizeType.Percent, 50));
				for(int i = 0; i < 4; i++)
					table.RowStyles.Add(new(SizeType.Percent, 50));

				AddThingProperty(table, "Is Hidden", $"{nameof(Thing.CubeSide)}{nameof(Thing.CubeSide.IsHidden)}", valueType: typeof(bool));
				AddThingProperty(table, "Texture Path", $"{nameof(Thing.CubeSide)}{nameof(Thing.CubeSide.TexturePath)}", valueType: typeof(string));
				AddThingProperty(table, "Texture Coordinate Unit A", $"{nameof(Thing.CubeSide)}{nameof(Thing.CubeSide.TexCoordUnitA)}",
					valueType: typeof(Vector2), labelSizeOffset: 2, smallNumericStep: true);
				AddThingProperty(table, "Texture Coordinate Unit B", $"{nameof(Thing.CubeSide)}{nameof(Thing.CubeSide.TexCoordUnitB)}",
					valueType: typeof(Vector2), labelSizeOffset: 2, smallNumericStep: true);

				var hidden = (CheckBox)table.Controls[0];
				var tex = (TextBox)table.Controls[2];
				var texCoordUnitA = table.Controls[4];
				var texCoordUnitB = table.Controls[6];

				var cubeSide = (Thing.CubeSide)Thing.Get(selectedUIDs[0], propName);

				cubeSideTexPath = tex;

				hidden.Checked = cubeSide.IsHidden;
				tex.Text = cubeSide.TexturePath;
				((NumericUpDown)texCoordUnitA.Controls[0]).Value = (decimal)cubeSide.TexCoordUnitA.X;
				((NumericUpDown)texCoordUnitA.Controls[1]).Value = (decimal)cubeSide.TexCoordUnitA.Y;
				((NumericUpDown)texCoordUnitB.Controls[0]).Value = (decimal)cubeSide.TexCoordUnitB.X;
				((NumericUpDown)texCoordUnitB.Controls[1]).Value = (decimal)cubeSide.TexCoordUnitB.Y;

				if(window.ShowDialog() != DialogResult.OK)
					return;

				cubeSide.IsHidden = hidden.Checked;
				cubeSide.TexturePath = tex.Text;
				cubeSide.TexCoordUnitA = new Vector2((float)((NumericUpDown)texCoordUnitA.Controls[0]).Value, (float)((NumericUpDown)texCoordUnitA.Controls[1]).Value);
				cubeSide.TexCoordUnitB = new Vector2((float)((NumericUpDown)texCoordUnitB.Controls[0]).Value, (float)((NumericUpDown)texCoordUnitB.Controls[1]).Value);

				Thing.Set(selectedUIDs[0], propName, cubeSide);
			}
			void GetBigText(object? sender, EventArgs e)
			{
				if(selectedUIDs.Count == 0)
					return;

				var value = (string)Thing.Get(selectedUIDs[0], propName);
				var sz = new Vector2i(W, H + 100);
				var window = new Form()
				{
					Width = sz.X,
					Height = sz.Y,
					FormBorderStyle = FormBorderStyle.FixedToolWindow,
					Text = "Edit Text",
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White,
					StartPosition = FormStartPosition.CenterScreen
				};
				var textBox = new TextBox()
				{
					Left = SPACING_X,
					Top = SPACING_Y,
					Width = sz.X - BUTTON_W - SPACING_X * 3,
					Height = sz.Y - BUTTON_H - SPACING_Y * 4,
					Text = value,
					Multiline = true,
					BackColor = System.Drawing.Color.Black,
					ForeColor = System.Drawing.Color.White
				};
				var button = new Button()
				{
					Text = "OK",
					Left = sz.X - BUTTON_W - SPACING_X - SPACING_X / 4,
					Width = BUTTON_W,
					Height = BUTTON_H,
					Top = SPACING_Y + textBox.Height,
					DialogResult = DialogResult.OK
				};
				window.FormClosing += (sender, e) =>
				{
					if(IsKeyPressed(Key.Enter) && textBox.Focused)
						e.Cancel = true;
				};
				textBox.PreviewKeyDown += (sender, e) =>
				{
					if(e.KeyCode == Keys.Enter)
						textBox.AppendText(Environment.NewLine);
				};
				button.Click += (sender, e) => window.Close();
				window.Controls.Add(textBox);
				window.Controls.Add(button);
				window.AcceptButton = button;

				if(window.ShowDialog() != DialogResult.OK)
					return;

				Thing.Set(selectedUIDs[0], propName, textBox.Text);
				UpdateThingPanel();
			}

			void SetFocusHotkeyPrevention(Control control)
			{
				control.GotFocus += (s, e) => { isTyping = true; };
				control.LostFocus += (s, e) => { isTyping = false; };
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
					SetFocusHotkeyPrevention(col);
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
			// forms can't be accessed from a separate thread
			// the tile palette edit preview creates a window and a thread but can't access any of the forms
			// so it sets a string which updates the form here
			if(tilePaletteHoveredIndexesLabel != null)
				tilePaletteHoveredIndexesLabel.Text = tilePaletteHoveredIndexes;

			loading.Visible = loadingDescr != "";
			loading.Text = loadingDescr;
			windowSplit.Visible = loading.Visible == false;

			window.Size = new((uint)windowPicture.Width, (uint)windowPicture.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.DispatchEvents();
			window.Clear(bgCol);

			DrawGrid();
			UpdateSceneValues();

			TrySelect();

			TryDrawAllNonVisuals();

			TryHotkeys();

			Game.UpdateEngine(window);
			TryDrawSelection();
			Game.FinishRendering(window, window);

			if(editHitbox != null && (editHitbox.Checked == false).Once("uncheck-edit-hitbox"))
			{
				selectedHitboxPointIndexesA.Clear();
				selectedHitboxPointIndexesB.Clear();
			}
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
					case "ReadOnlyCollection<Int32>": ProcessList((ComboBox)control, (ReadOnlyCollection<int>)Thing.Get(uid, propName)); break;
					case "BlendMode": ProcessEnumList((ComboBox)control, typeof(Thing.BlendMode), propName); break;
					case "TextboxAlignment": ProcessEnumList((ComboBox)control, typeof(SMPL.UI.Thing.TextboxAlignment), propName); break;
					case "Styles": ProcessEnumFlagList((TextBox)control, typeof(Text.Styles)); break;
					case "Effect": ProcessEnumList((ComboBox)control, typeof(Thing.Effect), propName); break;
					case "AudioStatus": ProcessEnumList((ComboBox)control, typeof(Thing.AudioStatus), propName); break;
					case "Hitbox": SetText((TextBox)control, $"{((Hitbox)Thing.Get(uid, propName)).Lines.Count} Lines", readOnly); break;
					case "CubeSide":
						{
							var cubeSide = (Thing.CubeSide)Thing.Get(uid, propName);
							var texPath = string.IsNullOrWhiteSpace(cubeSide.TexturePath) ?
								(string)Thing.Get(uid, Thing.Property.VISUAL_TEXTURE_PATH) : cubeSide.TexturePath;
							SetText((TextBox)control, $"{texPath}", true);
							break;
						}
					case "Dictionary<String, Tile>":
						{
							var prevIndex = ((ComboBox)control).SelectedIndex;
							var palette = (Dictionary<string, Thing.Tile>)Thing.Get(uid, Thing.Property.TILEMAP_TILE_PALETTE);
							ProcessList((ComboBox)control, palette.Keys);
							((ComboBox)control).SelectedIndex = prevIndex == -1 && palette.Count > 0 ? 0 : prevIndex;
							break;
						}
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
			void ProcessEnumFlagList(TextBox count, Type enumType)
			{
				if(enumType.IsEnum == false || enumType.IsDefined(typeof(FlagsAttribute), false) == false)
					return;

				var names = Enum.GetNames(enumType);
				count.Text = $"{names.Length} Options";
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
		private static void SetControlNumber(NumericUpDown number, float value, bool readOnly = false)
		{
			number.Enabled = readOnly == false;
			if(double.IsInfinity(value) || double.IsNaN(value))
				value = 0;
			number.Value = (decimal)value.Limit((float)number.Minimum, (float)number.Maximum);
		}

		private void TryDrawAllNonVisuals()
		{
			var uids = Thing.GetUIDs();
			for(int i = 0; i < uids.Count; i++)
			{
				var uid = uids[i];
				var type = ((ReadOnlyCollection<string>)Thing.Get(uid, Thing.Property.TYPES))[0];

				if(typeColors.ContainsKey(type) == false ||
					(type == "Tilemap" && paintTile != null && paintTile.Checked))
					continue;

				var boundingBox = (Hitbox)Thing.Get(uid, Thing.Property.BOUNDING_BOX);
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
			if(control.Enabled && listBoxPlaceholderTexts.ContainsKey(control.Name) &&
				control.Items.Contains(listBoxPlaceholderTexts[control.Name]) == false)
				control.Items.Add(listBoxPlaceholderTexts[control.Name]);
			else if(control.Enabled == false && control.Items.Contains("(none)") == false)
				control.Items.Add("(none)");
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
			var tileIndecies = Thing.CallGet(uid, Thing.Method.TILEMAP_GET_TILE_INDEXES, GetMousePosition());
			var startRightTileInd = Thing.CallGet(uid, Thing.Method.TILEMAP_GET_TILE_INDEXES, tilePaintRightClickPos);
			var alt = IsKeyPressed(Key.LAlt);

			if(Mouse.IsButtonPressed(Mouse.Button.Left))
			{
				if(alt)
					Thing.CallVoid(uid, Thing.Method.TILEMAP_REMOVE_TILES, tileIndecies);
				else
					Thing.CallVoid(uid, Thing.Method.TILEMAP_SET_TILE, tileIndecies, tilePaletteUID);
			}
			else if(Mouse.IsButtonPressed(Mouse.Button.Right) && tilePaintRightClickPos.IsNaN() == false)
			{
				if(alt)
					Thing.CallVoid(uid, Thing.Method.TILEMAP_REMOVE_TILE_SQUARE, startRightTileInd, tileIndecies);
				else
					Thing.CallVoid(uid, Thing.Method.TILEMAP_SET_TILE_SQUARE, startRightTileInd, tileIndecies, tilePaletteUID);
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
				SetColor("PropSceneClothColor", typeColors["Cloth"]);
				SetColor("PropSceneSpriteStackColor", typeColors["SpriteStack"]);
				SetColor("PropSceneCubeColor", typeColors["Cube"]);

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
				DrawBoundingBox((Hitbox)Thing.Get(selectedUIDs[i], Thing.Property.BOUNDING_BOX));

			var ptsA = selectedHitboxPointIndexesA;
			var ptsB = selectedHitboxPointIndexesB;
			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[i], Thing.Property.HITBOX);
				for(int j = 0; j < hitbox.Lines.Count; j++)
				{
					var col = ptsA.Contains(j) || ptsB.Contains(j) ? hitboxSelectCol : hitboxDeselectedCol;
					hitbox.Lines[j].Draw(window, col, sceneSc * 4);
				}
			}

			if(editHitbox == null || editHitbox.Checked == false)
				return;

			var lines = ((Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.HITBOX)).Lines;
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
					var pos = (Vector2)Thing.Get(selectedUIDs[i], Thing.Property.POSITION);
					pos.DrawPoint(window, Color.Red, sceneSc * 4);
				}

				window.Draw(fill, PrimitiveType.Quads);
			}
		}
		private void TryHotkeys()
		{
			if(ActiveForm != this || isTyping)
				return;

			foreach(var kvp in hotkeys)
			{
				var pressedKeys = 0;
				var id = "";

				for(int i = 0; i < kvp.Key.Length; i++)
				{
					if(IsKeyPressed(kvp.Key[i]))
						pressedKeys++;

					id += $"{kvp.Key[i]} ";
				}

				var enoughKeys = pressedKeys == kvp.Key.Length;
				if(enoughKeys.Once(id))
				{
					createPosition = GetMousePosition();
					sceneRightClickMenu.Hide();
					kvp.Value.Invoke();
				}
				if(enoughKeys)
					return; // no further hotkeys, order matters ('ctrl + s' comes before 's')
			}
		}
		#endregion

		#region View
		private void ResetView() => SetView();
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

			SetViewPosition((Vector2)Thing.Get(uid, Thing.Property.POSITION));

			var bb = (Hitbox)Thing.Get(uid, Thing.Property.BOUNDING_BOX);
			var dist1 = bb.Lines[0].A.DistanceBetweenPoints(bb.Lines[0].B);
			var dist2 = bb.Lines[1].A.DistanceBetweenPoints(bb.Lines[1].B);
			var scale = (dist1 > dist2 ? dist1 : dist2) / window.Size.X * 2f;
			SetViewScale(scale + 0.5f);
		}
		#endregion
		#region Get
		private static Hitbox? GetBoundingBox(string uid)
		{
			return Thing.Info.HasGetter(uid, Thing.Property.BOUNDING_BOX) == false ? default : (Hitbox)Thing.Get(uid, Thing.Property.BOUNDING_BOX);
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
		private void TrackAssets()
		{
			var assetsPath = GetAssetsPath();
			if(Directory.Exists(assetsPath))
			{
				Loading(LOADING_ASSETS);
				CopyMirrorFiles(assetsPath);
				Scene.CurrentScene.LoadAssets(GetMirrorAssetPath(assetsPath));
			}
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
			Loading();
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
			var time = new Clock();
			while(FileIsLocked(path))
			{
				if(time.ElapsedTime.AsSeconds() > 3)
					return; // takes too long, forget it

				Thread.Sleep(100);
			}

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

			if(Directory.GetFiles(path).Length == 0)
				Directory.Delete(path);
		}
		private void DeleteCache()
		{
			Loading("Deleting cached assets...");
			var dirs = Directory.GetDirectories(AppContext.BaseDirectory);
			for(int i = 0; i < dirs.Length; i++)
				if(Path.GetFileName(dirs[i]) != "runtimes")
					DeleteFiles(dirs[i]);
			Loading();
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
		private void TryLoadScene()
		{
			selectedUIDs.Clear();
			TryResetThingPanel();

			if(paintTile != null)
				paintTile.Checked = false;
			if(editHitbox != null)
				editHitbox.Checked = false;

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

			Scene.Load(load.FileName);
			Scene.CurrentScene.LoadAssets(GetMirrorAssetPath(assetsPath));

			TrackAssets();

			selectedUIDs.Clear();
			SetView();
			Loading();
		}
		private void TrySaveScene()
		{
			if(File.Exists(scenePath))
				SaveScene();
			else
				TrySaveSceneAs();
		}
		private void TrySaveSceneAs()
		{
			if(save.ShowDialog(this) != DialogResult.OK)
				return;

			SaveScene();
		}
		private void SaveScene()
		{
			scenePath = string.IsNullOrWhiteSpace(save.FileName) ? load.FileName : save.FileName;
			Scene.CurrentScene.Name = Path.GetFileNameWithoutExtension(scenePath);
			Loading("Saving Scene...");
			Scene.CurrentScene.Save(Path.GetDirectoryName(scenePath));

			TrackAssets();
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
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.HITBOX);
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
		private void TrySelectThing(string uid)
		{
			if(selectedUIDs.Contains(uid))
				return;

			selectedUIDs.Add(uid);
			TryResetThingPanel();
		}
		private void DeselectAll()
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
		private void TryDuplicateSelection()
		{
			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var uid = selectedUIDs[i];
				var dupUID = Thing.GetFreeUID(uid);
				var pos = (Vector2)Thing.Get(uid, Thing.Property.POSITION);
				var sc = (float)Thing.Get(uid, Thing.Property.SCALE);

				Thing.Duplicate(uid, dupUID);
				Thing.Set(dupUID, Thing.Property.POSITION, pos.PointMoveAtAngle(45, sc * 20, false));
			}
		}
		private void TryDestroySelection()
		{
			if(ActiveForm != this || (paintTile != null && paintTile.Checked))
				return;

			if(editHitbox != null && editHitbox.Checked)
			{
				// work on copy lists since removing from the main list changes other indexes in the main list but not the selectedPts lists
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.HITBOX);
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
				TryResetThingPanel();
			}
		}

		private void AddHitboxLine(string uid, List<Line> line)
		{
			var hitbox = (Hitbox)Thing.Get(uid, Thing.Property.HITBOX);
			for(int i = 0; i < line.Count; i++)
				hitbox.LocalLines.Add(new Line(Local(line[i].A), Local(line[i].B)));
			UpdateThingPanel();

			Vector2 Local(Vector2 global) => (Vector2)Thing.CallGet(uid, Thing.Method.GET_LOCAL_POSITION_FROM_SELF, global);
		}
		private void AddLineToHitbox()
		{
			if(editHitbox == null || editHitbox.Checked == false)
				return;

			var off = new Vector2(50f * sceneSc, 0);
			AddHitboxLine(selectedUIDs[0], new List<Line>() { new(createPosition - off, createPosition + off) });
		}
		private void AddSquareToHitbox()
		{
			if(editHitbox == null || editHitbox.Checked == false)
				return;

			var off = new Vector2(50f) * sceneSc;
			var uid = selectedUIDs[0];
			var p = createPosition;
			var lines = new List<Line>()
			{
				new(p + new Vector2(-off.X, -off.Y), p + new Vector2(off.X, -off.Y)),
				new(p + new Vector2(off.X, -off.Y), p + new Vector2(off.X, off.Y)),
				new(p + new Vector2(off.X, off.Y), p + new Vector2(-off.X, off.Y)),
				new(p + new Vector2(-off.X, off.Y), p + new Vector2(-off.X, -off.Y)),
			};
			AddHitboxLine(uid, lines);
		}
		private void AddCircleToHitbox()
		{
			if(editHitbox == null || editHitbox.Checked == false)
				return;

			var uid = selectedUIDs[0];
			var radius = 50f * sceneSc;
			var angStep = 360f / 8f;
			var lines = new List<Line>();

			for(int i = 0; i < 8; i++)
			{
				var p = createPosition.PointMoveAtAngle(angStep * i, radius, false);
				var p1 = createPosition.PointMoveAtAngle(angStep * (i - 1), radius, false);
				lines.Add(new(p, p1));
			}
			AddHitboxLine(uid, lines);
		}

		private void CreateSprite()
		{
			var uid = Thing.CreateSprite("Sprite", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateText()
		{
			var uid = Thing.CreateText("Text", GetFont("Create Text", "Text"));
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateNinePatch()
		{
			var uid = Thing.CreateNinePatch("NinePatch", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateLight()
		{
			var uid = Thing.CreateLight("Light", Color.White);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateCamera()
		{
			var res = GetVector2("Create Camera", "Provide the Camera resolution.", 1, 7680, 1, 4320, new(1280, 720));
			if(res == default)
				return;

			var uid = Thing.CreateCamera("Camera", res);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateTilemap()
		{
			var uid = Thing.CreateTilemap("Tilemap", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateAudio()
		{
			var result = DialogResult.None;
			if(string.IsNullOrWhiteSpace(GetAssetsPath()))
				MessageBox.Show(this, "This Audio will be silent without an Audio file.\n\n" + NO_ASSETS_MSG, "Create Audio");
			else if(MessageBox.Show(this, "Pick an Audio file?", "Create Audio", MessageBoxButtons.YesNo) == DialogResult.Yes)
				result = pickAsset.ShowDialog();

			var uid = Thing.CreateAudio("Audio", result != DialogResult.OK ? null : GetMirrorAssetPath(pickAsset.FileName));
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateCloth()
		{
			var size = GetVector2("Create Cloth", "Provide a constant size for the Cloth.", 1, int.MaxValue, 1, int.MaxValue, new(300));
			if(size == default)
				return;
			var fragmentAmount = GetVector2("Create Cloth", "Provide the amount of fragments the Cloth is made of (in the range [1-10]). " +
				"Or in other words - the detail.", 1, 10, 1, 10, new(5, 5));
			if(fragmentAmount == default)
				return;

			var uid = Thing.CreateCloth("Cloth", null, size.X, size.Y, (int)fragmentAmount.X, (int)fragmentAmount.Y);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateSpriteStack()
		{
			var texStackName = default(string);
			if(string.IsNullOrWhiteSpace(GetAssetsPath()))
				MessageBox.Show(this, "This Sprite Stack will be invisible without a Texture Stack.\n\n" + NO_ASSETS_MSG, "Create Sprite Stack");
			else if(MessageBox.Show(this, "Generate & Load a Texture Stack?", "Create Sprite Stack", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				TryCreateTextureStack();
				texStackName = lastCreatedTexStackName;
			}

			var uid = Thing.CreateSpriteStack("Text", texStackName);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateCube()
		{
			var uid = Thing.CreateCube("Sprite", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}

		private void CreateUIButton()
		{
			var uid = Thing.UI.CreateButton("Button", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUITextbox()
		{
			var font = GetFont("Create Textbox", "Textbox");
			var camUID = GetInput("Create Textbox", "Provide the Textbox's Camera UID.", "Textbox-Camera", true);
			if(string.IsNullOrWhiteSpace(camUID))
				return;
			var camRes = GetVector2("Create Textbox", "Provide the Textbox's Camera resolution.", 1, 2048, 1, 2048, new(200));
			if(camRes == default)
				return;
			var uid = Thing.UI.CreateTextbox("Textbox", camUID, font, resolutionX: (uint)camRes.X, resolutionY: (uint)camRes.Y);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUITextButton()
		{
			var font = GetFont("Create TextButton", "TextButton");
			var textUID = GetInput("Create TextButton", "Provide the TextButton's Text UID.", "Text", true);
			if(string.IsNullOrWhiteSpace(textUID))
				return;
			var uid = Thing.UI.CreateTextButton("TextButton", textUID, null, font);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIInputbox()
		{
			var font = GetFont("Create Inputbox", "Inputbox");
			var camUID = GetInput("Create Inputbox", "Provide the Inputbox's Camera UID.", "Inputbox-Camera");
			if(string.IsNullOrWhiteSpace(camUID))
				return;
			var camRes = GetVector2("Create Inputbox", "Provide the Inputbox's Camera resolution.", 1, 2048, 1, 2048, new(200));
			if(camRes == default)
				return;
			var uid = Thing.UI.CreateInputbox("Inputbox", camUID, font, resolutionX: (uint)camRes.X, resolutionY: (uint)camRes.Y);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUICheckbox()
		{
			var uid = Thing.UI.CreateCheckbox("Checkbox", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIProgressBar()
		{
			var uid = Thing.UI.CreateProgressBar("Checkbox", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUISlider()
		{
			var uid = Thing.UI.CreateSlider("Checkbox", null);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIScrollBar()
		{
			var btnUIDs = GetUpDownButtonUIDs("ScrollBar");
			if(btnUIDs == null)
				return;
			var uid = Thing.UI.CreateScrollBar("Checkbox", null, btnUIDs[0], btnUIDs[1]);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIList()
		{
			var btnUIDs = GetUpDownButtonUIDs("List");
			if(btnUIDs == null)
				return;
			var uid = Thing.UI.CreateList("List", null, btnUIDs[0], btnUIDs[1]);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIListCarousel()
		{
			var btnUIDs = GetUpDownButtonUIDs("ListCarousel");
			if(btnUIDs == null)
				return;
			var uid = Thing.UI.CreateListCarousel("ListCarousel", null, btnUIDs[0], btnUIDs[1]);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIListMultiselect()
		{
			var btnUIDs = GetUpDownButtonUIDs("ListMultiselect");
			if(btnUIDs == null)
				return;
			var uid = Thing.UI.CreateListMultiselect("ListMultiselect", null, btnUIDs[0], btnUIDs[1]);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private void CreateUIListDropdown()
		{
			var btnUIDs = GetUpDownButtonUIDs("ListDropdown");
			if(btnUIDs == null)
				return;
			var showUID = GetInput("Create ListDropdown", $"Provide the ListDropdown's Show Button UID.", "ListDropdown-Show");
			if(string.IsNullOrWhiteSpace(showUID))
				return;
			var uid = Thing.UI.CreateListDropdown("ListDropdown", null, btnUIDs[0], btnUIDs[1], showUID);
			Thing.Set(uid, Thing.Property.POSITION, createPosition);
		}
		private string[]? GetUpDownButtonUIDs(string name)
		{
			var btnUID1 = GetInput($"Create {name}", $"Provide the {name}'s Up Button UID.", $"{name}-Up");
			if(string.IsNullOrWhiteSpace(btnUID1))
				return default;
			var btnUID2 = GetInput($"Create {name}", $"Provide the {name}'s Down Button UID.", $"{name}-Down");
			return string.IsNullOrWhiteSpace(btnUID2) ? default : (new string[] { btnUID1, btnUID2 });
		}

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
			TrySaveSceneAs();
		}
		private void OnSaveClick(object sender, EventArgs e)
		{
			if(paintTile != null)
				paintTile.Checked = false;
			if(editHitbox != null)
				editHitbox.Checked = false;

			TryResetThingPanel();
			TrySaveScene();
		}
		private void OnLoadClick(object sender, EventArgs e) => TryLoadScene();

		private void OnSceneScroll(object? sender, MouseEventArgs e)
		{
			var delta = e.Delta > 0 ? -0.05f : 0.05f;
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
					pos = pos.PointPercentTowardPoint(mousePos, new(5));
					SetViewPosition(pos);
				}
				SetViewScale(sceneSc + delta);
				return;
			}
			if(editingHitbox)
			{
				var hitbox = (Hitbox)Thing.Get(selectedUIDs[0], Thing.Property.HITBOX);

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
				var sc = (float)Thing.Get(uid, Thing.Property.SCALE);

				Thing.Set(uid, Thing.Property.SCALE, sc + delta);
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
					var hitbox = (Hitbox)Thing.Get(uid, Thing.Property.HITBOX);
					var sc = (float)Thing.Get(uid, Thing.Property.SCALE);

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
						var thingPos = (Vector2)Thing.Get(uid, Thing.Property.POSITION);
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
						var pos = (Vector2)Thing.Get(uid, Thing.Property.POSITION);
						var ang = (float)Thing.Get(uid, Thing.Property.ANGLE);

						if(editIndex == 0)
							Thing.Set(uid, Thing.Property.POSITION, Drag(pos, false, true));
						else if(editIndex == 1)
							Thing.Set(uid, Thing.Property.ANGLE, DragAngle(pos, ang));
					}

				Cursor.Current = editCursors[editIndex];
			}

			var gridSp = new Vector2((float)snap.Value);
			prevFormsMousePos = GetFormsMousePos();
			prevMousePos = GetMousePosition();
			prevFormsMousePosGrid = prevFormsMousePos.PointToGrid(gridSp) + gridSp * 0.5f;

			TryPaintTile();
		}
		private void OnMouseDownScene(object sender, MouseEventArgs e)
		{
			searchBox.Select();
			TryPaintTile();

			if(e.Button == MouseButtons.Right)
			{
				windowPicture.ContextMenuStrip = paintTile != null && paintTile.Checked ? null : sceneRightClickMenu;

				tilePaintRightClickPos = GetMousePosition();
				createPosition = GetMousePosition();
			}

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
			TryPaintTile();
			tilePaintRightClickPos = new Vector2().NaN();

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isDragSelecting = false;
		}

		private void OnCreateTextureStack(object sender, EventArgs e) => TryCreateTextureStack();
		private void OnUnloadTextureStack(object sender, EventArgs e) => TryUnloadTextureStack();

		private void OnSearchSceneFocus(object sender, EventArgs e) => isTyping = true;
		private void OnSearchSceneUnfocus(object sender, EventArgs e) => isTyping = false;
		#endregion
		#region SceneRightClick
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e) => CreateSprite();
		private void OnSceneRightClickMenuCreateText(object sender, EventArgs e) => CreateText();
		private void OnSceneRightClickMenuCreateNinePatch(object sender, EventArgs e) => CreateNinePatch();
		private void OnSceneRightClickMenuCreateLight(object sender, EventArgs e) => CreateLight();
		private void OnSceneRightClickMenuCreateCamera(object sender, EventArgs e) => CreateCamera();
		private void OnSceneRightClickMenuCreateTilemap(object sender, EventArgs e) => CreateTilemap();
		private void OnSceneRightClickMenuCreateAudio(object sender, EventArgs e) => CreateAudio();
		private void OnSceneRightClickMenuCreateCloth(object sender, EventArgs e) => CreateCloth();
		private void OnSceneRightClickMenuCreateSpriteStack(object sender, EventArgs e) => CreateSpriteStack();
		private void OnSceneRightClickMenuCreateCube(object sender, EventArgs e) => CreateCube();

		private void OnSceneRightClickMenuCreateUIButton(object sender, EventArgs e) => CreateUIButton();
		private void OnSceneRightClickMenuCreateUITextbox(object sender, EventArgs e) => CreateUITextbox();
		private void OnSceneRightClickMenuCreateUITextButton(object sender, EventArgs e) => CreateUITextButton();
		private void OnSceneRightClickMenuCreateUIInputbox(object sender, EventArgs e) => CreateUIInputbox();
		private void OnSceneRightClickMenuCreateUICheckbox(object sender, EventArgs e) => CreateUICheckbox();
		private void OnSceneRightClickMenuCreateUIProgressBar(object sender, EventArgs e) => CreateUIProgressBar();
		private void OnSceneRightClickMenuCreateUISlider(object sender, EventArgs e) => CreateUISlider();
		private void OnSceneRightClickMenuCreateUIScroll(object sender, EventArgs e) => CreateUIScrollBar();
		private void OnSceneRightClickMenuCreateUIList(object sender, EventArgs e) => CreateUIList();
		private void OnSceneRightClickMenuCreateUIListMultiselect(object sender, EventArgs e) => CreateUIListMultiselect();
		private void OnSceneRightClickMenuCreateUICarousel(object sender, EventArgs e) => CreateUIListCarousel();
		private void OnSceneRightClickMenuCreateUIDropdown(object sender, EventArgs e) => CreateUIListDropdown();

		private void OnSceneRightClickMenuCreateHitboxLine(object sender, EventArgs e) => AddLineToHitbox();
		private void OnSceneRightClickMenuCreateHitboxSquare(object sender, EventArgs e) => AddSquareToHitbox();
		private void OnSceneRightClickMenuCreateHitboxCircle(object sender, EventArgs e) => AddCircleToHitbox();

		private void OnSceneRightClickMenu(object sender, EventArgs e)
		{
			var sel = sceneRightClickMenu.Items.Find("sceneRightClickMenuSelection", false);
			if(sel != null && sel.Length != 0)
				sel[0].Enabled = selectedUIDs.Count > 0;

			var hitboxOption = sceneRightClickMenu.Items.Find("sceneRightClickMenuHitbox", false);
			if(hitboxOption != null && hitboxOption.Length != 0)
				hitboxOption[0].Enabled = editHitbox != null && editHitbox.Checked;
		}
		private void OnSceneRightClickMenuSelectionDuplicate(object sender, EventArgs e) => TryDuplicateSelection();
		private void OnSceneRightClickMenuSelectionDelete(object sender, EventArgs e)
		{
			TryDestroySelection();
		}
		private void OnSceneRightClickMenuSelectionDeselect(object sender, EventArgs e) => DeselectAll();
		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
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
			else if(list.Name == $"Prop{Thing.Property.TYPES}")
			{
				var tableID = $"table{list.SelectedItem}";
				if(editThingTableTypes.ContainsKey(tableID) == false) // implemented in engine but not in editor, prevent crash
				{
					TryResetThingPanel();
					return;
				}

				var table = editThingTableTypes[tableID];

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
				Thing.Set(selectedUIDs[0], Thing.Property.TEXT_STYLE, (Text.Styles)(index * 2));
			}
			else if(list.Name == $"Prop{Thing.Property.UI_TEXTBOX_ALIGNMENT}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.UI_TEXTBOX_ALIGNMENT, (SMPL.UI.Thing.TextboxAlignment)index);
			}
			else if(list.Name == $"Prop{Thing.Property.AUDIO_STATUS}")
			{
				var index = list.SelectedIndex;
				Thing.Set(selectedUIDs[0], Thing.Property.AUDIO_STATUS, (Thing.AudioStatus)index);
			}
			else if(list.Name == $"Prop{Thing.Property.TILEMAP_TILE_PALETTE}")
				tilePaletteUID = selectedItem;
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
			if(propName.Contains(nameof(Thing.CubeSide)))
				return;

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

			if(propName.Contains(nameof(Thing.CubeSide)))
				return;

			var uid = selectedUIDs[0];

			if(propName != Thing.Property.HITBOX)
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

			if(parName.Contains(nameof(Thing.CubeSide)))
				return;
			else if(parName.StartsWith("Prop") || parName.StartsWith("Scene")) // is vector or color
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
					case "PropSceneClothColor": cols["Cloth"] = SetColor(cols["Cloth"]); break;
					case "PropSceneSpriteStackColor": cols["SpriteStack"] = SetColor(cols["SpriteStack"]); break;
					case "PropSceneCubeColor": cols["Cube"] = SetColor(cols["Cube"]); break;
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
			else if(parName.StartsWith("PropTilePalette"))
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
			else if(parName.StartsWith("PropSpriteStack"))
			{
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
			if(pickThingResult != null)
				pickThingResult.Text = e.ClickedItem.Text;

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
		private void TryCreateTextureStack()
		{
			const int TABLE_HEIGHT = 230;
			var sz = new Vector2i(W + SPACING_X * 4, H + TABLE_HEIGHT);
			var window = new Form()
			{
				Width = sz.X,
				Height = sz.Y,
				FormBorderStyle = FormBorderStyle.FixedToolWindow,
				Text = "Generate Sprite Stack",
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White,
				StartPosition = FormStartPosition.CenterScreen
			};
			var table = new TableLayoutPanel
			{
				Top = SPACING_Y,
				Left = SPACING_X,
				Width = sz.X - SPACING_X * 2 - SPACING_X / 2,
				Height = TABLE_HEIGHT,
				ColumnCount = 2,
				RowCount = 6,
				Name = "PropSpriteStackCreate"
			};
			var button = new Button()
			{
				Text = "OK",
				Top = table.Height + SPACING_Y * 2,
				Left = sz.X - BUTTON_W - SPACING_X - SPACING_X / 4,
				Width = BUTTON_W,
				Height = BUTTON_H,
				DialogResult = DialogResult.OK
			};
			button.Click += (sender, e) => { window.Close(); };
			window.Controls.Add(table);
			window.Controls.Add(button);
			window.AcceptButton = button;

			for(int i = 0; i < 2; i++)
				table.ColumnStyles.Add(new(SizeType.Percent, 50));
			for(int i = 0; i < 6; i++)
				table.RowStyles.Add(new(SizeType.Percent, 50));

			AddThingProperty(table, "Obj Path", "SpriteStackObjPath", valueType: typeof(string));
			AddThingProperty(table, "Texture Path", "SpriteStackTexturePath", valueType: typeof(string));
			AddThingProperty(table, "Model Scale", "SpriteStackScale", valueType: typeof(Vector3));
			AddThingProperty(table, "Model Rotation", "SpriteStackRotation", valueType: typeof(Vector3));
			AddThingProperty(table, "Stack Count", "SpriteStackCount", valueType: typeof(int));
			AddThingProperty(table, "Stack Detail", "SpriteStackDetail", valueType: typeof(float));

			var obj = (TextBox)table.Controls[0];
			var tex = (TextBox)table.Controls[2];
			var scale = table.Controls[4];
			var rotation = table.Controls[6];
			var count = (NumericUpDown)table.Controls[8];
			var detail = (NumericUpDown)table.Controls[10];
			((NumericUpDown)scale.Controls[0]).Value = 1;
			((NumericUpDown)scale.Controls[1]).Value = 1;
			((NumericUpDown)scale.Controls[2]).Value = 1;
			count.Value = 20;
			detail.Value = 10;

			isSpriteStackCreationOpen = true;
			spriteStackCreateTexPath = tex;
			spriteStackCreateObjPath = obj;

			if(window.ShowDialog() != DialogResult.OK)
				return;

			var sc = new Vector3(
				(float)((NumericUpDown)scale.Controls[0]).Value,
				(float)((NumericUpDown)scale.Controls[1]).Value,
				(float)((NumericUpDown)scale.Controls[2]).Value);
			var rot = new Vector3(
				(float)((NumericUpDown)rotation.Controls[0]).Value,
				(float)((NumericUpDown)rotation.Controls[1]).Value,
				(float)((NumericUpDown)rotation.Controls[2]).Value);

			var name = Path.GetFileNameWithoutExtension(obj.Text);
			Scene.CurrentScene.LoadTextureStack(obj.Text, tex.Text, sc, rot, (int)count.Value, (float)detail.Value);
			isSpriteStackCreationOpen = false;

			lastCreatedTexStackName = name;
		}
		private void TryUnloadTextureStack()
		{
			var name = GetInput("Unload Texture Stack", "Provide the Texture Stack's name. The name comes from the .obj file upon generating a Texture Stack.");
			var count = GetNumber("Unload Texture Stack", "Provide the Texture Stack's count. The count was provided upon generating a Texture Stack.");

			for(int i = 0; i < (int)count; i++)
			{
				var id = $"{name}-{i}";
				Scene.CurrentScene.UnloadAssets(id);
			}
		}

		private void OnAssetRename(object sender, RenamedEventArgs e)
		{
			Scene.CurrentScene.UnloadAssets(GetMirrorAssetPath(e.OldFullPath));
			DeleteMirrorFiles(e.OldFullPath);
			CopyMirrorFiles(e.FullPath);
			Scene.CurrentScene.LoadAssets(GetMirrorAssetPath(e.FullPath));
		}
		private void OnAssetCreate(object sender, FileSystemEventArgs e)
		{
			Loading(LOADING_ASSETS);
			CopyMirrorFiles(e.FullPath);
			Scene.CurrentScene.LoadAssets(GetMirrorAssetPath(e.FullPath));
			Loading();
		}
		private void OnAssetDelete(object sender, FileSystemEventArgs e)
		{
			Loading(LOADING_ASSETS);
			Scene.CurrentScene.UnloadAssets(GetMirrorAssetPath(e.FullPath));
			DeleteMirrorFiles(e.FullPath);
			Loading();
		}

		private void OnAssetFolderRename(object sender, RenamedEventArgs e)
		{
			if(e.FullPath != GetAssetsPath()) // prevents renaming
				Directory.Move(e.FullPath, GetAssetsPath());
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
				TrySaveScene();

			selectedUIDs.Clear();
			Scene.CurrentScene.UnloadAssets();

			// skip that since some of the resources might still be in use somehow
			// cache is deleted the moment the app is started, no resources can be in use then
			// DeleteCache();
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
		private void Loading(string description = "")
		{
			loadingDescr = description;
			Application.DoEvents();
		}

		private string? GetFont(string title, string name)
		{
			var result = DialogResult.None;
			if(string.IsNullOrWhiteSpace(GetAssetsPath()))
				MessageBox.Show(this, $"This {name} will be invisible without a Font.\n\n" + NO_ASSETS_MSG, title);
			else if(MessageBox.Show(this, "Pick a Font?", title, MessageBoxButtons.YesNo) == DialogResult.Yes)
				result = pickAsset.ShowDialog();

			return result != DialogResult.OK ? null : GetMirrorAssetPath(pickAsset.FileName);
		}
		private string GetInput(string title, string text, string defaultInput = "", bool isThingUID = false)
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
				Width = sz.X - BUTTON_W - SPACING_X * 3 - (isThingUID ? BUTTON_W : 0),
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
			var things = new Button()
			{
				Text = "Things",
				Left = sz.X - BUTTON_W * 2 - SPACING_X * 2,
				Width = BUTTON_W,
				Height = BUTTON_H,
				Top = SPACING_Y + textLabel.Height
			};
			things.Click += (sender, e) => PickThing(things, textBox);
			button.Click += (sender, e) => window.Close();
			window.Controls.Add(textBox);
			window.Controls.Add(button);
			window.Controls.Add(textLabel);
			if(isThingUID)
				window.Controls.Add(things);
			window.AcceptButton = button;

			return window.ShowDialog() == DialogResult.OK ? textBox.Text : "";
		}
		private static float GetNumber(string title, string text, float defaultValue = 0f)
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
			var numeric = new NumericUpDown()
			{
				Left = SPACING_X,
				Top = SPACING_Y + textLabel.Height,
				Width = sz.X - BUTTON_W - SPACING_X * 3,
				Height = TEXTBOX_H,
				Value = (decimal)defaultValue,
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
			window.Controls.Add(numeric);
			window.Controls.Add(button);
			window.Controls.Add(textLabel);
			window.AcceptButton = button;

			return window.ShowDialog() == DialogResult.OK ? (float)numeric.Value : float.NaN;
		}
		private static Vector2 GetVector2(string title, string text, float minX, float maxX, float minY, float maxY, Vector2 placeholder = default)
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
			var numberXLabel = new Label()
			{
				Left = SPACING_X,
				Top = SPACING_Y + textLabel.Height + TEXTBOX_H / 8,
				Text = "X:",
				Width = 20,
				Height = TEXTBOX_H
			};
			var numberX = new NumericUpDown()
			{
				Left = SPACING_X + numberXLabel.Width,
				Top = SPACING_Y + textLabel.Height,
				Width = 50,
				Height = TEXTBOX_H,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White,
				Minimum = (decimal)minX,
				Maximum = (decimal)maxX,
				Value = (decimal)placeholder.X
			};
			var numberYLabel = new Label()
			{
				Left = SPACING_X + numberXLabel.Width + numberX.Width + SPACING_X / 2,
				Top = SPACING_Y + textLabel.Height + TEXTBOX_H / 8,
				Text = "Y:",
				Width = 20,
				Height = TEXTBOX_H
			};
			var numberY = new NumericUpDown()
			{
				Left = SPACING_X + numberXLabel.Width + numberX.Width + SPACING_X / 2 + numberYLabel.Width,
				Top = SPACING_Y + textLabel.Height,
				Width = 50,
				Height = TEXTBOX_H,
				BackColor = System.Drawing.Color.Black,
				ForeColor = System.Drawing.Color.White,
				Minimum = (decimal)minY,
				Maximum = (decimal)maxY,
				Value = (decimal)placeholder.Y
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
			window.Controls.Add(numberX);
			window.Controls.Add(numberY);
			window.Controls.Add(button);
			window.Controls.Add(textLabel);
			window.Controls.Add(numberXLabel);
			window.Controls.Add(numberYLabel);
			window.AcceptButton = button;

			return window.ShowDialog() != DialogResult.OK ? default : (new((float)numberX.Value, (float)numberY.Value));
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
					PickThing(buttonThings, textBox);
				};
				window.Controls.Add(buttonThings);
			}

			if(window.ShowDialog() != DialogResult.OK)
				return;

			list.Clear();
			foreach(string item in listBox.Items)
				list.Add(item);
		}
		private void PickThing(Control button, Control? result, string property = "")
		{
			var uids = Thing.GetUIDs();

			thingsList.MaximumSize = new(thingsList.MaximumSize.Width, 300);
			thingsList.Items.Clear();
			for(int i = 0; i < uids.Count; i++)
				if(selectedUIDs.Count == 0 || selectedUIDs[0] != uids[i])
					thingsList.Items.Add(uids[i]);

			pickThingProperty = property;
			pickThingResult = result;
			// show twice cuz first time has wrong position
			Show();
			Show();

			void Show() => thingsList.Show(this, button.PointToScreen(new(-thingsList.Width / 4, 10)));
		}
		#endregion
	}
}