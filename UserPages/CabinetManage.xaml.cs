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
    /// CabinetManage.xaml 的交互逻辑
    /// </summary>
    public partial class CabinetManage : UserControl
    {
        private bool isDragging = false;
        private UIElement draggedElement = null;
        private Point offset;

        public CabinetManage()
        {
            InitializeComponent();
            DrawRectangle();
        }


        private void DrawRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 100;
            rectangle.Height = 20;
            rectangle.Fill = Brushes.DarkCyan;

            // 设置矩形在Canvas上的位置
            Canvas.SetLeft(rectangle, 50);
            Canvas.SetTop(rectangle, 100);

            // 将矩形添加到Canvas中
            MyCanvas.Children.Add(rectangle);

            // 添加拖拽事件处理
            rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
            rectangle.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 标记开始拖拽
            isDragging = true;
            draggedElement = sender as UIElement;
            offset = e.GetPosition(draggedElement);
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 结束拖拽
            isDragging = false;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            draggedElement = e.Source as UIElement;
            offset = e.GetPosition(draggedElement);
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedElement != null)
            {
                Point mousePosition = e.GetPosition(MyCanvas);
                double newX = mousePosition.X - offset.X;
                double newY = mousePosition.Y - offset.Y;

                // 确保图形不会超出Canvas范围
                newX = Math.Max(0, Math.Min(MyCanvas.ActualWidth - draggedElement.RenderSize.Width, newX));
                newY = Math.Max(0, Math.Min(MyCanvas.ActualHeight - draggedElement.RenderSize.Height, newY));

                Canvas.SetLeft(draggedElement, newX);
                Canvas.SetTop(draggedElement, newY);
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggedElement = null;
        }

    }
}

