﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

namespace RPG_Paper_Maker
{
    public partial class SuperListBox : UserControl
    {
        public List<SuperListItem> ModelList { get { return listBox.Items.Cast<SuperListItem>().ToList(); } }
        public Type DialogKind;
        public Type TypeItem;
        public ListBox[] ListBoxes;
        public SuperListItem CopiedItem = null;
        public int Min, Max;
        public System.Timers.Timer DragTimer = new System.Timers.Timer(20);
        public bool CanDrag = false;

        public SuperListBox()
        {
            InitializeComponent();
        }

        // -------------------------------------------------------------------
        // InitializeListParameters
        // -------------------------------------------------------------------

        public void InitializeListParameters(ListBox[] list, List<SuperListItem> modelList, Type type, Type typeItem, int min, int max)
        {
            ListBoxes = list;
            DialogKind = type;
            TypeItem = typeItem;
            Min = min;
            Max = max;

            listBox.FormattingEnabled = false;
            DragTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoDrag);

            for (int i = 0; i < modelList.Count; i++)
            {
                listBox.Items.Add(modelList[i]);
            }
        }

        // -------------------------------------------------------------------
        // GetListBox
        // -------------------------------------------------------------------

        public ListBox GetListBox()
        {
            return listBox;
        }

        // -------------------------------------------------------------------
        // UnselectAllLists
        // -------------------------------------------------------------------

        public void UnselectAllLists()
        {
            for (int i = 0; i < ListBoxes.Length; i++)
            {
                ListBoxes[i].ClearSelected();
            }
        }

        // -------------------------------------------------------------------
        // EditItem
        // -------------------------------------------------------------------

