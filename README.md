# Message-Handling-Api

## Overview

This is an API that handles messages and it is meant to be consumed by a messaging app, similar to WhatsApp.

## Authentication and Authorization

Users get access to the API via Token Authentication. The only features that are available to unauthenticated users is creating an account and logging in to obtain a user token.

Only users with a `GroupAdmin` role can edit, add or remove members or delete groups they have created. Users obtain this role when they create a chat group, but they have to login again afterwards to obtain another token that identifies them as group admins in order to have `GroupAdmin` priviledges.

### Token Authentication

To use a token on your http request, you apply it in the `Authorization` http header as follows:

- Authorization: Bearer [your token]. For instance: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9`

To authorize the Swagger UI, click the 'Authorize' button and insert 'Bearer [your token]' in the `Value` input field.

## Prerequisites

- Visual Studio Code
- .NET Core SDK 2.1 or later
- C# for Visual Studio Code
- SQL Server

## Setting up the database

- To generate the database, execute the `MessageHandling.sql` file. This will create the `MessageHandling` database and its accompanying tables as illustrated in the [conceptual design](Database/Conceptual-design.jpg).

- Don't forget to change the database connection string on `appsettings.json` to the one corresponding to your database configuration.

## Running the API

- After opening the API folder with Visual Studio Code, a dialog box may appear asking if you want to add required assets to the project, select Yes.

- If there is another dialog box that tells you there are unresolved dependencies from 'MessageHandlingApi.csproj', select Restore and wait for the restore process to complete. You may need to restart Visual Studio Code afterwards.

- Press F5 to run the API. A default browser will automatically be launched with the following URL: http://localhost:5025/index.html where the swagger UI is served.

## Using the API

Thanks to the the swagger UI, there is a summary guide for what every API controller method does. Wherever there is a `contact` method parameter, an account unique username is to be provided. The rest of the parameters are self explanatory.

For profile picture upload, the form data http header needs to be applied as follows: `Content-Type": "multipart/form-data`. The data must be present in the body.