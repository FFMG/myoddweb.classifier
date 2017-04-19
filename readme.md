# MyOddWeb.Classifier

<!---
Mono does not work currently...
 ## Status
[comment]: <> [![Build Status](https://travis-ci.org/FFMG/myoddweb.classifier.svg?branch=master)](https://travis-ci.org/FFMG/myoddweb.classifier)
-->

## What is it
MyOddWeb classifier, as it's name mentions, classifies emails as they arrive and move them to predefined folders.

Traditionally, rules are used to move emails to folders, for example an email from work, would be moved to the "Work" folder.

With a classifier you can create categories, "Personal", "NSFW", "Spam" and so on, and if a mail arrives, regardless who it is from, it will be moved to the appropriate folder.

Using the "Work" example above, if Bob from Human Resources sends you a joke email, a rule would have traditionally moved that email to the "Work" folder, (as Bob is a co-worker). With a Classifier, that email would be moved to a "Joke" folder and you can then look at it later. 

## How does it work

MyOddWeb is a [Naive Bayes classifier](https://en.wikipedia.org/wiki/Naive_Bayes_classifier), in simple terms, it makes an educated guess as to what category the email might belong in considering all the thousand of emails that you might have already received. 

At first, you 'train' your classifier with your own "Categories", what you consider a "joke" might be considered "NSFW" by others.

The more you train it, the better the classifier gets.

But it does not take that long, really, invest a couple of day creating good Magnets and good Categories and in no time your classifier will be self sufficient. 

## Setup
### From setup

- Download the setup app, (in the [Classifier.Setup/Output](https://github.com/FFMG/myoddweb.classifier/tree/master/Classifier.Setup/Output) sub folder for now)
- Run it
- Start outlook
	- You will need to accept the certificate, (only first time).
- Create some categories
- Create some magnets
- Categorise some emails.

### From the code

- Get the code
- Build it using visual studio 2015 or later, (I have not tried the others).
- You can either debug and/or simply run outlook.

## Versions
#### 0.6.0.1 - 0.6.0.4 (19/04/2017)
- Added debug log messages to time how slow classification is.
- Updated to classifier engine 1.5.6.0, (faster)
  - Added a couple of css values to no parse, (max-width/width etc...)
- Updated the interface 0.6.0.4, (no change, just align the number)
- Fixed a couple of issues with the 'detailed' html view.
  - Speed up the way the display is created.
  - Fixed actual html created, (invalid tables and so on).

#### 0.5.0.1 - 0.5.0.2 (06/11/2016)
- Option to automatically train classified mail, (false by default)
- Option to automatically train classified mail using magnet, (true by default)
- Updated to 1.5.2 engine, ( has better/faster classification)
	- Classifications are now threaded.
- Options text now gives the version number + engine version number.	

#### 0.4.0.1 - 0.4.0.2 (02/11/2016)
- Fixed a few issues in the tokenizer.
- Made some changes in the engine with the lessons learned over the last 6 months.
  - More html code is now ignored.
- We can now see the classification details to see each words classification.
	- Added a menu option to display raw-text classification.
	- Added a 'viewer' application to test raw text to confirm that the engine is working as expected.
  
#### 0.3.0.5 (01/06/2016)
- Fixed a couple of EventSource issues.
- Changed the output name of the setup.exe so it includes the version number.

#### 0.3.0.4 (17/02/2016)
- Updated to new Classifier engine 1.1.0, (bug fixes)
	- New setting to remove very common words, ("Option.CommonWordsMinPercent").
- Added ***GetEngineVersion()*** to use the new engine version.  
- Added possibility to change the common word percentage.

#### 0.3.0.3 (01/02/2016)
- Updated to new Classifier engine, (bug fixes)  

#### 0.3.0.2 (20/01/2016)
- Removed a couple of outlook/html/xml words that could confuse the classifier.  

#### 0.3.0.1 (24/12/2015)
- Fixed a typo in the name  

#### 0.3.0.0 (24/12/2015)
- Added weights
- Added option to set the magnets/user specified items weight.  

#### 0.2.1.0 (11/11/2015)
- Removed dependency on .NET 4.5.2 and replaced it with more realistic 4.5.0  

### 0.2.0.0 (09/11/2015)

- By default we compile the x86 and x64 binaries.
- Created 2 output folders.
- We now automatically load the x64 or x86 dll depending on the version of outlook running.

### 0.1.0.0 (05/11/2015)

- **Beta**
- Initial release


## FAQ
### General
#### What are magnets
Magnets are like 'shortcuts' for your classifier, for example, if you get an email from your mom, you can create a magnet to always classify her emails has 'Personal', this will help the classifier as well as prevent some of her mails been wrongly classified.
As a rule, the more magnet you have, the better.

**NB**: Should you add a magnet for spam emails? The short answer is no, spam emails come from thousands if not hundreds of thousands of servers. You cannot realistically create a magnet for all of them.
But you could always create one for the repeat offender, personally, I don't have one.

#### How quickly does the app learn
It is fairly quick, but you should monitor the first few days to make sure that everything is on the right path.
Eventually, it will keep on learning and you will not need to worry.

**NB**: You should always check your spam folder to make sure that nothing has been sent there by mistake.

#### How many magnets/categories can I have
There is no limit on the number of categories and/or magnets.
But you have to be realistic and not have too many.

### Weights
Weights are ways of marking an email more or less important depending on .

For example, if you use a magnet then the weight of that email is slightly more than when the engine tries to categorize the email itself.

When you categorize an email yourself, then this is considered to have the greatest weight, (who knows better than you what an email should be). 

### Database
#### Where is the database
The database is located in your "**%appdata%\myoddweb\Classifier**" folder.

#### What format is the database
It is a [SQLite](https://www.sqlite.org/ "Sqlite") database

#### Can I clear my database
Just close outlook, delete the database and restart outlook.

# Todo

- Create some tutorials.
	- How to setup
	- Manage categories
	- Manage Magnets
	- Maybe create a youtube account and explain various features.
- Give a link to the setup program on http://www.myoddweb.com
- Update the site with more information.
- Test on versions of Outlook, please add a message if you are able to test.
	- Test on outlook 2016
	- <s>Test on outlook 2013</s>
	- Test on outlook 2010
	- Test on outlook 2007
- Test on 32bit versions of Outlook.
	- On 32bit machines
	- On 64bit machines
- Test on 64bit versions of Outlook
- Ask someone to give us a certificate rather than the temp one we have.
- Languages
- Add copyright notice in front of all the files...

## LICENSE
[Apache License](https://github.com/FFMG/myoddweb.classifier/blob/master/LICENSE) / (http://www.apache.org/licenses/)
