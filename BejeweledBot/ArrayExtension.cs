namespace BejeweledBot
{
    public static class ArrExtension
    {
        public static int TryGetValue(this int[,] arr, int row, int col)
        {
            bool widthOK = false;
            bool heightOK = false;
            if (row >= 0 && row < arr.GetLength(0))
                widthOK = true;
            if (col >= 0 && col < arr.GetLength(1))
                heightOK = true;

            if (widthOK && heightOK)
                return arr[row, col];
            else
                return -1; //we won't have values less than zero in the board array
        }
    }
}
