﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Infrastructure.Common.Windows.ViewModels;
using Common;

namespace Infrastructure.Common.TreeList
{
	public class TreeNodeViewModel : BaseViewModel
	{
		private RootTreeNodeViewModel _root;
		private RootTreeNodeViewModel Root
		{
			get
			{
				if (_root == null && ParentNode != null)
					_root = ParentNode.Root;
				return _root;
			}
		}
		public void AssignToRoot(RootTreeNodeViewModel root)
		{
			_root = root;
			//foreach (var child in Nodes)
			//    child.AssignToTree(Tree);
			//if (ParentNode == null)
			//{
			//    Tree.Rows.Add(this);
			//    CreateChildrenRows();
			//}
		}

		public class TreeItemCollection : ObservableCollectionAdv<TreeNodeViewModel>
		{
			private TreeNodeViewModel _owner;

			public TreeItemCollection(TreeNodeViewModel owner)
			{
				_owner = owner;
			}

			protected override void ClearItems()
			{
				while (Count != 0)
					RemoveAt(Count - 1);
			}
			protected override void InsertItem(int index, TreeNodeViewModel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");

				if (item.ParentNode != _owner)
				{
					if (item.ParentNode != null)
						item.ParentNode.Nodes.Remove(item);
					item.ParentNode = _owner;
					item.Index = index;
					//item._tree = _owner.Tree;
					for (int i = index; i < Count; i++)
						this[i].Index++;
					base.InsertItem(index, item);
				}
			}
			protected override void RemoveItem(int index)
			{
				TreeNodeViewModel item = this[index];
				item.ParentNode = null;
				item.Index = -1;
				item._root = null;
				for (int i = index + 1; i < Count; i++)
					this[i].Index--;
				base.RemoveItem(index);
			}
			protected override void SetItem(int index, TreeNodeViewModel item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				RemoveAt(index);
				InsertItem(index, item);
			}
		}

		protected bool IsRoot { get; set; }
		public int Level
		{
			get
			{
				if (ParentNode == null)
					return 0;
				else
					return ParentNode.Level + 1;
			}
		}
		public int Index { get; protected set; }
		public TreeNodeViewModel ParentNode { get; protected set; }
		public TreeItemCollection Nodes { get; private set; }
		public bool IsExpandable
		{
			get { return Nodes.Count > 0; }
		}

		internal TreeNodeViewModel()
		{
			IsRoot = false;
			_root = null;
			Index = -1;
			Nodes = new TreeItemCollection(this);
			Nodes.CollectionChanged += ChildrenChanged;
		}

		private bool IsVisible
		{
			get
			{
				TreeNodeViewModel node = ParentNode;
				while (node != null)
				{
					if (!node.IsExpanded)
						return false;
					node = node.ParentNode;
				}
				return true;
			}
		}
		private TreeNodeViewModel BottomNode
		{
			get
			{
				if (ParentNode != null)
				{
					if (ParentNode.NextNode != null)
						return ParentNode.NextNode;
					else
						return ParentNode.BottomNode;
				}
				return null;
			}
		}
		private TreeNodeViewModel NextVisibleNode
		{
			get
			{
				if (IsExpanded && Nodes.Count > 0)
					return Nodes[0];
				else
					return NextNode ?? BottomNode;
			}
		}
		private TreeNodeViewModel NextNode
		{
			get
			{
				if (ParentNode != null)
				{
					if (Index < ParentNode.Nodes.Count - 1)
						return ParentNode.Nodes[Index + 1];
				}
				return null;
			}
		}
		private int VisibleChildrenCount
		{
			get { return AllVisibleChildren.Count(); }
		}
		private IEnumerable<TreeNodeViewModel> AllVisibleChildren
		{
			get
			{
				int level = Level;
				TreeNodeViewModel node = this;
				while (true)
				{
					node = node.NextVisibleNode;
					if (node != null && node.Level > level)
						yield return node;
					else
						break;
				}
			}
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (IsExpanded && Root != null)
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						if (e.NewItems != null)
						{
							int index = -1;
							if (e.NewStartingIndex == 0)
								index = Root.Rows.IndexOf(this);
							else
								index = Root.Rows.IndexOf(Nodes[e.NewStartingIndex - 1]) + Nodes[e.NewStartingIndex - 1].VisibleChildrenCount;
							foreach (TreeNodeViewModel node in e.NewItems)
							{
								Root.Rows.Insert(index + 1, node);
								node.CreateChildrenRows();
								index++;
							}
						}
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (TreeNodeViewModel node in e.OldItems)
							node.DropChildrenRows(true);
						break;
					case NotifyCollectionChangedAction.Move:
					case NotifyCollectionChangedAction.Replace:
					case NotifyCollectionChangedAction.Reset:
						DropChildrenRows(false);
						CreateChildrenRows();
						break;
				}
			OnPropertyChanged(() => IsExpandable);
		}
		private void DropChildrenRows(bool removeParent)
		{
			int start = Root.Rows.IndexOf(this);
			if (start >= 0)
			{
				int count = VisibleChildrenCount;
				if (removeParent)
					count++;
				else
					start++;
				Root.Rows.RemoveRange(start, count);
			}
		}
		private void CreateChildrenRows()
		{
			int index = Root.Rows.IndexOf(this);
			if (index >= 0 || IsRoot)
			{
				var nodes = AllVisibleChildren.ToArray();
				Root.Rows.InsertRange(index + 1, nodes);
				//if (nodes.Contains(Tree.SelectedTreeItem))
				//    Tree.ResumeSelection();
			}
		}

