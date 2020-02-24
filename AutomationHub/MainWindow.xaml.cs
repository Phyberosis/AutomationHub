using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomationHub
{
    public struct VerboseInfo
    {
        public string pos;
        public string dir;
        public string msg;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private Controller controller;
        private ICom com;

        public MainWindow()
        {
            com = ComFactory.getDefaultCom();
            controller = ControllerFactory.makeDefaultController(com);
            InitializeComponent();
        }

        private void Click_Control(object sender, RoutedEventArgs e)
        {
            if (controller.inControl())
            {
                controller.ReleaseControl();
            }
            else
            {
                controller.GiveControl(setArmDataLabels);
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            controller.keyDown(sender, e);
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {
            controller.keyUp(sender, e);
        }

        public void setArmDataLabels(VerboseInfo info)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                lblArmPos.Content = "Position: " + info.pos;
                lblArmDir.Content = "Gimbal: " + info.dir;
                lblMsg.Content = info.msg;
            }));
        }

        private void FrmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controller.ReleaseControl();
        }
    }
}
