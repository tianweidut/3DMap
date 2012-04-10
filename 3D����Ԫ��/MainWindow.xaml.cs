using _3DTools;          //3D tools 
using System;
using System.IO;
using System.Diagnostics;
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
using Petzold;
using Petzold.Text3D;

using System.Windows.Threading;
       

namespace _D基本元素
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private GeometryModel3D mGeometry;
        private GeometryModel3D[] mGeometryAdd;
        private GeometryModel3D[] txt3D;
        private DrawOperator draw;

        private bool mDown;
        private bool mDownRight;
        private Point mLastPos;
        private Point StartPoint;
        private int AddCnt = 0;
        private int AddCntTxt = 0;

        private int MapCnt = 0;     //原始地图计数
        private int PathHave = 0;   //是否添加巡检路径
        private int DataHave = 0;   //是否添加巡检数据
        private int MapHave = 0;    //是否添加地图

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        public MainWindow()
        {
            this.InitializeComponent();
            //BuildSolid();   //导入图形入口

            // 在此点下面插入创建对象所需的代码。
            // 初始化
            // Geometry creation
            mGeometryAdd = new GeometryModel3D[100];
            txt3D = new GeometryModel3D[100];
            AddCnt = 0;
            AddCntTxt = 0;
            
           /*DiffuseMaterial tmp = new DiffuseMaterial();
            SolidColorBrush solid = new SolidColorBrush();
            solid.Opacity = 0.5;
            tmp.Brush = new SolidColorBrush(solid);
              mGeometry = new GeometryModel3D(test, tmp);*/

            mGeometry = new GeometryModel3D(test, new DiffuseMaterial(Brushes.YellowGreen));
           
            mGeometry.Transform = new Transform3DGroup();
            group.Children.Add(mGeometry);
            MapHave = 1;
            MapCnt++;

            draw = new DrawOperator();

            PathHave = 0;   //是否添加巡检路径
            DataHave = 0;   //是否添加巡检数据


        }

        private void BuildSolid()
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-10.5, -10.5, 10));     //Z轴正1处平面
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(10.5, -10.5, 1));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(10.5, 10.5, 10));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));
            mesh.Positions.Add(new Point3D(-10.5, 10.5, 10));
            //mesh.Normals.Add(new Vector3D(0, 0, 1));

            mesh.Positions.Add(new Point3D(-10, -10, -10));        //Z轴-1处平面
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(10, -10, -10));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(10, 10, -10));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));
            mesh.Positions.Add(new Point3D(-10, 10, -10));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            // Geometry creation
            mGeometryAdd[AddCnt] = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Blue));
            mGeometryAdd[AddCnt].Transform = new Transform3DGroup();
            group.Children.Add(mGeometryAdd[AddCnt]);     //将形状变化加入到group中
            AddCnt++;
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - e.Delta / 5D);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mDown)
            {
                this.Cursor = Cursors.Cross;
                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X, dy = actualPos.Y - mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0) mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));

                group = mGeometry.Transform as Transform3DGroup;
                group.Children.Add(new RotateTransform3D(r));

                for (int i = 0; i < AddCnt; i++)
                {
                    if (mGeometryAdd[i] != null)
                    {
                        group = mGeometryAdd[i].Transform as Transform3DGroup;
                        group.Children.Add(new RotateTransform3D(r));
                    }
                }

                for (int i = 0; i < AddCntTxt; i++)
                {
                    if (txt3D[i] != null)
                    {
                        group = txt3D[i].Transform as Transform3DGroup;
                        group.Children.Add(new RotateTransform3D(r));
                    }
                }

                for (int i = 0;i<draw.AddCnt ;i++ )
                {
                    if (draw.mGeometryAdd[i]!=null)
                    {
                        group = draw.mGeometryAdd[i].Transform as Transform3DGroup;
                        group.Children.Add(new RotateTransform3D(r));
                    }
                }
                for (int i = 0; i < draw.AddCntTxt; i++)
                {
                    if (draw.txt3D[i] != null)
                    {
                        group = draw.txt3D[i].Transform as Transform3DGroup;
                        group.Children.Add(new RotateTransform3D(r));
                    }
                }
                mLastPos = actualPos;

            }

            if (mDownRight)
            {
                this.Cursor = Cursors.SizeAll;
                Point mousePos = e.GetPosition(viewport);
                Vector3D diff = new Vector3D((mousePos.X - StartPoint.X) / 15, (StartPoint.Y - mousePos.Y) / 15, 0); //Y轴旋转，比例500参数
                TranslateTransform3D trans = new TranslateTransform3D(diff);

                Transform3DGroup group;
                group = mGeometry.Transform as Transform3DGroup;
                group.Children.Add(trans);

                for (int i = 0; i < AddCnt; i++)
                {
                    if (mGeometryAdd[i] != null)
                    {
                        group = mGeometryAdd[i].Transform as Transform3DGroup;
                        group.Children.Add(trans);
                    }
                }

                textTest.Origin = new Point(textTest.Origin.X + diff.X, textTest.Origin.Y + diff.Y);

                for (int i = 0; i < AddCntTxt; i++)
                {
                    if (txt3D[i] != null)
                    {
                        group = txt3D[i].Transform as Transform3DGroup;
                        group.Children.Add(trans);  
                    }
                }
                //Path
                for (int i = 0;i<draw.AddCnt ;i++ )
                {
                    if (draw.mGeometryAdd[i]!=null)
                    {
                        group = draw.mGeometryAdd[i].Transform as Transform3DGroup;
                        group.Children.Add(trans);
                    }
                }
                //data
                for (int i = 0; i < draw.AddCntTxt; i++)
                {
                    if (draw.txt3D[i] != null)
                    {
                        group = draw.txt3D[i].Transform as Transform3DGroup;
                        group.Children.Add(trans);
                    }
                }
                                
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mDown = true;
                Point pos = Mouse.GetPosition(viewport);
                mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                mDownRight = true;
                StartPoint = Mouse.GetPosition(viewport);
                //StartPoint = new Point(StartPoint.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - StartPoint.Y);
            }

        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
            mDownRight = false;
            this.Cursor = Cursors.Arrow;
        }

        private void btnPath_Click(object sender, RoutedEventArgs e)
        {
            if (PathHave == 0)
            {
                //根据数据库打印巡检路线
                //step1：复位当前视图  
                camerReset();
                ResetView();
                //step2：根据数据库和配置文件打印信息
                draw.GetAddInfo();
                if (1 == DataHave)
                {
                    //如果有数据正在显示，需要恢复成标准视角
                    for (int i = 0; i < draw.AddCntTxt; i++)
                    {
                        if (draw.txt3D[i] != null)
                        {
                            group.Children.Remove(draw.txt3D[i]);
                        }
                    }

                    draw.AddCntTxt = 0;
                    draw.DrawInfoText(group);
      
                }

                draw.DrawInfoPath(group);
                PathHave = 1;   //添加巡检路线
                //MessageBox.Show("path");
            }
            this.btnOnlyPath.IsEnabled = true;
        }

        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            if (DataHave == 0)
            {
                //根据数据库打印巡检点数据
                //step1：复位当前视图
                camerReset();
                ResetView();
                //step2：根据数据库和配置文件打印信息
                draw.GetAddInfo();
                if (1 == PathHave)
                {
                    //如果有数据正在显示，需要恢复成标准视角
                    for (int i = 0; i < draw.AddCnt; i++)
                    {
                        if (draw.mGeometryAdd[i] != null)
                        {
                            group.Children.Remove(draw.mGeometryAdd[i]);
                        }
                    }
                    draw.AddCnt = 0;
                    draw.DrawInfoPath(group);
                     
                }                
                draw.DrawInfoText(group);
                DataHave = 1;   //添加巡检数据
                //MessageBox.Show("data");
            }
        }


        private void btnZoomUp_Click_1(object sender, RoutedEventArgs e)
        {
            //zoom+
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - 15);
        }

        private void btnZoomDown_Click(object sender, RoutedEventArgs e)
        {
            //zoom-
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z + 15);
        }
        private void ResetView()
        {
     
                mGeometry.Transform = new Transform3DGroup();
                group.Children.Add(mGeometry);
                MapCnt++;
            

            /*for (int i = 0; i < draw.AddCnt; i++)
            {
                if (draw.mGeometryAdd[i] != null)
                {
                    draw.mGeometryAdd[i].Transform = new Transform3DGroup();
                    group.Children.Add(draw.mGeometryAdd[i]);
                }
            }

            for (int i = 0; i < draw.AddCntTxt; i++)
            {
                if (draw.txt3D[i] != null)
                {
                    draw.txt3D[i].Transform = new Transform3DGroup();
                    group.Children.Add(draw.txt3D[i]);
                }
            }*/

        }
        private GeometryModel3D CreateTextLabel3D(string text,Point3D basePoint,double height)
        {
            //文字
            TextBlock textblock = new TextBlock(new Run(text));
            textblock.Foreground = Brushes.Red;
            textblock.FontFamily = new FontFamily("Arial");
            //材料
            DiffuseMaterial mataterLabel = new DiffuseMaterial();
            mataterLabel.Brush = new VisualBrush(textblock);
            //坐标点
            double width = text.Length * height; 
            // p0: the lower left corner;  p1: the upper left
            // p2: the lower right; p3: the upper right
            Point3D p0 = basePoint;
            Point3D p1 = new Point3D(p0.X,p0.Y+height,p0.Z);      //此处应该为方向向量
            Point3D p2 = new Point3D(p0.X+width,p0.Y,p0.Z);
            Point3D p3 = new Point3D(p2.X,p2.Y+height,p2.Z);


            //矩形方框
            MeshGeometry3D mg_box = new MeshGeometry3D();
            mg_box.Positions = new Point3DCollection();
            //点
            mg_box.Positions.Add(p0);   //0
            mg_box.Positions.Add(p1);   //1
            mg_box.Positions.Add(p2);   //2
            mg_box.Positions.Add(p3);   //3
            mg_box.Positions.Add(p0);   //4
            mg_box.Positions.Add(p1);   //5
            mg_box.Positions.Add(p2);   //6
            mg_box.Positions.Add(p3);   //7
            //三角形面
            mg_box.TriangleIndices.Add(0);
            mg_box.TriangleIndices.Add(3);
            mg_box.TriangleIndices.Add(1);

            mg_box.TriangleIndices.Add(0);
            mg_box.TriangleIndices.Add(2);
            mg_box.TriangleIndices.Add(3);

            mg_box.TriangleIndices.Add(4);
            mg_box.TriangleIndices.Add(5);
            mg_box.TriangleIndices.Add(7);

            mg_box.TriangleIndices.Add(4);
            mg_box.TriangleIndices.Add(7);
            mg_box.TriangleIndices.Add(6);

            //拉伸
            mg_box.TextureCoordinates.Add(new Point(0,1));
            mg_box.TextureCoordinates.Add(new Point(0,0));
            mg_box.TextureCoordinates.Add(new Point(1,1));
            mg_box.TextureCoordinates.Add(new Point(1,0));
            mg_box.TextureCoordinates.Add(new Point(1,1));
            mg_box.TextureCoordinates.Add(new Point(1,0));
            mg_box.TextureCoordinates.Add(new Point(0,1));
            mg_box.TextureCoordinates.Add(new Point(0,0));

            //加入Group
            GeometryModel3D tmp = new GeometryModel3D(mg_box,mataterLabel);
           return tmp;

        }

        private void camerReset()
           {
                camera.Position = new Point3D(442.969970703127, 136.104995727539, 2228.20190607796);
                camera.LookDirection = new Vector3D(0, 0, -2247.7769068409);
                camera.UpDirection = new Vector3D(0, 1, 0);
                camera.NearPlaneDistance = 0.1;
                camera.FarPlaneDistance = 6508.0557228115213;
                camera.FieldOfView = 45;
            }

        private void btnRestartMap_Click(object sender, RoutedEventArgs e)
        {
            //按钮重启，仅显示地图，不显示数据和路线
            //step1：删除界面元素
            if (PathHave == 1)
            {
                for (int i = 0; i < draw.AddCnt; i++)
                {
                    if (draw.mGeometryAdd[i] != null)
                    {
                        group.Children.Remove(draw.mGeometryAdd[i]);
                    }
                }
                PathHave = 0;
                draw.AddCnt = 0;
            }

            if (DataHave == 1)
            {
                //data,删除巡检数据量
                for (int i = 0; i < draw.AddCntTxt; i++)
                {
                    if (draw.txt3D[i] != null)
                    {
                        group.Children.Remove(draw.txt3D[i]);
                    }
                }
                DataHave = 0;
                draw.AddCntTxt = 0;
            }
                      
            //step2：重置摄像头
            camerReset();
            //step3：重绘地图
            ResetView();
            //step4：参数初始化
           // draw.AddCnt = 0;
           // draw.AddCntTxt = 0;
        }

        private void btnFirstView_Click(object sender, RoutedEventArgs e)
        {
           
            //step2：重置摄像头
            camerReset();
        }

        private void btnOnlyPath_Click(object sender, RoutedEventArgs e)
        {
            //仅保留巡检路线和巡检数据，删除原始地图
            //step2：删除元素
            if(MapHave == 1)
            {
                for (int i = 0; i < MapCnt; i++)
                {
                    if (mGeometry != null)
                    {
                        group.Children.Remove(mGeometry);
                    }
                }
                MapCnt = 0;
                MapHave = 0;
            }
            //step1：复位当前视图
            camerReset();
        }

    }
}
