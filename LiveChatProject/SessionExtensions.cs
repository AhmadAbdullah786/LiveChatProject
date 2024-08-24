namespace LiveChatProject
{
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetString(key, value ? "true" : "false");
        }

        public static bool GetBool(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null && value == "true";
        }
    }
}
