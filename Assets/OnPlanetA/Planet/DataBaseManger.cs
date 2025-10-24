using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class DataBaseManger :ScriptableObject
{
    //name is your gameobject's tag,not class name or script name;
    //�����λ���Լ���Script��дһ����������Ӧ������������淽����������ˣ�
    //�����Tag�������Enermy"
   private static Dictionary<string,Vector3> enermyposition=new Dictionary<string,Vector3>();
   private static Dictionary<string, Vector3> buildingposition = new Dictionary<string, Vector3>();
   private static Dictionary<string,int> buildingenergy= new Dictionary<string,int>();
 
 
  public static void RegisterBuildingData(string name,Vector3 position,int energy)
    {
        
        buildingenergy.Add(name,energy);
        buildingposition.Add(name, position);

    }
  public static void RegisterEnermyData(string name,Vector3 position)
    {
        
        enermyposition.Add(name ,position);
    }
  public static void RemoveBuildingData(string key)
    {
        buildingposition.Remove(key);
        buildingenergy.Remove(key);
    }
  public static void RemoveEnermyData(string key)
    {
        buildingenergy.Remove(key);
    }
  public static Vector3 GetEnermyPosition(string key)
     {
          Vector3 value;
          if (enermyposition.TryGetValue(key,out value))
          {
              
                return value;
          }
          else
          {
                Debug.LogError("No such key in enermy position database");
                return Vector3.zero;
          }

    }
  public static int GetBuildingEnergy(string key)
    {
        int value;
        if (buildingenergy.TryGetValue(key,out value))
        {
           
            return value;
        }
        else
        {
            Debug.LogError("No such key in building energy database");
            return -1;
        }
        
    }
  public static Vector3 GetBuildingPosition(string key)
    {
        Vector3 value;
        if (buildingposition.TryGetValue(key,out value))
        {
           
            return value;
        }
        else
        {
            Debug.LogError("No such key in building position database");
            return Vector3.zero;
        }

    }
private static bool Find(string key,string str,int count1)
    {
        int count = count1;
        int count2 = 0;
        if (count1<key.Length-str.Length) {
            for (int i = count1; i < count1+str.Length; i++)
            {
                if (key[i] == str[i] && count2 == str.Length - 1)
                {
                    return true;
                }
                else
                {
                    if (count2 == str.Length - 1)
                    {
                        Find(key, str, count + 1);
                    }
                }
                count2++;
            }
        }
        else
        {
            return false;
        }
        return false;

    }
public static List<int> GetBuildingEnergys(string str)
    {
        List<int> energys = new List<int>();
        
        foreach(string keys in buildingenergy.Keys)
        {
            if (Find(keys,str,0))
            {
                energys.Add(buildingenergy[keys]);
            }
        }
        return energys;
    }
public static List<Vector3> GetEnermyPositions(string str)
    {
        List<Vector3> positions = new List<Vector3>();
        
        foreach(string keys in enermyposition.Keys)
        {
            if (Find(keys,str,0))
            {
                positions.Add(enermyposition[keys]);
            }
        }
        return positions;
    }
    public static List<Vector3> GetBuildingPositions(string str)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (string keys in buildingposition.Keys)
        {
            if (Find(keys, str, 0))
            {
                positions.Add(buildingposition[keys]);
            }
        }
        return positions;
    }

    public static void ClearAllData()
    {
        enermyposition.Clear();
        buildingposition.Clear();
        buildingenergy.Clear();
        
    }
    public static void SaveDataToJSONFile()
    {
        try
        {
            string savePath = Application.persistentDataPath + "GameData.json";
            string jsonData = JsonUtility.ToJson(new DataBaseManger());
            File.WriteAllText(savePath, jsonData);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save Data Failed:" + e.Message);
        }
    }
    public  static void LoadDataFromJSONFile()
    {
        try
        {
            string loadPath = Application.persistentDataPath + "GameData.json";
            if (File.Exists(loadPath))
            {
                string jsonData = File.ReadAllText(loadPath);
                JsonUtility.FromJsonOverwrite(jsonData, new DataBaseManger());
            }
            else
            {
                Debug.LogWarning("Load Data Failed: File does not exist.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load Data Failed:" + e.Message);
        }
    }
}


