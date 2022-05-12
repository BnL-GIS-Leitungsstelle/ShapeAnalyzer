using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Reflection;

namespace ShapeAnalyzer.Utils
{
    public static class MailUtils
    {
        private const string _tmpMailAddress = "tmp.tmp@tmp.com";

        public static void CreateAndOpenMailMessage(string to, string subject, string attachmentPath)
        {
            var filename = Path.GetTempFileName().Replace(".tmp", ".eml");
            var message = new MailMessage
            {
                // if From isn't set Message can't be saved
                From = new MailAddress(_tmpMailAddress),
                Subject = subject,
                Attachments = { new Attachment(attachmentPath) }
            };
            message.To.Add(to);

            using (var filestream = File.Open(filename, FileMode.Create))
            {
                var binaryWriter = new BinaryWriter(filestream);
                //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));

                var assembly = typeof(SmtpClient).Assembly;
                var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
            }

            RemoveTmpFrom(filename);

            Process.Start(filename);
        }

        private static void RemoveTmpFrom(string filename)
        {
            string content = File.ReadAllText(filename);
            content = content.Replace(_tmpMailAddress, string.Empty);
            File.WriteAllText(filename, content);
        }
    }
}
