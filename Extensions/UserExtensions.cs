using Mtd.OrderMaker.Server.Areas.Identity.Data;

namespace Mtd.OrderMaker.Server.Extensions
{
    public static class UserExtensions
    {
        public static string GetFullName(this WebAppUser user)
        {
            string name = user.Title ?? "no name";
            string group = user.TitleGroup ?? "";
            if (user.TitleGroup != null && user.TitleGroup.Length>1) { group = $"({group})"; }

            return $"{name} {group}";
        }
    }
}
