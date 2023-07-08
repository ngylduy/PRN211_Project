using BusinessObject;
using DataAccess.DAO;

namespace DataAccess.Repository.MemberRepo
{
    public class MemberRepository : IMemberRepository
    {
        public void AddMember(Member member) => MemberDAO.Instance.AddMember(member);

        public void DeleteMember(int memberId) => MemberDAO.Instance.Delete(memberId);

        public IEnumerable<Member> GetMembersList()
        {
            return MemberDAO.Instance.GetMembersList();
        }

        public IEnumerable<Member> SearchMember(string name)
        {
            return MemberDAO.Instance.SearchMember(name);
        }

        public Member Login(string username, string password)
        {
            return MemberDAO.Instance.Login(username, password);
        }

        public void UpdateMember(Member member) => MemberDAO.Instance.Update(member);

        public IEnumerable<Member> SearchMemberByCountry(string country, IEnumerable<Member> searchList)
        {
            return MemberDAO.Instance.FilterMemberByCountry(country, searchList);
        }

        public IEnumerable<Member> SearchMemberByCity(string country, string city, IEnumerable<Member> searchList)
        {
            return MemberDAO.Instance.FilterMemberByCity(country, city, searchList);
        }

        public Member GetMember(int memberId)
        {
            return MemberDAO.Instance.GetMember(memberId);
        }

        public int GetNextMemberId()
        {
            return MemberDAO.Instance.GetNextMemberId();
        }

        public Member GetMember(string memberEmail) => MemberDAO.Instance.GetMember(memberEmail);

        public void UpdateMemberStatus(int id, int status) => MemberDAO.Instance.UpdateStatus(id, status);
    }
}
