# Gelf4NLog [![Build status](https://ci.appveyor.com/api/projects/status/pb6q928iay96pyop?svg=true)](https://ci.appveyor.com/project/Certegy/gelf4nlog) [![Coverage Status](https://coveralls.io/repos/github/Certegy/Gelf4NLog/badge.svg?branch=master)](https://coveralls.io/github/Certegy/Gelf4NLog?branch=master)

Gelf4NLog is an [NLog] target implementation to push log messages to [GrayLog2]. It implements the [Gelf] specification and communicates with GrayLog server via UDP.

## Solution
Solution is comprised of 2 projects: *Target* is the actual NLog target implementation, and *UnitTest* contains the unit tests for the NLog target
## Usage
Use Nuget:
```
PM> Install-Package Gelf4NLog.Target
```
### Configuration
Here is a sample nlog configuration snippet:
```xml
<configSections>
  <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog" />
</configSections>

<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

	<extensions>
	  <add assembly="Gelf4NLog.Target"/>
	</extensions>

	<targets>
	  <!-- Other targets (e.g. console) -->
    
	  <target name="graylog" 
			  xsi:type="graylog" 
			  endpoint="udp://logs.local:12201" 
			  application="MyApp"
			  environment="PROD"
	  />
	</targets>

	<rules>
	  <logger name="*" minlevel="Debug" writeTo="graylog" />
	</rules>

</nlog>
```
### Redacting sensitive log information
It is possible to redact sensitive information from each log entry using redaction regular expressions:

```xml
<targets>
	<target name="graylog" 
		xsi:type="graylog" 
		endpoint="udp://logs.local:12201" 
		application="MyApp" environment="PROD">
		<redact pattern="4[0-9]{12}(?:[0-9]{3})?" replacement="_REDACTED_" />
		<redact pattern="TEST" replacement="****" />
	</target>
</targets>
```

Options are the following:
* __name:__ arbitrary name given to the target
* __type:__ set this to "graylog"
* __endpoint:__ The address of the GrayLog2 server e.g. udp://logs.local:12201
* __application:__ The application
* __environment:__ The environment of the application instance

###Code
```c#
//excerpt from ConsoleRunner
var eventInfo = new LogEventInfo
    			{
					Message = comic.Title,
					Level = LogLevel.Info,
				};
eventInfo.Properties.Add("Publisher", comic.Publisher);
eventInfo.Properties.Add("ReleaseDate", comic.ReleaseDate);
Logger.Log(eventInfo);
```

[NLog]: http://nlog-project.org/
[GrayLog2]: https://www.graylog.org/
[Gelf]: http://docs.graylog.org/en/2.1/pages/gelf.html

##Contributing
Would you be interested in contributing? All PRs are welcome.
