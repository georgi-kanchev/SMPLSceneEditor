namespace SMPLSceneEditor
{
	partial class FormWindow
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.windowSplit = new System.Windows.Forms.SplitContainer();
			this.loading = new System.Windows.Forms.Label();
			this.sceneValues = new System.Windows.Forms.Label();
			this.sceneStatusPanel = new System.Windows.Forms.Panel();
			this.searchBox = new System.Windows.Forms.GroupBox();
			this.searchScene = new System.Windows.Forms.TextBox();
			this.editSelectionGroup = new System.Windows.Forms.GroupBox();
			this.editSelectionOptions = new System.Windows.Forms.ListBox();
			this.snap = new System.Windows.Forms.NumericUpDown();
			this.gridGroup = new System.Windows.Forms.GroupBox();
			this.gridSpacing = new System.Windows.Forms.NumericUpDown();
			this.gridThickness = new System.Windows.Forms.TrackBar();
			this.windowPicture = new System.Windows.Forms.PictureBox();
			this.sceneRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSprite = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateText = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateTileMap = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateNinePatch = new System.Windows.Forms.ToolStripMenuItem();
			this.uIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.uBButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuCreateCloth = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSpriteStack = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateCube = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuCreateAudio = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateLight = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateCamera = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuHitbox = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftLLine2PointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftSSquare4PointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftCCircle8PointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSelection = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSelectionDuplicate = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSelectionDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSelectionDeselect = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuScene = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSceneSave = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSceneSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSceneLoad = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSceneSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuSceneResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.textureStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.generateLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rightTable = new System.Windows.Forms.TableLayoutPanel();
			this.thingTypesTable = new System.Windows.Forms.TableLayoutPanel();
			this.load = new System.Windows.Forms.OpenFileDialog();
			this.pickColor = new System.Windows.Forms.ColorDialog();
			this.pickAsset = new System.Windows.Forms.OpenFileDialog();
			this.thingsList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.save = new System.Windows.Forms.SaveFileDialog();
			this.uTTextboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).BeginInit();
			this.windowSplit.Panel1.SuspendLayout();
			this.windowSplit.Panel2.SuspendLayout();
			this.windowSplit.SuspendLayout();
			this.sceneStatusPanel.SuspendLayout();
			this.searchBox.SuspendLayout();
			this.editSelectionGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.snap)).BeginInit();
			this.gridGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridSpacing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.windowPicture)).BeginInit();
			this.sceneRightClickMenu.SuspendLayout();
			this.rightTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// windowSplit
			// 
			this.windowSplit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.windowSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.windowSplit.Location = new System.Drawing.Point(0, 0);
			this.windowSplit.Name = "windowSplit";
			// 
			// windowSplit.Panel1
			// 
			this.windowSplit.Panel1.Controls.Add(this.loading);
			this.windowSplit.Panel1.Controls.Add(this.sceneValues);
			this.windowSplit.Panel1.Controls.Add(this.sceneStatusPanel);
			this.windowSplit.Panel1.Controls.Add(this.windowPicture);
			// 
			// windowSplit.Panel2
			// 
			this.windowSplit.Panel2.Controls.Add(this.rightTable);
			this.windowSplit.Size = new System.Drawing.Size(1564, 761);
			this.windowSplit.SplitterDistance = 1205;
			this.windowSplit.TabIndex = 0;
			this.windowSplit.TabStop = false;
			// 
			// loading
			// 
			this.loading.BackColor = System.Drawing.Color.Transparent;
			this.loading.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loading.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.loading.ForeColor = System.Drawing.Color.White;
			this.loading.Location = new System.Drawing.Point(0, 58);
			this.loading.Name = "loading";
			this.loading.Size = new System.Drawing.Size(1201, 699);
			this.loading.TabIndex = 6;
			this.loading.Text = "Loading...";
			this.loading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.loading.Visible = false;
			// 
			// sceneValues
			// 
			this.sceneValues.AutoSize = true;
			this.sceneValues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.sceneValues.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.sceneValues.ForeColor = System.Drawing.Color.White;
			this.sceneValues.Location = new System.Drawing.Point(0, 61);
			this.sceneValues.Name = "sceneValues";
			this.sceneValues.Size = new System.Drawing.Size(0, 21);
			this.sceneValues.TabIndex = 4;
			// 
			// sceneStatusPanel
			// 
			this.sceneStatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sceneStatusPanel.Controls.Add(this.searchBox);
			this.sceneStatusPanel.Controls.Add(this.editSelectionGroup);
			this.sceneStatusPanel.Controls.Add(this.gridGroup);
			this.sceneStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneStatusPanel.Location = new System.Drawing.Point(0, 0);
			this.sceneStatusPanel.Name = "sceneStatusPanel";
			this.sceneStatusPanel.Size = new System.Drawing.Size(1201, 58);
			this.sceneStatusPanel.TabIndex = 2;
			// 
			// searchBox
			// 
			this.searchBox.Controls.Add(this.searchScene);
			this.searchBox.Dock = System.Windows.Forms.DockStyle.Right;
			this.searchBox.ForeColor = System.Drawing.Color.White;
			this.searchBox.Location = new System.Drawing.Point(1055, 0);
			this.searchBox.Name = "searchBox";
			this.searchBox.Size = new System.Drawing.Size(142, 54);
			this.searchBox.TabIndex = 9;
			this.searchBox.TabStop = false;
			this.searchBox.Text = "Search";
			// 
			// searchScene
			// 
			this.searchScene.BackColor = System.Drawing.Color.Black;
			this.searchScene.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchScene.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.searchScene.ForeColor = System.Drawing.Color.White;
			this.searchScene.Location = new System.Drawing.Point(3, 19);
			this.searchScene.Name = "searchScene";
			this.searchScene.PlaceholderText = "UID";
			this.searchScene.Size = new System.Drawing.Size(136, 29);
			this.searchScene.TabIndex = 6;
			this.searchScene.TabStop = false;
			this.searchScene.Enter += new System.EventHandler(this.OnSearchSceneFocus);
			this.searchScene.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownObjectSearch);
			this.searchScene.Leave += new System.EventHandler(this.OnSearchSceneUnfocus);
			// 
			// editSelectionGroup
			// 
			this.editSelectionGroup.Controls.Add(this.editSelectionOptions);
			this.editSelectionGroup.Controls.Add(this.snap);
			this.editSelectionGroup.Dock = System.Windows.Forms.DockStyle.Left;
			this.editSelectionGroup.ForeColor = System.Drawing.Color.White;
			this.editSelectionGroup.Location = new System.Drawing.Point(168, 0);
			this.editSelectionGroup.Name = "editSelectionGroup";
			this.editSelectionGroup.Size = new System.Drawing.Size(186, 54);
			this.editSelectionGroup.TabIndex = 6;
			this.editSelectionGroup.TabStop = false;
			this.editSelectionGroup.Text = "Edit Selection";
			// 
			// editSelectionOptions
			// 
			this.editSelectionOptions.BackColor = System.Drawing.Color.Black;
			this.editSelectionOptions.Dock = System.Windows.Forms.DockStyle.Left;
			this.editSelectionOptions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.editSelectionOptions.ForeColor = System.Drawing.Color.White;
			this.editSelectionOptions.FormattingEnabled = true;
			this.editSelectionOptions.Items.AddRange(new object[] {
            "         Move",
            "          Spin"});
			this.editSelectionOptions.Location = new System.Drawing.Point(3, 19);
			this.editSelectionOptions.Name = "editSelectionOptions";
			this.editSelectionOptions.Size = new System.Drawing.Size(91, 32);
			this.editSelectionOptions.TabIndex = 3;
			this.editSelectionOptions.TabStop = false;
			// 
			// snap
			// 
			this.snap.BackColor = System.Drawing.Color.Black;
			this.snap.DecimalPlaces = 2;
			this.snap.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.snap.ForeColor = System.Drawing.Color.White;
			this.snap.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.snap.Location = new System.Drawing.Point(100, 19);
			this.snap.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.snap.Name = "snap";
			this.snap.Size = new System.Drawing.Size(82, 27);
			this.snap.TabIndex = 5;
			this.snap.TabStop = false;
			this.snap.Enter += new System.EventHandler(this.OnSearchSceneFocus);
			this.snap.Leave += new System.EventHandler(this.OnSearchSceneUnfocus);
			// 
			// gridGroup
			// 
			this.gridGroup.Controls.Add(this.gridSpacing);
			this.gridGroup.Controls.Add(this.gridThickness);
			this.gridGroup.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridGroup.ForeColor = System.Drawing.Color.White;
			this.gridGroup.Location = new System.Drawing.Point(0, 0);
			this.gridGroup.Name = "gridGroup";
			this.gridGroup.Size = new System.Drawing.Size(168, 54);
			this.gridGroup.TabIndex = 0;
			this.gridGroup.TabStop = false;
			this.gridGroup.Text = "Grid";
			// 
			// gridSpacing
			// 
			this.gridSpacing.BackColor = System.Drawing.Color.Black;
			this.gridSpacing.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.gridSpacing.ForeColor = System.Drawing.Color.White;
			this.gridSpacing.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.gridSpacing.Location = new System.Drawing.Point(110, 18);
			this.gridSpacing.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.gridSpacing.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.gridSpacing.Name = "gridSpacing";
			this.gridSpacing.ReadOnly = true;
			this.gridSpacing.Size = new System.Drawing.Size(49, 27);
			this.gridSpacing.TabIndex = 6;
			this.gridSpacing.TabStop = false;
			this.gridSpacing.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.gridSpacing.Enter += new System.EventHandler(this.OnSearchSceneFocus);
			this.gridSpacing.Leave += new System.EventHandler(this.OnSearchSceneUnfocus);
			// 
			// gridThickness
			// 
			this.gridThickness.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.gridThickness.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridThickness.LargeChange = 1;
			this.gridThickness.Location = new System.Drawing.Point(3, 19);
			this.gridThickness.Name = "gridThickness";
			this.gridThickness.Size = new System.Drawing.Size(104, 32);
			this.gridThickness.TabIndex = 0;
			this.gridThickness.TabStop = false;
			this.gridThickness.Value = 2;
			// 
			// windowPicture
			// 
			this.windowPicture.BackColor = System.Drawing.Color.Black;
			this.windowPicture.ContextMenuStrip = this.sceneRightClickMenu;
			this.windowPicture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.windowPicture.Location = new System.Drawing.Point(0, 0);
			this.windowPicture.Name = "windowPicture";
			this.windowPicture.Size = new System.Drawing.Size(1201, 757);
			this.windowPicture.TabIndex = 5;
			this.windowPicture.TabStop = false;
			this.windowPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDownScene);
			this.windowPicture.MouseEnter += new System.EventHandler(this.OnMouseEnterScene);
			this.windowPicture.MouseLeave += new System.EventHandler(this.OnMouseLeaveScene);
			this.windowPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveScene);
			this.windowPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpScene);
			// 
			// sceneRightClickMenu
			// 
			this.sceneRightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuCreate,
            this.sceneRightClickMenuHitbox,
            this.sceneRightClickMenuSelection,
            this.sceneRightClickMenuScene,
            this.textureStackToolStripMenuItem});
			this.sceneRightClickMenu.Name = "sceneRightClickMenu";
			this.sceneRightClickMenu.Size = new System.Drawing.Size(181, 136);
			this.sceneRightClickMenu.Opened += new System.EventHandler(this.OnSceneRightClickMenu);
			// 
			// sceneRightClickMenuCreate
			// 
			this.sceneRightClickMenuCreate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuCreateSprite,
            this.sceneRightClickMenuCreateText,
            this.sceneRightClickMenuCreateTileMap,
            this.sceneRightClickMenuCreateNinePatch,
            this.uIToolStripMenuItem,
            this.sceneRightClickMenuCreateSeparator1,
            this.sceneRightClickMenuCreateCloth,
            this.sceneRightClickMenuCreateSpriteStack,
            this.sceneRightClickMenuCreateCube,
            this.sceneRightClickMenuCreateSeparator2,
            this.sceneRightClickMenuCreateAudio,
            this.sceneRightClickMenuCreateLight,
            this.sceneRightClickMenuCreateCamera});
			this.sceneRightClickMenuCreate.Name = "sceneRightClickMenuCreate";
			this.sceneRightClickMenuCreate.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreate.Text = "Create";
			// 
			// sceneRightClickMenuCreateSprite
			// 
			this.sceneRightClickMenuCreateSprite.Name = "sceneRightClickMenuCreateSprite";
			this.sceneRightClickMenuCreateSprite.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateSprite.Text = "(C+S) Sprite";
			this.sceneRightClickMenuCreateSprite.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateSprite);
			// 
			// sceneRightClickMenuCreateText
			// 
			this.sceneRightClickMenuCreateText.Name = "sceneRightClickMenuCreateText";
			this.sceneRightClickMenuCreateText.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateText.Text = "(C+T) Text";
			this.sceneRightClickMenuCreateText.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateText);
			// 
			// sceneRightClickMenuCreateTileMap
			// 
			this.sceneRightClickMenuCreateTileMap.Name = "sceneRightClickMenuCreateTileMap";
			this.sceneRightClickMenuCreateTileMap.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateTileMap.Text = "(C+M) Tilemap";
			this.sceneRightClickMenuCreateTileMap.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateTilemap);
			// 
			// sceneRightClickMenuCreateNinePatch
			// 
			this.sceneRightClickMenuCreateNinePatch.Name = "sceneRightClickMenuCreateNinePatch";
			this.sceneRightClickMenuCreateNinePatch.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateNinePatch.Text = "(C+N) Nine Patch";
			this.sceneRightClickMenuCreateNinePatch.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateNinePatch);
			// 
			// uIToolStripMenuItem
			// 
			this.uIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uBButtonToolStripMenuItem,
            this.uTTextboxToolStripMenuItem});
			this.uIToolStripMenuItem.Name = "uIToolStripMenuItem";
			this.uIToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.uIToolStripMenuItem.Text = "UI";
			// 
			// uBButtonToolStripMenuItem
			// 
			this.uBButtonToolStripMenuItem.Name = "uBButtonToolStripMenuItem";
			this.uBButtonToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.uBButtonToolStripMenuItem.Text = "(U+B) Button";
			this.uBButtonToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateUIButton);
			// 
			// sceneRightClickMenuCreateSeparator1
			// 
			this.sceneRightClickMenuCreateSeparator1.Name = "sceneRightClickMenuCreateSeparator1";
			this.sceneRightClickMenuCreateSeparator1.Size = new System.Drawing.Size(177, 6);
			// 
			// sceneRightClickMenuCreateCloth
			// 
			this.sceneRightClickMenuCreateCloth.Name = "sceneRightClickMenuCreateCloth";
			this.sceneRightClickMenuCreateCloth.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateCloth.Text = "(C+H) Cloth";
			this.sceneRightClickMenuCreateCloth.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateCloth);
			// 
			// sceneRightClickMenuCreateSpriteStack
			// 
			this.sceneRightClickMenuCreateSpriteStack.Name = "sceneRightClickMenuCreateSpriteStack";
			this.sceneRightClickMenuCreateSpriteStack.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateSpriteStack.Text = "(C+P) Sprite Stack";
			this.sceneRightClickMenuCreateSpriteStack.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateSpriteStack);
			// 
			// sceneRightClickMenuCreateCube
			// 
			this.sceneRightClickMenuCreateCube.Name = "sceneRightClickMenuCreateCube";
			this.sceneRightClickMenuCreateCube.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateCube.Text = "(C+Q) Cube";
			this.sceneRightClickMenuCreateCube.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateCube);
			// 
			// sceneRightClickMenuCreateSeparator2
			// 
			this.sceneRightClickMenuCreateSeparator2.Name = "sceneRightClickMenuCreateSeparator2";
			this.sceneRightClickMenuCreateSeparator2.Size = new System.Drawing.Size(177, 6);
			// 
			// sceneRightClickMenuCreateAudio
			// 
			this.sceneRightClickMenuCreateAudio.Name = "sceneRightClickMenuCreateAudio";
			this.sceneRightClickMenuCreateAudio.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateAudio.Text = "(C+A) Audio";
			this.sceneRightClickMenuCreateAudio.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateAudio);
			// 
			// sceneRightClickMenuCreateLight
			// 
			this.sceneRightClickMenuCreateLight.Name = "sceneRightClickMenuCreateLight";
			this.sceneRightClickMenuCreateLight.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateLight.Text = "(C+I) Light";
			this.sceneRightClickMenuCreateLight.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateLight);
			// 
			// sceneRightClickMenuCreateCamera
			// 
			this.sceneRightClickMenuCreateCamera.Name = "sceneRightClickMenuCreateCamera";
			this.sceneRightClickMenuCreateCamera.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuCreateCamera.Text = "(C+R) Camera";
			this.sceneRightClickMenuCreateCamera.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateCamera);
			// 
			// sceneRightClickMenuHitbox
			// 
			this.sceneRightClickMenuHitbox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shiftLLine2PointsToolStripMenuItem,
            this.shiftSSquare4PointsToolStripMenuItem,
            this.shiftCCircle8PointsToolStripMenuItem});
			this.sceneRightClickMenuHitbox.Name = "sceneRightClickMenuHitbox";
			this.sceneRightClickMenuHitbox.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuHitbox.Text = "Add to Hitbox";
			// 
			// shiftLLine2PointsToolStripMenuItem
			// 
			this.shiftLLine2PointsToolStripMenuItem.Name = "shiftLLine2PointsToolStripMenuItem";
			this.shiftLLine2PointsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.shiftLLine2PointsToolStripMenuItem.Text = "(H+L) Line (2 points)";
			this.shiftLLine2PointsToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateHitboxLine);
			// 
			// shiftSSquare4PointsToolStripMenuItem
			// 
			this.shiftSSquare4PointsToolStripMenuItem.Name = "shiftSSquare4PointsToolStripMenuItem";
			this.shiftSSquare4PointsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.shiftSSquare4PointsToolStripMenuItem.Text = "(H+S) Square (4 points)";
			this.shiftSSquare4PointsToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateHitboxSquare);
			// 
			// shiftCCircle8PointsToolStripMenuItem
			// 
			this.shiftCCircle8PointsToolStripMenuItem.Name = "shiftCCircle8PointsToolStripMenuItem";
			this.shiftCCircle8PointsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.shiftCCircle8PointsToolStripMenuItem.Text = "(H+C) Circle (8 points)";
			this.shiftCCircle8PointsToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateHitboxCircle);
			// 
			// sceneRightClickMenuSelection
			// 
			this.sceneRightClickMenuSelection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuSelectionDuplicate,
            this.sceneRightClickMenuSelectionDelete,
            this.sceneRightClickMenuSelectionDeselect});
			this.sceneRightClickMenuSelection.Name = "sceneRightClickMenuSelection";
			this.sceneRightClickMenuSelection.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuSelection.Text = "Selection";
			// 
			// sceneRightClickMenuSelectionDuplicate
			// 
			this.sceneRightClickMenuSelectionDuplicate.Name = "sceneRightClickMenuSelectionDuplicate";
			this.sceneRightClickMenuSelectionDuplicate.Size = new System.Drawing.Size(170, 22);
			this.sceneRightClickMenuSelectionDuplicate.Text = "(Ctrl+C) Duplicate";
			this.sceneRightClickMenuSelectionDuplicate.Click += new System.EventHandler(this.OnSceneRightClickMenuSelectionDuplicate);
			// 
			// sceneRightClickMenuSelectionDelete
			// 
			this.sceneRightClickMenuSelectionDelete.Name = "sceneRightClickMenuSelectionDelete";
			this.sceneRightClickMenuSelectionDelete.Size = new System.Drawing.Size(170, 22);
			this.sceneRightClickMenuSelectionDelete.Text = "(Delete) Delete";
			this.sceneRightClickMenuSelectionDelete.Click += new System.EventHandler(this.OnSceneRightClickMenuSelectionDelete);
			// 
			// sceneRightClickMenuSelectionDeselect
			// 
			this.sceneRightClickMenuSelectionDeselect.Name = "sceneRightClickMenuSelectionDeselect";
			this.sceneRightClickMenuSelectionDeselect.Size = new System.Drawing.Size(170, 22);
			this.sceneRightClickMenuSelectionDeselect.Text = "(Ctrl+D) Deselect";
			this.sceneRightClickMenuSelectionDeselect.Click += new System.EventHandler(this.OnSceneRightClickMenuSelectionDeselect);
			// 
			// sceneRightClickMenuScene
			// 
			this.sceneRightClickMenuScene.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuSceneSave,
            this.sceneRightClickMenuSceneSaveAs,
            this.sceneRightClickMenuSceneLoad,
            this.sceneRightClickMenuSceneSeparator1,
            this.sceneRightClickMenuSceneResetView});
			this.sceneRightClickMenuScene.Name = "sceneRightClickMenuScene";
			this.sceneRightClickMenuScene.Size = new System.Drawing.Size(180, 22);
			this.sceneRightClickMenuScene.Text = "Scene";
			// 
			// sceneRightClickMenuSceneSave
			// 
			this.sceneRightClickMenuSceneSave.Name = "sceneRightClickMenuSceneSave";
			this.sceneRightClickMenuSceneSave.Size = new System.Drawing.Size(188, 22);
			this.sceneRightClickMenuSceneSave.Text = "(Ctrl+S) Save";
			this.sceneRightClickMenuSceneSave.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// sceneRightClickMenuSceneSaveAs
			// 
			this.sceneRightClickMenuSceneSaveAs.Name = "sceneRightClickMenuSceneSaveAs";
			this.sceneRightClickMenuSceneSaveAs.Size = new System.Drawing.Size(188, 22);
			this.sceneRightClickMenuSceneSaveAs.Text = "(Ctrl+N) Save As New";
			this.sceneRightClickMenuSceneSaveAs.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// sceneRightClickMenuSceneLoad
			// 
			this.sceneRightClickMenuSceneLoad.Name = "sceneRightClickMenuSceneLoad";
			this.sceneRightClickMenuSceneLoad.Size = new System.Drawing.Size(188, 22);
			this.sceneRightClickMenuSceneLoad.Text = "(Ctrl+L) Load";
			this.sceneRightClickMenuSceneLoad.Click += new System.EventHandler(this.OnLoadClick);
			// 
			// sceneRightClickMenuSceneSeparator1
			// 
			this.sceneRightClickMenuSceneSeparator1.Name = "sceneRightClickMenuSceneSeparator1";
			this.sceneRightClickMenuSceneSeparator1.Size = new System.Drawing.Size(185, 6);
			// 
			// sceneRightClickMenuSceneResetView
			// 
			this.sceneRightClickMenuSceneResetView.Name = "sceneRightClickMenuSceneResetView";
			this.sceneRightClickMenuSceneResetView.Size = new System.Drawing.Size(188, 22);
			this.sceneRightClickMenuSceneResetView.Text = "(Space) Reset View";
			this.sceneRightClickMenuSceneResetView.Click += new System.EventHandler(this.OnSceneRightClickMenuResetView);
			// 
			// textureStackToolStripMenuItem
			// 
			this.textureStackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateLoadToolStripMenuItem,
            this.unloadToolStripMenuItem});
			this.textureStackToolStripMenuItem.Name = "textureStackToolStripMenuItem";
			this.textureStackToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.textureStackToolStripMenuItem.Text = "Texture Stack";
			// 
			// generateLoadToolStripMenuItem
			// 
			this.generateLoadToolStripMenuItem.Name = "generateLoadToolStripMenuItem";
			this.generateLoadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.generateLoadToolStripMenuItem.Text = "(Ctrl+P) Generate";
			this.generateLoadToolStripMenuItem.Click += new System.EventHandler(this.OnCreateTextureStack);
			// 
			// unloadToolStripMenuItem
			// 
			this.unloadToolStripMenuItem.Name = "unloadToolStripMenuItem";
			this.unloadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.unloadToolStripMenuItem.Text = "(Ctrl+Alt+P) Unload";
			this.unloadToolStripMenuItem.Click += new System.EventHandler(this.OnUnloadTextureStack);
			// 
			// rightTable
			// 
			this.rightTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
			this.rightTable.ColumnCount = 1;
			this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.rightTable.Controls.Add(this.thingTypesTable, 0, 0);
			this.rightTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rightTable.Location = new System.Drawing.Point(0, 0);
			this.rightTable.Name = "rightTable";
			this.rightTable.RowCount = 2;
			this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.123713F));
			this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95.87629F));
			this.rightTable.Size = new System.Drawing.Size(351, 757);
			this.rightTable.TabIndex = 1;
			// 
			// thingTypesTable
			// 
			this.thingTypesTable.ColumnCount = 2;
			this.thingTypesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.thingTypesTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.thingTypesTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.thingTypesTable.Location = new System.Drawing.Point(6, 6);
			this.thingTypesTable.Name = "thingTypesTable";
			this.thingTypesTable.RowCount = 1;
			this.thingTypesTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.thingTypesTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.thingTypesTable.Size = new System.Drawing.Size(339, 24);
			this.thingTypesTable.TabIndex = 1;
			// 
			// load
			// 
			this.load.Filter = "Scene|*.scene";
			this.load.Title = "Load Scene";
			// 
			// pickColor
			// 
			this.pickColor.Color = System.Drawing.Color.White;
			this.pickColor.FullOpen = true;
			// 
			// pickAsset
			// 
			this.pickAsset.Title = "Pick Asset";
			// 
			// thingsList
			// 
			this.thingsList.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
			this.thingsList.Name = "contextMenuStrip1";
			this.thingsList.Size = new System.Drawing.Size(61, 4);
			this.thingsList.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.OnThingListPick);
			// 
			// save
			// 
			this.save.DefaultExt = "scene";
			this.save.Filter = "Scene|*.scene";
			this.save.Title = "Save Scene";
			// 
			// uTTextboxToolStripMenuItem
			// 
			this.uTTextboxToolStripMenuItem.Name = "uTTextboxToolStripMenuItem";
			this.uTTextboxToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.uTTextboxToolStripMenuItem.Text = "(U+T) Textbox";
			this.uTTextboxToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateUITextbox);
			// 
			// FormWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(1564, 761);
			this.Controls.Add(this.windowSplit);
			this.Name = "FormWindow";
			this.Text = "SMPL Scene Editor";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDownScene);
			this.MouseEnter += new System.EventHandler(this.OnMouseEnterScene);
			this.MouseLeave += new System.EventHandler(this.OnMouseLeaveScene);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveScene);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpScene);
			this.windowSplit.Panel1.ResumeLayout(false);
			this.windowSplit.Panel1.PerformLayout();
			this.windowSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).EndInit();
			this.windowSplit.ResumeLayout(false);
			this.sceneStatusPanel.ResumeLayout(false);
			this.searchBox.ResumeLayout(false);
			this.searchBox.PerformLayout();
			this.editSelectionGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.snap)).EndInit();
			this.gridGroup.ResumeLayout(false);
			this.gridGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridSpacing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.windowPicture)).EndInit();
			this.sceneRightClickMenu.ResumeLayout(false);
			this.rightTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer windowSplit;
		private Panel sceneStatusPanel;
		private GroupBox gridGroup;
		private TrackBar gridThickness;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
		private Label sceneValues;
		private ListBox editSelectionOptions;
		private NumericUpDown snap;
		private NumericUpDown gridSpacing;
		private PictureBox windowPicture;
		private GroupBox editSelectionGroup;
		private OpenFileDialog load;
		private GroupBox searchBox;
		private TableLayoutPanel rightTable;
		private TableLayoutPanel thingTypesTable;
		private TextBox searchScene;
		private ToolStripMenuItem sceneRightClickMenuCreateLight;
		private ToolStripMenuItem sceneRightClickMenuCreateCamera;
		private ColorDialog pickColor;
		private OpenFileDialog pickAsset;
		private ContextMenuStrip thingsList;
		private ToolStripMenuItem sceneRightClickMenuCreateText;
		private ToolStripSeparator sceneRightClickMenuCreateSeparator1;
		private ToolStripMenuItem sceneRightClickMenuCreateNinePatch;
		private Label loading;
		private ToolStripSeparator sceneRightClickMenuCreateSeparator2;
		private ToolStripMenuItem sceneRightClickMenuCreateAudio;
		private ToolStripMenuItem sceneRightClickMenuCreateTileMap;
		private SaveFileDialog save;
		private ToolStripMenuItem sceneRightClickMenuCreateCloth;
		private ToolStripMenuItem sceneRightClickMenuCreateSpriteStack;
		private ToolStripMenuItem sceneRightClickMenuCreateCube;
		private ToolStripMenuItem sceneRightClickMenuSelection;
		private ToolStripMenuItem sceneRightClickMenuSelectionDuplicate;
		private ToolStripMenuItem sceneRightClickMenuSelectionDelete;
		private ToolStripMenuItem sceneRightClickMenuSelectionDeselect;
		private ToolStripMenuItem sceneRightClickMenuScene;
		private ToolStripMenuItem sceneRightClickMenuSceneSave;
		private ToolStripMenuItem sceneRightClickMenuSceneSaveAs;
		private ToolStripMenuItem sceneRightClickMenuSceneLoad;
		private ToolStripSeparator sceneRightClickMenuSceneSeparator1;
		private ToolStripMenuItem sceneRightClickMenuSceneResetView;
		private ToolStripMenuItem textureStackToolStripMenuItem;
		private ToolStripMenuItem generateLoadToolStripMenuItem;
		private ToolStripMenuItem unloadToolStripMenuItem;
		private ToolStripMenuItem sceneRightClickMenuHitbox;
		private ToolStripMenuItem shiftLLine2PointsToolStripMenuItem;
		private ToolStripMenuItem shiftSSquare4PointsToolStripMenuItem;
		private ToolStripMenuItem shiftCCircle8PointsToolStripMenuItem;
		private ToolStripMenuItem uIToolStripMenuItem;
		private ToolStripMenuItem uBButtonToolStripMenuItem;
		private ToolStripMenuItem uTTextboxToolStripMenuItem;
	}
}