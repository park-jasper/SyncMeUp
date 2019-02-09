﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QRCoder;
using Xamarin.Forms;

namespace SyncMeUp
{
    public partial class MainPage : ContentPage
    {
        public ImageSource QrCode { get; set; }

        private string _labelText;

        public string LabelText
        {
            get => _labelText;
            set
            {
                if (_labelText != value)
                {
                    _labelText = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            var inst = new TestInstance
            {
                Text1 = "SomeText",
                Number2 = 15
            };

            //LabelText = JsonConvert.SerializeObject(inst, Formatting.Indented);
            LabelText = "";

            //OnPropertyChanged(nameof(LabelText));

            MakeQrCode("abcd1234");
            Task.Run(() => StartListening());

            Di.RegisterInstance(this);
        }

        public void MakeQrCode(string content)
        {
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
            var code = new BitmapByteQRCode(data);
            var rawData = code.GetGraphic(20);
            var stream = new MemoryStream(rawData);
            //image.Save(stream, ImageFormat.Bmp);
            QrCode = ImageSource.FromStream(() => stream);

            OnPropertyChanged(nameof(QrCode));
        }

        public async void StartListening()
        {
            var hostName = Dns.GetHostName();
            var address = Dns.GetHostAddresses(hostName);
            var hostEntry = Dns.GetHostEntry(hostName);
            var otherAddresses = hostEntry.AddressList;
            var filtered = otherAddresses.Where(addr => addr.AddressFamily == AddressFamily.InterNetwork).ToArray();
            var names = filtered.Select(addr => addr.ToString()).ToArray();
            var listener = new TcpListener(IPAddress.Parse("192.168.0.129"), 1585);

            var tcpGeneralListener = new TcpListener(IPAddress.Any, 30465);
            tcpGeneralListener.Start();

            Task.Run(() =>
            {
                var socket = tcpGeneralListener.AcceptSocket();
                int i;
                byte[] bytes = new byte[256];

                while (( i = socket.Receive(bytes) ) != 0)
                {
                    string data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    if (data == "Hello")
                    {
                        socket.Send(Encoding.ASCII.GetBytes("Hello yourself"));
                    }
                }
            });
            //var listener = TcpListener.Create(1585);

            listener.Start();

            while (true)
            {
                int beforeAccept = 1;
                //var client = listener.AcceptTcpClient();
                var socket = listener.AcceptSocket();

                //var stream = client.GetStream();

                int i;
                byte[] bytes = new byte[256];

                while (( i = socket.Receive(bytes) ) != 0)
                {
                    string text = "";

                    //while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    //{
                    string data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    text += data;
                    //}

                    MakeQrCode(text);
                }

                socket.Dispose();
            }
        }
    }

    public class TestInstance
    {
        public string Text1 { get; set; }
        public int Number2 { get; set; }
    }
}