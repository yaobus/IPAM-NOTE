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
			ScaleTransform scaleTransform = MyCanvas.RenderTransform as ScaleTransform;
			if (scaleTransform == null)
			{
				// 如果没有缩放变换，则创建一个新的缩放变换，并应用到 Canvas 上
				scaleTransform = new ScaleTransform(1.0, 1.0);
				MyCanvas.RenderTransform = scaleTransform;
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

		private void MyCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				isDragging = true;
				lastPosition = e.GetPosition(MyCanvas);
				MyCanvas.CaptureMouse();
			}
		}

		private void MyCanvas_OnMouseMove(object sender, MouseEventArgs e)
		{
			if (isDragging)
			{
				Point newPosition = e.GetPosition(MyCanvas);
				double deltaX = newPosition.X - lastPosition.X;
				double deltaY = newPosition.Y - lastPosition.Y;

				TranslateTransform translateTransform = MyCanvas.RenderTransform as TranslateTransform;
				if (translateTransform == null)
				{
					translateTransform = new TranslateTransform();
					MyCanvas.RenderTransform = translateTransform;
				}

				translateTransform.X += deltaX;
				translateTransform.Y += deltaY;

				lastPosition = newPosition;
			}
		}

		private void MyCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (isDragging)
			{
				isDragging = false;
				MyCanvas.ReleaseMouseCapture();
			}
		}
	}
}

