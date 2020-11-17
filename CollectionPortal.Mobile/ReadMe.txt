Project Description

This project implements an ASP.NET application with several services that will let you easily create iOS, Android and Windows Phone apps. These mobile apps  will reuse the database, as well as certain aspects of the data model, application logic and UI settings of your existing XAF applications. This project contains several main files:

1. MobileApplication.cs - this class derives from the new MobileApplication class similarly to WinApplication or WebApplication in appropriate platforms. An instance of this class is created for each request from the client application.

2. DataService.svc - this is the backend service for mobile applications used to serve requests, manage security and execute actions. Technically, it is a regular OData service (WCF Data Services 5.0) based on XPO OData V3 provider , XPO being the only currently supported ORM (Entity Framework support is coming later).  

3. MetadataService.svc - this service supplies mobile application configuration data to a separate device simulator service.

4. Index.cshtml and player.cshtml - these web pages provide the capability to run and test a mobile app locally.  They contain s script that queries the aforementioned backend data and UI metadata services and generate the actual HTML5/JS UI inside the web browser.

5. DownloadReportService.svc - this service provides the Download Report Action that enables downloading a report as PDF or other popular formats.

6. Application Model difference files (XAFML files) that keep application settings specific to the current application. Difference files can be designed with the Model Editor. 



Review Frequently Asked Questions

To learn more about the capabilities and limitations of the XAF Mobile UI, review the knowledge base article that answers the most frequently asked questions.
https://www.devexpress.com/go/Newsletter_LateApr2016_XafFaq.aspx

Relevant Documentation

Application Solution Components
https://docs.devexpress.com/eXpressAppFramework/112569

MobileApplication Class
https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Mobile.MobileApplication

XafApplication Class
https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.XafApplication

Application Designer
https://docs.devexpress.com/eXpressAppFramework/112827

Application Model
https://docs.devexpress.com/eXpressAppFramework/112579

Model Editor
https://docs.devexpress.com/eXpressAppFramework/112582
