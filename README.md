2/17/2025

The CWDocMgr solution as of 4 or 5 days ago was basically an ASP.NET web application. Motivated by a job application, I decided to add a WPF version 
as quickly as possible. I didn’t know WPF at the time, so I had to learn.

CWDocMgrApp is the result. It does the basic tasks:
-	Uploads document images and puts document information a Sqlite database.
-	Displays the list of documents.
-	Views the document
-	Gives the user the option to OCR a document.
-	Displays the OCR text next to the viewed document.

It’s not finished: the UI needs to be cleaned up, the displayed document needs to be scaled to fit the page (or some other scheme), tests need to be 
written, etc. But I feel pretty good about getting that much done (and learning some WPF) in a short time frame.

Note that because I could not find a way to use database-based Identity in a WPF project, I had to rip it out and put in homegrown authentication. 
I have not yet updated the old CWDocMgr to match, so that project doesn’t currently compile.

Also, to run it (if anybody's interested) you'll have to add some folders and update appsettings.json to match and provide a Sqlite database. I don't currently
have db migrations written, so the tables will have to be set up by hand. Probably not worth the effort.
