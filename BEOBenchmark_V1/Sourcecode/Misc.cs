using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace GHEnergyPlus
{
    internal static class Misc
    {
        internal static void RunEplus(string FileName, string command)
        {
            string eplusexe = FileName;
            System.Diagnostics.Process P = new System.Diagnostics.Process();
            P.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            P.StartInfo.FileName = eplusexe;
            P.StartInfo.Arguments = command;
            P.Start();
            P.WaitForExit();
        }

        internal static void RunEplus(string FileName, string command, string directory)
        {
            string eplusexe = FileName;
            System.Diagnostics.Process P = new System.Diagnostics.Process();
            P.StartInfo.WorkingDirectory = directory;
            P.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            P.StartInfo.FileName = eplusexe;
            P.StartInfo.Arguments = command;
            P.Start();
            P.WaitForExit();
        }


        internal static void insert_surface(out double[][] p, out double[][] pi, double floor_area, double x1)
        {
            double dist2internal = 2;

            p = new double[12][];
            for (int i = 0; i < 12; i++)
            {
                p[i] = new double[3];
                p[i][0] = 0;
                p[i][1] = 0;
                p[i][2] = 0;
            }

            pi = new double[8][];
            for (int i = 0; i < 8; i++)
            {
                pi[i] = new double[3];
                pi[i][0] = 0;
                pi[i][1] = 0;
                pi[i][2] = 0;
            }

            double a = Math.Sqrt(floor_area * x1);
            double b = Math.Sqrt(floor_area / x1);

            for (int i = 0; i < 12; i++)
            {
                if (i < 4)
                    p[i][2] = 0;
                else
                    p[i][2] = 3;

                if (i > 7)
                    p[i][2] = p[i][2] + 0.6;

                if (i == 0 || i == 1 || i == 4 || i == 5 || i == 8 || i == 9)
                    p[i][0] = 0;
                else
                    p[i][0] = a;

                if (i == 2 || i == 1 || i == 6 || i == 5 || i == 9 || i == 10)
                    p[i][1] = 0;
                else
                    p[i][1] = b;
            }

            pi[1][0] = dist2internal;
            pi[1][1] = dist2internal;
            pi[1][2] = 0;

            for (int i = 0; i < 8; i++)
            {
                if (i < 4)
                    pi[i][2] = 0;
                else
                    pi[i][2] = 3;

                if (i == 0 || i == 1 || i == 4 || i == 5)
                    pi[i][0] = dist2internal;
                else
                    pi[i][0] = a - dist2internal;

                if (i == 2 || i == 1 || i == 6 || i == 5)
                    pi[i][1] = dist2internal;
                else
                    pi[i][1] = b - dist2internal;
            }


        }


        internal static void insert_window(out double[] xstart, out double[] zstart, out double[] length, out double[] height,
            double[][] p, double[] x3to6)
        {
            xstart = new double[4]; //N, S, E, W
            zstart = new double[4];
            length = new double[4];
            height = new double[4];

            createWindow(ref xstart[0], ref zstart[0], ref length[0], ref height[0], p[7], p[3], p[0], p[4], x3to6[0]); // N
            createWindow(ref xstart[1], ref zstart[1], ref length[1], ref height[1], p[5], p[1], p[2], p[6], x3to6[1]); // S
            createWindow(ref xstart[2], ref zstart[2], ref length[2], ref height[2], p[6], p[2], p[3], p[7], x3to6[2]); // E
            createWindow(ref xstart[3], ref zstart[3], ref length[3], ref height[3], p[4], p[0], p[1], p[5], x3to6[3]); // W

        }

        internal static void createWindow(ref double xstart, ref double zstart, ref double length, ref double height,
            double[] p1, double[] p2, double[] p3, double[] p4, double x)
        {
            double[] v1 = new double[3];
            double[] v2 = new double[3];
            v1[0] = p4[0] - p1[0];
            v1[1] = p4[1] - p1[1];
            v1[2] = p4[2] - p1[2];

            v2[0] = p2[0] - p1[0];
            v2[1] = p2[1] - p1[1];
            v2[2] = p2[2] - p1[2];

            double mV1 = Math.Sqrt(Math.Pow(v1[0], 2) + Math.Pow(v1[1], 2) + Math.Pow(v1[2], 2));
            double mV2 = Math.Sqrt(Math.Pow(v2[0], 2) + Math.Pow(v2[1], 2) + Math.Pow(v2[2], 2));

            xstart = mV1 / 2.0 - Math.Sqrt(x) * mV1 / 2.0;
            zstart = mV2 / 2.0 - Math.Sqrt(x) * mV2 / 2.0;

            length = mV1 - 2.0 * xstart;
            height = mV2 - 2.0 * zstart;
        }


        internal static double CalcArea2Dpts(double[][] pts)
        {
            double area = 0;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                area += pts[i][0] * pts[i + 1][1] - pts[i][1] * pts[i + 1][0];
            }
            area += pts[pts.Length - 1][0] * pts[0][1] - pts[pts.Length - 1][1] * pts[0][0];
            area *= 0.5;
            area = Math.Abs(area);
            return area;
        }

        internal static double[] Centroid(double[][] X)
        {
            double[] centroid = new double[X[0].Length];
            for (int i = 0; i < X[0].Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < X.Length; j++)
                {
                    sum += X[j][i];
                }
                centroid[i] = sum / X.Length;
            }
            return centroid;
        }

        internal static double[][] PtsFromOffsetRectangle(List<Point3d> plist, double offsetdistance)
        {

            double[][] ptz = new double[4][];
            for (int i = 0; i < 4; i++)
            {
                ptz[i] = new double[2];
                ptz[i][0] = plist[i].X;
                ptz[i][1] = plist[i].Y;
            }

            double[] dblcen = Misc.Centroid(ptz);
            Point3d cen = new Point3d(dblcen[0], dblcen[1], 0);

            plist.Add(plist[0]);
            PolylineCurve crv = new PolylineCurve(plist);
            crv.MakeClosed(0.001);
            Curve[] offsetcr = crv.Offset(cen, Vector3d.ZAxis, offsetdistance, 0.001, CurveOffsetCornerStyle.None);
            PolylineCurve offsetpl = offsetcr[0].ToPolyline(0, 0, 0, System.Double.MaxValue, 0, 0.001, 0, 0, true);
            Point3d offsetpt0 = offsetpl.Point(0);
            Point3d offsetpt1 = offsetpl.Point(1);
            Point3d offsetpt2 = offsetpl.Point(2);
            Point3d offsetpt3 = offsetpl.Point(3);

            return new double[][] { 
                new double[] {offsetpt0.X, offsetpt0.Y }, 
                new double[] {offsetpt1.X, offsetpt1.Y },
                new double[] {offsetpt2.X, offsetpt2.Y}, 
                new double[] {offsetpt3.X, offsetpt3.Y}};
        }

        /// <summary>
        /// Planar projection of a point to a new plane, using projection matrix C
        /// </summary>
        /// <param name="C"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        internal static double[] TransformPoints(double[,] C, double[] pin)
        {
            double[] p1 = new double[3];
            for (int i = 0; i < 3; i++)                //C*pin;
            {
                p1[i] = 0;
                for (int u = 0; u < 3; u++)
                {
                    p1[i] += C[i, u] * pin[u];
                }
            }
            double px = p1[0] / p1[2];          // px = p1(1) / p1(3);
            double py = p1[1] / p1[2];          // py = p1(2) / p1(3);
            double pz = 1;

            return new double[3] { px, py, pz };            // p = [px;py;1];
        }
    }
}
