namespace SMPLSceneEditor
{
	partial class SMPLSceneEditor
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
			this.sceneRightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sceneRightClickMenuCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuCreateSprite = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneRightClickMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.sceneRightClickMenuResetView = new System.Windows.Forms.ToolStripMenuItem();
			this.sceneMousePos = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.gridGroup = new System.Windows.Forms.GroupBox();
			this.gridSpacing = new System.Windows.Forms.MaskedTextBox();
			this.gridThickness = new System.Windows.Forms.TrackBar();
			this.sceneAngle = new System.Windows.Forms.HScrollBar();
			this.sceneZoom = new System.Windows.Forms.VScrollBar();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).BeginInit();
			this.windowSplit.Panel1.SuspendLayout();
			this.windowSplit.SuspendLayout();
			this.sceneRightClickMenu.SuspendLayout();
			this.panel1.SuspendLayout();
			this.gridGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).BeginInit();
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
			this.windowSplit.Panel1.Controls.Add(this.sceneMousePos);
			this.windowSplit.Panel1.Controls.Add(this.panel1);
			this.windowSplit.Panel1.Controls.Add(this.sceneAngle);
			this.windowSplit.Panel1.Controls.Add(this.sceneZoom);
			this.windowSplit.Panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDownScene);
			this.windowSplit.Panel1.MouseEnter += new System.EventHandler(this.OnMouseEnterScene);
			this.windowSplit.Panel1.MouseLeave += new System.EventHandler(this.OnMouseLeaveScene);
			this.windowSplit.Panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveScene);
			this.windowSplit.Panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpScene);
			this.windowSplit.Size = new System.Drawing.Size(800, 450);
			this.windowSplit.SplitterDistance = 629;
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
			// sceneMousePos
			// 
			this.sceneMousePos.AutoSize = true;
			this.sceneMousePos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.sceneMousePos.Dock = System.Windows.Forms.DockStyle.Left;
			this.sceneMousePos.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.sceneMousePos.ForeColor = System.Drawing.Color.White;
			this.sceneMousePos.Location = new System.Drawing.Point(0, 60);
			this.sceneMousePos.Name = "sceneMousePos";
			this.sceneMousePos.Size = new System.Drawing.Size(81, 21);
			this.sceneMousePos.TabIndex = 3;
			this.sceneMousePos.Text = "mousePos";
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.gridGroup);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(612, 60);
			this.panel1.TabIndex = 2;
			// 
			// gridGroup
			// 
			this.gridGroup.Controls.Add(this.gridSpacing);
			this.gridGroup.Controls.Add(this.gridThickness);
			this.gridGroup.Dock = System.Windows.Forms.DockStyle.Left;
			this.gridGroup.ForeColor = System.Drawing.Color.White;
			this.gridGroup.Location = new System.Drawing.Point(0, 0);
			this.gridGroup.Name = "gridGroup";
			this.gridGroup.Size = new System.Drawing.Size(162, 56);
			this.gridGroup.TabIndex = 0;
			this.gridGroup.TabStop = false;
			this.gridGroup.Text = "Grid";
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
			this.gridThickness.Size = new System.Drawing.Size(104, 34);
			this.gridThickness.TabIndex = 0;
			this.gridThickness.TabStop = false;
			this.gridThickness.Value = 2;
			// 
			// sceneAngle
			// 
			this.sceneAngle.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.sceneAngle.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.sceneAngle.LargeChange = 1;
			this.sceneAngle.Location = new System.Drawing.Point(0, 433);
			this.sceneAngle.Name = "sceneAngle";
			this.sceneAngle.Size = new System.Drawing.Size(612, 17);
			this.sceneAngle.TabIndex = 1;
			this.sceneAngle.ValueChanged += new System.EventHandler(this.OnSceneRotate);
			// 
			// sceneZoom
			// 
			this.sceneZoom.Cursor = System.Windows.Forms.Cursors.SizeNS;
			this.sceneZoom.Dock = System.Windows.Forms.DockStyle.Right;
			this.sceneZoom.LargeChange = 1;
			this.sceneZoom.Location = new System.Drawing.Point(612, 0);
			this.sceneZoom.Name = "sceneZoom";
			this.sceneZoom.Size = new System.Drawing.Size(17, 450);
			this.sceneZoom.TabIndex = 0;
			this.sceneZoom.Value = 5;
			this.sceneZoom.ValueChanged += new System.EventHandler(this.OnSceneZoom);
			// 
			// SMPLSceneEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.windowSplit);
			this.Name = "SMPLSceneEditor";
			this.Text = "SMPL Scene Editor";
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDownScene);
			this.MouseEnter += new System.EventHandler(this.OnMouseEnterScene);
			this.MouseLeave += new System.EventHandler(this.OnMouseLeaveScene);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveScene);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUpScene);
			this.windowSplit.Panel1.ResumeLayout(false);
			this.windowSplit.Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.windowSplit)).EndInit();
			this.windowSplit.ResumeLayout(false);
			this.sceneRightClickMenu.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.gridGroup.ResumeLayout(false);
			this.gridGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridThickness)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer windowSplit;
		private VScrollBar sceneZoom;
		private HScrollBar sceneAngle;
		private Panel panel1;
		private GroupBox gridGroup;
		private TrackBar gridThickness;
		private MaskedTextBox gridSpacing;
		private Label sceneMousePos;
		private ContextMenuStrip sceneRightClickMenu;
		private ToolStripMenuItem sceneRightClickMenuCreate;
		private ToolStripSeparator sceneRightClickMenuSeparator1;
		private ToolStripMenuItem sceneRightClickMenuResetView;
		private ToolStripMenuItem sceneRightClickMenuCreateSprite;
	}
}