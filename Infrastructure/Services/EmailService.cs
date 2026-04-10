using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task Send(string to, string subject, string body)
        {
            var message = BuildBaseMessage(to, subject);

            message.Body = new TextPart("plain")
            {
                Text = body ?? string.Empty
            };

            await SendAsync(message, to);
        }

        
        public async Task SendAccountStatement(CustomerDto customer, AccountStatementDto statement)
        {
            var subject = $"Account Statement - {statement?.Month ?? "Unknown"}";
            var message = BuildBaseMessage(customer?.Email, subject);

            var htmlBody = BuildAccountStatementHtml(customer, statement);
            var textBody = BuildAccountStatementPlain(customer, statement);

            _logger.LogDebug("SendAccountStatement HTML body: {HtmlBody}", htmlBody);
            _logger.LogDebug("SendAccountStatement Text body: {TextBody}", textBody);
            _logger.LogDebug("Customer DTO: {@Customer}", customer);
            _logger.LogDebug("Statement DTO: {@Statement}", statement);

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendAsync(message, customer?.Email ?? throw new ArgumentNullException(nameof(customer)));
        }

      
        private async Task SendAsync(MimeMessage message, string to)
        {
            var host = _config["Smtp:Host"] ?? throw new InvalidOperationException("SMTP host not configured");
            var portText = _config["Smtp:Port"] ?? "587";
            if (!int.TryParse(portText, out var port)) port = 587;
            var user = _config["Smtp:User"] ?? throw new InvalidOperationException("SMTP user not configured");
            var pass = _config["Smtp:Pass"] ?? throw new InvalidOperationException("SMTP pass not configured");

            using var client = new SmtpClient();

            try
            {
                var socketOptions = port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTlsWhenAvailable;

                _logger.LogInformation("Connecting to SMTP {Host}:{Port}", host, port);
                await client.ConnectAsync(host, port, socketOptions);

                if (client.AuthenticationMechanisms.Contains("XOAUTH2"))
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(user, pass);

                _logger.LogInformation("Sending email to {To}", to);
                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                _logger.LogInformation("Email successfully sent to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP send failed for {To}", to);
                throw;
            }
        }

        private MimeMessage BuildBaseMessage(string to, string subject)
        {
            var user = _config["Smtp:User"] ?? throw new InvalidOperationException("SMTP user not configured");

            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(user));
            message.To.Add(MailboxAddress.Parse(to ?? throw new ArgumentNullException(nameof(to))));
            message.Subject = subject ?? string.Empty;

            return message;
        }

        private string BuildAccountStatementHtml(CustomerDto customer, AccountStatementDto statement)
        {
            var name = customer?.Name ?? string.Empty;
            var id = customer?.Id.ToString() ?? string.Empty;
            var email = customer?.Email ?? string.Empty;
            var phone = customer?.Phone ?? string.Empty;
            var address = customer?.Address ?? string.Empty;

            var stmtId = statement?.Id ?? string.Empty;
            var month = statement?.Month ?? string.Empty;
            var balance = statement != null ? $"{statement.Balance:C}" : string.Empty;

            return $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial;
            background-color: #f7f7f7;
        }}
        .container {{
            background: white;
            padding: 20px;
            border-radius: 10px;
        }}
        h2 {{
            color: #2c3e50;
        }}
        .box {{
            margin-bottom: 15px;
        }}
        .label {{
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Account Statement</h2>
        <hr/>

        <div class='box'>
            <p><span class='label'>Customer Name:</span> {name}</p>
            <p><span class='label'>Customer ID:</span> {id}</p>
            <p><span class='label'>Email:</span> {email}</p>
            <p><span class='label'>Phone:</span> {phone}</p>
            <p><span class='label'>Address:</span> {address}</p>
        </div>

        <hr/>

        <div class='box'>
            <p><span class='label'>Statement ID:</span> {stmtId}</p>
            <p><span class='label'>Month:</span> {month}</p>
            <p><span class='label'>Balance:</span> {balance}</p>
        </div>

        <hr/>

        <p style='font-size:12px;color:gray'>
            This is an automated account statement. Please do not reply.
        </p>
    </div>
</body>
</html>";
        }

     
        private string BuildAccountStatementPlain(CustomerDto customer, AccountStatementDto statement)
        {
            return $@"
Account Statement

Customer Name: {customer?.Name}
Customer ID: {customer?.Id}
Email: {customer?.Email}
Phone: {customer?.Phone}
Address: {customer?.Address}

Statement ID: {statement?.Id}
Month: {statement?.Month}
Balance: {(statement != null ? $"{statement.Balance:C}" : string.Empty)}

This is an automated account statement. Please do not reply.
";
        }
    }
}