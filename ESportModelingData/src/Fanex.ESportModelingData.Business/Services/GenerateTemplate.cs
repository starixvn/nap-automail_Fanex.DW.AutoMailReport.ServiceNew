using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface GenerateTemplate
    {
        string Subject(Sport sport, Period period);

        string CustInfoPath(Sport sport, Period period);

        string ParentFolderPath(Sport sport, Period period);

        string ParentFolder();
    }
}