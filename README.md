BrowserLog
============
Use your browser as a live log viewer with this tiny .net library 

[![Build status](https://ci.appveyor.com/api/projects/status/github/alexvictoor/BrowserLog?svg=true)](https://ci.appveyor.com/project/alexvictoor/BrowserLog)

[![Server logs in your browser console](https://raw.githubusercontent.com/alexvictoor/BrowserLog/master/screenshot.png)]

BrowserLog is a log4net appender leveraging "HTML5 Server Sent Event" (SSE) to push logs on browser consoles. 
It relies on .NET 4.5 and log4net, no other external dependencies!


Usage
-----

Activation requires 3 steps:
1. configuration of your build to add a dependency to this project 
2. configuration of the appender in the log4net configuration
3. inclusion of a javascript snippet in your HTML code to open a SSE connection

TBD - nuget package and command

Below an XML fragment example that shows how to configure logback on the server side
```xml
<log4net>
  ...
  <appender name="WEB" type="BrowserLog.BrowserConsoleAppender, BrowserLog">
    <Host>192.168.0.7</Host> <!-- Optional, the IP address on which the SSE server will be bound. If not specified try to detect the locla IP of the host by itself -->
    <Port>8082</Port> <!-- Optional, this is the port on which the HTTP SSE server will listen. Default port is 8765 -->
    <Active>true</Active> <!-- Optional, if false the appender is disabled. Default value is true -->
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline" /> <!-- Use whatever pattern you want -->
    </layout>
  </appender>
  ...
```

In the browser side, the easiest way to get the logs is to include in your HTML document javascript file BrowserLog.js. This script is delivered by the embedded HTTP SSE server at URL path "/BrowserLog.js".  
A simple way to achieve that is to use a bookmarklet. If you use your browser to display the 'home page' of the embedded HTTP SSE server, you will see an ugly white page. However on this ugly blank page you will see a ready to use bookmarklet whose code looks like that:

    (function () { 
        var jsCode = document.createElement('script'); 
        jsCode.setAttribute('src', 'http://HOST:PORT/BrowserLog.js'); 
        document.body.appendChild(jsCode); 
    }());

Disclaimer
---------
1. For obvious security concerns, **do not activate it in production!**  
2. This is a first basic not opimized implementation, no batching of log messages, no buffering, etc
