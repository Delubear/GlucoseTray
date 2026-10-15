# Diagnostic privacy

HTTP logs record only allowed methods and numeric statuses. Failure logs contain fixed operation/category labels; URLs, headers, payloads, settings and raw exceptions are excluded, including at Trace level.

Use the final HTTPS Nightscout address without embedded URL credentials. Plain HTTP and HTTP redirects are rejected to avoid forwarding credentials. This is an intentional compatibility change for HTTP-only or redirecting installations. Passwords and tokens still need to reach the selected service for authentication; OS memory dumps and remote-service logging are outside these controls.
