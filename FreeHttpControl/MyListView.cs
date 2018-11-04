﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FreeHttp.FreeHttpControl
{
    public class MyListView : ListView
    { 
        private const int WM_LBUTTONDBLCLK = 0x0203;  //左键双击
        private int moveItemIndex = -1;
        public MyListView() : base() 
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }
        protected override void WndProc(ref Message m)
        { 
            if (m.Msg == WM_LBUTTONDBLCLK)
            { 
                Point p = PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y)); 
                ListViewItem lvi = GetItemAt(p.X, p.Y); 
                if (lvi != null) 
                    lvi.Selected = true; 
                OnDoubleClick(new EventArgs()); 
            } 
            else 
                base.WndProc(ref m); 
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MyListView
            // 
            this.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.MyListView_ItemDrag);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MyListView_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MyListView_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.MyListView_DragOver);
            this.DragLeave += new System.EventHandler(this.MyListView_DragLeave);
            this.ResumeLayout(false);

        }

        private bool AppearAboveItem(int nowIndex)
        {
            if (nowIndex < moveItemIndex)
            {
                return true;
            }
            return false;
        }

        private void MyListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (this.SelectedItems!=null && this.SelectedItems.Count>0)
            {
                moveItemIndex = this.SelectedItems[0].Index;
                this.DoDragDrop(this.SelectedItems, DragDropEffects.Move);
            }
        }

        private void MyListView_DragDrop(object sender, DragEventArgs e)
        {
            int targetIndex = this.InsertionMark.Index;
            if (targetIndex == -1)
            {
                return;
            }
            SelectedListViewItemCollection draggedItems =(SelectedListViewItemCollection)e.Data.GetData(typeof(SelectedListViewItemCollection));
            foreach (ListViewItem draggedItem in draggedItems)
            {
                this.Items.Remove(draggedItem);
                this.Items.Insert(targetIndex, draggedItem);
                if (AppearAboveItem(targetIndex))
                {
                    targetIndex++;
                }
            }

        }

        private void MyListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void MyListView_DragOver(object sender, DragEventArgs e)
        {
            Point targetPoint = this.PointToClient(new Point(e.X, e.Y));
            int targetIndex = this.InsertionMark.NearestIndex(targetPoint);
            System.Diagnostics.Debug.WriteLine(targetIndex.ToString() + this.InsertionMark.AppearsAfterItem.ToString());
            if (targetIndex > -1)
            {
                this.InsertionMark.Color = Color.PowderBlue;
            }
            //Rectangle itemBounds = myListView.GetItemRect(targetIndex);
            //myListView.InsertionMark.AppearsAfterItem = (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2));
            this.InsertionMark.AppearsAfterItem = (!AppearAboveItem(targetIndex));
            this.InsertionMark.Index = targetIndex;
        }

        private void MyListView_DragLeave(object sender, EventArgs e)
        {
            this.InsertionMark.Index = -1;
        }
    }
}