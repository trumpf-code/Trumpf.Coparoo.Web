# Writing tests in cooperative projects or when the user interfaces is designed, yet not implemented
We can split the definition of Process, Page and Control Objects into *assemblies* with interfaces and implementations:
- *Interfaces* define which controls and operations a page provides, e.g. page `LoginWindow` may have button `Login` and method `LoginUser(username, password)`.
- *Implementations*, in contrast, define how these controls are located (e.g. in the DOM tree) and how operations are broken down into actions sequences on the page controls or other operations, e.g. Button `Login` is located by searching for a control with tag `a` (a link) and text "Sign in", and a user is logged in via method `LoginUser` by entering the user name and password into input fields `Username` and `Password`, and finally clicking button `Login`.

If both layers are separated, the test cases can be defined in another assembly so that they solely *reference the interface layer*, while their implementations are loaded and resolved *dynamically*, i.e. when the test executes. 

![decoupling]

Tests cases and page objects implementations are thus *decoupled*.

## What's the benefit of decoupling tests case and page objects?
Decoupling tests case and page objects has several benefits.

*Better test maintanance and reusability:* If the interface remains unchanged, the same test fits for different user interface versions (*no* code or reference change). As an example, if a test invokes interface method `LoginUser(user, password)` on page object interface `ILoginWindow`, it does not care about the concrete actions that need to be taken to log on the given user: Adding a check box "Accept the terms and conditions", e.g., needs to be adapted in the implementation, yet the page interface remains fully unchanged, and so does the test code and its references.

*Tests cases can be coded when the product is still under development:* Tests can be coded directly after, e.g., the UX has finished the design and the user stories (and associated interfaces), and hence *before* the pages' functionality has been implemented.

*Systems consisting of many components or developed concurrently by many teams can be tested easier:* When several components with user interfaces are developed concurrently and then integrated, and we want to code test cases for that integration: User interface tests for the integrated system can be coded once every component provides its set of page interfaces. A test-driven approach can be realized up to the system-level.

## How to decouple test from page object code?
The tests and page objects can be decoupled via interfaces as follows:
1. Create an *interface assembly*
   - Create a new assembly for page object interfaces (the "*interface assembly*").
   - Add the interfaces for the page objects that shall be exposed, e.g., `IMyTab`, `ILink`, `IMenu` for classes `MyTab`, `Link`, and `Menu`, respectively.
1. Create an *implementation assembly*
   - Add the interface assembly to the list of referenced projects in the *page object assembly*.
   - Move all tab, control and page objects into that assembly.
   - Extend the page objects by the respective interface, e.g., `class Menu : PageObject, IChildOf<MyTab>` turns into `class Menu : PageObject, IChildOf<MyTab>, IMenu` etc.
   - Replace all types these classes return by their corresponding interface, e.g., the property `Link Events => Find<Link>...` in page object `Menu` turns into `ILink Events => Find<ILink>`. The `ILink` will be resolved at runtime.
1. Adapt the *test assembly*
   - Ensure that all tests are defined in an assembly other than the interface or implementation assembly.
   - Make sure this assembly has a reference to the interface assembly, *not* however a direct or indirect reference to the page object assembly. 
   - Replace all usages of page and control object implementations by their corresponding interface, i.e. `tab.On<Menu>()` turns into `tab.On<IMenu>()` etc.
   - Locate the tab object via the `TabObject.Resolve` method, e.g. `var tab = new MyTab()` turns into `var tab = TabObject.Resolve<IMyTab>()`.
   - Finally, ensure the page object assembly is available and loaded, e.g., via `Assembly.LoadFrom`, before tests get executed.

[decoupling]: ./Resources/decoupling.png "coparoo web logo"