using System;
namespace DisjointSet
{
    public class DisjointSet
    {
        // this array holds all possible values that could be in this DisjointSet
        // if a value is negative, then it is a root and abs(value) is equal to the size of the current
        // if a value is positive, then it represents the parent of this value in the set
        private int[] arrayValues;

        public DisjointSet(int numElements)
        {
            arrayValues = new int[numElements];

            // use -1 as a placeholder value
            // an item with -1 as its value points to NULL
            for (int i = 0; i < arrayValues.Length; i++)
            {
                arrayValues[i] = -1;
            }
        }

        public int Find(int index)
        {
            // if the value is less than 0 then we just return it (we are at root)
            // otherwise, we recursively go up a level and return what that value is
            if (arrayValues[index] < 0)
            {
                return index;
            }
            else
            {
                // with recursion, all values will end up pointing to the root (path compression)
                arrayValues[index] = Find(arrayValues[index]);
                return arrayValues[index];
            }
        }

        public void Union(int firstIndex, int secondIndex)
        {
            int firstRootIndex = this.Find(firstIndex);
            int secondRootIndex = this.Find(secondIndex);

            // if the two roots are equal then they are already in the same set, so no union needed
            // otherwise, compare sizes of each tree and set smaller tree to point to larger tree
            if (firstRootIndex == secondRootIndex)
            {
                return;
            }

            // because set sizes are stored as negatives, a larger tree size will be mathematically smaller
            // than a smaller tree size
            if (arrayValues[firstRootIndex] < arrayValues[secondRootIndex])
            {
                arrayValues[firstRootIndex] += arrayValues[secondRootIndex];
                arrayValues[secondRootIndex] = firstRootIndex;
            }
            else
            {
                arrayValues[secondRootIndex] += arrayValues[firstRootIndex];
                arrayValues[firstRootIndex] = secondRootIndex;
            }
        }
    }
}
