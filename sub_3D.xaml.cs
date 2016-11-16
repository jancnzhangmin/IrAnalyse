using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace IrAnalyse
{
    /// <summary>
    /// sub_3D.xaml 的交互逻辑
    /// </summary>
    public partial class sub_3D : UserControl
    {
        public sub_3D()
        {
            InitializeComponent();
            

        }
        public string move_type = "";
        DispatcherTimer timer = new DispatcherTimer();
        public List<double> mesh_point_z;
        public Point oldpoint;
        double old_rotationz;
        double old_rotationx;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(0.1);
            mesh_point_z = new List<double>();
            for (int i = 0; i < mesh3d.Positions.Count; i++)
            {
                mesh_point_z.Add(mesh3d.Positions[i].Z);
            }
            ZIN.Value = 1;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (move_type == "y+")
            {
                rotationy.Angle += 1;
            }
           else if (move_type == "y-")
            {
                rotationy.Angle -= 1;
            }
          else  if (move_type == "x-")
            {
                rotationx.Angle -= 1;
            }
            else if (move_type == "x+")
            {
                rotationx.Angle += 1;
            }
            else if (move_type == "z+")
            {
                rotationz.Angle += 1;
            }
            else if (move_type == "z-")
            {
                rotationz.Angle -= 1;
            }

            else if (move_type == "r+")
            {
                cam.FieldOfView--;
            }
            else if (move_type == "r-")
            {
                cam.FieldOfView++;
            }


        }


        private void Ya_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
            rotationy.Angle += 1;
            move_type = "y+";
            timer.Start();
        }

        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
        }

        private void Yd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rotationy.Angle -= 1;
            move_type = "y-";
            timer.Start();
        }

        private void Xd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rotationx.Angle -= 1;
            move_type = "x-";
            timer.Start();
        }

        private void Xa_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rotationx.Angle += 1;
            move_type = "x+";
            timer.Start();
        }

        private void Za_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rotationz.Angle += 1;
            move_type = "z+";
            timer.Start();
        }

        private void Zd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            rotationz.Angle -= 1;
            move_type = "z-";
            timer.Start();
        }

        private void rst_Click(object sender, RoutedEventArgs e)
        {
            rotationx.Angle = 0;
            rotationy.Angle = 0;
            rotationz.Angle = 0;
            cam.FieldOfView = 100;
             for (int i = 0; i < mesh3d.Positions.Count; i++)
            {
                mesh3d.Positions[i] = new Point3D(mesh3d.Positions[i].X, mesh3d.Positions[i].Y, mesh_point_z[i]);
            }
            ZIN.Value = 1;
                
        }

        private void Ra_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cam.FieldOfView--;
            move_type = "r+";
            timer.Start();
        }
        private void Rd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            cam.FieldOfView++;
            move_type = "r-";
            timer.Start();
        }
        private void ZIN_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < mesh3d.Positions.Count; i++)
            {
                mesh3d.Positions[i] = new Point3D(mesh3d.Positions[i].X, mesh3d.Positions[i].Y, mesh_point_z[i] * ZIN.Value);
            }
        }

        private void Viewport3D_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if (e.GetPosition(base_vp).X > oldpoint.X)
                //{
                //    rotationz.Angle -= 1;
                //}

                rotationz.Angle = old_rotationz + (oldpoint.X - e.GetPosition(base_vp).X) / 5;
                rotationx.Angle = old_rotationx + (e.GetPosition(base_vp).Y - oldpoint.Y) / 5;

            }
        }

        private void Viewport3D_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            oldpoint = e.GetPosition(base_vp);
            old_rotationz = rotationz.Angle;
            old_rotationx = rotationx.Angle;
        }

        private void base_vp_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                cam.FieldOfView--;
            }
            else if (e.Delta < 0)
            {
                cam.FieldOfView++;
            }
        }

      

     
        

      
    }
}
