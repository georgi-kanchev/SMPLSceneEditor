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
			this.fps = new System.Windows.Forms.Label();
			this.sceneMousePos = new System.Windows.Forms.Label();
			this.sceneStatusPanel = new System.Windows.Forms.Panel();
			this.searchBox = new System.Windows.Forms.GroupBox();
			this.searchScene = new System.Windows.Forms.MaskedTextBox();
			this.loadButton = new System.Windows.Forms.Button();
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
			this.selectThingTip = new System.Windows.Forms.Label();
			this.rightTable = new System.Windows.Forms.TableLayoutPanel();
			this.thingTypesTable = new System.Windows.Forms.TableLayoutPanel();
			this.thingButtonsTable = new System.Windows.Forms.TableLayoutPanel();
			this.gameDir = new System.Windows.Forms.FolderBrowserDialog();
			this.load = new System.Windows.Forms.OpenFileDialog();
			this.save = new System.Windows.Forms.SaveFileDialog();
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
			this.windowSplit.Panel1.Controls.Add(this.fps);
			this.windowSplit.Panel1.Controls.Add(this.sceneMousePos);
			this.windowSplit.Panel1.Controls.Add(this.sceneStatusPanel);
			this.windowSplit.Panel1.Controls.Add(this.windowPicture);
			// 
			// windowSplit.Panel2
			// 
			this.windowSplit.Panel2.Controls.Add(this.selectThingTip);
			this.windowSplit.Panel2.Controls.Add(this.rightTable);
			this.windowSplit.Size = new System.Drawing.Size(1564, 761);
			this.windowSplit.SplitterDistance = 1205;
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
			this.sceneStatusPanel.Controls.Add(this.searchBox);
			this.sceneStatusPanel.Controls.Add(this.loadButton);
			this.sceneStatusPanel.Controls.Add(this.editSelectionGroup);
			this.sceneStatusPanel.Controls.Add(this.saveButton);
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
			this.searchBox.ForeColor = System.Drawing.Color.White;
			this.searchBox.Location = new System.Drawing.Point(365, 1);
			this.searchBox.Name = "searchBox";
			this.searchBox.Size = new System.Drawing.Size(142, 53);
			this.searchBox.TabIndex = 9;
			this.searchBox.TabStop = false;
			this.searchBox.Text = "Search Thing";
			// 
			// searchScene
			// 
			this.searchScene.BackColor = System.Drawing.Color.Black;
			this.searchScene.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchScene.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.searchScene.ForeColor = System.Drawing.Color.White;
			this.searchScene.Location = new System.Drawing.Point(3, 19);
			this.searchScene.Name = "searchScene";
			this.searchScene.Size = new System.Drawing.Size(136, 26);
			this.searchScene.TabIndex = 2;
			this.searchScene.TabStop = false;
			this.searchScene.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDownObjectSearch);
			// 
			// loadButton
			// 
			this.loadButton.BackColor = System.Drawing.Color.Black;
			this.loadButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.loadButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.loadButton.ForeColor = System.Drawing.Color.White;
			this.loadButton.Location = new System.Drawing.Point(1043, 0);
			this.loadButton.Name = "loadButton";
			this.loadButton.Size = new System.Drawing.Size(77, 54);
			this.loadButton.TabIndex = 7;
			this.loadButton.TabStop = false;
			this.loadButton.Text = "Load";
			this.loadButton.UseVisualStyleBackColor = false;
			this.loadButton.Click += new System.EventHandler(this.OnLoadClick);
			// 
			// editSelectionGroup
			// 
			this.editSelectionGroup.Controls.Add(this.editSelectionOptions);
			this.editSelectionGroup.Controls.Add(this.snap);
			this.editSelectionGroup.ForeColor = System.Drawing.Color.White;
			this.editSelectionGroup.Location = new System.Drawing.Point(174, 0);
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
            "    Reposition",
            "       Rotate"});
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
			// saveButton
			// 
			this.saveButton.BackColor = System.Drawing.Color.Black;
			this.saveButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.saveButton.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.saveButton.ForeColor = System.Drawing.Color.White;
			this.saveButton.Location = new System.Drawing.Point(1120, 0);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(77, 54);
			this.saveButton.TabIndex = 8;
			this.saveButton.TabStop = false;
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
			// selectThingTip
			// 
			this.selectThingTip.BackColor = System.Drawing.Color.Black;
			this.selectThingTip.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selectThingTip.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.selectThingTip.ForeColor = System.Drawing.Color.White;
			this.selectThingTip.Location = new System.Drawing.Point(0, 0);
			this.selectThingTip.Name = "selectThingTip";
			this.selectThingTip.Size = new System.Drawing.Size(351, 757);
			this.selectThingTip.TabIndex = 6;
			this.selectThingTip.Text = "Select a {Thing} to Edit";
			this.selectThingTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// rightTable
			// 
			this.rightTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
			this.rightTable.ColumnCount = 1;
			this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.rightTable.Controls.Add(this.thingTypesTable, 0, 0);
			this.rightTable.Controls.Add(this.thingButtonsTable, 0, 2);
			this.rightTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rightTable.Location = new System.Drawing.Point(0, 0);
			this.rightTable.Name = "rightTable";
			this.rightTable.RowCount = 3;
			this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.921569F));
			this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.17647F));
			this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.901961F));
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
			this.thingTypesTable.Size = new System.Drawing.Size(339, 23);
			this.thingTypesTable.TabIndex = 1;
			// 
			// thingButtonsTable
			// 
			this.thingButtonsTable.ColumnCount = 2;
			this.thingButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.thingButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.thingButtonsTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.thingButtonsTable.Location = new System.Drawing.Point(6, 720);
			this.thingButtonsTable.Name = "thingButtonsTable";
			this.thingButtonsTable.RowCount = 1;
			this.thingButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.thingButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.thingButtonsTable.Size = new System.Drawing.Size(339, 31);
			this.thingButtonsTable.TabIndex = 2;
			// 
			// gameDir
			// 
			this.gameDir.Description = "Select a Game Directory";
			this.gameDir.UseDescriptionForTitle = true;
			// 
			// load
			// 
			this.load.Filter = "Scene|*.scene";
			this.load.Title = "Load Scene";
			// 
			// save
			// 
			this.save.DefaultExt = "scene";
			this.save.FileName = "myScene";
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
		private Label sceneMousePos;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripSeparator sceneRightClickMenuSeparator1;
		private ToolStripMenuItem sceneRightClickMenuResetView;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
		private MaskedTextBox searchScene;
		private Button saveButton;
		private Label fps;
		private ListBox editSelectionOptions;
		private NumericUpDown snap;
		private NumericUpDown gridSpacing;
		private PictureBox windowPicture;
		private GroupBox editSelectionGroup;
		private Button loadButton;
		private FolderBrowserDialog gameDir;
		private OpenFileDialog load;
		private SaveFileDialog save;
		private GroupBox searchBox;
		private Label selectThingTip;
		private TableLayoutPanel rightTable;
		private TableLayoutPanel thingTypesTable;
		private TableLayoutPanel thingButtonsTable;
	}
}