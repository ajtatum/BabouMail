![alt text](https://raw.githubusercontent.com/ajtatum/BabouMail/master/assets/Babou-100x100.png "Babou loves mail!") <!-- markdownlint-disable -->

# BabouMail - SMTP  

Send email via good old fashion SMTP.

## Packages

View the NuGet at https://www.nuget.org/packages/BabouMail.Smtp/

* **Package Manager:** Install-Package BabouMail.Smtp
* **.NET CLI:** dotnet add package BabouMail.Smtp

## Usage

Create a new email and sends it to the MailGun API.

    var babouEmail = new BabouEmail()
        .From(emalAddress, displayName)
        .To(emalAddress, displayName)
        .Subject(subject)
        .Body(htmlMessage, true);

    babouEmail.Sender = new SmtpSender(new SmtpClient(host, 25));

    var response = await babouEmail.SendAsync();

Or user Dependency Injection:

In Startup.cs:

    services
        .AddBabouEmail(defaultFromAddress)
        .AddSmtpSender(host, port, username, password);

In a Controller or Service:

        private readonly IBabouEmail _babouEmail;

        public EmailService(IBabouEmail babouEmail)
        {
            _babouEmail = babouEmail;
        }

        private async Task SendMailAsync(string email, string subject, string htmlMessage)
        {
            var response = await _babouEmail
                .From(emailAddress, displayName)
                .To(emalAddress, displayName)
                .Subject(subject)
                .Body(htmlMessage, true)
                .SendAsync();
        }