global using System.Collections.ObjectModel;
global using System.Numerics;
global using SFML.Graphics;
global using SFML.System;
global using SFML.Window;
global using SMPL;
global using SMPL.Tools;
global using Color = SFML.Graphics.Color;
global using Cursor = System.Windows.Forms.Cursor;

namespace SMPLSceneEditor
{
	public partial class FormWindow : Form
	{
		private readonly System.Windows.Forms.Timer loop;
		private readonly RenderWindow window;
		private float sceneSc = 1;
		private int selectDepthIndex;
		private bool isDragSelecting, isHoveringScene, confirmDeleteChildrenMsgShown;
		private Vector2 prevFormsMousePos, prevMousePos, prevFormsMousePosGrid, selectStartPos, rightClickPos;
		private readonly List<string> selectedUIDs = new();
		private readonly Cursor[] editCursors = new Cursor[] { Cursors.NoMove2D, Cursors.Cross, Cursors.SizeAll };

		public FormWindow()
		{
			InitializeComponent();

			WindowState = FormWindowState.Maximized;

			window = new(windowPicture.Handle);
			windowPicture.MouseWheel += OnSceneScroll;

			loop = new() { Interval = 1 };
			loop.Tick += OnUpdate;
			loop.Start();

			SetView();

			editSelectionOptions.SelectedIndex = 0;
			FocusObjectsTree();

			Scene.CurrentScene = new MainScene();
		}

		private void OnUpdate(object? sender, EventArgs e)
		{
			window.Size = new((uint)windowPicture.Width, (uint)windowPicture.Height);

			var view = window.GetView();
			view.Size = new(window.Size.X * sceneSc, window.Size.Y * sceneSc);
			window.SetView(view);

			window.Clear();
			DrawGrid();
			TryShowMousePos();
			UpdateFPS();

			TrySelect();
			TryDestroy();

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
			return MathF.Max(gridSpacing.Text.ToNumber(), (float)gridSpacing.Minimum);
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
					UnselectObjects();

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
					UnselectObjects();
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

				new Line(topL, topR).Draw(window, outCol, sceneSc * 4);
				new Line(topR, botR).Draw(window, outCol, sceneSc * 4);
				new Line(botR, botL).Draw(window, outCol, sceneSc * 4);
				new Line(botL, topL).Draw(window, outCol, sceneSc * 4);

				window.Draw(fill, PrimitiveType.Quads);
			}
		}
		private void TryDestroy()
		{
			var delClick = Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once("delete-selected-objects");

			if(confirmDeleteChildrenMsgShown == false && selectedUIDs.Count > 0 && delClick)
			{
				confirmDeleteChildrenMsgShown = true;
				var result = MessageBox.Show($"Also delete the children of the selected {{Things}}?", "Delete Selection", MessageBoxButtons.YesNoCancel);
				confirmDeleteChildrenMsgShown = false;
				var includeChildren = result == DialogResult.Yes;
				if(result == DialogResult.Cancel)
					return;

				var sel = new List<string>(selectedUIDs);
				for(int i = 0; i < sel.Count; i++)
				{
					var uid = selectedUIDs[i];
					var node = sceneObjects.Nodes.Find(uid, true)[0];
					selectedUIDs.Remove(uid);

					if(includeChildren == false)
						foreach(TreeNode childNode in node.Nodes)
						{
							childNode.Remove();
							sceneObjects.Nodes[0].Nodes.Add(childNode);
						}

					node.Remove();

					ThingManager.Destroy(uid, includeChildren);
				}
			}
		}

		private static Hitbox? GetHitbox(string uid)
		{
			return ThingManager.HasGet(uid, "Hitbox") == false ? default : (Hitbox)ThingManager.Get(uid, "Hitbox");
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
			var view = window.GetView();
			view.Rotation = angle;
			window.SetView(view);
		}
		private void SetViewScale(float scale)
		{
			sceneSc = scale.Limit(0.1f, 10f);
		}

		private Vector2 Drag(Vector2 point, bool reverse = false, bool snapToGrid = false)
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

