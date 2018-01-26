# The Control/Page/Root-Pattern
The abbreviation *Coparoo* is short-hand for **Co**ntrol-, **Pa**ge- and **Ro**ot-**O**bject, the design pattern shaping this framework.

## What makes a good user interface test?
As with many other test types, user interface tests should ideally be
- quickly *adaptable* to user interface and work flow changes
- *readable* ("what is being tested?")
- able to incorporate multiple *perspectives* (testers, users, product managers, etc.)
- *compact*, *reliable* and *fast* (at least as fast as its manually executed counterpart)
- *quickly* to implement (ratio between the development time of a feature and its tests)
- independent from the context, e.g., screen resolution, local setting, language etc.

The Page Object Pattern is a common approach to address these needs.

## What are Page Objects and what's their purpose?
Page Objects are *classes* that wrap pages of a graphical user interface.
Tests interact with these classes whenever they want to interact with pages.

The main object for using Page Objects is to
- wrap what an operator can see and do on a page: text fields become *strings*, check boxes *Booleans*, and so on,
- separate technical details for accessing the UI elements (implemented as `private`'s in the page object) from the test code, and
- provide action sequences that require multiple page interactions and occur often in tests.

If `Login` is the page object wrapping a web site's *login dialog*, then a test may
- read the contents of a text box *Username* via `string username = Login.Username`, 
- set a check box *Accept* via `Login.Accept = true`, or
- call `Login.LoginUser(name, password)`, which types the user name and password in the corresponding fields, and then confirms the dialog via a "sign in" button.

In 2009, Simon Stewart introduced the pattern in a Selenium PageObject [wiki entry](https://github.com/SeleniumHQ/selenium/wiki/PageObjects) as a means to write good user interface tests. 
Today, it is extensively used in practice, and applied on a wide range of UI technologies, including desktop applications and mobile apps. 
[*Google Trends*](https://trends.google.com/trends/explore?date=all&q=page%20object%20pattern) illustrates the stable interest in the approach since its emergence around 2009.

## What are Control Objects and why do we need them?
When it comes to maintainability, the Page Object Pattern has certain limitations.
This is due to the fact that page objects expose *properties of their controls*, yet these controls tend to appear frequently. 
As an example, a Page Object with a text box *Username* may have methods to
- write and read the text: `string UsernameText { get; set; }`
- check its visibility: `bool IsUsernameVisible { get; }`
- click it: `void ClickUsername()`.

For other text boxes on that page, e.g. for entering the *password*, or different pages these properties and methods would have to be repeated again and again.
In practice, it is therefore desirable to treat *pages* and *controls* separately: 
- Pages are wrapped by "Page Objects", 
- Controls by "Control Objects".

The `username` and `password` text boxes used in the previous example, could, e.g., be exposed though a `TextBox`-Control Object class with properties `string Text { get; set; }` and `bool IsVisible { get; }`, and method `void Click()`. Hence, the two text boxes turn into *two instantiations* of exactly that Control Object class.

The distinction has some more benefits.
Compared to Page Objects, Control Objects are usually not unique: "the login dialog" or "the cancel button of the login dialog" is of interest for a test, not so, however, "the button" (which one? on which page?) or even "the cancel button" (of which page?) on their own.

This "uniqueness-property" is why Page Objects are often "*navigatable*" (certain actions will, e.g., open the login dialog), while controls are not.
On the other hand, clicking a page object in general makes no sense, while it does do for controls.
Summed up, using Controls Objects in your code can dramatically improve readability and shrink code size.

## Finally, what are Root Objects?
Root Objects are *classes* that wrap the root node of the DOM, hence essentially a *browser tab* that display the web page under test. 
In contrast to Page and Controls Objects they have no parent in the DOM, nor can they be navigated to, or clicked.
Instead, a Root Object has things like 
- a driver from which every search for a page or control objects initially starts, 
- a class to configure settings fitting all page and control objects, and
- an address that can be opened in a browser via calls to the `Open()` and `Close()` methods.

## Putting things together in the *Coparoo Graph*
Following the Coparoo pattern essentially boils down to structuring the control, page and root objects in a graph, which we call the *Coparoo Graph*.
This graph is a *human-readable abstraction* of the DOM tree and serves as an interface for coding test cases.

The following image sketches the idea:

![tree]

At the top is the root object, e.g. the browser tab on a smart phone, with several page object underneath, e.g. one for info message boxes.
Each page object has one or more control objects, e.g. the info message box page objects has two button control objects (dashed lines) and so forth.

## How can we benefit from this approach?
Recall that web browser render pages into a [DOM tree](https://en.wikipedia.org/wiki/Document_Object_Model) wherein each node is an object, e.g. a control, page or layout element.
Automating a web page with Selenium-like tools thus boils down to 
1. *finding* nodes of pages and controls the test interacts with, and 
1. *invoking* certain operations on these nodes, like clicking it or retrieving its text.

As an example, `WebElement user = driver.findElement(By.id("User"));` will start from the browser tab node `driver` and search for the first occurrence of a node with `id` set to `User`. 

### Why not searching in the most obvious way?
One way to search for nodes is to specify the entire search path from the browser tab node and just start searching from that node over and over again.
The downside of this approach:
- Poor maintainability (we get redundant and large search criteria, and as a result small redesigns can have a big impact on tests).
- High risk of non-unique results (how to distinguish different regions in the DOM tree?).
- Slow search operations as localizations do not use results from previous searches, and hence a bad test performance.

For testing non-trivial web pages it is therefore better to avoid this approach.

### Why does the DOM structure enable us to write better maintainable tests?
Looking at how web pages are defined and how their DOM trees are structured, we observe that if we have a page `A`, say, which resides *graphically inside* another page `B`, say, then the node of page `A` is usually a direct or indirect child of the node of the surrounding page `B`.
In the same way, nodes of controls of a page `A` tend to reside below `A`'s.
The following image sketches the idea:
![domProperty]

In practice, this property is helpful as it is the reason why we can essentially always come up with "simple", hence human-readable *Coparoo Graphs*,  like the one sketched above.

By avoiding redundant search patterns we obtain significantly better maintainable code, e.g. when we need to adapt to redesigns.
Exploiting this simplicity enables us to write *maintainable* user interface test.

### And why does it help us to speed-up tests?
Besides ensuring maintainability, taking advantage of the DOM structure can significantly *speed-up test runs*.
This can be achieved by exploiting the following simple idea: If page `A`'s DOM node `a` is already located, and page `B` is known to be under `a`, then we can start the search of `B`'s DOM node `b` in `a` rather than at the root.

The following example illustrates, why this can significantly speed-up searches in practice, and hence test runs: When we search DOM node `b` from the root `driver` using a breadth first search via `driver.findElement(By.id("b"))`, 11 nodes need to be traversed in total while `a.findElement(By.id("b"))` will traverse just one, namely `b`.

![fastSearch]

The same applies when we search the node of a control of that page, and moreover, it is the more effective the larger the DOM tree gets.
Since page objects are usually attached to unique node in the DOM, these are well suited for a user-transparent *caching*, i.e. the user gets the caching and speed-up for free, viz. as a side-effect of structuring the page objects in a tree.

Strictly speaking, the Page Object Tree does not really have to be a tree.
It can take the form of a cycle-free graph if the same Page Object appears in different places in the DOM tree. 
This is the case, for example, if a page can be "docked" in different places as can often be found on dashboards.

### So, how can we derive the Coparoo Graph from the DOM?
In order to derive the Coparoo Graph from the DOM ask the following questions:
- What are the pages the test code will interact with, these are good candidates for page objects.
- Which auxiliary page objects would help to significantly shorten search paths, or yield a tree that is better to read or maintain?

When you have identified these page objects, you can follow this simple rule to derive your page object relations of the Coparoo Graph: The Page Object of page `B` is a *direct child* of the Page Object of page `A` if
- the node `b` of `B` is a direct or indirect child of the node `a` of `A`, and ideally
- no node `c` of another page object `C` is located on the path from `a` to `b`.

### Getting started...
As a next step, consider reading [this code example](DEMO.md).
It illustrates how the *Coparoo* framework helps you putting these ideas into practice.

[tree]: ./Resources/pageObjectTree.png "Coparoo tree"
[fastSearch]: ./Resources/fastSearch.PNG "Finding faster"
[domProperty]: ./Resources/domProperty.PNG "The DOM property"