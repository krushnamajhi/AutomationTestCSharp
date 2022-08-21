# AutomationTestCSharp

Steps To Use
1. Clone the project
2. Open .sln file

How to Start Automation?

Ans. This is only an Automation Library. Hence, don't write any automation script inside this project.
Add a new Nunit Project to your solution and add this project as dependency.
Once you add a new project, add this project as project reference of your new preject.
Create a class file with name "SetUps.cs", add an annotation to the class "[SetUpFixture]" and extend Class "TestSetUps" to it.

[SetUpFixture]
public class Setups : TestSetups
{
}

While creating a test class, extend "TestBase" every time.


