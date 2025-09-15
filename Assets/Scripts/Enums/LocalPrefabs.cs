namespace Content.Local.Prefabs
{
    public static class Effects
    {
        public enum Particles
        {
            CashFlow,
            DiamondsRain,
            Victory
        }
    }

    public static class UI
    {
        public enum Pages
        {
            MainUICanvas,
            MetaUI,
            MinigameLoadingCurtain
        }
        public enum Elements
        {
            CharacterCardMini,
            SelectableButton
        }
    }

    public static class CharacterCards
    {
        public static class Models3D
        {
            public enum S256
            {
                Cat256,
                Squid256
            }
        }
        public static class UI
        {
            public enum S64
            {
                Cat64,
                Squid64
            }

            public enum S128
            {
                Cat128,
                Squid128
            }

            public enum S256
            {
                Cat256,
                Squid256
            }
        }
    }
    public static class MinigameResources
    {
        public static class UI
        {
            public enum S64
            {
                Cash64,
                Diamond64
            }

            public enum S128
            {
                Cash128,
                Diamond128
            }

            public enum S256
            {
                Cash256,
                Diamond256
            }
        }
    }
}

