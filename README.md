BrowserLog
============
Use your browser as a live log viewer with this tiny .net library 

[![Build status](https://ci.appveyor.com/api/projects/status/github/alexvictoor/BrowserLog?svg=true)](https://ci.appveyor.com/project/alexvictoor/BrowserLog)

![Server logs in your browser console](https://raw.githubusercontent.com/alexvictoor/BrowserLog/master/screenshot.png)

BrowserLog is a log4net appender leveraging "HTML5 Server Sent Event" (SSE) to push logs on browser consoles. 
It relies on .NET 4.5 and log4net, no other external dependencies!


Usage
-----

Activation requires 3 steps:  

1. configuration of your build to add a dependency to this project 
2. configuration of the appender in the log4net configuration
3. inclusion of a javascript snippet in your HTML code to open a SSE connection

First thing first, you need to add a nuget dependency to BrowserLog. If you are using a recent version of log4net you can add a nuget reference to BrowserLog to your project as follow:

    PM> Install-Package BrowserLog.log4net

If you are using an old version of log4net (version<=1.2.10) you need to use the 'old' log4net package:

    PM> Install-Package BrowserLog.Old.log4net

There are two different packages because log4net 'public token' has changed between version 1.2.10 and 2.03... Hence it might be very difficult to upgrade log4net on some legacy projects.

Below an XML fragment example that shows how to configure logback on the server side
```xml
<log4net>
  ...
  <appender name="WEB" type="BrowserLog.BrowserConsoleAppender, BrowserLog.log4net">
    <Host>192.168.0.7</Host> <!-- Optional, the IP address on which the SSE server will be bound. If not specified try to detect the local IP of the host by itself -->
    <Port>8082</Port> <!-- Optional, this is the port on which the HTTP SSE server will listen. Default port is 8765 -->
    <Active>true</Active> <!-- Optional, if false the appender is disabled. Default value is true -->
    <Buffer>10</Buffer> <!-- Optional, the size of the buffer used to replay logs on connection. Default value is 1 -->
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" /> <!-- Use whatever pattern you want -->
    </layout>
  </appender>
  ...
```

Warning: using default configuration, without specifying HOST property, the server is not reachable on http://localhost or 127.0.0.1 . You need to use the "windows host name" of your box, the first one returned by command "ipconfig /all" 

In the browser side, the easiest way to get the logs is to include in your HTML document javascript file BrowserLog.js. This script is delivered by the embedded HTTP SSE server at URL path "/BrowserLog.js":

    <script src="http://HOST:PORT/BrowserLog.js"></script> 

It gets even simpler when using a bookmarklet. To do so use your browser to display the 'home page' of the embedded HTTP SSE server (at URL http:// HOST : PORT where HOST & PORT are the parameters you have used in the log4net configuration). You will get an ugly blank page, the main purpose of this page is to test your configuration but most of all to bring you a ready to use bookmarklet. This bookmarklet looks like code fragment below:

    (function () { 
        var jsCode = document.createElement('script'); 
        jsCode.setAttribute('src', 'http://HOST:PORT/BrowserLog.js'); 
        document.body.appendChild(jsCode); 
    }());

Why SSE?
--------
[Server Sent Events](https://en.wikipedia.org/wiki/Server-sent_events) while being quite simple to implement, allows to stream data form a server to a browser leveraging on the HTTP protocol, just the HTTP protocol. Resources required on the server side are also very low. Simply put, to stream text log it makes no sens to use WebSocket which is much more complex.   
All newest browsers implement SSE, except IE... (no troll intended). Chrome even implements a nice reconnection strategy. Hence when you are using a Chrome console as a log viewer, log streams will survive to a restart of a server side process, reconnection are handled automatically by the browser.

Multiple server-side process?
-----------------------------
Let's say that you are working on a web application in a micro service architecture. You might need to get logs from a lot of services. You could use a bookmarklet for each process but you can also create your own bookmarklet for a given environment.
Below an example:

    (function () { 
        var jsCode = document.createElement('script');                        // open a first stream  
        jsCode.setAttribute('src', 'http://HOST1:PORT1/BrowserLog.js'); 
        document.body.appendChild(jsCode); 
        jsCode = document.createElement('script'); 
        jsCode.setAttribute('src', 'http://HOST2:PORT2/BrowserLog.js');      // a second one
        document.body.appendChild(jsCode);
        ...                                                                  // as many as you need
    }());


Custom colors and styles?
-------------------------
Once you are connected to several log streams, you will might want to get different visual appearance for those streams.  
By default all streams use default browser log styles. Styles can be customized by adding special attributed to BrowserLog.js script tag:

    <script src="http://HOST:PORT/BrowserLog.js" style="font-size: 18px; background: cyan" ></script>

Styles attrbutes can also be specific to a logging category:

    <script src="http://HOST:PORT/BrowserLog.js" style="color: black;" style-error="color: red; font-size: 18px;" ></script>

With above example, all logs are written in black on white, size 12px, except error logs written in red, size 18px.  
Below a bookmarklet code that gives similar results:

    (function () { 
        var jsCode = document.createElement('script'); 
        jsCode.setAttribute('src', 'http://HOST:PORT/BrowserLog.js'); 
        jsCode.setAttribute('style' 'color: black;');
        jsCode.setAttribute('style-error' 'color: red; font-size: 18px;');
        document.body.appendChild(jsCode); 
    }());

Custom styles can be specify for info (style-info) and warn (style-warn) logs as well.

Disclaimer
---------
1. For obvious security concerns, **do not activate it in production!**  
2. This is a first basic not opimized implementation, no batching of log messages, no buffering, etc
