   Using Windows Presentation Foundation (or WPF) and C#, I managed  to create a software application aiming at facilitating the
process of sending multiple email messages to recipients’ addresses imported from CSV files, along with other relevant 
information, in order to customise each subject and body fields, if that’s what the user wishes.

   Both email subject and body are written only once and strings like “{1}”,”{2}” are used within, so that one can indicate 
which CSV column values should replace them, during the process of sending the messages, resulting in the customisation 
I have already mentioned.
   
Suppose we have the following information in a .csv file: 

user1@yahoo.com         Sir      United Kindom      Monday       the first of June
user2@gmail.com	      Madam	   Germany	          Monday	     the eight of June
user3@myDomain.org      Sir      Hungary  	       Monday	     the fifteenth of June
user4@gmail.com	      Madam    Romania	          Monday	     the twenty-second of June
