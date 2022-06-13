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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("World");
			this.windowSplit = new System.Windows.Forms.SplitContainer();
			this.fps = new System.Windows.Forms.Label();
			this.sceneMousePos = new System.Windows.Forms.Label();
			this.sceneStatusPanel = new System.Windows.Forms.Panel();
			this.editSelectionGroup = new System.Windows.Forms.GroupBox();
			this.editSelectionOptions = new System.Windows.Forms.ListBox();
			this.snap = new System.Windows.Forms.NumericUpDown();
			this.saveButton = new System.Windows.Forms.Button();
			this.gridGroup = new System.Windows.Forms.GroupBox();
			this.gridSpacing = new System.Windows.Forms.NumericUpDown();
			this.gridThickness = new System.Windows.Forms.TrackBar();
			this.windowPicture = new System.Windows.Forms.PictureBox();
			this.sceneRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSprite = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.rightPanelTabs = new System.Windows.Forms.TabControl();
			this.sceneObjectEdit = new System.Windows.Forms.TabPage();
			this.thingSelectSplit = new System.Windows.Forms.SplitContainer();
			this.sceneTreeObjectsTable = new System.Windows.Forms.TableLayoutPanel();
			this.sceneObjects = new System.Windows.Forms.TreeView();
			this.sceneTreeObjectsBottomTable = new System.Windows.Forms.TableLayoutPanel();
			this.sceneObjectsMoveDown = new System.Windows.Forms.Button();
			this.searchScene = new System.Windows.Forms.MaskedTextBox();
			this.sceneTreeObjectsTopTable = new System.Windows.Forms.TableLayoutPanel();
			this.unparentSelectionButton = new System.Windows.Forms.Button();
			this.sceneObjectsMoveUp = new System.Windows.Forms.Button();
			this.assetsTab = new System.Windows.Forms.TabPage();
			this.assetsSplit = new System.Windows.Forms.TableLayoutPanel();
			this.assetDelete = new System.Windows.Forms.Button();
			this.assetsMoveToRootButton = new System.Windows.Forms.Button();
			this.assetsCreateFolderButton = new System.Windows.Forms.Button();
			this.importAssetsButton = new System.Windows.Forms.Button();
			this.assets = new System.Windows.Forms.TreeView();
			this.save = new System.Windows.Forms.SaveFileDialog();
			this.importAssets = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).BeginInit();
			this.windowSplit.Panel1.SuspendLayout();
			this.windowSplit.Panel2.SuspendLayout();
			this.windowSplit.SuspendLayout();
			this.sceneStatusPanel.SuspendLayout();
			this.editSelectionGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.snap)).BeginInit();
			this.gridGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridSpacing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.windowPicture)).BeginInit();
			this.sceneRightClickMenu.SuspendLayout();
			this.rightPanelTabs.SuspendLayout();
			this.sceneObjectEdit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.thingSelectSplit)).BeginInit();
			this.thingSelectSplit.Panel1.SuspendLayout();
			this.thingSelectSplit.SuspendLayout();
			this.sceneTreeObjectsTable.SuspendLayout();
			this.sceneTreeObjectsBottomTable.SuspendLayout();
			this.sceneTreeObjectsTopTable.SuspendLayout();
			this.assetsTab.SuspendLayout();
			this.assetsSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// windowSplit
			// 
			this.windowSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.windowSplit.Location = new System.Drawing.Point(0, 0);
			this.windowSplit.Name = "windowSplit";
			// 
			// windowSplit.Panel1
			// 
			this.windowSplit.Panel1.Controls.Add(this.fps);
			this.windowSplit.Panel1.Controls.Add(this.sceneMousePos);
			this.windowSplit.Panel1.Controls.Add(this.sceneStatusPanel);
			this.windowSplit.Panel1.Controls.Add(this.windowPicture);
			// 
			// windowSplit.Panel2
			// 
			this.windowSplit.Panel2.Controls.Add(this.rightPanelTabs);
			this.windowSplit.Size = new System.Drawing.Size(1564, 761);
			this.windowSplit.SplitterDistance = 1295;
			this.windowSplit.TabIndex = 0;
			this.windowSplit.TabStop = false;
			// 
			// fps
			// 
			this.fps.AutoSize = true;
			this.fps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.fps.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.fps.ForeColor = System.Drawing.Color.White;
			this.fps.Location = new System.Drawing.Point(0, 61);
			this.fps.Name = "fps";
			this.fps.Size = new System.Drawing.Size(31, 21);
			this.fps.TabIndex = 4;
			this.fps.Text = "fps";
			// 
			// sceneMousePos
			// 
			this.sceneMousePos.AutoSize = true;
			this.sceneMousePos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.sceneMousePos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.sceneMousePos.ForeColor = System.Drawing.Color.White;
			this.sceneMousePos.Location = new System.Drawing.Point(0, 82);
			this.sceneMousePos.Name = "sceneMousePos";
			this.sceneMousePos.Size = new System.Drawing.Size(81, 21);
			this.sceneMousePos.TabIndex = 3;
			this.sceneMousePos.Text = "mousePos";
			// 
			// sceneStatusPanel
			// 
			this.sceneStatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sceneStatusPanel.Controls.Add(this.editSelectionGroup);
			this.sceneStatusPanel.Controls.Add(this.saveButton);
			this.sceneStatusPanel.Controls.Add(this.gridGroup);
			this.sceneStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneStatusPanel.Location = new System.Drawing.Point(0, 0);
			this.sceneStatusPanel.Name = "sceneStatusPanel";
			this.sceneStatusPanel.Size = new System.Drawing.Size(1295, 58);
			this.sceneStatusPanel.TabIndex = 2;
			// 
			// editSelectionGroup
			// 
			this.editSelectionGroup.Controls.Add(this.editSelectionOptions);
			this.editSelectionGroup.Controls.Add(this.snap);
			this.editSelectionGroup.ForeColor = System.Drawing.Color.White;
			this.editSelectionGroup.Location = new System.Drawing.Point(174, 0);
			this.editSelectionGroup.Name = "editSelectionGroup";
			this.editSelectionGroup.Size = new System.Drawing.Size(193, 54);
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
            "    Reposition",
            "       Rotate"});
			this.editSelectionOptions.Location = new System.Drawing.Point(3, 19);
			this.editSelectionOptions.Name = "editSelectionOptions";
			this.editSelectionOptions.Size = new System.Drawing.Size(91, 32);
			this.editSelectionOptions.TabIndex = 3;
			this.editSelectionOptions.Click += new System.EventHandler(this.SelectObjectsTree);
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
			// 
			// saveButton
			// 
			this.saveButton.BackColor = System.Drawing.Color.Black;
			this.saveButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.saveButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.saveButton.ForeColor = System.Drawing.Color.White;
			this.saveButton.Location = new System.Drawing.Point(1214, 0);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(77, 54);
			this.saveButton.TabIndex = 1;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = false;
			this.saveButton.Click += new System.EventHandler(this.OnSaveClick);
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
			this.gridSpacing.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.gridSpacing.Click += new System.EventHandler(this.SelectObjectsTree);
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
			this.gridThickness.ValueChanged += new System.EventHandler(this.SelectObjectsTree);
			// 
			// windowPicture
			// 
			this.windowPicture.BackColor = System.Drawing.Color.Black;
			this.windowPicture.ContextMenuStrip = this.sceneRightClickMenu;
			this.windowPicture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.windowPicture.Location = new System.Drawing.Point(0, 0);
			this.windowPicture.Name = "windowPicture";
			this.windowPicture.Size = new System.Drawing.Size(1295, 761);
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
            this.sceneRightClickMenuResetView});
			this.sceneRightClickMenu.Name = "sceneRightClickMenu";
			this.sceneRightClickMenu.Size = new System.Drawing.Size(131, 54);
			// 
			// sceneRightClickMenuCreate
			// 
			this.sceneRightClickMenuCreate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sceneRightClickMenuCreateSprite});
			this.sceneRightClickMenuCreate.Name = "sceneRightClickMenuCreate";
			this.sceneRightClickMenuCreate.Size = new System.Drawing.Size(130, 22);
			this.sceneRightClickMenuCreate.Text = "Create";
			// 
			// sceneRightClickMenuCreateSprite
			// 
			this.sceneRightClickMenuCreateSprite.Name = "sceneRightClickMenuCreateSprite";
			this.sceneRightClickMenuCreateSprite.Size = new System.Drawing.Size(104, 22);
			this.sceneRightClickMenuCreateSprite.Text = "Sprite";
			this.sceneRightClickMenuCreateSprite.Click += new System.EventHandler(this.OnSceneRightClickMenuCreateSprite);
			// 
			// sceneRightClickMenuSeparator1
			// 
			this.sceneRightClickMenuSeparator1.Name = "sceneRightClickMenuSeparator1";
			this.sceneRightClickMenuSeparator1.Size = new System.Drawing.Size(127, 6);
			// 
			// sceneRightClickMenuResetView
			// 
			this.sceneRightClickMenuResetView.Name = "sceneRightClickMenuResetView";
			this.sceneRightClickMenuResetView.Size = new System.Drawing.Size(130, 22);
			this.sceneRightClickMenuResetView.Text = "Reset View";
			this.sceneRightClickMenuResetView.Click += new System.EventHandler(this.OnSceneRightClickMenuResetView);
			// 
			// rightPanelTabs
			// 
			this.rightPanelTabs.Controls.Add(this.sceneObjectEdit);
			this.rightPanelTabs.Controls.Add(this.assetsTab);
			this.rightPanelTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rightPanelTabs.Location = new System.Drawing.Point(0, 0);
			this.rightPanelTabs.Name = "rightPanelTabs";
			this.rightPanelTabs.SelectedIndex = 0;
			this.rightPanelTabs.Size = new System.Drawing.Size(265, 761);
			this.rightPanelTabs.TabIndex = 2;
			this.rightPanelTabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.OnRightTabSelect);
			// 
			// sceneObjectEdit
			// 
			this.sceneObjectEdit.BackColor = System.Drawing.Color.Black;
			this.sceneObjectEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sceneObjectEdit.Controls.Add(this.thingSelectSplit);
			this.sceneObjectEdit.ForeColor = System.Drawing.Color.White;
			this.sceneObjectEdit.Location = new System.Drawing.Point(4, 24);
			this.sceneObjectEdit.Name = "sceneObjectEdit";
			this.sceneObjectEdit.Padding = new System.Windows.Forms.Padding(3);
			this.sceneObjectEdit.Size = new System.Drawing.Size(257, 733);
			this.sceneObjectEdit.TabIndex = 0;
			this.sceneObjectEdit.Text = "Edit Thing";
			// 
			// thingSelectSplit
			// 
			this.thingSelectSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.thingSelectSplit.Location = new System.Drawing.Point(3, 3);
			this.thingSelectSplit.Name = "thingSelectSplit";
			this.thingSelectSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// thingSelectSplit.Panel1
			// 
			this.thingSelectSplit.Panel1.Controls.Add(this.sceneTreeObjectsTable);
			this.thingSelectSplit.Size = new System.Drawing.Size(247, 723);
			this.thingSelectSplit.SplitterDistance = 242;
			this.thingSelectSplit.TabIndex = 3;
			// 
			// sceneTreeObjectsTable
			// 
			this.sceneTreeObjectsTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.sceneTreeObjectsTable.ColumnCount = 1;
			this.sceneTreeObjectsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.sceneTreeObjectsTable.Controls.Add(this.sceneObjects, 0, 1);
			this.sceneTreeObjectsTable.Controls.Add(this.sceneTreeObjectsBottomTable, 0, 2);
			this.sceneTreeObjectsTable.Controls.Add(this.sceneTreeObjectsTopTable, 0, 0);
			this.sceneTreeObjectsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneTreeObjectsTable.Location = new System.Drawing.Point(0, 0);
			this.sceneTreeObjectsTable.Name = "sceneTreeObjectsTable";
			this.sceneTreeObjectsTable.RowCount = 3;
			this.sceneTreeObjectsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
			this.sceneTreeObjectsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
			this.sceneTreeObjectsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
			this.sceneTreeObjectsTable.Size = new System.Drawing.Size(247, 242);
			this.sceneTreeObjectsTable.TabIndex = 6;
			// 
			// sceneObjects
			// 
			this.sceneObjects.AllowDrop = true;
			this.sceneObjects.BackColor = System.Drawing.Color.Black;
			this.sceneObjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneObjects.ForeColor = System.Drawing.Color.White;
			this.sceneObjects.FullRowSelect = true;
			this.sceneObjects.LabelEdit = true;
			this.sceneObjects.Location = new System.Drawing.Point(4, 40);
			this.sceneObjects.Name = "sceneObjects";
			treeNode1.Name = "world";
			treeNode1.Text = "World";
			this.sceneObjects.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.sceneObjects.Size = new System.Drawing.Size(239, 160);
			this.sceneObjects.TabIndex = 1;
			this.sceneObjects.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnTreeObjectRename);
			this.sceneObjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.OnTreeNodeDrag);
			this.sceneObjects.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnSceneObjectClick);
			this.sceneObjects.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeViewDoubleClick);
			this.sceneObjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragDrop);
			this.sceneObjects.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragEnter);
			this.sceneObjects.DragOver += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragOver);
			// 
			// sceneTreeObjectsBottomTable
			// 
			this.sceneTreeObjectsBottomTable.ColumnCount = 2;
			this.sceneTreeObjectsBottomTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.sceneTreeObjectsBottomTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.sceneTreeObjectsBottomTable.Controls.Add(this.sceneObjectsMoveDown, 1, 0);
			this.sceneTreeObjectsBottomTable.Controls.Add(this.searchScene, 0, 0);
			this.sceneTreeObjectsBottomTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneTreeObjectsBottomTable.Location = new System.Drawing.Point(4, 207);
			this.sceneTreeObjectsBottomTable.Name = "sceneTreeObjectsBottomTable";
			this.sceneTreeObjectsBottomTable.RowCount = 1;
			this.sceneTreeObjectsBottomTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.sceneTreeObjectsBottomTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.sceneTreeObjectsBottomTable.Size = new System.Drawing.Size(239, 31);
			this.sceneTreeObjectsBottomTable.TabIndex = 8;
			// 
			// sceneObjectsMoveDown
			// 
			this.sceneObjectsMoveDown.BackColor = System.Drawing.Color.Black;
			this.sceneObjectsMoveDown.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneObjectsMoveDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.sceneObjectsMoveDown.Location = new System.Drawing.Point(122, 3);
			this.sceneObjectsMoveDown.Name = "sceneObjectsMoveDown";
			this.sceneObjectsMoveDown.Size = new System.Drawing.Size(114, 25);
			this.sceneObjectsMoveDown.TabIndex = 4;
			this.sceneObjectsMoveDown.Text = "v";
			this.sceneObjectsMoveDown.UseVisualStyleBackColor = false;
			this.sceneObjectsMoveDown.Click += new System.EventHandler(this.OnObjectsSelectMoveDown);
			// 
			// searchScene
			// 
			this.searchScene.BackColor = System.Drawing.Color.Black;
			this.searchScene.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.searchScene.ForeColor = System.Drawing.Color.White;
			this.searchScene.Location = new System.Drawing.Point(3, 3);
			this.searchScene.Name = "searchScene";
			this.searchScene.Size = new System.Drawing.Size(113, 26);
			this.searchScene.TabIndex = 2;
			this.searchScene.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownObjectSearch);
			// 
			// sceneTreeObjectsTopTable
			// 
			this.sceneTreeObjectsTopTable.ColumnCount = 2;
			this.sceneTreeObjectsTopTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.sceneTreeObjectsTopTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.sceneTreeObjectsTopTable.Controls.Add(this.unparentSelectionButton, 0, 0);
			this.sceneTreeObjectsTopTable.Controls.Add(this.sceneObjectsMoveUp, 1, 0);
			this.sceneTreeObjectsTopTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneTreeObjectsTopTable.Location = new System.Drawing.Point(4, 4);
			this.sceneTreeObjectsTopTable.Name = "sceneTreeObjectsTopTable";
			this.sceneTreeObjectsTopTable.RowCount = 1;
			this.sceneTreeObjectsTopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.sceneTreeObjectsTopTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.sceneTreeObjectsTopTable.Size = new System.Drawing.Size(239, 29);
			this.sceneTreeObjectsTopTable.TabIndex = 7;
			// 
			// unparentSelectionButton
			// 
			this.unparentSelectionButton.BackColor = System.Drawing.Color.Black;
			this.unparentSelectionButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.unparentSelectionButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.unparentSelectionButton.Location = new System.Drawing.Point(3, 3);
			this.unparentSelectionButton.Name = "unparentSelectionButton";
			this.unparentSelectionButton.Size = new System.Drawing.Size(113, 23);
			this.unparentSelectionButton.TabIndex = 5;
			this.unparentSelectionButton.Text = "Unparent";
			this.unparentSelectionButton.UseVisualStyleBackColor = false;
			this.unparentSelectionButton.Click += new System.EventHandler(this.OnUnparentSelectionButtonClick);
			// 
			// sceneObjectsMoveUp
			// 
			this.sceneObjectsMoveUp.BackColor = System.Drawing.Color.Black;
			this.sceneObjectsMoveUp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneObjectsMoveUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.sceneObjectsMoveUp.Location = new System.Drawing.Point(122, 3);
			this.sceneObjectsMoveUp.Name = "sceneObjectsMoveUp";
			this.sceneObjectsMoveUp.Size = new System.Drawing.Size(114, 23);
			this.sceneObjectsMoveUp.TabIndex = 3;
			this.sceneObjectsMoveUp.Text = "^";
			this.sceneObjectsMoveUp.UseVisualStyleBackColor = false;
			this.sceneObjectsMoveUp.Click += new System.EventHandler(this.OnObjectsSelectMoveUp);
			// 
			// assetsTab
			// 
			this.assetsTab.BackColor = System.Drawing.Color.Black;
			this.assetsTab.Controls.Add(this.assetsSplit);
			this.assetsTab.ForeColor = System.Drawing.Color.White;
			this.assetsTab.Location = new System.Drawing.Point(4, 24);
			this.assetsTab.Name = "assetsTab";
			this.assetsTab.Padding = new System.Windows.Forms.Padding(3);
			this.assetsTab.Size = new System.Drawing.Size(257, 733);
			this.assetsTab.TabIndex = 1;
			this.assetsTab.Text = "Assets";
			// 
			// assetsSplit
			// 
			this.assetsSplit.ColumnCount = 1;
			this.assetsSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.assetsSplit.Controls.Add(this.assetDelete, 0, 3);
			this.assetsSplit.Controls.Add(this.assetsMoveToRootButton, 0, 2);
			this.assetsSplit.Controls.Add(this.assetsCreateFolderButton, 0, 1);
			this.assetsSplit.Controls.Add(this.importAssetsButton, 0, 0);
			this.assetsSplit.Controls.Add(this.assets, 0, 4);
			this.assetsSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assetsSplit.Location = new System.Drawing.Point(3, 3);
			this.assetsSplit.Name = "assetsSplit";
			this.assetsSplit.RowCount = 5;
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.assetsSplit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.assetsSplit.Size = new System.Drawing.Size(251, 727);
			this.assetsSplit.TabIndex = 6;
			// 
			// assetDelete
			// 
			this.assetDelete.BackColor = System.Drawing.Color.Black;
			this.assetDelete.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assetDelete.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.assetDelete.Location = new System.Drawing.Point(3, 111);
			this.assetDelete.Name = "assetDelete";
			this.assetDelete.Size = new System.Drawing.Size(245, 30);
			this.assetDelete.TabIndex = 10;
			this.assetDelete.Text = "Delete";
			this.assetDelete.UseVisualStyleBackColor = false;
			this.assetDelete.Click += new System.EventHandler(this.OnAssetDeleteClick);
			// 
			// assetsMoveToRootButton
			// 
			this.assetsMoveToRootButton.BackColor = System.Drawing.Color.Black;
			this.assetsMoveToRootButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assetsMoveToRootButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.assetsMoveToRootButton.Location = new System.Drawing.Point(3, 75);
			this.assetsMoveToRootButton.Name = "assetsMoveToRootButton";
			this.assetsMoveToRootButton.Size = new System.Drawing.Size(245, 30);
			this.assetsMoveToRootButton.TabIndex = 9;
			this.assetsMoveToRootButton.Text = "Move to Root";
			this.assetsMoveToRootButton.UseVisualStyleBackColor = false;
			this.assetsMoveToRootButton.Click += new System.EventHandler(this.OnAssetMoveToRootClick);
			// 
			// assetsCreateFolderButton
			// 
			this.assetsCreateFolderButton.BackColor = System.Drawing.Color.Black;
			this.assetsCreateFolderButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assetsCreateFolderButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.assetsCreateFolderButton.Location = new System.Drawing.Point(3, 39);
			this.assetsCreateFolderButton.Name = "assetsCreateFolderButton";
			this.assetsCreateFolderButton.Size = new System.Drawing.Size(245, 30);
			this.assetsCreateFolderButton.TabIndex = 8;
			this.assetsCreateFolderButton.Text = "Create Folder";
			this.assetsCreateFolderButton.UseVisualStyleBackColor = false;
			this.assetsCreateFolderButton.Click += new System.EventHandler(this.OnAssetsCreateFolderClick);
			// 
			// importAssetsButton
			// 
			this.importAssetsButton.BackColor = System.Drawing.Color.Black;
			this.importAssetsButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.importAssetsButton.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.importAssetsButton.Location = new System.Drawing.Point(3, 3);
			this.importAssetsButton.Name = "importAssetsButton";
			this.importAssetsButton.Size = new System.Drawing.Size(245, 30);
			this.importAssetsButton.TabIndex = 7;
			this.importAssetsButton.Text = "Import";
			this.importAssetsButton.UseVisualStyleBackColor = false;
			this.importAssetsButton.Click += new System.EventHandler(this.OnAssetsImportClick);
			// 
			// assets
			// 
			this.assets.AllowDrop = true;
			this.assets.BackColor = System.Drawing.Color.Black;
			this.assets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assets.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.assets.FullRowSelect = true;
			this.assets.LabelEdit = true;
			this.assets.Location = new System.Drawing.Point(3, 147);
			this.assets.Name = "assets";
			this.assets.Size = new System.Drawing.Size(245, 577);
			this.assets.TabIndex = 0;
			this.assets.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnAssetRename);
			this.assets.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.OnTreeNodeDrag);
			this.assets.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeViewDoubleClick);
			this.assets.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnAssetsDragDrop);
			this.assets.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnAssetsDragEnter);
			this.assets.DragOver += new System.Windows.Forms.DragEventHandler(this.OnAssetsDragOver);
			// 
			// save
			// 
			this.save.DefaultExt = "cdb";
			this.save.FileName = "scene";
			this.save.Filter = "Database (CastleDB)|*.cdb";
			this.save.InitialDirectory = ".";
			this.save.Title = "Save Scene";
			// 
			// importAssets
			// 
			this.importAssets.Filter = "Texture|*.png|Texture|*.jpg|Texture|*.bmp|Audio|*.ogg|Audio|*.flac|Audio|*.wav|Fo" +
    "nt|*.ttf|Font|*.otf|Database (CastleDB)|*.cdb|Shader|*.frag|Shader|*.vert|Model|" +
    "*.obj";
			this.importAssets.Multiselect = true;
			this.importAssets.Title = "Import Assets";
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
			this.editSelectionGroup.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.snap)).EndInit();
			this.gridGroup.ResumeLayout(false);
			this.gridGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridSpacing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.windowPicture)).EndInit();
			this.sceneRightClickMenu.ResumeLayout(false);
			this.rightPanelTabs.ResumeLayout(false);
			this.sceneObjectEdit.ResumeLayout(false);
			this.thingSelectSplit.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.thingSelectSplit)).EndInit();
			this.thingSelectSplit.ResumeLayout(false);
			this.sceneTreeObjectsTable.ResumeLayout(false);
			this.sceneTreeObjectsBottomTable.ResumeLayout(false);
			this.sceneTreeObjectsBottomTable.PerformLayout();
			this.sceneTreeObjectsTopTable.ResumeLayout(false);
			this.assetsTab.ResumeLayout(false);
			this.assetsSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer windowSplit;
		private Panel sceneStatusPanel;
		private GroupBox gridGroup;
		private TrackBar gridThickness;
		private Label sceneMousePos;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripSeparator sceneRightClickMenuSeparator1;
		private ToolStripMenuItem sceneRightClickMenuResetView;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
		private TreeView sceneObjects;
		private TabControl rightPanelTabs;
		private TabPage sceneObjectEdit;
		private TabPage assetsTab;
		private MaskedTextBox searchScene;
		private SplitContainer thingSelectSplit;
		private Button sceneObjectsMoveDown;
		private Button sceneObjectsMoveUp;
		private SaveFileDialog save;
		private Button saveButton;
		private Label fps;
		private ListBox editSelectionOptions;
		private NumericUpDown snap;
		private NumericUpDown gridSpacing;
		private Button unparentSelectionButton;
		private PictureBox windowPicture;
		private GroupBox editSelectionGroup;
		private Button importAssetsButton;
		private TreeView assets;
		private TableLayoutPanel assetsSplit;
		private OpenFileDialog importAssets;
		private Button assetsCreateFolderButton;
		private Button assetsMoveToRootButton;
		private TableLayoutPanel sceneTreeObjectsTable;
		private TableLayoutPanel sceneTreeObjectsBottomTable;
		private TableLayoutPanel sceneTreeObjectsTopTable;
		private Button assetDelete;
	}
}