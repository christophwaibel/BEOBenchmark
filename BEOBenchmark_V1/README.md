**BEOBenchmark_V1**\
BEOBenchmark_V1 contains 15 BEO problems using [EnergyPlus V 8.5.0](https://github.com/NREL/EnergyPlus/releases/tag/v8.5.0) as simulator and Rhinoceros 3D (5.0) and Grasshopper as software platform. For connecting your own optimization algorithm to Rhino Grasshopper, you may want to use [FrOG](https://github.com/Tomalwo/FrOG) as an interface. 

How-to:

1. Download the [GHEnergyPlus.gha](../BEOBenchmark_V1/GHEnergyPlus.gha) file and put it into your Grasshopper components folder. You can find this folder by entering "GrasshopperFolders", "Components" into your Rhino command line. Make sure that the "read-only"-option of the .gha file properties is unchecked.
2. Create a folder "C:/eplus/EPOpti17". 
3. Create another folder inside that folder: "C:/eplus/EPOpti17/Output0".
4. Download [Input](../BEOBenchmark_V1/Input), put it into "C:/eplus/EPOpti17", and rename it to "C:/eplus/EPOpti17/Input0". You can copy paste this "Input0" folder and give it different indices, e.g. "Input1", "Input2", etc. This is useful if you want to run several processes in parallel (careful, your Grasshopper optimization component and/or optimization library needs to support it).
5. Download the [Grasshopper problem files folder](../BEOBenchmark_V1/Grasshopper) and put it anywhere you want. This folder contains all 15 problems of the test bed.
6. Start Rhino as admin, otherwise it won't work. You are good to go.
\
\
\
Should you want to re-implement the problems in another platform (e.g. GenOpt), you can still use the .idf files frmo the Input folder. All the necessary code (i.e. objective function and idf-file modification) can be found in the [C# source code](../BEOBenchmark_V1/Sourcecode). Only with BEO problem 15 this could become tricky, since here geometry is modified using the Rhino geometry library.
\
\
\
The 15 problems are from following studies (please cite respectively, should you publish own work using any of these problems): 
* M. Wetter & J. Wright (2004). A comparison of deterministic and probabilistic optimization algorithms for nonsmooth simulation-based optimization. In: Building and Environment 39:8, 989-999. (Problems 1 to 8)
* J. Kämpf, M. Wetter, D. Robinson (2010). A comparison of global optimization algorithms with standard benchmark functions and real-world applications using EnergyPlus. In: Journal of Building Performance Simulation 3:2, 103-120. (Problems 9 and 10)
* A. Nguyen & S. Reiter (2014). Passive designs and strategies for low-cost housing using simulation-based optimization and different thermal comfort criteria. In: Journal of Building Performance Simulation 7:1, 68-81. (Problems 11 and 12)
* N. Djuric, V. Novakovic, J. Holst, Z. Mitrovic (2007). Optimization of energy consumption in buildings with hydronic heating systems considering thermal comfort by use of computer-based tools. In: Energy and Buildings 39:4, 471-477. (Problem 13)
* A.P. Ramallo-González & D.A. Coley (2014). Using self-adaptive optimisation methods to perform sequential optimisation for low-energy building design. In: Energy and Buildings 81, 18-29. (Problem 14)
* C. Waibel, R. Evins, J. Carmeliet (2016). Holistic Optimization of Urban Morphology and District Energy Systems. In: Systems Thinking in the Built Environment, Sustainable Built Environment (SBE) Regional Conference Zurich, June 15th – 17th. Ed.: G. Habert and A. Schlüter. (Problem 15)
\
\
\
For questions, contact us: chwaibel@student.ethz.ch and thomas_wortmann@mymail.sutd.edu.sg

