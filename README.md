# Kindle NewsReader

A web application that scrapes news articles from 
Associated Press, formats them into newspaper-like 
pdf files, and automatically delivers them to registered 
Kindle devices every morning.

Associated Press allows non-commercial use of their 
content as per Section 7 of their 
<a href="https://apnews.com/termsofservice" target="_blank">Terms of Service</a>.
Kindle NewsReader is a completely free and open source 
application.

This repo consists of two applications, NewsScraper and 
NewsScraper-Web

### NewsScraper

A .NET console application that performs the web scraping,
newspaper pdf generation, and delivery to Kindle devices.
This program is scheduled to run every morning at 8am EST.

### NewsScraper-Web

An ASP .NET CORE MVC application that provides a web
user interface for users to register and curate the news
categories that they want to read in their newspapers.
