﻿using System;
using Data_Manager2.Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP_XMPP_Client.Pages.SettingsPages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using Data_Manager2.Classes.DBTables;
using Data_Manager2.Classes.DBManager;
using UWP_XMPP_Client.Classes;
using UWP_XMPP_Client.Pages;
using System.Threading.Tasks;
using Windows.UI.Popups;
using XMPP_API.Classes.Network.XML.Messages.XEP_0045;

namespace UWP_XMPP_Client.Dialogs
{
    public sealed partial class AddMUCContentDialog : ContentDialog
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public bool cancled;
        private ObservableCollection<string> accounts;
        private ObservableCollection<string> servers;
        private List<XMPPClient> clients;
        private string server;

        private string requestedServer;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 03/01/2018 Created [Fabian Sauter]
        /// </history>
        public AddMUCContentDialog() : this(null)
        {

        }

        public AddMUCContentDialog(string roomJid)
        {
            this.cancled = true;
            this.accounts = new ObservableCollection<string>();
            this.servers = new ObservableCollection<string>();
            InitializeComponent();
            loadAccounts();
            loadServers();

            if(roomJid != null)
            {
                requestedServer = Utils.getDomainFromBareJid(roomJid);
                roomName_tbx.Text = Utils.getUserFromBareJid(roomJid) ?? "";
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        public void loadAccounts()
        {
            clients = ConnectionHandler.INSTANCE.getClients();
            if (clients != null)
            {
                accounts.Clear();
                foreach (XMPPClient c in clients)
                {
                    accounts.Add(c.getXMPPAccount().getIdAndDomain());
                }
            }
        }

        private void loadServers()
        {
            servers.Clear();
            foreach (DiscoFeatureTable f in DiscoManager.INSTANCE.getAllMUCServers())
            {
                servers.Add(f.fromServer);
            }
        }

        private async Task addRoomAsync()
        {
            if(await checkUserInputsAndWarnAsync())
            {
                XMPPClient c = clients[account_cbx.SelectedIndex];

                string roomJid = roomName_tbx.Text + '@' + servers[server_cbx.SelectedIndex];
                MUCJoinHelper helper = c.getNewMUCJoinHelper(roomJid);

                // Request reserved nick names:
                await helper.requestReservedNicksAsync();

                // Join room:
                if ((bool)enablePassword_cbx.IsChecked)
                {
                    await helper.enterRoomAsync(nick_tbx.Text, password_pwb.Password);
                }
                else
                {
                    await helper.enterRoomAsync(nick_tbx.Text);
                }
            }
        }

        private async Task<bool> checkUserInputsAndWarnAsync()
        {
            if(account_cbx.SelectedIndex < 0)
            {
                await showErrorDialogAsync("No account selected!");
                return false;
            }

            if (!clients[account_cbx.SelectedIndex].isConnected())
            {
                await showErrorDialogAsync("Account is not connected!");
                accountNotConnected_tblck.Visibility = Visibility.Visible;
                return false;
            }

            if (string.IsNullOrWhiteSpace(nick_tbx.Text))
            {
                await showErrorDialogAsync("No nickname given!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(roomName_tbx.Text))
            {
                await showErrorDialogAsync("No room name given!");
                return false;
            }

            if (server_cbx.SelectedIndex < 0)
            {
                await showErrorDialogAsync("No server selected!");
                return false;
            }

            return true;
        }

        private async Task showErrorDialogAsync(string msg)
        {
            MessageDialog dialog = new MessageDialog(msg, "Error!");
            await dialog.ShowAsync();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void add_btn_Click(object sender, RoutedEventArgs e)
        {
            await addRoomAsync();
        }

        private void cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            cancled = true;
            Hide();
        }

        private void account_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (account_cbx.Items.Count > 0)
            {
                account_cbx.SelectedIndex = 0;
            }
        }

        private void server_cbx_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (server_cbx.Items.Count > 0)
            {
                if(requestedServer != null)
                {
                    for (int i = 0; i < servers.Count; i++)
                    {
                        if (servers[i].Equals(requestedServer))
                        {
                            server_cbx.SelectedIndex = i;
                            return;
                        }
                    }
                }
                server_cbx.SelectedIndex = 0;
            }
        }

        private void addAccount_tblck_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Hide();
            (Window.Current.Content as Frame).Navigate(typeof(AccountSettingsPage));
        }

        private void enablePassword_cbx_Checked(object sender, RoutedEventArgs e)
        {
            password_pwb.Visibility = Visibility.Visible;
        }

        private void enablePassword_cbx_Unchecked(object sender, RoutedEventArgs e)
        {
            password_pwb.Visibility = Visibility.Collapsed;
        }

        private void roomName_tbx_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Space)
            {
                e.Handled = true;
            }
        }

        private void roomName_tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectionStart = roomName_tbx.SelectionStart;
            roomName_tbx.Text = roomName_tbx.Text.ToLower();
            roomName_tbx.SelectionStart = selectionStart;
            roomName_tbx.SelectionLength = 0;
        }

        private void server_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            browse_btn.IsEnabled = server_cbx.SelectedIndex >= 0;
        }

        private void browse_btn_Click(object sender, RoutedEventArgs e)
        {
            object o = server_cbx.SelectedItem;
            if(o is string && account_cbx.SelectedIndex >= 0)
            {
                (Window.Current.Content as Frame).Navigate(typeof(BrowseMUCRoomsPage), new BrowseMUCNavigationParameter(o as string, clients[account_cbx.SelectedIndex]));
                Hide();
            }
        }

        private void account_cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(account_cbx.SelectedIndex >= 0)
            {
                nick_tbx.Text = Utils.getUserFromBareJid(accounts[account_cbx.SelectedIndex]);
                accountNotConnected_tblck.Visibility = clients[account_cbx.SelectedIndex].isConnected() ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion
    }
}
