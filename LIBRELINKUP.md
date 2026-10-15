# LibreLinkUp setup

GlucoseTray can read shared Libre readings directly from Abbott's cloud, using the unofficial LibreLinkUp API.

1. Keep the official Libre app collecting and uploading sensor readings on your phone.
2. In **Connected Apps > LibreLinkUp > Manage > Add connection**, invite a follower email you control.
3. In the official LibreLinkUp app, create/sign into that account, verify its email, accept the invitation and any pending terms, and confirm a current reading appears.
4. Set `DataSource` to `LibreLinkUp` in `appsettings.json`, then run GlucoseTray. The default source remains Dexcom.
5. Enter the follower email/password in the masked sign-in dialog. Right-click the tray icon to sign in again or sign out.

The password is stored in Windows Credential Manager under `GlucoseTray/LibreLinkUp/<email>`, never in settings. Only the selected follower email stays in settings; bearer tokens stay in memory. Sign-out deletes that account's saved password. Startup migrates a legacy `LibreLinkUpPassword` field into Credential Manager before removing it; failed vault writes preserve the original file.

## Settings

`LibreLinkUpRegion` defaults to `EU`; bounded region redirects use a fixed HTTPS allowlist. Swiss account connectivity has been tested locally. Supported codes: AE, AP, AU, CA, DE, EU, EU2, FR, JP, US, LA, RU, CN. HTTP redirects and unknown regions are rejected.

`LibreLinkUpApiVersion` defaults to `4.16.0`. Compatibility can change because this is an unofficial API. Complete account consent and terms in the official app.

With one shared connection, leave `LibreLinkUpPatientId` empty. With multiple connections, configure the selected patient ID. Missing/ambiguous patients and changed accounts clear previous readings, including requests finishing after settings changed. Same-patient outages may retain the original reading and timestamp with a visible connection error.

Use `DisplayUnitType: Mmol` for mmol/L or `Mg` for mg/dL. LibreLinkUp uses its UTC FactoryTimestamp and ValueInMgPerDl fields; the server-unit setting does not affect the conversion. Existing thresholds and alert behavior remain configurable.

## Status and limits

Hover/double-click the tray number for connection status and reading time. Failed refreshes do not produce glucose alerts. Glucose notifications occur on alert transitions; they are not repeating audible alarms. Keep official Libre alarms enabled.

Tests cover production-DI reading/alert/cache paths, region/header handling, reauthentication, malformed data, account/patient changes during requests, and a synthetic Windows Credential Manager round-trip. Live Swiss follower login/polling has also been checked in the development fork. Native Windows notification sound and sleep/resume remain manual checks. Passwords/tokens are never included in LibreLinkUp response/error messages; HTTP factory logging is disabled for this client.
