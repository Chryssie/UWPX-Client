﻿using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class OmemoFingerprintControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoFingerprintControlDataTemplate MODEL = new OmemoFingerprintControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public void UpdateView(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is AccountDataTemplate account)
            {
                MODEL.UpdateView(account);
            }
            else
            {
                MODEL.UpdateView(null);
            }
        }

        public void CopyFingerprintToClipboard()
        {
            UiUtils.SetClipboardText(MODEL.Fingerprint);
        }

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
