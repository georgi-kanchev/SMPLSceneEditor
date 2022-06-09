global using System.Collections.ObjectModel;
global using System.Numerics;
global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;
global using SMPL;
global using SMPL.Tools;
global using Color = SFML.Graphics.Color;

namespace SMPLSceneEditor
{
	public partial class FormWindow : Form
	{
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene;
		private Vector2 prevFormsMousePos, prevFormsMousePosGrid, selectStartPos, rightClickPos;
		private readonly List<string> selectedUIDs = new();

		public FormWindow()
		{
			InitializeComponent();

			WindowState = FormWindowState.Maximized;

			window = new(windowSplit.Panel1.Handle);

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			SetView();
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
			UpdateFPS();

			TrySelect();

			ThingManager.UpdateAllThings();
			ThingManager.DrawAllVisuals(window);

			TryDrawSelection();

			window.Display();
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
		private float GetGridSpacing()
		{
			return MathF.Max(gridSpacing.Text.ToNumber(), 8);
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

				new Line(topL, topR).Draw(window, outCol);
				new Line(topR, botR).Draw(window, outCol);
				new Line(botR, botL).Draw(window, outCol);
				new Line(botL, topL).Draw(window, outCol);

				window.Draw(fill, PrimitiveType.Quads);
			}
		}

		private static Hitbox? GetHitbox(string uid)
		{
			return ThingManager.HasGet(uid, "Hitbox") == false ? default : (Hitbox)ThingManager.Get(uid, "Hitbox");
		}
		private Vector2 GetFormsMousePos()
		{
			var view = window.GetView();
			var scale = view.Size.ToSystem() / new Vector2(windowSplit.Panel1.Width, windowSplit.Panel1.Height);
			return new Vector2(MousePosition.X, MousePosition.Y) * scale;
		}
		private Vector2 GetMousePosition()
		{
			var mp = Mouse.GetPosition(window);
			var mp2 = window.MapPixelToCoords(new(mp.X, mp.Y), window.GetView());
			return new Vector2(mp2.X, mp2.Y);
		}

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
			sceneAngle.Value = (int)angle.Map(0, 360, 0, 100);

			var view = window.GetView();
			view.Rotation = angle;
			window.SetView(view);
		}
		private void SetViewScale(float scale)
		{
			sceneSc = scale;
			sceneZoom.Value = (int)scale.Map(0.1f, 10, 0, 100);
		}
		private void UpdateSceneScale()
		{
			sceneSc = ((float)sceneZoom.Value).Map(0, 100, 0.1f, 10);
		}

