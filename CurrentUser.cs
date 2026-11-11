namespace CircusApp
{
    public static class CurrentUser
    {
        public static int ID_Пользователя { get; set; }
        public static string Логин { get; set; }
        public static int ID_Роли { get; set; }
        public static int? ID_Работника { get; set; }
        public static int? ID_Клиента { get; set; }

        public static void Clear()
        {
            ID_Пользователя = 0;
            Логин = null;
            ID_Роли = 0;
            ID_Работника = null;
            ID_Клиента = null;
        }
    }
}