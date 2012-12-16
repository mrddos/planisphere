using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;

namespace Scada.Controls
{

	public class ListViewEx : ListView
	{
		private ListViewItem.ListViewSubItem _clickedsubitem; //clicked ListViewSubItem
		private ListViewItem _clickeditem; //clicked ListViewItem
		private int _col; //index of double-clicked ListViewSubItem
		private TextBox txtbx; //the default edit control
		private int _sortcol; //index of clicked ColumnHeader
		private Brush _sortcolbrush; //color of items in sorted column
		private Brush _highlightbrush; //color of highlighted items
		private int _cpadding; //padding of the embedded controls


		private const UInt32 LVM_FIRST = 0x1000;
		private const UInt32 LVM_SCROLL = (LVM_FIRST + 20);
		private const int WM_HSCROLL = 0x114;
		private const int WM_VSCROLL = 0x115;
		private const int WM_MOUSEWHEEL = 0x020A;
		private const int WM_PAINT = 0x000F;

		private struct EmbeddedControl
		{
			public Control MyControl;
			public ControlListViewSubItemEx MySubItem;
		}

		private ArrayList _controls;

		[DllImport("user32.dll")]
		private static extern bool SendMessage(IntPtr hWnd, UInt32 m, int wParam, int lParam);

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_PAINT)
			{
				foreach (EmbeddedControl c in _controls)
				{
					Rectangle r = c.MySubItem.Bounds;
					if (r.Y > 0 && r.Y < this.ClientRectangle.Height)
					{
						c.MyControl.Visible = true;
						c.MyControl.Bounds = new Rectangle(r.X + _cpadding, r.Y + _cpadding, r.Width - (2 * _cpadding), r.Height - (2 * _cpadding));
					}
					else
					{
						c.MyControl.Visible = false;
					}
				}
			}
			switch (m.Msg)
			{
				case WM_HSCROLL:
				case WM_VSCROLL:
				case WM_MOUSEWHEEL:
					this.Focus();
					break;
			}
			base.WndProc(ref m);
		}

		private void ScrollMe(int x, int y)
		{
			SendMessage((IntPtr)this.Handle, LVM_SCROLL, x, y);
		}

		public ListViewEx()
		{
			_cpadding = 4;
			_controls = new ArrayList();
			_sortcol = -1;
			_sortcolbrush = SystemBrushes.ControlLight;
			_highlightbrush = SystemBrushes.Highlight;
			this.OwnerDraw = true;
			this.FullRowSelect = true;
			this.View = View.Details;
			this.MouseDown += new MouseEventHandler(this_MouseDown);
			this.MouseDoubleClick += new MouseEventHandler(this_MouseDoubleClick);
			this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(this_DrawColumnHeader);
			this.DrawSubItem += new DrawListViewSubItemEventHandler(this_DrawSubItem);
			this.MouseMove += new MouseEventHandler(this_MouseMove);
			this.ColumnClick += new ColumnClickEventHandler(this_ColumnClick);
			txtbx = new TextBox();
			txtbx.Visible = false;
			this.Controls.Add(txtbx);
			txtbx.Leave += new EventHandler(c_Leave);
			txtbx.KeyPress += new KeyPressEventHandler(txtbx_KeyPress);
		}

		public void AddControlToSubItem(Control control, ControlListViewSubItemEx subitem)
		{
			this.Controls.Add(control);
			subitem.MyControl = control;
			EmbeddedControl ec;
			ec.MyControl = control;
			ec.MySubItem = subitem;
			this._controls.Add(ec);
		}

		public void RemoveControlFromSubItem(ControlListViewSubItemEx subitem)
		{
			Control c = subitem.MyControl;
			for (int i = 0; i < this._controls.Count; i++)
			{
				if (((EmbeddedControl)this._controls[i]).MySubItem == subitem)
				{
					this._controls.RemoveAt(i);
					subitem.MyControl = null;
					this.Controls.Remove(c);
					c.Dispose();
					return;
				}
			}
		}

		public int ControlPadding
		{
			get { return _cpadding; }
			set { _cpadding = value; }
		}

		public Brush MySortBrush
		{
			get { return _sortcolbrush; }
			set { _sortcolbrush = value; }
		}

		public Brush MyHighlightBrush
		{
			get { return _highlightbrush; }
			set { _highlightbrush = value; }
		}

		private void txtbx_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Return)
			{
				_clickedsubitem.Text = txtbx.Text;
				txtbx.Visible = false;
				_clickeditem.Tag = null;
			}
		}

		private void c_Leave(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			_clickedsubitem.Text = c.Text;
			c.Visible = false;
			_clickeditem.Tag = null;
		}

		private void this_MouseDown(object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo lstvinfo = this.HitTest(e.X, e.Y);
			ListViewItem.ListViewSubItem subitem = lstvinfo.SubItem;
			if (subitem == null) return;
			int subx = subitem.Bounds.Left;
			if (subx < 0)
			{
				this.ScrollMe(subx, 0);
			}
		}

		private void this_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewItemEx lstvItem = this.GetItemAt(e.X, e.Y) as ListViewItemEx;
			if (lstvItem == null) return;
			_clickeditem = lstvItem;
			int x = lstvItem.Bounds.Left;
			int i;
			for (i = 0; i < this.Columns.Count; i++)
			{
				x = x + this.Columns[i].Width;
				if (x > e.X)
				{
					x = x - this.Columns[i].Width;
					_clickedsubitem = lstvItem.SubItems[i];
					_col = i;
					break;
				}
			}
			if (!(this.Columns[i] is ColumnHeaderEx)) return;
			ColumnHeaderEx col = (ColumnHeaderEx)this.Columns[i];
			if (col.GetType() == typeof(EditableColumnHeaderEx))
			{
				EditableColumnHeaderEx editcol = (EditableColumnHeaderEx)col;
				if (editcol.MyControl != null)
				{
					Control c = editcol.MyControl;
					if (c.Tag != null)
					{
						this.Controls.Add(c);
						c.Tag = null;
						if (c is ComboBox)
						{
							((ComboBox)c).SelectedValueChanged += new EventHandler(cmbx_SelectedValueChanged);
						}
						c.Leave += new EventHandler(c_Leave);
					}
					c.Location = new Point(x, this.GetItemRect(this.Items.IndexOf(lstvItem)).Y);
					c.Width = this.Columns[i].Width;
					if (c.Width > this.Width) c.Width = this.ClientRectangle.Width;
					c.Text = _clickedsubitem.Text;
					c.Visible = true;
					c.BringToFront();
					c.Focus();
				}
				else
				{
					txtbx.Location = new Point(x, this.GetItemRect(this.Items.IndexOf(lstvItem)).Y);
					txtbx.Width = this.Columns[i].Width;
					if (txtbx.Width > this.Width) txtbx.Width = this.ClientRectangle.Width;
					txtbx.Text = _clickedsubitem.Text;
					txtbx.Visible = true;
					txtbx.BringToFront();
					txtbx.Focus();
				}
			}
			else if (col.GetType() == typeof(BoolColumnHeaderEx))
			{
				BoolColumnHeaderEx boolcol = (BoolColumnHeaderEx)col;
				if (boolcol.Editable)
				{
					BoolListViewSubItemEx boolsubitem = (BoolListViewSubItemEx)_clickedsubitem;
					if (boolsubitem.BoolValue == true)
					{
						boolsubitem.BoolValue = false;
					}
					else
					{
						boolsubitem.BoolValue = true;
					}
					this.Invalidate(boolsubitem.Bounds);
				}
			}
		}

		private void cmbx_SelectedValueChanged(object sender, EventArgs e)
		{
			if (((Control)sender).Visible == false || _clickedsubitem == null) return;
			if (sender.GetType() == typeof(ComboBoxEx))
			{
				ComboBoxEx excmbx = (ComboBoxEx)sender;
				object item = excmbx.SelectedItem;
				//Is this an combobox item with one image?
				if (item.GetType() == typeof(ComboBoxEx.EXImageItem))
				{
					ComboBoxEx.EXImageItem imgitem = (ComboBoxEx.EXImageItem)item;
					//Is the first column clicked -- in that case it's a ListViewItem
					if (_col == 0)
					{
						if (_clickeditem.GetType() == typeof(ImageListViewItemEx))
						{
							((ImageListViewItemEx)_clickeditem).MyImage = imgitem.MyImage;
						}
						else if (_clickeditem.GetType() == typeof(MultipleImagesListViewItemEx))
						{
							MultipleImagesListViewItemEx imglstvitem = (MultipleImagesListViewItemEx)_clickeditem;
							imglstvitem.MyImages.Clear();
							imglstvitem.MyImages.AddRange(new object[] { imgitem.MyImage });
						}
						//another column than the first one is clicked, so we have a ListViewSubItem
					}
					else
					{
						if (_clickedsubitem.GetType() == typeof(ImageListViewSubItemEx))
						{
							ImageListViewSubItemEx imgsub = (ImageListViewSubItemEx)_clickedsubitem;
							imgsub.MyImage = imgitem.MyImage;
						}
						else if (_clickedsubitem.GetType() == typeof(MultipleImagesListViewSubItemEx))
						{
							MultipleImagesListViewSubItemEx imgsub = (MultipleImagesListViewSubItemEx)_clickedsubitem;
							imgsub.MyImages.Clear();
							imgsub.MyImages.Add(imgitem.MyImage);
							imgsub.MyValue = imgitem.MyValue;
						}
					}
					//or is this a combobox item with multiple images?
				}
				else if (item.GetType() == typeof(ComboBoxEx.EXMultipleImagesItem))
				{
					ComboBoxEx.EXMultipleImagesItem imgitem = (ComboBoxEx.EXMultipleImagesItem)item;
					if (_col == 0)
					{
						if (_clickeditem.GetType() == typeof(ImageListViewItemEx))
						{
							((ImageListViewItemEx)_clickeditem).MyImage = (Image)imgitem.MyImages[0];
						}
						else if (_clickeditem.GetType() == typeof(MultipleImagesListViewItemEx))
						{
							MultipleImagesListViewItemEx imglstvitem = (MultipleImagesListViewItemEx)_clickeditem;
							imglstvitem.MyImages.Clear();
							imglstvitem.MyImages.AddRange(imgitem.MyImages);
						}
					}
					else
					{
						if (_clickedsubitem.GetType() == typeof(ImageListViewSubItemEx))
						{
							ImageListViewSubItemEx imgsub = (ImageListViewSubItemEx)_clickedsubitem;
							if (imgitem.MyImages != null)
							{
								imgsub.MyImage = (Image)imgitem.MyImages[0];
							}
						}
						else if (_clickedsubitem.GetType() == typeof(MultipleImagesListViewSubItemEx))
						{
							MultipleImagesListViewSubItemEx imgsub = (MultipleImagesListViewSubItemEx)_clickedsubitem;
							imgsub.MyImages.Clear();
							imgsub.MyImages.AddRange(imgitem.MyImages);
							imgsub.MyValue = imgitem.MyValue;
						}
					}
				}
			}
			ComboBox c = (ComboBox)sender;
			_clickedsubitem.Text = c.Text;
			c.Visible = false;
			_clickeditem.Tag = null;
		}

		private void this_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = this.GetItemAt(e.X, e.Y);
			if (item != null && item.Tag == null)
			{
				this.Invalidate(item.Bounds);
				item.Tag = "t";
			}
		}

		private void this_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void this_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			e.DrawBackground();
			if (e.ColumnIndex == _sortcol)
			{
				e.Graphics.FillRectangle(_sortcolbrush, e.Bounds);
			}
			if ((e.ItemState & ListViewItemStates.Selected) != 0)
			{
				e.Graphics.FillRectangle(_highlightbrush, e.Bounds);
			}
			int fonty = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(e.SubItem.Font.Height / 2));
			int x = e.Bounds.X + 2;
			if (e.ColumnIndex == 0)
			{
				ListViewItemEx item = (ListViewItemEx)e.Item;
				if (item.GetType() == typeof(ImageListViewItemEx))
				{
					ImageListViewItemEx imageitem = (ImageListViewItemEx)item;
					if (imageitem.MyImage != null)
					{
						Image img = imageitem.MyImage;
						int imgy = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(img.Height / 2));
						e.Graphics.DrawImage(img, x, imgy, img.Width, img.Height);
						x += img.Width + 2;
					}
				}
				e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), x, fonty);
				return;
			}
			ListViewSubItemExBase subitem = e.SubItem as ListViewSubItemExBase;
			if (subitem == null)
			{
				e.DrawDefault = true;
			}
			else
			{
				x = subitem.DoDraw(e, x, this.Columns[e.ColumnIndex] as ColumnHeaderEx);
				e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), x, fonty);
			}
		}

		private void this_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if (this.Items.Count == 0) return;
			for (int i = 0; i < this.Columns.Count; i++)
			{
				this.Columns[i].ImageKey = null;
			}
			for (int i = 0; i < this.Items.Count; i++)
			{
				this.Items[i].Tag = null;
			}
			if (e.Column != _sortcol)
			{
				_sortcol = e.Column;
				this.Sorting = SortOrder.Ascending;
				this.Columns[e.Column].ImageKey = "up";
			}
			else
			{
				if (this.Sorting == SortOrder.Ascending)
				{
					this.Sorting = SortOrder.Descending;
					this.Columns[e.Column].ImageKey = "down";
				}
				else
				{
					this.Sorting = SortOrder.Ascending;
					this.Columns[e.Column].ImageKey = "up";
				}
			}
			if (_sortcol == 0)
			{
				//ListViewItem
				if (this.Items[0].GetType() == typeof(ListViewItemEx))
				{
					//sorting on text
					this.ListViewItemSorter = new ListViewItemComparerText(e.Column, this.Sorting);
				}
				else
				{
					//sorting on value
					this.ListViewItemSorter = new ListViewItemComparerValue(e.Column, this.Sorting);
				}
			}
			else
			{
				//ListViewSubItem
				if (this.Items[0].SubItems[_sortcol].GetType() == typeof(ListViewSubItemExBase))
				{
					//sorting on text
					this.ListViewItemSorter = new ListViewSubItemComparerText(e.Column, this.Sorting);
				}
				else
				{
					//sorting on value
					this.ListViewItemSorter = new ListViewSubItemComparerValue(e.Column, this.Sorting);
				}
			}
		}

		class ListViewSubItemComparerText : System.Collections.IComparer
		{

			private int _col;
			private SortOrder _order;

			public ListViewSubItemComparerText()
			{
				_col = 0;
				_order = SortOrder.Ascending;
			}

			public ListViewSubItemComparerText(int col, SortOrder order)
			{
				_col = col;
				_order = order;
			}

			public int Compare(object x, object y)
			{
				int returnVal = -1;

				string xstr = ((ListViewItem)x).SubItems[_col].Text;
				string ystr = ((ListViewItem)y).SubItems[_col].Text;

				decimal dec_x;
				decimal dec_y;
				DateTime dat_x;
				DateTime dat_y;

				if (Decimal.TryParse(xstr, out dec_x) && Decimal.TryParse(ystr, out dec_y))
				{
					returnVal = Decimal.Compare(dec_x, dec_y);
				}
				else if (DateTime.TryParse(xstr, out dat_x) && DateTime.TryParse(ystr, out dat_y))
				{
					returnVal = DateTime.Compare(dat_x, dat_y);
				}
				else
				{
					returnVal = String.Compare(xstr, ystr);
				}
				if (_order == SortOrder.Descending) returnVal *= -1;
				return returnVal;
			}

		}

		class ListViewSubItemComparerValue : System.Collections.IComparer
		{

			private int _col;
			private SortOrder _order;

			public ListViewSubItemComparerValue()
			{
				_col = 0;
				_order = SortOrder.Ascending;
			}

			public ListViewSubItemComparerValue(int col, SortOrder order)
			{
				_col = col;
				_order = order;
			}

			public int Compare(object x, object y)
			{
				int returnVal = -1;

				string xstr = ((ListViewSubItemExBase)((ListViewItem)x).SubItems[_col]).MyValue;
				string ystr = ((ListViewSubItemExBase)((ListViewItem)y).SubItems[_col]).MyValue;

				decimal dec_x;
				decimal dec_y;
				DateTime dat_x;
				DateTime dat_y;

				if (Decimal.TryParse(xstr, out dec_x) && Decimal.TryParse(ystr, out dec_y))
				{
					returnVal = Decimal.Compare(dec_x, dec_y);
				}
				else if (DateTime.TryParse(xstr, out dat_x) && DateTime.TryParse(ystr, out dat_y))
				{
					returnVal = DateTime.Compare(dat_x, dat_y);
				}
				else
				{
					returnVal = String.Compare(xstr, ystr);
				}
				if (_order == SortOrder.Descending) returnVal *= -1;
				return returnVal;
			}

		}

		class ListViewItemComparerText : System.Collections.IComparer
		{

			private int _col;
			private SortOrder _order;

			public ListViewItemComparerText()
			{
				_col = 0;
				_order = SortOrder.Ascending;
			}

			public ListViewItemComparerText(int col, SortOrder order)
			{
				_col = col;
				_order = order;
			}

			public int Compare(object x, object y)
			{
				int returnVal = -1;

				string xstr = ((ListViewItem)x).Text;
				string ystr = ((ListViewItem)y).Text;

				decimal dec_x;
				decimal dec_y;
				DateTime dat_x;
				DateTime dat_y;

				if (Decimal.TryParse(xstr, out dec_x) && Decimal.TryParse(ystr, out dec_y))
				{
					returnVal = Decimal.Compare(dec_x, dec_y);
				}
				else if (DateTime.TryParse(xstr, out dat_x) && DateTime.TryParse(ystr, out dat_y))
				{
					returnVal = DateTime.Compare(dat_x, dat_y);
				}
				else
				{
					returnVal = String.Compare(xstr, ystr);
				}
				if (_order == SortOrder.Descending) returnVal *= -1;
				return returnVal;
			}

		}

		class ListViewItemComparerValue : System.Collections.IComparer
		{

			private int _col;
			private SortOrder _order;

			public ListViewItemComparerValue()
			{
				_col = 0;
				_order = SortOrder.Ascending;
			}

			public ListViewItemComparerValue(int col, SortOrder order)
			{
				_col = col;
				_order = order;
			}

			public int Compare(object x, object y)
			{
				int returnVal = -1;

				string xstr = ((ListViewItemEx)x).MyValue;
				string ystr = ((ListViewItemEx)y).MyValue;

				decimal dec_x;
				decimal dec_y;
				DateTime dat_x;
				DateTime dat_y;

				if (Decimal.TryParse(xstr, out dec_x) && Decimal.TryParse(ystr, out dec_y))
				{
					returnVal = Decimal.Compare(dec_x, dec_y);
				}
				else if (DateTime.TryParse(xstr, out dat_x) && DateTime.TryParse(ystr, out dat_y))
				{
					returnVal = DateTime.Compare(dat_x, dat_y);
				}
				else
				{
					returnVal = String.Compare(xstr, ystr);
				}
				if (_order == SortOrder.Descending) returnVal *= -1;
				return returnVal;
			}

		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.ResumeLayout(false);

		}

	}

	public class ColumnHeaderEx : ColumnHeader
	{

		public ColumnHeaderEx()
		{

		}

		public ColumnHeaderEx(string text)
		{
			this.Text = text;
		}

		public ColumnHeaderEx(string text, int width)
		{
			this.Text = text;
			this.Width = width;
		}

	}

	public class EditableColumnHeaderEx : ColumnHeaderEx
	{

		private Control _control;

		public EditableColumnHeaderEx()
		{

		}

		public EditableColumnHeaderEx(string text)
		{
			this.Text = text;
		}

		public EditableColumnHeaderEx(string text, int width)
		{
			this.Text = text;
			this.Width = width;
		}

		public EditableColumnHeaderEx(string text, Control control)
		{
			this.Text = text;
			this.MyControl = control;
		}

		public EditableColumnHeaderEx(string text, Control control, int width)
		{
			this.Text = text;
			this.MyControl = control;
			this.Width = width;
		}

		public Control MyControl
		{
			get { return _control; }
			set
			{
				_control = value;
				_control.Visible = false;
				_control.Tag = "not_init";
			}
		}

	}

	public class BoolColumnHeaderEx : ColumnHeaderEx
	{

		private Image _trueimage;
		private Image _falseimage;
		private bool _editable;

		public BoolColumnHeaderEx()
		{
			init();
		}

		public BoolColumnHeaderEx(string text)
		{
			init();
			this.Text = text;
		}

		public BoolColumnHeaderEx(string text, int width)
		{
			init();
			this.Text = text;
			this.Width = width;
		}

		public BoolColumnHeaderEx(string text, Image trueimage, Image falseimage)
		{
			init();
			this.Text = text;
			_trueimage = trueimage;
			_falseimage = falseimage;
		}

		public BoolColumnHeaderEx(string text, Image trueimage, Image falseimage, int width)
		{
			init();
			this.Text = text;
			_trueimage = trueimage;
			_falseimage = falseimage;
			this.Width = width;
		}

		private void init()
		{
			_editable = false;
		}

		public Image TrueImage
		{
			get { return _trueimage; }
			set { _trueimage = value; }
		}

		public Image FalseImage
		{
			get { return _falseimage; }
			set { _falseimage = value; }
		}

		public bool Editable
		{
			get { return _editable; }
			set { _editable = value; }
		}

	}

	public abstract class ListViewSubItemExBase : ListViewItem.ListViewSubItem
	{

		private string _value = "";

		public ListViewSubItemExBase()
		{

		}

		public ListViewSubItemExBase(string text)
		{
			this.Text = text;
		}

		public string MyValue
		{
			get { return _value; }
			set { _value = value; }
		}

		//return the new x coordinate
		public abstract int DoDraw(DrawListViewSubItemEventArgs e, int x, Scada.Controls.ColumnHeaderEx ch);

	}

	public class ListViewSubItemEx : ListViewSubItemExBase
	{

		public ListViewSubItemEx()
		{

		}

		public ListViewSubItemEx(string text)
		{
			this.Text = text;
		}

		public override int DoDraw(DrawListViewSubItemEventArgs e, int x, Scada.Controls.ColumnHeaderEx ch)
		{
			return x;
		}

	}

	public class ControlListViewSubItemEx : ListViewSubItemExBase
	{
		private Control _control;

		public ControlListViewSubItemEx()
		{

		}

		public Control MyControl
		{
			get { return _control; }
			set { _control = value; }
		}

		public override int DoDraw(DrawListViewSubItemEventArgs e, int x, ColumnHeaderEx ch)
		{
			return x;
		}

	}

	public class ImageListViewSubItemEx : ListViewSubItemExBase
	{
		private Image _image;

		public ImageListViewSubItemEx()
		{

		}

		public ImageListViewSubItemEx(string text)
		{
			this.Text = text;
		}

		public ImageListViewSubItemEx(Image image)
		{
			_image = image;
		}

		public ImageListViewSubItemEx(Image image, string value)
		{
			_image = image;
			this.MyValue = value;
		}

		public ImageListViewSubItemEx(string text, Image image, string value)
		{
			this.Text = text;
			_image = image;
			this.MyValue = value;
		}

		public Image MyImage
		{
			get { return _image; }
			set { _image = value; }
		}

		public override int DoDraw(DrawListViewSubItemEventArgs e, int x, Scada.Controls.ColumnHeaderEx ch)
		{
			if (this.MyImage != null)
			{
				Image img = this.MyImage;
				int imgy = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(img.Height / 2));
				e.Graphics.DrawImage(img, x, imgy, img.Width, img.Height);
				x += img.Width + 2;
			}
			return x;
		}

	}

	public class MultipleImagesListViewSubItemEx : ListViewSubItemExBase
	{
		private ArrayList _images;

		public MultipleImagesListViewSubItemEx()
		{

		}

		public MultipleImagesListViewSubItemEx(string text)
		{
			this.Text = text;
		}

		public MultipleImagesListViewSubItemEx(ArrayList images)
		{
			_images = images;
		}

		public MultipleImagesListViewSubItemEx(ArrayList images, string value)
		{
			_images = images;
			this.MyValue = value;
		}

		public MultipleImagesListViewSubItemEx(string text, ArrayList images, string value)
		{
			this.Text = text;
			_images = images;
			this.MyValue = value;
		}

		public ArrayList MyImages
		{
			get { return _images; }
			set { _images = value; }
		}

		public override int DoDraw(DrawListViewSubItemEventArgs e, int x, ColumnHeaderEx ch)
		{
			if (this.MyImages != null && this.MyImages.Count > 0)
			{
				for (int i = 0; i < this.MyImages.Count; i++)
				{
					Image img = (Image)this.MyImages[i];
					int imgy = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(img.Height / 2));
					e.Graphics.DrawImage(img, x, imgy, img.Width, img.Height);
					x += img.Width + 2;
				}
			}
			return x;
		}

	}

	public class BoolListViewSubItemEx : ListViewSubItemExBase
	{
		private bool _value;

		public BoolListViewSubItemEx()
		{

		}

		public BoolListViewSubItemEx(bool val)
		{
			_value = val;
			this.MyValue = val.ToString();
		}

		public bool BoolValue
		{
			get { return _value; }
			set
			{
				_value = value;
				this.MyValue = value.ToString();
			}
		}

		public override int DoDraw(DrawListViewSubItemEventArgs e, int x, ColumnHeaderEx ch)
		{
			BoolColumnHeaderEx boolcol = (BoolColumnHeaderEx)ch;
			Image boolimg;
			if (this.BoolValue == true)
			{
				boolimg = boolcol.TrueImage;
			}
			else
			{
				boolimg = boolcol.FalseImage;
			}
			int imgy = e.Bounds.Y + ((int)(e.Bounds.Height / 2)) - ((int)(boolimg.Height / 2));
			e.Graphics.DrawImage(boolimg, x, imgy, boolimg.Width, boolimg.Height);
			x += boolimg.Width + 2;
			return x;
		}

	}

	public class ListViewItemEx : ListViewItem
	{
		private string _value;

		public ListViewItemEx()
		{

		}

		public ListViewItemEx(string text)
		{
			this.Text = text;
		}

		public string MyValue
		{
			get { return _value; }
			set { _value = value; }
		}

	}

	public class ImageListViewItemEx : ListViewItemEx
	{

		private Image _image;

		public ImageListViewItemEx()
		{

		}

		public ImageListViewItemEx(string text)
		{
			this.Text = text;
		}

		public ImageListViewItemEx(Image image)
		{
			_image = image;
		}

		public ImageListViewItemEx(string text, Image image)
		{
			_image = image;
			this.Text = text;
		}

		public ImageListViewItemEx(string text, Image image, string value)
		{
			this.Text = text;
			_image = image;
			this.MyValue = value;
		}

		public Image MyImage
		{
			get { return _image; }
			set { _image = value; }
		}

	}

	public class MultipleImagesListViewItemEx : ListViewItemEx
	{

		private ArrayList _images;

		public MultipleImagesListViewItemEx()
		{

		}

		public MultipleImagesListViewItemEx(string text)
		{
			this.Text = text;
		}

		public MultipleImagesListViewItemEx(ArrayList images)
		{
			_images = images;
		}

		public MultipleImagesListViewItemEx(string text, ArrayList images)
		{
			this.Text = text;
			_images = images;
		}

		public MultipleImagesListViewItemEx(string text, ArrayList images, string value)
		{
			this.Text = text;
			_images = images;
			this.MyValue = value;
		}

		public ArrayList MyImages
		{
			get { return _images; }
			set { _images = value; }
		}

	}

}