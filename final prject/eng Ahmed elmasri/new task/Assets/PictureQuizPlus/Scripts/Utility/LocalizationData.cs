[System.Serializable]
public class LocalizationData //Serializable classes to store text game data in a separate file
{
    public TasksDictionary[] tasksData; //Stores tasks info
    public LocalizationItem[] gameItems; //Stores text for UI elements
    public char[] randomLetters; //Stores random letters to be implemented
}

[System.Serializable]
public class LocalizationItem
{
    public LocalizationItem(ElementType key, string value)
    {
        this.key = key;
        this.value = value;
    }
    public ElementType key;
    public string value;
}