using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using _3DTools;          //3D tools 
using System.IO;
using System.Diagnostics;
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
using Petzold.MeshGeometries;


namespace _D基本元素
{
    class DrawOperator      //绘制直线块，文本类
    {
        private string[,] AddInfoArray = new string[100, 4];    //存储地点表信息
        private string Cur_X, Cur_Y, Cur_Z;
        private int AddInfoCnt;
       
        private IniOperator tmpIni;                             //文件操作类
        private SqlHelperExample testSql;                       //数据库操作类
        
        public GeometryModel3D[] mGeometryAdd;
        public int AddCnt = 0;
        public GeometryModel3D[] txt3D;
        public int AddCntTxt = 0;

        public int cnt = 100;

        public DrawOperator()
        {
            Cur_X = Convert.ToString(0);
            Cur_Y = Convert.ToString(0);
            Cur_Z = Convert.ToString(0);
            AddInfoCnt = 0;

            tmpIni = new IniOperator();     //操作文件
            testSql = new SqlHelperExample();

            mGeometryAdd = new GeometryModel3D[cnt];
            AddCnt = 0;
            txt3D = new GeometryModel3D[cnt];
            AddCntTxt = 0;
        }
        public void GetAddInfo()
        {
            //获取地点表信息
            DataSet result = new DataSet();
            string sql = "select Aid,X3D,Y3D,Z3D from AddressList";
            result =  testSql.Query(sql);
            for (int i = 0;i< result.Tables[0].Rows.Count ;i++ )
            {
                AddInfoArray[i, 0] = result.Tables[0].Rows[i]["Aid"].ToString();
                AddInfoArray[i, 1] = result.Tables[0].Rows[i]["X3D"].ToString();
                AddInfoArray[i, 2] = result.Tables[0].Rows[i]["Y3D"].ToString();
                AddInfoArray[i, 3] = result.Tables[0].Rows[i]["Z3D"].ToString();
            }
            AddInfoCnt = result.Tables[0].Rows.Count;
        }
        private void GetCurrentXYZ(string Aid)
        {
            for (int i = 0;i<AddInfoCnt ;i++ )
            {
                if (string.Equals(Aid,AddInfoArray[i,0]))
                {
                    Cur_X = AddInfoArray[i, 1];
                    Cur_Y = AddInfoArray[i, 2];
                    Cur_Z = AddInfoArray[i, 3];
                }
            }
        }
        public void DrawInfoText(Model3DGroup group)     //绘制文本
        {
            string sql = "";
            string str = "";
            Boolean flag = false;
            DataSet result = new DataSet();
            for (int i = 0;i<AddInfoCnt ;i++ )
            {
                flag = false;
                sql = @"select Data,Pattern,state,GetDate,GetTime from PatrolList where GetDate = '" + GetShowDate() + "' and Aid = '" + AddInfoArray[i, 0] + "'";
                result = testSql.Query(sql);
                for (int j = 0;j<result.Tables[0].Rows.Count ;j++ )
                {
                    flag = true;
                    if (GetDateCheck()) str = str + result.Tables[0].Rows[j]["Data"].ToString() +" " +result.Tables[0].Rows[j]["Pattern"].ToString()+" ";
                    if (GetDateCheck()) str = str + result.Tables[0].Rows[j]["GetDate"].ToString() + " " + result.Tables[0].Rows[j]["GetTime"].ToString()+" ";
                    if (GetDateCheck()) str = str + result.Tables[0].Rows[j]["state"].ToString()+" ";
                    str = str + "  ";
                }
                //绘制汉字文本
                if (flag)
                {
                    AddText(AddInfoArray[i, 1],         //X
                            AddInfoArray[i, 2],         //Y
                            AddInfoArray[i, 3],         //Z
                            str,                        //文本
                            GetHeight(),         //高度
                            GetWeight(),         //宽度
                            GetColor(),
                            group);
                }
                str = "";
            }
        }
        public void DrawInfoPath(Model3DGroup group)     //绘制路线
        {
            string startX, startY, startZ;
            string endX, endY, endZ;
            string sql, seqstr;
            int PointCnt = 0;
            int PathColor = 0;
            string lineWeight = "";
            
            DataSet result = new DataSet();
            sql = "select pname,lineColor,lineWeight,lineSequence,PointCnt from pathList ";
            result = testSql.Query(sql); 
            for (int i = 0;i<result.Tables[0].Rows.Count ;i++ )
            {
                if (GetShowPath().IndexOf(result.Tables[0].Rows[i]["pname"].ToString()) != -1)  //包含巡检路线名称字符串
                {
                    //包含字串，提取序列属性
                    seqstr = result.Tables[0].Rows[i]["lineSequence"].ToString();
                    if ((result.Tables[0].Rows[i]["PointCnt"] == null) || (result.Tables[0].Rows[i]["PointCnt"].ToString() == ""))
                    {
                        PointCnt = 0;
                    }
                    else
                    {
                        //PointCnt = int.Parse(result.Tables[0].Rows[i]["PointCnt"].ToString());
                        PointCnt = int.Parse((result.Tables[0].Rows[i]["PointCnt"]).ToString());
                    }
                    if (PointCnt >2)
                    {
                        string[] splitStr = seqstr.Split(new char[2]{'\r','\n'} );
                        for (int j = 0,k=0;j < PointCnt ;j++ )
                        {
                            GetCurrentXYZ(splitStr[k]); startX = Cur_X; startY = Cur_Y; startZ = Cur_Z;
                            GetCurrentXYZ(splitStr[k+2]); endX = Cur_X; endY = Cur_Y; endZ = Cur_Z;
                            PathColor = int.Parse((result.Tables[0].Rows[i]["lineColor"]).ToString());
                            lineWeight = (int.Parse((result.Tables[0].Rows[i]["lineWeight"]).ToString())/5).ToString();
                            k += 2;
                            AddLineRight(startX, startY, startZ,
                                    endX, endY, endZ,
                                    lineWeight,
                                    PathColor,
                                    group);
                        }
                    }
                    

                }
            }

        }

