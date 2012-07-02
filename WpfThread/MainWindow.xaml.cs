using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageFlip
{
    /// <summary>
    /// WPF  多线程将图片分割
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource source;
        private object lockObj = new object();

        public MainWindow()
        {
            InitializeComponent();
            //首先获取图片
            Bitmap orginalImage = new Bitmap(@"E:\Projects\ThreadTest\WpfThread\Image\trooper.jpg");
            //创建线程1
            Thread t1 = new Thread(new ParameterizedThreadStart
                (
                  obj =>
                  {
                      //WPF中使用多线程的话最后一定要返回UI线程，否则操作界面控件时会报错
                      //BeginInvoke方法便是返回UI线程的方法
                      this.Dispatcher.BeginInvoke((Action)(() =>
                      {
                          //通过Parameter类的属性裁剪图片
                          ClipImageAndBind(obj);
                          //图片的部分绑定到页面控件
                          this.TestImage1.Source = source;
                      }));
                  }
                ));
            //创建线程2
            Thread t2 = new Thread(new ParameterizedThreadStart
            (
              obj =>
              {
                  //WPF中使用多线程的话最后一定要返回UI线程，否则操作界面控件时会报错
                  //BeginInvoke方法便是返回UI线程的方法
                  this.Dispatcher.BeginInvoke((Action)(() =>
                  {
                      //通过Parameter类的属性裁剪图片
                      ClipImageAndBind(obj);
                      //图片的部分绑定到页面控件
                      this.TestImage2.Source = source;
                      //尝试将线程1的启动逻辑放在线程2所持有的方法中
                      // t1.Start(new Parameter { OrginalImage = orginalImage, ClipHeight = 500, ClipWidth = 500, StartX = 0, StartY = 0 });
                  }));
              }
            ));

            t2.Start(new Parameter { OrginalImage = orginalImage, ClipHeight = 500, ClipWidth = 500, StartX = orginalImage.Width - 500, StartY = orginalImage.Height - 500 });
            //尝试下注释掉t2.join方法后是什么情况,其实注释掉之后，两个线程会一起工作，
            //去掉注释后，界面一直到两个图片部分都绑定完成后才出现
            //t2.Join();
            t1.Start(new Parameter { OrginalImage = orginalImage, ClipHeight = 500, ClipWidth = 500, StartX = 0, StartY = 0 });
        }

        /// <summary>
        /// 根据参数类进行剪裁图片，加锁防止共享资源被破坏
        /// </summary>
        /// <param name="para">Parameter类对象</param>
        private void ClipImageAndBind(object para)
        {
            lock (lockObj)
            {
                Parameter paraObject = (para as Parameter);
                source = this.ClipPartOfImage(paraObject);
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 具体裁剪图片，大家不必在意这个方法，关键是线程的使用
        /// </summary>
        /// <param name="para">Parameter</param>
        /// <returns>部分图片</returns>
        private BitmapSource ClipPartOfImage(Parameter para)
        {
            if (para == null) { throw new NullReferenceException("para 不能为空"); }
            if (para.OrginalImage == null) { throw new NullReferenceException("OrginalImage 不能为空"); }
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(para.StartX, para.StartY, para.ClipWidth, para.ClipHeight);
            var bitmap2 = para.OrginalImage.Clone(rect, para.OrginalImage.PixelFormat) as Bitmap;
            return ChangeBitmapToBitmapSource(bitmap2);
        }

        private BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }
    }

    /// <summary>
    /// 参数类
    /// </summary>
    public class Parameter
    {
        public Bitmap OrginalImage { get; set; }

        public int StartX { get; set; }

        public int StartY { get; set; }

        public int ClipWidth { get; set; }

        public int ClipHeight { get; set; }
    }
}