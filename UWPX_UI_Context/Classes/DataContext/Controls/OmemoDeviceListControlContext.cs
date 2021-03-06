﻿using Logging;
using System.Linq;
using System.Threading.Tasks;
using UWPX_UI_Context.Classes.DataTemplates;
using UWPX_UI_Context.Classes.DataTemplates.Controls;
using Windows.UI.Xaml;
using XMPP_API.Classes;
using XMPP_API.Classes.Network.XML.Messages;
using XMPP_API.Classes.Network.XML.Messages.Helper;
using XMPP_API.Classes.Network.XML.Messages.XEP_0384;

namespace UWPX_UI_Context.Classes.DataContext.Controls
{
    public sealed class OmemoDeviceListControlContext
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly OmemoDeviceListControlDataTemplate MODEL = new OmemoDeviceListControlDataTemplate();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public async Task ResetOmemoDevicesAsync(XMPPClient client)
        {
            MODEL.ResettingDevices = true;
            OmemoDevices devices = new OmemoDevices();
            devices.IDS.Add(client.getXMPPAccount().omemoDeviceId);
            await client.OMEMO_COMMAND_HELPER.setDeviceListAsync(devices);
            MODEL.ResettingDevices = false;
        }

        private void OnResetDeviceListResult(bool success)
        {
            MODEL.ResettingDevices = false;
        }

        public async Task RefreshOmemoDevicesAsync(XMPPClient client)
        {
            MODEL.RefreshingDevices = true;
            MessageResponseHelperResult<IQMessage> result = await client.OMEMO_COMMAND_HELPER.requestDeviceListAsync(client.getXMPPAccount().getBareJid());
            if (result.STATE == MessageResponseHelperResultState.SUCCESS)
            {
                if (result.RESULT is OmemoDeviceListResultMessage deviceListResultMessage)
                {
                    MODEL.DEVICES.Clear();
                    MODEL.DEVICES.AddRange(deviceListResultMessage.DEVICES.IDS.Select(x => new UintDataTemplate { Value = x }));
                }
                else
                {
                    Logger.Warn("Failed to request device list (" + result.RESULT.ToString() + ").");
                }
            }
            else
            {
                Logger.Warn("Failed to request device list (" + result.STATE.ToString() + ").");
            }
            MODEL.RefreshingDevices = false;
        }

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
