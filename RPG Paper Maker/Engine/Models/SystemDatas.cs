﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Paper_Maker
{
    [Serializable]
    public class SystemDatas
    {
        public Dictionary<string,string> GameName;
        public List<string> Langs;
        public string StartMapName = "MAP0001";
        public int[] StartPosition = new int[] { 12, 0, 0, 12 };
        public int ScreenWidth = 640;
        public int ScreenHeight = 480;
        public bool FullScreen = false;
        public int SquareSize = 16;
        public string PathRTP;

        // ListBoxes
        public List<SystemColor> Colors = new List<SystemColor>();


        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        public SystemDatas(string gameName)
        {
            // Game name and langages
            GameName = new Dictionary<string, string>();
            Langs = new List<string>(new string[] { "eng" });
            for (int i = 0; i < Langs.Count; i++)
            {
                GameName[Langs[i]] = gameName;
            }

            // Path RTP
            PathRTP = Path.Combine(WANOK.ExcecutablePathDir, "RTP");

            // Get defaults lists
            Colors = SystemColor.GetDefaultColors();
        }

        // -------------------------------------------------------------------
        // GetColorById
        // -------------------------------------------------------------------

        public SystemColor GetColorById(int id)
        {
            if (id > Colors.Count) return new SystemColor(-1);
            return Colors.Find(i => i.Id == id);
        }

        // -------------------------------------------------------------------
        // GetColorIndexById
        // -------------------------------------------------------------------

        public int GetColorIndexById(int id)
        {
            return Colors.IndexOf(GetColorById(id));
        }

        // -------------------------------------------------------------------
        // NoStart
        // -------------------------------------------------------------------

        public void NoStart()
        {
            StartMapName = "";
            StartPosition = new int[] { 0, 0, 0 };
        }
    }
}
