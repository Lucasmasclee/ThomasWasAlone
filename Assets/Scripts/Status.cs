using UnityEngine;

/// <summary>
/// Class to manage some changes in the player status.
/// </summary>
public class Status
{
    /// <summary>
    /// The stage of splits this char will start (the main char has index 0, his childs 1, and so on)
    /// </summary>
    public int SplitIndex { get; private set; }
    public int SplitCount { get; private set; }
    public int SplitLimit { get; private set; }

    /// <summary>
    /// Limit base is needed since the chars may have different limits for each index
    /// </summary>
    private static int SplitLimitBase = 3;
    public static int SetSplitLimitBase(int value) => SplitLimitBase = value;
    public bool CanSplit => SplitLimit > SplitCount;

    public Status(int index)
    {
        SplitIndex = index;
        SplitCount = 0;
        SplitLimit = SplitLimitBase - index;
    }

    public void Split()
    {
        SplitCount++;
    }

    public void Merge()
    {
        SplitCount = Mathf.Clamp(SplitCount - 1, 0, SplitCount);
        SplitIndex--;

        /// Condition to check if the char is merging even without splits or merged to the same state
        if (SplitIndex < 0)
        {
            SplitIndex = 0;
            SplitLimit++;
        }
    }

}
