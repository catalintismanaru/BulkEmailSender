using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Net.Mail;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Bulk_Email_Sender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User user;
        InputData inputData;
        private readonly BackgroundWorker worker = new BackgroundWorker();


        public MainWindow()
        {
            InitializeComponent();


            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }


        private void attachementsAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                MyUtilities.FullAttachamentsFilesPathsArray = dlg.FileNames;


                string[] filenames = dlg.SafeFileNames;
                foreach (string filename in filenames)
                    attachementsListBox.Items.Add(filename); 
            }
        }


        private void attachementsRemove_Click(object sender, RoutedEventArgs e)
        {
            attachementsListBox.Items.Remove(attachementsListBox.SelectedItem);
        }


        private void attachementsClearAll_Click(object sender, RoutedEventArgs e)
        {
            attachementsListBox.Items.Clear();
        }


        private void emailsAdd_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "CSV Files|*.csv";
            Nullable<bool> result = dlg.ShowDialog();


            if (result == true)
            {
                MyUtilities.FullEmailsFilesPathsArray = dlg.FileNames;


                string[] filenames = dlg.SafeFileNames;
                foreach (string filename in filenames)
                    emailsFilesListBox.Items.Add(filename);
            }
        }


        private void emailsRemove_Click(object sender, RoutedEventArgs e)
        {
            emailsFilesListBox.Items.Remove(emailsFilesListBox.SelectedItem);
        }


        private void emailsClearAll_Click(object sender, RoutedEventArgs e)
        {
            emailsFilesListBox.Items.Clear();
        }


        private void startSendingEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            //I check the all input data.
            if (!CheckInputData.CheckSubject(subjectTextBox))
                return;
            if (!CheckInputData.CheckSenderEmailAddress(fromTextBox))
                return;
            if (!CheckInputData.CheckSenderUsername(userTextBox))
                return;
            if (!CheckInputData.CheckPassword(passwordBox))
                return;
            if (!CheckInputData.CheckSMTPServer(SMTPServerTextBox))
                return;
            if (!CheckInputData.CheckSMTPPort(SMTPPortTextBox))
                return;
            if(!CheckInputData.CheckEmailsFiles(emailsFilesListBox))
                return;
            if (!CheckInputData.CheckBodyMessage(richTextBox))
                return;


            user = new User
            {
                From = fromTextBox.Text,
                UserName = userTextBox.Text,
                Password = passwordBox.Password
            };


            inputData = new InputData
            {
                User = user,
                Subject = subjectTextBox.Text,
                SMTPServer = SMTPServerTextBox.Text,
                SMTPPort = Convert.ToInt32(SMTPPortTextBox.Text),
                EmailsFiles = emailsFilesListBox.Items.OfType<String>().ToList(),
                AttachamentFiles = attachementsListBox.Items.OfType<String>().ToList(),
                TextMessage = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text
            };


            //A new thread begins by sending all the email messages. I am very carefull not to add multiple times the same event handlers in case the user clicks by the Start button several times in order to start a new session. By default, the Start button is enabled after a session has just started.
            worker.RunWorkerAsync(); 
        }


        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                MessageBox.Show("All the e-mail messages have been sent");
            }


            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.startSendingEmailsButton.IsEnabled = true));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.stopSendingEmailsButton.IsEnabled = true));


            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.progressBar.Value = 0));
        }


        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MailSession(sender,e);
        }


        ///<summary>
        ///This method is executed at the beginning of a background thread which handles the sending of email messages. The method is called in the event handler subscribed to BackgroundWorker.DoWork.
        ///</summary>
        private void MailSession(object sender, DoWorkEventArgs e)
        {
            List<List<string>> allCsvRows = null;
            List<string> emailsFiles = inputData.EmailsFiles;
             

            //Start button is disabled
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.startSendingEmailsButton.IsEnabled = false));


            //reading each CSV file and storing each row in allCsvRows
            int size = emailsFiles.Count;
            string[] FullEmailsFilesPathsArray = MyUtilities.FullEmailsFilesPathsArray;
            for (int i = 0; i < size; i++)
            {
                MyUtilities.ParseCSVFile(ref allCsvRows, FullEmailsFilesPathsArray[i]); 
            }
            try
            {
                MailMessage mailObject = new MailMessage();
                SmtpClient smtpServer = new SmtpClient(inputData.SMTPServer);
                StringBuilder deducedEmailSubject = new StringBuilder();
                StringBuilder deducedEmailText = new StringBuilder();


                mailObject.From = new MailAddress(user.From);
                smtpServer.Port = inputData.SMTPPort;
                smtpServer.Credentials = new System.Net.NetworkCredential(user.UserName, user.Password);
                smtpServer.EnableSsl = true;


                //Check if Attachment Exists
                if (inputData.AttachamentFiles.Count != 0)
                {
                    String[] fullAttachamentsFilesPathsArray = MyUtilities.FullAttachamentsFilesPathsArray;
                    foreach (String attachamentFile in fullAttachamentsFilesPathsArray)
                    {
                        mailObject.Attachments.Add(new Attachment(attachamentFile));
                    }
                }


                int numberOfEmailAddresses = allCsvRows[0].Count;
                //sending each email message
                for (int i = 0; i < numberOfEmailAddresses; i++)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }


                    mailObject.To.Add(allCsvRows[0][i]);


                    //each email subject is here customised in case the user wanted to
                    if (!String.IsNullOrWhiteSpace(inputData.Subject))
                    {
                        MyUtilities.DeduceEmailSubject(inputData.Subject, allCsvRows, i,ref deducedEmailSubject);
                    }
                    mailObject.Subject = deducedEmailSubject.ToString();


                    //each email text is here customised in case the user wanted to
                    if (!String.IsNullOrWhiteSpace(inputData.Subject))
                    {
                        MyUtilities.DeduceEmailText(inputData.TextMessage, allCsvRows, i, ref deducedEmailText);
                    }
                    mailObject.Body = deducedEmailText.ToString();


                    smtpServer.Send(mailObject);
                    mailObject.To.Clear();


                    //5 seconds delay before a new email is sent 
                    Thread.Sleep(5000);


                    //the progressbar control will be "informed" after each sent email in order to change its state accordingly.
                    (sender as BackgroundWorker).ReportProgress((i+1) * 100 / numberOfEmailAddresses);    
                }
            }
            catch (Exception ex)
            {
                if(ex is SmtpException)
                {
                    MessageBox.Show("Please check again your email address, credentials and SMTP data");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
                    
                e.Cancel = true;
            }
        }


        private void stopSendingEmailsButton_Click(object sender, RoutedEventArgs e)
        {
            //if there is no ongoing mail session, just ignore the click
            if (startSendingEmailsButton.IsEnabled)
                return;
            else
            {
                //disable Stop button. It will be enabled again after the background thread finishies, which will also lead to the progress bar being reset and the start button being enabled again.
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => this.stopSendingEmailsButton.IsEnabled = false));


                worker.CancelAsync();
            }
        }


        private void fromTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckInputData.CheckSenderEmailAddress(sender as TextBox);
        }


        private void SMTPPortTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckInputData.CheckSMTPPort(sender as TextBox);
        }
    }
}
