namespace Fanex.MailLetstalkTemplate.Business.Services.FormatMailTemplate
{
    public interface MailFormat
    {
        string Subject();

        string FileName();

        string Header();
    }
}