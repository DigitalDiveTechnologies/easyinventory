using System.Text.Json;
using System.Text.Json.Serialization;
using EasyInventory.PgData.Entities;

namespace ShopOn.Web.Infrastructure;

public static class SessionExtensions
{
    private const string CurrentUserKey = "CurrentUser";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public static void SetCurrentUser(this ISession session, Employee? user)
    {
        if (user == null)
        {
            session.Remove(CurrentUserKey);
            return;
        }

        session.SetString(CurrentUserKey, JsonSerializer.Serialize(user, JsonOptions));
    }

    public static Employee? GetCurrentUser(this ISession session)
    {
        var json = session.GetString(CurrentUserKey);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<Employee>(json, JsonOptions);
    }
}
