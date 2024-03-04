namespace NET_LAb_1
{
    public static class Config
    {
        public const string keyPath = "\\\\wsl.localhost\\Ubuntu\\home\\radzone\\key.json";
        public const string UserEndpoint = "grpcs://ydb.serverless.yandexcloud.net:2135";
        public const string UserDatabasePath = "/ru-central1/b1g7j764i7vamqpln0vb/etnnu7nm5s0f2cu35q95";

        public static bool IsDebugMode = true;
        public static bool IsWriteKey = false;
        public static bool IsWriteResponse = true;
    }
}
