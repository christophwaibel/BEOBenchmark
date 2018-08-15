using System;
using System.Collections.Generic;
using System.Threading;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;


namespace GHEnergyPlus
{
    public class GHEPlusRunParametric4to8 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GHEPlusRunDetailedParametric class.
        /// </summary>
        public GHEPlusRunParametric4to8()
            : base("WetterDetailedPara", "WetterDetailedPara",
                "Run WetterDetailed idf, output kWh/m2a as number. Input parameters.",
                "EnergyHubs", "BuildingSimulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("idf", "idf", "idf file name. has to be in C:\\eplus\\EPOpti17\\Input\\", GH_ParamAccess.item);
            pManager.AddTextParameter("weather", "weather", "weather file name. has to be in \\WeatherData of your Energyplus folder", GH_ParamAccess.item);
            pManager.AddBooleanParameter("run", "run", "Run the simulation", GH_ParamAccess.item);

            //3-6
            pManager.AddNumberParameter("window l N", "wn", "Window length North in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("window l W", "ww", "Window length West in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("window l E", "we", "Window length East in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("window l S", "ws", "Window length South in [m]", GH_ParamAccess.item);

            //7-9
            pManager.AddNumberParameter("overhang W", "ow", "overhang depth West in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("overhang E", "oe", "overhang depth East in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("overhang S", "os", "overhang depth South in [m]", GH_ParamAccess.item);

            //10-12
            pManager.AddNumberParameter("shading W", "sw", "shading setpoint West in [W/m2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("shading E", "se", "shading setpoint East in [W/m2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("shading S", "ss", "shading setpoint South in [W/m2]", GH_ParamAccess.item);

            //13-15
            pManager.AddNumberParameter("setpoint night summer", "tu", "setpoint night cooling summer in [°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("setpoint night winter", "ti", "setpoint night cooling winter in [°C]", GH_ParamAccess.item);
            pManager.AddNumberParameter("cooling supply temp", "td", "cooling design supply air temperature in [°C]", GH_ParamAccess.item);

            pManager.AddNumberParameter(" ", " ", " ", GH_ParamAccess.item);
            pManager[16].Optional = true;
            pManager.AddIntegerParameter("sleep", "sleep", "sleep. default is 1500", GH_ParamAccess.item);
            pManager[17].Optional = true;

            pManager.AddIntegerParameter("folder", "folder", "folder number, like 1,2,3, for parallel runs", GH_ParamAccess.item);
            pManager[18].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("kWh/m2a", "kWh/m2a", "annual specific primary energy consumption for cooling, heating, fan, lighting in [kWh/m2a]", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int sleeptime = 1500;
            if (!DA.GetData(17, ref sleeptime)) { sleeptime = 1500; }


            int folderint = 0;
            if (!DA.GetData(18, ref folderint)) { folderint = 0; }
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


            //get input parameters
            double wn = double.NaN;
            double ww = double.NaN;
            double we = double.NaN;
            double ws = double.NaN;
            if (!DA.GetData(3, ref wn)) { return; };
            if (!DA.GetData(4, ref ww)) { return; };
            if (!DA.GetData(5, ref we)) { return; };
            if (!DA.GetData(6, ref ws)) { return; };

            double ow = double.NaN;
            double oe = double.NaN;
            double os = double.NaN;
            if (!DA.GetData(7, ref ow)) { return; }
            if (!DA.GetData(8, ref oe)) { return; }
            if (!DA.GetData(9, ref os)) { return; }

            double sw = double.NaN;
            double se = double.NaN;
            double ss = double.NaN;
            if (!DA.GetData(10, ref sw)) { return; }
            if (!DA.GetData(11, ref se)) { return; }
            if (!DA.GetData(12, ref ss)) { return; }

            double tu = double.NaN;
            double ti = double.NaN;
            double td = double.NaN;
            if (!DA.GetData(13, ref tu)) { return; }
            if (!DA.GetData(14, ref ti)) { return; }
            if (!DA.GetData(15, ref td)) { return; }






            if (runit == true)
            {
                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                //modify idf file with parameters and save as new idf file
                //string now = DateTime.Now.ToString("h:mm:ss");
                //now = now.Replace(':', '_');
                string idfmodified = idffile + "_modi";

                //load idf into a huge string
                string[] lines;
                var list = new List<string>();
                var fileStream = new FileStream(path_in + idffile + ".idf", FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
                lines = list.ToArray();
                fileStream.Close();

                //to be replaced
                string[] replacethis = new string[29];
                replacethis[0] = @"%x0WinNor%";
                replacethis[1] = @"%x1WinNor%";
                replacethis[2] = @"%y0WinNor%";
                replacethis[3] = @"%x0OveNor%";
                replacethis[4] = @"%x1OveNor%";

                replacethis[5] = @"%x0WinWes%";
                replacethis[6] = @"%x1WinWes%";
                replacethis[7] = @"%y0WinWes%";
                replacethis[8] = @"%x0OveWes%";
                replacethis[9] = @"%x1OveWes%";
                replacethis[10] = @"%yOveWes%";

                replacethis[11] = @"%x0WinEas%";
                replacethis[12] = @"%x1WinEas%";
                replacethis[13] = @"%y0WinEas%";
                replacethis[14] = @"%x0OveEas%";
                replacethis[15] = @"%x1OveEas%";
                replacethis[16] = @"%yOveEas%";

                replacethis[17] = @"%x0WinSou%";
                replacethis[18] = @"%x1WinSou%";
                replacethis[19] = @"%y0WinSou%";
                replacethis[20] = @"%x0OveSou%";
                replacethis[21] = @"%x1OveSou%";
                replacethis[22] = @"%yOveSou%";

                replacethis[23] = @"%shaWes%";
                replacethis[24] = @"%shaEas%";
                replacethis[25] = @"%shaSou%";

                replacethis[26] = @"%TZonCooNigSum%";
                replacethis[27] = @"%TZonCooNigWin%";
                replacethis[28] = @"%CooDesSupTem%";


                //replacers
                string[] replacers = new string[29];
                replacers[0] = (3.0 - (wn / 2.0)).ToString();       //x0
                replacers[1] = (3.0 + (wn / 2.0)).ToString();       //x1
                replacers[2] = (0.6).ToString();                    //y0
                replacers[3] = (3.0 - (wn / 2.0) - 0.5).ToString(); //x0 over
                replacers[4] = (3.0 + (wn / 2.0) + 0.5).ToString(); //x1 over

                replacers[5] = (12.0 - (ww / 2.0)).ToString();      //x0
                replacers[6] = (12.0 + (ww / 2.0)).ToString();      //x1
                replacers[7] = (0.6).ToString();
                replacers[8] = (12.0 - (ww / 2.0) - 0.5).ToString();
                replacers[9] = (12.0 + (ww / 2.0) + 0.5).ToString();
                replacers[10] = (ow + 8.0).ToString();

                replacers[11] = (12.0 - (we / 2.0)).ToString();      //x0
                replacers[12] = (12.0 + (we / 2.0)).ToString();      //x1
                replacers[13] = (0.6).ToString();
                replacers[14] = (12.0 - (we / 2.0) - 0.5).ToString();
                replacers[15] = (12.0 + (we / 2.0) + 0.5).ToString();
                replacers[16] = (oe + 8.0).ToString();


                replacers[17] = (3.0 - (ws / 2.0)).ToString();       //x0
                replacers[18] = (3.0 + (ws / 2.0)).ToString();       //x1
                replacers[19] = (0.6).ToString();                    //y0
                replacers[20] = (3.0 - (ws / 2.0) - 0.5).ToString(); //x0 over
                replacers[21] = (3.0 + (ws / 2.0) + 0.5).ToString(); //x1 over
                replacers[22] = (os + 8.0).ToString();

                replacers[23] = sw.ToString();
                replacers[24] = se.ToString();
                replacers[25] = ss.ToString();

                replacers[26] = tu.ToString();
                replacers[27] = ti.ToString();
                replacers[28] = td.ToString();


                //scan string for keywords and replace them with parameters
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int u = 0; u < replacethis.Length; u++)
                    {
                        lines[i] = lines[i].Replace(replacethis[u], replacers[u]);
                    }
                }


                //write a new idf file
                File.WriteAllLines(path_in + idfmodified + ".idf", lines);
                string idffilenew = path_in + idfmodified + ".idf";
                string weatherfilein = path_in + @"ep\WeatherData\" + weatherfile + ".epw";



                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                //run eplus
                string command = @" -w " + weatherfilein + @" -d " + path_out + @" " + idffilenew;
                Misc.RunEplus(eplusexe, command);

                //System.Threading.Thread.Sleep(sleeptime);





                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                while (!File.Exists(path_out + "eplusout.eso"))
                {
                    Console.WriteLine("waiting");
                }
                //System.Threading.Thread.Sleep(sleeptime);


                //output result (kWh/m2a) 
                double result = double.NaN;
                //identify correct result file. load it. get the right numbers from it
                lines = new string[] { };
                list = new List<string>();
                fileStream = new FileStream(path_out + "eplusout.eso", FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                }
                lines = list.ToArray();
                fileStream.Close();


                double primEnElec = 3.0;
                double primEnGas = 1.0;

                string[] split;
                char delimiter = ',';
                //split = lines[49].Split(delimiter );
                //string heat = split[3];
                //double dblHeat = Convert.ToDouble(heat) * primEnGas;

                //split = lines[50].Split(delimiter);
                //string cool = split[2];
                //double dblCool = Convert.ToDouble(cool) * primEnElec;

                //split = lines[51].Split(delimiter);
                //string light = split[2];
                //double dblLight = Convert.ToDouble(light) * primEnElec;

                //split = lines[55].Split(delimiter);
                //string fan = split[2];
                //double dblFan = Convert.ToDouble(fan) * primEnElec;
                split = lines[28].Split(delimiter);
                string heat_north = split[1];
                split = lines[29].Split(delimiter);
                string heat_west = split[1];
                split = lines[30].Split(delimiter);
                string heat_east = split[1];
                split = lines[31].Split(delimiter);
                string heat_south = split[1];
                split = lines[32].Split(delimiter);
                string heat_interior = split[1];
                split = lines[33].Split(delimiter);
                string heat_main = split[1];
                double dblHeat = (Convert.ToDouble(heat_north) + Convert.ToDouble(heat_west)
                    + Convert.ToDouble(heat_east) + Convert.ToDouble(heat_south)
                    + Convert.ToDouble(heat_interior) + Convert.ToDouble(heat_main))
                    * primEnGas / 3600000;

                split = lines[36].Split(delimiter);
                string cool = split[1];
                double dblCool = Convert.ToDouble(cool) * primEnElec / 3600000;

                split = lines[23].Split(delimiter);
                string light_north = split[1];
                split = lines[24].Split(delimiter);
                string light_west = split[1];
                split = lines[25].Split(delimiter);
                string light_east = split[1];
                split = lines[26].Split(delimiter);
                string light_south = split[1];
                split = lines[27].Split(delimiter);
                string light_interior = split[1];
                double dblLight = (Convert.ToDouble(light_north) * 5.0 + Convert.ToDouble(light_west)
                    + Convert.ToDouble(light_east) + Convert.ToDouble(light_south) * 5.0 + Convert.ToDouble(light_interior))
                    * primEnElec / 3600000;  //zone north and south need to be multiplied with 5 (5 rooms). This is not considered in the .eso

                split = lines[34].Split(delimiter);
                string fan_supply = split[1];
                split = lines[34].Split(delimiter);
                string fan_return = split[1];
                double dblFan = (Convert.ToDouble(fan_supply) + Convert.ToDouble(fan_return)) * primEnElec / 3600000;

                result = (dblHeat + dblCool + dblLight + dblFan) / 1104;   //1104 is the square meter     


                System.Threading.Thread.Sleep(sleeptime);
                System.IO.File.Delete(path_in + idfmodified + ".idf");
                System.IO.DirectoryInfo di = new DirectoryInfo(path_out);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                // System.Threading.Thread.Sleep(sleeptime);
                DA.SetData(0, result);
            }

        }


        static void RunEplus(string eplusexe, string command)
        {
            var outt = System.Diagnostics.Process.Start(eplusexe, command);

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
                return GHEnergyPlus.Properties.Resources.opti_4to8;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{c5d2ca48-1ce7-4a20-b1d6-5f2e7e361b32}"); }
        }
    }
}