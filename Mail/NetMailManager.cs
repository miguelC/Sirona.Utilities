using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;

namespace Sirona.Utilities.Mail
{
  /// <summary>
  /// A class that makes it easier to send emails
  /// </summary>
  public class NetMailManager
  {
    private string fromAddress;
    private string sender;
    private string replyTo;
    private string smtpHostName;
    private int smtpPort = 25;
    private string username;
    private string password;
    private bool authenticate = false;
    private Dictionary<string, NetMailTemplate> templates;
    private string defaultTemplateName;

    /// <summary>
    /// Default template
    /// </summary>
    public string DefaultTemplateName
    {
      get { return defaultTemplateName; }
      set { defaultTemplateName = value; }
    }

    /// <summary>
    /// For templating mail
    /// </summary>
    public Dictionary<string, NetMailTemplate> Templates
    {
      get { return templates; }
      set { templates = value; }
    }
    /// <summary>
    /// Whether or not authentication is required by the server to send the mail
    /// </summary>
    public bool Authenticate
    {
      get { return authenticate; }
      set { authenticate = value; }
    }
    /// <summary>
    /// Default from address
    /// </summary>
    public string FromAddress
    {
      get 
      {
        if (string.IsNullOrEmpty(fromAddress))
        {
          return this.Sender;
        }
        return fromAddress; 
      }
      set { fromAddress = value; }
    }
    /// <summary>
    /// Default sender address
    /// </summary>
    public string Sender
    {
      get { return sender; }
      set { sender = value; }
    }
    /// <summary>
    /// Default reply to address
    /// </summary>
    public string ReplyTo
    {
      get
      {
        if (string.IsNullOrEmpty(fromAddress))
        {
          return this.Sender;
        } 
        return replyTo;
      }
      set { replyTo = value; }
    }
    /// <summary>
    /// Default to address(es)
    /// </summary>
    public List<string> ToAddresses { get; set; }

