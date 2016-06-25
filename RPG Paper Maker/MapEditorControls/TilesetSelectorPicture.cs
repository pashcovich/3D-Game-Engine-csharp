﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG_Paper_Maker
{
    
    class TilesetSelectorPicture : PictureBox
    {
        protected SelectionRectangle SelectionRectangle;
        public InterpolationMode InterpolationMode { get; set; }
        protected Image TexCursor;


        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        public TilesetSelectorPicture()
        {
            
            int BORDER_SIZE = 4;
            try
            {
                using (FileStream stream = new FileStream(Path.Combine("Config", "bmp", "tileset_cursor.png"), FileMode.Open, FileAccess.Read))
                {
                    TexCursor = Image.FromStream(stream);
                }
            }
            catch { }
            SelectionRectangle = new SelectionRectangle(32, 32, WANOK.BASIC_SQUARE_SIZE*3, WANOK.BASIC_SQUARE_SIZE*1, BORDER_SIZE);
        }

        // -------------------------------------------------------------------
        // SetCursorRealPosition
        // -------------------------------------------------------------------

        public void SetCursorRealPosition()
        {
            SelectionRectangle.SetRealPosition();
        }

        // -------------------------------------------------------------------
        // LoadTexture
        // -------------------------------------------------------------------

        public void LoadTexture(string path)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Image = Image.FromStream(stream);
                }
            }
            catch { }

            Size = new Size((int)(Image.Width * WANOK.RELATION_SIZE), (int)(Image.Height * WANOK.RELATION_SIZE));
            Location = new Point(0, 0); 
        }

        // -------------------------------------------------------------------
        // GetCurrentTexture
        // -------------------------------------------------------------------

        public int[] GetCurrentTexture()
        {
            return SelectionRectangle.GetRectangleArray();
        }

        // -------------------------------------------------------------------
        // MakeFirstRectangleSelection
        // -------------------------------------------------------------------

        public void MakeFirstRectangleSelection(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height) SelectionRectangle.SetRectangle(x, y, 1, 1);
        }

        // -------------------------------------------------------------------
        // MakeRectangleSelection
        // -------------------------------------------------------------------

        public void MakeRectangleSelection(int x, int y)
        {
            if (x < 0) x = 0;
            if (x >= Width) x = Width - 1;
            int init_pos_x = SelectionRectangle.X / WANOK.BASIC_SQUARE_SIZE;
            int pos_x = x / WANOK.BASIC_SQUARE_SIZE;
            int i_x = init_pos_x <= pos_x ? 1 : -1;
            int width = (pos_x - init_pos_x) + i_x;
            SelectionRectangle.Width = width * WANOK.BASIC_SQUARE_SIZE;

            if (y < 0) y = 0;
            if (y >= Height) y = Height - 1;
            int init_pos_y = SelectionRectangle.Y / WANOK.BASIC_SQUARE_SIZE;
            int pos_y = y / WANOK.BASIC_SQUARE_SIZE;
            int i_y = init_pos_y <= pos_y ? 1 : -1;
            int height = (pos_y - init_pos_y) + i_y;
            SelectionRectangle.Height = height * WANOK.BASIC_SQUARE_SIZE;
        }

        // -------------------------------------------------------------------
        // OnPaint
        // -------------------------------------------------------------------

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode;
            Graphics g = e.Graphics;

            base.OnPaint(e);

            try
            {
                SelectionRectangle.Draw(g, TexCursor);
            }
            catch { }
        }
    }
}