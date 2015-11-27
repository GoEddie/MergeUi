
#THIS HAS NOW BEEN MOVED INTO THE SSDT-DevPack (https://the.agilesql.club/Projects/SSDT-Dev-Pack) Still and always will be OSS :)

# MergeUi

Sql Server Data Tools (SSDT) is a way to write, refactor and deploy Sql Server database code. When you have a Sql Server database you sometimes need to deploy data such as lookup or reference data and doing that can be a pain. This is a package for Visual Studio that gives a UI to automatically create merge statements for any table in your project and also lets you import the data from an existing table into the project.

### TLDR;

- Manage static data in SSDT with T-Sql merge statements
- Use this to create, edit and remove static data from merge statements

### Version
0.1.9

### Installation

Download the VSIX installer, it currently works with Visual Studio 2013 and Visual Studio 2015, Visual Studio 2012 has a different VSIX format so I haven't built that but you can clone the code and build it in Visual Studio 2012 with the Visual Studio 2012 sdk which should then deploy.

* [Download VSIX here](https://the.agilesql.club/assets/downloads/AgileSqlClub.MergeUiPackage.vsix)

Once it is downloaded on your machine with Visual Studio, just double click it, the Visual Studio tools wizard should then ask you if you want to install it. When it has finsished restart Visual Studio, open an SSDT project and go to View-->Other Windows-->MergeUi (It is the lovely looking large M icon).

---
### License
MIT

I don't really care what license it has so if you want to use anything here but don't like MIT just send a pull request with a new license and I will add it in.


### Further Info

I have a couple of blog posts, the first one was for an early version:

https://the.agilesql.club/Blog/Ed-Elliott/Introducing-MergeUi-Create-And-Edit-Merge-Statements

This one is more recent and includes the updated ui:

https://the.agilesql.club/Blogs/Ed-Elliott/MergeUi-0-1-9-4-Released

