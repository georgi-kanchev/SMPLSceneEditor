global using System.Numerics;
global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;
global using SMPL;
global using SMPL.Tools;
global using Color = SFML.Graphics.Color;

namespace SMPLSceneEditor
{
	public partial class SMPLSceneEditor : Form
	{
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1;
		private bool isSelecting;
		private Vector2 prevMousePos, selectStartPos, rightClickPos;

		public SMPLSceneEditor()
		{
			InitializeComponent();

			WindowState = FormWindowState.Maximized;

			window = new(windowSplit.Panel1.Handle);
			var view = window.GetView();
			view.Center = new();
			window.SetView(view);

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			ResetView();
		}

		private void OnUpdate(object? sender, EventArgs e)
		{
			window.Size = new((uint)windowSplit.Panel1.Width, (uint)windowSplit.Panel1.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.Clear();
			DrawGrid();
			TryShowMousePos();

			ThingManager.UpdateAllThings();
			ThingManager.DrawAllVisuals(window);

			TryDrawSelection();

			window.Display();
		}

		private void DrawGrid()
		{
			if(gridThickness.Value == gridThickness.Minimum)
				return;

			var cellVerts = new VertexArray(PrimitiveType.Quads);
			var specialCellVerts = new VertexArray(PrimitiveType.Quads);
			var sz = new Vector2(windowSplit.Panel1.Width, windowSplit.Panel1.Height) * sceneSc;
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

			SFML.Graphics.Color GetColor(float coordinate)
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
		private void TryDrawSelection()
		{
			if(isSelecting == false)
				return;

			var mousePos = Mouse.GetPosition(window);
			var topLeft = new Vector2i((int)selectStartPos.X, (int)selectStartPos.Y);
			var botRight = new Vector2i(mousePos.X, mousePos.Y);
			var topRight = new Vector2i(botRight.X, topLeft.Y);
			var botLeft = new Vector2i(topLeft.X, botRight.Y);
			var tl = window.MapPixelToCoords(topLeft);
			var tr = window.MapPixelToCoords(topRight);
			var br = window.MapPixelToCoords(botRight);
			var bl = window.MapPixelToCoords(botLeft);
			var fillCol = new Color(0, 180, 255, 100);
			var outCol = Color.Black;
			var fill = new Vertex[]
			{
				new(tl, fillCol),
				new(tr, fillCol),
				new(br, fillCol),
				new(bl, fillCol),
			};

			DrawLine(new Line(tl.ToSystem(), tr.ToSystem()), outCol);
			DrawLine(new Line(tr.ToSystem(), br.ToSystem()), outCol);
			DrawLine(new Line(br.ToSystem(), bl.ToSystem()), outCol);
			DrawLine(new Line(bl.ToSystem(), tl.ToSystem()), outCol);

			window.Draw(fill, PrimitiveType.Quads);

			void DrawLine(Line line, Color color = default, float width = 4)
			{
				color = color == default ? Color.White : color;

				width /= 2;
				var startLeft = line.A.PointMoveAtAngle(line.Angle - 90, width, false);
				var startRight = line.A.PointMoveAtAngle(line.Angle + 90, width, false);
				var endLeft = line.B.PointMoveAtAngle(line.Angle - 90, width, false);
				var endRight = line.B.PointMoveAtAngle(line.Angle + 90, width, false);

				var vert = new Vertex[]
				{
					new(new(startLeft.X, startLeft.Y), color),
					new(new(startRight.X, startRight.Y), color),
					new(new(endRight.X, endRight.Y), color),
					new(new(endLeft.X, endLeft.Y), color),
				};
				window.Draw(vert, PrimitiveType.Quads);
			}
		}

		private float GetGridSpacing()
		{
			return MathF.Max(gridSpacing.Text.ToNumber(), 8);
		}
		private Vector2 GetMousePosition()
		{
			var mp = Mouse.GetPosition(window);
			var mp2 = window.MapPixelToCoords(new(mp.X, mp.Y), window.GetView());
			return new Vector2(mp2.X, mp2.Y);
		}
		private void UpdateZoom()
		{
			sceneSc = ((float)sceneZoom.Value).Map(0, 100, 0.1f, 10f);
		}
		private void ResetView()
		{
			sceneAngle.Value = 0;
			sceneZoom.Value = 10;
			var view = window.GetView();
			view.Center = new();
			view.Rotation = 0;
			sceneSc = 1;
			UpdateZoom();
		}

		private void OnMouseLeaveScene(object sender, EventArgs e)
		{
			//sceneMousePos.Hide();
		}
		private void OnMouseEnterScene(object sender, EventArgs e)
		{
			//sceneMousePos.Show();
		}
		private void OnMouseMoveScene(object sender, MouseEventArgs e)
		{
			var view = window.GetView();
			var scale = view.Size.ToSystem() / new Vector2(windowSplit.Panel1.Width, windowSplit.Panel1.Height);
			var pos = new Vector2(MousePosition.X, MousePosition.Y) * scale;
			var dist = prevMousePos.DistanceBetweenPoints(pos);
			var ang = prevMousePos.AngleBetweenPoints(pos);
			prevMousePos = pos;

			if(e.Button != MouseButtons.Middle || dist == 0)
				return;

			view.Center = view.Center.ToSystem().PointMoveAtAngle(view.Rotation + ang, -dist, false).ToSFML();
			window.SetView(view);

			System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
		}
		private void OnSceneZoom(object sender, EventArgs e)
		{
			UpdateZoom();
		}
		private void OnSceneRotate(object sender, EventArgs e)
		{
			var view = window.GetView();
			view.Rotation = ((float)sceneAngle.Value).Map(0, 100, 0, 360);
			window.SetView(view);
		}
		private void OnMouseDownScene(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
				return;

			windowSplit.Panel1.Focus();

			isSelecting = true;
			var pos = Mouse.GetPosition(window);
			selectStartPos = new(pos.X, pos.Y);
		}
		private void OnMouseUpScene(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
				rightClickPos = GetMousePosition();

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isSelecting = false;
		}
		private void OnKeyDownTopLeftTabs(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//Hotkey.TryTriggerHotkeys();
		}
		private void OnSceneStatusClick(object sender, EventArgs e)
		{
			windowSplit.Panel1.Focus();
		}

		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			ResetView();
		}
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			var uid = ThingManager.CreateSprite("sprite");
			ThingManager.Set(uid, "Position", rightClickPos);
			ThingManager.Set(uid, "Tint", Color.Red);
		}
	}
}