		private Vector2 Drag(Vector2 point, bool reverse = false, bool snapToGrid = false)
		{
			var view = window.GetView();
			var prev = snapToGrid ? prevFormsMousePosGrid : prevFormsMousePos;
			var pos = GetFormsMousePos();
			var gridSp = new Vector2(GetGridSpacing());

			if(snapToGrid)
				pos = pos.PointToGrid(gridSp) + gridSp * 0.5f;

			var dist = prev.DistanceBetweenPoints(pos);
			var ang = prev.AngleBetweenPoints(pos);

			if(reverse)
				dist *= -1;

			return dist == 0 ? point : point.PointMoveAtAngle(view.Rotation + ang, dist, false);
		}
		private static void TryTransformHitbox(string uid)
		{
			if(ThingManager.HasGet(uid, "Hitbox") == false)
				return;

			var children = (ReadOnlyCollection<string>)ThingManager.Get(uid, "ChildrenUIDs");
			var hitbox = (Hitbox)ThingManager.Get(uid, "Hitbox");
			hitbox.TransformLocalLines(uid);

			for(int i = 0; i < children.Count; i++)
				TryTransformHitbox(children[i]);
		}
		private void MoveSelectedObject(bool up)
		{
			var selectedNode = sceneObjects.SelectedNode;
			if(selectedNode == null)
				return;

			var nodes = selectedNode.Parent == null ? sceneObjects.Nodes : selectedNode.Parent.Nodes;
			var selectedIndex = nodes.IndexOf(selectedNode);
			if(selectedIndex == -1)
				return;

			nodes.RemoveAt(selectedIndex);
			nodes.Insert(selectedIndex + (up ? -1 : 1), selectedNode);

			sceneObjects.Select();
			sceneObjects.SelectedNode = selectedNode;
		}
		private void SelectObject(string uid)
		{
			if(selectedUIDs.Contains(uid) == false)
				selectedUIDs.Add(uid);
			if(Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
				selectedUIDs.Remove(uid);
		}
		private void UpdateSceneObjectsTree()
		{
			if(selectedUIDs.Count == 0)
				return;

			var node = sceneObjects.Nodes.Find(selectedUIDs[0], true)[0];
			sceneObjects.SelectedNode = node;
			node.EnsureVisible();
			sceneObjects.Select();
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
				if(selectedUIDs.Count == 0)
				{
					var view = window.GetView();
					view.Center = Drag(view.Center.ToSystem(), true).ToSFML();
					window.SetView(view);
				}
				else
				{
					for(int i = 0; i < selectedUIDs.Count; i++)
					{
						var uid = selectedUIDs[i];
						var pos = (Vector2)ThingManager.Get(uid, "LocalPosition");

						ThingManager.Set(uid, "LocalPosition", Drag(pos, false, gridSnap.Checked));
						TryTransformHitbox(uid);
					}
				}

				System.Windows.Forms.Cursor.Current = Cursors.NoMove2D;
			}

			var gridSp = new Vector2(GetGridSpacing());
			prevFormsMousePos = GetFormsMousePos();
			prevFormsMousePosGrid = prevFormsMousePos.PointToGrid(gridSp) + gridSp * 0.5f;
		}
		private void OnSceneZoom(object sender, EventArgs e)
		{
			UpdateSceneScale();
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

			isDragSelecting = true;
			var pos = Mouse.GetPosition(window);
			selectStartPos = new(pos.X, pos.Y);
		}
		private void OnMouseUpScene(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
				rightClickPos = GetMousePosition();

			if(e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
				return;

			isDragSelecting = false;
			UpdateSceneObjectsTree();
		}
		private void OnObjectsSelectMoveUp(object sender, EventArgs e)
		{
			MoveSelectedObject(true);
		}
		private void OnObjectsSelectMoveDown(object sender, EventArgs e)
		{
			MoveSelectedObject(false);
		}
		private void OnSceneObjectClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			selectedUIDs.Clear();
			SelectObject(e.Node.Name);
		}
		private void OnSceneObjectDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			SetViewPosition((Vector2)ThingManager.Get(e.Node.Name, "Position"));
		}
		private void OnSaveClick(object sender, EventArgs e)
		{
			if(save.ShowDialog(this) != DialogResult.OK)
				return;
		}
		private void OnSceneTreeObjectDrag(object sender, ItemDragEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
				return;

			if(e.Item != null)
			{
				var node = (TreeNode)e.Item;
				DoDragDrop(node, DragDropEffects.Move);
			}
		}
		private void OnTreeObjectRename(object sender, NodeLabelEditEventArgs e)
		{
			var uid = e.Node.Name;
			var newUID = e.Label;

			e.CancelEdit = true;

			if(string.IsNullOrWhiteSpace(newUID))
				return;

			if(ThingManager.Exists(newUID))
			{
				MessageBox.Show($"A {{Thing}} already exists with the [UID] '{newUID}'.", "Failed [UID] change!");
				return;
			}

			ThingManager.Set(uid, "UID", newUID);
			e.Node.Text = newUID;
			e.Node.Name = newUID;

			var selectionIndex = selectedUIDs.IndexOf(uid);
			if(selectionIndex == -1)
				return;
			selectedUIDs.Remove(uid);
			selectedUIDs.Insert(selectionIndex, newUID);
		}
		private void OnTreeObjectDragOver(object sender, DragEventArgs e)
		{
			var targetPoint = sceneObjects.PointToClient(new Point(e.X, e.Y));
			sceneObjects.SelectedNode = sceneObjects.GetNodeAt(targetPoint);
		}
		private void OnTreeObjectDragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.AllowedEffect;
		}
		private void OnTreeObjectDragDrop(object sender, DragEventArgs e)
		{
			var targetPoint = sceneObjects.PointToClient(new Point(e.X, e.Y));
			var targetNode = sceneObjects.GetNodeAt(targetPoint);
			var draggedNode = e.Data != null ? (TreeNode)e.Data.GetData(typeof(TreeNode)) : default;

			if(draggedNode != null && targetNode != draggedNode && ContainsNode(draggedNode, targetNode) == false)
			{
				if(e.Effect == DragDropEffects.Move)
				{
					draggedNode.Remove();
					targetNode.Nodes.Add(draggedNode);
					ThingManager.Set(draggedNode.Name, "ParentUID", targetNode.Name);
				}

				targetNode.Expand();
			}

			bool ContainsNode(TreeNode node1, TreeNode node2)
			{
				return node1 == node2.Parent || (node2.Parent != null && ContainsNode(node1, node2.Parent));
			}
		}
		private void OnKeyDownObjectSearch(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Return || string.IsNullOrWhiteSpace(searchScene.Text))
				return;

			var bestGuessSymbolsMatch = 0;
			var bestGuess = default(TreeNode);

			Search(sceneObjects.Nodes);

			if(bestGuess == null)
				return;

			searchScene.Text = "";
			sceneObjects.SelectedNode = bestGuess;
			sceneObjects.Select();
			bestGuess.EnsureVisible();

			void Search(TreeNodeCollection nodes)
			{
				if(nodes == null || searchScene.Text.Length == bestGuessSymbolsMatch)
					return;

				foreach(TreeNode node in nodes)
				{
					var curMatchingSymbols = 0;
					for(int i = 0; i < node.Name.Length; i++)
					{
						if(searchScene.Text.Length <= i || node.Name[i] != searchScene.Text[i])
							break;

						curMatchingSymbols++;
					}

					if(curMatchingSymbols > bestGuessSymbolsMatch)
					{
						bestGuessSymbolsMatch = curMatchingSymbols;
						bestGuess = node;
					}
					Search(node.Nodes);
				}
			}
		}
		private void OnSceneStatusClick(object sender, EventArgs e)
		{
			windowSplit.Panel1.Focus();
		}
		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
		}
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			for(int i = 0; i < 200; i++)
			{
				var uid = ThingManager.CreateSprite($"sprite{i}");
				ThingManager.Set(uid, "Position", rightClickPos);
				ThingManager.Do(uid, "ApplyDefaultHitbox");
				TryTransformHitbox(uid);
				sceneObjects.Nodes.Add(uid, uid);
			}
		}
	}
}