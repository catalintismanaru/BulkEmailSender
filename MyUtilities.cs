using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Bulk_Email_Sender
{
    static class MyUtilities
    {
        internal static string[] FullEmailsFilesPathsArray { get; set; }

        internal static string[] FullAttachamentsFilesPathsArray { get; set; }

        ///<summary>
        ///This static method replaces the first found substring (2nd parameter) of the StringBuilder source with a given string (3rd parameter).
        ///</summary>
        internal static void ReplaceFirstOccurrence(ref StringBuilder Source, string Find, string Replace)
        {
            try
            {
                int Place = Source.ToString().IndexOf(Find);
                if (Place == -1)
                    return;


                Source.Remove(Place, Find.Length).Insert(Place, Replace);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ///<summary>
        ///This static method reads a .csv file and stores the information into allCsvRows in a meaningful order.
        ///</summary>
        internal static void ParseCSVFile(ref List<List<string>> allCsvRows, string absoluteFilePath)
        {
            try
            {
                using (var reader = new StreamReader(absoluteFilePath))
                {
                    String line;
                    String[] values;


                    if (allCsvRows == null)
                    {
                        line = reader.ReadLine();
                        values = line.Split(',');


                        allCsvRows = new List<List<string>>(values.Length);
                        for (int i = 0; i < values.Length; i++)
                        {
                            allCsvRows.Add(new List<string>());
                            allCsvRows[i].Add(values[i]);
                        }
                    }
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        values = line.Split(',');


                        for (int i = 0; i < values.Length; i++)
                        {
                            allCsvRows[i].Add(values[i]);
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        ///<summary>
        ///This static method is called when it's necessary to change the input subject, so that each email message can have a customised one.
        ///</summary>
        internal static void DeduceEmailSubject(string inputedSubject, List<List<string>> allCsvRows, int recipientIndexinallCvsRows, ref StringBuilder deducedEmailSubject)
        {
            try
            {
                deducedEmailSubject.Clear();
                deducedEmailSubject.Append(inputedSubject);


                int size = allCsvRows.Count;
                for (int i = 0; i < size; i++)
                {
                    ReplaceFirstOccurrence(ref deducedEmailSubject, "{" + (i + 1) + "}", allCsvRows[i][recipientIndexinallCvsRows]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ///<summary>
        ///This static method is called when it's necessary to change the input body text , so that each email message can have a customised one.
        ///</summary>
        internal static void DeduceEmailText(string inputedMessageBody, List<List<string>> allCsvRows, int recipientIndexinallCvsRows, ref StringBuilder deducedEmailText)
        {
            try
            {
                deducedEmailText.Clear();
                deducedEmailText.Append(inputedMessageBody);


                int size = allCsvRows.Count;
                for (int i = 0; i < size; i++)
                {
                    ReplaceFirstOccurrence(ref deducedEmailText, "{" + (i + 1) + "}", allCsvRows[i][recipientIndexinallCvsRows]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
