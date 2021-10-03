<p align="center">
  <img src="https://user-images.githubusercontent.com/24313771/130939816-62267ff3-533c-462f-aed5-f12da5610828.png" width="350" title="Salad Slicer Logo">
</p>
  
  <br>
  
 <p align="center"> 
  <img src="https://img.shields.io/github/v/release/3DCP-TUe/SaladSlicer?label=stable&style=flat-square">
  <img src="https://img.shields.io/github/v/release/3DCP-TUe/SaladSlicer?label=latest&include_prereleases&style=flat-square">
  <img src="https://img.shields.io/github/downloads/3DCP-TUe/SaladSlicer/total?style=flat-square">
  <img src="https://img.shields.io/github/license/3DCP-TUe/SaladSlicer?style=flat-square">
  <img src="https://img.shields.io/github/issues-raw/3DCP-TUe/SaladSlicer?style=flat-square">
  <img src="https://img.shields.io/github/issues-closed-raw/3DCP-TUe/SaladSlicer?style=flat-square">
</p>

---

Salad Slicer is an open source Rhinoceros Grasshopper plugin developed for slicing objects for 3D concrete printing, however, the plugin is also suitable for slicing objects to print other materials such as ceramics or polymers. Among other functionalities, Salad Slicer offers: 

- Slicing planar 2.5D objects from a 2D curve
- Slicing planar 3D objects from a 3D mesh
- Possibility to define your own slicers and/or reconstruct and customize the pre-defined slicers with the components from the Geometry category. 
- Possibility to construct an NC program (G-code) for the sliced geometry. 
- Possibility to convert all sliced objects to planes that can be used to construct programs for, for example, ABB and KUKA robots. 

## Getting Started
You can download the latest release directly from this repository's [releases page](https://github.com/3DCP-TUe/SaladSlicer/releases). Download the `SaladSlicer.gha` file and place it in your Grasshopper components folder (in GH, File > Special Folders > Components Folder). Make sure that the file is unblocked (right-click on the file and select `Properties` from the menu, click `Unblock` on the `General` tab). Restart Rhino and you are ready to go!

You can find a collection of example files demonstrating the main features of Salad Slicer in this repository in the folder [Example Files](https://github.com/3DCP-TUe/SaladSlicer/tree/master/ExampleFiles). 

## Contribute

**Bug reports**: Please report bugs at our [issue page](https://github.com/3DCP-TUe/SaladSlicer/issues). 

**Feature requests**: Feature request can be proposed on our [issue page](https://github.com/3DCP-TUe/SaladSlicer/issues). Please include how this feature should work by explaining it in detail and if possible by adding relevant documentation.

**Code contributions**: We accept code contributions through [pull requests](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests). For this you have to [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) or [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) this repository. To compile the code all necessary references are placed in the folder [DLLs](https://github.com/3DCP-TUe/SaladSlicer/tree/master/DLLs). We only accept code contributions if they are commented. You can read more about this topic [here](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc). If you want to make a significant contribution, please let us know what you want to add or change to avoid doing things twice. For questions or if you want to discuss your contribution you can reach out to one of the [developers](https://github.com/3DCP-TUe/SaladSlicer/graphs/contributors). 

## Credits
Salad Slicer is an open source project that is developed and initiated by the [3D Concrete Printing Research Group at Eindhoven University of Technology](https://www.tue.nl/en/research/research-groups/structural-engineering-and-design/3d-concrete-printing/). The technical development is executed by the PhD and PDEng candidates who are listed [here](https://github.com/3DCP-TUe/SaladSlicer/graphs/contributors).

We would like to acknowledge the authors of [Robot Components](https://github.com/RobotComponents/RobotComponents) for making their Grasshopper plugin available and open source. The code structure of Salad Slicer is heavily influenced by [Robot Components](https://github.com/RobotComponents/RobotComponents). As an acknowledgement, we have provided [Example Files](https://github.com/3DCP-TUe/SaladSlicer/tree/master/ExampleFiles) wherein we demonstrate how to use Salad Slicer in combination with [Robot Components](https://github.com/RobotComponents/RobotComponents). 

## Cite Salad Slicer
Salad Slicer is a free to use Grasshopper plugin and does not legally bind you to cite it. However, we have invested time and effort in creating Salad Slicer and would appreciate a citation if used. To cite Salad Slicer in publications use:

```
3D Concrete Printing Research Group at Eindhoven University of Technology (2021). 
Salad Slicer v0.1.0: A slicer for 3D concrete printing. 
URL https://github.com/3DCP-TUe/SaladSlicer
```

Note that there are two reasons for citing the software used. One is giving recognition to the work done by others which we already addressed. The other is giving details on the system used so that experiments can be replicated. For this, you should cite the version of Salad Slicer used. See [How to cite and describe software](https://software.ac.uk/how-cite-software) for more details and an in-depth discussion.

## Version numbering
Salad Slicer uses the following [Semantic Versioning](https://semver.org/) scheme: 

```
0.x.x ---> MAJOR version: incompatible changes. 
x.0.x ---> MINOR version: functionality added in a backwards compatible manner.  
x.x.0 ---> PATCH version: backwards compatible bug fixes.
```

## License
Salad Slicer

Copyright (c) 2021 [The Salad Slicer contributors](https://github.com/3DCP-TUe/SaladSlicer/graphs/contributors)

Salad Slicer is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. 

Salad Slicer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Salad Slicer; If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>
