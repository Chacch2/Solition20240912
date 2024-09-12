using BookStore.FrontEnd.Site.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.FrontEnd.Site.Models.interfaces
{
    public interface IMemberRepository
    {
        void Create(RegisterDto dto);
        bool IsAccountExist(string account);
    }
}
