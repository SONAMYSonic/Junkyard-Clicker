namespace JunkyardClicker.Resource
{
    public static class DamageCalculator
    {
        private static readonly int[] s_toolDamageTable = new int[] { 1, 3, 8, 15, 30, 60, 120 };
        private static readonly int[] s_workerDpsTable = new int[] { 0, 1, 3, 8, 20, 50 };

        public static int CalculateClickDamage(int toolLevel)
        {
            int index = ClampIndex(toolLevel, s_toolDamageTable.Length);
            return s_toolDamageTable[index];
        }

        public static int CalculateAutoDamagePerSecond(int workerLevel)
        {
            int index = ClampIndex(workerLevel, s_workerDpsTable.Length);
            return s_workerDpsTable[index];
        }

        private static int ClampIndex(int level, int maxLength)
        {
            if (level < 0)
            {
                return 0;
            }

            if (level >= maxLength)
            {
                return maxLength - 1;
            }

            return level;
        }

        public static int GetMaxToolLevel()
        {
            return s_toolDamageTable.Length - 1;
        }

        public static int GetMaxWorkerLevel()
        {
            return s_workerDpsTable.Length - 1;
        }
    }
}
