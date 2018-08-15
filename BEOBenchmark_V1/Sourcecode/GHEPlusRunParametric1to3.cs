using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;

namespace GHEnergyPlus
{
    public class GHEPlusRunParametric1to3 : GH_Component
    {


        public GHEPlusRunParametric1to3()
            : base("WetterSimplePara", "WetterSimplePara",
                "Run WetterSimple idf, output kWh/m2a as number. Input parameters.",
                "EnergyHubs", "BuildingSimulation")
        {
        }



        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("idf", "idf", "idf file name. has to be in C:\\eplus\\EPOpti17\\Input\\", GH_ParamAccess.item);
            pManager.AddTextParameter("weather", "weather", "weather file name. has to be in \\WeatherData of your Energyplus folder", GH_ParamAccess.item);
            pManager.AddBooleanParameter("run", "run", "Run the simulation", GH_ParamAccess.item);

            //3-6
            pManager.AddNumberParameter("orientation", "α", "Building orientation in [°]", GH_ParamAccess.item);
            pManager.AddNumberParameter("window l W", "ww", "Window West width in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("window l E", "we", "Window East width in [m]", GH_ParamAccess.item);
            pManager.AddNumberParameter("transmittance", "τ", "Shading device transmittance [fraction]", GH_ParamAccess.item);

            pManager.AddNumberParameter(" ", " ", " ", GH_ParamAccess.item);
            pManager[7].Optional = true;
            pManager.AddIntegerParameter("sleep", "sleep", "sleep. default is 1500", GH_ParamAccess.item);
            pManager[8].Optional = true;

            pManager.AddIntegerParameter("folder", "folder", "folder number, like 1,2,3, for parallel runs", GH_ParamAccess.item);
            pManager[9].Optional = true;
        }



        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("kWh/m2a", "kWh/m2a", "annual zone loads for cooling, heating, lighting in [kWh/m2a].", GH_ParamAccess.item);
        }



        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int sleeptime = 1500;
            if (!DA.GetData(8, ref sleeptime)) { sleeptime = 1500; }

            int folderint = 0;
            if (!DA.GetData(9, ref folderint)) { folderint = 0; }
            string path_in = @"c:\eplus\EPOpti17\Input" + folderint + @"\";
            string path_out = @"c:\eplus\EPOpti17\Output" + folderint + @"\";
            string eplusbat = @"c:\eplus\EPOpti17\Input" + folderint + @"\ep\RunEPlusEPOpti17_" + folderint + @".bat";
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
            double alpha = double.NaN;
            double ww = double.NaN;
            double we = double.NaN;
            double tau = double.NaN;
            if (!DA.GetData(3, ref alpha)) { return; };
            if (!DA.GetData(4, ref ww)) { return; };
            if (!DA.GetData(5, ref we)) { return; };
            if (!DA.GetData(6, ref tau)) { return; };




            if (runit == true)
            {
                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                //modify idf file with parameters and save as new idf file
                //string now = DateTime.Now.ToString("h:mm:ss");
                //now = now.Replace(':', '_');
                //string idfmodified = idffile + "_" + now;
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
                string[] replacethis = new string[4];
                replacethis[0] = @"%azimuth%";
                replacethis[1] = @"%w_we_win%";
                replacethis[2] = @"%w_ea_win%";
                replacethis[3] = @"%tau%";


                //replacers
                string[] replacers = new string[4];
                replacers[0] = alpha.ToString();
                replacers[1] = ww.ToString();
                replacers[2] = we.ToString();
                replacers[3] = tau.ToString();




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
                double EffHeat = 0.44;
                double EffCool = 0.77;

                string[] split;
                //split = System.Text.RegularExpressions.Regex.Split(lines[49], "\r\n");
                char delimiter = ',';
                split = lines[12].Split(delimiter);
                string light = split[1];
                double dblLight = Convert.ToDouble(light) / 3600000 / 96 * primEnElec;
                split = lines[13].Split(delimiter);
                string heat = split[1];
                double dblHeat = Convert.ToDouble(heat) / 3600000 / 96 / EffHeat;
                split = lines[14].Split(delimiter);
                string cool = split[1];
                double dblCool = Convert.ToDouble(cool) / 3600000 / 96 / EffCool;



                result = (dblHeat + dblCool + dblLight);
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



                DA.SetData(0, result);
                //System.Threading.Thread.Sleep(sleeptime);
            }






        }




        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return GHEnergyPlus.Properties.Resources.opti_1to3;
            }
        }



        public override Guid ComponentGuid
        {
            get { return new Guid("{96e7fe41-181a-483c-b682-c0829550aefb}"); }
        }
    }
}