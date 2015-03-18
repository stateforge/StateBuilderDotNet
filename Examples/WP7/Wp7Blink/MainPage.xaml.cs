using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace StateForge.Examples.Wp7Blink
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Costruttore
        private BlinkContext context;

        private SolidColorBrush colorOn, colorOff, colorIdle;
        public MainPage()
        {
            this.colorOn = new SolidColorBrush();
            this.colorOn.Color = Colors.Green;
            this.colorOff = new SolidColorBrush();
            this.colorOff.Color = Colors.Red;

            this.colorIdle = new SolidColorBrush();
            this.colorIdle.Color = Colors.White;

            InitializeComponent();
            this.context = new BlinkContext(this);
            this.context.EnterInitialState();
           
        }

        internal void EnterIdle()
        {
            Dispatcher.BeginInvoke(() => { 
                this.ButtonStartStop.Content = "Start";
                RectangleView.Fill = this.colorIdle;
            });
        }

        internal void EnterOperating()
        {
            Dispatcher.BeginInvoke(() => { this.ButtonStartStop.Content = "Stop"; });
        }

        internal void DisplayOn()
        {
            Dispatcher.BeginInvoke(() => { RectangleView.Fill = this.colorOn; });
        }

        internal void DisplayOff()
        {
            Dispatcher.BeginInvoke(() => { RectangleView.Fill = this.colorOff; });
        }

        private void ButtonStartStop_Click(object sender, RoutedEventArgs e)
        {
            this.context.StartStop();
        }
    }
}