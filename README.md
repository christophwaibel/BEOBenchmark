# BEOBenchmark
Building Energy Optimization (BEO) problem test bed for benchmarking black-box optimization algorithms. Idea is to provide common an easy accessible set of problems in the building energy context such that testing and comparing optimization algorithms becomes traceable. Previous studies in BEO usually use different and only few BEO problems, thus inhibiting general conclusions and fair comparisons. 

The first version BEOBenchmark_V1 is down here. But everyone interested is invited to collaborate to extend the test set with more representative BEO problems, also coded maybe in different platforms such as GenOpt and using other simulators such as Modelica. Right now all is in EnergyPlus as simulator and Rhinoceros 3D Grasshopper as software platform. An easy interface for connecting any optimization algorithm (library) to Grasshopper is given with the [FrOG](https://github.com/Tomalwo/FrOG) (**F**ramework **f**or **O**ptimization in **G**rasshopper) tool by T. Wortmann and A. Zuardin.
\\
Contact us under: chwaibel@student.ethz.ch and thomas_wortmann@mymail.sutd.edu.sg
\
\
\
\
**BEOBenchmark_V1**\
BEOBenchmark_V1 contains 15 BEO problems using [EnergyPlus V 8.5.0](https://github.com/NREL/EnergyPlus/releases/tag/v8.5.0) as simulator and Rhinoceros 3D Grasshopper as software platform.

link to FrOG, example how to implement .dll. Example how to use external process, ask thomas? open-source Opossum?

The 15 problems are from following studies: 
* M. Wetter & J. Wright (2004). A comparison of deterministic and probabilistic optimization algorithms for nonsmooth simulation-based optimization. In: Building and Environment 39:8, 989-999. (Problems 1 to 8)
* J. Kämpf, M. Wetter, D. Robinson (2010). A comparison of global optimization algorithms with standard benchmark functions and real-world applications using EnergyPlus. In: Journal of Building Performance Simulation 3:2, 103-120. (Problems 9 and 10)
* A. Nguyen & S. Reiter (2014). Passive designs and strategies for low-cost housing using simulation-based optimization and different thermal comfort criteria. In: Journal of Building Performance Simulation 7:1, 68-81. (Problems 11 and 12)
* N. Djuric, V. Novakovic, J. Holst, Z. Mitrovic (2007). Optimization of energy consumption in buildings with hydronic heating systems considering thermal comfort by use of computer-based tools. In: Energy and Buildings 39:4, 471-477. (Problem 13)
* A.P. Ramallo-González & D.A. Coley (2014). Using self-adaptive optimisation methods to perform sequential optimisation for low-energy building design. In: Energy and Buildings 81, 18-29. (Problem 14)
* C. Waibel, R. Evins, J. Carmeliet (2016). Holistic Optimization of Urban Morphology and District Energy Systems. In: Systems Thinking in the Built Environment, Sustainable Built Environment (SBE) Regional Conference Zurich, June 15th – 17th. Ed.: G. Habert and A. Schlüter. (Problem 15)

