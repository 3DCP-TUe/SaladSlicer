<p align="center">
  <img src="https://user-images.githubusercontent.com/24313771/130939816-62267ff3-533c-462f-aed5-f12da5610828.png" width="350" title="Salad Slicer Logo">
</p>
  
  <br>
  
 <p align="center"> 
  <img src="https://img.shields.io/github/v/release/3DCP-TUe/SaladSlicer?label=stable">
  <img src="https://img.shields.io/github/v/release/3DCP-TUe/SaladSlicer?label=latest&include_prereleases">
  <img src="https://img.shields.io/github/downloads/3DCP-TUe/SaladSlicer/total?">
  <img src="https://img.shields.io/github/license/3DCP-TUe/SaladSlicer?">
  <a href="https://zenodo.org/doi/10.5281/zenodo.7818240"><img src="https://zenodo.org/badge/348778701.svg" alt="DOI"></a>
</p>

---
## Words of warning
**Salad Slicer is currently undergoing rapid development and breaking changes. Until we achieve a 1.0 release, we are playing a little fast and loose with semantic versioning. Check your files carefully when you update to a new version.**

---
Salad Slicer is an open-source Rhinoceros Grasshopper plugin developed for slicing objects for 3D concrete printing, however, the plugin is also suitable for slicing objects to print other materials such as ceramics or polymers. Among other functionalities, Salad Slicer offers: 

- Slicing planar 2.5D objects from a 2D curve
- Slicing planar 3D objects from a 3D mesh
- Possibility to define your own slicers and/or reconstruct and customize the pre-defined slicers with the components from the Geometry category. 
- Possibility to construct an NC program (G-code) for the sliced geometry. 
- Possibility to convert all sliced objects to planes that can be used to construct programs for, for example, ABB and KUKA robots. 

## Installation
You can install Salad Slicer directly via Rhino’s [Package Manager](https://www.rhino3d.com/features/package-manager/) (v7 and higher). Alternatively, download the latest release from this repository’s [releases page](https://github.com/3DCP-TUe/SaladSlicer/releases). Simply place the `SaladSlicer.gha` file in your Grasshopper components folder (*GH: File > Special Folders > Components Folder*). Ensure the file is unblocked (*right-click > Properties > General tab > Unblock*). Restart Rhino, and you're ready to go!  

A collection of example files showcasing Salad Slicer’s main features is available in the [Example Files](https://github.com/3DCP-TUe/SaladSlicer/tree/master/ExampleFiles) folder in this repository.

## Contribute

**Bug reports**: Please report bugs at our [issue page](https://github.com/3DCP-TUe/SaladSlicer/issues). 

**Feature requests**: Feature request can be proposed on our [issue page](https://github.com/3DCP-TUe/SaladSlicer/issues). Please include how this feature should work by explaining it in detail and if possible by adding relevant documentation.

**Code contributions**: We accept code contributions through [pull requests](https://help.github.com/en/github/collaborating-with-issues-and-pull-requests/about-pull-requests). For this you have to [fork](https://help.github.com/en/github/getting-started-with-github/fork-a-repo) or [clone](https://help.github.com/en/github/creating-cloning-and-archiving-repositories/cloning-a-repository) this repository. We only accept code contributions if they are commented. You can read more about this topic [here](https://docs.microsoft.com/en-us/dotnet/csharp/codedoc). If you want to make a significant contribution, please let us know what you want to add or change to avoid doing things twice. For questions or if you want to discuss your contribution you can reach out to one of the [developers](https://github.com/3DCP-TUe/SaladSlicer/graphs/contributors). 

## Credits
Salad Slicer is an open-source project that is developed and initiated by the [3D Concrete Printing Research Group at Eindhoven University of Technology](https://www.tue.nl/en/research/research-groups/structural-engineering-and-design/3d-concrete-printing/). The technical development is executed by the PhD and PDEng candidates who are listed [here](https://github.com/3DCP-TUe/SaladSlicer/graphs/contributors).

We would like to acknowledge the authors of [Robot Components](https://github.com/RobotComponents/RobotComponents) for making their Grasshopper plugin available and open source. The code structure of Salad Slicer is heavily influenced by [Robot Components](https://github.com/RobotComponents/RobotComponents). As an acknowledgment, we have provided [Example Files](https://github.com/3DCP-TUe/SaladSlicer/tree/master/ExampleFiles) wherein we demonstrate how to use Salad Slicer in combination with [Robot Components](https://github.com/RobotComponents/RobotComponents). 

## Cite
Salad Slicer is a free-to-use Grasshopper plugin, and we kindly ask you to cite it if used. By citing the software, you recognize the work that went into its development and allow us to track its usage, making it easier to secure funding for further improvements. More importantly, citing the software and providing details on the tools used ensures that results can be reproduced. See [How to cite and describe software](https://software.ac.uk/how-cite-software) for more details and an in-depth discussion.

To cite all versions of Salad Slicer - as a reference to the whole project -  in publications, use:

```
Arjen Deetman, Derk Bos, Matthew Ferguson, & Jelle Versteege (2023). Salad Slicer: An open-source slicer toolkit for 3D concrete printing. Zenodo. https://doi.org/10.5281/zenodo.7818240
```

On our [Zenodo page](https://doi.org/10.5281/zenodo.7818240) you can find how to cite specific versions.

## Version numbering
Salad Slicer uses the following [Semantic Versioning](https://semver.org/) scheme: 

```
0.x.x ---> MAJOR version: incompatible changes. 
x.0.x ---> MINOR version: functionality added in a backwards compatible manner.  
x.x.0 ---> PATCH version: backwards compatible bug fixes.
```

## Funding

This software could be developed and maintained with the financial support of the following projects:
- The project _"Parametric mortar design and control of system parameters"_ funded by [Saint-Gobain Weber Beamix](https://www.nl.weber/).
- The project _"Additive manufacturing of functional construction materials on-demand"_ (with project number 17895) of the research program _"Materialen NL: Challenges 2018"_ which is financed by the [Dutch Research Council](https://www.nwo.nl/en) (NWO).

## Open science statement

We are committed to the principles of open science to ensure that our work can be reproduced and built upon by others, by sharing detailed methodologies, data, and results generated with the unique equipment that is available in our lab. To spread Open Science, we encourage others to do the same to create an (even more) open and collaborative scientific community. 
Since it took a lot of time and effort to make our data and software available, we license our software under the General Public License version 3 or later (free to use, with attribution, share with source code) and our data and documentation under CC BY-SA (free to use, with attribution, share-alike), which requires you to apply the same licenses if you use our resources and share its derivatives with others.
  
## License
Copyright (c) 2021-2025 [3D Concrete Printing Research Group at Eindhoven University of Technology](https://www.tue.nl/en/research/research-groups/structural-engineering-and-design/3d-concrete-printing)

Salad Slicer is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 3.0 as published by the Free Software Foundation. 

Salad Slicer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Salad Slicer; If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.html>

Contact us if you want to obtain a copy of the source code with a different license. 
