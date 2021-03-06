﻿using Data_Manager2.Classes.DBTables;
using Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Shared.Classes;
using System;
using Windows.Foundation.Metadata;
using Windows.Phone.Devices.Notification;
using Windows.UI.Notifications;

namespace Data_Manager2.Classes.Toast
{
    public static class ToastHelper
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private const string DEFAULT_MUC_IMAGE_PATH = "Assets/Images/default_muc_image.png";
        private const string DEFAULT_USER_IMAGE_PATH = "Assets/Images/default_user_image.png";
        private const string SEND_BUTTON_IMAGE_PATH = "Assets/Images/send.png";
        private const string SEND_BUTTON_ENCRYPTED_IMAGE_PATH = "Assets/Images/send_encrypted.png";
        public const string TEXT_BOX_ID = "msg_tbx";
        public const string WILL_BE_SEND_LATER_TOAST_GROUP = "will_be_send_later";

        private static readonly TimeSpan VIBRATE_TS = TimeSpan.FromMilliseconds(150);

        public delegate void OnChatMessageToastHandler(OnChatMessageToastEventArgs args);
        /// <summary>
        /// Called before the toast gets toasted.
        /// </summary>
        public static event OnChatMessageToastHandler OnChatMessageToast;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void removeToastGroup(string group)
        {
            ToastNotificationManager.History.RemoveGroup(group);
        }

        public static void showWillBeSendLaterToast(ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Your message will be send when the app is started again.",
                                HintMaxLines = 1
                            }
                        }
                    }
                },
                DisplayTimestamp = DateTime.Now,
            };

            removeToastGroup(WILL_BE_SEND_LATER_TOAST_GROUP);
            popToast(toastContent, chat, WILL_BE_SEND_LATER_TOAST_GROUP);
        }

        public static void showChatTextToast(ChatMessageTable msg, ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = msg.message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Actions = getActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, false).generate()
            };

            popToast(toastContent, chat);
        }

        public static void showChatTextImageToast(ChatMessageTable msg, ChatTable chat)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = chat.chatJabberId,
                                HintMaxLines = 1
                            },
                            new AdaptiveText
                            {
                                Text = "You received an image!"
                            }
                        },
                        HeroImage = new ToastGenericHeroImage
                        {
                            Source = msg.message
                        },
                        AppLogoOverride = new ToastGenericAppLogo
                        {
                            Source = chat.chatType == ChatType.CHAT ? DEFAULT_USER_IMAGE_PATH : DEFAULT_MUC_IMAGE_PATH,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
                Actions = getActions(msg, chat),
                DisplayTimestamp = msg.date,
                Launch = new ChatToastActivation(chat.id, false).generate()
            };

            popToast(toastContent, chat);
        }

        #endregion

        #region --Misc Methods (Private)--
        private static void popToast(ToastContent content, ChatTable chat)
        {
            popToast(content, chat, chat.id);
        }

        private static void popToast(ToastContent content, ChatTable chat, string group)
        {
            ToastNotification toast = new ToastNotification(content.GetXml())
            {
                Group = group
            };

            OnChatMessageToastEventArgs args = new OnChatMessageToastEventArgs(toast, chat);
            OnChatMessageToast?.Invoke(args);

            popToast(toast, args);
        }

        private static void popToast(ToastNotification toast, OnChatMessageToastEventArgs args)
        {
            switch (args.toasterTypeOverride)
            {
                case ChatMessageToasterType.FULL:
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                    Logger.Debug("Toast for group: " + toast.Group + " toasted with toaster type: " + args.toasterTypeOverride.ToString());
                    break;

                case ChatMessageToasterType.REDUCED:
                    popToastReduced();
                    Logger.Debug("Toast for group: " + toast.Group + " toasted with toaster type: " + args.toasterTypeOverride.ToString());
                    break;

                default:
                    Logger.Debug("Toast for group: " + toast.Group + " canceled.");
                    break;
            }
        }

        private static void popToastReduced()
        {
            // Vibrate:
            if (ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice") && !Settings.getSettingBoolean(SettingsConsts.DISABLE_VIBRATION_FOR_NEW_CHAT_MESSAGES))
            {
                VibrationDevice.GetDefault().Vibrate(VIBRATE_TS);
            }

            // Play sound:
            if(!Settings.getSettingBoolean(SettingsConsts.DISABLE_PLAY_SOUND_FOR_NEW_CHAT_MESSAGES))
            {
                SharedUtils.PlaySoundFromUri("ms-winsoundevent:Notification.Default");
            }
        }

        private static ToastActionsCustom getActions(ChatMessageTable msg, ChatTable chat)
        {
            return new ToastActionsCustom
            {
                Inputs =
                {
                    new ToastTextBox(TEXT_BOX_ID)
                    {
                        PlaceholderContent = "Reply"
                    }
                },
                Buttons =
                {
                    new ToastButton("Send", new SendReplyToastActivation(chat.id, false).generate())
                    {
                        ActivationType = ToastActivationType.Background,
                        ImageUri = chat.omemoEnabled ? SEND_BUTTON_ENCRYPTED_IMAGE_PATH : SEND_BUTTON_IMAGE_PATH,
                        TextBoxId = TEXT_BOX_ID,
                    },
                    new ToastButton("Mark chat as read", new MarkChatAsReadToastActivation(chat.id, false).generate())
                    {
                        ActivationType = ToastActivationType.Background
                    },
                    new ToastButton("Mark as read", new MarkMessageAsReadToastActivation(msg.id, false).generate())
                    {
                        ActivationType = ToastActivationType.Background
                    }
                }
            };
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
