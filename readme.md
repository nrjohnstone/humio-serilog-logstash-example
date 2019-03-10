# Summary
Contains a working example of using Serilog with the Http sink to log events via Logstash to the Humio logging platform.

# Overview

## Sample Application
The sample .NET application uses Serilog and the Serilog Http sink to send a user registered structured log event to logstash.

## Logstash
Because of the batching nature of the Serilog Http sink, the logstash pipeline for receiving them must split the array of events into individual events for sending to Humio.

This is what the "split" command in the filter section of the serilog-http.cfg is doing.

It is also removing the "headers" structure that the Serilog Http sink adds, since for my purposes this is information I prefer not to have on every single log record.

There is also an output for elasticsearch that points to the current Humio cloud API and uses an environment variable HUMIO_INGEST_TOKEN to authenticate. Getting this value is discussed later. 

# Setup
Before being able to run the example there a few things that need to be manually configured first.

## Humio

### Parsers
The Serilog Http Sink sends batches of events to logstash which means that the events arriving at Humio don't match the default serilog-jsonformatter currently provided by Humio @ 10-Mar-2019.

This means we need to configure a custom Parser to match the Logstash pipeline, because the Serilog event is contained within a JSON property called "Event".

The parser syntax looks like below

```
parseJson()
| @timestamp := parseTimestamp(field=Event.Timestamp)
| @display := format("%1$-13s | %2$s", field=[Event.Level, Event.RenderedMessage])
| drop([Event.Timestamp, Event.RenderedMessage])
```

Once you have the customer parser created in Humio you need to create an ingest token for it.

### Ingest Tokens
Parsers are attached to different ingest streams, in the Humio UI create a new ingest token and attach the custom parser from the previous step to it.

Logstash will also need the ingest token value to be able to authenticate and send logs to Humio.

In the docker-compose.yml file, the logstash container will pass the value of the environment variable HUMIO_INGEST_TOKEN

An alternative to using an environment variable is to create a docker compose environment file and configure the value in there. This is described more in the docs on docker-compose environment files [here](https://docs.docker.com/compose/env-file/)

# Running

Once you have configured your custom parser in Humio, created an ingest token and attached the custom parser to it and made your ingest token available to the logstash container you should be able to run the example as follows 

* Run docker compose up from the root directory to get logstash running
* Build and run the .NET sample project to send a structure log of a random user registering

