using System;
using System.IO;
using System.Net.Mail;

public class MailUtil
{
    /// <summary>
    /// 一个同步SMTP发送邮件的接口
    /// </summary>
    /// <param name="senderAddress">邮件发送者地址，例如xxx@163.com</param>
    /// <param name="recvAddress">邮件接收者地址（可以支持多个，通过分号隔开），例如yyy@163.com</param>
    /// <param name="copyTo">抄送列表（可以支持多个，通过分号隔开）</param>
    /// <param name="subject">主题</param>
    /// <param name="body">内容</param>
    /// <param name="attachments">附件（可以支持多个，通过分号隔开）</param>
    /// <param name="username">用于SMTP服务器鉴权的用户名</param>
    /// <param name="password">用于SMTP服务器鉴权的密码</param>
    /// <param name="smtpServer">SMTP服务器，例如smtp.163.com</param>
    /// <returns></returns>
    public static bool SendEmail(string senderAddress, string recvAddress, string copyTo, string subject, string body, string attachments, string username, string password, string smtpServer)
    {
        var message = new MailMessage();
        message.Subject = subject;
        message.Body = body;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.From = new MailAddress(senderAddress);

        if (!string.IsNullOrEmpty(recvAddress))
        {
            string[] recv_list = recvAddress.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string recv in recv_list)
            {
                message.To.Add(recv);
            }
        }

        if (!string.IsNullOrEmpty(copyTo))
        {
            string[] cc_list = copyTo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cc in cc_list)
            {
                message.CC.Add(cc);
            }
        }

        if (!string.IsNullOrEmpty(attachments))
        {
            string[] attachment_list = attachments.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string attachPath in attachment_list)
            {
                if (File.Exists(attachPath))
                {
                    var attachment = new Attachment(attachPath, System.Net.Mime.MediaTypeNames.Application.Octet);
                    var disposition = attachment.ContentDisposition;
                    var info = new FileInfo(attachPath);
                    disposition.CreationDate = info.CreationTime;
                    disposition.ModificationDate = info.LastWriteTime;
                    disposition.ReadDate = info.LastAccessTime;
                    message.Attachments.Add(attachment);
                }
            }
        }
        bool result = false;
        try
        {
            SmtpClient client = new SmtpClient(smtpServer);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(username, password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(message);
            result = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
        return result;
    }
}
