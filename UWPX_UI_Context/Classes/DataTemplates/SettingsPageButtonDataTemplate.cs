﻿using Shared.Classes;
using System;

namespace UWPX_UI_Context.Classes.DataTemplates
{
    public sealed class SettingsPageButtonDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        private string _Glyph;
        public string Glyph
        {
            get { return _Glyph; }
            set { SetProperty(ref _Glyph, value); }
        }
        private Type _NavTarget;
        public Type NavTarget
        {
            get { return _NavTarget; }
            set { SetProperty(ref _NavTarget, value); }
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
