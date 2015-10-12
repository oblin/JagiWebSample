using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jagi.Utility
{
    public class EmailSetting
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnEmail { get; set; }
        public string Title { get; set; }
    }

    public class Mailer
    {
        private EmailSetting _account;

        public Mailer()
        {
            var defaultSetting = ServiceLocator.Current.GetInstance<EmailSetting>();
            if (defaultSetting != null)
                this._account = defaultSetting;
            else
                throw new ArgumentNullException("無法找到預設的 Email Setting!");

            CompleteEmptyField();
        }

        public Mailer(EmailSetting account)
        {
            this._account = account;
            CompleteEmptyField();
        }

        /// <summary>
        /// 僅提供 GMail 的發送 EMAIL，須先設定 EmailSetting (使用 constructor)
        /// 若有需要知道是否完成發送，可以使用 callback & eventState
        /// </summary>
        /// <param name="emailAddress">需要發送 email 的 address</param>
        /// <param name="content">EMAIL 內容</param>
        /// <param name="subject">EMAIL 標題</param>
        /// <param name="callback">Call back，標準做法：new WaitCallback((o) => { // do something you want ((AutoResetEvent)o).Set(); }); </param>
        /// <param name="autoEvent">讓客戶端知道何時完成，標準做法： AutoResetEvent autoEvent = new AutoResetEvent(false);</param>
        public async Task SendGMail(string emailAddress, string content, string subject,
            WaitCallback callback = null, AutoResetEvent eventState = null)
        {
            MailMessage mail = new MailMessage();
            //using ()
            //{

            //}
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Credentials = new System.Net.NetworkCredential(_account.Email, _account.Password);
            smtpClient.EnableSsl = true;
            smtpClient.Port = 587;

            mail.From = new MailAddress(_account.ReturnEmail, _account.Title, System.Text.Encoding.UTF8);
            mail.To.Add(emailAddress);
            mail.Subject = subject;
            mail.Body = content;
            if (callback != null)
            {
                smtpClient.SendCompleted += (s, e) => {
                    object obj = null;
                    if (eventState != null)
                        obj = eventState;
                    callback(obj); 
                };
            }

            await smtpClient.SendMailAsync(mail);
        }

        private void CompleteEmptyField()
        {
            if (string.IsNullOrEmpty(_account.Email))
                throw new ArgumentException("Email 設定不可為空值");

            if (string.IsNullOrEmpty(this._account.ReturnEmail))
                this._account.ReturnEmail = this._account.Email;
            if (string.IsNullOrEmpty(this._account.Title))
                this._account.Title = this._account.Email;
        }
    }
}
