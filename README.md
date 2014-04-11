Recall-Hosting
==============

A group of projects for setting up and running a hosting company

This project has 3 branches
===========================

Recall-hosting-windows-service.  
This is the service project, the service is what interfaces with the other services (e.g. Email, iis etc) to se setup accounts, host headers, DNS records etc...

Interface-alpha.

This is an alpha build of the web interface which will be used by customers/staff to issue commands to the windows service.
The interface also stores a copy of the configuration of all email accounts/DNS records/iis headers etc.. So that they don't have to be read from the services.

Proof-of-concept
This is just test code to test some concepts, so that they can be implemented into the service e.g. Creating DNS records, IIS headers etc....


Installation and Usage
======================

The windows service must be build and installed first

The inteface can then be setup.


TO DO
=====================