		private bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set
			{
				if (value != IsExpanded)
				{
					if (value)
					{
						_isExpanded = true;
						if (Root != null)
						{
							CreateChildrenRows();
							Root.ResumeSelection();
						}
					}
					else
					{
						if (Root != null)
						{
							Root.SuspendSelection();
							DropChildrenRows(false);
						}
						_isExpanded = false;
					}
					OnPropertyChanged(() => IsExpandable);
					OnPropertyChanged(() => IsExpanded);
				}
			}
		}

		public bool IsSelected
		{
			get { return Root == null ? false : Root.SelectedTreeNode == this; }
			set
			{
				if (Root != null)
				{
					ExpandToThis();
					Root.SelectedTreeNode = this;
				}
				OnPropertyChanged(() => IsSelected);
			}
		}
		public bool HasChildren
		{
			get { return Nodes.Count > 0; }
		}

		public void ExpandToThis()
		{
			var parent = this.ParentNode;
			while (parent != null)
			{
				parent.IsExpanded = true;
				parent = parent.ParentNode;
			}
		}
		public void CollapseChildren(bool withSelf = true)
		{
			ProcessAllChildren(this, withSelf, item => item.IsExpanded = false);
		}
		public void ExpandChildren(bool withSelf = true)
		{
			ProcessAllChildren(this, withSelf, item => item.IsExpanded = true);
		}
		private void ProcessAllChildren(TreeNodeViewModel parent, bool withSelf, Action<TreeNodeViewModel> action)
		{
			if (withSelf)
				action(parent);
			foreach (TreeNodeViewModel t in parent.Nodes)
				ProcessAllChildren(t, true, action);
		}
	}

	public class TreeNodeViewModel<T> : TreeNodeViewModel
		where T : TreeNodeViewModel<T>
	{
		public TreeNodeViewModel()
		{
		}
		public TreeNodeViewModel(IEnumerable<T> children)
			: this()
		{
			Nodes.InsertRange(0, children);
		}

		public T Parent
		{
			get { return ParentNode as T; }
		}
		public T this[int index]
		{
			get { return Nodes[index] as T; }
		}
		public void AddChild(T item)
		{
			Nodes.Add(item);
		}
		public void ClearChildren()
		{
			Nodes.Clear();
		}
		public int ChildrenCount
		{
			get { return Nodes.Count; }
		}
		public IEnumerable<T> Children
		{
			get
			{
				foreach (T child in Nodes)
					yield return child;
			}
		}

		public List<T> GetAllParents()
		{
			if (ParentNode == null)
				return new List<T>();
			else
			{
				List<T> allParents = Parent.GetAllParents();
				allParents.Add(Parent);
				return allParents;
			}
		}
	}
}
