# BEOBenchmark
This repository aims to provide a Building Energy Optimization (BEO) problem test bed for benchmarking black-box optimization algorithms. Previous benchmarking studies in BEO usually included very few problems, thus inhibiting a fair and traceable performance comparison of optimization algorithms. By providing a larger set of BEO problems, we can facilitate reproducible future benchmarking studies, where researchers can focus on improving and designing algorithms suited to solve building energy design problems. Furthermore, it is crucial that benchmarks are conducted on the same problem set in order to make generalizable conclusions. 

The first test bed version [BEOBenchmark_V1](https://github.com/christophwaibel/BEOBenchmark/releases/tag/v1.0) includes 15 single-objective BEO problems from the literature, all using EnergyPlus as simulator. Original files are updated to EnergyPlus V 8.5.0 and implemented in Rhinoceros 3D Grasshopper as common software platform. An easy interface for connecting any optimization algorithm (library) to Grasshopper is given with the [FrOG](https://github.com/Tomalwo/FrOG) tool (**F**ramework **f**or **O**ptimization in **G**rasshopper). This test bed is introduced in the journal publication [Building energy optimization: An extensive benchmark of global search algorithms](https://doi.org/10.1016/j.enbuild.2019.01.048). 
\
\
Contributions and collaborations are more than welcome to extend the test bed! Further developments could include more EnergyPlus problems, problems using other simulators (such as [Modelica](https://simulationresearch.lbl.gov/modelica/), [CitySim](https://citysim.epfl.ch/), or [BRCM](https://brcm.ethz.ch/doku.php)), implementations in other software platforms such as [GenOpt](https://simulationresearch.lbl.gov/GO/), or multi-objective problems. 
\
\
Contact us under: chwaibel@student.ethz.ch
