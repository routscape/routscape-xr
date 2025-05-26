using UnityEngine;

public struct NewObjectData
{
    public string Name { get; set; }
    public int TypeID { get; set; }
}

public struct EditObjectData
{
    public int ObjectID { get; set; }
    public string Name { get; set; }
    public int TypeID { get; set; }
}

public struct RepositionObjectData
{
    public int ObjectID { get; set; }
}

public class SelectionService: MonoBehaviour
{
    public static NewObjectData NewMapObjectData;
    public static EditObjectData EditMapObjectData;
    public static RepositionObjectData RepositionMapObjectData; 
}
