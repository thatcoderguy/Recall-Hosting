Recall-Hosting
==============

The Recall Hosting Service is a windows service which is intended for people who wish to start their own hosting company. The service will evenutally be able to manage; email accounts, DNS records, databases, accounts, SSL certificates, and IIS entries.


Installation and Usage
======================

Before installing the service, the "Work horse database setup.sql" file needs to be executed on an SQL Server instance.

The project includes a service installation project; once the srrvice has been built, the service can be installed on a server. The seperate emailinterfacealpha project is the management software which creates "commands" for the service to do it's job.

The service project has references to MailEnable DLLs; the MailEnable email server software is required as the service has been build around using this software as the email service. 

To install the service use "installutil <appname.exe>"


Edit the config.txt file and change the lines related to the SQL server instance and IP address of the IIS server.


The interface project (only in Alpha status) can be setup and a website.

