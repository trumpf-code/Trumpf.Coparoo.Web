# Trumpf.Coparoo.Web Library for .NET 
![logo640]

## Description
*Trumpf.Coparoo.Web is a .NET library for C# that helps you write fast, maintainable, robust and fluent Selenium-driven web tests based on the **co**ntrol/**pa**ge/**ro**ot-**o**bject (Coparoo) pattern.*

The following sign-in/out test scenario illustrates how the framework facilitates writing user interface tests in "natural" way:
    
    var app = new GitHubWebDriver();                    // create the test driver
    app.Open();                                         // open the github page in a new browser tab
    app.On<Header>().SignIn.Click();                    // click the sign-in button
    app.On<SignInForm>().SignIn("myUser", "abc");       // enter the user credentials ...
    app.On<Header>().Profile.Click();                   // open the user profile
    app.On<ProfileDrowndown>().SignOut.Click();         // sign out

## NuGet Package Information
To make it easier for you to develop with the *Trumpf Coparoo Web* library we release it as NuGet package. The latest library is available on [https://www.nuget.org/packages/Trumpf.Coparoo.Web](https://www.nuget.org/packages/Trumpf.Coparoo.Web).
To install, just type `Install-Package Trumpf.Coparoo.Web` in the [Package Manager Console](https://docs.nuget.org/docs/start-here/using-the-package-manager-console).

## Getting Started
If you want to learn more about the *control/page/root-object pattern*, the idea behind this framework, consider reading [the design pattern introduction](PATTERN.md).
It illustrates how the framework can help you at writing maintainable and fast-running user interface tests.

If you can't wait getting started and want see some code, have a look at [this code example](DEMO.md).

Supposed you have already defined your first Coparoo test and want to know how to better structure page objects and related tests, have a look at [Coparoo's page test runner](PAGETESTS.md).

Finally, if things are set up and you want to work on user interface tests in a collaborative setup consisting of many possibly independent teams, or write test cases even before the user interfaces ready to execute (say, directly after the UX team is done) consider reading [this tutorial](DECOUPLING.md).

## Contributors
Main development by Alexander Kaiser (alexander.kai...@de.trumpf.com or alexander.kai...@cs.ox.ac.uk).

Ideas and contributions by many more including
- Daniel Knorreck, Gerald Waldherr / *Additive Manufacturing, TRUMPF Laser- und Systemtechnik GmbH, Ditzingen*
- Jochen Lange, Matthias Wetzel, Markus Ament, Bernd Gschwind, Bernd Theissler, Andreas Alavi, Sebastian Mayer, Daniel Boeck / *TRUMPF Werkzeugmaschinen GmbH + Co. KG, Ditzingen*
- Igor Mikhalev / *Trumpf Laser Marking Systems AG, Schweiz*
- Thanikaivel Natarajan / *India Metamation Software P. Ltd., India*
- Nol Zefaj, Nils Engelbach, Phi Dang, Mattanja Kern, Felix Eisele / *AXOOM GmbH, Karlsruhe*
- Manuel Pfemeter / *AIT – Applied Information Technologies GmbH & Co. KG, Stuttgart*
- Marie Jeutter / *Hochschule Karlsruhe*

## License
Copyright (c) TRUMPF Werkzeugmaschinen GmbH + Co. KG. All rights reserved. 2016, 2017, 2018.

Licensed under the [Apache License Version 2.0](LICENSE) License.

Coparoo uses the [Stashbox](https://github.com/z4kn4fein/stashbox) dependency injection framework (MIT license) by Peter Csajtai.

[logo640]: ./Resources/logo640.png "coparoo web logo"