        private Boolean GetDateCheck()     //获取ini文件状态信息
        {
            int rst = int.Parse(tmpIni.IniReadValue("文字显示", "数据量"));
            if (rst == -1)
            {
                //true
                return true;
            }
            return false;
            
        }
        private Boolean GetTimeCheck()
        {
            int rst = int.Parse(tmpIni.IniReadValue("文字显示", "时间"));
            if (rst == -1)
            {
                //true
                return true;
            }
            return false;
        }
        private Boolean GetStateCheck()
        {
            int rst = int.Parse(tmpIni.IniReadValue("文字显示", "状态"));
            if (rst == -1)
            {
                //true
                return true;
            }
            return false;
        }
        private string GetHeight()
        {
            return tmpIni.IniReadValue("基本参数", "高度");
        }
        private string GetWeight()
        {
            return tmpIni.IniReadValue("基本参数", "宽度");
        }
        private int GetColor()
        {
            return int.Parse(tmpIni.IniReadValue("基本参数", "颜色"));
        }
        private string GetShowPath()
        {
            return tmpIni.IniReadValue("基本参数", "巡检路线");
        }
        private string GetShowDate()
        {
            return tmpIni.IniReadValue("基本参数", "巡检日期");
        }
        private string GetColorR()
        {
            return tmpIni.IniReadValue("颜色参数", "R");
        }
        private string GetColorG()
        {
            return tmpIni.IniReadValue("颜色参数", "G");
        }
        private string GetColorB()
        {
            return tmpIni.IniReadValue("颜色参数", "B");
        }

