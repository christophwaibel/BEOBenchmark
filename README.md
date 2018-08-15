# BEOBenchmark
This repository aims to provide a Building Energy Optimization (BEO) problem test bed for benchmarking black-box optimization algorithms. Previous benchmarking studies in BEO usually included very few problems, thus inhibiting a fair and traceable performance comparison of optimization algorithms. By providing a larger set of BEO problems, we can facilitate reproducible future benchmarking studies, where researchers can focus on improving and designing algorithms suited to solve building energy design problems. Furthermore, it is crucial that benchmarks are conducted on the same problem set in order to make generalizable conclusions. 

The first test bed version [BEOBenchmark_V1](../master/BEOBenchmark_V1) is now online and is used in a forthcoming journal publication. This test bed includes 15 BEO problems all using EnergyPlus as simulator and Rhinoceros 3D Grasshopper as software platform. An easy interface for connecting any optimization algorithm (library) to Grasshopper is given with the [FrOG](https://github.com/Tomalwo/FrOG) tool (**F**ramework **f**or **O**ptimization in **G**rasshopper) by T. Wortmann and A. Zuardin.
\
\
Further developments on the test bed could include more EnergyPlus problems, problems using other simulators (such as [Modelica](https://simulationresearch.lbl.gov/modelica/), [CitySim](https://citysim.epfl.ch/), or [BRCM](https://brcm.ethz.ch/doku.php)), or implementations in other software platforms such as [GenOpt](https://simulationresearch.lbl.gov/GO/). Contributions and collaborations are more than welcome to improve the test bed!
\
\
Contact us under: chwaibel@student.ethz.ch and thomas_wortmann@mymail.sutd.edu.sg