			return dist == 0 ? point : point.PointMoveAtAngle(view.Rotation + ang, dist, false);
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

			FocusObjectsTree();
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
			FocusObjectsTree();
		}
		private void FocusObjectsTree()
		{
			if(selectedUIDs.Count == 0)
			{
				var world = sceneObjects.Nodes[0];
				sceneObjects.SelectedNode = world;
				world.EnsureVisible();
			}

			sceneObjects.Select();
		}
		private void UnselectObjects()
		{
			selectedUIDs.Clear();
			FocusObjectsTree();
		}
		private static float AngToGrid(float ang, float gridSz)
		{
			return new Vector2(ang).PointToGrid(new(gridSz)).X;
		}
		private void TryLoadFiles(string path)
		{
			var isFolder = Path.HasExtension(path) == false;

			if(isFolder)
			{
				var folderFiles = Directory.GetFiles(path);
				var subFolders = Directory.GetDirectories(path);

				for(int i = 0; i < subFolders.Length; i++)
					TryLoadFiles(subFolders[i]);

				for(int i = 0; i < folderFiles.Length; i++)
					TryLoadFiles(folderFiles[i]);
				return;
			}

			var newPath = Path.GetFileName(path);

			if(File.Exists(newPath))
				File.Delete(newPath);
			File.Copy(path, newPath);
			MainScene.Load(newPath);

			if(assets.Nodes.ContainsKey(newPath) == false)
				assets.Nodes.Add(newPath, newPath);
		}
		private static bool IsNodeDropped(TreeView tree, DragEventArgs e, out TreeNode? targetNode, out TreeNode? draggedNode, out string? prevFullPath)
		{
			var targetPoint = tree.PointToClient(new Point(e.X, e.Y));
			targetNode = tree.GetNodeAt(targetPoint);
			draggedNode = e.Data != null ? (TreeNode)e.Data.GetData(typeof(TreeNode)) : default;
			prevFullPath = null;

			if(targetNode == null)
				return false;

			if(draggedNode != null && targetNode != draggedNode && ContainsNode(draggedNode, targetNode) == false)
			{
				if(e.Effect == DragDropEffects.Move)
				{
					prevFullPath = draggedNode.FullPath;
					draggedNode.Remove();
					targetNode.Nodes.Add(draggedNode);

					tree.SelectedNode = draggedNode;
					tree.Select();
					return true;
				}
				targetNode.Expand();
			}

			return false;

			bool ContainsNode(TreeNode node1, TreeNode node2) => node1 == node2.Parent || (node2.Parent != null && ContainsNode(node1, node2.Parent));
		}

		private void OnTreeViewDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if(e.Node == null)
				return;

			e.Node.BeginEdit();

			if(e.Node.IsExpanded)
				e.Node.Collapse();
			else
				e.Node.Expand();
		}
		private void OnTreeNodeDrag(object sender, ItemDragEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
				return;

			if(e.Item != null)
			{
				var node = (TreeNode)e.Item;
				DoDragDrop(node, DragDropEffects.Move);
			}
		}
		#region Scene
		private void OnSaveClick(object sender, EventArgs e)
		{
			if(save.ShowDialog(this) != DialogResult.OK)
			{
				FocusObjectsTree();
				return;
			}

			FocusObjectsTree();
		}
		private void OnSceneScroll(object? sender, MouseEventArgs e)
		{
			var delta = e.Delta < 0 ? 0.1f : -0.1f;

			var pos = window.GetView().Center.ToSystem();
			var mousePos = GetMousePosition();
			var dist = pos.DistanceBetweenPoints(mousePos);

			if(selectedUIDs.Count == 0)
			{
				if(delta < 0)
				{
					pos = pos.PointMoveTowardPoint(mousePos, -dist * delta * 0.7f, false);
					SetViewPosition(pos);
				}
				SetViewScale(sceneSc + delta);
			}

			for(int i = 0; i < selectedUIDs.Count; i++)
			{
				var uid = selectedUIDs[i];
				var sc = (float)ThingManager.Get(uid, "Scale");

				ThingManager.Set(uid, "Scale", MathF.Max(sc + delta, 0.01f));
				TryTransformHitbox(uid);
			}
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
				var editIndex = editSelectionOptions.SelectedIndex;

				if(selectedUIDs.Count == 0)
				{
					var view = window.GetView();
					var pos = view.Center.ToSystem();

					if(editIndex == 0)
						SetViewPosition(Drag(pos, true));
					else if(editIndex == 1)
						SetViewAngle(DragAngle(pos, view.Rotation, true));
				}
				else
				{
					for(int i = 0; i < selectedUIDs.Count; i++)
					{
						var uid = selectedUIDs[i];
						var pos = (Vector2)ThingManager.Get(uid, "Position");
						var ang = (float)ThingManager.Get(uid, "Angle");


						if(editIndex == 0)
							ThingManager.Set(uid, "Position", Drag(pos, false, true));
						else if(editIndex == 1)
							ThingManager.Set(uid, "Angle", DragAngle(pos, ang));

						TryTransformHitbox(uid);
					}
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
			if(e.Button != MouseButtons.Left)
				return;

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
		private void OnSceneStatusClick(object sender, EventArgs e)
		{
			FocusObjectsTree();
		}
		#endregion
		#region SceneTreeObjects
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
			if(e.Node == sceneObjects.Nodes[0])
				return;

			if(e.Button == MouseButtons.Right)
			{
				SetViewPosition((Vector2)ThingManager.Get(e.Node.Name, "Position"));
				return;
			}

			selectedUIDs.Clear();
			SelectObject(e.Node.Name);
		}
		private void OnTreeObjectRename(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = true;
			if(e.Node == sceneObjects.Nodes[0])
				return;

			var uid = e.Node.Name;
			var newUID = e.Label;

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
		private void SelectObjectsTree(object sender, EventArgs e)
		{
			FocusObjectsTree();
		}
		private void OnUnparentSelectionButtonClick(object sender, EventArgs e)
		{
			sceneObjects.Select();
			var selectedNode = sceneObjects.SelectedNode;
			var selectedNodeParent = selectedNode.Parent;
			if(selectedNodeParent == null || selectedNodeParent == sceneObjects.Nodes[0])
				return;

			selectedNode.Remove();
			sceneObjects.Nodes[0].Nodes.Add(selectedNode);
			sceneObjects.SelectedNode = selectedNode;
			selectedNode.EnsureVisible();
			ThingManager.Set(selectedNode.Name, "ParentUID", null);
		}
		private void OnKeyDownObjectSearch(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode != Keys.Return || string.IsNullOrWhiteSpace(searchScene.Text))
				return;

			var bestGuessSymbolsMatch = 0;
			var bestGuess = default(TreeNode);

			Search(sceneObjects.Nodes[0].Nodes);

			if(bestGuess == null)
				return;

			searchScene.Text = "";
			sceneObjects.SelectedNode = bestGuess;
			FocusObjectsTree();
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

		private void OnTreeObjectDragOver(object sender, DragEventArgs e)
		{
			sceneObjects.SelectedNode = sceneObjects.GetNodeAt(sceneObjects.PointToClient(new Point(e.X, e.Y)));
		}
		private void OnTreeObjectDragEnter(object sender, DragEventArgs e)
		{
			var droppedDataIsNode = e.Data != null && e.Data.GetFormats().ToList().Contains(typeof(TreeNode).ToString());
			if(droppedDataIsNode)
				e.Effect = e.AllowedEffect;
		}
		private void OnTreeObjectDragDrop(object sender, DragEventArgs e)
		{
			if(IsNodeDropped(sceneObjects, e, out var targetNode, out var draggedNode, out _) == false)
				return;

			if(targetNode == null || draggedNode == null)
				return;

			var uid = draggedNode.Name;
			ThingManager.Set(uid, "ParentUID", targetNode == sceneObjects.Nodes[0] ? null : targetNode.Name);
			TryTransformHitbox(uid);

			selectedUIDs.Clear();
			selectedUIDs.Add(draggedNode.Name);
		}
		#endregion
		#region SceneRightClick
		private void OnSceneRightClickMenuResetView(object sender, EventArgs e)
		{
			SetView();
		}
		private void OnSceneRightClickMenuCreateSprite(object sender, EventArgs e)
		{
			var uid = ThingManager.CreateSprite($"sprite");
			ThingManager.Set(uid, "Position", rightClickPos);
			ThingManager.Do(uid, "ApplyDefaultHitbox");
			ThingManager.Set(uid, "TexturePath", "explosive.jpg");
			TryTransformHitbox(uid);

			var world = sceneObjects.Nodes[0];
			world.Nodes.Add(uid, uid);
			world.Expand();
		}

		private void OnAssetRename(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = true;
			var name = e.Node.Name;
			var newName = e.Label;

			if(string.IsNullOrWhiteSpace(newName) || IsInvalidFilename(newName))
				return;

			var isFolder = Path.HasExtension(newName) == false;
			if(isFolder)
				Directory.Move(name, newName);
			else
				File.Move(name, newName);

			e.Node.Text = newName;
			e.Node.Name = newName;

			bool IsInvalidFilename(string fileName)
			{
				var invalidChars = Path.GetInvalidFileNameChars();
				for(int i = 0; i < invalidChars.Length; i++)
					if(fileName.Contains(invalidChars[i]))
						return true;
				return false;
			}
		}
		#endregion

		#region ObjectEditRightTab
		private void OnRightTabSelect(object sender, TabControlEventArgs e)
		{
			if(e.TabPageIndex != 0)
				return;

			FocusObjectsTree();
		}
		#endregion
		#region Assets
		private void ImportAssetsClick(object sender, EventArgs e)
		{
			if(importAssets.ShowDialog(this) != DialogResult.OK)
			{
				FocusObjectsTree();
				return;
			}

			var files = importAssets.FileNames;
			for(int i = 0; i < files.Length; i++)
				TryLoadFiles(files[i]);

			FocusObjectsTree();
		}
		private void OnAssetsCreateFolderClick(object sender, EventArgs e)
		{
			var node = assets.SelectedNode;
			var selectedNodeIsFolder = node != null && Path.HasExtension(node.Name);
			var selectedFolder = "";

			if(selectedNodeIsFolder == false && node != null && node.Parent != null)
				selectedFolder = $"{node.Parent.Name}/";

			var path = selectedFolder + "Folder";
			var i = 1;
			var freePath = path;
			while(Directory.Exists(freePath))
			{
				freePath = $"{path}{i}";
				i++;
			}
			Directory.CreateDirectory(freePath);
			assets.Nodes.Add(freePath, freePath);
		}

		private void OnAssetsDragOver(object sender, DragEventArgs e)
		{
			assets.SelectedNode = assets.GetNodeAt(assets.PointToClient(new Point(e.X, e.Y)));
		}
		private void OnAssetsDragEnter(object sender, DragEventArgs e)
		{
			var droppedDataIsNode = e.Data != null && e.Data.GetFormats().ToList().Contains(typeof(TreeNode).ToString());
			//if(e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
			//	e.Effect = DragDropEffects.Copy;
			if(droppedDataIsNode)
				e.Effect = e.AllowedEffect;
		}
		private void OnAssetsDragDrop(object sender, DragEventArgs e)
		{
			if(e.Data == null)
				return;

			if(IsNodeDropped(assets, e, out var targetNode, out var draggedNode, out var prevFullPath))
			{
				if(targetNode == null || draggedNode == null || prevFullPath == null)
					return;

				var isFolder = Path.HasExtension(prevFullPath) == false;

				if(isFolder)
					Directory.Move(prevFullPath, draggedNode.FullPath);
				else
					File.Move(prevFullPath, draggedNode.FullPath);
				return;
			}

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if(files == null)
				return;

			for(int i = 0; i < files.Length; i++)
			{
				var file = files[i];
				TryLoadFiles(file);
			}

			assets.Sort();
		}
		#endregion
	}
}