        public void AddText(string X, string Y, string Z, string text, string Height, string weight, int color, Model3DGroup group)          //加入文本
        {
            Point3D pot = new Point3D(double.Parse(X), double.Parse(Y), double.Parse(Z));

            txt3D[AddCntTxt] = CreateTextLabel3D(text, pot, double.Parse(Height),color);
            txt3D[AddCntTxt].Transform = new Transform3DGroup();
            group.Children.Add(txt3D[AddCntTxt]);     //将形状变化加入到group中

            AddCntTxt++;
        }
        public void AddLine(string X1, string Y1, string Z1, string X2, string Y2, string Z2, string R, int Color,Model3DGroup group)    //加入直线
        {
            //将圆柱体进行三角形剖分
            MeshGeometry3D mesh = new MeshGeometry3D();

            Point3D start3DPoint = new Point3D(double.Parse(X1), double.Parse(Y1), double.Parse(Z1));
            Point3D end3DPoint = new Point3D(double.Parse(X2), double.Parse(Y2), double.Parse(Z2));
            
            Vector3D CircleVector = end3DPoint - start3DPoint;
            Vector3D tmp3D = new Vector3D(1, 1, 1);

            if(Vector3D.AngleBetween(tmp3D,CircleVector)< 0.1)
            {
                tmp3D = new Vector3D(1, 0, 0);
            }

            Vector3D A3D = Vector3D.CrossProduct(CircleVector, tmp3D);
            Vector3D B3D = Vector3D.CrossProduct(CircleVector, A3D);
            A3D.Normalize();
            B3D.Normalize();

            double theta;
            Point3D Pos1, Pos2;
            int stacks = 128;    //Stacks表示纬度剖分数

            for(int i =0 ;i< stacks;i++)
            {
                theta = i * Math.PI *2 /stacks ;
                Pos1 = int.Parse(R) * (A3D * Math.Cos(theta) + B3D * Math.Sin(theta)) + start3DPoint;
                Pos2 = int.Parse(R) * (A3D * Math.Cos(theta) + B3D * Math.Sin(theta)) + end3DPoint;

                mesh.Positions.Add(Pos1);
                mesh.Positions.Add(Pos2);

                mesh.TextureCoordinates.Add(new Point(i/stacks,0));     //略有不同
                mesh.TextureCoordinates.Add(new Point(i/stacks,1));
            }

            for (int i = 0; i< stacks ;i++ )
            {
                mesh.TriangleIndices.Add(i * 2);
                mesh.TriangleIndices.Add(i * 2 + 1);
                mesh.TriangleIndices.Add(i * 2 + 3);

                mesh.TriangleIndices.Add(i * 2);
                mesh.TriangleIndices.Add(i * 2 + 3);
                mesh.TriangleIndices.Add(i * 2 + 2);

            }
            mGeometryAdd[AddCnt] = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Red));
            mGeometryAdd[AddCnt].Transform = new Transform3DGroup();
            group.Children.Add(mGeometryAdd[AddCnt]);     //将形状变化加入到group中
            AddCnt++;

        }
        public void AddLine2(string X1, string Y1, string Z1, string X2, string Y2, string Z2, string R, int Color, Model3DGroup group)    //加入直线
        {
            //将圆柱体进行三角形剖分
            MeshGeometry3D mesh = new MeshGeometry3D();
            int Slices = 128;
            Point3D start3DPoint = new Point3D(int.Parse(X1), int.Parse(Y1), int.Parse(Z1));
            Point3D end3DPoint = new Point3D(int.Parse(X2), int.Parse(Y2), int.Parse(Z2));

            for (int i = 0; i < Slices; i++)
            {
                // Points around top of cylinder
                double x = Math.Sin(2 * i * Math.PI / Slices);
                double z = Math.Cos(2 * i * Math.PI / Slices);
                mesh.Positions.Add(new Point3D(x, 100, z));

                // Points around bottom of cylinder
                x = Math.Sin((2 * i + 1) * Math.PI / Slices);
                z = Math.Cos((2 * i + 1) * Math.PI / Slices);
                mesh.Positions.Add(new Point3D(x, 0, z));
            }
            // Points at center of top and bottom
            mesh.Positions.Add(new Point3D(0, 1, 0));
            mesh.Positions.Add(new Point3D(0, 0, 0));

            for (int i = 0; i < Slices; i++)
            {
                // Triangles along length of cylinder
                mesh.TriangleIndices.Add(2 * i + 0);
                mesh.TriangleIndices.Add(2 * i + 1);
                mesh.TriangleIndices.Add((2 * i + 2) % (2 * Slices));

                mesh.TriangleIndices.Add((2 * i + 2) % (2 * Slices));
                mesh.TriangleIndices.Add(2 * i + 1);
                mesh.TriangleIndices.Add((2 * i + 3) % (2 * Slices));

                // Triangles on top
                mesh.TriangleIndices.Add(2 * Slices);
                mesh.TriangleIndices.Add(2 * i + 0);
                mesh.TriangleIndices.Add((2 * i + 2) % (2 * Slices));

                // Triangles on bottom
                mesh.TriangleIndices.Add(2 * Slices + 1);
                mesh.TriangleIndices.Add((2 * i + 3) % (2 * Slices));
                mesh.TriangleIndices.Add(2 * i + 1);
            }

            mGeometryAdd[AddCnt] = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Red));
            mGeometryAdd[AddCnt].Transform = new Transform3DGroup();
            group.Children.Add(mGeometryAdd[AddCnt]);     //将形状变化加入到group中
            AddCnt++;

        }
        public void AddLineRight(string X1, string Y1, string Z1, string X2, string Y2, string Z2, string R, int colorint, Model3DGroup group)    //加入直线
        {
            //将圆柱体进行三角形剖分
            MeshGeometry3D mesh = new MeshGeometry3D();
            AxisAngleRotation3D rotate = new AxisAngleRotation3D();
            RotateTransform3D xform = new RotateTransform3D(rotate);

            int slices = 72;
            int stacks = 10;
            double Fold1 = 0.1 , Fold2 = 0.1;

            mesh.Positions = new Point3DCollection((slices + 1) * (stacks + 5) - 2);
            mesh.Normals = new Vector3DCollection((slices + 1) * (stacks + 5) - 2);
            mesh.TriangleIndices = new Int32Collection(6 * slices * (stacks + 1));
            mesh.TextureCoordinates = new PointCollection((slices + 1) * (stacks + 5) - 2);

            /////////////////////////////////////////////////////////////////////////
            //Generate the positions
            /////////////////////////////////////////////////////////////////////////
            Point3DCollection points = mesh.Positions;
            mesh.Positions = null;
            points.Clear();
            // Unhook Normals property and prepare for new vectors
            Vector3DCollection norms = mesh.Normals;
            mesh.Normals = null;
            norms.Clear();

            Point3D start3DPoint = new Point3D(int.Parse(X1), int.Parse(Y1), int.Parse(Z1));
            Point3D end3DPoint = new Point3D(int.Parse(X2), int.Parse(Y2), int.Parse(Z2));

            double radius1 = double.Parse(R);
            double radius2 = double.Parse(R);

            // vectRearRadius always points towards -Z (when possible)
            Vector3D CircleVector = end3DPoint - start3DPoint;
            Vector3D vectRearRadius;

            if (CircleVector.X == 0 && CircleVector.Y == 0)
            {
                vectRearRadius = new Vector3D(0, -1, 0);
            }
            else
            {
                // Find vector axis 90 degrees from cylinder where Z == 0
                rotate.Axis = Vector3D.CrossProduct(CircleVector, new Vector3D(0, 0, 1));
                rotate.Angle = -90;

                // Rotate cylinder 90 degrees to find radius vector
                vectRearRadius =CircleVector * xform.Value;
                vectRearRadius.Normalize();

            }
            // Will rotate radius around cylinder axis
            rotate.Axis = CircleVector;

            for (int i=0;i<=slices;i++)
            {
                // Rotate rear-radius vector 
                rotate.Angle = i * 360 / slices;
                Vector3D vectRadius = vectRearRadius * xform.Value;

                for (int j = 0; j <= stacks; j++)
                {
                    // Find points from top to bottom
                    Point3D pointCenter = start3DPoint + j * (end3DPoint - start3DPoint) / stacks;
                    double radius = radius1 + j * (radius2 - radius1) / stacks;
                    points.Add(pointCenter + radius * vectRadius);

                    norms.Add(vectRadius);
                }

                // Points on top and bottom
                points.Add(start3DPoint + radius1 * vectRadius);
                points.Add(end3DPoint + radius2 * vectRadius);

                // But normals point towards ends
                norms.Add(start3DPoint - end3DPoint);
                norms.Add(end3DPoint - start3DPoint);
            }
            // Add multiple center points on top and bottom ends
            for (int i = 0; i < slices; i++)
            {
                points.Add(start3DPoint);     // top end
                points.Add(end3DPoint);     // bottom end

                norms.Add(start3DPoint - end3DPoint);
                norms.Add(end3DPoint - start3DPoint);
            }
            // Set Normals and Positions properties from re-calced vectors
            mesh.Normals = norms;
            mesh.Positions = points;

            /////////////////////////////////////////////////////////////////////////
            //Generate the TriangleIndices collection
            /////////////////////////////////////////////////////////////////////////
            Int32Collection indices = mesh.TriangleIndices;
            mesh.TriangleIndices = null;
            indices.Clear();

            int indexTopPoints = (stacks + 3) * (slices + 1);

            for (int i = 0; i < slices; i++)
            {
                for (int j = 0; j < stacks; j++)
                {
                    // Triangles running length of cylinder
                    indices.Add((stacks + 3) * i + j);
                    indices.Add((stacks + 3) * i + j + stacks + 3);
                    indices.Add((stacks + 3) * i + j + 1);

                    indices.Add((stacks + 3) * i + j + 1);
                    indices.Add((stacks + 3) * i + j + stacks + 3);
                    indices.Add((stacks + 3) * i + j + stacks + 3 + 1);
                }

                // Triangles on top of cylinder
                indices.Add(indexTopPoints + 2 * i);
                indices.Add((stacks + 3) * i + (stacks + 3) * 2 - 2);
                indices.Add((stacks + 3) * i + (stacks + 3) - 2);

                // Triangles on bottom of cylinder
                indices.Add(indexTopPoints + 2 * i + 1);
                indices.Add((stacks + 3) * i + (stacks + 3) - 1);
                indices.Add((stacks + 3) * i + (stacks + 3) * 2 - 1);
            }
            mesh.TriangleIndices = indices;

            /////////////////////////////////////////////////////////////////////////
            //Generate the TextureCoordinates collection
            /////////////////////////////////////////////////////////////////////////
            PointCollection pts = mesh.TextureCoordinates;
            mesh.TextureCoordinates = null;
            pts.Clear();

            for (int i = 0; i <= slices; i++)
                {
                    for (int j = 0; j <= stacks; j++)
                    {
                        pts.Add(new Point((double)i / slices, 
                                        Fold1 + j * (Fold2 - Fold1) / stacks));
                    }

                    pts.Add(new Point((double)i / slices, Fold1));
                    pts.Add(new Point((double)i / slices, Fold2));
                }

                 // TextureType == TextureType.Drawing
                
                    for (int i = 0; i < slices; i++)
                    {
                        pts.Add(new Point((2 * i + 1) / (2.0 * slices), 0));
                        pts.Add(new Point((2 * i + 1) / (2.0 * slices), 1));
                    }
                
            mesh.TextureCoordinates = pts;

            Brush brush = new SolidColorBrush(CoverColorFromInt(colorint));
            mGeometryAdd[AddCnt] = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            mGeometryAdd[AddCnt].Transform = new Transform3DGroup();
            group.Children.Add(mGeometryAdd[AddCnt]);     //将形状变化加入到group中
            AddCnt++;

        }

        private GeometryModel3D CreateTextLabel3D(string text, Point3D basePoint, double height,int colorint)
        {

            Brush brush = new SolidColorBrush(CoverColorFromInt(colorint));
            //文字
            TextBlock textblock = new TextBlock(new Run(text));
            textblock.Foreground = brush;
            textblock.FontFamily = new FontFamily("Arial");
            //材料
            DiffuseMaterial mataterLabel = new DiffuseMaterial();
            mataterLabel.Brush = new VisualBrush(textblock);
            //坐标点
            double width = text.Length * height;
            // p0: the lower left corner;  p1: the upper left
            // p2: the lower right; p3: the upper right
            Point3D p0 = basePoint;
            Point3D p1 = new Point3D(p0.X, p0.Y + height, p0.Z);      //此处应该为方向向量
            Point3D p2 = new Point3D(p0.X + width, p0.Y, p0.Z);
            Point3D p3 = new Point3D(p2.X, p2.Y + height, p2.Z);


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
            mg_box.TextureCoordinates.Add(new Point(0, 1));
            mg_box.TextureCoordinates.Add(new Point(0, 0));
            mg_box.TextureCoordinates.Add(new Point(1, 1));
            mg_box.TextureCoordinates.Add(new Point(1, 0));
            mg_box.TextureCoordinates.Add(new Point(1, 1));
            mg_box.TextureCoordinates.Add(new Point(1, 0));
            mg_box.TextureCoordinates.Add(new Point(0, 1));
            mg_box.TextureCoordinates.Add(new Point(0, 0));

            //加入Group
            GeometryModel3D tmp = new GeometryModel3D(mg_box, mataterLabel);
            return tmp;

        }
        public Color CoverColorFromInt(int colorint)
        {
            //将color中RGB提取出来 colorint & 255, (colorint << 8) & 255, ((colorint << 16) & 255)
            /*int a = 255;*/
            int r = colorint & 255;
            int g=  (colorint>>8) & 255 ;
            int b = (colorint>>16) & 255;
            int a = 255;//,r=0,g=255,b=0;

            byte A = Convert.ToByte(a);
            byte R = Convert.ToByte(r);
            byte G = Convert.ToByte(g);
            byte B = Convert.ToByte(b);

            return Color.FromArgb(A, R, G, B);
           
        } 
    }
}