    /// <summary>
    /// URL of the smtp server
    /// </summary>
    public string SmtpHostName
    {
      get { return smtpHostName; }
      set { smtpHostName = value; }
    }
    /// <summary>
    /// Username if authentication is required
    /// </summary>
    public string Username
    {
      get { return username; }
      set { username = value; }
    }
    /// <summary>
    /// The port of the SMTP server
    /// </summary>
    public int SmtpPort
    {
      get { return smtpPort; }
      set { smtpPort = value; }
    }
    /// <summary>
    /// Password if authentication is required
    /// </summary>
    public string Password
    {
      get { return password; }
      set { password = value; }
    }
    /// <summary>
    /// Adds a template to the templates list of body style and perser type equals text
    /// </summary>
    /// <param name="name"></param>
    /// <param name="subject"></param>
    /// <param name="bodyFile"></param>
    public void AddTemplate(string name, string subject, string bodyFile)
    {
      this.AddTemplate(name, subject, bodyFile, NetMailTemplateTextStyle.Text, NetMailTemplateParserType.Text);
    }
    /// <summary>
    /// Adds a template to the templates list
    /// </summary>
    /// <param name="name"></param>
    /// <param name="subject"></param>
    /// <param name="bodyFile"></param>
    /// <param name="textStyle"></param>
    /// <param name="parserType"></param>
    public void AddTemplate(string name, string subject, string bodyFile, NetMailTemplateTextStyle textStyle, NetMailTemplateParserType parserType)
    {
      if (this.Templates == null) this.Templates = new Dictionary<string, NetMailTemplate>();
      NetMailTemplate template = new NetMailTemplate();
      template.MessageBodyFile = bodyFile;
      template.MessageSubject = subject;
      template.BodyStyle = textStyle;
      template.BodyParserType = parserType;
      this.templates.Add(name, template);
    }
    /// <summary>
    /// Sends a text message using all the default settings including toAddresses
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    public void SendTextMessage(string subject, string message)
    {
      SendMessage(subject, message, false, MailPriority.Normal, null, null, null, null, null, null);
    }
    /// <summary>
    /// Send a text message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="toAddresses"></param>
    /// <param name="ccAddresses"></param>
    /// <param name="bccAddresses"></param>
    public void SendTextMessage(string subject, string message,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses)
    {
      SendMessage(subject, message, false, MailPriority.Normal, null, null, null, toAddresses, ccAddresses, bccAddresses);
    }
    /// <summary>
    /// Send an HTML message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    public void SendHtmlMessage(string subject, string message)
    {
      SendMessage(subject, message, true, MailPriority.Normal, null, null, null, null, null, null);
    }
    /// <summary>
    /// Send an HTML message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="toAddresses"></param>
    /// <param name="ccAddresses"></param>
    /// <param name="bccAddresses"></param>
    public void SendHtmlMessage(string subject, string message,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses)
    {
      SendMessage(subject, message, true, MailPriority.Normal, null, null, null, toAddresses, ccAddresses, bccAddresses);
    }
    /// <summary>
    /// Sends an email message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="htmlFormat"></param>
    /// <param name="priority"></param>
    /// <param name="from"></param>
    /// <param name="sender"></param>
    /// <param name="replyTo"></param>
    /// <param name="toAddresses"></param>
    /// <param name="bccAddresses"></param>
    /// <param name="ccAddresses"></param>
    public void SendMessage(  string subject, 
                              string message, 
                              bool htmlFormat, 
                              MailPriority priority, 
                              string from, 
                              string sender, 
                              string replyTo,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses)
    {
      MailMessage mail = BuildMessage(subject, message, htmlFormat, priority, from, sender, replyTo, toAddresses, ccAddresses, bccAddresses);
      this.SendMessage(mail);
    }
    /// <summary>
    /// This method is used in conjuntion with the BuildMessage method to send messages that contain attachments
    /// </summary>
    /// <param name="mail"></param>
    public void SendMessage(MailMessage mail)
    {
        using (SmtpClient client = new SmtpClient())
        {
            client.Host = this.SmtpHostName;
            if (this.SmtpPort > 0)
            {
                client.Port = this.SmtpPort;
            }
            client.Send(mail);
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="priority"></param>
    /// <param name="from"></param>
    /// <param name="sender"></param>
    /// <param name="replyTo"></param>
    /// <param name="toAddresses"></param>
    /// <param name="ccAddresses"></param>
    /// <param name="bccAddresses"></param>
    /// <param name="dataObjects"></param>
    public void SendTemplatedMessage(
                              MailPriority priority,
                              string from,
                              string sender,
                              string replyTo,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses,
                              Dictionary<string, object> dataObjects)
    {
      this.SendTemplatedMessage(priority, from, sender, replyTo, toAddresses, ccAddresses, bccAddresses, this.DefaultTemplateName, dataObjects);
    }
    /// <summary>
    /// Sends a templated message
    /// </summary>
    /// <param name="templateName"></param>
    /// <param name="priority"></param>
    /// <param name="from"></param>
    /// <param name="sender"></param>
    /// <param name="replyTo"></param>
    /// <param name="toAddresses"></param>
    /// <param name="ccAddresses"></param>
    /// <param name="bccAddresses"></param>
    /// <param name="dataObjects"></param>
    public void SendTemplatedMessage(
                              MailPriority priority,
                              string from,
                              string sender,
                              string replyTo,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses,
                              string templateName,
                              Dictionary<string, object> dataObjects) 
    {
        MailMessage mail = this.BuildTemplatedMessage(priority, from, sender, replyTo, toAddresses, ccAddresses, bccAddresses, templateName, dataObjects);
        this.SendMessage(mail);
    }
    public MailMessage BuildTemplatedMessage(
                              MailPriority priority,
                              string from,
                              string sender,
                              string replyTo,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses,
                              Dictionary<string, object> dataObjects)
    {
        return this.BuildTemplatedMessage(priority, from, sender, replyTo, toAddresses, ccAddresses, bccAddresses, this.DefaultTemplateName, dataObjects);
    }
    public MailMessage BuildTemplatedMessage(
                              MailPriority priority,
                              string from,
                              string sender,
                              string replyTo,
                              List<string> toAddresses,
                              List<string> ccAddresses,
                              List<string> bccAddresses,
                              string templateName,
                              Dictionary<string, object> dataObjects)
    {
        NetMailTemplate template = this.Templates[templateName];
        if (template == null)
        {
            throw new NetMailException("A template named '" + templateName + "' can not be found. Email message failed to be constructed");
        }
        string subject = template.GenerateSubject(dataObjects);
        string body = template.GenerateBody(dataObjects);
        bool htmlFormat = template.BodyStyle == NetMailTemplateTextStyle.HTML;
        return this.BuildMessage(subject,
          body,
          htmlFormat,
          priority,
          from,
          sender,
          replyTo,
          toAddresses,
          ccAddresses,
          bccAddresses);
    }
    /// <summary>
    /// Builds a mail message using default settings
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public MailMessage BuildMessage(string subject, string body)
    {
      return BuildMessage(subject, body, false, MailPriority.Normal, null, null, null, null, null, null);
    }
    /// <summary>
    /// Builds a mail message
    /// </summary>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="htmlFormat"></param>
    /// <param name="priority"></param>
    /// <param name="from"></param>
    /// <param name="sender"></param>
    /// <param name="replyTo"></param>
    /// <param name="toAddresses"></param>
    /// <param name="ccAddresses"></param>
    /// <param name="bccAddresses"></param>
    /// <returns></returns>
    public MailMessage BuildMessage(  string subject, 
                                      string body, 
                                      bool htmlFormat, 
                                      MailPriority priority, 
                                      string from, 
                                      string sender, 
                                      string replyTo, 
                                      List<string> toAddresses, 
                                      List<string> ccAddresses, 
                                      List<string> bccAddresses)
    {
      MailMessage mail = new MailMessage();
      if (toAddresses != null && toAddresses.Count > 0)
      {
        foreach (string addy in toAddresses)
        {
          mail.To.Add(new MailAddress(addy.Trim()));
        }
      }
      else
      {
        if (this.ToAddresses == null || this.ToAddresses.Count <= 0)
        {
          throw new NetMailException("Recipient addresses are not defined in the configuration");
        }
        foreach (string addy in this.ToAddresses)
        {
            mail.To.Add(new MailAddress(addy.Trim()));
        }
      }
      if (ccAddresses != null && ccAddresses.Count > 0)
      {
        foreach (string addy in ccAddresses)
        {
          mail.To.Add(new MailAddress(addy));
        }
      }
      if (bccAddresses != null && bccAddresses.Count > 0)
      {
        foreach (string addy in bccAddresses)
        {
          mail.To.Add(new MailAddress(addy));
        }
      }
      if (string.IsNullOrEmpty(from))
      {
        if (string.IsNullOrEmpty(this.FromAddress))
        {
          throw new NetMailException("Sender address is not defined in the configuration");
        }
        mail.From = new MailAddress(this.FromAddress);
      }
      else
      {
        mail.From = new MailAddress(from);
      }
      if (string.IsNullOrEmpty(sender))
      {
        if (!string.IsNullOrEmpty(this.Sender))
        {
          mail.Sender = new MailAddress(this.Sender);
        }
      }
      else
      {
        mail.Sender = new MailAddress(sender);
      }
      if (string.IsNullOrEmpty(replyTo))
      {
        if (!string.IsNullOrEmpty(this.ReplyTo))
        {
          mail.ReplyToList.Add(new MailAddress(this.ReplyTo));
        }
      }
      else
      {
        mail.ReplyToList.Add( new MailAddress(replyTo));
      }
      mail.IsBodyHtml = htmlFormat;
      mail.Priority = priority;
      mail.Subject = subject;
      mail.Body = body;
      return mail;
    }
    /// <summary>
    /// Adds an attachment
    /// </summary>
    /// <param name="message"></param>
    /// <param name="attachmentName"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public MailMessage AddAttachment(MailMessage message,
                                      string attachmentName,
                                      Stream content)
    {
      return AddAttachment(message, attachmentName, content, null);
    }
    /// <summary>
    /// Adds an attachment
    /// </summary>
    /// <param name="message"></param>
    /// <param name="attachmentName"></param>
    /// <param name="content"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public MailMessage AddAttachment( MailMessage message, 
                                      string attachmentName,
                                      Stream content, 
                                      ContentType contentType)
    {
      Attachment att = new Attachment(content, attachmentName);
      if (contentType != null)
      {
        att.ContentType = contentType;
      }
      message.Attachments.Add(att);
      return message;
    }
    /// <summary>
    /// Attaches a file
    /// </summary>
    /// <param name="message"></param>
    /// <param name="location"></param>
    /// <returns></returns>
    public MailMessage AttachFile(MailMessage message,  string location)
    {
      Attachment att = new Attachment(location);
      message.Attachments.Add(att);
      return message;
    }
  }
}
