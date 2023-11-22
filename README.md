# Next Stop Announcement Generator

Generates next stop announcements from a CSV using [Google's Cloud Text-to-Speech](https://cloud.google.com/text-to-speech/) of stop names and [SSML pronounceation](https://cloud.google.com/text-to-speech/docs/ssml) data. The program could be easily extened to use other text-to-speach providers.

[![.NET](https://github.com/CUMTD/NextStopAnnouncementGenerator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/CUMTD/NextStopAnnouncementGenerator/actions/workflows/dotnet.yml)

## CSV Format

CSV should be in the format: `stop_name,ssml_data`. It should not containa header row.

`stop_name` is required. It is what will be read if no SSML data is provided.
`ssml_data` is optional and will override the stop name if provided.


## Setup

Before the app will function, you will need a Google Cloud Service Account and Credentials.
The account should have access to the Google Text To Voice services.
Follow the instructions here: https://cloud.google.com/docs/authentication/production#creating_a_service_account.

Download the credentials JSON file and save it to `App\googleCreds.json`.
