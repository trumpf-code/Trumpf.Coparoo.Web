# Tutorial

## A minimal code example
The goal of this contrived code example is to demonstrate how root, control and page object classes can be created and used, rather than to write a meaningful test case.

### Goal of the example
The following general steps shall be automated for some web page:
1. Open a page in a new Firefox browser tab.
1. Click a link in a menu bar.
1. Close the browser.

### Setting up a project
Before getting started, we need to create a C# project, e.g. of type "unit testing". 
Then we add a NuGet package reference to [Trumpf.Coparoo.Web](https://www.nuget.org/packages/Trumpf.Coparoo.Web/), which will also add a reference to the [Selenium.WebDriver](https://www.nuget.org/packages/Selenium.WebDriver/) NuGet package.

The base classes and extension methods of the *Coparoo* and *WebDriver* libraries are available after adding `using Trumpf.Coparoo.Web` and `using OpenQA.Selenium` to the preamble of your C# sources.

### How to define the root object?
In the first step, we create the root (or "tab") object `MyTab` wrapping the web page under test by deriving from `Trumpf.Coparoo.Web.TabObject`:

    public class MyTab : TabObject {
        protected override Func<IWebDriver> Creator => () => new Firefox.FirefoxDriver();   // set Firefox as the automation driver
        protected override string Url => "https://myWebPageUnderTest.com"; }                // set the address under test

The `Creator` and `Url` properties define the *Selenium driver* that will drive the test, here the *Firefox* browser, and the address to open when the inherited `Open()` method is called.

Equipped with this class, we are ready to automate the first and last step of the above goal:

    var tab = new MyTab();  // create the root page object
    tab.Open();             // open the web page in new tab
    tab.Exists.WaitFor();   // wait until the tab exists
    tab.Close();            // close the tab and browser

### How to create a control object for links?
In order to click a standard web link, we create a control object `Link` by deriving from `Trumpf.Coparoo.Web.ControlObject`:

    public class Link : ControlObject {
        protected override By SearchPattern => By.TagName("a"); // matching criteria for DOM nodes
        public string Text => Node.Text;                        // get the displayed link text
        public string URL => Node.GetAttribute("href"); }       // get the destination address

Property `SearchPattern` specifies how to locate the control object node in the DOM tree.
It is defined using Selenium's `OpenQA.Selenium.By` class which exposes methods for the most prominent search criteria like the `TagName` used here, `XPath`, `Id` and `LinkText`; check the [Selenium API `By` documentation](https://seleniumhq.github.io/selenium/docs/api/dotnet/html/T_OpenQA_Selenium_By.htm) for the full list.
Further methods to combine multiple search criteria, e.g. [`ByAll(by1, by2, ...)`](https://seleniumhq.github.io/selenium/docs/api/dotnet/html/T_OpenQA_Selenium_Support_PageObjects_ByAll.htm), are available through the `WebDriver.Support` assembly.

In the preceding example the search pattern `By.TagName("a")` specifies that the control object matches with `a`-tagged DOM nodes only, i.e. any `<a>...</a>`-tagged HTML element.

Properties `Text` and `URL` return the link's display text and the destination, respectively.
Both values are retrieved through the inherited `Node` property, which implements Selenium's `IWebElement` interface used to control DOM elements; check the [Selenium `IWebElement` API documentation](https://seleniumhq.github.io/selenium/docs/api/dotnet/html/T_OpenQA_Selenium_IWebElement.htm) for the list of members.

### How to find control objects?
In order to find a matching element in the DOM tree, tab, control and page objects expose `Find` method.
Equipped with an opened `tab` object, `tab.Find<Link>()` will, e.g., return a `Link` instance of *some* link in the DOM.
In order to find a *specific* one, search criteria can be *strengthened*: As an example, `tab.Find<Link>(By.LinkText("foo"))` will match DOM nodes satisfying *both criteria*.
This criteria chaining is achieved through the `ByAll` class mentioned above.

In addition to `Find`, `FindAll` return an enumeration of matching instances. The following code, e.g., prints details of all links in the DOM tree:

    foreach(var link in tab.FindAll<Link>()){
        Console.Writeline($"
            Text: {link.Text}, 
            URL: {link.URL}, 
            Displayed: {link.Displayed}, 
            Location: {link.Location.X}/{link.Location.Y}
        "); }

For a given control object instance returned by `Find` or `FindAll` the property `Node` is *bound lazy*, i.e., its first retrieval will *tie it to a specific DOM node*.
That is, caching happens for *specific instances only*, not across instances returned by `Find` or `FindAll`.
As an example, invoking `Find<Link>(By.LinkText("foo")).Displayed` twice will trigger two independent DOM searches.

### How to define a menu page object?
Writing tests though sole interactions with controls searched from a tab object, as demonstrated in the previous paragraph, is possible due to the powerful `XPath` search criteria, but will yield unmaintainable code and is therefore impractical.
To remedy that situation it is good practice to decompose the DOM into page objects.

In order to define a page object `Menu` for a web page's menu bar, say, we derive from `Trumpf.Coparoo.Web.PageObject` like this:

    public class Menu : PageObject, IChildOf<MyTab> {
        protected override By SearchPattern => By.ClassName("menu-header");  // some unique search criteria for the menu
        public Link Events => Find<Link>(By.LinkText("Events")); }          // some menu link

Note the interface `IChildOf<MyTab>` in the first line.
It states that the page object is a *direct child* of the root object `MyTab` in the *Coparoo graph*, and in turn `MyTab` is the *parent* of `Menu`.
By doing so, we stipulate that `Menu`'s node in the DOM is a descendant, i.e., direct or indirect child at any depth, of `MyTab`'s DOM node.

Analogous to control objects, property `SearchPattern` specifies how `Menu`'s DOM node is located from the *parent*'s DOM node, namely by searching for a node with CSS class name `menu-header`.
Finally, property `Events` returns a specific menu link from the DOM subtree rooted in `Menu`'s DOM node.
Due to criteria chaining, the returned instance will be `a`-tagged, with display text "Events".

### How to access tab and page objects from a test?
Page (and tab) objects can be accessed though the `On` method exposed by tab, control and page objects.
Equipped with an opened `MyTab` object `myTab` the following code will yield a ready-for-use `Menu` instance.
    
    Menu menu = myTab.On<Menu>();

Due to their uniqueness-property, and in contrast to control objects, page object's DOM nodes are cached "aggressively", i.e. across `On`-calls.
As an example, invoking `On<Menu>().Events.Displayed` *twice* triggers
- *one* search for `Menu`'s DOM node, and
- *two* searches for `Events.Displayed`; both starting for the cached `Menu`'s DOM node, though.

If the node of a page object in the cache is *invalid* (not displayed) on its retrieval, it is discarded and a fresh search for the page object's DOM node is triggered.

### Putting things together in a minimal "test"...
Now that we have defined a root, control and page object, we can automate the three test step set out at the very top:

    MyTab myTab = new MyTab();              // create the root page object
    myTab.Open();                           // open a new tab browser with the address 
    myTab.On<Menu>().Displayed.WaitFor();   // wait until the menu is displayed
    myTab.On<Menu>().Events.Click();        // on the menu click the events link
    myTab.Close();                          // kill the browser
 
For waiting, page and controls objects expose a set of properties, e.g. `Displayed`, `Enabled` and `Exists`.
Their values can be retrieved via `Exists.Value` or `bool b = Exists` etc., or alternatively for waiting: a call to `Exists.WaitFor()` or `Exists.WaitForFalse()` will, e.g., block execution until the condition turns true or false, respectively.
If the expectation is not fulfilled within the maximum waiting time, a timeout exception is thrown.
The waiting time can be customized in the tab object as follows:

    myTab.Configuration.WaitTimeout = TimeSpan.FromMinutes(1);

The non-throwing counterparts are `TryWaitFor` and `TryWaitForFalse`.

## Visualizing the *Coparoo graph*
As the number of page objects grows, question like these arise:
- Where should a new page object be added to the graph?
- Which page objects already exist?
- How can the search performance be improved?

The *Coparoo graph* helps answering these questions.
It is constructed as follows:
- The root node is the `TabObject`.
- Each page objects is represented as a node, connected according to `IChildOf<>`.
- Each `public` control object getter of a page object is represented as a node, connected to their parent node.

The graph should at all times be kept *free of cycles*.

By calling the `WriteGraph` method of the tab object the graph can be generated automatically:

    new MyTab().WriteGraph();

For a slight extension of the previous example the graph looks like this: 

![tree]

In order to render the graph as PDF, as was done for the previous picture, the [Graphviz](http://www.graphviz.org/) tool needs to be installed.
Graphviz is an open-source toolbox initiated by AT&T Labs Research for drawing graphs. Visit the [wiki page](https://en.wikipedia.org/wiki/Graphviz) for details.
If the tool is not available, the graph is stored in the [DOT](https://en.wikipedia.org/wiki/DOT_(graph_description_language)) format.
The default file names are `Coparoo.pdf` and `Coparoo.dot`, respectively, but are customizable.

### How to go to a page in a maintainable way?
In order to "*go to*" a page and do some actions, tests often contain *explicit navigation operations*, e.g., they explicitly click on a button to open the menu, click on a menu item, then select the second entry from the menu drop-down.
For tests that do not focus on the navigation itself this approach has crucial drawbacks:
- It *impairs readability* as such actions *distract* from the actual purpose of a test (what is tested?).
- It comes with a high risk for creating code duplicates, as such actions are seldom exclusive for a test.
- It *increases fragility* as they make tests more likely to break in the presence of *navigation redesigns*, i.e. changes to action sequences related to navigation.

It is therefore good practice to move them *out of the tests* and *into the page object*.
To this end, *page objects* come with a virtual `Goto`-method that can be enriched with such actions: the `Goto` method of Page Object `P` is thus the place to describe how to navigate to `P`.

We can, e.g., simplify the following somewhat longish *login/logout-test*

    var app = new GitHubWebDriver();                    // create the test driver
    app.Open();                                         // open the github page in new browser tab
    app.On<Header>().SignIn.Click();                    // click the sign-in button
    app.On<SignInForm>().SignIn("myUser", "abc");       // enter the user credentials ...
    app.On<Header>().Profile.Click();                   // open the user profile
    app.On<ProfileDrowndown>().SignOut.Click();         // sign out

to

    var app = new GitHubWebDriver();                    // create the test driver
    app.Goto<SignInForm>().SignIn("myUser", "abc");     // enter the user credentials ...
    app.SignOut();                                      // sign out

The navigation action of the sign-in form needs, e.g., be defined in `SignInForm.Goto()`'s as follows:

    public override void Goto() {
        if(!Displayed) {                        // no actions if the page is already displayed
            app.Goto<Header>().SignIn.Click();  // click the sign in button
            Displayed.WaitFor(); } }            // wait for the SignInForm to be displayed

The `Goto` method of the `ProfileDrowndown` and `Header` page object is similar.
Notably, the `Header.Goto` method needs to ensure that browser tab is opened once, which can be achieved by calling `Parent.Goto()` assuming `Parent` is the root object.
    
Observe that the codes line count has halved from 6 to 3, and how readability and robustness have increased. A dream.

[tree]: ./Resources/tree480.png "page object tree"