#Infi Coding Dojo - Event Sourcing

This is the codebase used for our [Coding Dojo](http://code.joejag.com/2009/the-coding-dojo.html) session on [Event Sourcing](https://msdn.microsoft.com/en-us/library/jj591559.aspx) (scheduled for July 7th, 2015). The code consists a stripped down version of the [NerdDinner](http://www.nerddinner.com) application, with a basic Event Sourcing set-up already implemented.

The main functionality of the (stripped down) NerdDinner application is the ability to host and find dinners, as well as RSVP to specific dinners. Initially, the flow for a user to RSVP to a dinner has been implemented using Event Sourcing techniques. Furthermore, a set of unit tests has been created to cover the basic use cases for the RSVP funciontality, as well as additional tests that are expected to cover new functionality added during the Coding Dojo.

### Requirements

In order to run this application you need Visual Studio (tested on version 2012) as well as [SQL Server](http://www.hanselman.com/blog/DownloadSQLServerExpress.aspx) (tested on version 2012). Simply build the solution and run the tests. You can also run the application and see if registering a user and hosting a dinner works properly. If any issues occur here, please let us know.

### Initial set-up ###

In our implementation based on Event Sourcing, specific actions trigger events that are stored in a general *Event* table in the database. This table contains the general properties of the events, as well as serialized data (JSON) for that specific event type. In the initial implementation only the *RSVPed* event exists, which contains the name of the user that has registered for the specific dinner identified by the event's *AggregateId* field. The dinner itself is identified by the *AggregateId* property of the event. This allows the system to easily retrieve all the events for a specific dinner (the aggregate in this case). When loading the dinner from the database, we can re-apply ('hydrate') all the known events for that dinner in order to obtain its current state. 

### Assignments

During the Coding Dojo the participants are expected to expand the application by implementing new features of modifying existing features. Make sure you keep runing the tests (of which most will fail initially) to see if you are going in the right direction, and also be sure to write your own tests if needed. 

Listed below (in suggested order) are the requested changes to the application:

**1 - Cancel an RSVP**

Currently, only registering an RSVP is implemented. However, the UI also supports canceling an RSVP, so this also has to be implemented in the backend. The most obvious place to start would be in *RSVPController.Cancel()*. Take note of the implementation for registering an RSVP, as canceling will likely be very simliar
    
**2 - Show a history of events (activity feed) on the dinner detail page**

Now that we have some basic events for a dinner, we would like to have an overview on the dinner detail page to show what has happened (and when) concerning that dinner. A UI implementation has already been created, which reads from the *Dinner.History* property. Ensure this property is filled with the right data and it will show up on the dinner detail page.

**3 - Changing a dinner's address (using Event Sourcing)**

Editing a dinner is already implemented using a basic CRUD and DB mapping. The assignment here is to extract a single property of a dinner (in this case the Address) and store changes to this property using Event Sourcing. We have already created a UI implementation in */Views/Dinners/Detail.cshtml*, simply remove '&& false' from the if statement. When clicking the address (as a dinner's host) an edit form will appear. The form will submit the data to *DinnerController.ChangeAddress()*, which now has to be implemented.

**4 - Specialized queries**

If there is enough time, you can attempt to optimize the hydrating of events when performing specific queries. A good example of this can be found in *SearchController.GetMostPopularDinners()*, which has to retrieve all dinners and all events from the database before hydrating the events to the corresponding dinners (in-memory), before actually filtering and selecting the dinners that are most popular. Obviously, this can be improved dramatically if the query could be done in the database instead of in-memory. 

A possible solution to this would be to create a specific database tables that specializes on this specific query. This table would have to be filled or updated everytime a new event is applied to the system (for example, by using hooks). Other approaches may also be possible, so feel free to experiment. It may be useful to read up more on *CQRS* and more specifically *Read Models*.