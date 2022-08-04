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
			this.sceneGroup = new System.Windows.Forms.GroupBox();
			this.saveAsButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.loadButton = new System.Windows.Forms.Button();
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
			this.sceneRightClickMenuCreateNinePatch = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuCreateTileMap = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateLight = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateCamera = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuCreateAudio = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuUnselectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.rightTable = new System.Windows.Forms.TableLayoutPanel();
			this.thingTypesTable = new System.Windows.Forms.TableLayoutPanel();
			this.load = new System.Windows.Forms.OpenFileDialog();
			this.pickColor = new System.Windows.Forms.ColorDialog();
			this.pickAsset = new System.Windows.Forms.OpenFileDialog();
			this.thingsList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuHitbox = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuHitboxCreateLine = new System.Windows.Forms.ToolStripMenuItem();
			this.lineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.squareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.circleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuHitboxSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuHitboxResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuDeselectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.save = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).BeginInit();
			this.windowSplit.Panel1.SuspendLayout();
			this.windowSplit.Panel2.SuspendLayout();
			this.windowSplit.SuspendLayout();
			this.sceneStatusPanel.SuspendLayout();
			this.sceneGroup.SuspendLayout();
			this.searchBox.SuspendLayout();
			this.editSelectionGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.snap)).BeginInit();
			this.gridGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridSpacing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.windowPicture)).BeginInit();
			this.sceneRightClickMenu.SuspendLayout();
			this.rightTable.SuspendLayout();
			this.sceneRightClickMenuHitbox.SuspendLayout();
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
			this.sceneStatusPanel.Controls.Add(this.sceneGroup);
			this.sceneStatusPanel.Controls.Add(this.searchBox);
			this.sceneStatusPanel.Controls.Add(this.editSelectionGroup);
			this.sceneStatusPanel.Controls.Add(this.gridGroup);
			this.sceneStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneStatusPanel.Location = new System.Drawing.Point(0, 0);
			this.sceneStatusPanel.Name = "sceneStatusPanel";
			this.sceneStatusPanel.Size = new System.Drawing.Size(1201, 58);
			this.sceneStatusPanel.TabIndex = 2;
			// 
			// sceneGroup
			// 
			this.sceneGroup.Controls.Add(this.saveAsButton);
			this.sceneGroup.Controls.Add(this.saveButton);
			this.sceneGroup.Controls.Add(this.loadButton);
			this.sceneGroup.Dock = System.Windows.Forms.DockStyle.Right;
			this.sceneGroup.ForeColor = System.Drawing.Color.White;
			this.sceneGroup.Location = new System.Drawing.Point(960, 0);
			this.sceneGroup.Name = "sceneGroup";
			this.sceneGroup.Size = new System.Drawing.Size(237, 54);
			this.sceneGroup.TabIndex = 10;
			this.sceneGroup.TabStop = false;
			this.sceneGroup.Text = "Scene";
			// 
			// saveAsButton
			// 
			this.saveAsButton.BackColor = System.Drawing.Color.Black;
			this.saveAsButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.saveAsButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.saveAsButton.ForeColor = System.Drawing.Color.White;
			this.saveAsButton.Location = new System.Drawing.Point(3, 19);
			this.saveAsButton.Name = "saveAsButton";
			this.saveAsButton.Size = new System.Drawing.Size(77, 32);
			this.saveAsButton.TabIndex = 9;
			this.saveAsButton.TabStop = false;
			this.saveAsButton.Text = "Save As";
			this.saveAsButton.UseVisualStyleBackColor = false;
			this.saveAsButton.Click += new System.EventHandler(this.OnSaveAsClick);
			// 
			// saveButton
			// 
			this.saveButton.BackColor = System.Drawing.Color.Black;
			this.saveButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.saveButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.saveButton.ForeColor = System.Drawing.Color.White;
			this.saveButton.Location = new System.Drawing.Point(80, 19);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(77, 32);
			this.saveButton.TabIndex = 8;
			this.saveButton.TabStop = false;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = false;
			this.saveButton.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// loadButton
			// 
			this.loadButton.BackColor = System.Drawing.Color.Black;
			this.loadButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.loadButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.loadButton.ForeColor = System.Drawing.Color.White;
			this.loadButton.Location = new System.Drawing.Point(157, 19);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(77, 32);
			this.loadButton.TabIndex = 7;
			this.loadButton.TabStop = false;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = false;
			this.loadButton.Click += new System.EventHandler(this.OnLoadClick);
			// 
			// searchBox
			// 
			this.searchBox.Controls.Add(this.searchScene);
			this.searchBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.searchBox.ForeColor = System.Drawing.Color.White;
			this.searchBox.Location = new System.Drawing.Point(354, 0);
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
			this.searchScene.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownObjectSearch);
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
            this.sceneRightClickMenuSeparator1,
            this.sceneRightClickMenuResetView,
            this.sceneRightClickMenuUnselectAll});
			this.sceneRightClickMenu.Name = "sceneRightClickMenu";
			this.sceneRightClickMenu.Size = new System.Drawing.Size(136, 76);
			// 
			// sceneRightClickMenuCreate
			// 
			this.sceneRightClickMenuCreate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuCreateSprite,
            this.sceneRightClickMenuCreateText,
            this.sceneRightClickMenuCreateNinePatch,
            this.sceneRightClickMenuCreateSeparator1,
            this.sceneRightClickMenuCreateTileMap,
            this.sceneRightClickMenuCreateLight,
            this.sceneRightClickMenuCreateCamera,
            this.sceneRightClickMenuCreateSeparator2,
            this.sceneRightClickMenuCreateAudio});
			this.sceneRightClickMenuCreate.Name = "sceneRightClickMenuCreate";
			this.sceneRightClickMenuCreate.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuCreate.Text = "Create";
			// 
			// sceneRightClickMenuCreateSprite
			// 
			this.sceneRightClickMenuCreateSprite.Name = "sceneRightClickMenuCreateSprite";
			this.sceneRightClickMenuCreateSprite.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateSprite.Text = "Sprite";
			this.sceneRightClickMenuCreateSprite.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateSprite);
			// 
			// sceneRightClickMenuCreateText
			// 
			this.sceneRightClickMenuCreateText.Name = "sceneRightClickMenuCreateText";
			this.sceneRightClickMenuCreateText.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateText.Text = "Text";
			this.sceneRightClickMenuCreateText.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateText);
			// 
			// sceneRightClickMenuCreateNinePatch
			// 
			this.sceneRightClickMenuCreateNinePatch.Name = "sceneRightClickMenuCreateNinePatch";
			this.sceneRightClickMenuCreateNinePatch.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateNinePatch.Text = "Nine Patch";
			this.sceneRightClickMenuCreateNinePatch.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateNinePatch);
			// 
			// sceneRightClickMenuCreateSeparator1
			// 
			this.sceneRightClickMenuCreateSeparator1.Name = "sceneRightClickMenuCreateSeparator1";
			this.sceneRightClickMenuCreateSeparator1.Size = new System.Drawing.Size(129, 6);
			// 
			// sceneRightClickMenuCreateTileMap
			// 
			this.sceneRightClickMenuCreateTileMap.Name = "sceneRightClickMenuCreateTileMap";
			this.sceneRightClickMenuCreateTileMap.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateTileMap.Text = "Tilemap";
			this.sceneRightClickMenuCreateTileMap.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateTilemap);
			// 
			// sceneRightClickMenuCreateLight
			// 
			this.sceneRightClickMenuCreateLight.Name = "sceneRightClickMenuCreateLight";
			this.sceneRightClickMenuCreateLight.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateLight.Text = "Light";
			this.sceneRightClickMenuCreateLight.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateLight);
			// 
			// sceneRightClickMenuCreateCamera
			// 
			this.sceneRightClickMenuCreateCamera.Name = "sceneRightClickMenuCreateCamera";
			this.sceneRightClickMenuCreateCamera.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateCamera.Text = "Camera";
			this.sceneRightClickMenuCreateCamera.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateCamera);
			// 
			// sceneRightClickMenuCreateSeparator2
			// 
			this.sceneRightClickMenuCreateSeparator2.Name = "sceneRightClickMenuCreateSeparator2";
			this.sceneRightClickMenuCreateSeparator2.Size = new System.Drawing.Size(129, 6);
			// 
			// sceneRightClickMenuCreateAudio
			// 
			this.sceneRightClickMenuCreateAudio.Name = "sceneRightClickMenuCreateAudio";
			this.sceneRightClickMenuCreateAudio.Size = new System.Drawing.Size(132, 22);
			this.sceneRightClickMenuCreateAudio.Text = "Audio";
			this.sceneRightClickMenuCreateAudio.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateAudio);
			// 
			// sceneRightClickMenuSeparator1
			// 
			this.sceneRightClickMenuSeparator1.Name = "sceneRightClickMenuSeparator1";
			this.sceneRightClickMenuSeparator1.Size = new System.Drawing.Size(132, 6);
			// 
			// sceneRightClickMenuResetView
			// 
			this.sceneRightClickMenuResetView.Name = "sceneRightClickMenuResetView";
			this.sceneRightClickMenuResetView.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuResetView.Text = "Reset View";
			this.sceneRightClickMenuResetView.Click += new System.EventHandler(this.OnSceneRightClickMenuResetView);
			// 
			// sceneRightClickMenuUnselectAll
			// 
			this.sceneRightClickMenuUnselectAll.Name = "sceneRightClickMenuUnselectAll";
			this.sceneRightClickMenuUnselectAll.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuUnselectAll.Text = "Deselect All";
			this.sceneRightClickMenuUnselectAll.Click += new System.EventHandler(this.OnSceneRightClickMenuDeselect);
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
			// sceneRightClickMenuHitbox
			// 
			this.sceneRightClickMenuHitbox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuHitboxCreateLine,
            this.sceneRightClickMenuHitboxSeparator1,
            this.sceneRightClickMenuHitboxResetView,
            this.sceneRightClickMenuDeselectAll});
			this.sceneRightClickMenuHitbox.Name = "sceneRightClickMenuHitbox";
			this.sceneRightClickMenuHitbox.Size = new System.Drawing.Size(136, 76);
			// 
			// sceneRightClickMenuHitboxCreateLine
			// 
			this.sceneRightClickMenuHitboxCreateLine.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lineToolStripMenuItem,
            this.squareToolStripMenuItem,
            this.circleToolStripMenuItem});
			this.sceneRightClickMenuHitboxCreateLine.Name = "sceneRightClickMenuHitboxCreateLine";
			this.sceneRightClickMenuHitboxCreateLine.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuHitboxCreateLine.Text = "Create";
			// 
			// lineToolStripMenuItem
			// 
			this.lineToolStripMenuItem.Name = "lineToolStripMenuItem";
			this.lineToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.lineToolStripMenuItem.Text = "Line (1 side)";
			this.lineToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateHitboxLine);
			// 
			// squareToolStripMenuItem
			// 
			this.squareToolStripMenuItem.Name = "squareToolStripMenuItem";
			this.squareToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.squareToolStripMenuItem.Text = "Square (4 sides)";
			this.squareToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateHitboxSquare);
			// 
			// circleToolStripMenuItem
			// 
			this.circleToolStripMenuItem.Name = "circleToolStripMenuItem";
			this.circleToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
			this.circleToolStripMenuItem.Text = "Circle (8 sides)";
			this.circleToolStripMenuItem.Click += new System.EventHandler(this.OnSceneRightclickMenuCreateHitboxCircle);
			// 
			// sceneRightClickMenuHitboxSeparator1
			// 
			this.sceneRightClickMenuHitboxSeparator1.Name = "sceneRightClickMenuHitboxSeparator1";
			this.sceneRightClickMenuHitboxSeparator1.Size = new System.Drawing.Size(132, 6);
			// 
			// sceneRightClickMenuHitboxResetView
			// 
			this.sceneRightClickMenuHitboxResetView.Name = "sceneRightClickMenuHitboxResetView";
			this.sceneRightClickMenuHitboxResetView.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuHitboxResetView.Text = "Reset View";
			this.sceneRightClickMenuHitboxResetView.Click += new System.EventHandler(this.OnSceneRightClickMenuResetView);
			// 
			// sceneRightClickMenuDeselectAll
			// 
			this.sceneRightClickMenuDeselectAll.Name = "sceneRightClickMenuDeselectAll";
			this.sceneRightClickMenuDeselectAll.Size = new System.Drawing.Size(135, 22);
			this.sceneRightClickMenuDeselectAll.Text = "Deselect All";
			this.sceneRightClickMenuDeselectAll.Click += new System.EventHandler(this.OnSceneRightClickMenuDeselect);
			// 
			// save
			// 
			this.save.DefaultExt = "scene";
			this.save.Filter = "Scene|*.scene";
			this.save.Title = "Save Scene";
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
			this.sceneGroup.ResumeLayout(false);
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
			this.sceneRightClickMenuHitbox.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer windowSplit;
		private Panel sceneStatusPanel;
		private GroupBox gridGroup;
		private TrackBar gridThickness;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripSeparator sceneRightClickMenuSeparator1;
		private ToolStripMenuItem sceneRightClickMenuResetView;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
		private Button saveButton;
		private Label sceneValues;
		private ListBox editSelectionOptions;
		private NumericUpDown snap;
		private NumericUpDown gridSpacing;
		private PictureBox windowPicture;
		private GroupBox editSelectionGroup;
		private Button loadButton;
		private OpenFileDialog load;
		private GroupBox searchBox;
		private TableLayoutPanel rightTable;
		private TableLayoutPanel thingTypesTable;
		private GroupBox sceneGroup;
		private TextBox searchScene;
		private ToolStripMenuItem sceneRightClickMenuCreateLight;
		private ToolStripMenuItem sceneRightClickMenuUnselectAll;
		private ToolStripMenuItem sceneRightClickMenuCreateCamera;
		private ColorDialog pickColor;
		private OpenFileDialog pickAsset;
		private ContextMenuStrip thingsList;
		private ContextMenuStrip sceneRightClickMenuHitbox;
		private ToolStripMenuItem sceneRightClickMenuHitboxCreateLine;
		private ToolStripSeparator sceneRightClickMenuHitboxSeparator1;
		private ToolStripMenuItem sceneRightClickMenuHitboxResetView;
		private ToolStripMenuItem sceneRightClickMenuDeselectAll;
		private ToolStripMenuItem lineToolStripMenuItem;
		private ToolStripMenuItem squareToolStripMenuItem;
		private ToolStripMenuItem circleToolStripMenuItem;
		private ToolStripMenuItem sceneRightClickMenuCreateText;
		private ToolStripSeparator sceneRightClickMenuCreateSeparator1;
		private ToolStripMenuItem sceneRightClickMenuCreateNinePatch;
		private Button saveAsButton;
		private Label loading;
		private ToolStripSeparator sceneRightClickMenuCreateSeparator2;
		private ToolStripMenuItem sceneRightClickMenuCreateAudio;
		private ToolStripMenuItem sceneRightClickMenuCreateTileMap;
		private SaveFileDialog save;
	}
}