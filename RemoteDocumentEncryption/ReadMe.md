# Demo Server and Client-side Library for Remote Document Encryption

## WARNING!

DO NOT USE this server for anything other that technical demonstrations!

It does not implement any of the mechanisms such as obtaining user consent or encrypting user data at rest and so is not compatible with EU law.

It also uses the default ASP.NET Identity Provider by Duende which is free for open source projects at time of writing.

## Contents

This repository contains the server and web client components for a demonstration of the Remote Document Encryption concept to be using in conjuction with the RDE Android App, found at http://sdfsfsfsf/github.com.

## Intro to Remote Document Encryption

See https://arxiv.org/abs/1704.05647 for Eric Verhuel's full introduction to the concept.

Using a subset of the readable contents of a given passport, a seed for an encryption key can derived such that the ONLY the physical prescence of the original passport can be used to re-calculate the seed, and so derive the same encryption key. 

Specifically, when using the Chip Authentication protocol, a cryptographic DH exchange is performed using the corresponding public key from the passport. Performing a Read Binary call in a Chip Authentication session be simulated WITHOUT the passport, providing a subset of it's contents have been read and stored previously.
The presence of the passport is required during decryption because the DH exchange private key used for the Chip Authentication protocol cannot be directly read from the passport and stored elsewhere.


The response to a Read Binary call on a file in a passport can be reconstructed WITHOUT the presence of the passport given:

* The contents of the passport's DG14 file are known (or at least the Chip Authentication security parameters)
* The parameters of the Read Binary call are known - the id of the file and number of bytes to read
* Sufficient contents of the passport file targetted by the Read Binary call are known.
* The PICC key pair used to start the Chip Authentication Session is known.

When a message is sent, the client-side library does the following:
1. Given passport enrolment information (Chip Authentication security parameters), a new PICC key pair is generated. 
1. The PICC private key and the PCD public key of the passport are used to start a Chip Authentication session simulator.
1. The Read Binary Command APDU and the corresponding Response APDU are generated using the simulator.
1. The Read Binary response can be used as a seed for an encryption process.

To decrpyt the message the reciever has to have:
* The enrolled passport
* The PICC public key
* The simulated Read Binary Command APDU.

Starting a new Chip Authentication session on the enrolled document with the PICC public key and sending the Read Binary Command APDU will generate the same Read Binary response as before, hence the message can be decrypted.

TLDR, the 'Remote' part of RDE refers to being 'away from the passport' when deriving an encryption key.

## Running the server

### Requirements
* Source code from this repository
* .NET 6
* SQL Server (see below)

SQL Server is not a strict requirement. Other databases compatible with Entity Framewok 6 can be used.

### Starting
1. Start the database.
2. Edit the database connection string in the settings file.
3. Edit the BaseURL setting in the settings file to correspond to the external domain/ip address and port that the site will be available on.
4. Type 'dotnet run' from the command line.

## Demo script

1. Keep your android phone and your Netherlands passport handy
1. Start this server
1. Register an account on the server
4. Login to your account
4. Navigate to the Home page.
5. Download and install the android app
4. Navigate to the Link page - this shows a QR code.
7. Open the app - it will prompt to scan the QR code and link the app to the user account on the server. The app will show the (empty) Receive Messages screen.
9. In the app, navigate to the Enrol Document screen.
10. Enter the MRZ and a memorable name for this passport and tap Next.
11. Scan the passport NFC to complete the process. The app will show the (empty) Receive Messages screen.
12. On the website, navigate to the Send Message page.
13. Send yourself a message, or ask another user to send you one.
14. In the app, refresh your received messages.
15. Tap the new message to read it.
11. Scan the passport NFC to complete the process.
17. Read the message!

## Using the client-side library in another project.

The project RdeCaSessionUtilities contains the source code of a .NET 6 library which can be used in either:

1. A web browser application, as part of a Blazor web client project.
2. A Xamarin phone application. This is untested but it should be possible to target .NET Standard without otherwise altering the source code.
3. A Javascript or other web browser project as a WASM component. However, this project does not provide WASM bindings at time of writing.

## Limitations

### Server

- Very basic UX.
- The login page is the default provided by the Duende IdentityServer. This can be customised by scaffolding. See https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-6.0&tabs=visual-studio.

### Android App

- Only a Netherlands passport using PACE or BAC is supported.
- Very basic UX.
- Most tasks do not give clear indications of progress.
- Camera scanning of MRTD MRZs is not supported.
- If the contents of a decrypted file are not plain text, they will be displayed as hex.
- Only the name and contents of the first decrypted file are displayed.

#### Developer Notes

- Current version of JMRTD is 0.7.34. This appears to have a bug when performing PACE on a Netherlands Driving License.

