#!/bin/bash
set -e

filebeat -e -c /etc/filebeat/filebeat.yml &

exec dotnet Weather.Api.dll
