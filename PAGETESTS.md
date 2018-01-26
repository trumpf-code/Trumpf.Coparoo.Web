# Structuring page objects and related tests
User interface tests are often linked with pages, dialogs and so on, e.g., a login page with login tests, a file import dialog with import tests.
Since in the page object pattern, *pages* are tightly coupled with *Page Objects*, this structure can be used for structuring tests as well.

## How to link test cases and page objects?
 After adding `using Trumpf.PageObjects.PageTests` to the preamble of a C# file, the generic `PageObjectTests<P>` base class can be used to associate a *sequence* of test steps with a page object `P`.

As an example, for a Page Object `Menu` a test class `MenuTests` can be defined as follows:

    public class MenuTests : PageObjectTests<Menu> { ... }

In this class, we can define the *sequence* of test steps using the `[PageTest]` attribute, which can attached to any parameter-less public `void` function:

    public class MenuTests : PageObjectTests<Menu> {
        [PageTest] public void IterateOverAllMenuItems() { ... }
        [PageTest] public void OpenAndCloseTheMenu() { ... }
        ...

The framework ensures that in each of the methods, property `Page` can be used as a substitute for the page object under test, `Menu` in the example.
A property `Events { get; }` of `Menu` can, e.g., accessed in the test method though `Page.Events`.

## How to test a page?
Page tests associated with a given page object `P` can be *executed* by calling `On<P>().Test()`, e.g., for the previous example in a Visual Studio test like this:

    [TestMethod] public void MenuTests() {
        var tab = new MyTab();
        tab.On<Menu>().Test();
        tab.Close(); }

The `Test` method will search all assemblies in the AppDomain for Page Test classes of the respective page object (here `Menu`) and, for each of them, execute every `PageTest`-attributed method from top to bottom.
If a Page Object has multiple Page Test classes associated with it, their execution order is undefined.

Before executing the first test in a test class is, the `Test` method
checks if the virtual property `ReadyToRun` returns `true`, and if this is the case
calls the virtual `BeforeFirstTest` method of the page test class.
By default `ReadyToRun` maps to `Page.Displayed`, and `BeforeFirstTest` to

    if (!ReadyToRun) {
        Page.Goto();
        Page.Displayed.TryWaitFor(); }
Hence, if sufficient implicit navigation actions are specified in the `Goto` method of `P` then no explicit navigation steps are required for preparation.

## How to visualize page tests?
To visualize page test results of page object `P` together with the Coparoo graph, call 

    tab.On<P>().Test(true);

This will generate a `Coparoo.pdf` or `Coparoo.dot` file in working directory; check [the Coparoo code example](DEMO) for details.
The generated graph contains discovered page test classes as well as their page tests.

The resulting graph looks like this:
![testTree]

The graph helps answering questions like
- Are all pages sufficiently tests?
- Which pages are no tested at all?
- Where is the bug?

## How to test all pages?
The entire set of page tests, i.e. of all page objects in the Coparoo graph, can be executed by calling the `TestBottomUp` method in this manner:

    [TestMethod]
    public void TestOfTests() {
        var tab = new MyTab();
        tab.TestBottomUp(true);
        tab.Close(); }

Page tests of page object are then executed from "bottom to top", i.e. starting from the leaves. 
The result will look like this:
![bottomUpTree]

[testTree]: ./Resources/testTree.PNG "Coparoo graph with test classes and results"
[bottomUpTree]: ./Resources/bottomUpTree.PNG "Coparoo graph with test classes and results"