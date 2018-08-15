using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;

namespace GHEnergyPlus
{
    public class GHEPlusRunParametric10 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GHEPlusRunParametric10 class.
        /// </summary>
        public GHEPlusRunParametric10()
            : base("Prob10Kämpf", "Prob10Kämpf",
                "Problem 10 Kämpf & Wetter 2010, large office, with VAV and reheat.",
                "EnergyHubs", "BuildingSimulation")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //0 - 5
            pManager.AddTextParameter("idf", "idf", "idf file name. has to be in C:\\eplus\\EPOpti17\\Input\\", GH_ParamAccess.item);
            pManager.AddTextParameter("weather", "weather", "weather file name. has to be in \\WeatherData of your Energyplus folder", GH_ParamAccess.item);
            pManager.AddBooleanParameter("run", "run", "Run the simulation", GH_ParamAccess.item);
            pManager.AddIntegerParameter("sleep", "sleep", "sleep. default is 1500", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddIntegerParameter("folder", "folder", "folder number, like 1,2,3, for parallel runs", GH_ParamAccess.item);
            pManager[4].Optional = true;
            pManager.AddNumberParameter("------------", "------------", "------------", GH_ParamAccess.item);
            pManager[5].Optional = true;

            //6 - 19
            pManager.AddNumberParameter("N_win_low", "x[0]", "North window lower position in [m] ∈ [0.8, 1.25].", GH_ParamAccess.item);
            pManager.AddNumberParameter("N_win_up", "x[1]", "North window upper position in [m] ∈ [1.35, 2.2].", GH_ParamAccess.item);
            pManager.AddNumberParameter("E_win_low", "x[2]", "East window lower position in [m] ∈ [0.8, 1.25].", GH_ParamAccess.item);
            pManager.AddNumberParameter("E_win_up", "x[3]", "East window upper position in [m] ∈ [1.35, 2.2].", GH_ParamAccess.item);
            pManager.AddNumberParameter("S_win_low", "x[4]", "South window lower position in [m] ∈ [0.8, 1.25].", GH_ParamAccess.item);
            pManager.AddNumberParameter("S_win_up", "x[5]", "South window upper position in [m] ∈ [1.35, 2.2].", GH_ParamAccess.item);
            pManager.AddNumberParameter("W_win_low", "x[6]", "West window lower position in [m] ∈ [0.8, 1.25].", GH_ParamAccess.item);
            pManager.AddNumberParameter("W_win_up", "x[7]", "West window upper position in [m] ∈ [1.35, 2.2].", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolSup", "x[8]", "Zone cooling design supply air temperature in [°] ∈ [12, 18]", GH_ParamAccess.item);
            pManager.AddNumberParameter("HeatSB_1", "x[9]", "Heating setback night set point temperatures for weekdays and Saturdays in [°] ∈ [13, 21]", GH_ParamAccess.item);
            pManager.AddNumberParameter("HeatSB_2", "x[10]", "Heating setback whole day set point temperatures for Sundays and holidays in [°] ∈ [13, 21]", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolSB_1", "x[11]", "Cooling setback night set point temperatures for weekdays and Saturdays in [°] ∈ [24, 36]", GH_ParamAccess.item);
            pManager.AddNumberParameter("CoolSB_2", "x[12]", "Cooling setback whole day set point temperatures for Sundays and holidays in [°] ∈ [24, 36]", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("MJ/m2", "MJ/m2", "Primary energy consumption of electricity and gas in [MJ/m2] for HVAC, lighting, interior equipment.", GH_ParamAccess.item);
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


            //get input parameters
            int dvar = 13;
            double[] x = new double[dvar];
            for (int i = 0; i < x.Length; i++)
                if (!DA.GetData(i + 6, ref x[i])) { return; };


            if (runit == true)
            {
                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                //modify idf file with parameters and save as new idf file
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
                string[] replacethis = new string[dvar + 17];
                for (int i = 0; i < replacethis.Length; i++)
                    replacethis[i] = @"%" + i.ToString() + @"%";


                //replacers
                string[] replacers = new string[replacethis.Length];
                for (int i = 0; i < dvar; i++)
                    replacers[i] = x[i].ToString();

                for (int i = dvar; i < dvar + 8; i++)
                    replacers[i] = (x[i - dvar] + 16.7683).ToString();

                for (int i = dvar + 8; i < dvar + 16; i++)
                    replacers[i] = (x[i - dvar - 8] + 33.5366).ToString();

                replacers[29] = (x[8] * 0.0006875 - 0.00025).ToString();


                //constraints
                bool[] gx = new bool[4];        //true means constraint violation
                if (x[0] - x[1] + 0.5488 >= 0) gx[0] = true;
                if (x[2] - x[3] + 0.5488 >= 0) gx[1] = true;
                if (x[4] - x[5] + 0.5488 >= 0) gx[2] = true;
                if (x[6] - x[7] + 0.5488 >= 0) gx[3] = true;


                //scan string for keywords and replace them with parameters
                for (int i = 0; i < lines.Length; i++)
                    for (int u = 0; u < replacethis.Length; u++)
                        lines[i] = lines[i].Replace(replacethis[u], replacers[u]);




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
                //process Outputs
                string outputfile = "eplustbl.csv";
                while (!File.Exists(path_out + outputfile))
                {
                    Console.WriteLine("waiting");
                }



                //output result 
                double result = double.NaN;
                //identify correct result file. load it. get the right numbers from it
                lines = new string[] { };
                list = new List<string>();
                fileStream = new FileStream(path_out + outputfile, FileMode.Open, FileAccess.Read);
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

                double Fx;


                string[] split;
                char delimiter = ',';

                split = lines[16].Split(delimiter);
                string MJm2 = split[4];

                Fx = (Convert.ToDouble(gx[0]) * 0.5 +
                    Convert.ToDouble(gx[1]) * 0.5 +
                    Convert.ToDouble(gx[2]) * 0.5 +
                    Convert.ToDouble(gx[3]) * 0.5 +
                    1) * Convert.ToDouble(MJm2);


                result = Fx;
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

            }

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
                return GHEnergyPlus.Properties.Resources.opti_10;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e1419975-7521-4f7e-9ab7-de899a0f8340}"); }
        }
    }
}