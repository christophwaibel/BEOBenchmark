using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;

namespace GHEnergyPlus
{
    public class GHEPlusRunParametric12A : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GHEPlusRunParametric12 class.
        /// </summary>
        public GHEPlusRunParametric12A()
            : base("Prob12NguyenA_NV", "Prob12A_NV",
                "Problem 12 A Nat.Vent., adaptive comfort with Natural Ventilation (objective function III, eqt. 6), Nguyen & Reiter 2014",
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

            //6 - 23
            pManager.AddNumberParameter("azimuth", "x[0]", "Building azimuth in [°] ∈ {-90,..., 90}, stepsize 30; or ∈ [-90, 90] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("width", "x[1]", "Building width in [m] ∈ {4,..., 10}, stepsize 2; or ∈ [4, 10] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("osize1", "x[2]", "South overhang in [m] ∈ {0.2,..., 0.8}, stepsize 0.3; or ∈ [0.2, 0.8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("osize2", "x[3]", "North overhang in [m] ∈ {0.2,..., 0.8}, stepsize 0.3; or ∈ [0.2, 0.8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("osize3", "x[4]", "East overhang in [m] ∈ {0.2,..., 0.8}, stepsize 0.3; or ∈ [0.2, 0.8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("osize4", "x[5]", "West overhang in [m] ∈ {0.2,..., 0.8}, stepsize 0.3; or ∈ [0.2, 0.8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("wwidth1", "x[6]", "South window width in [m] ∈ {5,..., 8}, stepsize 1; or ∈ [5, 8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("wwidth2", "x[7]", "North window width in [m] ∈ {5,..., 8, stepsize 1; or ∈ [5, 8] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("wwidth3", "x[8]", "West window width in [m] ∈ {0.5,..., 2.5}, stepsize 1; or ∈ [0.5, 2.5] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("wwidth4", "x[9]", "East window width in [m] ∈ {0.5,..., 2.5}, stepsize 1; or ∈ [0.5, 2.5] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("absor", "x[10]", "External wall absorptance ∈ {0.3,..., 0.9}, stepsize 0.3; or ∈ [0.3, 0.9] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("infil", "x[11]", "Window crack infiltration in [kg/s m] ∈ {0.002,..., 0.006}, stepsize 0.002; or ∈ [0.002, 0.006] ⊂ ℝ.", GH_ParamAccess.item);
            pManager.AddNumberParameter("tmass", "x[12]", "Thermal mass ∈ {600, 601, 602}.", GH_ParamAccess.item);
            pManager.AddNumberParameter("floortype", "x[13]", "Floor types ∈ {500, 501, 502}.", GH_ParamAccess.item);
            pManager.AddNumberParameter("rooftype", "x[14]", "Roof type ∈ {300, 301, 302}.", GH_ParamAccess.item);
            pManager.AddNumberParameter("wintype", "x[15]", "Window type ∈ {200, 201, 202}.", GH_ParamAccess.item);
            pManager.AddNumberParameter("exwall", "x[16]", "External walls ∈ {100, 101, 102, 103}.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("TotCost", "TotCost", "Life Cycle Cost, see eq. (6) in Nguyen&Reiter (2014). f(x) = f_c(x) + f_0,50(x).", GH_ParamAccess.item);
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
            int dvar = 17;
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
                string[] replacethis = new string[dvar + 1];
                replacethis[0] = @"%azimuth%";
                replacethis[1] = @"%width%";
                replacethis[2] = @"%osize1%";
                replacethis[3] = @"%osize2%";
                replacethis[4] = @"%osize3%";
                replacethis[5] = @"%osize4%";
                replacethis[6] = @"%wwidth1%";
                replacethis[7] = @"%wwidth2%";
                replacethis[8] = @"%wwidth3%";
                replacethis[9] = @"%wwidth4%";
                replacethis[10] = @"%absor%";
                replacethis[11] = @"%infil%";
                replacethis[12] = @"%tmass%";
                replacethis[13] = @"%floortype%";
                replacethis[14] = @"%rooftype%";
                replacethis[15] = @"%wintype%";
                replacethis[16] = @"%exwall%";
                replacethis[17] = @"%length%";


                //replacers
                string[] replacers = new string[dvar + 1];
                for (int i = 0; i < dvar; i++)
                    replacers[i] = x[i].ToString();

                double xlength = 100 / x[1];
                replacers[17] = xlength.ToString();


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

                split = lines[591].Split(delimiter);
                string LCC = split[2];


                Fx = Convert.ToDouble(LCC);



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
                return GHEnergyPlus.Properties.Resources.opti_12;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{3f7e7c91-ccea-475c-a351-10a1f84d98ef}"); }
        }
    }
}