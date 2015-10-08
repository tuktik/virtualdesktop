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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Threading;
using System.Diagnostics;

using System.Management;

using System.Runtime.InteropServices;


namespace virtual_desktop
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    //Declare Auto Function SetParent Lib "user32.dll" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer

    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]//SetLastError = true
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
            //string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, long dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent (IntPtr hWnd);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        private const int WM_SYSCOMMAND = 274;
        private const int SC_MAXIMIZE = 61488;
       // Private Const SC_MAXIMIZE As Integer = 61488

        private string currentDesktop = "";
        public static string Start = string.Empty;

        public MainWindow()
        {

            Start = "start";

            InitializeComponent();
            //default_image.Source = Screenshot.CaptureScreen();
            //DesktopInitialize
            //default_image = ConvertDrawingImageToWPFImage(Screenshot.CaptureScreen());
            //DesktopInitialize("Desktop2");

            //this.Loaded += new RoutedEventHandler(this.Window_Loaded);
        }

        private System.Windows.Controls.Image ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();

            //convert System.Drawing.Image to WPF image
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(gdiImg);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            img.Source = WpfBitmap;
            img.Width = 500;
            img.Height = 600;
            img.Stretch = System.Windows.Media.Stretch.Fill;
            return img;
        }

        private void LoadScreenshots()
        {
            //Console.WriteLine("loadScreens");
            for (int index = 1; index < 5; index++)
            {
                string path = ".\\Desktop" + index.ToString();  // Name of the image

                if (File.Exists(path))  // If file exists
                {
                    // Create memory stream to hold the image for use, otherwise the file will be locked and unusable forther
                    MemoryStream stream = new MemoryStream();
                    System.Drawing.Image image = Bitmap.FromFile(path);
                    image.Save(stream, ImageFormat.Jpeg);  // Load image file to memory stream
                    image.Dispose();     // Dispose and unlock the file

                    
                    switch(index)
                    {
                        case 2:
                 //   Image2 = ConvertDrawingImageToWPFImage(Bitmap.FromStream(stream));
                            break;
                        case 3:
                 //Image3 = ConvertDrawingImageToWPFImage(Bitmap.FromStream(stream));
                            break;
                        case 4:
                 // Image4 = ConvertDrawingImageToWPFImage(Bitmap.FromStream(stream));
                            break;
                        default:
                            //default_image.Source = Screenshot.CaptureScreen();
                  // default_image = ConvertDrawingImageToWPFImage(Bitmap.FromStream(stream));
                            break;
                    }
                    
                    
                    // Get the PictureBox name and load in it image from memory
                    //string pictureBox = "pictureBox" + index.ToString();
                    //((PictureBox)tableLayoutPanel1.Controls[pictureBox]).Image = Bitmap.FromStream(stream);
                    
                }// if
                else
                {
                    switch (index)
                    {
                        case 2:
                            //Console.WriteLine("!!");
                     //Image2.Source = (ImageSource)new BitmapImage((new Uri("/black.jpg", UriKind.Relative)));
                            break;
                        case 3:
                     //Image3.Source = (ImageSource)new BitmapImage((new Uri("/black.jpg", UriKind.Relative)));
                            break;
                        case 4:
                     //Image4.Source = (ImageSource)new BitmapImage((new Uri("/black.jpg", UriKind.Relative)));
                            break;
                        default:
                            break;
                    }
                }
            }// for
        }

        private void LoadScreenshot()
        {
            //Console.WriteLine("loadscreen");
            switch (currentDesktop)
            {
                case "Default":
                    Console.WriteLine(currentDesktop);
            ///default_image.Source = Screenshot.CaptureScreen();
                    //default_image = ConvertDrawingImageToWPFImage(Screenshot.CaptureScreen());
                    break;
                case "Desktop2":
                    Console.WriteLine(currentDesktop);
            //Image2.Source = Screenshot.CaptureScreen();
                    //Image2 = ConvertDrawingImageToWPFImage(Screenshot.CaptureScreen());
                    break;
                case "Desktop3":
                    Console.WriteLine(currentDesktop);
             //Image3.Source = Screenshot.CaptureScreen();
                    //Image3 = ConvertDrawingImageToWPFImage(Screenshot.CaptureScreen());
                    break;
                case "Desktop4":
                    Console.WriteLine(currentDesktop);
              //Image4.Source = Screenshot.CaptureScreen();
                    //Image4 = ConvertDrawingImageToWPFImage(Screenshot.CaptureScreen());
                    break;
            }// switch
        }

        private void DesktopInitialize(string name)
        {
            DesktopSave();

            if (!Desktops.DesktopExists(name))
            {
                Console.WriteLine("create"+name);
                Desktops.DesktopCreate(name);
                Desktops.ProcessCreate(name, System.Reflection.Assembly.GetExecutingAssembly().Location, "start");     // Start VirtualDesktop application
                Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
            }

            Console.WriteLine(name+" desktop Handle ID"+Desktops.get_DesktopHandle(name));
            Desktops.DesktopSwitch(name);
        }

        private void DesktopSave()
        {
            string path = currentDesktop;
            if (path == "Default") { path = "Desktop1"; }
            path = ".\\" + path;
            Thread.Sleep(300);     // Wait to minimize current window
            Screenshot.SaveScreen(path, ImageFormat.Jpeg); // Save file to disk
        }

        void DeleteScreenshots()
        {
            for (int index = 2; index < 5; index++)
            {
                string desktopName = "Desktop" + index.ToString();
                string path = ".\\" + desktopName;

                // If file exists and desktop do not exists, delete the file
                if (File.Exists(path) && !Desktops.DesktopExists(desktopName))
                {
                    File.Delete(path);
                }
            }// for
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Start == "start")
            {
                Thread.Sleep(500);  
                //Console.WriteLine("call loaded");

                Process myProcess = new Process();
                
                myProcess.StartInfo.FileName = "C:\\Windows\\explorer.exe";
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
                myProcess.StartInfo.CreateNoWindow = true;
                

                myProcess.Start();
                //Process proc = Process.Start("explorer.exe");

                /*
                var myId = Process.Start("explorer.exe"); //Process.GetCurrentProcess().Id;
                var query = string.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", myId);
                var search = new ManagementObjectSearcher("root\\CIMV2", query);
                //var results = search.Get().GetEnumerator();
                //var queryObj;
                foreach (ManagementObject results in search.Get())
                {
                    // show the service
                    //queryObj = service.Current;
                    var parentId = (uint)results["ParentProcessId"];
                    var parent = Process.GetProcessById((int)parentId);
                    Console.WriteLine("I was started by {0}", parent.ProcessName);

                    //Console.WriteLine(service.ToString());

                }
                
        
                //results.MoveNext();
               // var queryObj = results.Current;
                //var parentId = (uint)queryObj["ParentProcessId"];
                //var parent = Process.GetProcessById((int)parentId);
                //Console.WriteLine("I was started by {0}", parent.ProcessName);
                //Console.ReadLine();
                
                                                     // Start explorer
                //Process.GetProcessById(proc.Handle).pare
               // uint pr = GetCurrentProcessId();
                //Console.WriteLine("parentHandle???pr" + pr);
                //Console.WriteLine("parentHandle@@@GetParent((IntPtr)pr)" + GetParent((IntPtr)pr));
                //Console.WriteLine("parentHandle!!!GetParent(proc.Handle)" + GetParent(proc.Handle));
                //GetParent(proc.Handle);
               // proc.WaitForInputIdle();
         //--------   

                //Console.WriteLine("proc:"+proc.Handle);
                //Console.WriteLine("proc"+ proc);
                //SetParent(proc.MainWindowHandle, Desktops.get_DesktopHandle(currentDesktop));
                //SendMessage(proc.MainWindowHandle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                 * */
                Thread.Sleep(500);                                                         // Wait explorer to initialize
                
                // Reposition window on taskbar   
                this.Opacity = 0;
                this.WindowState = WindowState.Normal;
                this.WindowState = WindowState.Minimized;
                Thread.Sleep(300);
                this.Opacity = 100;
            }
                
            

            // Initialize
            currentDesktop = Desktops.DesktopName(Desktops.DesktopOpenInput());
            DeleteScreenshots();

            //Console.WriteLine(currentDesktop);
        }

        void window_Activated(object sender, EventArgs e)
        {
            Console.WriteLine("Active");

            LoadScreenshots();
            LoadScreenshot();

            // Set form caption
            string caption = currentDesktop;
            if (caption == "Default") { caption = "Desktop1"; }
            //this.Name = "VirtualDesktop IGProgram - " + caption;
        }

        void window_Deactivated(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        void Image1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("image1click");
            this.WindowState = WindowState.Minimized;
            DesktopInitialize("Default");
        }

        void Image2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("image2click");
            this.WindowState = WindowState.Minimized;
            DesktopInitialize("Desktop2");
        }

        void Image3_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            DesktopInitialize("Desktop3");
        }

        void Image4_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            DesktopInitialize("Desktop4");
        }

    }

    
}
