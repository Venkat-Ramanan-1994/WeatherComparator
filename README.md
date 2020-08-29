SUMMARY: A Hybrid BDD automation framework with Page Obeject Model(POM) and external test data from a JSON file,
that compares the temperature object from ndtv website and a restful weather API based on a variance logic(min and max temperature)
VERSION: 1.0

ENVIRONMENTAL SETUP: 
IDE          - Visual Studio Professional 2019 (Trial Version)
BDD Tools    - Specflow for Visual Studio 2019
		[This installs the below 3 packages & other dependencies required for building the solution
		 and running the tests ]
	BDD compiler - Specflow v2.4.1
	Build        - Specflow MsBuild v2.4.1
	Runner       - SpecRun v3.2.15

DEPENDENCIES: All the required dependencies are present in the packages.config file. Right Click on the solution and select Restore NuGet Packages to restore all dependencies.

STEPS FOR RUNNING TESTS IN VISUAL STUDIO: 
1. Clone the project in Visual Studio 2019
2. In the toolbar at the top, Go to Extensions --> Manage Extensions --> Search specflow --> Install SpecFlow for Visual Studio 2019
3. Restart the Visual Studio for the specflow changes to take effect
4. Right Click on the solution and select Restore NuGet Packages
5. In the toolbar at the top, Go to Test --> Configure Run Settings --> Select Solution Wide runsettings File --> WeatherComparator --> localtesting.runsettings
6. Right click on the solution and Build the Solution
7. Now the tests should be displayed in Test Explorer
	If Test Explorer is not displayed by default, Go to View --> Test Explorer from the toolbar at the top
8. Right click on the tests in Test Explorer, select Run to run the tests