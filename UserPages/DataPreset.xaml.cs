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

namespace IPAM_NOTE.UserPages
{
    /// <summary>
    /// DataPreset.xaml 的交互逻辑
    /// </summary>
    public partial class DataPreset : UserControl
    {
        public DataPreset()
        {
            InitializeComponent();
        }

        private void DataPreset_OnLoaded(object sender, RoutedEventArgs e)
        {
            //加载初始页面
            PresetPanel.Children.Clear();
            ModelPreset modelPreset = new ModelPreset();
            //networkManage.Style = (Style)FindResource("UserControlStyle");
            PresetPanel.Children.Add(modelPreset);


        }

        private void PresetTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PresetTabControl.SelectedIndex;

            if (index != -1)
            {

                PresetPanel.Children.Clear();

                switch (index)
                {
                    case 0:

                        ModelPreset modelPreset = new ModelPreset();

                        //dataPreset.Style = (Style)FindResource("DataPresetStyle");

                        PresetPanel.Children.Add(modelPreset);

                        break;

                    case 1:

                        PeoplePreset peoplePreset = new PeoplePreset();

                        //dataPreset.Style = (Style)FindResource("DataPresetStyle");

                        PresetPanel.Children.Add(peoplePreset);

                        break;

                }
            }

        }
    }
}
