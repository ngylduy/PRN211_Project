using BusinessObject;
using Microsoft.Extensions.Configuration;

namespace DataAccess.DAO
{
    public class MemberDAO
    {
        // Using Singleton Pattern
        private static MemberDAO instance = null;
        private static object instanceLook = new object();

        public static MemberDAO Instance
        {
            get
            {
                lock (instanceLook)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        // Get default user from appsettings
        private Member GetDefaultMember()
        {
            Member Default = null;
            using (StreamReader r = new StreamReader("appsettings.json"))
            {
                string json = r.ReadToEnd();
                IConfiguration config = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", true, true)
                                        .Build();
                string email = config["account:defaultAccount:email"];
                string password = config["account:defaultAccount:password"];
                string username = config["account:defaultAccount:username"];
                Default = new Member
                {
                    MemberId = 0,
                    Email = email,
                    Phone = "",
                    Username = username,
                    Password = password,
                    Address = "",
                    City = "",
                    Country = "",
                    Fullname = "Admin",
                    Role = 0,
                    Status = 1
                };
            }
            return Default;
        }

        public IEnumerable<Member> GetMembersList()
        {
            IEnumerable<Member> members = null;

            try
            {
                var context = new FStoreContext();
                // Get From Database
                members = context.Members;
                // Add Default User
                members = members.Append(GetDefaultMember()).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return members;
        }

        public Member Login(string username, string password)
        {
            Member member;
            try
            {
                IEnumerable<Member> members = GetMembersList();
                member = members.SingleOrDefault(mb => mb.Username.Equals(username) && mb.Password.Equals(password));
                if (member == null)
                {
                    throw new Exception("Login failed! Please check your email and password!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }

        public Member GetMember(int memberId)
        {
            Member member = null;

            try
            {
                var context = new FStoreContext();
                member = context.Members.SingleOrDefault(mem => mem.MemberId == memberId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return member;
        }
        public Member GetMember(string memberEmail)
        {
            Member member = null;

            try
            {
                var context = new FStoreContext();
                member = context.Members.SingleOrDefault(mem => mem.Email.Equals(memberEmail));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return member;
        }

        public Member GetMemberByUsername(string username)
        {
            Member member = null;

            try
            {
                var context = new FStoreContext();
                member = context.Members.SingleOrDefault(mem => mem.Username.Equals(username));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return member;
        }

        public int GetNextMemberId()
        {
            int nextMemberId = -1;

            try
            {
                var context = new FStoreContext();
                nextMemberId = context.Members.Max(mem => mem.MemberId) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return nextMemberId;
        }
        public void AddMember(Member member)
        {
            if (member == null)
            {
                throw new Exception("Member is undefined!!");
            }
            try
            {
                if (GetMember(member.MemberId) == null && GetMember(member.Email) == null && GetMemberByUsername(member.Username) == null)
                {
                    var context = new FStoreContext();
                    member.Role = 1;
                    member.Status = 1;
                    context.Members.Add(member);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Member is existed!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Member member)
        {
            if (member == null)
            {
                throw new Exception("Member is undefined!!");
            }
            try
            {
                Member _mem = GetMember(member.MemberId);
                if (_mem != null)
                {
                    var context = new FStoreContext();
                    member.Role = 1;
                    member.Status = 1;
                    context.Members.Update(member);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Member does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateStatus(int id, int status)
        {
            try
            {
                Member _mem = GetMember(id);
                if (_mem != null)
                {
                    var context = new FStoreContext();
                    _mem.Status = status;
                    context.Members.Update(_mem);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Member does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int MemberId)
        {
            try
            {
                Member member = GetMember(MemberId);
                if (member != null)
                {
                    var context = new FStoreContext();
                    context.Members.Remove(member);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Member does not exist!!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Member> SearchMember(string name)
        {
            IEnumerable<Member> searchResult = null;

            try
            {
                var context = new FStoreContext();
                searchResult = context.Members.Where(mem => mem.Fullname.ToLower().Contains(name.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return searchResult;
        }

        public IEnumerable<Member> FilterMemberByCountry(string country, IEnumerable<Member> searchList)
        {
            IEnumerable<Member> searchResult = null;
            if (searchList != null)
            {
                var memberSearch = from member in searchList
                                   where member.Country == country
                                   select member;
                if (country.Equals("All"))
                {
                    memberSearch = from member in searchList
                                   select member;
                }
                searchResult = memberSearch.ToList();
            }
            else
            {
                var context = new FStoreContext();
            }

            return searchResult;
        }

        public IEnumerable<Member> FilterMemberByCity(string country, string city, IEnumerable<Member> searchList)
        {
            IEnumerable<Member>? searchResult = null;
            var memberSearch = from member in searchList
                               where member.City == city
                               select member;
            if (city.Equals("All"))
            {
                memberSearch = from member in searchList
                               where member.Country == country
                               select member;
                if (country.Equals("All"))
                {
                    memberSearch = from member in searchList
                                   select member;
                }
            }
            searchResult = memberSearch.ToList();
            return searchResult;
        }
    }
}
