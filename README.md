   Using Windows Presentation Foundation (or WPF) and C#, I managed  to create a software application aiming at facilitating the process of sending multiple email messages to recipients’ addresses imported from CSV files, along with other relevant 
information, in order to customise each subject and body fields, if that’s what the user wishes.

Both email subject and body inserted  in the GUI  may  contain  strings like “{1}”,”{2}”, so that one can indicate which CSV column values should replace them, during the process of sending the messages, resulting in the customisation I have already mentioned.
Given the test.csv file (uploaded as well on github.com/BulkEmailSender), let's suppose the user inserts in the GUI the following email subject and body:

Subject:
I look forward to visiting {3}

Body:
Dear {2},

I will be happy to visit your beautiful country {3} on {4}, {5}.

All the best!

Catalin Tismanaru


Given the fact that both subject and body contain substrings like {"numberOfColumn"}, before sending each email message, both will be changed.
For example the message sent to user1@yahoo.com (first field of the first row of test.csv file) will have the following subject and body:

Subject: 
I look forward to visiting United Kingdom

Body:
Dear Sir,


I will be happy to visit your beautiful country United Kingdom on Monday, the first of June.

All the best!

Catalin Tismanaru





The following constraints apply:

1. Each row shall have at least an empty string on each column.
2. No field shall contain line breaks (CRLF), double quotes, or commas.
