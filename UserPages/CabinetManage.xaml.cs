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
		private Point lastPosition;

        private bool isDraggingCanvas = false;
        private bool isDraggingContent = false;
        private Point lastCanvasPosition;
        private Point lastContentPosition;

        public CabinetManage()
		{
			InitializeComponent();


		}

		private void Canvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
		{
			// 获取当前鼠标滚轮滚动的增量
			int delta = e.Delta;

			// 定义缩放比例的增量，可以根据实际需求进行调整
			double scaleIncrement = 0.1;

			// 获取当前 Canvas 的缩放变换
			ScaleTransform scaleTransform = SubCanvas.RenderTransform as ScaleTransform;
			if (scaleTransform == null)
			{
				// 如果没有缩放变换，则创建一个新的缩放变换，并应用到 Canvas 上
				scaleTransform = new ScaleTransform(1.0, 1.0);
				SubCanvas.RenderTransform = scaleTransform;
			}

			// 计算新的缩放比例
			double newScale = delta > 0 ? scaleTransform.ScaleX + scaleIncrement : scaleTransform.ScaleX - scaleIncrement;

			// 设置缩放比例的上下限，可以根据实际需求进行调整
			double minScale = 1.0;
			double maxScale = 3.0;
			newScale = Math.Max(minScale, Math.Min(maxScale, newScale));

			// 应用新的缩放比例到 Canvas 上
			scaleTransform.ScaleX = newScale;
			scaleTransform.ScaleY = newScale;
		}




        private void SubCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 右键按下时开始拖动操作
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    // 只拖动内容
                    isDraggingContent = true;
                    lastContentPosition = e.GetPosition(SubCanvas);
                   SubCanvas.CaptureMouse();
                }
                else
                {
                    // 拖动整个画布
                    isDraggingCanvas = true;
                    lastCanvasPosition = e.GetPosition(SubCanvas);
                    SubCanvas.CaptureMouse();
                }
            }
        }

        private void SubCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            Point newPosition = e.GetPosition(SubCanvas);
            double deltaX = newPosition.X - lastCanvasPosition.X;
            double deltaY = newPosition.Y - lastCanvasPosition.Y;

            Canvas.SetLeft(SubCanvas, Canvas.GetLeft(SubCanvas) + deltaX);
            Canvas.SetTop(SubCanvas, Canvas.GetTop(SubCanvas) + deltaY);

            lastCanvasPosition = newPosition;
        }
    }
}

