using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace GraficDisplay
{
    using GraphLib;
     
    public partial class MainForm : Form
    {
        //  plot & stock تعداد ادغام نمودار با استفاده از کتابخونه های
        private int NumGraphs = 1;
        // نوع نمودار که در پایین می توان تعریف کرد
        private String CurExample = "NORMAL";
        // رنگ بک گراند چارت
        private String CurColorSchema = "RED";      
        private PrecisionTimer.Timer mTimer = null;
        // استارت تایمر از ثانیه صفرم
        private DateTime lastTimerTick = DateTime.Now;
        
       
        public MainForm()
        {           
            InitializeComponent();

            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
            
            // پر کردن دیتاهای هر چارت
            CalcDataGraphs();
             // رفرش کردن خودکار چارت
            display.Refresh();

             UpdateGraphCountMenu();

            UpdateColorSchemaMenu();

            mTimer = new PrecisionTimer.Timer();
            // دوره زمانی 
            mTimer.Period = 40;                         // 20 fps
            mTimer.Tick += new EventHandler(OnTimerTick);
            lastTimerTick = DateTime.Now;
            //  تایمر نشان دهنده ی زمان صرف شده برای نمایش نقاط و نمودار
            mTimer.Start();             
        }

        //کردن پنجره close تابع
        protected override void OnClosed(EventArgs e)
        {
            mTimer.Stop();
            mTimer.Dispose();
            base.OnClosed(e);
        }

       // ست کردن تایمر برای نمودار
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (CurExample == "ANIMATED_AUTO" )
            {
                    try
                    {
                        TimeSpan dt = DateTime.Now - lastTimerTick;

                        for (int j = 0; j < NumGraphs; j++)
                        {                                                     
                            CalcFunction_3(display.DataSources[j], j, (float)dt.TotalMilliseconds);
                            
                        }
                   
                        this.Invoke(new MethodInvoker(RefreshGraph));
                    }
                    catch (ObjectDisposedException ex)
                    {
                        // we get this on closing of form
                    }
                    catch (Exception ex)
                    {
                        Console.Write("exception invoking refreshgraph(): " + ex.Message);
                    }
                 
                
            }
        }      
        // تابع رفرش کردن نمودار و شروع از اول که در داخل کد هر جا نیاز باشه میتونید استفاده کنید
        private void RefreshGraph()
        {                             
            display.Refresh();             
        }

        // ست کردن اعداد برای استفاده در نمودار
        protected void array_digit(Array x, Array y)
        {
            int[] xs = new int[] { 10, 23, 30, 48, 58, 72, 81, 94, 102, 125, 135, 185, 186, 190, 201, 220, 225, 235, 305, 310, 358, 399 };
            int[] ys = new int[] { 5, 12, 18, 35, 38, 42, 51, 52, 65, 66, 69, 72, 75, 80, 81, 85, 90, 91, 150, 165, 185, 252, 259, 320 };
        }

        // نمونه تابع اول برای تست با دو ورودی
        protected void CalcFunction_0(DataSource src, int idx)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;
                src.Samples[i].y = (float)(((float)200 * Math.Sin((idx + 1) *(i + 1.0) * 48 / src.Length)));
            }            
        }       

        // نمونه تابع دوم برای تست با دو ورودی
        protected void CalcFunction_1(DataSource src, int idx)
        {
            //array_digit();
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;

                src.Samples[i].y = (float)(((float)20 *
                                            Math.Sin(20 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                            Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                            (float)(((float)200 *
                                            Math.Sin(200 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
            }
           src.OnRenderYAxisLabel = RenderYLabel;
        }

        // نمونه تابع سوم برای تست با دو ورودی
        protected void CalcFunction_2(DataSource src, int idx)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].x = i;

                src.Samples[i].y = (float)(((float)20 *
                                            Math.Sin(40 * (idx + 1) * (i + 1) * Math.PI / src.Length)) *
                                            Math.Sin(160 * (idx + 1) * (i + 1) * Math.PI / src.Length)) +
                                            (float)(((float)200 *
                                            Math.Sin(4 * (idx + 1) * (i + 1) * Math.PI / src.Length)));
            }
            src.OnRenderYAxisLabel = RenderYLabel;

        }

        // نمونه تابع چهارم برای تست با سه ورودی
        protected void CalcFunction_3(DataSource ds, int idx, float time)
        {
            cPoint[] src = ds.Samples;
            for (int i = 0; i < src.Length; i++)
            {
                src[i].x = i;
                src[i].y = 200 + (float)((200 * Math.Sin((idx + 1) * (time + i * 100) / 8000.0)))+
                                +(float)((40  * Math.Sin((idx + 1) * (time + i * 200) / 2000.0)));               
            }
            
        }       
               
        // تابعی برای اعمال رنگ بک گراند چارت        
        private void ApplyColorSchema()
        {            
            switch (CurColorSchema)
            {
                case "DARK_GREEN":
                    {
                        Color[] cols = { Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0),
                                         Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0), 
                                         Color.FromArgb(0,255,0) ,
                                         Color.FromArgb(0,255,0),                              
                                         Color.FromArgb(0,255,0) };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.FromArgb(0, 64, 0);
                        display.BackgroundColorBot = Color.FromArgb(0, 64, 0);
                        display.SolidGridColor = Color.FromArgb(0, 128, 0);
                        display.DashedGridColor = Color.FromArgb(0, 128, 0);
                    }
                    break;
                case "WHITE":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j%7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.White;
                        display.SolidGridColor = Color.LightGray;
                        display.DashedGridColor = Color.LightGray;
                    }
                    break;

                case "BLUE":
                    {
                        Color[] cols = { Color.Red, 
                                         Color.Orange,
                                         Color.Yellow, 
                                         Color.LightGreen, 
                                         Color.Blue ,
                                         Color.DarkSalmon,                              
                                         Color.LightPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j%7];
                        }

                        display.BackgroundColorTop = Color.Navy;
                        display.BackgroundColorBot = Color.FromArgb(0, 0, 64);
                        display.SolidGridColor = Color.Blue;
                        display.DashedGridColor = Color.Blue;
                    }
                    break;

                case "GRAY":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.LightGray;
                        display.SolidGridColor = Color.LightGray;
                        display.DashedGridColor = Color.LightGray;
                    }
                    break;

                case "RED":
                    {
                        Color[] cols = { Color.DarkCyan, 
                                         Color.Yellow,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.DarkRed;
                        display.BackgroundColorBot = Color.Black;
                        display.SolidGridColor = Color.Red;
                        display.DashedGridColor = Color.Red;
                    }
                    break;

                case "LIGHT_BLUE":
                    {
                        Color[] cols = { Color.DarkRed, 
                                         Color.DarkSlateGray,
                                         Color.DarkCyan, 
                                         Color.DarkGreen, 
                                         Color.DarkBlue ,
                                         Color.DarkMagenta,                              
                                         Color.DeepPink };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.White;
                        display.BackgroundColorBot = Color.FromArgb(183,183,255);
                        display.SolidGridColor = Color.Blue;
                        display.DashedGridColor = Color.Blue;
                    }
                    break;

                case "BLACK":
                    {
                        Color[] cols = { Color.FromArgb(255,0,0), 
                                         Color.FromArgb(0,255,0),
                                         Color.FromArgb(255,255,0), 
                                         Color.FromArgb(64,64,255), 
                                         Color.FromArgb(0,255,255) ,
                                         Color.FromArgb(255,0,255),                              
                                         Color.FromArgb(255,128,0) };

                        for (int j = 0; j < NumGraphs; j++)
                        {
                            display.DataSources[j].GraphColor = cols[j % 7];
                        }

                        display.BackgroundColorTop = Color.Black;
                        display.BackgroundColorBot = Color.Black;
                        display.SolidGridColor = Color.DarkGray;
                        display.DashedGridColor = Color.DarkGray;
                    }
                    break;
            }

        }


        // :اکشنی داینامیک برای پر کردن دیتا ها نمودار مثل 
        // SetDisplayRangeY
        // SetGridDistanceY
        // DataSources.length
        // chart.text , etc
        protected void CalcDataGraphs( )
        {

            this.SuspendLayout();
           
            display.DataSources.Clear();
            display.SetDisplayRangeX(0, 900);

            for (int j = 0; j < NumGraphs; j++)
            {
                display.DataSources.Add(new DataSource());
                display.DataSources[j].Name = "Graph " + (j + 1);                
                display.DataSources[j].OnRenderXAxisLabel += RenderXLabel;
              
                switch (CurExample)
                {
                    case  "NORMAL":
                        this.Text = "Graph1";
                        display.DataSources[j].Length = 5800;
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        CalcFunction_2(display.DataSources[j], j);                        
                        break;

                    case "NORMAL_AUTO":
                        this.Text = "Graph2";
                        display.DataSources[j].Length = 5800;
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        CalcFunction_0(display.DataSources[j], j);
                        break;

                    case "STACKED":
                        this.Text = "Graph3";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.STACKED;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-250, 250);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_0(display.DataSources[j], j);
                        break;

                    case "VERTICAL_ALIGNED":
                        this.Text = "Graph4";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
                        display.DataSources[j].Length = 11;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_0(display.DataSources[j], j);    
                        break;

                    case "VERTICAL_ALIGNED_AUTO":
                        this.Text = "Graph5";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 300);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_0(display.DataSources[j], j);
                        break;

                    case "TILED_VERTICAL":
                        this.Text = "Graph6";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_1(display.DataSources[j], j);    
                        break;

                    case "TILED_VERTICAL_AUTO":
                        this.Text = "Graph7";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_1(display.DataSources[j], j);
                        break;

                    case "TILED_HORIZONTAL":
                        this.Text = "Graph8";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_2(display.DataSources[j], j);    
                        break;

                    case "TILED_HORIZONTAL_AUTO":
                        this.Text = "Graph9";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 5800;
                        display.DataSources[j].AutoScaleY = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 600);
                        display.DataSources[j].SetGridDistanceY(100);
                        CalcFunction_2(display.DataSources[j], j);
                        break;

                    case "ANIMATED_AUTO":
                       
                        this.Text = "Graph10";
                        display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
                        display.DataSources[j].Length = 402;
                        display.DataSources[j].AutoScaleY = false;
                        display.DataSources[j].AutoScaleX = true;
                        display.DataSources[j].SetDisplayRangeY(-300, 500);
                        display.DataSources[j].SetGridDistanceY(100);
                        display.DataSources[j].XAutoScaleOffset = 50;
                        CalcFunction_3(display.DataSources[j], j, 0);
                        display.DataSources[j].OnRenderYAxisLabel = RenderYLabel;
                        break;
                }             
            }
            
            ApplyColorSchema();

            this.ResumeLayout();
            display.Refresh();
           
        }                
        private String RenderXLabel(DataSource s, int idx)
        {
            if (s.AutoScaleX) 
            {
                //if (idx % 2 == 0)
                {
                    int Value = (int)(s.Samples[idx].x );
                    return "" + Value;
                }
                return "";
            }
            else
            {
                int Value = (int)(s.Samples[idx].x / 200);
                String Label = "" + Value + "\"";
                return Label;
            }
        }

        private String RenderYLabel(DataSource s, float  value)
        {             
            return String.Format("{0:0.0}", value);
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            display.Dispose();

            base.OnClosing(e);
        }

        
        // هایی برای کل پنجره نمودار مانند دکمه پرینت کردن و استپ و استارت event
        private void stackedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.NORMAL;
        }

        private void verticalALignedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.VERTICAL_ARRANGED;
        }

        private void tiledVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_VER;
        }

        private void tiledHorizontalyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
        }
                
        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
        }

        private void antiAliasedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void highSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void highQualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            display.Smoothing = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        }

        // نام های مربوط به هر نمودار که می توان دیتای آن را در تابع مربوط به خودش پر کرد
        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "NORMAL";
            CalcDataGraphs();
        }

        private void normalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "NORMAL_AUTO";
            CalcDataGraphs();
        }
        
        private void stackedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            CurExample = "STACKED";
            CalcDataGraphs();
        }

        private void verticallyAlignedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "VERTICAL_ALIGNED";
            CalcDataGraphs();
        }
        private void verticallyAlignedAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "VERTICAL_ALIGNED_AUTO";
            CalcDataGraphs();
        }

        private void tiledVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_VERTICAL";
            CalcDataGraphs();
        }
        private void tiledVerticalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_VERTICAL_AUTO";
            CalcDataGraphs();
        }

        private void tiledHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_HORIZONTAL";
            CalcDataGraphs();
        }

        private void tiledHorizontalAutoscaledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "TILED_HORIZONTAL_AUTO";
            CalcDataGraphs();
        }

        private void animatedGraphDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurExample = "ANIMATED_AUTO";
            CalcDataGraphs();
        }
   

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "BLUE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "WHITE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "GRAY";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

         private void lightBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "LIGHT_BLUE";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
            
        }

        private void blackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "BLACK";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "RED";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
        }

          private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurColorSchema = "DARK_GREEN";
            CalcDataGraphs();
            UpdateColorSchemaMenu();
           
        }
        
        
        
        private void UpdateColorSchemaMenu()
        {
            blueToolStripMenuItem.Checked = false;
            whiteToolStripMenuItem.Checked = false;
            grayToolStripMenuItem.Checked = false;
            lightBlueToolStripMenuItem.Checked = false;
            blackToolStripMenuItem.Checked = false;
            redToolStripMenuItem.Checked = false;

            if (CurColorSchema == "WHITE") whiteToolStripMenuItem.Checked = true;
            if (CurColorSchema == "BLUE") blueToolStripMenuItem.Checked = true;
            if (CurColorSchema == "GRAY") grayToolStripMenuItem.Checked = true;
            if (CurColorSchema == "LIGHT_BLUE") lightBlueToolStripMenuItem.Checked = true;
            if (CurColorSchema == "BLACK") blackToolStripMenuItem.Checked = true;
            if (CurColorSchema == "RED") redToolStripMenuItem.Checked = true;
            if (CurColorSchema == "DARK_GREEN") greenToolStripMenuItem.Checked = true;
        } 

          
        private void UpdateGraphCountMenu()
        {
            toolStripMenuItem2.Checked = false;
            toolStripMenuItem3.Checked = false;
            toolStripMenuItem4.Checked = false;
            toolStripMenuItem5.Checked = false;
            toolStripMenuItem6.Checked = false;
            toolStripMenuItem7.Checked = false;

            switch (NumGraphs)
            {
                case 1: toolStripMenuItem2.Checked = true; break;
                case 2: toolStripMenuItem3.Checked = true; break;
                case 3: toolStripMenuItem4.Checked = true; break;
                case 4: toolStripMenuItem5.Checked = true; break;
                case 5: toolStripMenuItem6.Checked = true; break;
                case 6: toolStripMenuItem7.Checked = true; break;
                
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            NumGraphs = 1;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            NumGraphs = 2;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            NumGraphs = 3;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            NumGraphs = 4;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            NumGraphs = 5;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            NumGraphs = 6;
            CalcDataGraphs();
            UpdateGraphCountMenu();
        }

      

     
       

       
       
        
    

     

        

        

         
         
    }
}