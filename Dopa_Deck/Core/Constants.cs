public static class Constants
{
    public const int maxDamage = 1000000;

    public static class Channel
    {
        public const string JsonChannel = "Resources/Datas/";
	}

    public static class MonsterName
    {
        public const string Mushroom = "Mushroom";
		public const string Radish   = "Radish";
		public const string Cat      = "Cat";
		public const string Golem    = "Golem";
	}

	public static class Tag 
    {
        public const string Player  = "Player";
        public const string Monster = "Monster";
    }

    public static class MyDeck
    {
		public const int MaxDeckSize = 3;

        public const int FirstCard   = 0;
		public const int SecondCard  = 1;
		public const int ThirdCard   = 2;
	}

	public static class BlendTreeThreshold
    {
        public static class PlayerMove
        {
            public const float RunLeft  = 0;
            public const float Run      = 0.5f;
            public const float RunRight = 1.0f;
        }
    }

    public static class MovementDirection  
    {
        public const float Forward  =  0.01f;
        public const float Backward = -0.01f;
        public const float Left     = -0.01f;
        public const float Middle   =  0;
        public const float Right    =  0.01f;
    }
}