﻿using Box.V2.Auth;
using Box.V2.Contracts;
using Box.V2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Box.V2.W8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public const string ClientId = "pweqblqwil7cpmvgu45jaokt3qw77wbo";
        public const string ConsumerKey = "hdivvq08t2gnj19zssp6xqmovjp42u2g";
        public const string ClientSecret = "dTrKxu2JYDeYIyQKSKLDf57HVlWjvU10";
        public const string RedirectUri = "http://localhost";

        private IBoxConfig _config;
        private BoxClient _client;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _config = new BoxConfig(ClientId, ClientSecret, RedirectUri);
            _client = new BoxClient(_config);

            string authCode = await Authenticate();
            OAuthSession session = await _client.Auth.AuthenticateAsync(authCode);

            //Folder f = await _client.FoldersManager.GetItemsAsync("0", 10);
            //OAuthSession session1 = await _client.Auth.RefreshAccessTokenAsync(session.AccessToken);
            
            //File f = await _client.FilesManager.GetInformationAsync("7546361455");
            byte[] data = await _client.FilesManager.DownloadBytesAsync("7546361455");

            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedFileName = "test.xml"; //f.Name;
            //string ext = Path.GetExtension(f.Name);
            fileSavePicker.FileTypeChoices.Add("xml", new string[] { ".xml" });
            //fileSavePicker.SuggestedFileName 
            StorageFile file = await fileSavePicker.PickSaveFileAsync();

            await Windows.Storage.FileIO.WriteBytesAsync(file, data);
        }
        
        public async Task<string> Authenticate()
        {
            WebAuthenticationResult war = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                _client.Auth.AuthCodeUri,
                new Uri(_config.RedirectUri));

            switch (war.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    {
                        // grab auth code
                        var response = war.ResponseData;
                        WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(new Uri(response).Query);
                        return decoder.GetFirstValueByName("code");
                    }
                case WebAuthenticationStatus.UserCancel:
                    {
                        //log("HTTP Error returned by AuthenticateAsync() : " + war.ResponseErrorDetail.ToString());
                        break;
                    }
                default:
                case WebAuthenticationStatus.ErrorHttp:
                    //log("Error returned by AuthenticateAsync() : " + war.ResponseStatus.ToString());
                    break;
            }

            return string.Empty;
        }
    }
}
