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
        void Active(int memberId);
        void Create(RegisterDto dto);
        MemberDto Get(int memberId);
        MemberDto Get(string account);
        bool IsAccountExist(string account);
        void Update(MemberDto memberInDb);
    }
}
