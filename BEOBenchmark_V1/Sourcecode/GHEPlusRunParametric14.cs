using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.IO;

namespace GHEnergyPlus
{
    public class GHEPlusRunParametric14 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GHEPlusRunParametric14 class.
        /// </summary>
        public GHEPlusRunParametric14()
            : base("Prob14Gonzalez", "Prob14Gonzalez",
                "Problem 14 Gonzalez & Coley 2014, office room.",
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

            //6 - 26
            pManager.AddNumberParameter("Infiltration", "x[0]", "Infiltration in [m3/s] ∈ [0.021, 0.6].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Aspect ratio", "x[1]", "Aspect ratio in [m/m] ∈ [0.3, 3.0].", GH_ParamAccess.item);
            pManager.AddNumberParameter("U-value windows", "x[2]", "U-value windows in [W/m2K] ∈ [1.7, 5.2].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Fenestration north", "x[3]", "Fenestration north wall fraction ∈ [0.12, 8].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Fenestration south", "x[4]", "Fenestration south wall raction ∈ [0.12, 0.8].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Fenestration east", "x[5]", "Fenestration east wall fraction ∈ [0.12, 0.8].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Fenestration west", "x[6]", "Fenestration west wall fraction ∈ [0.12, 0.8].", GH_ParamAccess.item);
            pManager.AddNumberParameter("Wall type", "x[7]", "Wall construction type ∈ {1,2,3,4}.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Insulation", "x[8]", "Insulation thickness in [mm] ∈ [100, 500]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Conductivity", "x[9]", "Conductivity of internal walls in [W/mK] ∈ [0.2, 2.4]", GH_ParamAccess.item);
            pManager.AddNumberParameter("SHC", "x[10]", "Specific heat capacity of internal walls in [J/kgK] ∈ [200, 3000]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_S_depth", "x[11]", "South overhang depth in [m] ∈ [0, 2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_S_left", "x[12]", "South overhang left extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_S_right", "x[13]", "South overhang right extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_E_depth", "x[14]", "East overhang depth in [m] ∈ [0, 2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_E_left", "x[15]", "East overhang left extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_E_right", "x[16]", "East overhang right extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_W_depth", "x[17]", "West overhang depth in [m] ∈ [0, 2]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_W_left", "x[18]", "West overhang left extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang_W_right", "x[19]", "West overhang right extension in [m] ∈ [0, 5]", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("kWh", "kWh", "Primary energy consumption of heating and cooling in [kWh]. PEF for cooling = 3, heating = 1.", GH_ParamAccess.item);
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
            int dvar = 20;
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
                string[] replacethis = new string[dvar + 71];

                replacethis[0] = @"%x0%"; //infiltration
                int xvar_count = 1;
                for (int i = 1; i < 13; i++)        //aspect ratio
                {
                    replacethis[xvar_count] = @"%x1_p" + i.ToString() + @"_x%";
                    replacethis[xvar_count + 1] = @"%x1_p" + i.ToString() + @"_y%";
                    replacethis[xvar_count + 2] = @"%x1_p" + i.ToString() + @"_z%";
                    xvar_count += 3;
                }
                for (int i = 1; i < 9; i++)
                {
                    replacethis[xvar_count] = @"%x1_pi" + i.ToString() + @"_x%";
                    replacethis[xvar_count + 1] = @"%x1_pi" + i.ToString() + @"_y%";
                    replacethis[xvar_count + 2] = @"%x1_pi" + i.ToString() + @"_z%";
                    xvar_count += 3;
                }
                replacethis[xvar_count] = @"%x2%"; // u-value
                xvar_count++;

                replacethis[xvar_count] = @"%x3_xstart%";         // fenestration xstart, zstart, length, height. North
                replacethis[xvar_count + 1] = @"%x3_zstart%";
                replacethis[xvar_count + 2] = @"%x3_length%";
                replacethis[xvar_count + 3] = @"%x3_height%";
                replacethis[xvar_count + 4] = @"%x4_xstart%";     // South
                replacethis[xvar_count + 5] = @"%x4_zstart%";
                replacethis[xvar_count + 6] = @"%x4_length%";
                replacethis[xvar_count + 7] = @"%x4_height%";
                replacethis[xvar_count + 8] = @"%x5_xstart%";     // East
                replacethis[xvar_count + 9] = @"%x5_zstart%";
                replacethis[xvar_count + 10] = @"%x5_length%";
                replacethis[xvar_count + 11] = @"%x5_height%";
                replacethis[xvar_count + 12] = @"%x6_xstart%";    // West
                replacethis[xvar_count + 13] = @"%x6_zstart%";
                replacethis[xvar_count + 14] = @"%x6_length%";
                replacethis[xvar_count + 15] = @"%x6_height%";
                xvar_count += 16;

                for (int i = 7; i < dvar; i++)
                {
                    replacethis[xvar_count] = @"%x" + i.ToString() + @"%";
                    xvar_count++;
                }



                //replacers
                string[] replacers = new string[replacethis.Length];
                replacers[0] = x[0].ToString();

                double floor_area = 70; //according to paper. but in his file its 100 though
                double[][] x1_p;
                double[][] x1_pi;
                Misc.insert_surface(out x1_p, out x1_pi, floor_area, x[1]);

                xvar_count = 1;
                for (int i = 0; i < 12; i++)        //aspect ratio
                {
                    replacers[xvar_count] = x1_p[i][0].ToString();
                    replacers[xvar_count + 1] = x1_p[i][1].ToString();
                    replacers[xvar_count + 2] = x1_p[i][2].ToString();
                    xvar_count += 3;
                }
                for (int i = 0; i < 8; i++)
                {
                    replacers[xvar_count] = x1_pi[i][0].ToString();
                    replacers[xvar_count + 1] = x1_pi[i][1].ToString();
                    replacers[xvar_count + 2] = x1_pi[i][2].ToString();
                    xvar_count += 3;
                }
                replacers[xvar_count] = x[2].ToString();
                xvar_count++;

                double[] xstart;
                double[] zstart;
                double[] length;
                double[] height;
                Misc.insert_window(out xstart, out zstart, out length, out height, x1_p, new double[4] { x[3], x[4], x[5], x[6] });

                replacers[xvar_count] = xstart[0].ToString();         // fenestration xstart, zstart, length, height. North
                replacers[xvar_count + 1] = zstart[0].ToString();
                replacers[xvar_count + 2] = length[0].ToString();
                replacers[xvar_count + 3] = height[0].ToString();
                replacers[xvar_count + 4] = xstart[1].ToString();     // South
                replacers[xvar_count + 5] = zstart[1].ToString();
                replacers[xvar_count + 6] = length[1].ToString();
                replacers[xvar_count + 7] = height[1].ToString();
                replacers[xvar_count + 8] = xstart[2].ToString();     // East
                replacers[xvar_count + 9] = zstart[2].ToString();
                replacers[xvar_count + 10] = length[2].ToString();
                replacers[xvar_count + 11] = height[2].ToString();
                replacers[xvar_count + 12] = xstart[3].ToString();    // West
                replacers[xvar_count + 13] = zstart[3].ToString();
                replacers[xvar_count + 14] = length[3].ToString();
                replacers[xvar_count + 15] = height[3].ToString();
                xvar_count += 16;

                switch (Convert.ToInt16(x[7]))
                {
                    case 1:
                        replacers[xvar_count] = "Wall_1";
                        break;
                    case 2:
                        replacers[xvar_count] = "Wall_2";
                        break;
                    case 3:
                        replacers[xvar_count] = "Wall_3";
                        break;
                    case 4:
                        replacers[xvar_count] = "Wall_4";
                        break;
                }
                xvar_count++;
                replacers[xvar_count] = (x[8] / 1000).ToString();
                xvar_count++;

                for (int i = 9; i < dvar; i++)
                {
                    replacers[xvar_count] = x[i].ToString();
                    xvar_count++;
                }



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
                string command = @" -w " + weatherfilein + @" -x -d " + path_out + @" -i " + path_in + @"ep\Energy+.idd " + idffilenew;
                string directory = path_out;
                Misc.RunEplus(eplusexe, command, directory);






                //***********************************************************************************
                //***********************************************************************************
                //***********************************************************************************
                //process Outputs
                string outputfile = "eplusout.eso";
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

                split = lines[11].Split(delimiter);
                string heat = split[1];
                split = lines[12].Split(delimiter);
                string cool = split[1];
                double dblheat = 0.001 * Convert.ToDouble(heat) / 3600;
                double dblcool = 0.001 * Convert.ToDouble(cool) / 3600 * 3;

                Fx = dblheat + dblcool;


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
                return GHEnergyPlus.Properties.Resources.opti_14;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{a54c279c-cab1-4284-9a8e-b6fd83430749}"); }
        }
    }
}