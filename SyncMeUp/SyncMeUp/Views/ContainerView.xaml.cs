using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SyncMeUp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerView : ContentView
    {
        public ContainerView()
        {
            InitializeComponent();
        }
    }
}