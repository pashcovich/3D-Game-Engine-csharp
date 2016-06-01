﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Paper_Maker
{
    public class KeyboardManager
    {
        private Dictionary<Keys, bool> OnKeyboard = new Dictionary<Keys, bool>();
        private List<Keys> FirstKeyboard = new List<Keys>();


        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        public KeyboardManager()
        {
            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                OnKeyboard[k] = false;
            }
        }

        // -------------------------------------------------------------------
        // InitializeKeyboard
        // -------------------------------------------------------------------

        public void InitializeKeyboard()
        {
            FirstKeyboard = new List<Keys>();
            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                OnKeyboard[k] = false;
            }
        }

        // -------------------------------------------------------------------
        // Set status
        // -------------------------------------------------------------------

        public void SetKeyDownStatus(Keys k)
        {
            FirstKeyboard.Add(k);
            OnKeyboard[k] = true;
        }

        public void SetKeyUpStatus(Keys k)
        {
            FirstKeyboard.Add(k);
            OnKeyboard[k] = false;
        }

        public void Update()
        {
            FirstKeyboard = new List<Keys>();
        }

        // -------------------------------------------------------------------
        // Tests
        // -------------------------------------------------------------------

        public bool IsButtonDown(Keys k)
        {
            return OnKeyboard[k] && FirstKeyboard.Contains(k);
        }

        public bool IsButtonDownRepeat(Keys k)
        {
            return OnKeyboard[k];
        }

        public bool IsButtonUp(Keys k)
        {
            return !OnKeyboard[k] && FirstKeyboard.Contains(k);
        }
    }
}
