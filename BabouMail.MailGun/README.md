![alt text](https://raw.githubusercontent.com/ajtatum/BabouMail/master/assets/Babou-100x100.png "Babou loves mail!") <!-- markdownlint-disable -->

# BabouMail - MailGun  

Send email via the MailGun REST API.

## Packages

View the NuGet at https://www.nuget.org/packages/BabouMail.MailGun/

* **Package Manager:** Install-Package BabouMail.MailGun
* **.NET CLI:** dotnet add package BabouMail.MailGun

## Usage

Create a new email and sends it to the MailGun API.

    var babouEmail = new BabouEmail()
        .From(emalAddress, displayName)
        .To(emalAddress, displayName)
        .Subject(subject)
        .Body(htmlMessage, true);

    babouEmail.Sender = new MailgunSender(domain, apiKey);

    var response = await babouEmail.SendAsync();

Or user Dependency Injection:

In Startup.cs:

    services
        .AddBabouEmail(defaultFromAddress)
        .AddMailGunSender(domain, apiKey);

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