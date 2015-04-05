using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.ComponentModel.DataAnnotations;

namespace Bulk_Email_Sender
{
    /// <summary>
    /// This class contains methods that can perform basic input data validations.
    /// </summary>
    static class CheckInputData
    {
        internal static bool CheckSubject(TextBox subjectTextBox)
        {
            if (String.IsNullOrEmpty(subjectTextBox.Text) || subjectTextBox.Text.Equals("Insert the subject"))
            {
                if (MessageBox.Show("Do you want to send an email message with an empty subject?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    subjectTextBox.Focus();
                    subjectTextBox.Text = "Insert the subject";
                    subjectTextBox.SelectAll();
                    return false;
                }
            }
                return true;
        }
        internal static bool CheckSenderEmailAddress(TextBox fromTextBox)
        {
            if (String.IsNullOrEmpty(fromTextBox.Text) || fromTextBox.Text.Equals("Insert email address") || fromTextBox.Text.Equals("Invalid email address format"))
            {
                MessageBox.Show("You must insert your email address.");

                fromTextBox.Text = "Insert email address";
                fromTextBox.SelectAll();
                return false;
            }
            else
            {
                var foo = new EmailAddressAttribute();
                if (!foo.IsValid(fromTextBox.Text))
                {
                    MessageBox.Show("Invalid email address format");
                    fromTextBox.Text = " - Insert a valid email address";
                    fromTextBox.SelectAll();
                    return false;
                }
            }
            return true;
        }
        internal static bool CheckSenderUsername(TextBox userTextBox)
        {
            if (String.IsNullOrEmpty(userTextBox.Text) || userTextBox.Text.Equals("Insert your username"))
            {
                MessageBox.Show("You must insert your username.");
                {
                    userTextBox.Focus();
                    userTextBox.Text = "Inseret your username";
                    userTextBox.SelectAll();
                    return false;
                }
            }
            return true;
        }
        internal static bool CheckPassword(PasswordBox passwordBox)
        {
            if (String.IsNullOrEmpty(passwordBox.Password))
            {
               MessageBox.Show("You must insert your password.");
                {
                    passwordBox.Focus();
                    return false;
                }
            }
            return true;
        }
        internal static bool CheckSMTPServer(TextBox SMTPServerTextBox)
        {
            if (String.IsNullOrEmpty(SMTPServerTextBox.Text) || SMTPServerTextBox.Text.Equals("Insert SMTPServer"))
            {
                MessageBox.Show("You must insert the SMTPServer.");
                {
                    SMTPServerTextBox.Focus();
                    SMTPServerTextBox.Text = "Insert SMTPServer";
                    SMTPServerTextBox.SelectAll();
                    return false;
                }
            }
            return true;
        }
        internal static bool CheckSMTPPort(TextBox SMTPPortTextBox)
        {
            if (String.IsNullOrEmpty(SMTPPortTextBox.Text) || SMTPPortTextBox.Text.Equals("Insert Port"))
            {
                MessageBox.Show("You must insert the SMTP port.");
                SMTPPortTextBox.Text = "Insert Port";
                SMTPPortTextBox.SelectAll();
                return false;
            }
            else
            {
                try
                {
                    Convert.ToInt32(SMTPPortTextBox.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please insert a number in the SMTPort TextBox");
                    SMTPPortTextBox.SelectAll();
                    return false;
                }
                catch (OverflowException ex)
                {
                    MessageBox.Show(ex.Message);
                    SMTPPortTextBox.SelectAll();
                    return false;
                }
            }
            return true;
        }
        internal static bool CheckEmailsFiles(ListBox emailsFilesListBox)
        {
            if (emailsFilesListBox.Items.Count == 0)
            {
                if (MessageBox.Show("You must insert at least one file with recipients' email addresses. Insert now?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
                    return true;
                }
                else
                    return false;
            }
            return true;
        }
        internal static bool CheckBodyMessage(RichTextBox richTextBox)
        {
            string bodyMessage = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;

            if (String.IsNullOrEmpty(bodyMessage) || bodyMessage.Equals("Insert the message"))
            {
                if (MessageBox.Show("Do you want to send an email message with an empty body?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    richTextBox.Focus();
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.AppendText("Insert the message");
                    richTextBox.SelectAll();
                    return false;
                }
                else
                    richTextBox.Document.Blocks.Clear();
            }
            return true;
        }
    }
}
