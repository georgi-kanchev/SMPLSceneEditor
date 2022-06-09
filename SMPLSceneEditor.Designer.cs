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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("folder");
			this.windowSplit = new System.Windows.Forms.SplitContainer();
			this.sceneRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSprite = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.fps = new System.Windows.Forms.Label();
			this.sceneMousePos = new System.Windows.Forms.Label();
			this.sceneStatusPanel = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.gridGroup = new System.Windows.Forms.GroupBox();
			this.gridSnap = new System.Windows.Forms.CheckBox();
			this.gridSpacing = new System.Windows.Forms.MaskedTextBox();
			this.gridThickness = new System.Windows.Forms.TrackBar();
			this.sceneAngle = new System.Windows.Forms.HScrollBar();
			this.sceneZoom = new System.Windows.Forms.VScrollBar();
			this.rightTabsPanel = new System.Windows.Forms.TabControl();
			this.sceneObjectEdit = new System.Windows.Forms.TabPage();
			this.thingSelectSplit = new System.Windows.Forms.SplitContainer();
			this.sceneObjects = new System.Windows.Forms.TreeView();
			this.sceneObjectsMoveUp = new System.Windows.Forms.Button();
			this.sceneObjectsMoveDown = new System.Windows.Forms.Button();
			this.searchScene = new System.Windows.Forms.MaskedTextBox();
			this.assetsTab = new System.Windows.Forms.TabPage();
			this.assets = new System.Windows.Forms.TreeView();
			this.save = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).BeginInit();
			this.windowSplit.Panel1.SuspendLayout();
			this.windowSplit.Panel2.SuspendLayout();
			this.windowSplit.SuspendLayout();
			this.sceneRightClickMenu.SuspendLayout();
			this.sceneStatusPanel.SuspendLayout();
			this.gridGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).BeginInit();
			this.rightTabsPanel.SuspendLayout();
			this.sceneObjectEdit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.thingSelectSplit)).BeginInit();
			this.thingSelectSplit.Panel1.SuspendLayout();
			this.thingSelectSplit.Panel2.SuspendLayout();
			this.thingSelectSplit.SuspendLayout();
			this.assetsTab.SuspendLayout();
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
			this.windowSplit.Panel1.ContextMenuStrip = this.sceneRightClickMenu;
			this.windowSplit.Panel1.Controls.Add(this.fps);
			this.windowSplit.Panel1.Controls.Add(this.sceneMousePos);
			this.windowSplit.Panel1.Controls.Add(this.sceneStatusPanel);
			this.windowSplit.Panel1.Controls.Add(this.sceneAngle);
			this.windowSplit.Panel1.Controls.Add(this.sceneZoom);
			this.windowSplit.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDownScene);
			this.windowSplit.Panel1.MouseEnter += new System.EventHandler(this.OnMouseEnterScene);
			this.windowSplit.Panel1.MouseLeave += new System.EventHandler(this.OnMouseLeaveScene);
			this.windowSplit.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveScene);
			this.windowSplit.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpScene);
			// 
			// windowSplit.Panel2
			// 
			this.windowSplit.Panel2.Controls.Add(this.rightTabsPanel);
			this.windowSplit.Size = new System.Drawing.Size(1264, 681);
			this.windowSplit.SplitterDistance = 1047;
			this.windowSplit.TabIndex = 0;
			this.windowSplit.TabStop = false;
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
			// fps
			// 
			this.fps.AutoSize = true;
			this.fps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.fps.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.fps.ForeColor = System.Drawing.Color.White;
			this.fps.Location = new System.Drawing.Point(3, 60);
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
			this.sceneMousePos.Location = new System.Drawing.Point(0, 81);
			this.sceneMousePos.Name = "sceneMousePos";
			this.sceneMousePos.Size = new System.Drawing.Size(81, 21);
			this.sceneMousePos.TabIndex = 3;
			this.sceneMousePos.Text = "mousePos";
			// 
			// sceneStatusPanel
			// 
			this.sceneStatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.sceneStatusPanel.Controls.Add(this.button1);
			this.sceneStatusPanel.Controls.Add(this.gridGroup);
			this.sceneStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneStatusPanel.Location = new System.Drawing.Point(0, 0);
			this.sceneStatusPanel.Name = "sceneStatusPanel";
			this.sceneStatusPanel.Size = new System.Drawing.Size(1030, 57);
			this.sceneStatusPanel.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.Color.Black;
			this.button1.Dock = System.Windows.Forms.DockStyle.Right;
			this.button1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.button1.ForeColor = System.Drawing.Color.White;
			this.button1.Location = new System.Drawing.Point(949, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(77, 53);
			this.button1.TabIndex = 1;
			this.button1.Text = "Save";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.OnSaveClick);
			// 
			// gridGroup
			// 
			this.gridGroup.Controls.Add(this.gridSnap);
			this.gridGroup.Controls.Add(this.gridSpacing);
			this.gridGroup.Controls.Add(this.gridThickness);
			this.gridGroup.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridGroup.ForeColor = System.Drawing.Color.White;
			this.gridGroup.Location = new System.Drawing.Point(0, 0);
			this.gridGroup.Name = "gridGroup";
			this.gridGroup.Size = new System.Drawing.Size(218, 53);
			this.gridGroup.TabIndex = 0;
			this.gridGroup.TabStop = false;
			this.gridGroup.Text = "Grid";
			// 
			// gridSnap
			// 
			this.gridSnap.AutoSize = true;
			this.gridSnap.Location = new System.Drawing.Point(162, 21);
			this.gridSnap.Name = "gridSnap";
			this.gridSnap.Size = new System.Drawing.Size(52, 19);
			this.gridSnap.TabIndex = 2;
			this.gridSnap.Text = "Snap";
			this.gridSnap.UseVisualStyleBackColor = true;
			// 
			// gridSpacing
			// 
			this.gridSpacing.BackColor = System.Drawing.Color.Black;
			this.gridSpacing.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridSpacing.ForeColor = System.Drawing.Color.White;
			this.gridSpacing.Location = new System.Drawing.Point(107, 19);
			this.gridSpacing.Mask = "00000";
			this.gridSpacing.Name = "gridSpacing";
			this.gridSpacing.PromptChar = '-';
			this.gridSpacing.Size = new System.Drawing.Size(49, 23);
			this.gridSpacing.TabIndex = 1;
			this.gridSpacing.TabStop = false;
			this.gridSpacing.Text = "100";
			this.gridSpacing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.gridSpacing.ValidatingType = typeof(int);
			// 
			// gridThickness
			// 
			this.gridThickness.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.gridThickness.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridThickness.LargeChange = 1;
			this.gridThickness.Location = new System.Drawing.Point(3, 19);
			this.gridThickness.Name = "gridThickness";
			this.gridThickness.Size = new System.Drawing.Size(104, 31);
			this.gridThickness.TabIndex = 0;
			this.gridThickness.TabStop = false;
			this.gridThickness.Value = 2;
			// 
			// sceneAngle
			// 
			this.sceneAngle.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.sceneAngle.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sceneAngle.LargeChange = 1;
			this.sceneAngle.Location = new System.Drawing.Point(0, 664);
			this.sceneAngle.Name = "sceneAngle";
			this.sceneAngle.Size = new System.Drawing.Size(1030, 17);
			this.sceneAngle.TabIndex = 1;
			this.sceneAngle.ValueChanged += new System.EventHandler(this.OnSceneRotate);
			// 
			// sceneZoom
			// 
			this.sceneZoom.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.sceneZoom.Dock = System.Windows.Forms.DockStyle.Right;
			this.sceneZoom.LargeChange = 1;
			this.sceneZoom.Location = new System.Drawing.Point(1030, 0);
			this.sceneZoom.Name = "sceneZoom";
			this.sceneZoom.Size = new System.Drawing.Size(17, 681);
			this.sceneZoom.TabIndex = 0;
			this.sceneZoom.Value = 5;
			this.sceneZoom.ValueChanged += new System.EventHandler(this.OnSceneZoom);
			// 
			// rightTabsPanel
			// 
			this.rightTabsPanel.Controls.Add(this.sceneObjectEdit);
			this.rightTabsPanel.Controls.Add(this.assetsTab);
			this.rightTabsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rightTabsPanel.Location = new System.Drawing.Point(0, 0);
			this.rightTabsPanel.Name = "rightTabsPanel";
			this.rightTabsPanel.SelectedIndex = 0;
			this.rightTabsPanel.Size = new System.Drawing.Size(213, 681);
			this.rightTabsPanel.TabIndex = 2;
			// 
			// sceneObjectEdit
			// 
			this.sceneObjectEdit.BackColor = System.Drawing.Color.Black;
			this.sceneObjectEdit.Controls.Add(this.thingSelectSplit);
			this.sceneObjectEdit.ForeColor = System.Drawing.Color.White;
			this.sceneObjectEdit.Location = new System.Drawing.Point(4, 24);
			this.sceneObjectEdit.Name = "sceneObjectEdit";
			this.sceneObjectEdit.Padding = new System.Windows.Forms.Padding(3);
			this.sceneObjectEdit.Size = new System.Drawing.Size(205, 653);
			this.sceneObjectEdit.TabIndex = 0;
			this.sceneObjectEdit.Text = "Thing";
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
			this.thingSelectSplit.Panel1.Controls.Add(this.sceneObjects);
			this.thingSelectSplit.Panel1.Controls.Add(this.sceneObjectsMoveUp);
			// 
			// thingSelectSplit.Panel2
			// 
			this.thingSelectSplit.Panel2.Controls.Add(this.sceneObjectsMoveDown);
			this.thingSelectSplit.Panel2.Controls.Add(this.searchScene);
			this.thingSelectSplit.Size = new System.Drawing.Size(199, 647);
			this.thingSelectSplit.SplitterDistance = 178;
			this.thingSelectSplit.TabIndex = 3;
			// 
			// sceneObjects
			// 
			this.sceneObjects.AllowDrop = true;
			this.sceneObjects.BackColor = System.Drawing.Color.Black;
			this.sceneObjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sceneObjects.ForeColor = System.Drawing.Color.White;
			this.sceneObjects.LabelEdit = true;
			this.sceneObjects.Location = new System.Drawing.Point(0, 23);
			this.sceneObjects.Name = "sceneObjects";
			this.sceneObjects.Size = new System.Drawing.Size(199, 155);
			this.sceneObjects.TabIndex = 1;
			this.sceneObjects.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.OnTreeObjectRename);
			this.sceneObjects.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.OnSceneTreeObjectDrag);
			this.sceneObjects.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnSceneObjectClick);
			this.sceneObjects.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnSceneObjectDoubleClick);
			this.sceneObjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragDrop);
			this.sceneObjects.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragEnter);
			this.sceneObjects.DragOver += new System.Windows.Forms.DragEventHandler(this.OnTreeObjectDragOver);
			// 
			// sceneObjectsMoveUp
			// 
			this.sceneObjectsMoveUp.BackColor = System.Drawing.Color.Black;
			this.sceneObjectsMoveUp.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneObjectsMoveUp.Location = new System.Drawing.Point(0, 0);
			this.sceneObjectsMoveUp.Name = "sceneObjectsMoveUp";
			this.sceneObjectsMoveUp.Size = new System.Drawing.Size(199, 23);
			this.sceneObjectsMoveUp.TabIndex = 3;
			this.sceneObjectsMoveUp.Text = "^";
			this.sceneObjectsMoveUp.UseVisualStyleBackColor = false;
			this.sceneObjectsMoveUp.Click += new System.EventHandler(this.OnObjectsSelectMoveUp);
			// 
			// sceneObjectsMoveDown
			// 
			this.sceneObjectsMoveDown.BackColor = System.Drawing.Color.Black;
			this.sceneObjectsMoveDown.Dock = System.Windows.Forms.DockStyle.Top;
			this.sceneObjectsMoveDown.Location = new System.Drawing.Point(0, 0);
			this.sceneObjectsMoveDown.Name = "sceneObjectsMoveDown";
			this.sceneObjectsMoveDown.Size = new System.Drawing.Size(199, 23);
			this.sceneObjectsMoveDown.TabIndex = 4;
			this.sceneObjectsMoveDown.Text = "v";
			this.sceneObjectsMoveDown.UseVisualStyleBackColor = false;
			this.sceneObjectsMoveDown.Click += new System.EventHandler(this.OnObjectsSelectMoveDown);
			// 
			// searchScene
			// 
			this.searchScene.BackColor = System.Drawing.Color.Black;
			this.searchScene.ForeColor = System.Drawing.Color.White;
			this.searchScene.Location = new System.Drawing.Point(3, 29);
			this.searchScene.Name = "searchScene";
			this.searchScene.Size = new System.Drawing.Size(196, 23);
			this.searchScene.TabIndex = 2;
			this.searchScene.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownObjectSearch);
			// 
			// assetsTab
			// 
			this.assetsTab.BackColor = System.Drawing.Color.Black;
			this.assetsTab.Controls.Add(this.assets);
			this.assetsTab.ForeColor = System.Drawing.Color.White;
			this.assetsTab.Location = new System.Drawing.Point(4, 24);
			this.assetsTab.Name = "assetsTab";
			this.assetsTab.Padding = new System.Windows.Forms.Padding(3);
			this.assetsTab.Size = new System.Drawing.Size(205, 653);
			this.assetsTab.TabIndex = 1;
			this.assetsTab.Text = "Assets";
			// 
			// assets
			// 
			this.assets.BackColor = System.Drawing.Color.Black;
			this.assets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assets.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.assets.Location = new System.Drawing.Point(3, 3);
			this.assets.Name = "assets";
			treeNode1.Checked = true;
			treeNode1.Name = "Node0";
			treeNode1.Text = "folder";
			this.assets.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.assets.Size = new System.Drawing.Size(199, 647);
			this.assets.TabIndex = 0;
			// 
			// save
			// 
			this.save.DefaultExt = "cdb";
			this.save.FileName = "myScene";
			this.save.Filter = "CastleDB files|*.cdb";
			this.save.InitialDirectory = ".";
			this.save.Title = "Save Scene";
			// 
			// FormWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(1264, 681);
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
			this.sceneRightClickMenu.ResumeLayout(false);
			this.sceneStatusPanel.ResumeLayout(false);
			this.gridGroup.ResumeLayout(false);
			this.gridGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).EndInit();
			this.rightTabsPanel.ResumeLayout(false);
			this.sceneObjectEdit.ResumeLayout(false);
			this.thingSelectSplit.Panel1.ResumeLayout(false);
			this.thingSelectSplit.Panel2.ResumeLayout(false);
			this.thingSelectSplit.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.thingSelectSplit)).EndInit();
			this.thingSelectSplit.ResumeLayout(false);
			this.assetsTab.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer windowSplit;
		private VScrollBar sceneZoom;
		private HScrollBar sceneAngle;
		private Panel sceneStatusPanel;
		private GroupBox gridGroup;
		private TrackBar gridThickness;
		private MaskedTextBox gridSpacing;
		private Label sceneMousePos;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripSeparator sceneRightClickMenuSeparator1;
		private ToolStripMenuItem sceneRightClickMenuResetView;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
		private CheckBox gridSnap;
		private TreeView sceneObjects;
		private TabControl rightTabsPanel;
		private TabPage sceneObjectEdit;
		private TabPage assetsTab;
		private MaskedTextBox searchScene;
		private SplitContainer thingSelectSplit;
		private Button sceneObjectsMoveDown;
		private Button sceneObjectsMoveUp;
		private SaveFileDialog save;
		private Button button1;
		private TreeView assets;
		private Label fps;
	}
}