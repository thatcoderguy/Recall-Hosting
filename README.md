Recall-Hosting
==============

A group of projects for setting up and running a hosting company

This project has 5 branches
===========================

<b>Recall-hosting-windows-service.</b><br />
This is the service project, the service is what interfaces with the other services (e.g. Email, iis etc) to se setup accounts, host headers, DNS records etc...

<b>Example-Interface.</b><br />
This is an example of how a web interface can be built, this will be used by customers/staff to issue commands to the windows service. The interface also stores a copy of the configuration of all email accounts/DNS records/iis headers etc.. So that they don't have to be read from the services.

<b>IIS-Proof-of-concept.</b><br />
This is just test code to test some concepts, so that they can be implemented into the service e.g. Creating DNS records, IIS headers etc...

<b>Account-Service.</b><br />
This service monitors accounts and payments; the ideal is to alert customers/staff when accounts are overdue, and eventually issue a command to deactive an account's services when over an overdue threshold

<b>Website-Monitor.</b><br />
This monitors a website watching for changes; initially this monitor will alert the website owner if the website has "gone down". Later this can be updated to be a version history for the website owner.


Installation and Usage
======================

The windows service must be build and installed first, then the interface can then be setup.
The windows service has a SQL file which sets up the database, as does the example interface.


TO DO
=====================

A real web interface needs writing, with some branding features.

The windows service still needs to support:<br />
> IIS header create/delete/update<br />
> DNS record create/delete/update<br />
> Tie into a domain registrar to register domains (e.g. Using fasthosts API)<br />
> Database create/delete<br />
> FTP area & accounts create/delete/update<br />
> SSL certificate registration<br />
> Setup website monitors<br />

> There is also an update class which can eventually be used for updating the service when a new version is released.<br />

> Lastly, the worker class also needs to call a stored procedure that tells it how many commands are waiting to be processed, and once the count goes over a threshold, more worker threads are created.<br />

The account service, and website monitor services also need writing.