        public void EditItem()
        {
            // If double clic is opening a window...
            if (DialogKind != null)
            {
                SuperListDialog dialog = (SuperListDialog)Activator.CreateInstance(DialogKind, listBox.Items[listBox.SelectedIndex]);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    int index = listBox.SelectedIndex;
                    listBox.Items.RemoveAt(index);
                    listBox.Items.Insert(index, dialog.GetObject());
                }
            }
            // If the settings are directly on the right panel...
            else
            {

            }
        }

        // -------------------------------------------------------------------
        // CopyItem
        // -------------------------------------------------------------------

        public void CopyItem()
        {
            CopiedItem = ((SuperListItem)listBox.Items[listBox.SelectedIndex]).CreateCopy();
        }

        // -------------------------------------------------------------------
        // PasteItem
        // -------------------------------------------------------------------

        public void PasteItem()
        {
            CopiedItem.Id = ((SuperListItem)listBox.Items[listBox.SelectedIndex]).Id;
            listBox.Items[listBox.SelectedIndex] = CopiedItem;
            CopiedItem = CopiedItem.CreateCopy();
        }

        // -------------------------------------------------------------------
        // DeleteItem
        // -------------------------------------------------------------------

        public void DeleteItem()
        {
            int id = ((SuperListItem)listBox.Items[listBox.SelectedIndex]).Id;
            SuperListItem defaultValue = (SuperListItem)Activator.CreateInstance(TypeItem);
            defaultValue.Id = id;
            listBox.Items[listBox.SelectedIndex] = defaultValue;
        }

        // -------------------------------------------------------------------
        // listBox_MouseDown
        // -------------------------------------------------------------------

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            // If left clic, can drag and drop
            if (e.Button == MouseButtons.Left)
            {
                if (listBox.SelectedItem == null) return;
                if (!DragTimer.Enabled) DragTimer.Start();
            }

            // If right clic, open ContextMenu
            if (e.Button == MouseButtons.Right)
            {
                int index = listBox.IndexFromPoint(e.X, e.Y);
                UnselectAllLists();
                listBox.SelectedIndex = index;
                if (listBox.SelectedIndex != -1)
                {
                    ItemPaste.Enabled = CopiedItem != null;
                    contextMenuStrip.Show(listBox, e.Location);
                }
            }
        }

        // -------------------------------------------------------------------
        // listBox_DoubleClick
        // -------------------------------------------------------------------

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                EditItem();
            }
        }

        // -------------------------------------------------------------------
        // ItemEdit_Click
        // -------------------------------------------------------------------

        private void ItemEdit_Click(object sender, EventArgs e)
        {
            EditItem();
        }

        // -------------------------------------------------------------------
        // ItemCopy_Click
        // -------------------------------------------------------------------

        private void ItemCopy_Click(object sender, EventArgs e)
        {
            CopyItem();
        }

        // -------------------------------------------------------------------
        // ItemPaste_Click
        // -------------------------------------------------------------------

        private void ItemPaste_Click(object sender, EventArgs e)
        {
            PasteItem();
        }

        // -------------------------------------------------------------------
        // ItemDelete_Click
        // -------------------------------------------------------------------

        private void ItemDelete_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }

        // -------------------------------------------------------------------
        // listBox_KeyDown
        // -------------------------------------------------------------------

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                if (e.KeyCode == Keys.Delete) DeleteItem();
                if (e.Control && e.KeyCode == Keys.C) CopyItem();
                if (e.Control && e.KeyCode == Keys.V && CopiedItem != null) PasteItem();
            }
        }

        // -------------------------------------------------------------------
        // DoDrag
        // -------------------------------------------------------------------

        private void DoDrag(object sender, System.Timers.ElapsedEventArgs e)
        {
            CanDrag = true;
            DragTimer.Stop();
        }

        // -------------------------------------------------------------------
        // listBox_DragDrop
        // -------------------------------------------------------------------

        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            Point point = listBox.PointToClient(new Point(e.X, e.Y));
            int newIndex = listBox.IndexFromPoint(point);
            if (newIndex < 0) newIndex = listBox.Items.Count - 1;
            object data = e.Data.GetData(TypeItem);

            listBox.Items.Remove(data);
            listBox.Items.Insert(newIndex, data);

            /*
            if (newIndex < beforeIndex)
            {
                for (int i = 0; i < OrderList.Count; i++) if (OrderList[i] >= newIndex && OrderList[i] < beforeIndex) OrderList[i]++;
            }
            else if (newIndex > beforeIndex)
            {
                for (int i = 0; i < OrderList.Count; i++) if (OrderList[i] > beforeIndex && OrderList[i] <= newIndex) OrderList[i]--;
            }
            OrderList[idItemMoved - 1] = newIndex;
            
            
            for (int i = 0; i < OrderList.Count; i++)
            {
                if (((SuperListItem)listBox.Items[OrderList[i]]).Id != i + 1) throw new Exception();
            }*/
        }

        // -------------------------------------------------------------------
        // listBox_DragOver
        // -------------------------------------------------------------------

        private void listBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        // -------------------------------------------------------------------
        // listBox_MouseUp
        // -------------------------------------------------------------------

        private void listBox_MouseUp(object sender, MouseEventArgs e)
        {
            CanDrag = false;
        }

        // -------------------------------------------------------------------
        // listBox_MouseMove
        // -------------------------------------------------------------------

        private void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (CanDrag)
            {
                listBox.DoDragDrop(listBox.SelectedItem, DragDropEffects.Move);
                CanDrag = false;
            }
        }

        // -------------------------------------------------------------------
        // listBox_MouseEnter
        // -------------------------------------------------------------------

        private void listBox_MouseEnter(object sender, EventArgs e)
        {
            listBox.Focus();
        }

        // -------------------------------------------------------------------
        // button_Click
        // -------------------------------------------------------------------

        private void button_Click(object sender, EventArgs e)
        {
            DialogEnterNumber dialog = new DialogEnterNumber(listBox.Items.Count, Min, Max, ModelList);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Suppress
                if (listBox.Items.Count > dialog.Value)
                {
                    int nb = listBox.Items.Count - dialog.Value;
                    for (int i = 0; i < nb; i++) listBox.Items.RemoveAt(dialog.Value);
                }
                // Add
                else if (listBox.Items.Count < dialog.Value)
                {
                    int nb = dialog.Value - listBox.Items.Count;
                    for (int i = 0; i < nb; i++) {
                        SuperListItem defaultValue = (SuperListItem)Activator.CreateInstance(TypeItem);
                        defaultValue.Id = listBox.Items.Count + 1;
                        listBox.Items.Add(defaultValue);
                    }
                }
            }
        }
    }
}