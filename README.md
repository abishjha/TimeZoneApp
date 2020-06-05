# Time Zone App

This app is a full-stack application with the backend as a .NET Core 3.0 REST API and the front-end as an Angular 8 application.  It lets the user register, login, and CRUD time zones that are of value to the user.  The display shows all of the users time zones and the difference with the current browser time.  

Additionally there are three different types of user: User, User Manager, and Administrator.  These roles have different permissions.  They are:
1. User => able to CRUD ones own time zone entries.
2. User Manager => able to CRUD users.
3. Admin => able to CRUD users and their time zones.

This is a simple application (in terms of business logic) but demonstrates good coding practices and MVC architecture with proper authentication, authorization, and a good app flow.
