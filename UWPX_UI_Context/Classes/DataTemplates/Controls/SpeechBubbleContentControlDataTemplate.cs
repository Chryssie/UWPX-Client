﻿using Data_Manager2.Classes;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes.DBTables;
using Shared.Classes;
using System;
using System.Threading.Tasks;

namespace UWPX_UI_Context.Classes.DataTemplates.Controls
{
    public sealed class SpeechBubbleContentControlDataTemplate : AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }
        private string _NicknameText;
        public string NicknameText
        {
            get { return _NicknameText; }
            set { SetProperty(ref _NicknameText, value); }
        }
        private DateTime _Date;
        public DateTime Date
        {
            get { return _Date; }
            set { SetProperty(ref _Date, value); }
        }
        private bool _IsEncrypted;
        public bool IsEncrypted
        {
            get { return _IsEncrypted; }
            set { SetProperty(ref _IsEncrypted, value); }
        }
        private bool _IsCarbonCopy;
        public bool IsCarbonCopy
        {
            get { return _IsCarbonCopy; }
            set { SetProperty(ref _IsCarbonCopy, value); }
        }
        private string _MessageType;
        public string MessageType
        {
            get { return _MessageType; }
            set { SetProperty(ref _MessageType, value); }
        }
        private bool _IsImage;
        public bool IsImage
        {
            get { return _IsImage; }
            set { SetProperty(ref _IsImage, value); }
        }
        private MessageState _State;
        public MessageState State
        {
            get { return _State; }
            set { SetProperty(ref _State, value); }
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
        public void UpdateView(ChatTable chat, ChatMessageTable message)
        {
            if (chat is null || message is null)
            {
                return;
            }

            Text = message.message;
            IsCarbonCopy = message.isCC;
            IsEncrypted = message.isEncrypted;
            Date = message.date;
            MessageType = message.type;
            IsImage = message.isImage;
            State = message.state;

            if(State == MessageState.UNREAD)
            {
                Task.Run(() => ChatDBManager.INSTANCE.markMessageAsRead(message));
            }

            if (chat.chatType == ChatType.MUC)
            {
                NicknameText = message.fromUser;
            }
            else
            {
                NicknameText = string.Empty;
            }

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
