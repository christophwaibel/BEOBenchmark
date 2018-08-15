using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;

namespace GHEnergyPlus
{
    public class GHEPlusRunParametric13 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GHEPlusRunParametric13 class.
        /// </summary>
        public GHEPlusRunParametric13()
            : base("Prob13Djuric", "Prob13Djuric", "Problem 13 Djuric (now Nord) 2007, school building.", "EnergyHubs", "BuildingSimulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("idf", "idf", "idf file name. has to be in C:\\eplus\\EPOpti17\\Input\\", (GH_ParamAccess)0);
            pManager.AddTextParameter("weather", "weather", "weather file name. has to be in \\WeatherData of your Energyplus folder", (GH_ParamAccess)0);
            pManager.AddBooleanParameter("run", "run", "Run the simulation", (GH_ParamAccess)0);
            pManager.AddIntegerParameter("sleep", "sleep", "sleep. default is 1500", (GH_ParamAccess)0);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("folder", "folder", "folder number, like 1,2,3, for parallel runs", (GH_ParamAccess)0);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("------------", "------------", "------------", (GH_ParamAccess)0);
            pManager[5].Optional = true;
            pManager.AddNumberParameter("delta", "x[0]", "Insulation thickness in [m] ∈ [0.05, 0.3].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA1", "x[1]", "Radiator1 in Class Zone2, U-factor times area value [W/K] ∈ [400, 800].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA2", "x[2]", "Radiator2 in Work Zone, U-factor times area value [W/K] ∈ [60, 120].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA3", "x[3]", "Radiator3 in Kitchen Zone, U-factor times area value [W/K] ∈ [50, 110].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA4", "x[4]", "Radiator4 in Music Zone, U-factor times area value [W/K] ∈ [80, 160].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA5", "x[5]", "Radiator5 in Service Zone, U-factor times area value [W/K] ∈ [20, 50].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA6", "x[6]", "Radiator6 in Washing Zone, U-factor times area value [W/K] ∈ [80, 180].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA7", "x[7]", "Radiator7 in Big Zone, U-factor times area value [W/K] ∈ [140, 280].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA8", "x[8]", "Radiator8 in Class Zone1, U-factor times area value [W/K] ∈ [380, 760].", (GH_ParamAccess)0);
            pManager.AddNumberParameter("UA9", "x[9]", "Radiator9 in Office Zone, U-factor times area value [W/K] ∈ [350, 700].", (GH_ParamAccess)0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("€", "€", "Total Cost for Energy, Radiators and Insulation in [€].", (GH_ParamAccess)0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int sleeptime = 1500;
            if (!DA.GetData(3, ref sleeptime)) { sleeptime = 1500; }


            int folderint = 0;
            if (!DA.GetData(4, ref folderint)) { folderint = 0; }
            string path_in = @"c:\eplus\EPOpti17\Input" + folderint + @"\";
            string path_out = @"c:\eplus\EPOpti17\Output" + folderint + @"\";
            string eplusexe = @"c:\eplus\EPOpti17\Input" + folderint + @"\ep\energyplus.exe";

            //get idf and weather files
            string idffile = @"blabla";
            if (!DA.GetData(0, ref idffile)) { return; }
            string weatherfile = @"blabla";
            if (!DA.GetData(1, ref weatherfile)) { return; }

            //RUN SIMU
            bool runit = false;
            if (!DA.GetData(2, ref runit)) { return; }

            int dvar = 10;
            double[] x = new double[dvar];
            for (int i = 0; i < x.Length; i++)
                if (!DA.GetData(i + 6, ref x[i])) { return; };





            if (!runit)
                return;
            string idfmodified = idffile + "_modi";
            List<string> list = new List<string>();
            FileStream fileStream = new FileStream(path_in + idffile + ".idf", FileMode.Open, FileAccess.Read);
            using (StreamReader streamReader = new StreamReader((Stream)fileStream))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                    list.Add(line);
            }
            string[] lines = list.ToArray();
            fileStream.Close();
            string[] strArray1 = new string[dvar];
            strArray1[0] = "%delta%";
            strArray1[1] = "%UA1%";
            strArray1[2] = "%UA2%";
            strArray1[3] = "%UA3%";
            strArray1[4] = "%UA4%";
            strArray1[5] = "%UA5%";
            strArray1[6] = "%UA6%";
            strArray1[7] = "%UA7%";
            strArray1[8] = "%UA8%";
            strArray1[9] = "%UA9%";
            string[] strArray2 = new string[strArray1.Length];
            for (int index = 0; index < dvar; ++index)
                strArray2[index] = x[index].ToString();
            for (int index1 = 0; index1 < lines.Length; ++index1)
            {
                for (int index2 = 0; index2 < strArray1.Length; ++index2)
                    lines[index1] = lines[index1].Replace(strArray1[index2], strArray2[index2]);
            }
            File.WriteAllLines(path_in + idfmodified + ".idf", lines);
            string str6 = path_in + idfmodified + ".idf";
            string command = " -w " + (path_in + "ep\\WeatherData\\" + weatherfile + ".epw") + " -x -d " + path_out + " -i " + path_in + "ep\\Energy+.idd " + str6;
            string directory1 = path_out;
            Misc.RunEplus(eplusexe, command, directory1);
            string str7 = "eplusout.eso";
            while (!File.Exists(path_out + str7))
                Console.WriteLine("waiting");
            string[] strArray3 = new string[0];
            List<string> stringList2 = new List<string>();
            FileStream fileStream2 = new FileStream(path_out + str7, FileMode.Open, FileAccess.Read);
            using (StreamReader streamReader = new StreamReader((Stream)fileStream2))
            {
                string str5;
                while ((str5 = streamReader.ReadLine()) != null)
                    stringList2.Add(str5);
            }
            string[] array2 = stringList2.ToArray();
            fileStream2.Close();
            char ch = ',';
            double num2 = Convert.ToDouble(array2[55].Split(ch)[1]) / 3600000.0 * (2520.0 * 0.6 * 0.75 / (19.0 - -11.5)) * 0.034 + Convert.ToDouble(array2[56].Split(ch)[1]) / 1000.0 * 12.42;
            double maxValue = (double)byte.MaxValue;
            double num3 = 0.19 * (x[0] * 100.0) - 0.15;
            double num4 = 10.0;
            double num5 = 1.4 * (maxValue * num3 / num4);
            double num6 = 10.5;
            double num7 = 0.3;
            double num8 = 8.0;
            double num9 = 0.0;
            for (int index = 0; index < 9; ++index)
                num9 += x[index + 1];
            double num10 = num6 / num7 * (num9 / num8) * (1.0 / num4);
            double num11 = num2 + num10 + num5;
            System.Threading.Thread.Sleep(sleeptime);
            File.Delete(path_in + idfmodified + ".idf");
            DirectoryInfo directoryInfo = new DirectoryInfo(path_out);
            foreach (FileSystemInfo file in directoryInfo.GetFiles())
                file.Delete();
            foreach (DirectoryInfo directory2 in directoryInfo.GetDirectories())
                directory2.Delete(true);
            DA.SetData(0, num11);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return GHEnergyPlus.Properties.Resources.opti_13;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{79a2342b-5ba8-49a8-9e0a-499abec2c635}"); }
        }
    }
}